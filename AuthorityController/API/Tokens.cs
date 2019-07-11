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
            if (when < DateTime.UtcNow.AddMinutes(-Data.Config.Active.TokenValidTimeMinutes))
            {
                // Confirm expiration.
                return true;
            }

            // Conclude that token is valid.
            return false;
        }


        /// <summary>
        /// Check does this token has all requested rights.
        /// If token is not registred on this server then will throw UnauthorizedAccessException.
        /// </summary>
        /// <param name="token">Unitque token of the user.</param>
        /// <param name="error">Error that describe a reasone of fail. Could be send backward to client.</param>
        /// <param name="requesterRights">Rights detected to that token.</param>
        /// <param name="requiredRights">Array that contain the rights that need to by existed.</param>
        /// <returns></returns>
        public static bool IsHasEnoughRigths(
            string token,
            out string[] requesterRights, 
            out string error, 
            params string[] requiredRights)
        {
            // Initialize outputs
            requesterRights = null;
            error = null;

            try
            {
                // Check if the base rights exist.
                if (!API.Tokens.IsHasEnoughRigths(token, out requesterRights,
                    Data.Config.Active.QUERY_UserBan_RIGHTS))
                {
                    // Inform that token not registred.
                    error = "ERROR 401: Unauthorized";
                    return false;
                }
            }
            catch (Exception ex)
            {
                // if token not registred.
                if (ex is UnauthorizedAccessException)
                {
                    // Inform that token not registred.
                    error = "ERROR 401: Invalid token";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Check does this token has all requested rights.
        /// If token is not registred on this server then will throw UnauthorizedAccessException.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="requiredRights"></param>
        /// <param name="requesterRights">Rights detected to that token.</param>
        /// <returns></returns>
        public static bool IsHasEnoughRigths(string token, out string[] requesterRights, params string[] requiredRights)
        {
            // Try to get token rights.
            if (!Session.Current.TryGetTokenRights(token, out requesterRights))
            {
                // Create unathorized exception.
                throw new UnauthorizedAccessException("Token not registred in the table.");
            }

            // Compare arrays.
            return IsHasEnoughRigths(requesterRights, requiredRights);
        }

        /// <summary>
        /// Compare two arrays that contain rights code.
        /// Prefix '!' before rquired right will work like "not contain this right."
        /// </summary>
        /// <param name="providedRights">Rights that provided to user.</param>
        /// <param name="requiredRights">Rights that required to get permisssion.</param>
        /// <returns></returns>
        public static bool IsHasEnoughRigths(string[] providedRights, params string[] requiredRights)
        {
            // Check every requeted and provided rights.
            bool[] operationAllowenceMask = new bool[requiredRights.Length];
            for (int i = 0; i < operationAllowenceMask.Length; i++)
            {
                // Get right that requested.
                string requiredRight = requiredRights[i];

                #region Modifiers
                // Get modifiers.
                char prefix = requiredRight[0];

                // Check non prefix.
                bool non = prefix == '!';

                // Check increse\decrease posfix
                bool higher = prefix == '>';
                bool lower = prefix == '<';

                // Remove modifier from string for comparing.
                if (non || higher || lower)
                {
                    // Exclude non prefix.
                    requiredRight = requiredRight.Substring(1);
                }
                #endregion

                // Compare with every right provided to token.
                foreach (string providedRight in providedRights)
                {
                    #region Value compare
                    if (higher || lower)
                    {
                        // Try to operate data.
                        try
                        {
                            // Value that required.
                            int requiredValue = Int32.Parse(requiredRight.Split('=')[1]);
                            // Value that provided to user.
                            int providedValue = Int32.Parse(providedRight.Split('=')[1]);

                            // Compare.
                            if (higher)
                            {
                                operationAllowenceMask[i] = providedValue > requiredValue;
                            }
                            else
                            {
                                operationAllowenceMask[i] = providedValue < requiredValue;
                            }

                            // Stop search for this right cause found.
                            break;
                        }
                        catch
                        {
                            // Mark as failed.
                            operationAllowenceMask[i] = false;
                            // Stop search for this right cause found.
                            break;
                        }
                    }
                    #endregion

                    #region Normal compare
                    // Compare provided with target.
                    if (providedRight.Equals(requiredRight))
                    {
                        // Set result.
                        // Required: true
                        // Non: false
                        operationAllowenceMask[i] = !non;
                        // Stop search for this right cause found.
                        break;
                    }
                    #endregion
                }

                // If requsted right not found via provided.
                if (!operationAllowenceMask[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Check right existing in rights array.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value">Value that stored to that property by using "PROP_NAME=PROP_VALUE" format. 
        /// NULL if empty.</param>
        /// <param name="rights">Array that contain all rights.</param>
        /// <returns>Result of the seeking.</returns>
        public static bool TryToGetRight(string name, out string value, params string[] rights)
        {
            value = null;
            string[] parts = new string[2];

            // Check every right.
            foreach(string right in rights)
            {
                // Continue incorrct property.
                if(!right.StartsWith(name))
                {
                    continue;
                }

                // Try to detect value.
                int valueIndex = right.IndexOf('=');
                if (valueIndex != -1)
                {
                    // Get prop name
                    parts[0] = right.Substring(0, valueIndex);

                    // Skip if property name just seems similar to request.
                    if (!parts[0].Equals(name))
                    {
                        continue;
                    }

                    // Get value.
                    parts[1] = right.Substring(valueIndex + 1);

                    // Set value to ourput.
                    value = parts[1];

                    // Iform about success.
                    return true;
                }
                // If property not contain value.
                else
                {
                    // Skip if property name just seems similar to request.
                    if (!right.Equals(name))
                    {
                        continue;
                    }

                    // Iform about success.
                    return true;
                }
            }

            // Inform that property not found.
            return false;
        }
    }
}
