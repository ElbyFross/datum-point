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
using DatumPoint.Types.Personality;
using DatumPoint.Queries.Handlers;

namespace DatumPoint.Queries.Users
{
    /// <summary>
    /// Getting an information from the certain user profile.
    /// Relative to token rights will return different list of data.
    /// </summary>
    public class USER_GET_INFO : Handlers.UniformedSqlGetQueryHandler
    {
        public override UserRank RankUperThen { get; set; } = UserRank.Guest;
        public override string SharedObjectProperty { get; set; } = "user";
        public override Type TableType { get; set; } = typeof(User);
        public override string[] RequiredRights { get; set; } = null;
        
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
                    if (sharedData is User user)
                    {
                        return new string[] { user.id >= 1 ? "userid" : "login" };
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

        public override Action<object, QueryMeta> PreComplete
        {
            get
            {
                // Configurate select property in sql query.
                return delegate (object sharedData, QueryMeta meta)
                {
                    if (meta.TokenInfo == null)
                    {
                        Console.WriteLine("ERROR : USER INFO GET | TOKEN not registred.");
                        return;
                    }

                    if (sharedData is User user)
                    {
                        // Drop personal data.
                        user.password = null;
                        user.tokens = new List<string>();

                        // Drop data allowed only to user themself.
                        if (meta.TokenInfo.userId != user.id)
                        {
                            user.phone = null;
                            user.email = null;
                        }
                    }
                    else
                    {
                        Console.WriteLine("ERROR : USER INFO GET | Shared data miscast.");
                        return;
                    }
                };
            }
        }

        public override string Description(string cultureKey)
        {
            switch (cultureKey)
            {
                case "en-US":
                default:
                    return "USER=[binary] INFO GET\n" +
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
