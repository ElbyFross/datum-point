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
using UniformDataOperator.Sql.Attributes;
using UniformDataOperator.Sql.MySql.Attributes;
using AuthorityController.Data.Personal;

namespace DatumPoint.Types.Personality
{
    /// <summary>
    /// Additive fields and API suiteble for AC users in Datum Point.
    /// </summary>
    [System.Serializable]
    [Table("datum-point", "user")]
    public partial class DPUser : User
    {
        /// <summary>
        /// Middle name of user if applicable.
        /// </summary>
        [Column("middlename", System.Data.DbType.String)]
        [MySqlDBTypeOverride(MySql.Data.MySqlClient.MySqlDbType.VarChar, "VARCHAR(45)")]
        public string middleName = null;

        /// <summary>
        /// Birthday of user.
        /// </summary>
        [Column("birthday", System.Data.DbType.Date)]
        public DateTime birthday;

        /// <summary>
        /// Personal phone number of the user.
        /// </summary>
        [Column("phone", System.Data.DbType.String)]
        [MySqlDBTypeOverride(MySql.Data.MySqlClient.MySqlDbType.VarChar, "VARCHAR(15)")]
        public string phone = null;

        /// <summary>
        /// Personal e-mail of user.
        /// </summary>
        [Column("email", System.Data.DbType.String)]
        [MySqlDBTypeOverride(MySql.Data.MySqlClient.MySqlDbType.VarChar, "VARCHAR(45)")]
        public string email = null;

        /// <summary>
        /// Id of current group.
        /// </summary>
        [Column("gender_idgender", System.Data.DbType.Int32)]
        [MySqlDBTypeOverride(MySql.Data.MySqlClient.MySqlDbType.Int32, "INT")]
        public int groupId = -1;

        /// <summary>
        /// Id of gender.
        /// </summary>
        [Column("group_idgroup", System.Data.DbType.Int32)]
        [MySqlDBTypeOverride(MySql.Data.MySqlClient.MySqlDbType.Int32, "INT")]
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
        /// Culture preferences of user in string format suitable to duplex exchange with database.
        /// </summary>
        [Column("culture_preferences_order", System.Data.DbType.String)]
        [MySqlDBTypeOverride(MySql.Data.MySqlClient.MySqlDbType.VarChar, "VARCHAR(15)")]
        [System.Xml.Serialization.XmlIgnore]
        public string CulturePreferencesString
        {
            get
            {
                string output = "";
                // Add all codes to string.
                foreach(string code in culturePreferences)
                {
                    // Add spliter if not first one.
                    if(!string.IsNullOrEmpty(output))
                    {
                        output += "+";
                    }

                    // Add code to string.
                    output += code;
                }
                return output;
            }
            set
            {
                // Create list with string splited by special symbol.
                culturePreferences = new List<string>(value?.Split('+'));
            }
        }

        /// <summary>
        /// When profile created.
        /// </summary>
        [Column("created_at", System.Data.DbType.DateTime)]
        public DateTime createdAt;
    }
}
