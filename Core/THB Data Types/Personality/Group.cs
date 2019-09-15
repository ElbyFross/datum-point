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
using System.Data.Common;
using System.Globalization;
using UniformDataOperator.Sql.Attributes;
using UniformDataOperator.Sql.MySql.Attributes;

namespace DatumPoint.Types.Personality
{
    /// <summary>
    /// Object that contains data that describing group.
    /// </summary>
    [System.Serializable]
    [Table("datum-point", "group")]
    public class Group
    {
        /// <summary>
        /// Unique id of this group.
        /// </summary>
        [Column("groupid", System.Data.DbType.Int32), IsPrimaryKey, IsNotNull, IsAutoIncrement]
        [MySqlDBTypeOverride(MySql.Data.MySqlClient.MySqlDbType.Int32, "INT")]
        public int groupId = -1;

        /// <summary>
        /// Title of this group.
        /// </summary>
        [Column("title", System.Data.DbType.String), IsNotNull]
        [MySqlDBTypeOverride(MySql.Data.MySqlClient.MySqlDbType.VarChar, "VARCHAT(45)")]
        public string title = null;

        /// <summary>
        /// Language culture code in string format suitable for data base duplex data exchange.
        /// </summary>
        [Column("language", System.Data.DbType.String)]
        [MySqlDBTypeOverride(MySql.Data.MySqlClient.MySqlDbType.VarChar, "VARCHAT(7)")]
        [System.Xml.Serialization.XmlIgnore]
        public string LanguageCode
        {
            get
            {
                return language?.Name;
            }
            set
            {
                language = new CultureInfo(value);
            }
        }

        /// <summary>
        /// Primary language of this group.
        /// </summary>
        public CultureInfo language;

        /// <summary>
        /// Id of president of this group.
        /// Detailed history can be gained via orders.
        /// </summary>
        [Column("presidentid", System.Data.DbType.String), IsNotNull]
        [IsForeignKey("datum-point", "user", "userid")]
        [MySqlDBTypeOverride(MySql.Data.MySqlClient.MySqlDbType.Int32, "INT")]
        public int presidentId = -1;

        /// <summary>
        /// Id of group's superviser user.
        /// Detailed history can be gained via orders.
        /// </summary>
        [Column("superviserid", System.Data.DbType.String), IsNotNull]
        [IsForeignKey("datum-point", "user", "userid")]
        [MySqlDBTypeOverride(MySql.Data.MySqlClient.MySqlDbType.Int32, "INT")]
        public int superviserid = -1;
    }
}
