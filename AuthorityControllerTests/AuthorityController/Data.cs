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
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AuthorityController.Data;
using System.Threading;

namespace AuthorityController.Tests
{
    [TestClass]
    public class Data
    {
        /// <summary>
        /// Set configs related to base password test.
        /// </summary>
        private void SetBaseDataConfig()
        {
            new Config()
            {
                PasswordRequireUpperSymbol = false,
                PasswordRequireDigitSymbol = false,
                PasswordRequireSpecialSymbol = false,
                PasswordMinAllowedLength = 5,
                PasswordMaxAllowedLength = 16
            };
        }

        /// <summary>
        /// Return unicue subfolder for the test.
        /// </summary>
        public string TestSubfolder
        {
            get
            {
                if (_testSubFolder == null)
                {
                    _testSubFolder = "Tests\\" + Guid.NewGuid().ToString();
                }
                return _testSubFolder;
            }
        }
        private string _testSubFolder;

        /// <summary>
        /// Directory of config file created at this session.
        /// </summary>
        public string CofigFilePath { get; set; }

        [TestInitialize]
        public void Setup()
        {
            // Open folder.
            Directory.CreateDirectory(TestSubfolder);
            System.Diagnostics.Process.Start(TestSubfolder);
        }

        [TestMethod]
        public void ConfigValidation()
        {
            ConfigCreate();
            ConfigLoad_ValidData();
            ConfigLoad_CoruptedData();
        }

        /// <summary>
        /// Create new config file.
        /// </summary>
        public void ConfigCreate()
        {
            // Set new directory.
            string directoryBufer = Config.DIRECTORY;
            Config.DIRECTORY = TestSubfolder + Config.DIRECTORY;

            // Init file.
            _ = Config.Active;

            // Make path to file.
            CofigFilePath = 
                Config.DIRECTORY + 
                Config.CONFIG_FILE_NAME;

            // Check existing.
            bool result = File.Exists(CofigFilePath);
            
            // Revert value.
            Config.DIRECTORY = directoryBufer;

            // Assert.
            Assert.IsTrue(result, "File creation failed.");
        }

        /// <summary>
        /// Try to load valid config data file.
        /// </summary>
        public void ConfigLoad_ValidData()
        {
            // Try to load config from directory.
            bool result = Config.TryToLoad<Config>(
                CofigFilePath, out Config _);

            Assert.IsTrue(result, "Loading failed.");
        }

        /// <summary>
        /// Trying to load corrupted config dile.
        /// </summary>
        public void ConfigLoad_CoruptedData()
        {
            string corruptedFileDirectory = CofigFilePath + ".invalid";

            // Copy valid file.
            File.Copy(CofigFilePath, corruptedFileDirectory);

            #region Damage file
            string[] lines = File.ReadAllLines(corruptedFileDirectory);
            // Add invalid entrance.
            for(int i = 0; i < lines.Length; i += 4)
            {
                lines[i] = lines[i] + "misclick";
            }

            // Add invalid tags.
            for (int i = 1; i < lines.Length; i += 4)
            {
                lines[i] = lines[i] + "<invalidSheme>";
            }

            // Swithc digits valut to string
            for (int i = 1; i < lines.Length; i ++)
            {
                // Get end of tag.
                int valueIndex = lines[i].IndexOf('>');
                // Skip if closed.
                if (valueIndex + 1 >= lines[i].Length)
                {
                    continue;
                }

                // Check if value is digit.
                bool isDigit = Int32.TryParse(lines[i][valueIndex + 1].ToString(), out int _);

                // Change digit to string.
                if (isDigit)
                {
                    lines[i] = lines[i].Remove(valueIndex + 1, 1);
                    lines[i] = lines[i].Insert(valueIndex + 1, "nonDigit");
                }
            }

            // Write corrupted lines.
            File.WriteAllLines(corruptedFileDirectory, lines);
            #endregion

            // Try to load config from directory.
            bool result = Config.TryToLoad<Config>(
            corruptedFileDirectory, out Config config);

            Assert.IsTrue(!result, "Corrupted file cause error.");
        }
        

        /// <summary>
        /// Stress test in working with huge data.
        /// </summary>
        [TestMethod]
        public void HugePoolValidation()
        {
            NewUsersPool();
            LoadProfilesPool();
        }

