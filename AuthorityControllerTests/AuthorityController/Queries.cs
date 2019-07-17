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
using UniformServer;
using UniformClient;
using PipesProvider.Server.TransmissionControllers;

namespace AuthorityController.Tests
{
    [TestClass]
    public class Queries
    {
        /// <summary>
        /// Get routing table situale for local broadcasting.
        /// </summary>
<<<<<<< HEAD
        PipesProvider.Networking.Routing.RoutingTable BroadcastingRoutingTable
=======
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
        public void SetBaseUserPool()
>>>>>>> Authorization
        {
            get
            {
                // Create new if not found.
                if(_broadcastingRoutingTable == null)
                {

                }

                return _broadcastingRoutingTable;
            }
        }
        PipesProvider.Networking.Routing.RoutingTable _broadcastingRoutingTable;

        [TestInitialize]
        public void Setup()
        {
            // Set guest token provider as MessageHandeler.
            BroadcastingServerTransmissionController.MessageHandeler messageHandler =
                AuthorityController.API.Tokens.AuthorizeNewGuestToken;

            // Start broadcastig.
            BaseServer.StartBroadcastingViaPP(
                "guests",
                PipesProvider.Security.SecurityLevel.Anonymous,
                messageHandler, 
                1);
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
                new QueryPart("GET"),
                new QueryPart("GUEST"),
                new QueryPart("TOKEN")
            };

            #region Server answer processing
            // Start listening client.
            bool reciverStarted = UniformClient.Standard.SimpleClient.ReceiveDelayedAnswerViaPP(
                UniformClient.BaseClient.OpenOutTransmissionLineViaPP("localhost", "guests"),
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

            if(!reciverStarted)
            {
                Assert.IsTrue(false, "Reciver not started");
                return;
            }
            #endregion

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
<<<<<<< HEAD
=======
            lock (Locks.CONFIG_LOCK)
            {
                // Create users for test.
                SetBaseUserPool();
            }
>>>>>>> Authorization
        }

        /// <summary>
        /// Trying to update token rights with walid permition.
        /// </summary>
        [TestMethod]
        public void SetTokenRights_HasRights()
        {
<<<<<<< HEAD
=======
            lock (Locks.CONFIG_LOCK)
            {
                // Create users for test.
                SetBaseUserPool();
            }
>>>>>>> Authorization
        }

        /// <summary>
        /// Try to ban user but has no rights to this.
        /// </summary>
        [TestMethod]
        public void UserBan_NoRights()
        {
<<<<<<< HEAD
=======
            lock (Locks.CONFIG_LOCK)
            {
                // Create users for test.
                SetBaseUserPool();
            }
>>>>>>> Authorization
        }

        /// <summary>
        /// Try to ban user that has a higher rank than requester.
        /// </summary>
        [TestMethod]
        public void UserBan_HighrankerBan()
        {
<<<<<<< HEAD
=======
            lock (Locks.CONFIG_LOCK)
            {
                // Create users for test.
                SetBaseUserPool();
            }
>>>>>>> Authorization
        }

        /// <summary>
        /// Try to ban user with enough rights to that.
        /// </summary>
        [TestMethod]
        public void UserBan_HasRights()
        {
<<<<<<< HEAD
=======
            lock (Locks.CONFIG_LOCK)
            {
                // Create users for test.
                SetBaseUserPool();
            }
>>>>>>> Authorization
        }

        /// <summary>
        /// Try to logon as existed user.
        /// </summary>
        [TestMethod]
        public void Logon_UserExist()
        {
<<<<<<< HEAD
=======
            lock (Locks.CONFIG_LOCK)
            {
                // Create users for test.
                SetBaseUserPool();

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
>>>>>>> Authorization
        }


        /// <summary>
        /// Try to logon as existed user that not registred.
        /// </summary>
        [TestMethod]
        public void Logon_UserNotExist()
        {
<<<<<<< HEAD
=======
            lock (Locks.CONFIG_LOCK)
            {
                // Create users for test.
                SetBaseUserPool();

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
>>>>>>> Authorization
        }

        /// <summary>
        /// Try to logon with incorrrect password.
        /// </summary>
        [TestMethod]
        public void Logon_InvalidData()
        {
<<<<<<< HEAD
=======
            lock (Locks.CONFIG_LOCK)
            {
                // Create users for test.
                SetBaseUserPool();

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
>>>>>>> Authorization
        }

        /// <summary>
        /// Try to logoff user by invalid token.
        /// </summary>
        [TestMethod]
        public void Logoff_InvalidToken()
        {
<<<<<<< HEAD
=======
            lock (Locks.CONFIG_LOCK)
            {
                // Create users for test.
                SetBaseUserPool();

                // Generate new token.
                string newToken = AuthorityController.API.Tokens.UnusedToken;

                // Logoff unregistred token.
                bool result = AuthorityController.Queries.USER_LOGOFF.LogoffToken(newToken);

                // Assert that token was rejected by a system.
                // If token was processed that this mean that system failed.
                Assert.IsTrue(!result, "Token detected, that can't be true.");
            }
>>>>>>> Authorization
        }

        /// <summary>
        /// Try to logoff user by valid token.
        /// </summary>
        [TestMethod]
        public void Logoff_ValidToken()
        {
<<<<<<< HEAD
=======
            lock (Locks.CONFIG_LOCK)
            {
                // Create users for test.
                SetBaseUserPool();

                // Get token of registred user.
                string userToken = user_User.tokens[0];

                // Logoff unregistred token.
                bool result = AuthorityController.Queries.USER_LOGOFF.LogoffToken(userToken);

                // Assert that token was rejected by a system.
                // If token was processed that this mean that system failed.
                Assert.IsTrue(result, "Token not detected.");
            }
>>>>>>> Authorization
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
<<<<<<< HEAD
=======
            lock (Locks.CONFIG_LOCK)
            {
                // Create users for test.
                SetBaseUserPool();
            }
>>>>>>> Authorization
        }

        /// <summary>
        /// Try to change passoword of user with lowes rank.
        /// </summary>
        [TestMethod]
        public void NewPasswrod_LowerRankUser()
        {
<<<<<<< HEAD
=======
            lock (Locks.CONFIG_LOCK)
            {
                // Create users for test.
                SetBaseUserPool();
            }
>>>>>>> Authorization
        }

        /// <summary>
        /// Try to change passoword of user with higher rank.
        /// </summary>
        [TestMethod]
        public void NewPasswrod_HigherRankUser()
        {
<<<<<<< HEAD
=======
            lock (Locks.CONFIG_LOCK)
            {
                // Create users for test.
                SetBaseUserPool();
            }
>>>>>>> Authorization
        }
    }
}
