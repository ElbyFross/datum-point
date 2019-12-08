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

namespace DatumPoint.Queries.Schedule
{
    /// <summary>
    /// Returns data relative to the certen period in days.
    /// </summary>
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

        public void Execute(object sender, Query query)
        {
            throw new NotImplementedException();
        }

        public bool IsTarget(Query query)
        {
            if (!query.QueryParamExist("GET")) return false;
            if (!query.QueryParamExist("DAYSRANGE")) return false;
            return true;
        }
    }
}
