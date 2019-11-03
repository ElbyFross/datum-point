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

namespace DatumPoint.Plugins.Social.AuditoryPlanner
{
    /// <summary>
    /// Implement interface that allow to manage a some count of seats as a single block.
    /// Configurating drawing grid.
    /// Controlling indexes of places.
    /// </summary>
    public partial class SeatsBlock : UserControl
    {
        public static readonly DependencyProperty BlockProperty = DependencyProperty.Register(
            "Block", typeof(Types.AuditoryPlanner.SeatsBlock), typeof(SeatsBlock));

        public static readonly DependencyProperty EditorModeProperty = DependencyProperty.Register(
            "EditorMode", typeof(bool), typeof(SeatsBlock));

        public static readonly DependencyProperty CellSizeProperty = DependencyProperty.Register(
            "CellSize", typeof(float), typeof(SeatsBlock), new PropertyMetadata(25));

        public static readonly DependencyProperty CellsSpaceProperty = DependencyProperty.Register(
            "CellSize", typeof(float), typeof(SeatsBlock), new PropertyMetadata(5));

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
                UpdateGrid();
            }
        }
        
        /// <summary>
        /// Size of the grid's cell.
        /// </summary>
        public float CellSize
        {
            get { return (float)this.GetValue(CellSizeProperty); }
            set
            {
                this.SetValue(CellSizeProperty, value);

                // Update GUI
                UpdateGrid();
            }
        }

        /// <summary>
        /// Space between grid's cells.
        /// </summary>
        public float CellsSpace
        {
            get { return (float)this.GetValue(CellsSpaceProperty); }
            set
            {
                this.SetValue(CellsSpaceProperty, value);

                // Update GUI
                UpdateGrid();
            }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SeatsBlock()
        {
            InitializeComponent();
            DataContext = this;
        }

        /// <summary>
        /// Updating grid of elements relative to that block.
        /// </summary>
        public void UpdateGrid()
        {
            #region Init & validation
            // Prevent empty grid.
            if (Block == null) Block = new Types.AuditoryPlanner.SeatsBlock();

            var width = Block.grid.GetLength(0);
            var height = Block.grid.GetLength(1);
            #endregion

            #region Clear current grid.
            canvas.ColumnDefinitions.Clear();
            canvas.RowDefinitions.Clear();
            canvas.Children.Clear();
            #endregion

            #region Instiniating new grid.
            // Define columns.
            for (int i = 0; i < width; i++)
            {
                // Adding space between columns.
                canvas.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(CellsSpace) });
                // Adding rows.
                canvas.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(CellSize) });
            }
            // Adding final space column.
            canvas.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(CellsSpace) });

            // Define rows.
            for (int i = 0; i < height; i++)
            {
                // Adding space between rows.
                canvas.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(CellsSpace) });
                // Adding rows.
                canvas.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(CellSize) });
            }
            // Adding final space row.
            canvas.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(CellsSpace) });
            #endregion

            // TODO FILL GRID
            #region Fill grid by content
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    // Compute x position for cell.
                    var xOffset = (x + 1) * CellsSpace + x * CellSize;

                    // Add editor interface if required.
                    if (EditorMode)
                    {
                        // Add up active border.
                        var yOffset = y * (CellsSpace + CellSize);

                        var upActiveBorder = new Canvas();
                        canvas.Children.Add(upActiveBorder);
                        upActiveBorder.Width = CellSize;
                        upActiveBorder.Height = CellsSpace;
                        upActiveBorder.Margin = new Thickness(x, y, -x, -y);

                        // React on mouse focusing
                        upActiveBorder.MouseEnter += delegate (object sender, MouseEventArgs e)
                        {

                        };

                        // Add left active border.
                    }

                    // Add set item.
                }
            #endregion;
        }
    }
}
