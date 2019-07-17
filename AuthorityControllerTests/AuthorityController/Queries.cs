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
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AuthorityController.Data;

namespace AuthorityController.Tests
{
    [TestClass]
    public class Queries
    {
        /// <summary>
        /// Name  of the pipe that would be started on server during tests.
        /// </summary>
        string PIPE_NAME = "ACTestPublic";

        #region Users
        User user_SuperAdmin = null;
        User user_Admin = null;
        User user_Moderator = null;
        User user_PrivilegedUser = null;
        User user_User = null;
        User user_Guest = null;
        #endregion

        /// <summary>
        /// Creating and apply base users pool:
        /// -Super admin
        /// -Admin
        /// -Moderator
        /// -Privileged user
        /// -User
        /// -Guest
        /// </summary>
        public void SetBaseUsersPool()
        {
            lock (Locks.CONFIG_LOCK)
            {
                // Set new test directory to avoid conflicts with users profiles.
                Config.Active.UsersStorageDirectory = "Tests\\Queries\\Users\\" + Guid.NewGuid().ToString() + "\\";

                // Clear current user pool.
                AuthorityController.API.Users.ClearUsersLoadedData();

                #region Create superadmin
                user_SuperAdmin = new User()
                {
                    id = 1,
                    login = "sadmin",
                    password = API.Users.GetHashedPassword("password", Config.Active.Salt),
                    tokens = new System.Collections.Generic.List<string>
                    (new string[] { AuthorityController.API.Tokens.UnusedToken }),
                    rights = new string[]{
                    "rank=16",
                    "bannhammer",
                    "passwordManaging" }
                };

                // Generate ID.
                user_SuperAdmin.id = API.Users.GenerateID(user_SuperAdmin);

                // Save profile.
                AuthorityController.API.Users.SetProfileAsync(user_SuperAdmin, Config.Active.UsersStorageDirectory);

                #endregion

                #region Create admin
                user_Admin = new User()
                {
                    login = "admin",
                    password = API.Users.GetHashedPassword("password", Config.Active.Salt),
                    tokens = new System.Collections.Generic.List<string>
                    (new string[] { AuthorityController.API.Tokens.UnusedToken }),
                    rights = new string[]{
                    "rank=8",
                    "bannhammer",
                    "passwordManaging" }
                };

                // Generate ID.
                user_Admin.id = API.Users.GenerateID(user_Admin);

                // Save profile.
                AuthorityController.API.Users.SetProfileAsync(user_Admin, Config.Active.UsersStorageDirectory);
                #endregion

                #region Create moderator
                user_Moderator = new User()
                {
                    login = "moderator",
                    password = API.Users.GetHashedPassword("password", Config.Active.Salt),
                    tokens = new System.Collections.Generic.List<string>
                    (new string[] { AuthorityController.API.Tokens.UnusedToken }),
                    rights = new string[]{
                    "rank=4",
                    "bannhammer",
                    "passwordManaging" }
                };

                // Generate ID.
                user_Moderator.id = API.Users.GenerateID(user_Moderator);

                // Save profile.
                AuthorityController.API.Users.SetProfileAsync(user_Moderator, Config.Active.UsersStorageDirectory);
                #endregion

                #region Create privileged user
                user_PrivilegedUser = new User()
                {
                    login = "puser",
                    password = API.Users.GetHashedPassword("password", Config.Active.Salt),
                    tokens = new System.Collections.Generic.List<string>
                    (new string[] { AuthorityController.API.Tokens.UnusedToken }),
                    rights = new string[]{
                    "rank=2",
                    "passwordManaging" }
                };

                // Generate ID.
                user_PrivilegedUser.id = API.Users.GenerateID(user_PrivilegedUser);

                // Save profile.
                AuthorityController.API.Users.SetProfileAsync(user_PrivilegedUser, Config.Active.UsersStorageDirectory);
                #endregion

                #region Create user
                user_User = new User()
                {
                    login = "user",
                    password = API.Users.GetHashedPassword("password", Config.Active.Salt),
                    tokens = new System.Collections.Generic.List<string>
                    (new string[] { AuthorityController.API.Tokens.UnusedToken }),
                    rights = new string[]{
                    "rank=1",
                    "passwordManaging" }
                };

                // Generate ID.
                user_User.id = API.Users.GenerateID(user_User);

                // Save profile.
                AuthorityController.API.Users.SetProfileAsync(user_User, Config.Active.UsersStorageDirectory);
                #endregion

                #region Create guest
                user_Guest = new User()
                {
                    login = "guest",
                    password = API.Users.GetHashedPassword("password", Config.Active.Salt),
                    tokens = new System.Collections.Generic.List<string>
                    (new string[] { AuthorityController.API.Tokens.UnusedToken }),
                    rights = new string[]{
                    "rank=0"}
                };

                // Generate ID.
                user_Guest.id = API.Users.GenerateID(user_Guest);

                // Save profile.
                AuthorityController.API.Users.SetProfileAsync(user_Guest, Config.Active.UsersStorageDirectory);
                #endregion

                // Wait until loading.
                while (API.Users.HasAsyncLoadings)
                {
                    Thread.Sleep(5);
                }

                #region Authorize tokens
                // Super admin
                AuthorityController.Session.Current.AsignTokenToUser(user_SuperAdmin, user_SuperAdmin.tokens[0]);
                AuthorityController.Session.Current.SetTokenRights(user_SuperAdmin.tokens[0], user_SuperAdmin.rights);

                // Admin
                AuthorityController.Session.Current.AsignTokenToUser(user_Admin, user_Admin.tokens[0]);
                AuthorityController.Session.Current.SetTokenRights(user_Admin.tokens[0], user_Admin.rights);

                // Moderator
                AuthorityController.Session.Current.AsignTokenToUser(user_Moderator, user_Moderator.tokens[0]);
                AuthorityController.Session.Current.SetTokenRights(user_Moderator.tokens[0], user_Moderator.rights);

                // Privileged user
                AuthorityController.Session.Current.AsignTokenToUser(user_PrivilegedUser, user_PrivilegedUser.tokens[0]);
                AuthorityController.Session.Current.SetTokenRights(user_PrivilegedUser.tokens[0], user_PrivilegedUser.rights);

                // User
                AuthorityController.Session.Current.AsignTokenToUser(user_User, user_User.tokens[0]);
                AuthorityController.Session.Current.SetTokenRights(user_User.tokens[0], user_User.rights);

                // Guest
                AuthorityController.Session.Current.AsignTokenToUser(user_Guest, user_Guest.tokens[0]);
                AuthorityController.Session.Current.SetTokenRights(user_Guest.tokens[0], user_Guest.rights);
                #endregion

                // Stop previos servers.
                PipesProvider.Server.ServerAPI.StopAllServers();

                // Start new server pipe.
                AuthorityTestServer.Server.StartQueryProcessing(PIPE_NAME);
            }
        }

