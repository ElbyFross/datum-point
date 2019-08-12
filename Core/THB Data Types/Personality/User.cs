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
using System.Text;
using UniformDataOperator.SQL.Tables;
using AuthorityController.Data.Personal;

namespace DatumPoint.Types.Personality
{
    /// <summary>
    /// Additive fields and API suiteble for AC users in Datum Point.
    /// </summary>
    [System.Serializable]
    public partial class DPUser : User, ISQLTable, ISQLDataReadCompatible
    {
        /// <summary>
        /// Middle name of user if applicable.
        /// </summary>
        public string middleName = null;

        /// <summary>
        /// Birthday of user.
        /// </summary>
        public DateTime birthday;

        /// <summary>
        /// Personal phone number of the user.
        /// </summary>
        public string phone = null;

        /// <summary>
        /// Personal e-mail of user.
        /// </summary>
        public string email = null;

        /// <summary>
        /// Id of current group.
        /// </summary>
        public int groupId = -1;

        /// <summary>
        /// Id of gender.
        /// </summary>
        public int genderId = -1;

        /// <summary>
        /// List of culture codes that prefered by this user.
        /// In order of priority.
        /// 
        /// Define what a UI language will selected after user login.
        /// Useful in multicultural environment like universities.
        /// </summary>
        public List<string> culturePreferences = new List<string>();

        /// <summary>
        /// When profile created.
        /// </summary>
        public DateTime createdAt;

        #region SQL handlers
        public string TableName
        {
            get { return "user"; }
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
                            name = "userid",
                            type = "INT",
                            isPrimaryKey = true,
                            isNotNull = true,
                            isAutoIncrement = true
                        },
                        new TableColumnMeta()
                        {
                            name = "login",
                            type = "VARCHAR(45)",
                            isNotNull = true
                        },
                        new TableColumnMeta()
                        {
                            name = "password",
                            type = "BLOB(512)",
                            isNotNull = true
                        },
                        new TableColumnMeta()
                        {
                            name = "firstname",
                            type = "VARCHAR(45)",
                            isNotNull = true
                        },
                        new TableColumnMeta()
                        {
                            name = "middlename",
                            type = "VARCHAR(45)"
                        },
                        new TableColumnMeta()
                        {
                            name = "lastname",
                            type = "VARCHAR(45)",
                            isNotNull = true
                        },
                        new TableColumnMeta()
                        {
                            name = "birthday",
                            type = "DATE"
                        },
                        new TableColumnMeta()
                        {
                            name = "phone",
                            type = "VARCHAR(15)"
                        },
                        new TableColumnMeta()
                        {
                            name = "email",
                            type = "VARCHAR(45)"
                        },
                        new TableColumnMeta()
                        {
                            name = "culture_preferences_order",
                            type = "VARCHAR(15)"
                        },
                        new TableColumnMeta()
                        {
                            name = "gender_idgender",
                            type = "INT",
                            isNotNull = true,
                            isForeignKey = true,
                            refSchema = "datum-point",
                            refTable = "gender",
                            refColumn = "genderid"
                        },
                        new TableColumnMeta()
                        {
                            name = "group_idgroup",
                            type = "INT",
                            isNotNull = true,
                            isForeignKey = true,
                            refSchema = "datum-point",
                            refTable = "group",
                            refColumn = "groupid"
                        },
                        new TableColumnMeta()
                        {
                            name = "rights",
                            type = "VARCHAR(1000)"
                        },
                        new TableColumnMeta()
                        {
                            name = "bans",
                            type = "TYNYBLOB"
                        },
                        new TableColumnMeta()
                        {
                            name = "created_at",
                            type = "DATETIME"
                        }
                    };
                }
                return _TableFields;
            }
        }
        protected TableColumnMeta[] _TableFields;

        public string SchemaName
        {
            get { return "datum-point"; }
        }

        public string TableEngine
        {
            get { return "InnoDB"; }
        }

        /// <summary>
        /// Read object data from DB data reader.
        /// </summary>
        /// <param name="reader"></param>
        public void ReadSQLObject(DbDataReader reader)
        {
            try { id = (uint)reader.GetInt32(reader.GetOrdinal("userid")); } catch { };

            try { login = reader.GetString(reader.GetOrdinal("login")); } catch { };
            try { password = reader["password"] as byte[]; } catch { };

            try { firstName = reader.GetString(reader.GetOrdinal("firstname")); } catch { };
            try { middleName = reader.GetString(reader.GetOrdinal("middlename")); } catch { };
            try { secondName = reader.GetString(reader.GetOrdinal("lastname")); } catch { };

            try { phone = reader.GetString(reader.GetOrdinal("phone")); } catch { };
            try { email = reader.GetString(reader.GetOrdinal("email")); } catch { };

            try { culturePreferences = new List<string>(((string)reader["culture_preferences_order"]).Split('+')); } catch { };

            try { genderId = reader.GetInt32(reader.GetOrdinal("gender_idgender")); } catch { };
            try { groupId = reader.GetInt32(reader.GetOrdinal("group_idgroup")); } catch { };

            try { rights = reader.GetString(reader.GetOrdinal("rights")).Split('+'); } catch { };

            try
            {
                byte[] bansBinary = reader["bans"] as byte[];
                if (bansBinary != null)
                {
                    bans = UniformDataOperator.Binary.BinaryHandler.FromByteArray<List<BanInformation>>(bansBinary);
                }
            }
            catch { };

            try { birthday = reader.GetDateTime(reader.GetOrdinal("birthday")); } catch { };
            try { createdAt = reader.GetDateTime(reader.GetOrdinal("created_at")); } catch { };
        }
        #endregion
    }
}
