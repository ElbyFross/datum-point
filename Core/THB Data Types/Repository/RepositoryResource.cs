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

namespace DatumPoint.Types.Repository
{
    /// <summary>
    /// Object that contain data that would stored in database or file system like and object container.
    /// </summary>
    [System.Serializable]
    [Table("datum-point", "repository")]
    public class RepositoryResource
    {
        /// <summary>
        /// Unique if of this container in repository.
        /// </summary>
        [Column("resourceid", System.Data.DbType.Int32), IsPrimaryKey, IsNotNull, IsAutoIncrement]
        [MySqlDBTypeOverride(MySql.Data.MySqlClient.MySqlDbType.Int32, "INT")]
        public int resourceId = -1;

        /// <summary>
        /// Data shared via container.
        /// </summary>
        [Column("data", System.Data.DbType.Binary)]
        [MySqlDBTypeOverride(MySql.Data.MySqlClient.MySqlDbType.LongBlob, "LONGBLOB")]
        public byte[] data = null;

        /// <summary>
        /// Name of this resource.
        /// </summary>
        [Column("name", System.Data.DbType.String), IsNotNull]
        [MySqlDBTypeOverride(MySql.Data.MySqlClient.MySqlDbType.VarChar, "VARCHAT(45)")]
        public string name = "New resource";

        /// <summary>
        /// Description of stored resource.
        /// </summary>
        [Column("description", System.Data.DbType.String)]
        [MySqlDBTypeOverride(MySql.Data.MySqlClient.MySqlDbType.VarChar, "VARCHAT(500)")]
        public string description = null;

        /// <summary>
        /// Id of user owner.
        /// </summary>
        [Column("user_ownerid", System.Data.DbType.Int32), IsNotNull, IsForeignKey("datum-point", "user", "userid")]
        [MySqlDBTypeOverride(MySql.Data.MySqlClient.MySqlDbType.Int32, "INT")]
        public int ownerID = -1;

        /// <summary>
        /// Time when object loaded at server.
        /// </summary>
        [Column("created_at", System.Data.DbType.DateTime), IsNotNull]
        public DateTime createdAt;

        /// <summary>
        /// When resource was called at last time.
        /// </summary>
        [Column("used_at", System.Data.DbType.DateTime), IsNotNull]
        public DateTime usedAt;

        /// <summary>
        /// Who can access to this file:
        /// 0 - author only.
        /// 1 - author's friends.
        /// 2 - author's local group.
        /// 3 - everyone by access link.
        /// 4 - everyone.
        /// </summary>
        [Column("sharing_rule", System.Data.DbType.Int32), IsNotNull]
        [MySqlDBTypeOverride(MySql.Data.MySqlClient.MySqlDbType.Int32, "INT")]
        public int sharingRule = 0;

        /// <summary>
        /// How many times this resource would called.
        /// </summary>
        [Column("views", System.Data.DbType.Int32), IsNotNull]
        [MySqlDBTypeOverride(MySql.Data.MySqlClient.MySqlDbType.Int32, "INT")]
        public int views = 0;
    }
}
