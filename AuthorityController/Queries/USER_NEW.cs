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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UniformQueries;

namespace AuthorityController.Queries
{
    /// <summary>
    /// Create new user.
    /// </summary>
    public class USER_NEW : IQueryHandlerProcessor
    {
        public string Description(string cultureKey)
        {
            switch (cultureKey)
            {
                case "en-US":
                default:
                    return "USER NEW\n" +
                            "\tDESCRIPTION: Request new password for user." +
                            "\n\tToken confirm rights to change it.\n" +
                            "\n\tOld password required to avoid access from public places.\n" +
                            "\tQUERY FORMAT: user=XMLSetializedUser" + UniformQueries.API.SPLITTING_SYMBOL +
                            "new\n";
            }
        }

        public void Execute(QueryPart[] queryParts)
        {
            // Get params.
            UniformQueries.API.TryGetParamValue("login", out QueryPart login, queryParts);
            UniformQueries.API.TryGetParamValue("password", out QueryPart password, queryParts);
            UniformQueries.API.TryGetParamValue("fn", out QueryPart firstName, queryParts);
            UniformQueries.API.TryGetParamValue("sn", out QueryPart secondName, queryParts);

            UniformQueries.API.TryGetParamValue("os", out QueryPart os, queryParts);
            UniformQueries.API.TryGetParamValue("mac", out QueryPart mac, queryParts);
            UniformQueries.API.TryGetParamValue("stamp", out QueryPart timeStamp, queryParts);

            System.Threading.Thread.Sleep(5);

            #region Validate login
            if (string.IsNullOrEmpty(login.propertyValue) || 
               login.propertyValue.Length < Data.Config.Active.LoginMinSize ||
               login.propertyValue.Length > Data.Config.Active.LoginMaxSize)
            {
                // Inform about incorrect login size.
                UniformServer.BaseServer.SendAnswer(
                    "ERROR 401: Invalid login size. Require " +
                    Data.Config.Active.LoginMinSize + "-" +
                    Data.Config.Active.LoginMaxSize + " caracters.",
                    queryParts);
                return;
            }

            // Check login format.
            if(!Regex.IsMatch(login.propertyValue, @"^[a-zA-Z0-9@._]+$"))
            {
                // Inform about incorrect login size.
                UniformServer.BaseServer.SendAnswer(
                    "ERROR 401: Invalid login format. Allowed symbols: [a-z][A-Z][0-9]@._" ,
                    queryParts);
                return;

            }

            // Check login exist.
            if (API.Users.TryToFindUser(login.propertyValue, out Data.User _))
            {
                // Inform that target user has the same or heigher rank then requester.
                UniformServer.BaseServer.SendAnswer("ERROR 401: Login occupied", queryParts);
                return;
            }
            #endregion

            System.Threading.Thread.Sleep(5);

            #region Validate password
            if (string.IsNullOrEmpty(login.propertyValue) ||
               password.propertyValue.Length < Data.Config.Active.PasswordMinAllowedLength ||
               password.propertyValue.Length > Data.Config.Active.PasswordMaxAllowedLength)
            {
                // Inform about incorrect login size.
                UniformServer.BaseServer.SendAnswer(
                    "ERROR 401: Invalid password size. Require " +
                    Data.Config.Active.LoginMinSize + "-" +
                    Data.Config.Active.LoginMaxSize + " caracters.",
                    queryParts);
                return;
            }

            // Validate format
            if (!Regex.IsMatch(login.propertyValue, @"^[a-zA-Z0-9@!#$%_]+$"))
            {
                // Inform about incorrect login size.
                UniformServer.BaseServer.SendAnswer(
                    "ERROR 401: Invalid password format. Allowed symbols: [a-z][A-Z][0-9]@!#$%_",
                    queryParts);
                return;

            }

            // Special symbol required.
            if (Data.Config.Active.PasswordRequireDigitSymbol)
            {
                if (!Regex.IsMatch(login.propertyValue, @"^[0-9]+$"))
                {
                    // Inform about incorrect login size.
                    UniformServer.BaseServer.SendAnswer(
                        "ERROR 401: Invalid password format. Need to have at least one digit 0-9",
                        queryParts);
                    return;

                }
            }

            // Special symbol required.
            if (Data.Config.Active.PasswordRequireNotLetterSymbol)
            {
                if (!Regex.IsMatch(login.propertyValue, @"^[@!#$%_]+$"))
                {
                    // Inform about incorrect login size.
                    UniformServer.BaseServer.SendAnswer(
                        "ERROR 401: Invalid password format. Need to have at least one of followed symbols: @!#$%_",
                        queryParts);
                    return;

                }
            }

            // Upper cse required.
            if (Data.Config.Active.PasswordRequireUpperSymbol)
            {
                if (!Regex.IsMatch(login.propertyValue, @"^[A-Z]+$"))
                {
                    // Inform about incorrect login size.
                    UniformServer.BaseServer.SendAnswer(
                        "ERROR 401: Invalid password format. Need to have at least one symbol in upper case.",
                        queryParts);
                    return;

                }
            }
            #endregion
            
            System.Threading.Thread.Sleep(5);

            // TODO Validate names.

            // TODO Create user profile data to server.
        }

        public bool IsTarget(QueryPart[] queryParts)
        {
            // USER prop.
            if (!UniformQueries.API.QueryParamExist("user", queryParts))
                return false;

            // NEW prop.
            if (!UniformQueries.API.QueryParamExist("new", queryParts))
                return false;


            if (!UniformQueries.API.QueryParamExist("login", queryParts))
                return false;

            if (!UniformQueries.API.QueryParamExist("password", queryParts))
                return false;

            if (!UniformQueries.API.QueryParamExist("fn", queryParts))
                return false;

            if (!UniformQueries.API.QueryParamExist("sn", queryParts))
                return false;


            // User operation system.
            if (!UniformQueries.API.QueryParamExist("os", queryParts))
                return false;

            // Mac adress of logon device.
            if (!UniformQueries.API.QueryParamExist("mac", queryParts))
                return false;

            // Session open time
            if (!UniformQueries.API.QueryParamExist("stamp", queryParts))
                return false;

            return true;
        }
    }
}