        /// <summary>
        /// Trying to get guest token using query.
        /// </summary>
        [TestMethod]
        public void GetGuestToken()
        {
        }

        /// <summary>
        /// Trying to update token rights without authority.
        /// </summary>
        [TestMethod]
        public void SetTokenRights_NoRights()
        {
            lock (Locks.CONFIG_LOCK)
            {
                // Create users for test.
                SetBaseUsersPool();
            }
        }

        /// <summary>
        /// Trying to update token rights with walid permition.
        /// </summary>
        [TestMethod]
        public void SetTokenRights_HasRights()
        {
            lock (Locks.CONFIG_LOCK)
            {
                // Create users for test.
                SetBaseUsersPool();
            }
        }

        /// <summary>
        /// Try to ban user but has no rights to this.
        /// </summary>
        [TestMethod]
        public void UserBan_NoRights()
        {
            lock (Locks.CONFIG_LOCK)
            {
                // Create users for test.
                SetBaseUsersPool();
            }
        }

        /// <summary>
        /// Try to ban user that has a higher rank than requester.
        /// </summary>
        [TestMethod]
        public void UserBan_HighrankerBan()
        {
            lock (Locks.CONFIG_LOCK)
            {
                // Create users for test.
                SetBaseUsersPool();
            }
        }

        /// <summary>
        /// Try to ban user with enough rights to that.
        /// </summary>
        [TestMethod]
        public void UserBan_HasRights()
        {
            lock (Locks.CONFIG_LOCK)
            {
                // Create users for test.
                SetBaseUsersPool();
            }
        }

