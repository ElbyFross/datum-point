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
    /// Registrate token with guest rights in the system and return to client.
    /// </summary>
    class GET_GUEST_TOKEN : IQueryHandlerProcessor
    {
        public string Description(string cultureKey)
        {
            throw new NotImplementedException();
        }

        public void Execute(QueryPart[] queryParts)
        {
            // Get free token.
            string sessionToken = API.Tokens.UnusedToken;

            // Registrate token with guest rank.
            Session.Current.SetTokenRights(sessionToken, new string[] { "rank=0" });

            // Return session data to user.
            string query = string.Format("token={1}{0}expiryIn={2}{0}rights=rank=0",
                UniformQueries.API.SPLITTING_SYMBOL,
                sessionToken,
                Data.Config.Active.TokenValidTimeMinutes);

            // Send token to client.
            UniformServer.BaseServer.SendAnswer(query, queryParts);
        }

        public bool IsTarget(QueryPart[] queryParts)
        {
            if (!UniformQueries.API.QueryParamExist("get", queryParts))
                return false;

            if (!UniformQueries.API.QueryParamExist("guest", queryParts))
                return false;

            if (!UniformQueries.API.QueryParamExist("token", queryParts))
                return false;

            return true;
        }
    }
}
