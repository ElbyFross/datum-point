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

namespace DatumPoint.Plugins.Social.Types.AuditoryPlanner
{
    /// <summary>
    /// Data that describes the block of seats on the schema.
    /// </summary>
    [Serializable]
    public class SeatsBlock
    {
        #region Core data
        /// <summary>
        /// Grid of seats applied to the schema.
        /// </summary>
        public Seat[,] grid;
        #endregion

        #region Layout
        /// <summary>
        /// X offset of the block from shema center.
        /// </summary>
        public float anchorX;

        /// <summary>
        /// Y offset of the block from shema center.
        /// </summary>
        public float anchorY;

        /// <summary>
        /// Additive distance between subblocks.
        /// Can't be less then 0;
        /// </summary>
        [XmlIgnore]
        public float SubblocksDistance
        {
            get
            {
                return _SubblocksDistance;
            }
            set
            {
                _SubblocksDistance = Math.Max(0, value);
            }
        }

        /// <summary>
        /// Bufer that contains current value of the @SubblocksDistance property.
        /// </summary>
        protected float _SubblocksDistance;

        /// <summary>
        /// If seats will dublicated on left and rights sides by the same schema.
        /// </summary>
        public bool HorizontalSymmetry
        {
            get
            {
                return _HorizontalSymmetry;
            }
            set
            {
                // If valie is differend then modify array of seats.
                if(value != _HorizontalSymmetry)
                {
                    // if required creation of additive seats.
                    if(value)
                    {
                        int bW = grid.GetLength(0);
                        int bH = grid.GetLength(1);

                        // Dublicate data from old array.
                        var nGrid = new Seat[bW * 2, bH];
                        for (int x = 0; x < bW; x++)
                            for (int y = 0; y < bH; y++)
                            {
                                nGrid[x, y] = grid[x, y];
                            }

                        // Init new data.
                        for (int x = bW; x < nGrid.GetLength(0); x++)
                            for (int y = 0; y < bH; y++)
                            {
                                nGrid[x, y] = new Seat();
                            }

                        // Override data.
                        grid = nGrid;
                    }
                    // If required removing of created seats.
                    else
                    {
                        // Compute new size params.
                        int nW = grid.GetLength(0) / 2;
                        int nH = grid.GetLength(1);

                        // Dublicate left data from old array.
                        var nGrid = new Seat[nW, nH];
                        for(int x = 0; x < nW; x++)
                            for (int y = 0; y < nH; y++)
                            {
                                nGrid[x, y] = grid[x, y];
                            }

                        // Override data.
                        grid = nGrid;
                    }

                    // update data.
                    _HorizontalSymmetry = value;
                    RecomputeIndexes();
                }
            }
        }

        /// <summary>
        /// Bufer that contains current value of the @HorizontalSymmetry property.
        /// </summary>
        protected bool _HorizontalSymmetry;
        #endregion

        #region Numeric rights
        /// <summary>
        /// Index of the first seat in the grid.
        /// Can't be less then 0.
        /// </summary>
        public int StartIndex
        {
            get
            {
                return _StartIndex;
            }
            set
            {
                _StartIndex = Math.Max(0, value);
                RecomputeIndexes();
            }
        }

        /// <summary>
        /// Bufer that contains current value of the @StartIndex property.
        /// </summary>
        protected int _StartIndex;

        /// <summary>
        /// Every raw wil start from @startIndex.
        /// @subBlockVerticalSkip still can be applied. 
        /// 
        /// Example:
        /// Enable that toggle, @startIndex on 10 and set @startIndexForEveryRaw to 10 for get result like
        /// 10|11|12|...
        /// 20|21|22|...
        /// </summary>
        public bool StartIndexForEveryRaw
        {
            get
            {
                return _StartIndexForEveryRaw;
            }
            set
            {
                value = StartIndexForEveryRaw;
                RecomputeIndexes();
            }
        }

        /// <summary>
        /// Bufer that contains current value of the @StartIndexForEveryRaw property.
        /// </summary>
        protected int _StartIndexForEveryRaw;

