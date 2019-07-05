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
    /// API that provide operation with authority data.
    /// </summary>
    public static class API
    {
        /// <summary>
        /// How many minutes token is valid.
        /// </summary>
        public static int tokenValidTimeMinuts = 1440;

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
        /// Table that contains rights provided to token.
        /// 
        /// Key - string token
        /// Value - TokenInfo
        /// </summary>
        private static readonly Hashtable tokensRights = new Hashtable();
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
        public static void SetTokenRights(string token, params string[] rights)
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
        }

        /// <summary>
        /// Trying to load rights registred for token.
        /// </summary>
        /// <param name="token">Session token.</param>
        /// <param name="rights">Array of rights' codes relative to token.</param>
        /// <returns></returns>
        public static bool TryGetTokenRights(string token, out string[] rights)
        {
            // Try to get regustred rights.
            if(tokensRights[token] is Data.TokenInfo rightsBufer)
            {
                rights = rightsBufer.rights;
                return true;
            }

            // Inform about fail.
            rights = null;
            return false;
        }

        /// <summary>
        /// Removing token from table.
        /// </summary>
        /// <param name="token"></param>
        public static void RemoveToken(string token)
        {
            try
            {
                tokensRights.Remove(token);
            }
            catch (Exception ex)
            {
                Console.WriteLine("TOKEN REMOVING ERROR:\n{0}", ex.Message);
            }
        }

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
        public static bool IsExpired(string token)
        {
            // Convert token to bytes array.
            byte[] data = Convert.FromBase64String(token);

            // Get when token created. Date time will take the first bytes that contain data stamp.
            DateTime when = DateTime.FromBinary(BitConverter.ToInt64(data, 0));

            // Compare with allowed token time.
            if (when < DateTime.UtcNow.AddMinutes(-tokenValidTimeMinuts))
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
