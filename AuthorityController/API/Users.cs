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
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using AuthorityController.Data;

namespace AuthorityController.API
{
    /// <summary>
    /// API that provide operation with authority data.
    /// </summary>
    public static class Users
    {
        #region Events
        /// <summary>
        /// Event that will be called when loading of users from directory will be finished.
        /// </summary>
        public static event System.Action<string> DirectoryLoadingUnlocked;
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


        #region Data
        /// <summary>
        /// Loading users data from directory.
        /// </summary>
        /// <param name="directory"></param>
        public static async void LoadProfilesAsync(string directory)
        {
            // Validate directory.
            if(!Directory.Exists(directory))
            {
                Console.WriteLine("ERROR (ACAPI 0): USERS LOADING NOT POSSIBLE. DIRECTORY NOT FOUND.");
                return;
            }

            // Block if certain directory already in loading process.
            if(LoadingLockedDirectories.Contains(directory))
            {
                Console.WriteLine("ERROR (ACAPI 10): Directory alredy has active loading process. Wait until finish previous one. ({0})",
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
                            Console.WriteLine("ERROR(ACAPI 20): Profile damaged. Reason:\n{0}\n", ex.Message);
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
        /// Adding\updating user's profile by directory sete up via config file.
        /// </summary>
        /// <param name="user"></param>
        public static void SetProfile(User user)
        {
            SetProfile(user, Config.Active.UsersStorageDirectory);
        }

        /// <summary>
        /// Adding\updating user's profile by directory.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="directory"></param>
        public static void SetProfile(User user, string directory)
        {
            // Check directory exist.
            if (!Directory.Exists(directory))
            {
                // Create if not found.
                Directory.CreateDirectory(directory);
            }

            // Convert user to XML file.
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                XmlSerializer serializer = new XmlSerializer(typeof(User));
                using (MemoryStream stream = new MemoryStream())
                {
                    serializer.Serialize(stream, user);
                    stream.Position = 0;
                    xmlDocument.Load(stream);
                    xmlDocument.Save(directory + GetUserFileName(user));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR(ACAPI 30):  Not serialized. Reason:\n{0}", ex.Message);
            }
        }

        /// <summary>
        /// Remove user profile from directory seted up via Config file.
        /// </summary>
        /// <param name="user"></param>
        public static void RemoveProfile(User user)
        {
            RemoveProfile(user, Config.Active.UsersStorageDirectory);
        }

        /// <summary>
        /// Remove user profile from directory.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="directory"></param>
        public static void RemoveProfile(User user, string directory)
        {
            // Expire user sessions.
            foreach (string token in user.tokens)
            {
                Session.Current.SetExpired(token);
            }

            // Remove profile.
            try
            {
                File.Delete(directory + GetUserFileName(user));
            }
            catch(Exception ex)
            {
                Console.WriteLine("ERROR(ACAPI 60):  Prifile removing failed. Reason:\n{0}", ex.Message);
            }
        }

        /// <summary>
        /// Looking for free id.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static int GenerateID(User user)
        {
            // Generate ID by hash code.
            int id = user.login.GetHashCode();

            // If already exist.
            if (TryToFindUser(id, out User _))
            {
                // Increment id until finde free one.
                do
                {
                    id++;
                }
                while (TryToFindUser(id, out User _));
            }

            return id;
        }
        #endregion

        #region Cash
        /// <summary>
        /// Remove all loaded users data.
        /// </summary>
        public static void ClearUsersLoadedData()
        {
            UsersById.Clear();
            UsersByLogin.Clear();
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
            if (UsersById[id] is User bufer)
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

        /// <summary>
        /// Seeking for user.
        /// </summary>
        /// <param name="uniformValue">ID or login in string format.</param>
        /// <param name="userProfile">Field that will contain user's profile in case of found.</param>
        /// <param name="error">Error that describe a reasone of fail. Could be send backward to client.</param>
        /// <returns></returns>
        public static bool TryToFindUserUniform(string uniformValue, out User userProfile, out string error)
        {
            // Initialize outputs.
            userProfile = null;
            error = null;
            // Seeking marker.
            bool userFound = false;

            // Try to parse id from query.
            if (Int32.TryParse(uniformValue, out int userId))
            {
                // Try to find user by id.
                if (API.Users.TryToFindUser(userId, out userProfile))
                {
                    userFound = true;
                }
            }

            // if user not found by ID.
            if (!userFound)
            {
                // Try to find user by login.
                if (!API.Users.TryToFindUser(uniformValue, out userProfile))
                {
                    // If also not found.
                    error = "ERROR 404: User not found";
                    return false;
                }
            }

            return true;
        }
        #endregion

        #region Security
        /// <summary>
        /// Convert password to heshed and salted.
        /// </summary>
        /// <param name="input">Password recived from user.</param>
        /// <returns></returns>
        public static byte[] GetHashedPassword(string input)
        {
            // Get recived password to byte array.
            byte[] plainText = Encoding.UTF8.GetBytes(input);

            // Create hash profider.
            HashAlgorithm algorithm = new SHA256Managed();

            // Allocate result array.
            byte[] plainTextWithSaltBytes =
              new byte[plainText.Length + Config.Active.Salt.Length];

            // Copy input to result array.
            for (int i = 0; i < plainText.Length; i++)
            {
                plainTextWithSaltBytes[i] = plainText[i];
            }

            // Add salt to array.
            for (int i = 0; i < Config.Active.Salt.Length; i++)
            {
                plainTextWithSaltBytes[plainText.Length + i] = Config.Active.Salt[i];
            }

            // Get hash of salted array.
            return algorithm.ComputeHash(plainTextWithSaltBytes);
        }

        /// <summary>
        /// Check permition for action.
        /// </summary>
        /// <param name="user">Target user.</param>
        /// <param name="rightCode">Code of right that required for action.</param>
        /// <returns></returns>
        public static bool IsBanned(User user, string rightCode)
        {
            // Check every ban.
            for(int i = 0; i < user.bans.Count; i++)
            {
                // Get ban data.
                BanInformation banInformation = user.bans[i];

                // Skip if ban expired.
                if (!banInformation.active)
                    continue;

                // Validate ban and disable it if already expired.
                if (banInformation.IsExpired)
                {
                    // Disactivate ban.
                    banInformation.active = false;

                    // Update profile.
                    API.Users.SetProfile(user, Config.Active.UsersStorageDirectory);
                }

                // Check every baned right.
                foreach(string blockedRights in banInformation.blockedRights)
                {
                    // Compare rights codes.
                    if(blockedRights == rightCode)
                    {
                        // Confirm band if equal.
                        return true;
                    }
                }
            }

            // ban not found.
            return false;
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Return unified name based on user's profile.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private static string GetUserFileName(User user)
        {
            if (user == null)
            {
                Console.WriteLine("ERROR(ACAPI 40): User can't be null");
                throw new NullReferenceException();
            }

            // Get user ID in string format.
            string name = user.id.ToString() + ".xml";
            return name;
        }
        #endregion
    }
}
