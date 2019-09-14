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

namespace DatumPoint.Types.Schedule
{
    /// <summary>
    /// Attomar object that descibe min block of the schedule - a lesson.
    /// </summary>
    [System.Serializable]
    [Table("datum-point", "lesson")]
    public class Lesson
    {
        /// <summary>
        /// Row index of lesson.
        /// </summary>
        [Column("lessonindex", System.Data.DbType.Int32), IsPrimaryKey, IsNotNull]
        public int index;

        /// <summary>
        /// Date of the day with that lesson.
        /// </summary>
        [Column("day_date", System.Data.DbType.Date), IsNotNull, IsForeignKey("datum-point", "day", "date")]
        public DateTime dayDate;

        /// <summary>
        /// Id of subject that would be on this lesson.
        /// </summary>
        [Column("subjectId", System.Data.DbType.Int32), IsNotNull]
        public int subjectId;

        /// <summary>
        /// Id of statistic block applied to that lesson.
        /// </summary>
        [Column("day_date", System.Data.DbType.Date), IsForeignKey("datum-point", "day", "date", IsForeignKey.Action.Cascade, IsForeignKey.Action.Cascade)]
        public int statBlockId;

        /// <summary>
        /// TODO Add connection to database.
        /// Id of user that would be a teacher on this lesson.
        /// </summary>
        public int teacherId;

        /// <summary>
        /// TODO Add connection to database.
        /// Array that contains IDs of groups.
        /// </summary>
        public int[] groupIds;

        /// <summary>
        /// TODO Add connection to database.
        /// Array that contains IDs of resources in repository that binded to this lesson.
        /// </summary>
        public int[] resourceIds;
    }
}
