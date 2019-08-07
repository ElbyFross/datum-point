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

using System.Data.Common;
using UniformDataOperator.SQL.Tables;

namespace DatumPoint.Types.Orders
{
    /// <summary>
    /// Class that can be stored as query data's block to data base or file system.
    /// </summary>
    [System.Serializable]
    public class Order : ISQLTable, ISQLDataReadCompatible
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
        /// Binary timestamp of posting time.
        /// </summary>
        public long postingTime;

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
        public object sharedData = null;

        // TODO Implement required
        public string TableName => throw new System.NotImplementedException();

        // TODO Implement required
        public TableFieldMeta[] TableFields => throw new System.NotImplementedException();

        // TODO Implement required
        public void ReadSQLObject(DbDataReader reader)
        {
            throw new System.NotImplementedException();
        }
    }
}
