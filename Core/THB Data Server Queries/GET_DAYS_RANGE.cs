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
using System.Text;
using UniformQueries;
using UniformQueries.Executable;

namespace DatumPoint.Queries
{
    public class GET_DAYS_RANGE : IQueryHandler
    {
        public string Description(string cultureKey)
        {
            switch(cultureKey)
            {
                case "en-US":
                default:
                    return "GET DAYSRANGE DD:MM:YYYY DD:MM:YYYY\n" +
                            "\tDESCRIPTION: Will return List of days with shedule reletive to orders.\n" +
                            "\tQUERY FORMAT: q=GET" + API.SPLITTING_SYMBOL + 
                            "sq=DAYSRANGE" + API.SPLITTING_SYMBOL + 
                            "f=DD:MM:YYYY" + API.SPLITTING_SYMBOL + 
                            "t=DD:MM:YYYY\n";
            }
        }

        public void Execute(QueryPart[] queryParts)
        {
            throw new NotImplementedException();
        }

        public bool IsTarget(QueryPart[] queryParts)
        {
            if (API.TryGetParamValue("q", out QueryPart query, queryParts)) return false;

            // Save the time and avoid if query even not hace subquery.
            if (!API.TryGetParamValue("sq", out QueryPart subQuery, queryParts)) return false;

            // Comare q and sq with target.
            bool comparison =
                query.ParamNameEqual("GET") &&
                subQuery.ParamNameEqual("DAYSRANGE");

            // Return result of compression.
            return comparison;
        }
    }
}