        /// <summary>
        /// Try to logon as existed user with corerct logon data.
        /// </summary>
        [TestMethod]
        public void Logon_ValidData()
        {
            lock (Locks.CONFIG_LOCK)
            {
                // Create users for test.
                SetBaseUsersPool();

                // Create the query that would simulate logon.
                UniformQueries.QueryPart[] query = new UniformQueries.QueryPart[]
                {
                    new UniformQueries.QueryPart("token", AuthorityController.API.Tokens.UnusedToken),
                    new UniformQueries.QueryPart("guid", AuthorityController.API.Tokens.UnusedToken),

                    new UniformQueries.QueryPart("user", null),
                    new UniformQueries.QueryPart("logon", null),

                    new UniformQueries.QueryPart("login", "sadmin"),
                    new UniformQueries.QueryPart("password", "password"),
                    new UniformQueries.QueryPart("os", Environment.OSVersion.VersionString),
                    new UniformQueries.QueryPart("mac", "anonymous"),
                    new UniformQueries.QueryPart("stamp", DateTime.Now.ToBinary().ToString()),
                };

                // Marker that avoid finishing of the test until receiving result.
                bool operationCompete = false;

                // Start reciving clent line.
                UniformClient.BaseClient.EnqueueDuplexQuery(

                    // Request connection to localhost server via main pipe.
                    "localhost", PIPE_NAME,

                    // Convert query parts array to string view in correct format provided by UniformQueries API.
                    UniformQueries.QueryPart.QueryPartsArrayToString(query),

                    // Handler that would recive ther ver answer.
                    (PipesProvider.Client.TransmissionLine line, object answer) =>
                    {
                        // Try to convert answer to string
                        if (answer is string answerS)
                        {
                        // Is operation success?
                        if (answerS.StartsWith("error", StringComparison.OrdinalIgnoreCase))
                            {
                            // Log error.
                            Assert.Fail("Recived error:\n" + answerS);
                                operationCompete = true;
                            }
                            else
                            {
                            // Try to get toekn from answer.
                            if (UniformQueries.API.TryGetParamValue("token", out string value, answerS))
                                {
                                // Confirm logon.
                                Assert.IsTrue(true);
                                    operationCompete = true;
                                }
                                else
                                {
                                // Log error.
                                Assert.Fail("Answer not contain token:\nFull answer:" + answerS);
                                    operationCompete = true;
                                }
                            }
                        }
                        else
                        {
                        // Assert error.
                        Assert.Fail("Incorrect format of answer. Required format is string. Type:" + answer.GetType());
                            operationCompete = true;
                        }
                    });

                // Wait until operation would complete.
                while (!operationCompete)
                {
                    Thread.Sleep(5);
                }
            }
        }


        /// <summary>
        /// Try to logon as existed user that not registred.
        /// </summary>
        [TestMethod]
        public void Logon_UserNotExist()
        {
            lock (Locks.CONFIG_LOCK)
            {
                // Create users for test.
                SetBaseUsersPool();

                // Create the query that would simulate logon.
                UniformQueries.QueryPart[] query = new UniformQueries.QueryPart[]
                {
                    new UniformQueries.QueryPart("token", AuthorityController.API.Tokens.UnusedToken),
                    new UniformQueries.QueryPart("guid", AuthorityController.API.Tokens.UnusedToken),

                    new UniformQueries.QueryPart("user", null),
                    new UniformQueries.QueryPart("logon", null),

                    new UniformQueries.QueryPart("login", "notExistedUser"),
                    new UniformQueries.QueryPart("password", "password"),
                    new UniformQueries.QueryPart("os", Environment.OSVersion.VersionString),
                    new UniformQueries.QueryPart("mac", "anonymous"),
                    new UniformQueries.QueryPart("stamp", DateTime.Now.ToBinary().ToString()),
                };

                // Marker that avoid finishing of the test until receiving result.
                bool operationCompete = false;

                // Start reciving clent line.
                UniformClient.BaseClient.EnqueueDuplexQuery(

                    // Request connection to localhost server via main pipe.
                    "localhost", PIPE_NAME,

                    // Convert query parts array to string view in correct format provided by UniformQueries API.
                    UniformQueries.QueryPart.QueryPartsArrayToString(query),

                    // Handler that would recive ther ver answer.
                    (PipesProvider.Client.TransmissionLine line, object answer) =>
                    {
                        // Try to convert answer to string
                        if (answer is string answerS)
                        {
                            // Is operation success?
                            if (answerS.StartsWith("error", StringComparison.OrdinalIgnoreCase))
                            {
                                // Log error.
                                Assert.IsTrue(true);
                                operationCompete = true;
                            }
                            else
                            {
                                // Log error.
                                Assert.Fail("Unexisted user found on server.\nAnswer:" + answerS);
                                operationCompete = true;
                            }
                        }
                        else
                        {
                            // Assert error.
                            Assert.Fail("Incorrect format of answer. Required format is string. Type:" + answer.GetType());
                            operationCompete = true;
                        }
                    });

                // Wait until operation would complete.
                while (!operationCompete)
                {
                    Thread.Sleep(5);
                }
            }
        }

