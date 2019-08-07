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

namespace DatumPoint.Types.Schedule
{
    /// <summary>
    /// Attomar object that descibe min block of the schedule - a lesson.
    /// </summary>
    [System.Serializable]
    public class Lesson : ISQLDataReadCompatible
    {
        /// <summary>
        /// Id of subject that would be on this lesson.
        /// </summary>
        public int subjectId;

        /// <summary>
        /// Id of user that would be a teacher on this lesson.
        /// </summary>
        public int teacherId;

        /// <summary>
        /// Array that contains IDs of groups.
        /// </summary>
        public int[] groupIds;

        /// <summary>
        /// Array that contains IDs of resources in repository that binded to this lesson.
        /// </summary>
        public int[] resourceIds;

        // TODO Implement required
        public void ReadSQLObject(DbDataReader reader)
        {
            throw new System.NotImplementedException();
        }
    }
}
