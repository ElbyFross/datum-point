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

namespace AuthorityController
{
    /// <summary>
    /// Object that control data relative to current authority session.
    /// </summary>
    [System.Serializable]
    public class Session
    {
        #region Public properties
        /// <summary>
        /// Last created session.
        /// </summary>
        public static Session Last
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
        #endregion

        #region Constructors
        public Session()
        {
            // Set this session as active.
            Last = this;
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

        /// <summary>
        /// Table that contain users.
        /// 
        /// Key - int id.
        /// Value - User object.
        /// </summary>
        private readonly Hashtable users = new Hashtable();
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
                // Loading toking info.
                TokenInfo info = (TokenInfo)tokensRights[token];
                // Update rights.
                info.rights = rights;
            }
            else
            {
                // Create anonymous container.
                TokenInfo info = TokenInfo.Anonymous;

                // Apply fields.
                info.token = token;
                info.rights = rights;

                // Set as new.
                tokensRights.Add(token, info);
            }

            // TODO inform relative servers.
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
                // TODO inform relative servers.
            }
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
                tokensRights.Remove(token);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("TOKEN REMOVING ERROR:\n{0}", ex.Message);
                return false;
            }
        }
        #endregion
    }
}
