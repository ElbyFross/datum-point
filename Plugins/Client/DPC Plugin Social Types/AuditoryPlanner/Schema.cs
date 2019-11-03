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

namespace DatumPoint.Plugins.Social.Types.AuditoryPlanner
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
        /// Seats blocls binary format to storing in databases.
        /// </summary>
        [Column("blocks", System.Data.DbType.Binary)]
        [MySqlDBTypeOverride(MySql.Data.MySqlClient.MySqlDbType.TinyBlob, "TINYBLOB")]
        [XmlIgnore]
        public byte[] BlocksBlob
        { 
            get
            {
                if (blocks == null) return null;
                return UniformDataOperator.Binary.BinaryHandler.ToByteArray(blocks);
            }
            set
            {
                try
                {
                    // Trying to decode blocks from binary.
                    blocks = UniformDataOperator.Binary.BinaryHandler.FromByteArray<SeatsBlock[]>(value);
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Auditorium schema's grid damaged. Details:" + ex.Message);

                    // Set base auditorium.
                    blocks = new SeatsBlock[1];
                    blocks[0].grid = new Seat[1, 1];
                    blocks[0].grid[0, 0] = new Seat();
                }
            }
        }

        /// <summary>
        /// Bunch of blocks applied to the schema.
        /// </summary>
        public SeatsBlock[] blocks;

        /// <summary>
        /// Check if all indexes is valid.
        /// </summary>
        public bool IsIndexesValid
        {
            get
            {
                // Table that will contains all indexes
                HashSet<int> usedIndexes = new HashSet<int>();
                foreach(SeatsBlock block in blocks)
                {
                    // Check every registred index.
                    for(int i = 0; i < block.grid.GetLength(0); i++)
                        for (int k = 0; k < block.grid.GetLength(1); k++)
                        {
                            // Check if index alredy in hashset.
                            if(!usedIndexes.Contains(block.grid[i,k].index))
                            {
                                // Add index to set.
                                usedIndexes.Add(block.grid[i, k].index);
                            }
                            else
                            {
                                // Drop due conflict.
                                return false;
                            }
                        }
                }
                return true;
            }
        }
        #endregion

        #region API
        #endregion
    }
}
