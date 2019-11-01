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
using System.Xml.Serialization;
using UniformDataOperator.Sql.Attributes;
using UniformDataOperator.Sql.MySql.Attributes;

namespace DatumPoint.Plugins.Social.AuditoryPlanner
{
    /// <summary>
    /// Class that describe shema of auditorium.
    /// </summary>
    [Table("datum-point", "auditoriumSchema")]
    [Serializable]
    public class Schema
    {
        #region Fields & properties
        /// <summary>
        /// Unique id of that schema.
        /// </summary>
        [Column("schemaid", System.Data.DbType.Int32), IsPrimaryKey, IsNotNull]
        [MySqlDBTypeOverride(MySql.Data.MySqlClient.MySqlDbType.Int32, "INT")]
        public int id = -1;

        /// <summary>
        /// Seats grid in binary format to storing in databases.
        /// </summary>
        [Column("grid", System.Data.DbType.Binary)]
        [MySqlDBTypeOverride(MySql.Data.MySqlClient.MySqlDbType.TinyBlob, "TINYBLOB")]
        [XmlIgnore]
        public byte[] GridBlob
        { 
            get
            {
                if (grid == null) return null;
                return UniformDataOperator.Binary.BinaryHandler.ToByteArray(grid);
            }
            set
            {
                try
                {
                    grid = UniformDataOperator.Binary.BinaryHandler.FromByteArray<Seat[,]>(value);
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Auditorium schema's grid damaged. Details:" + ex.Message);

                    // Set empty grid.
                    grid = new Seat[0, 0];
                }
            }
        }

        /// <summary>
        /// Grid of seats applied to the schema.
        /// </summary>
        public Seat[,] grid;
        #endregion

        #region API
        #endregion
    }
}
