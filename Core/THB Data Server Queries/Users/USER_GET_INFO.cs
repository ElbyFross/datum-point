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

namespace DatumPoint.Queries.Users
{
    /// <summary>
    /// Getting an information from the certain user profile.
    /// Relative to token rights will return different list of data.
    /// </summary>
    public class USER_GET_INFO : IQueryHandler
    {
        public string Description(string cultureKey)
        {
            throw new NotImplementedException();
        }

        public void Execute(object sender, Query query)
        {
            throw new NotImplementedException();
        }

        public bool IsTarget(Query query)
        {
            if (!query.QueryParamExist("info")) return false;
            if (!query.QueryParamExist("user")) return false;
            if (!query.QueryParamExist("get")) return false;
            return true;
        }
    }
}
