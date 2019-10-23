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
    /// Setting the new order.
    /// </summary>
    class ORDER_SET : UniformedSqlSetQueryHandler
    {
        public override UserRank RankUperThen { get; set; } = UserRank.User;
        public override string SharedObjectProperty { get; set; } = "set";
        public override Type TableType { get; set; } = typeof(Types.Orders.Order);
        public override string[] RequiredRights { get; set; } = new string[] { "ordersManagment" };

        public override string Description(string cultureKey)
        {
            return "ORDER SET=[binary]\n" +
                "\tDESCRIPTION:" +
                "Set new or update existed order.\n" +
                "\tQUERY FORMAT: order property must contain binary serialized " +
                "`" + TableType.FullName + "` object that will applied to the database ot local storage.\n" +
                "\tREQUIRMENTS: User must has `PrivilegedUser` right.";
        }

        public override bool IsTarget(Query query)
        {
            if (!query.QueryParamExist("order")) return false;
            if (!query.QueryParamExist("set")) return false;
            return true;
        }
    }
}
