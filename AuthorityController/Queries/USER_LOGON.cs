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

namespace AuthorityController.Queries
{
    /// <summary>
    /// Logon user in system.
    /// Provide token as result.
    /// 
    /// USER&LOGON&login=...&password=...&mac=...&os=....&
    /// </summary>
    public class USER_LOGON : IQueryHandlerProcessor
    {
        public string Description(string cultureKey)
        {
            throw new NotImplementedException();
        }

        public void Execute(QueryPart[] queryParts)
        {
            // Get params.
            UniformQueries.API.TryGetParamValue("login", out QueryPart login, queryParts);
            UniformQueries.API.TryGetParamValue("password", out QueryPart password, queryParts);
            UniformQueries.API.TryGetParamValue("os", out QueryPart os, queryParts);
            UniformQueries.API.TryGetParamValue("mac", out QueryPart mac, queryParts);
            UniformQueries.API.TryGetParamValue("stamp", out QueryPart timeStamp, queryParts);

            // Find user.
            if(!API.Users.TryToFindUser(login.propertyValue, out Data.User user))
            {
                // Inform that user not found.
                UniformServer.BaseServer.SendAnswer("ERROR 412: User not found", queryParts);
                return;
            }

            #region Validate password.
            // Comapre password with stored.
            if(!user.IsOpenPasswordCorrect(password.propertyValue))
            {
                // Inform that password is incorrect.
                UniformServer.BaseServer.SendAnswer("ERROR 412: Incorrect password", queryParts);
                return;
            }

            // Check for logon bans
            if(API.Users.IsBanned(user, "logon"))
            {
                // Inform that password is incorrect.
                UniformServer.BaseServer.SendAnswer("ERROR 412: User banned.", queryParts);
                return;
            }

            // Generate new token.
            string sessionToken = API.Tokens.UnusedToken;

            // Registrate token in session.
            user.tokens.Add(sessionToken);
            Session.Current.SetTokenRights(sessionToken, user.rights);

            // Return session data to user.
            string query = string.Format("token={1}{0}expiryIn={2}{0}rights=",
                UniformQueries.API.SPLITTING_SYMBOL,
                sessionToken,
                Data.Config.Active.TokenValidTimeMinutes);

            // Add rights' codes.
            foreach(string rightsCode in user.rights)
            {
                // Add every code splited by '+'.
                query += "+" + rightsCode;
            }
            #endregion
            
            // Send token to client.
            UniformServer.BaseServer.SendAnswer(query, queryParts);
        }

        public bool IsTarget(QueryPart[] queryParts)
        {
            // Check query.
            if (!UniformQueries.API.QueryParamExist("USER", queryParts))
                return false;

            // Check query.
            if (!UniformQueries.API.QueryParamExist("LOGON", queryParts))
                return false;


            // Login for logon.
            if (!UniformQueries.API.QueryParamExist("login", queryParts))
                return false;

            // Password for logon.
            if (!UniformQueries.API.QueryParamExist("password", queryParts))
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
