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
using UniformDataOperator.SQL.Tables;

namespace DatumPoint.Types.Schedule
{
    /// <summary>
    /// Object that contains data that describing group.
    /// </summary>
    [System.Serializable]
    public class Group : ISQLTable, ISQLDataReadCompatible
    {
        /// <summary>
        /// Unique id of this group.
        /// </summary>
        public int id = -1;

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
