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

namespace DatumPoint.Queries.Orders
{
    /// <summary>
    /// TODO require processign by regex formated command.
    /// Looking for the details of order.
    /// </summary>
    class ORDER_GET : UniformedSqlGetQueryHandler
    {
        public override UserRank RankUperThen { get; set; } = UserRank.User;
        public override string SharedObjectProperty { get; set; } = "get";
        public override Type TableType { get; set; } = typeof(Types.Orders.Order);
        public override string[] RequiredRights { get; set; } = new string[] { "ordersReading" };

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
                    if (sharedData is Types.Orders.Order gender)
                    {
                        // TODO no implemented.
                        throw new NotImplementedException();
                    }
                    else
                    {
                        Console.WriteLine("ERROR : ORDER GET | Shared data miscast.");
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
                    return "ORDER GET=[binary]\n" +
                            "\tDESCRIPTION: Return data for specified Order." +
                            "In case if specified id then lookin using it. Use command field in other case.\n";
            }
        }

        public override bool IsTarget(Query query)
        {
            if (!query.QueryParamExist("order")) return false;
            if (!query.QueryParamExist("set")) return false;
            return true;
        }
    }
}
