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
    public class Group
    {
        /// <summary>
        /// Unique id of this group.
        /// </summary>
        public int groupId = -1;

        /// <summary>
        /// Title of this group.
        /// </summary>
        public string title = null;

        /// <summary>
        /// Primari language of this group.
        /// </summary>
        public CultureInfo language;

        /// <summary>
        /// Id of president of this group.
        /// Detailed history can be gained via orders.
        /// </summary>
        public int presidentId = -1;

        /// <summary>
        /// Id of group's superviser user.
        /// Detailed history can be gained via orders.
        /// </summary>
        public int superviserid = -1;

        public string TableName
        {
            get { return "group"; }
        }

        public string SchemaName
        {
            get { return "datum-point"; }
        }

        public string TableEngine
        {
            get { return "InnoDB"; }
        }

        public TableColumnMeta[] TableFields
        {
            get
            {
                // Init field if not init.
                if (_TableFields == null)
                {
                    _TableFields = new TableColumnMeta[]
                    {
                        new TableColumnMeta()
                        {
                            name = "groupid",
                            type = "INT",
                            isPrimaryKey = true,
                            isNotNull = true
                        },
                        new TableColumnMeta()
                        {
                            name = "title",
                            type = "VARCHAR(150)",
                            isNotNull = true
                        },
                        new TableColumnMeta()
                        {
                            name = "language",
                            type = "VARCHAR(7)",
                        },
                        new TableColumnMeta()
                        {
                            name = "presidentid",
                            type = "int",
                            isNotNull = true,
                            isForeignKey = true,
                            refSchema = "datum-point",
                            refTable = "user",
                            refColumn = "userid"
                        },
                        new TableColumnMeta()
                        {
                            name = "superviserid",
                            type = "int",
                            isNotNull = true,
                            isForeignKey = true,
                            refSchema = "datum-point",
                            refTable = "user",
                            refColumn = "userid"
                        }
                    };
                }
                return _TableFields;
            }
        }
        protected TableColumnMeta[] _TableFields;

        public void ReadSQLObject(DbDataReader reader)
        {
            try { groupId = reader.GetInt32(reader.GetOrdinal("groupid")); } catch { };
            try { title = reader.GetString(reader.GetOrdinal("title")); } catch { };

            try { language = new CultureInfo(reader.GetString(reader.GetOrdinal("language"))); } catch { };

            try { presidentId = reader.GetInt32(reader.GetOrdinal("presidentid")); } catch { };
            try { superviserid = reader.GetInt32(reader.GetOrdinal("superviserid")); } catch { };
        }
    }
}
