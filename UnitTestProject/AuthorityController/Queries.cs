using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject.AuthorityController
{
    [TestClass]
    public class Queries
    {
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
        }

        /// <summary>
        /// Trying to update token rights with walid permition.
        /// </summary>
        [TestMethod]
        public void SetTokenRights_HasRights()
        {
        }

        /// <summary>
        /// Try to ban user but has no rights to this.
        /// </summary>
        [TestMethod]
        public void UserBan_NoRights()
        {
        }

        /// <summary>
        /// Try to ban user that has a higher rank than requester.
        /// </summary>
        [TestMethod]
        public void UserBan_HighrankerBan()
        {
        }

        /// <summary>
        /// Try to ban user with enough rights to that.
        /// </summary>
        [TestMethod]
        public void UserBan_HasRights()
        {
        }

        /// <summary>
        /// Try to logon as existed user.
        /// </summary>
        [TestMethod]
        public void Logon_UserExist()
        {
        }


        /// <summary>
        /// Try to logon as existed user that not registred.
        /// </summary>
        [TestMethod]
        public void Logon_UserNotExist()
        {
        }

        /// <summary>
        /// Try to logon with incorrrect password.
        /// </summary>
        [TestMethod]
        public void Logon_InvalidData()
        {
        }

        /// <summary>
        /// Try to logoff user by invalid token.
        /// </summary>
        [TestMethod]
        public void Logoff_InvalidToken()
        {
        }

        /// <summary>
        /// Try to logoff user by valid token.
        /// </summary>
        [TestMethod]
        public void Logoff_ValidToken()
        {
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
        }

        /// <summary>
        /// Try to change passoword of user with lowes rank.
        /// </summary>
        [TestMethod]
        public void NewPasswrod_LowerRankUser()
        {
        }

        /// <summary>
        /// Try to change passoword of user with higher rank.
        /// </summary>
        [TestMethod]
        public void NewPasswrod_HigherRankUser()
        {
        }
    }
}
