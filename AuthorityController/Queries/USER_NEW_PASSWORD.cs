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
            // Get params.
            UniformQueries.API.TryGetParamValue("user",         out QueryPart user, queryParts);
            UniformQueries.API.TryGetParamValue("password",     out QueryPart password, queryParts);
            UniformQueries.API.TryGetParamValue("oldPassword",  out QueryPart oldPassword, queryParts);
            UniformQueries.API.TryGetParamValue("token",        out QueryPart token, queryParts);

            // TODO Check token rights.

            // TODO Validate old password.

            // TODO Sending data to server
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
    }
}