        /// <summary>
        /// Try to logon with incorrrect password.
        /// </summary>
        [TestMethod]
        public void Logon_InvalidData()
        {
            lock (Locks.CONFIG_LOCK)
            {
                // Create users for test.
                SetBaseUsersPool();

                // Create the query that would simulate logon.
                UniformQueries.QueryPart[] query = new UniformQueries.QueryPart[]
                {
                    new UniformQueries.QueryPart("token", AuthorityController.API.Tokens.UnusedToken),
                    new UniformQueries.QueryPart("guid", AuthorityController.API.Tokens.UnusedToken),

                    new UniformQueries.QueryPart("user", null),
                    new UniformQueries.QueryPart("logon", null),

                    new UniformQueries.QueryPart("login", "user"),
                    new UniformQueries.QueryPart("password", "invalidPassword"),
                    new UniformQueries.QueryPart("os", Environment.OSVersion.VersionString),
                    new UniformQueries.QueryPart("mac", "anonymous"),
                    new UniformQueries.QueryPart("stamp", DateTime.Now.ToBinary().ToString()),
                };

                // Marker that avoid finishing of the test until receiving result.
                bool operationCompete = false;

                // Start reciving clent line.
                UniformClient.BaseClient.EnqueueDuplexQuery(

                    // Request connection to localhost server via main pipe.
                    "localhost", PIPE_NAME,

                    // Convert query parts array to string view in correct format provided by UniformQueries API.
                    UniformQueries.QueryPart.QueryPartsArrayToString(query),

                    // Handler that would recive ther ver answer.
                    (PipesProvider.Client.TransmissionLine line, object answer) =>
                    {
                        // Try to convert answer to string
                        if (answer is string answerS)
                        {
                            // Is operation success?
                            if (answerS.StartsWith("error", StringComparison.OrdinalIgnoreCase))
                            {
                                // Log error.
                                Assert.IsTrue(true);
                                operationCompete = true;
                            }
                            else
                            {
                                // Log error.
                                Assert.Fail("Unexisted user found on server.\nAnswer:" + answerS);
                                operationCompete = true;
                            }
                        }
                        else
                        {
                            // Assert error.
                            Assert.Fail("Incorrect format of answer. Required format is string. Type:" + answer.GetType());
                            operationCompete = true;
                        }
                    });

                // Wait until operation would complete.
                while (!operationCompete)
                {
                    Thread.Sleep(5);
                }
            }
        }

        /// <summary>
        /// Try to logoff user by invalid token.
        /// </summary>
        [TestMethod]
        public void Logoff_InvalidToken()
        {
            lock (Locks.CONFIG_LOCK)
            {
                // Create users for test.
                SetBaseUsersPool();

                // Generate new token.
                string newToken = AuthorityController.API.Tokens.UnusedToken;

                // Logoff unregistred token.
                bool result = AuthorityController.Queries.USER_LOGOFF.LogoffToken(newToken);

                // Assert that token was rejected by a system.
                // If token was processed that this mean that system failed.
                Assert.IsTrue(!result, "Token detected, that can't be true.");
            }
        }

