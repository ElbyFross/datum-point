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
using AuthorityControllerTests;
using Microsoft.Win32.SafeHandles;
using UniformQueries;

namespace AuthorityController.Tests
{
    [TestClass]
    public class Queries
    {
        [TestInitialize]
        public void Setup()
        {
        }

        /// <summary>
        /// Trying to get guest token using query.
        /// </summary>
        [TestMethod]
        public void GetGuestToken()
        {
            // Marker.
            bool waitingAnswer = true;

            // Make query.
            QueryPart[] queryParts = new QueryPart[]
            {
                new QueryPart("guid", "GetGuestToken"),
                new QueryPart("GET"),
                new QueryPart("GUEST"),
                new QueryPart("TOKEN")
            };

            // Start listening client.
            UniformClient.SimpleClient.ReceiveAnswer(
                UniformClient.BaseClient.OpenOutTransmissionLine("localhost", "GetGuestToken"),
                queryParts,
                (PipesProvider.Client.TransmissionLine line, object obj) =>
                {
                    // Validate answer.
                    if (obj is string answer)
                    {
                        // Unlock finish blocker.
                        waitingAnswer = false;

                        QueryPart[] recivedQuery = UniformQueries.API.DetectQueryParts(answer);

                        // Check token.
                        if(UniformQueries.API.TryGetParamValue("token", out QueryPart token, recivedQuery))
                        {
                            bool tokenProvided = !string.IsNullOrEmpty(token.propertyValue);
                            Assert.IsTrue(tokenProvided, "Token is null.\n"+ answer);
                        }
                        else
                        {
                            // Inform that failed.
                            Assert.IsTrue(false, "Token not provided.\n" + answer);
                            return;
                        }

                        // Check expire time.
                        if (!UniformQueries.API.TryGetParamValue("expireIn", out QueryPart _, recivedQuery))
                            Assert.IsTrue(false, "Expire time not provided.\n" + answer);

                        // Check rights providing.
                        if (!UniformQueries.API.TryGetParamValue("rights", out QueryPart _, recivedQuery))
                            Assert.IsTrue(false, "Rights not shared.\n" + answer);
                    }
                    else
                    {
                        // Inform that failed.
                        Assert.IsTrue(false, "Incorrect answer type.");
                        return;
                    }
                });

            // Start processing.
            if (UniformQueries.API.TryFindQueryHandler(queryParts, out IQueryHandler handler))
            {
                // Execute query.
                handler.Execute(queryParts);
            }
            else
            {
                // Inform that failed.
                Assert.IsTrue(false, "Query not found.");
            }

            // Wait server answer.
            while(waitingAnswer)
            {
                Thread.Sleep(5);
            }
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
