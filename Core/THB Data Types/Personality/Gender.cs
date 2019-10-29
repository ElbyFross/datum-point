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
using System.Data.Common;
using UniformDataOperator.Sql.Attributes;
using UniformDataOperator.Sql.MySql.Attributes;

namespace DatumPoint.Types.Personality
{
    /// <summary>
    /// User gender descriptor.
    /// </summary>
    [System.Serializable]
    [Table("datum-point", "gender")]
    public class Gender
    {
        /// <summary>
        /// Id of gender in collection.
        /// </summary>
        [Column("genderid", System.Data.DbType.Int32), IsPrimaryKey, IsNotNull, IsAutoIncrement]
        [MySqlDBTypeOverride(MySql.Data.MySqlClient.MySqlDbType.Int32, "INT")]
        public int genderId = -1;

        /// <summary>
        /// Title that would displayed in application.
        /// 
        /// Atention: Recommend to use name of field in XAML dictionary to provide localization possibility.
        /// Example: gender_id0
        /// </summary>
        [Column("title", System.Data.DbType.String), IsNotNull]
        [MySqlDBTypeOverride(MySql.Data.MySqlClient.MySqlDbType.VarChar, "VARCHAR(45)")]
        public string title = "Undefined";

        /// <summary>
        /// Prefix that would be put before name.
        /// 
        /// Atention: Recommend to use name of field in XAML dictionary to provide localization possibility.
        /// Example: gender_id0_prefix
        /// </summary>
        [Column("prefix", System.Data.DbType.String), IsNotNull]
        [MySqlDBTypeOverride(MySql.Data.MySqlClient.MySqlDbType.VarChar, "VARCHAR(45)")]
        public string prefix = "";
    }
}
