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
        [Column("orderid", System.Data.DbType.Int32), IsPrimaryKey, IsNotNull, IsAutoIncrement]
        [MySqlDBTypeOverride(MySql.Data.MySqlClient.MySqlDbType.Int32, "INT")]
        public int orderId = -1;

        /// <summary>
        /// ID of user that had posted this order.
        /// </summary>
        [Column("user_userid", System.Data.DbType.String), IsNotNull]
        [IsForeignKey("datum-point", "user", "userid")]
        [MySqlDBTypeOverride(MySql.Data.MySqlClient.MySqlDbType.Int32, "INT")]
        public int postedUserId = -1;

        /// <summary>
        /// Time when will had been created.
        /// </summary>
        [Column("created_at", System.Data.DbType.DateTime), IsNotNull]
        public DateTime createdAt;

        /// <summary>
        /// Title of this order.
        /// </summary>
        [Column("title", System.Data.DbType.String)]
        [MySqlDBTypeOverride(MySql.Data.MySqlClient.MySqlDbType.VarChar, "VARCHAR(150)")]
        public string title = "New order";

        /// <summary>
        /// Description added by user to this order.
        /// </summary>
        [Column("description", System.Data.DbType.String)]
        [MySqlDBTypeOverride(MySql.Data.MySqlClient.MySqlDbType.VarChar, "VARCHAR(1024)")]
        public string description = null;

        /// <summary>
        /// Command descriptor that could be decomposed by regex to determine of high end instructions.
        /// </summary>
        [Column("command", System.Data.DbType.String)]
        [MySqlDBTypeOverride(MySql.Data.MySqlClient.MySqlDbType.VarChar, "VARCHAR(128)")]
        public string command;

        /// <summary>
        /// Data attached to this command.
        /// Can be binary or serizlised object.
        /// Max size 8196 bytes.
        /// </summary>
        [Column("shared_data", System.Data.DbType.Binary)]
        [MySqlDBTypeOverride(MySql.Data.MySqlClient.MySqlDbType.Blob, "BLOB(8196)")]
        public byte[] sharedData = null;
    }
}
