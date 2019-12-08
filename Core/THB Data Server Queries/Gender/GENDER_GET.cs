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
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UniformQueries;
using UniformQueries.Executable;
using DatumPoint.Types.Personality;

namespace DatumPoint.Queries.Gender
{
    /// <summary>
    /// Returns the gender settigns by gender's id.
    /// </summary>
    public class GENDER_GET : Handlers.UniformedSqlGetQueryHandler
    {
        public override UserRank RankUperThen { get; set; } = UserRank.Moderator;
        public override string SharedObjectProperty { get; set; } = "get";
        public override Type TableType { get; set; } = typeof(Types.Personality.Gender);
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
                    if (sharedData is Types.Personality.Gender gender)
                    {
                        return new string[] { gender.genderId >= 0 ? "genderid" : "title" };
                    }
                    else
                    {
                        Console.WriteLine("ERROR : GENDER GET | Shared data miscast.");
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
                    return "GENDER GET=[binary]\n" +
                            "\tDESCRIPTION: Return data for specified Gender." +
                            "In case if specified id then lookin using it. Use title field in other case.\n";
            }
        }
        
        public override bool IsTarget(Query query)
        {
            if (!query.QueryParamExist("gender")) return false;
            if (!query.QueryParamExist("get")) return false;
            return true;
        }
    }
}
