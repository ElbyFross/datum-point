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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniformDataOperator.SQL.MySQL
{
    /// <summary>
    /// TODO Operator that provides possibility to operate data on MySQL data base server.
    /// </summary>
    public class MySQLDataOperator : ISQLOperator
    {
        /// <summary>
        /// Active single tone instance of MySQL data provider.
        /// </summary>
        public ISQLOperator Active
        {
            get
            {
                // Create new instence of MySQL data operator.
                if(_Active == null)
                {
                    _Active = new MySQLDataOperator();
                }
                return _Active;
            }
        }
        private MySQLDataOperator _Active;

        public string Server { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Database { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string UserId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Password { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
                
        public bool OpenConnection()
        {
            throw new NotImplementedException();
        }

        public bool CloseConnection()
        {
            throw new NotImplementedException();
        }

        public void ExecuteNonQuery(string query)
        {
            throw new NotImplementedException();
        }

        public object ExecuteScalar(string query)
        {
            throw new NotImplementedException();
        }

        public DbDataReader ExecuteReader(string query)
        {
            throw new NotImplementedException();
        }

        public int Count(string query)
        {
            throw new NotImplementedException();
        }

        public void Backup(string directory)
        {
            throw new NotImplementedException();
        }

        public void Restore(string filePath)
        {
            throw new NotImplementedException();
        }
    }
}
