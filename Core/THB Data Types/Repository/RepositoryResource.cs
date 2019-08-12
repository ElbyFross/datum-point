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
using UniformDataOperator.SQL.Tables;

namespace DatumPoint.Types.Repository
{
    /// <summary>
    /// Object that contain data that would stored in database or file system like and object container.
    /// </summary>
    [System.Serializable]
    public class RepositoryResource : ISQLTable, ISQLDataReadCompatible
    {
        /// <summary>
        /// Unique if of this container in repository.
        /// </summary>
        public int resourceId = -1;

        /// <summary>
        /// Data shared via container.
        /// </summary>
        public byte[] data = null;

        /// <summary>
        /// Name of this resource.
        /// </summary>
        public string name = "New resource";

        /// <summary>
        /// Description of stored resource.
        /// </summary>
        public string description = null;

        /// <summary>
        /// Id of user owner.
        /// </summary>
        public int ownerID = -1;

        /// <summary>
        /// Time when object loaded at server.
        /// </summary>
        public DateTime createdAt;

        /// <summary>
        /// When resource was called at last time.
        /// </summary>
        public DateTime usedAt;

        /// <summary>
        /// Who can access to this file:
        /// 0 - author only.
        /// 1 - author's friends.
        /// 2 - author's local group.
        /// 3 - everyone by access link.
        /// 4 - everyone.
        /// </summary>
        public int sharingRule = 0;

        /// <summary>
        /// How many times this resource would called.
        /// </summary>
        public int views = 0;


        public string TableName
        {
            get { return "repository"; }
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
                            name = "resourceid",
                            type = "INT",
                            isPrimaryKey = true,
                            isNotNull = true,
                            isAutoIncrement = true
                        },
                        new TableColumnMeta()
                        {
                            name = "data",
                            type = "LONGBLOB",
                        },
                        new TableColumnMeta()
                        {
                            name = "name",
                            type = "VARCHAR(45)",
                            isNotNull = true
                        },
                        new TableColumnMeta()
                        {
                            name = "description",
                            type = "VARCHAR(500)"
                        },
                        new TableColumnMeta()
                        {
                            name = "user_ownerid",
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
                            name = "sharing_rule",
                            type = "INT",
                            isNotNull = true
                        },
                        new TableColumnMeta()
                        {
                            name = "views",
                            type = "INT",
                            isNotNull = true
                        },
                        new TableColumnMeta()
                        {
                            name = "used_at",
                            type = "DATETIME",
                            isNotNull = true
                        }
                    };
                }
                return _TableFields;
            }
        }
        protected TableColumnMeta[] _TableFields;

        public void ReadSQLObject(DbDataReader reader)
        {
            try { resourceId = reader.GetInt32(reader.GetOrdinal("resourceid")); } catch { };
            try { ownerID = reader.GetInt32(reader.GetOrdinal("user_ownerid")); } catch { };
            try { sharingRule = reader.GetInt32(reader.GetOrdinal("sharing_rule")); } catch { };
            try { views = reader.GetInt32(reader.GetOrdinal("views")); } catch { };

            try { name = reader.GetString(reader.GetOrdinal("name")); } catch { };
            try { description = reader.GetString(reader.GetOrdinal("description")); } catch { };

            try { byte[] data = reader["data"] as byte[]; } catch { };

            try { createdAt = reader.GetDateTime(reader.GetOrdinal("created_at")); } catch { };
            try { usedAt = reader.GetDateTime(reader.GetOrdinal("used_at")); } catch { };
        }
    }
}
