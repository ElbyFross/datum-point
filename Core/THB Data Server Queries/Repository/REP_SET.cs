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

namespace DatumPoint.Queries.Repository
{
    /// <summary>
    /// Setting a data to repository.
    /// </summary>
    public class REP_SET : Handlers.UniformedSqlSetQueryHandler
    {
        public override UserRank RankUperThen { get; set; } = UserRank.Guest;
        public override string SharedObjectProperty { get; set; } = "set";
        public override Type TableType { get; set; } = typeof(Types.Repository.RepositoryResource);
        public override string[] RequiredRights { get; set; } = new string[] { "repositoryPost" };

        public override string Description(string cultureKey)
        {
            return "SET=[binary] REP\n" +
                "\tDESCRIPTION:" +
                "Set new or update existed repository file.\n" +
                "\tQUERY FORMAT: rep property must contain binary serialized " +
                "`" + TableType.FullName + "` object that will applied to the database ot local storage.\n" +
                "\tREQUIRMENTS: User must has `User` right.";
        }

        public override bool IsTarget(Query query)
        {
            if (!query.QueryParamExist("rep")) return false;
            if (!query.QueryParamExist("set")) return false;
            return true;
        }
    }
}
