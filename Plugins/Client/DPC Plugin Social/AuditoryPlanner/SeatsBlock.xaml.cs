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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfHandler.UI.Controls;

namespace DatumPoint.Plugins.Social.AuditoryPlanner
{
    /// <summary>
    /// Implement interface that allow to manage a some count of seats as a single block.
    /// Configurating drawing grid.
    /// Controlling indexes of places.
    /// </summary>
    public partial class SeatsBlock
    {
        /// <summary>
        /// Event that will be called when the grid's block will be activated.
        /// </summary>
        public event Action<SelectableGrid> BlockSelected
        {
            add
            {
                seatsBlock.GridSelected += value;
            }
            remove
            {
                seatsBlock.GridSelected -= value;
            }
        }

        /// <summary>
        /// Event that will be called when some border will selected.
        /// </summary>
        public event Action<SelectableGrid, SelectableGrid.ActiveBorder> BorderSelected
        {
            add
            {
                seatsBlock.BorderSelected += value;
            }
            remove
            {
                seatsBlock.BorderSelected -= value;
            }
        }

        #region Dependency properties
        public static readonly DependencyProperty BlockProperty = DependencyProperty.Register(
            "Block", typeof(Types.AuditoryPlanner.SeatsBlock), typeof(SeatsBlock));

        public static readonly DependencyProperty EditorModeProperty = DependencyProperty.Register(
            "EditorMode", typeof(bool), typeof(SeatsBlock));

        public static readonly DependencyProperty CellSizeProperty = DependencyProperty.Register(
            "CellSize", typeof(float), typeof(SeatsBlock), new PropertyMetadata(25.0f));

        public static readonly DependencyProperty CellsSpaceProperty = DependencyProperty.Register(
            "CellsSpace", typeof(float), typeof(SeatsBlock), new PropertyMetadata(5.0f));
        #endregion

        #region Static members
        /// <summary>
        /// Current active block.
        /// </summary>
        public static Types.AuditoryPlanner.SeatsBlock Current { get; set; }
        #endregion

        #region Public members
        /// <summary>
        /// Binded seats block data.
        /// </summary>
        public Types.AuditoryPlanner.SeatsBlock Block
        {
            get { return (Types.AuditoryPlanner.SeatsBlock)this.GetValue(BlockProperty); }
            set
            {
                this.SetValue(BlockProperty, value);

                // Update GUI
                UpdateGrid();
            }
        }

        /// <summary>
        /// Is UI allow to modify seats block grid content.
        /// </summary>
        public bool EditorMode
        {
            get { return (bool)this.GetValue(EditorModeProperty); }
            set
            {
                this.SetValue(EditorModeProperty, value);

                // Update GUI
                seatsBlock.SelectebleBlocks = value;
                seatsBlock.SelectebleColumns = value;
                seatsBlock.SelectebleRows = value;
                seatsBlock.UpdateGrid();
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        public SeatsBlock()
        {
            InitializeComponent();
            DataContext = this;

            // Subscribe on data update events.
            seatsBlock.OnElementIstiniation += UpdateDataElementHandler;
        }

        ~SeatsBlock()
        {
            // Unsubscribe from data update events.
            seatsBlock.OnElementIstiniation -= UpdateDataElementHandler;
        }
        #endregion

        /// <summary>
        /// Updating seats grid.
        /// </summary>
        public void UpdateGrid()
        {
            seatsBlock.ColumnsCount = Block.grid.GetLength(0);
            seatsBlock.RowsCount = Block.grid.GetLength(1);

            seatsBlock.UpdateGrid();
        }

        /// <summary>
        /// Set a new data to UI element. 
        /// </summary>
        /// <param name="x">X coordinate of data element.</param>
        /// <param name="y">Y coordinate of data element.</param>
        /// <returns>instiniated element.</returns>
        public UIElement UpdateDataElementHandler(int x, int y)
        {
            return new Seat()
            {
                Meta = Block.grid[x, y]
            };
        }
    }
}
