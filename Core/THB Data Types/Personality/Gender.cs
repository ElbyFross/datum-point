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
using System.Text;
using System.Data.Common;
using UniformDataOperator.SQL.Tables;

namespace DatumPoint.Types.Personality
{
    /// <summary>
    /// User gender descriptor.
    /// </summary>
    [System.Serializable]
    public class Gender : ISQLTable, ISQLDataReadCompatible
    {
        /// <summary>
        /// Id of gender in collection.
        /// </summary>
        public int genderId = 0;

        /// <summary>
        /// Title that would displayed in application.
        /// 
        /// Atention: Recommend to use name of field in XAML dictionary to provide localization possibility.
        /// Example: gender_id0
        /// </summary>
        public string title = "Undefined";

        /// <summary>
        /// Prefix that would be put before name.
        /// 
        /// Atention: Recommend to use name of field in XAML dictionary to provide localization possibility.
        /// Example: gender_id0_prefix
        /// </summary>
        public string prefix = "";



        public string TableName
        {
            get { return "gender"; }
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
                            name = "genderid",
                            type = "INT",
                            isPrimaryKey = true,
                            isNotNull = true
                        },
                        new TableColumnMeta()
                        {
                            name = "title",
                            type = "VARCHAR(45)",
                            isNotNull = true
                        },
                        new TableColumnMeta()
                        {
                            name = "prefix",
                            type = "VARCHAR(5)",
                            isNotNull = true
                        }
                    };
                }
                return _TableFields;
            }
        }
        protected TableColumnMeta[] _TableFields;

        public void ReadSQLObject(DbDataReader reader)
        {
            try { genderId = reader.GetInt32(reader.GetOrdinal("genderid")); } catch { };
            try { title = reader.GetString(reader.GetOrdinal("title")); } catch { };
            try { prefix = reader.GetString(reader.GetOrdinal("prefix")); } catch { };
        }
    }
}
