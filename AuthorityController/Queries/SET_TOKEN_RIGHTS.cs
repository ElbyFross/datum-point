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
    /// Change rights list to provided token.
    /// Require admin rights.
    /// </summary>
    public class SET_TOKEN_RIGHTS : IQueryHandlerProcessor
    {
        public string Description(string cultureKey)
        {
            throw new NotImplementedException();
        }

        public void Execute(QueryPart[] queryParts)
        {
            string[] requesterRights = null;

            #region Get fields from query
            // Get params.
            UniformQueries.API.TryGetParamValue("token", out QueryPart token, queryParts);
            UniformQueries.API.TryGetParamValue("targetToken", out QueryPart targetToken, queryParts);
            UniformQueries.API.TryGetParamValue("rights", out QueryPart rights, queryParts);
            #endregion

            #region Check requester rights
            try
            {
                // Check token rights.
                if (!API.Tokens.IsHasEnoughRigths(token, out requesterRights, 
                    Data.Config.Active.QUERY_SetTokenRights_RIGHTS))
                {
                    // Inform that rights not enought..
                    UniformServer.BaseServer.SendAnswer("ERROR 401: Unauthorized", queryParts);
                    return;
                }
            }
            catch
            {
                // Inform that token not registred.
                UniformServer.BaseServer.SendAnswer("ERROR 401: Token not found", queryParts);
                return;
            }
            #endregion

            #region Get target token rights
            if (Session.Current.TryGetTokenRights(targetToken.propertyValue, out string[] targetTokenRights))
            {
                // If also not found.
                UniformServer.BaseServer.SendAnswer("ERROR 404: User not found", queryParts);
                return;
            }
            #endregion

            #region Compare ranks
            // String that will contain instruction.
            string rankRequirmentsInstruction = null;
            // Find requester rank.
            foreach (string rr in requesterRights)
            {
                // Check if the rights is the "rank".
                if (rr.StartsWith("rank="))
                {
                    rankRequirmentsInstruction = rr;
                    break;
                }
            }

            // If requester rank not detected.
            if (string.IsNullOrEmpty(rankRequirmentsInstruction))
            {
                // Inform that rank not defined.
                UniformServer.BaseServer.SendAnswer("ERROR 401: User rank not defined", queryParts);
                return;
            }
            else
            {
                // Add modifier that will require from user higher rank the target.
                rankRequirmentsInstruction = "<" + rankRequirmentsInstruction;
            }

            // Check is the target user has the less rank then requester.
            if (!API.Tokens.IsHasEnoughRigths(targetTokenRights, rankRequirmentsInstruction))
            {
                // Inform that target user has the same or heigher rank then requester.
                UniformServer.BaseServer.SendAnswer("ERROR 401: Unauthorized", queryParts);
                return;
            }
            #endregion

            // Apply new rights
            string[] rightsArray = rights.propertyValue.Split('+');
            Session.Current.SetTokenRights(targetToken.propertyValue, rightsArray);
        }

        public bool IsTarget(QueryPart[] queryParts)
        { 
            // Request set property.
            if (!UniformQueries.API.QueryParamExist("set", queryParts))
                return false;

            // Token that will be a target in case if requester has enough rights to do this.
            if (!UniformQueries.API.QueryParamExist("targetToken", queryParts))
                return false;

            // List of rights' keys.
            if (!UniformQueries.API.QueryParamExist("rights", queryParts))
                return false;

            return true;
        }
    }
}
