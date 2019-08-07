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
        public object data = null;

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
        public DateTime postTime;

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

        /// <summary>
        /// When resource was called at last time.
        /// </summary>
        public DateTime lastCallAt;

        // TODO Implement required
        public string TableName => throw new NotImplementedException();

        // TODO Implement required
        public TableFieldMeta[] TableFields => throw new NotImplementedException();

        // TODO Implement required
        public void ReadSQLObject(DbDataReader reader)
        {
            throw new NotImplementedException();
        }
    }
}
