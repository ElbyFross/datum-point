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
using System.Reflection;
using System.Windows.Controls;

namespace WpfHandler.UI.Controls.AutoLayout
{
    /// <summary>
    /// Class that provides adopting of members by AutoLayout UI.
    /// </summary>
    public abstract class UIDescriptor
    {
        /// <summary>
        /// Cyrrent active UI layer.
        /// </summary>
        LayoutLayer activeLayer = new LayoutLayer();

        /// <summary>
        /// Insiniate UI by descriptor's attributes map and add it as child to parent element.
        /// </summary>
        /// <param name="parent">UI element that would contain instiniated UI elemets.</param>
        public void BindTo(IAddChild parent)
        {
            // Get all memebers.
            var members = GetType().GetMembers();

            // Instiniate first UILayer.
            activeLayer = new LayoutLayer()
            {
                root = parent // Thet binding target as root for cuurent layer.
            };

            // Perform all descriptor map.
            foreach (MemberInfo member in members)
            {
                // Getting all attributes.
                var attributes = member.GetCustomAttributes<Attribute>(true);

                // Perform general attributes.
                foreach(Attribute attr in attributes)
                {
                    // Skip if an option.
                    if (attr is Interfaces.IGUILayoutOption) continue;

                    // Apply layout control to GUI.
                    if (attr is Interfaces.ILayoutControl control)
                    {
                        control.OnGUI(ref activeLayer, this, member);
                    }
                }

                // Spawn UI field relative to member type.

                // Perform options attributes.
                foreach (Attribute attr in attributes)
                {
                    // Skip if not an option.
                    if (!(attr is Interfaces.IGUILayoutOption)) continue;
                }





                // Try to get attribute that depends layer settings.
                var layerAttr = member.GetCustomAttribute(typeof(Interfaces.ILayerAttribute), true);

                #region Group begin
                // Start new UI group if requested.
                if (layerAttr is Interfaces.ILayerBeginAttribute)
                {
                    // Element that will contains childs of next UI group.
                    IAddChild newGroup = null;

                    // Start new horizontal group.
                    if (layerAttr is Attributes.BeginHorizontalGroup)
                    {
                        // Create grid.
                        newGroup = new Grid();
                    }
                    // Start new vertical group.
                    else
                    {
                        // Create stack panel.
                        newGroup = new StackPanel()
                        { Orientation = Orientation.Vertical };
                    }

                    // Set new layer.
                    activeLayer = activeLayer.GoDeeper(newGroup);
                }
                #endregion

                #region Header
                // Perform header if requested.
                if (member.GetCustomAttribute(typeof(Attributes.Header), true) is Attributes.Header headerAttr)
                {
                    // Instiniate UI header.
                    var header = new Header() { Content = headerAttr.Content };
                    ControlSignUp(header, member, true);
                }
                #endregion

                #region Init fields & properties

                #endregion

                #region End group
                // Ending UI group if requested.
                if (layerAttr is Interfaces.ILayerEndAttribute)
                {
                    // Close current layer.
                    activeLayer = activeLayer.GoUpper();
                }
                #endregion
            }
        }

        /// <summary>
        /// Safely init layout control element and sighn up on internal hadler events.
        /// </summary>
        /// <param name="control">The GUI control that will be instiniated.</param>
        /// <param name="member">The member that will be binded to the GUI.</param>
        /// <param name="value">Value that will applied as default.</param>
        public void ControlSignUp(Interfaces.ILayoutControl control, MemberInfo member, object value)
        {
            try
            {
                // Registrate member in auto layout handler.
                control.RegistrateField(this, member, value);

                // Adding herader to layout.
                activeLayer?.ApplyControl(control as FrameworkElement);
            }
            catch { }
        }


        /// <summary>
        /// Trying to bind control to the auto layout handler.
        /// </summary>
        /// <param name="control">Control that would be binded.</param>
        /// <param name="args">Must contains @UIDescriptor and @MemberInfo for success performing.</param>
        /// <returns>Is control was binded?</returns>
        public static bool TryToBindControl(Interfaces.ILayoutControl control, params object[] args)
        {
            try
            {
                // Trying to bind.
                ToBindControl(control, args);

                // Success if esception not occured
                return true;
            }
            catch
            {
                // Inform about binding fail.
                return false;
            }
        }

        /// <summary>
        /// Bind control to the auto layout handler.
        /// </summary>
        /// <param name="control">Control that would be binded.</param>
        /// <param name="args">Must contains @UIDescriptor and @MemberInfo.</param>
        public static void ToBindControl(Interfaces.ILayoutControl control, params object[] args)
        {
            // Find required referendes.
            UIDescriptor desc = null;
            MemberInfo member = null;

            // Trying to get shared properties.
            foreach (object obj in args)
            {
                if (obj is UIDescriptor) desc = (UIDescriptor)obj;
                if (obj is MemberInfo) member = (MemberInfo)obj;
            }

            // Drop control sign up in case if member not shared.
            if (desc == null) throw new NullReferenceException("@UIDescriptor not shared with @args");
            if (member == null) throw new NullReferenceException("@MemberInfo not shared with @args");

            // Sing up this control on desctiptor events.
            desc.ControlSignUp(control, member, true);
        }
    }
}
