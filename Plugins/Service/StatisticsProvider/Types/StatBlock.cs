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
using System.Data.Common;
using UniformDataOperator.Sql.Attributes;
using UniformDataOperator.Sql.MySql.Attributes;

namespace StatisticsProvider.Types
{
    /// <summary>
    /// Statistic block that contain recorded data about time period.
    /// </summary>
    [System.Serializable]
    [Table("datum-point", "stat_block")]
    public class StatBlock
    {
        /// <summary>
        /// Unique ID of statistic block registred in DB.
        /// </summary>
        [Column("blockid", System.Data.DbType.Int32), IsPrimaryKey, IsNotNull, IsAutoIncrement]
        public int blockId = -1;

        /// <summary>
        /// ID of user that is a target for that statistic data.
        /// </summary>
        [Column("user_userid", System.Data.DbType.Int32), IsForeignKey("datum-point", "user", "userid")]
        public int targetUserId = -1;

        /// <summary>
        /// Assessment of this session. In case of not atomar session it would be average value.
        /// </summary>
        [Column("assessment", System.Data.DbType.Int32)]
        public int assessment;

        /// <summary>
        /// Difference between highest and lowes assessment during session. Zero for atomar session.
        /// </summary>
        [Column("assessments_delta", System.Data.DbType.Int32)]
        public int assessmentsDelta;

        /// <summary>
        /// Mask that show behevior in skiping of sessions frequency of sessions leveng during one period. 
        /// Example: 16+7+3+3+2+4+9+14 - this mask show that student has strong tend to skip first and last two lessons.
        /// </summary>
        [Column("abcences_day_scale_mask", System.Data.DbType.String)]
        public string abcencesDayScaleMask;

        /// <summary>
        /// Mask that show behevior in skiping of sessions frequency of sessions leveng during one period. 
        /// Example: 3+1+2+3+4+8+0+0 - The mask of absence relative to week's days. It showing that student has tend to absent at friday.
        /// </summary>
        [Column("abcences_week_scale_mask", System.Data.DbType.String)]
        public string abcencesWeekScaleMask;
    }
}