        /// <summary>
        /// Try to logoff user by valid token.
        /// </summary>
        [TestMethod]
        public void Logoff_ValidToken()
        {
            lock (Locks.CONFIG_LOCK)
            {
                // Create users for test.
                SetBaseUsersPool();

                // Get token of registred user.
                string userToken = user_User.tokens[0];

                // Logoff unregistred token.
                bool result = AuthorityController.Queries.USER_LOGOFF.LogoffToken(userToken);

                // Assert that token was rejected by a system.
                // If token was processed that this mean that system failed.
                Assert.IsTrue(result, "Token not detected.");
            }
        }
        
        /// <summary>
        /// Try to create user with valid data.
        /// </summary>
        [TestMethod]
        public void NewUser_ValidData()
        {
        }

        /// <summary>
        /// Try to logoff invalid token.
        /// </summary>
        [TestMethod]
        public void NewUser_InvalidPassword()
        {
        }

        /// <summary>
        /// Try to create new user with invalid login.
        /// </summary>
        [TestMethod]
        public void NewUser_InvalidLogin()
        {
        }

        /// <summary>
        /// Try to create user with invalid personal data.
        /// </summary>
        [TestMethod]
        public void NewUser_InvalidPersonal()
        {
        }
        
        /// <summary>
        /// Try to change self passoword.
        /// </summary>
        [TestMethod]
        public void NewPasswrod_Self()
        {
            lock (Locks.CONFIG_LOCK)
            {
                // Create users for test.
                SetBaseUsersPool();

                // Create the query that would simulate logon.
                UniformQueries.QueryPart[] query = new UniformQueries.QueryPart[]
                {
                    new UniformQueries.QueryPart("token", user_User.tokens[0]),
                    new UniformQueries.QueryPart("guid", AuthorityController.API.Tokens.UnusedToken),

                    new UniformQueries.QueryPart("user=" + user_User.id, null),
                    new UniformQueries.QueryPart("new", null),

                    new UniformQueries.QueryPart("password", "newPassword!2"),
                    new UniformQueries.QueryPart("oldpassword", "password"),
                    new UniformQueries.QueryPart("os", Environment.OSVersion.VersionString),
                    new UniformQueries.QueryPart("mac", "anonymous"),
                    new UniformQueries.QueryPart("stamp", DateTime.Now.ToBinary().ToString()),
                };

                // Marker that avoid finishing of the test until receiving result.
                bool operationCompete = false;

                // Start reciving clent line.
                UniformClient.BaseClient.EnqueueDuplexQuery(

                    // Request connection to localhost server via main pipe.
                    "localhost", PIPE_NAME,

                    // Convert query parts array to string view in correct format provided by UniformQueries API.
                    UniformQueries.QueryPart.QueryPartsArrayToString(query),

                    // Handler that would recive ther ver answer.
                    (PipesProvider.Client.TransmissionLine line, object answer) =>
                    {
                        // Try to convert answer to string
                        if (answer is string answerS)
                        {
                            // Is operation success?
                            if (!answerS.StartsWith("error", StringComparison.OrdinalIgnoreCase))
                            {
                                // Log error.
                                Assert.IsTrue(true);
                                operationCompete = true;
                            }
                            else
                            {
                                // Log error.
                                Assert.Fail("Recived error: " + answerS);
                                operationCompete = true;
                            }
                        }
                        else
                        {
                            // Assert error.
                            Assert.Fail("Incorrect format of answer. Required format is string. Type:" + answer.GetType());
                            operationCompete = true;
                        }
                    });

                // Wait until operation would complete.
                while (!operationCompete)
                {
                    Thread.Sleep(5);
                }

                Thread.Sleep(200);
            }
        }

        /// <summary>
        /// Try to change passoword of user with lowes rank.
        /// </summary>
        [TestMethod]
        public void NewPasswrod_LowerRankUser()
        {
            lock (Locks.CONFIG_LOCK)
            {
                // Create users for test.
                SetBaseUsersPool();
            }
        }

        /// <summary>
        /// Try to change passoword of user with higher rank.
        /// </summary>
        [TestMethod]
        public void NewPasswrod_HigherRankUser()
        {
            lock (Locks.CONFIG_LOCK)
            {
                // Create users for test.
                SetBaseUsersPool();
            }
        }
    }
}
