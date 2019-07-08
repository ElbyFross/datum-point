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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthorityController.API
{
    public class Tokens
    {   
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
            if (when < DateTime.UtcNow.AddMinutes(-Data.Config.Active.tokenValidTimeMinutes))
            {
                // Confirm expiration.
                return true;
            }

            // Conclude that token is valid.
            return false;
        }

        /// <summary>
        /// Check does this token has all requested rights.
        /// If token is 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="requiredRights"></param>
        /// <returns></returns>
        public static bool IsHasEnoughRigths(string token, params string[] requiredRights)
        {
            // Try to get token rights.
            if (!Session.Current.TryGetTokenRights(token, out string[] requesterRights))
            {
                // Create unathorized exception.
                throw new UnauthorizedAccessException("Token not registred in the table.");
            }

            // Compare arrays.
            return IsHasEnoughRigths(requesterRights, requiredRights);
        }

        public static bool IsHasEnoughRigths(string[] providedRights, params string[] requiredRights)
        {
            // Check every requeted and provided rights.
            bool[] operationAllowenceMask = new bool[requiredRights.Length];
            for (int i = 0; i < operationAllowenceMask.Length; i++)
            {
                // Get right that requested.
                string targetRight = requiredRights[i];

                // Compare with every right provided to token.
                foreach (string providedRight in providedRights)
                {
                    // Comare provided with target.
                    if (providedRight.Equals(targetRight))
                    {
                        operationAllowenceMask[i] = true;
                        break;
                    }
                }

                // If requsted right not found via provided.
                if (!operationAllowenceMask[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
