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
using System.Reflection;
using System.Collections;
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
using WpfHandler.UI.Controls.AutoLayout.Interfaces;

namespace WpfHandler.UI.Controls.AutoLayout
{
    /// <summary>
    /// Provides common methods for layout controls.
    /// </summary>
    public static class LayoutHandler
    {
        /// <summary>
        /// Table that contains all registread value update callbacks.
        /// 
        /// Key = ILayoutControl
        /// </summary>
        private static readonly Hashtable RegistredCallbacks = new Hashtable();

        /// <summary>
        /// Adding child to horizontal grid.
        /// </summary>
        /// <param name="parent">Grid that will contain child.</param>
        /// <param name="element">Element that will be added to the grid as child.</param>
        public static void HorizontalLayoutAddChild(IAddChild parent, FrameworkElement element)
        {
            // Drop ivalid elelment.
            if(!(parent is Grid grid))
            {
                throw new InvalidCastException("Parent mast be `" + typeof(Grid).FullName + "`.");
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
        /// Adding child to the bertical layout group.
        /// </summary>
        /// <param name="parent">VerticalStackPanel that will contin child.</param>
        /// <param name="element">Element that will be added to the panel as child.</param>
        public static void VerticalLayoutAddChild(IAddChild parent, FrameworkElement element)
        {
            // Validate type cast.
            if(!(parent is StackPanel panel))
            {
                throw new InvalidCastException("Parent mast be `" + typeof(StackPanel).FullName + "`.");
            }

            // Set element to the parent panel.
            panel.Children.Add(element);
        }

        /// <summary>
        /// Registrating bool property into auto layout ui.
        /// </summary>
        /// <param name="control">Instiniated layout control.</param>
        /// <param name="descriptor">Descriptor that hold fields or properties.</param>
        /// <param name="member">Member in descriptor instance that will be used as target for value update.</param>
        /// <param name="defautltValue">Value that will be setted by default.</param>
        public static void RegistrateField(this ILayoutControl control, UIDescriptor descriptor, System.Reflection.MemberInfo member, object defautltValue)
        {
            // Apply default value.
            control.Value = defautltValue;

            // instiniate UI field update callback.
            Action<ILayoutControl> changeCallback = delegate (ILayoutControl _)
            {
                if (member is System.Reflection.PropertyInfo prop)
                {
                    // Try to set value.
                    try { prop.SetValue(descriptor, control.Value); } catch { };
                }
            };

            // To to registrate control into handler.
            try { RegistredCallbacks.Add(control, changeCallback); }
            catch { throw new NotSupportedException("@ILayoutControl could be registred only once."); }

            // Subscribe on value change.
            control.ValueChanged += changeCallback;
        }

        /// <summary>
        /// Unbind layout control from auto layout handler.
        /// </summary>
        /// <param name="control">Target layout control.</param>
        public static void UnregistrateField(this ILayoutControl control)
        {
            try
            {
                // Unregistreting of registred callback.
                control.ValueChanged -= (Action<ILayoutControl>)RegistredCallbacks[control];
            }
            catch
            {
                // Log error.
                MessageBox.Show("You trying to unregistred layout control " +
                    "that was not registred into the auto layout handler.\n" +
                    "Use `LayoutHandler.RegistrateYOURTYPEField` before.");
            }
        }
    }
}
