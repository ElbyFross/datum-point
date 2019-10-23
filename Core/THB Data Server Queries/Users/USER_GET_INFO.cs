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
    public class USER_GET_INFO : UniformedSqlGetQueryHandler
    {
        public override UserRank RankUperThen { get; set; } = UserRank.Moderator;
        public override string SharedObjectProperty { get; set; } = "get";
        public override Type TableType { get; set; } = typeof(Types.Personality.DPUser);
        public override string[] RequiredRights { get; set; } = null;

        public override ObjectHander Select
        {
            get
            {
                // TODO Implemet validation of rights by rights and relations preferences of target token.
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Returns array of properties for where bloc of sql query.
        /// </summary>
        public override ObjectHander Where
        {
            get
            {
                // Configurate where property in sql query.
                string[] SqlWhereConfiguration(object sharedData, QueryMeta meta)
                {
                    if (sharedData is Types.Personality.DPUser user)
                    {
                        return new string[] { user.id >= 0 ? "userid" : "login" };
                    }
                    else
                    {
                        Console.WriteLine("ERROR : USER INFO GET | Shared data miscast.");
                        return null;
                    }
                }
                return SqlWhereConfiguration;
            }
        }

        public override string Description(string cultureKey)
        {
            switch (cultureKey)
            {
                case "en-US":
                default:
                    return "USER INFO GET=[binary]\n" +
                            "\tDESCRIPTION: Return data for specified user." +
                            "In case if specified id then lookin using it. Use login field in other case.\n";
            }
        }

        public override bool IsTarget(Query query)
        {
            if (!query.QueryParamExist("info")) return false;
            if (!query.QueryParamExist("user")) return false;
            if (!query.QueryParamExist("get")) return false;
            return true;
        }
    }
}
