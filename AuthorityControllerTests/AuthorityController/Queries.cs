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
        public void SetBaseUserPool()
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

            // Registrate token in session.
            user_SuperAdmin.tokens.Add(user_SuperAdmin.tokens[0]);
            AuthorityController.Session.Current.SetTokenRights(user_SuperAdmin.tokens[0], user_SuperAdmin.rights);
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

            // Registrate token in session.
            user_Admin.tokens.Add(user_Admin.tokens[0]);
            AuthorityController.Session.Current.SetTokenRights(user_Admin.tokens[0], user_Admin.rights);
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

            // Registrate token in session.
            user_Moderator.tokens.Add(user_Moderator.tokens[0]);
            AuthorityController.Session.Current.SetTokenRights(user_Moderator.tokens[0], user_Moderator.rights);
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

            // Registrate token in session.
            user_PrivilegedUser.tokens.Add(user_PrivilegedUser.tokens[0]);
            AuthorityController.Session.Current.SetTokenRights(user_PrivilegedUser.tokens[0], user_PrivilegedUser.rights);
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

            // Registrate token in session.
            user_User.tokens.Add(user_User.tokens[0]);
            AuthorityController.Session.Current.SetTokenRights(user_User.tokens[0], user_User.rights);
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

            // Registrate token in session.
            user_Guest.tokens.Add(user_User.tokens[0]);
            AuthorityController.Session.Current.SetTokenRights(user_Guest.tokens[0], user_Guest.rights);
            #endregion

            // Wait until loading.
            while(API.Users.HasAsyncLoadings)
            {
                Thread.Sleep(5);
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
            // Create users for test.
            SetBaseUserPool();
        }

        /// <summary>
        /// Trying to update token rights with walid permition.
        /// </summary>
        [TestMethod]
        public void SetTokenRights_HasRights()
        {
            // Create users for test.
            SetBaseUserPool();
        }

        /// <summary>
        /// Try to ban user but has no rights to this.
        /// </summary>
        [TestMethod]
        public void UserBan_NoRights()
        {
            // Create users for test.
            SetBaseUserPool();
        }

        /// <summary>
        /// Try to ban user that has a higher rank than requester.
        /// </summary>
        [TestMethod]
        public void UserBan_HighrankerBan()
        {
            // Create users for test.
            SetBaseUserPool();
        }

        /// <summary>
        /// Try to ban user with enough rights to that.
        /// </summary>
        [TestMethod]
        public void UserBan_HasRights()
        {
            // Create users for test.
            SetBaseUserPool();
        }

        /// <summary>
        /// Try to logon as existed user.
        /// </summary>
        [TestMethod]
        public void Logon_UserExist()
        {
            // Create users for test.
            SetBaseUserPool();
        }


        /// <summary>
        /// Try to logon as existed user that not registred.
        /// </summary>
        [TestMethod]
        public void Logon_UserNotExist()
        {
            // Create users for test.
            SetBaseUserPool();
        }

        /// <summary>
        /// Try to logon with incorrrect password.
        /// </summary>
        [TestMethod]
        public void Logon_InvalidData()
        {
            // Create users for test.
            SetBaseUserPool();
        }

        /// <summary>
        /// Try to logoff user by invalid token.
        /// </summary>
        [TestMethod]
        public void Logoff_InvalidToken()
        {
            // Create users for test.
            SetBaseUserPool();
        }

        /// <summary>
        /// Try to logoff user by valid token.
        /// </summary>
        [TestMethod]
        public void Logoff_ValidToken()
        {
            // Create users for test.
            SetBaseUserPool();
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
            // Create users for test.
            SetBaseUserPool();
        }

        /// <summary>
        /// Try to change passoword of user with lowes rank.
        /// </summary>
        [TestMethod]
        public void NewPasswrod_LowerRankUser()
        {
            // Create users for test.
            SetBaseUserPool();
        }

        /// <summary>
        /// Try to change passoword of user with higher rank.
        /// </summary>
        [TestMethod]
        public void NewPasswrod_HigherRankUser()
        {
            // Create users for test.
            SetBaseUserPool();
        }
    }
}
