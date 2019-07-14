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
using System.Text;
using System.Threading.Tasks;
using AuthorityController.Data;
using PipesProvider.Networking.Routing;

namespace AuthorityController
{
    /// <summary>
    /// Object that control data relative to current authority session.
    /// </summary>
    [System.Serializable]
    public class Session
    {
        #region Public properties and fields
        /// <summary>
        /// Last created session.
        /// </summary>
        public static Session Current
        {
            get
            {
                if(last == null)
                {
                    last = new Session();
                }
                return last;
            }

            protected set { last = value; }
        }

        /// <summary>
        /// Routing table that contain instructions to access reletive servers
        /// that need to be informed about token events.
        /// 
        /// Before sharing query still will check is the query stituable for that routing instruction.
        /// If you no need any filtring then just leave query patterns empty.
        /// </summary>
        public RoutingTable relatedServers;
        #endregion

        #region Constructors
        public Session()
        {
            // Set this session as active.
            Current = this;
        }
        #endregion

        #region Private fields
        /// Object that contain current session.
        private static Session last;

        /// <summary>
        /// Table that contains rights provided to token.
        /// 
        /// Key - string token
        /// Value - TokenInfo
        /// </summary>
        private readonly Hashtable tokensRights = new Hashtable();
        #endregion

        #region Public methods
        /// <summary>
        /// Set rights' codes array as relative to token.
        /// 
        /// In case if token infor not registred then will create anonimouse info with applied rights.
        /// Applicable to purposes of servers that depends to session provider one, 
        /// but not require entire token information, cause not manage it.
        /// </summary>
        /// <param name="token">Session token.</param>
        /// <param name="rights">Array og rights' codes.</param>
        public void SetTokenRights(string token, params string[] rights)
        {
            // Update rights if already exist.
            if (tokensRights.ContainsKey(token))
            {
                // Loading token info.
                TokenInfo info = (TokenInfo)tokensRights[token];

                // If not anonymous user.
                if (API.Users.TryToFindUser(info.userId, out User user))
                {
                    // Update every token.
                    foreach(string additiveTokens in user.tokens)
                    {
                        // Loading toking info.
                        TokenInfo additiveInfo = (TokenInfo)tokensRights[additiveTokens];

                        // Update rights.
                        additiveInfo.rights = rights;

                        // Send info to relative servers.
                        SendNewRightsForToken(additiveTokens, rights);
                    }
                }
                // if user not detected.
                else
                {
                    // Update rights.
                    info.rights = rights;

                    // Send info to relative servers.
                    SendNewRightsForToken(token, rights);
                }
            }
            // If user was not loaded.
            else
            {
                // Create anonymous container.
                TokenInfo info = TokenInfo.Anonymous;

                // Apply fields.
                info.token = token;
                info.rights = rights;

                // Set as new.
                tokensRights.Add(token, info);

                // Send info to relative servers.
                SendNewRightsForToken(token, rights);
            }
        }

        /// <summary>
        /// Trying to load rights registred for token.
        /// </summary>
        /// <param name="token">Session token.</param>
        /// <param name="rights">Array of rights' codes relative to token.</param>
        /// <returns></returns>
        public bool TryGetTokenRights(string token, out string[] rights)
        {
            // Try to get regustred rights.
            if (tokensRights[token] is Data.TokenInfo rightsBufer)
            {
                rights = rightsBufer.rights;
                return true;
            }

            // Inform about fail.
            rights = null;
            return false;
        }
        
        /// <summary>
        /// Remove token from table and inform relative servers about that.
        /// </summary>
        /// <param name="token"></param>
        public void SetExpired(string token)
        {
            if (RemoveToken(token))
            {
                // Compose query that will shared to related servers to update them local data.
                string informQuery = string.Format("set{0}token={1}{0}expired",
                    UniformQueries.API.SPLITTING_SYMBOL,
                    token);

                // Send query to infrom related servers about event.
                SendEqueryToRelatedServers(informQuery);
            }
        }

        /// <summary>
        /// Try to find registred token info.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool TryGetTokenInfo(string token, out TokenInfo info)
        {
            if(tokensRights[token] is TokenInfo bufer)
            {
                info = bufer;
                return true;
            }

            info = TokenInfo.Anonymous;
            return false;
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Removing token from table.
        /// </summary>
        /// <param name="token"></param>
        /// <returns>Is removed successful?</returns>
        private bool RemoveToken(string token)
        {
            try
            {
                // If user not anonymous.
                if(tokensRights[token] is User user)
                {
                    // Remove tooken.
                    user.tokens.Remove(token);
                }

                // Unregister token from table.
                tokensRights.Remove(token);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("TOKEN REMOVING ERROR:\n{0}", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Sending new rights of token to releted servers.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="rights"></param>
        private void SendNewRightsForToken(string token, string[] rights)
        {
            // Compose query that will shared to related servers to update them local data.
            string informQuery = string.Format("set{0}token={1}{0}rights=",
                UniformQueries.API.SPLITTING_SYMBOL,
                token);

            // Add rights' codes.
            foreach (string rightsCode in rights)
            {
                // Add every code splited by '+'.
                informQuery += "+" + rightsCode;
            }

            // Send query to infrom related servers about event.
            SendEqueryToRelatedServers(informQuery);
        }

        /// <summary>
        /// Transmit information to every related servers that situable for queries.
        /// </summary>
        /// <param name="query"></param>
        private void SendEqueryToRelatedServers(string query)
        {
            // Inform relative servers.
            if (relatedServers != null)
            {
                // Check every instruction.
                for (int i = 0; i < relatedServers.intructions.Count; i++)
                {
                    // Get instruction.
                    Instruction instruction = relatedServers.intructions[i];

                    // Does instruction situable to query.
                    if (!instruction.IsRoutingTarget(query))
                    {
                        // Skip if not.
                        continue;
                    }

                    // Open transmission line to server.
                    UniformClient.BaseClient.OpenOutTransmissionLineViaPP(instruction.routingIP, instruction.pipeName).
                        EnqueueQuery(query).              // Add query to queue.
                        SetInstructionAsKey(ref instruction).   // Apply encryption if requested.
                        TryLogonAs(instruction.logonConfig);    // Profide logon data to access remote machine.
                }
            }
        }
        #endregion
    }
}