        /// <summary>
        /// Create huge pool of users.
        /// Use for stress tests.
        /// </summary>
        public void NewUsersPool()
        {
            bool poolFailed = false;
            int poolUsersCount = 50000;

            // Fail callback
            System.Action<User, string> FailHandler = null;
            FailHandler = (User obj, string error) =>
            {
                poolFailed = true;
                API.Users.UserProfileNotStored -= FailHandler;

                Assert.IsTrue(false, "Data storing failed.");
            };
            API.Users.UserProfileNotStored += FailHandler;


            for (int i = 0; i < poolUsersCount; i++)
            {
                // Create user.
                User user = new User()
                {
                    login = "user" + i
                };
                // Get GUID
                user.id = API.Users.GenerateID(user);

                // Save profile.
                API.Users.SetProfileAsync(user, TestSubfolder + "\\USERS\\");
            }

            // Wait until operation compleeting.
            while(!poolFailed)
            {
                if (Directory.GetFiles(TestSubfolder + "\\USERS\\").Length == poolUsersCount)
                {
                    break;
                }
                Thread.Sleep(50);
            }

            Assert.IsTrue(!poolFailed);
        }

        /// <summary>
        /// Try to load a huge pool of users. Some users can be corrupted.
        /// Need to finish operation without freezing of other threads and without crush due corupted data.
        /// </summary>
        public void LoadProfilesPool()
        {
            // Init
            string loadDirectory = TestSubfolder + "\\USERS\\";
            bool loaded = false;

            #region Test finish handler.
            // Wait until loading finish.
            System.Action<string, int, int> FinishHandler = null;
            FinishHandler = (string dir, int succeed, int failed) =>
            {
                // If currect directory
                if (dir.Equals(loadDirectory))
                {
                    // Unsubscribe
                    API.Users.DirectoryLoadingFinished -= FinishHandler;

                    // Infrom waitnig loop about finish.
                    loaded = true;

                    // Check is passed all?
                    int commonCountOfFiles = Directory.GetFiles(loadDirectory).Length;
                    Assert.IsTrue(commonCountOfFiles == succeed + failed, "No all files was processed");

                    // Check corrupted.
                    Assert.IsTrue(failed == 0, "Some files corrupted: " + failed);
                }
            };
            API.Users.DirectoryLoadingFinished += FinishHandler;
            #endregion

            API.Users.LoadProfilesAsync(loadDirectory);

            // Wait until load.
            while (!loaded)
            {
                Thread.Sleep(5);
            }
        }


        /// <summary>
        /// Validate full stack of fetaures related to user profile.
        /// </summary>
        [TestMethod]
        public void UserProfileValidation()
        {
            User testUser = NewUser();
            UpdateUser(testUser);
            RemoveUser(testUser);
        }
        
        /// <summary>
        /// Creating new user profile and saving to storage.
        /// </summary>
        public User NewUser()
        {
            // Create user.
            User testUser = new User()
            {
                login = "userLogin"
            };
            // Get GUID
            testUser.id = API.Users.GenerateID(testUser);

            // Save profile.
            API.Users.SetProfileAsync(testUser, TestSubfolder + "\\USERS\\TEMP\\");
            bool userTestPaused = true;

            #region Wait for result
            // Failed
            API.Users.UserProfileNotStored += (User u, string error) =>
            {
                if (u.Equals(testUser))
                {
                    userTestPaused = false;
                    Assert.IsTrue(false, error);
                }
            };

            // Stored
            API.Users.UserProfileStored += (User u) =>
            {
                if (u.Equals(testUser))
                {
                    userTestPaused = false;
                    Assert.IsTrue(true);
                }
            };
            #endregion

            // Wait untol complete.
            while(userTestPaused)
            {
                Thread.Sleep(50);
            }

            return testUser;
        }

        /// <summary>
        /// Update already existed profile.
        /// </summary>
        public void UpdateUser(User testUser)
        {
            testUser.firstName = "Updated";
            testUser.secondName = "Updated";

            // Save profile.
            API.Users.SetProfileAsync(testUser, TestSubfolder + "\\USERS\\TEMP\\");

            bool userTestPaused = true;

            #region Wait for result
            // Failed
            API.Users.UserProfileNotStored += (User u, string error) =>
            {
                if (u.Equals(testUser))
                {
                    userTestPaused = false;
                    Assert.IsTrue(false, error);
                }
            };

            // Stored
            API.Users.UserProfileStored += (User u) =>
            {
                if (u.Equals(testUser))
                {
                    userTestPaused = false;
                    Assert.IsTrue(true);
                }
            };
            #endregion

            // Wait untol complete.
            while (userTestPaused)
            {
                Thread.Sleep(50);
            }
        }

        /// <summary>
        /// Remove profile from storage.
        /// </summary>
        public void RemoveUser(User testUser)
        {
            Assert.IsTrue(API.Users.RemoveProfile(testUser, TestSubfolder + "\\USERS\\TEMP\\"));
        }

        
        /// <summary>
        /// Creating a new salt data.
        /// </summary>
        [TestMethod]
        public void SaltInit()
        {
        }

        /// <summary>
        /// Validate loading of already created salt
        /// </summary>
        [TestMethod]
        public void SaltLoading()
        {
        }
    }
}
