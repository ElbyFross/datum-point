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
using System.Windows.Markup;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfHandler.UI.Controls.AutoLayout
{
    /// <summary>
    /// Provides common methods for layout controls.
    /// </summary>
    public static class LayoutHandler
    {
        /// <summary>
        /// Adding child to horizontal grid.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="element"></param>
        public static void HorizontalLayoutAddChild(IAddChild parent, FrameworkElement element)
        {
            // Drop ivalid elelment.
            if(!(parent is Grid grid))
            {
                throw new InvalidCastException("Parent must has `" + typeof(Grid).FullName + "` type.");
            }

            // Add new column fo element.
            grid.ColumnDefinitions.Add(new ColumnDefinition()
            {
                // define required width.
                // Auto - if width of element less or equals 0, or is NaN.
                // Shared element's width in case if defined.
                Width = new GridLength(double.IsNaN(element.Width) || element.Width <= 0 ? double.NaN : element.Width)
            });

            // Add element as child.
            parent.AddChild(element);

            // Set als column as target for element.
            Grid.SetColumn(element, grid.ColumnDefinitions.Count - 1);
        }

        /// <summary>
        /// Initialize the int field into UI.
        /// </summary>
        /// <param name="value">Default field's value.</param>
        /// <returns>Current value from the field.</returns>
        public static int IntField(int value)
        {
            return IntField(GUIContent.None, value);
        }

        /// <summary>
        /// Initialize the int field into UI.
        /// </summary>
        /// <param name="lable">Title that will be setted up to the lable.</param>
        /// <param name="value">Default field's value.</param>
        /// <returns>Current value from the field.</returns>
        public static int IntField(string lable, int value)
        {
            return IntField(new GUIContent(lable), value);
        }

        /// <summary>
        /// Initialize the int field into UI.
        /// </summary>
        /// <param name="lable">Lable content.</param>
        /// <param name="value">Default field's value.</param>
        /// <returns>Current value from the field.</returns>
        public static int IntField(GUIContent lable, int value)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Set header to UI.
        /// </summary>
        /// <param name="content">Descriptor of the header contnent.</param>
        public static void Header(GUIContent content)
        {

        }
    }
}
