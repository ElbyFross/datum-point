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
using System.Data.Common;
using UniformDataOperator.Sql.Attributes;
using UniformDataOperator.Sql.MySql.Attributes;

namespace DatumPoint.Types.Orders
{
    /// <summary>
    /// Class that can be stored as query data's block to data base or file system.
    /// </summary>
    [System.Serializable]
    public class Order
    {
        /// <summary>
        /// Unique id of this order.
        /// </summary>
        public int orderId = -1;

        /// <summary>
        /// ID of user that had posted this order.
        /// </summary>
        public int postedUserId = -1;

        /// <summary>
        /// Time when will had been created.
        /// </summary>
        public DateTime createdAt;

        /// <summary>
        /// Title of this order.
        /// </summary>
        public string title = "New order";

        /// <summary>
        /// Description added by user to this order.
        /// </summary>
        public string description = null;

        /// <summary>
        /// Command descriptor that could be decomposed by regex to determine of high end instructions.
        /// </summary>
        public string command;

        /// <summary>
        /// Data attached to this command.
        /// Can be binary or serizlised object.
        /// Max size 8196 bytes.
        /// </summary>
        public byte[] sharedData = null;

        public string TableName
        {
            get { return "order"; }
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
                            name = "orderid",
                            type = "INT",
                            isPrimaryKey = true,
                            isNotNull = true,
                            isAutoIncrement = true
                        },
                        new TableColumnMeta()
                        {
                            name = "user_userid",
                            type = "int",
                            isNotNull = true,
                            isForeignKey = true,
                            refSchema = "datum-point",
                            refTable = "user",
                            refColumn = "userid"
                        },
                        new TableColumnMeta()
                        {
                            name = "created_at",
                            type = "DATETIME",
                            isNotNull = true
                        },
                        new TableColumnMeta()
                        {
                            name = "title",
                            type = "VARCHAR(150)",
                        },
                        new TableColumnMeta()
                        {
                            name = "description",
                            type = "VARCHAR(1024)"
                        },
                        new TableColumnMeta()
                        {
                            name = "command",
                            type = "VARCHAR(128)"
                        },
                        new TableColumnMeta()
                        {
                            name = "shared_data",
                            type = "BLOB(8196)"
                        }
                    };
                }
                return _TableFields;
            }
        }
        protected TableColumnMeta[] _TableFields;

        public void ReadSQLObject(DbDataReader reader)
        {
            try { orderId = reader.GetInt32(reader.GetOrdinal("orderid")); } catch { };
            try { postedUserId = reader.GetInt32(reader.GetOrdinal("user_userid")); } catch { };
            try { createdAt = reader.GetDateTime(reader.GetOrdinal("created_at")); } catch { };

            try { title = reader.GetString(reader.GetOrdinal("title")); } catch { };
            try { description = reader.GetString(reader.GetOrdinal("description")); } catch { };
            try { command = reader.GetString(reader.GetOrdinal("command")); } catch { };

            try { byte[] sharedData = reader["shared_data"] as byte[]; } catch { };
        }
    }
}