        /// <summary>
        /// H0w many indexes will be skiped between symmetric sub blocks in horizontal order.
        /// Can't be less then 0.
        /// 
        /// Example: 1|2|3|4 _ _ _ _ 9|10|11|12 - skiped 4 indexes that could be used by other blocks. 
        /// </summary>
        public int SubBlockHorizontalSkip
        {
            get
            {
                return _SubBlockHorizontalSkip;
            }
            set
            {
                _SubBlockHorizontalSkip = Math.Max(0, value);
                RecomputeIndexes();
            }
        }

        /// <summary>
        /// Bufer that contains current value of the SubBlockHorizontalSkip property.
        /// </summary>
        protected int _SubBlockHorizontalSkip;

        /// <summary>
        /// How many indexes will be skiped between rows in vertical order.
        /// Can't be less then 0.
        /// 
        /// Example: 
        /// |1 |2 |3 |4 | First row started from 1.
        /// |  |  |  |  | skiped 4 indexes that could be used by other blocks. 
        /// |9 |10|11|12| Secon row started from FirstRow.Lenght + 4
        /// </summary>
        public int SubBlockVerticalSkip
        {
            get
            {
                return _SubBlockVerticalSkip;
            }
            set
            {
                _SubBlockVerticalSkip = Math.Max(0, value);
                RecomputeIndexes();
            }
        }

        /// <summary>
        /// Bufer that contains current value of the SubBlockVerticalSkip property.
        /// </summary>
        protected int _SubBlockVerticalSkip;
        #endregion

        public SeatsBlock()
        {
            // Set default seat.
            grid = new Seat[1, 1];
            grid[0, 0] = new Seat();
        }

        #region API
        /// <summary>
        /// Recomputing current indexes of the seats relative to numeric rules.
        /// </summary>
        public void RecomputeIndexes()
        {
            // Buferize size.
            var bW = grid.GetLength(0);
            var bH = grid.GetLength(1);

            #region Validation
            // Initialize seats in case in data not found.
            for(int i = 0; i < bW; i++)
                for(int k = 0; k < bH; k++)
                {
                    if(grid[i,k] == null)
                    {
                        grid[i, k] = new Seat();
                    }
                }

            #endregion

            #region Couple sub blocks computing
            // If symmetric enabled.
            if (HorizontalSymmetry)
            {
                #region Computing left sub block
                for (int y = 0; y < bH; y++)
                {
                    #region Computing example
                    // Check: HSkip=2 | VSkip=6 
                    //
                    // 1 2 _ _ 5 6
                    // _ _ _ _ _ _
                    // 13 14 _ _ 17 18
                    // _ _ _ _ _ _ 
                    // 25 26 _ _ 29 30
                    //
                    // Results:
                    // 1 * (4 + 2 + 6) + 1 = 13
                    // 2 * -||- = 25
                    #endregion

                    var yOffset = StartIndexForEveryRaw ? 0 : y * (bW + SubBlockHorizontalSkip + SubBlockVerticalSkip);
                    for (int x = 0; x < bW / 2; x++)
                    {
                        grid[x, y].index = x + yOffset + StartIndex;
                    }
                }
                #endregion

                #region Computing right sub block
                for (int y = 0; y < bH; y++)
                {
                    var yOffset = StartIndexForEveryRaw ? 0 : y * (bW + SubBlockHorizontalSkip + SubBlockVerticalSkip);
                    var xOffset = bW / 2 + SubBlockHorizontalSkip;
                    for (int x = 0; x < bW / 2; x++)
                    {
                        grid[x, y].index = xOffset + yOffset + StartIndex + x;
                    }
                }
                #endregion
            }
            #endregion
            #region Single sub block computing
            else
            {
                for (int y = 0; y < bH; y++)
                {
                    var yOffset = StartIndexForEveryRaw ? 0 : y * bW;
                    for (int x = 0; x < bW / 2; x++)
                    {
                        grid[x, y].index = yOffset + x + StartIndex;
                    }
                }
            }
            #endregion
        }
        #endregion
    }
}
