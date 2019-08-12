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
using UniformDataOperator.SQL.Tables;

namespace StatisticsProvider.Types
{
    /// <summary>
    /// Statistic block that contain recorded data about time period.
    /// </summary>
    [System.Serializable]
    public class StatBlock : ISQLTable, ISQLDataReadCompatible
    {
        /// <summary>
        /// Unique ID of statistic block registred in DB.
        /// </summary>
        public int blockId = -1;

        /// <summary>
        /// ID of user that is a target for that statistic data.
        /// </summary>
        public int targetUserId = -1;

        /// <summary>
        /// Assessment of this session. In case of not atomar session it would be average value.
        /// </summary>
        public int assessment;

        /// <summary>
        /// Difference between highest and lowes assessment during session. Zero for atomar session.
        /// </summary>
        public int assessmentsDelta;

        /// <summary>
        /// Mask that show behevior in skiping of sessions frequency of sessions leveng during one period. 
        /// Example: 16+7+3+3+2+4+9+14 - this mask show that student has strong tend to skip first and last two lessons.
        /// </summary>
        public string abcencesDayScaleMask;

        /// <summary>
        /// Mask that show behevior in skiping of sessions frequency of sessions leveng during one period. 
        /// Example: 3+1+2+3+4+8+0+0 - The mask of absence relative to week's days. It showing that student has tend to absent at friday.
        /// </summary>
        public string abcencesWeekScaleMask;



        public string TableName
        {
            get { return "stat_block"; }
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
                            name = "blockid",
                            type = "INT",
                            isPrimaryKey = true,
                            isNotNull = true,
                            isAutoIncrement = true
                        },
                        new TableColumnMeta()
                        {
                            name = "user_userid",
                            type = "int",
                            isNotNull = true,
                            isForeignKey = true,
                            refSchema = "datum-point",
                            refTable = "user",
                            refColumn = "userid"
                        },
                        new TableColumnMeta()
                        {
                            name = "assessment",
                            type = "INT"
                        },
                        new TableColumnMeta()
                        {
                            name = "assessments_delta",
                            type = "INT"
                        },
                        new TableColumnMeta()
                        {
                            name = "abcences_day_scale_mask",
                            type = "VARCHAR(45)",
                            isNotNull = true
                        },
                        new TableColumnMeta()
                        {
                            name = "abcences_week_scale_mask",
                            type = "VARCHAR(45)"
                        }
                    };
                }
                return _TableFields;
            }
        }
        protected TableColumnMeta[] _TableFields;

        public void ReadSQLObject(DbDataReader reader)
        {
            try { blockId = reader.GetInt32(reader.GetOrdinal("blockId")); } catch { };
            try { targetUserId = reader.GetInt32(reader.GetOrdinal("user_userid")); } catch { };

            try { assessment = reader.GetInt32(reader.GetOrdinal("assessment")); } catch { };
            try { assessmentsDelta = reader.GetInt32(reader.GetOrdinal("assessments_delta")); } catch { };

            try { abcencesDayScaleMask = reader.GetString(reader.GetOrdinal("abcences_day_scale_mask")); } catch { };
            try { abcencesWeekScaleMask = reader.GetString(reader.GetOrdinal("abcences_week_scale_mask")); } catch { };
        }
    }
}
