//Copyright 2019 Volodymyr Podshyvalov
//
//Licensed under the Apache License, Version 2.0 (the "License");
//you may not use this file except in compliance with the License.
//You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//Unless required by applicable law or agreed to in writing, software
//distributed under the License is distributed on an "AS IS" BASIS,
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//See the License for the specific language governing permissions and
//limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Threading;
using System.Text;
using System.Threading.Tasks;
using AuthorityController.Data;

namespace AuthorityController
{
    /// <summary>
    /// API that provide operation with authority data.
    /// </summary>
    public static class API
    {
        #region Events
        /// <summary>
        /// Event that will be called when loading of users from directory will be finished.
        /// </summary>
        public static event System.Action<string> DirectoryLoadingUnlocked;
        #endregion

        #region Configs
        /// <summary>
        /// How many minutes token is valid.
        /// </summary>
        public static int tokenValidTimeMinutes = 1440;
        #endregion

        #region Public properties
        /// <summary>
        /// Return free token.
        /// </summary>
        public static string UnusedToken
        {
            get
            {
                // Get current time.
                byte[] time = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
                // Generate id.
                byte[] key = Guid.NewGuid().ToByteArray();
                // Create token.
                string token = Convert.ToBase64String(time.Concat(key).ToArray());

                return token;
            }
        }
        #endregion
        
        #region Private fields
        /// <summary>
        /// Table that provide aaccess to user data by login.
        /// </summary>
        private static readonly Hashtable UsersByLogin = new Hashtable();

        /// <summary>
        /// Table that provide access to user by unique ID.
        /// </summary>
        private static readonly Hashtable UsersById = new Hashtable();

        /// <summary>
        /// Contains directories that has users loading process and blocked for new ones.
        /// </summary>
        private static readonly HashSet<string> LoadingLockedDirectories = new HashSet<string>();
        #endregion


        #region Users API
        /// <summary>
        /// Loading users data from directory.
        /// </summary>
        /// <param name="directory"></param>
        public static async void LoadUsersAsync(string directory)
        {
            // Validate directory.
            if(!Directory.Exists(directory))
            {
                Console.WriteLine("ERROR (ACAPI0): USERS LOADING NOT POSSIBLE. DIRECTORY NOT FOUND.");
                return;
            }

            // Block if certain directory already in loading process.
            if(LoadingLockedDirectories.Contains(directory))
            {
                Console.WriteLine("ERROR (ACAPI1): Directory alredy has active loading process. Wait until finish previous one. ({0})",
                    directory);
                return;
            }

            // Detect files in provided directory.
            string[] xmlFiles = Directory.GetFiles(directory, "*.xml", SearchOption.TopDirectoryOnly);
            
            // Running async task with loading of every profile.
            await Task.Run(() => 
            {
                // Init encoder.
                XmlSerializer xmlSer = new XmlSerializer(typeof(User));

                // Deserialize every file to table if possible.
                foreach (string fileDir in xmlFiles)
                {
                    // Open stream to XML file.
                    using (FileStream fs = new FileStream(fileDir, FileMode.Open))
                    {
                        User loadedUser = null;
                        try
                        {
                            // Try to deserialize routing table from file.
                            loadedUser = xmlSer.Deserialize(fs) as User;

                            #region Add user to ids table.
                            if(UsersById[loadedUser.id] is User idU)
                            {
                                // Override if already exist.
                                UsersById[loadedUser.id] = loadedUser;
                            }
                            else
                            {
                                // Add as new.
                                UsersById.Add(loadedUser.id, loadedUser);
                            }
                            #endregion

                            #region Add user to logins table.
                            if (UsersByLogin[loadedUser.login] is User loginU)
                            {
                                // Override if already exist.
                                UsersByLogin[loadedUser.login] = loadedUser;
                            }
                            else
                            {
                                // Add as new.
                                UsersByLogin.Add(loadedUser.login, loadedUser);
                            }
                            #endregion
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("ERROR(ACAPIw): Profile damaged. Reason:\n{0}\n", ex.Message);
                        }

                    }
                }

                // Remove directory from blocklist.
                LoadingLockedDirectories.Remove(directory);

                // Inform subscribers about location unlock.
                DirectoryLoadingUnlocked?.Invoke(directory);
            });
        }

        /// <summary>
        /// Remove al loaded users data.
        /// </summary>
        public static void ClearUsersData()
        {
            UsersById.Clear();
            UsersByLogin.Clear();
        }

        /// <summary>
        /// Adding or update user profile by directory.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="directory"></param>
        public static void SetUser(User user, string directory)
        {
            //TODO
        }

        /// <summary>
        /// Remove user profile from directory.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="directory"></param>
        public static void RemoveUser(User user, string directory)
        {
            //TODO
        }

        /// <summary>
        /// Try to find user by ID in loaded users table.
        /// </summary>
        /// <param name="id">Unique user's id.</param>
        /// <param name="user">Reference on loaded user profile.</param>
        /// <returns>Result of operation.</returns>
        public static bool TryToFindUser(int id, out User user)
        {
            // Try to find user in table.
            if(UsersById[id] is User bufer)
            {
                user = bufer;
                return true;
            }

            // Inform about fail.
            user = null;
            return false;
        }

        /// <summary>
        /// Try to find user by ID in loaded users table.
        /// </summary>
        /// <param name="login">Unique user's login.</param>
        /// <param name="user">Reference on loaded user profile.</param>
        /// <returns>Result of operation.</returns>
        public static bool TryToFindUser(string login, out User user)
        {
            // Try to find user in table.
            if (UsersByLogin[login] is User bufer)
            {
                user = bufer;
                return true;
            }

            // Inform about fail.
            user = null;
            return false;
        }
        #endregion

        #region Tokens API
        /// <summary>
        /// Check if token expired based on encoded token data.
        /// Use it on Queries Server to avoid additive time spending on data servers and unnecessary connections.
        /// 
        /// If token have hacked allocate date this just will lead to passing of this check.
        /// Server wouldn't has has token so sequrity will not be passed.
        /// Also server will control expire time by him self.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static bool IsTokenExpired(string token)
        {
            // Convert token to bytes array.
            byte[] data = Convert.FromBase64String(token);

            // Get when token created. Date time will take the first bytes that contain data stamp.
            DateTime when = DateTime.FromBinary(BitConverter.ToInt64(data, 0));

            // Compare with allowed token time.
            if (when < DateTime.UtcNow.AddMinutes(-tokenValidTimeMinutes))
            {
                // Confirm expiration.
                return true;
            }

            // Conclude that token is valid.
            return false;
        }
        #endregion
    }
}
