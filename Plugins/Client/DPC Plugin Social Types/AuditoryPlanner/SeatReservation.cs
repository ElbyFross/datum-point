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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniformDataOperator.Sql.Attributes;
using UniformDataOperator.Sql.MySql.Attributes;

namespace DatumPoint.Plugins.Social.Types.AuditoryPlanner
{
    /// <summary>
    /// Class that provides data to seat reservation.
    /// </summary>
    [Table("datum-point", "SeatReservation")]
    [Serializable]
    public class SeatReservation
    {
        #region Fields & properties
        /// <summary>
        /// Id of user that reserve seat.
        /// </summary>
        [Column("userid", System.Data.DbType.Int32), IsNotNull]
        [MySqlDBTypeOverride(MySql.Data.MySqlClient.MySqlDbType.Int32, "INT")]
        [IsForeignKey("datum-point", "user", "userid")]
        public int userId = -1;

        /// <summary>
        /// Uniqume index of auditory.
        /// </summary>
        [Column("auditoriumid", System.Data.DbType.Int32), IsNotNull]
        [MySqlDBTypeOverride(MySql.Data.MySqlClient.MySqlDbType.Int32, "INT")]
        [IsForeignKey("datum-point", "auditorium", "auditoriumid")]
        public int auditoriumId = -1;

        /// <summary>
        /// Unique index of lesson that relative to that reservation.
        /// </summary>
        [Column("lessonindex", System.Data.DbType.Int32), IsNotNull]
        [MySqlDBTypeOverride(MySql.Data.MySqlClient.MySqlDbType.Int32, "INT")]
        [IsForeignKey("datum-point", "lesson", "lessonindex")]
        public int lessonIndex;

        /// <summary>
        /// Index of the seat on the schema of auditory.
        /// </summary>
        [Column("seatindex", System.Data.DbType.Int32), IsNotNull]
        [MySqlDBTypeOverride(MySql.Data.MySqlClient.MySqlDbType.Int32, "INT")]
        public int seatIndex = -1;
        #endregion

        #region API
        #endregion
    }
}
