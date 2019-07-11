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
using UniformQueries;
using System.Text.RegularExpressions;

namespace AuthorityController.Queries
{
    /// <summary>
    /// Set new password for user.
    /// Require admin or certen user rights.
    /// </summary>
    public class USER_NEW_PASSWORD : IQueryHandlerProcessor
    {
        public string Description(string cultureKey)
        {
            switch (cultureKey)
            {
                case "en-US":
                default:
                    return "USER NEW PASSWORD\n" +
                            "\tDESCRIPTION: Request new password for user." +
                            "\n\tToken confirm rights to change it.\n" +
                            "\n\tOld password required to avoid access from public places.\n" +
                            "\tQUERY FORMAT: user=..." + UniformQueries.API.SPLITTING_SYMBOL + 
                            "new" + UniformQueries.API.SPLITTING_SYMBOL +
                            "password=..." + UniformQueries.API.SPLITTING_SYMBOL + 
                            "oldPassword=..." + UniformQueries.API.SPLITTING_SYMBOL +
                            "token=..." + "\n";
            }
        }

        public void Execute(QueryPart[] queryParts)
        {
            string error;

            #region Get params.
            UniformQueries.API.TryGetParamValue("user",         out QueryPart user, queryParts);
            UniformQueries.API.TryGetParamValue("password",     out QueryPart password, queryParts);
            UniformQueries.API.TryGetParamValue("oldPassword",  out QueryPart oldPassword, queryParts);
            UniformQueries.API.TryGetParamValue("token",        out QueryPart token, queryParts);
            #endregion

            #region Detect target user
            if (!API.Users.TryToFindUserUniform(user.propertyValue, out Data.User userProfile, out error))
            {
                // Inform about error.
                UniformServer.BaseServer.SendAnswer(error, queryParts);
                return;
            }
            #endregion

            #region Check base requester rights
            if (!API.Tokens.IsHasEnoughRigths(
                token.propertyValue,
                out string[] requesterRights,
                out error,
                Data.Config.Active.QUERY_UserNewPassword_RIGHTS))
            {
                // Inform about error.
                UniformServer.BaseServer.SendAnswer(error, queryParts);
                return;
            }
            #endregion

            #region Check rank permition
            // Is that the self update?
            bool isSelfUpdate = false;

            // Check every token provided to target user.
            foreach(string userToken in userProfile.tokens)
            {
                // Comare tokens.
                if (token == userToken)
                {
                    // Mark as self target.
                    isSelfUpdate = true;

                    // Interupt loop.
                    break;
                }
            }

            // If not the self update request, then check rights to moderate.
            if (!isSelfUpdate)
            {
                // Get target User's rank.
                if(!API.Tokens.TryToGetRight("rank", out string userRank, userProfile.rights))
                {
                    // Inform that rights not enough.
                    UniformServer.BaseServer.SendAnswer("ERROR 401: Unauthorized", queryParts);
                    return;
                }

                // Check token rights.
                if (!API.Tokens.IsHasEnoughRigths(requesterRights,
                    // Request hiegher rank then user and at least moderator level.
                    ">rank=" + userRank, ">rank=2"))
                {
                    // Inform that rank not defined.
                    UniformServer.BaseServer.SendAnswer("ERROR 401: User rank not defined", queryParts);
                    return;
                }
            }
            #endregion

            #region Validate password.
            // Comapre password with stored.
            if (!userProfile.IsOpenPasswordCorrect(oldPassword.propertyValue))
            {
                // Inform that password is incorrect.
                UniformServer.BaseServer.SendAnswer("ERROR 412: Incorrect password", queryParts);
                return;
            }
            #endregion

            #region Validate new password
            if(!PasswordValidation(password.propertyValue, out string errorMessage))
            {
                // Inform about incorrect login size.
                UniformServer.BaseServer.SendAnswer(
                    errorMessage,
                    queryParts);
                return;
            }
            #endregion

            // Update password.
            userProfile.password = API.Users.GetHashedPassword(password.propertyValue);

            // Update stored profile.
            API.Users.SetProfile(userProfile);
        }

        public bool IsTarget(QueryPart[] queryParts)
        {
            // USER prop.
            if(!UniformQueries.API.QueryParamExist("user", queryParts))
                return false;

            // NEW prop.
            if (!UniformQueries.API.QueryParamExist("new", queryParts))
                return false;

            // PASSWORD prop.
            if (!UniformQueries.API.QueryParamExist("password", queryParts))
                return false;

            // OLD PASSWORD prop.
            if (!UniformQueries.API.QueryParamExist("oldPassword", queryParts))
                return false;

            return true;
        }

        /// <summary>
        /// Validate password before converting to salted hash.
        /// </summary>
        /// <param name="password">Open password.</param>
        /// <param name="error">Error string that will be situable in case of validation fail.</param>
        /// <returns>Result of validation.</returns>
        public static bool PasswordValidation(string password, out string error)
        {
            error = null;

            if (string.IsNullOrEmpty(password) ||
               password.Length < Data.Config.Active.PasswordMinAllowedLength ||
               password.Length > Data.Config.Active.PasswordMaxAllowedLength)
            {
                // Inform about incorrect login size.
                error =
                    "ERROR 401: Invalid password size. Require " +
                    Data.Config.Active.LoginMinSize + "-" +
                    Data.Config.Active.LoginMaxSize + " caracters.";
                return false;
            }

            // Validate format
            if (!Regex.IsMatch(password, @"^[a-zA-Z0-9@!#$%_]+$"))
            {
                // Inform about incorrect login size.
                error = "ERROR 401: Invalid password format. Allowed symbols: [a-z][A-Z][0-9]@!#$%_";
                return false;
            }

            // Special symbol required.
            if (Data.Config.Active.PasswordRequireDigitSymbol)
            {
                if (!Regex.IsMatch(password, @"^[0-9]+$"))
                {
                    // Inform about incorrect login size.
                    error = "ERROR 401: Invalid password format. Need to have at least one digit 0-9";
                    return false;
                }
            }

            // Special symbol required.
            if (Data.Config.Active.PasswordRequireNotLetterSymbol)
            {
                if (!Regex.IsMatch(password, @"^[@!#$%_]+$"))
                {
                    // Inform about incorrect login size.
                    error = "ERROR 401: Invalid password format. Need to have at least one of followed symbols: @!#$%_";
                    return false;
                }
            }

            // Upper cse required.
            if (Data.Config.Active.PasswordRequireUpperSymbol)
            {
                if (!Regex.IsMatch(password, @"^[A-Z]+$"))
                {
                    // Inform about incorrect login size.
                    error = "ERROR 401: Invalid password format. Need to have at least one symbol in upper case.";
                    return false;
                }
            }

            return true;
        }

    }
}
