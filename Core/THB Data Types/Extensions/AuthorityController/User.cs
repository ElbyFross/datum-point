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

namespace AuthorityController.Data.Personal
{
    /// <summary>
    /// Additive fields and API suiteble for AC users in Datum Point.
    /// </summary>
    public partial class User : ISQLTable, ISQLDataReadCompatible
    {
        /// <summary>
        /// Middle name of user if applicable.
        /// </summary>
        public string middleName = null;

        /// <summary>
        /// Birthday of user in binary format.
        /// </summary>
        public long birthday;
        
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
