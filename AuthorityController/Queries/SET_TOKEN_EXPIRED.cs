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
    /// Set certain token expired.
    /// Require system rights.
    /// </summary>
    public class SET_TOKEN_EXPIRED : IQueryHandlerProcessor
    {
        public string Description(string cultureKey)
        {
            throw new NotImplementedException();
        }

        public void Execute(QueryPart[] queryParts)
        {
            // Get params.
            UniformQueries.API.TryGetParamValue("token", out QueryPart token, queryParts);
            UniformQueries.API.TryGetParamValue("expiredToken", out QueryPart expiredToken, queryParts);

            // TODO Check ritghts.

            Session.Current.SetExpired(expiredToken);
        }

        public bool IsTarget(QueryPart[] queryParts)
        {
            // Request set property.
            if (!UniformQueries.API.QueryParamExist("set", queryParts))
                return false;

            // Token that will expired in case if requester has enough rights to do this.
            if (!UniformQueries.API.QueryParamExist("expiredToken", queryParts))
                return false;

            return true;
        }
    }
}
