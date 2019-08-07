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

namespace UniformDataOperator
{
    public class MySQLDataOperator : IUniformDataOperator
    {
        /// <summary>
        /// Active single tone instance of MySQL data provider.
        /// </summary>
        public IUniformDataOperator Active
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
        
        /// <summary>
        /// Selecting data by query from MySQL data base.
        /// </summary>
        /// <param name="query">Query that will has been sending to the server.</param>
        /// <returns>Recived data.</returns>
        public object Select(string query)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Try to delete object from MySQL data base.
        /// </summary>
        /// <param name="query">Query that will has been sending to the server.</param>
        /// <returns>Result of operation.</returns>
        public bool Delete(string query)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Inserting data by query to MySQL data base.
        /// </summary>
        /// <param name="query">Query that will has been sending to the server.</param>
        /// <returns>Result of operation.</returns>
        public bool Insert(string query)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updating data by query in MySQL data base.
        /// </summary>
        /// <param name="query">Query that will has been sending on the server.</param>
        /// <returns>Result of operation.</returns>
        public bool Update(string query)
        {
            throw new NotImplementedException();
        }

        public bool IsStorageExist(params object[] args)
        {
            throw new NotImplementedException();
        }

        public bool SetData(params object[] args)
        {
            throw new NotImplementedException();
        }

        public object GetData(params object[] args)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Request data deletition from table by params.
        /// </summary>
        /// <param name="args">
        /// (string) args[0] - table directory.
        /// (string) args[1-...] - where conditions.
        /// 
        /// 
        /// Example request format:
        /// DeleteData("usersTable", "name='John Smith'", "location='London'", "age='45'");
        /// 
        /// Example converted sql query:
        /// DELETE FROM usersTable WHERE name='John Smith' AND location='London' AND age='45'
        /// </param>
        /// <returns></returns>
        public bool DeleteData(params object[] args)
        {
            throw new NotImplementedException();
        }
    }
}
