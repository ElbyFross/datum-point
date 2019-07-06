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

            #region Validate password.
            // Convert password to hashed one.

            // Compare with stored.

            #endregion
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
