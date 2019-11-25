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
using WpfHandler.UI.ECS;
using WpfHandler.UI.AutoLayout.Interfaces;

namespace WpfHandler.UI.AutoLayout
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
        /// <param name="root">UI element that would contain instiniated UI elemets.</param>
        public void BindTo(Panel root)
        {
            // Get all memebers.
            var members = GetType().GetMembers();

            // Sort in declaretion order.
            members = members.OrderBy(f => f.MetadataToken).ToArray<MemberInfo>();

            // Instiniate first UILayer.
            activeLayer = new LayoutLayer()
            {
                root = root // Thet binding target as root for cuurent layer.
            };

            // Perform all descriptor map.
            foreach (MemberInfo member in members)
            {
                if(!MembersHandler.GetSpecifiedMemberInfo(member, out PropertyInfo prop, out FieldInfo field))
                {
                    // Skip if the member is not field or property.
                    continue;
                }

                Type memberType = MembersHandler.GetSpecifiedMemberType(member);

                // Skip if member excluded from instpector.
                if (member.GetCustomAttribute<Attributes.Layout.HideInInspector>() != null)
                {
                    continue;
                }

                // Getting all attributes.
                var attributes = member.GetCustomAttributes<Attribute>(true);

                #region Perform general layout attributes
                // Perform general attributes.
                foreach(Attribute attr in attributes)
                {
                    // Skip if an option.
                    if (attr is IGUILayoutOption) continue;

                    // Apply layout control to GUI.
                    if (attr is IGUIElement attrControl)
                    {
                        attrControl.OnGUI(ref activeLayer, this, member);
                    }
                }
                #endregion

                #region Spawn field to UI
                // Spawn UI field relative to member type.
                Type controlType = null;

                // Check if default control was overrided by custom one.
                var customControlDesc = member.GetCustomAttribute<Attributes.Configuration.CustomControl>();
                if (customControlDesc != null && // Is overriding requested?
                    customControlDesc.ControlType != null && // Is target type is not null?
                    customControlDesc.ControlType.IsSubclassOf(typeof(IGUIField))) // Is target type has correct inherience
                {
                    // Set redefined control like target to instinitation.
                    controlType = customControlDesc.ControlType;
                }
                else
                {
                    // Set binded type like target to instiniation.
                    controlType = LayoutHandler.GetBindedControl(memberType, true);
                }

                // Is control defined to that member?
                if (controlType != null)
                {
                    // Instiniating target type.
                    var control = (IGUIField)Activator.CreateInstance(controlType);

                    // Sing up this control on desctiptor events.
                    ControlSignUp(control, member, true);

                    // Initialize control.
                    control.OnGUI(ref activeLayer, this, member);

                    #region Perform Layout options
                    // Check if spawned control is framework element.
                    if (control is FrameworkElement fEl)
                    {
                        // Perform options attributes.
                        foreach (Attribute attr in attributes)
                        {
                            // Skip if not an option.
                            if (!(attr is IGUILayoutOption option)) continue;

                            option.ApplyLayoutOption(fEl);
                        }
                    }
                    #endregion
                }
                else
                {
                    // Check if that just other descriptor.
                    if (memberType.IsSubclassOf(typeof(UIDescriptor)))
                    {
                        #region Configurating layout
                        // Add horizontal shift for sub descriptor.
                        new Attributes.Layout.BeginHorizontalGroup().OnGUI(ref activeLayer);
                        new Attributes.Elements.Space(10).OnGUI(ref activeLayer);

                        // Add vertical group.
                        var vertGroup = new Attributes.Layout.BeginVerticalGroup();
                        vertGroup.OnGUI(ref activeLayer);
                        #endregion

                        #region Looking for descriptor object.
                        // Bufer that will contain value of the descriptor.
                        UIDescriptor subDesc = null;

                        // Trying to get value via reflection.
                        subDesc = prop != null ? 
                            prop.GetValue(this) as UIDescriptor : // Operate like property.
                            field.GetValue(this) as UIDescriptor; // Operate like fields.

                        // Instiniate default in case if value is null.
                        if(subDesc == null)
                        {
                            try
                            {
                                // Insiniate empty constructor.
                                subDesc = Activator.CreateInstance(memberType) as UIDescriptor;
                            }
                            catch(Exception ex)
                            {
                                // Log error.
                                MessageBox.Show("UIDescriptor must contain empty constructor, " +
                                    "or be instiniated before calling into UI." +
                                    "\n\nDetails:\n" + ex.Message);

                                // Skip to the next member.
                                continue;
                            }

                            // Updating stored value for current member.
                            if (prop != null) prop.SetValue(this, subDesc);
                            else field.SetValue(this, subDesc);
                        }
                        #endregion

                        // Bind descriptor to the UI.
                        subDesc.BindTo((Panel)activeLayer.root);
                    }
                }
                #endregion
            }
        }

        /// <summary>
        /// Unbinding descriptor from panel.
        /// Will affect only memeber defied into descriptor and leave and other GUI elements childed to the panel.
        /// </summary>
        /// <param name="root">Root panle that was binding target for descriptor.</param>
        public void UnbindFrom(Panel root)
        {
            // Get all memebers.
            var members = GetType().GetMembers();

            // Checking every child into the root.
            for (int i = 0; i < root.Children.Count; i++)
            {
                // Getting current child.
                var child = root.Children[i];

                // If child is layout control and has a binded memeber.
                if (child is IGUIField control &&
                    control.BindedMember != null)
                {
                    #region Validation
                    // Checking is the binded memeber is a part of the descriptor.
                    bool thisDesc = false;
                    foreach(var member in members)
                    {
                        // Check if binded member is the one from the descriptor.
                        if(member.Equals(control.BindedMember))
                        {
                            thisDesc = true;
                            break;
                        }
                    }

                    // Skip if not possesed to that descriptor.
                    if (!thisDesc) continue;
                    #endregion

                    #region Unbind control from UI
                    // Unsubscribe element from hadler's events.
                    LayoutHandler.UnregistrateField(control);

                    // Remove from UI.
                    root.Children.RemoveAt(i);
                    i--;
                    #endregion
                    
                    #region Sub descriptor processing
                    // If member is UI descriptor.
                    if (control is Panel subPanel &&
                        MembersHandler.GetSpecifiedMemberType(control.BindedMember).IsSubclassOf(typeof(UIDescriptor)))
                    {
                        try
                        {
                            // Requiest descriptor unbinding.
                            ((UIDescriptor)control.Value).UnbindFrom(subPanel);
                        }
                        catch(Exception ex)
                        {
                            // Log error.
                            MessageBox.Show("Subpanel UIDescriptor unbind operation failed." +
                                "\n\nDetails:\n" + ex.Message);
                        }
                    }
                    #endregion
                }
            }
        }

        /// <summary>
        /// Safely init layout control element and sighn up on internal hadler events.
        /// </summary>
        /// <param name="control">The GUI control that will be instiniated.</param>
        /// <param name="member">The member that will be binded to the GUI.</param>
        /// <param name="value">Value that will applied as default.</param>
        public void ControlSignUp(IGUIField control, MemberInfo member, object value)
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
        public static bool TryToBindControl(IGUIField control, params object[] args)
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
        public static void ToBindControl(IGUIField control, params object[] args)
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

            // Request binding.
            ToBindControl(control, desc, member);
        }

        /// <summary>
        /// Bind control to the auto layout handler.
        /// </summary>
        /// <param name="control">Control that would be binded.</param>
        /// <param name="descriptor">Source descriptor.</param>
        /// <param name="member">Member from the descriptor.</param>
        public static void ToBindControl(
            IGUIField control, 
            UIDescriptor descriptor, 
            MemberInfo member)
        {
            // Drop control sign up in case if member not shared.
            if (descriptor == null) throw new NullReferenceException("@UIDescriptor not shared with @args");
            if (member == null) throw new NullReferenceException("@MemberInfo not shared with @args");

            // Detecting default value seted up into descriptor.
            object defaultValue;
            // Getting from property member.
            if (member is PropertyInfo pi) defaultValue = pi.GetValue(descriptor);
            // Getting from field memeber.
            else if (member is FieldInfo fi) defaultValue = fi.GetValue(descriptor);
            // Member cast is invalid and not supported intor that operation.
            else throw new InvalidCastException("@member must inherit PropertyInfo of FieldInfo");

            // Sing up this control on desctiptor events.
            descriptor.ControlSignUp(control, member, defaultValue);
        }


        /// <summary>
        /// Handling tasks with members suitable for UI descriptor's operations.
        /// </summary>
        public static class MembersHandler
        {
            /// <summary>
            /// Get info suitable for field and properties members.
            /// </summary>
            /// <param name="member"></param>
            /// <param name="propInfo"></param>
            /// <param name="fieldInfo"></param>
            /// <returns>Is the member is property of field?</returns>
            public static bool GetSpecifiedMemberInfo(MemberInfo member,
                out PropertyInfo propInfo, out FieldInfo fieldInfo)
            {
                propInfo = null;
                fieldInfo = null;

                // Check if is property.
                if (member is PropertyInfo propBufer)
                {
                    // Getting stored value.
                    propInfo = propBufer;
                    return true;
                }
                // Check if is field.
                else if (member is FieldInfo fieldBufer)
                {
                    fieldInfo = fieldBufer;
                    return true;
                }

                return false;
            }

            /// <summary>
            /// Set value to the member.
            /// </summary>
            /// <param name="member">PropertyInfo ot FieldInfo instance.</param>
            /// <param name="target">Object that contains member.</param>
            /// <param name="value">Value to set.</param>
            public static void SetValue(MemberInfo member, object target, object value)
            {
                // Trying to get specified memebers.
                if(GetSpecifiedMemberInfo(member, out PropertyInfo pi, out FieldInfo fi))
                {
                    if (pi != null) pi.SetValue(target, value); // Operate as property.
                    else fi.SetValue(target, value); // Operate as field.
                }
                else
                {
                    throw new NotSupportedException("SetValue can be applied only to peopreties and fields.");
                }
            }

            public static Type GetSpecifiedMemberType(MemberInfo member)
            {
                // Trying to get specified memebers.
                if (GetSpecifiedMemberInfo(member, out PropertyInfo pi, out FieldInfo fi))
                {
                    if (pi != null) return pi.PropertyType; // Operate as property.
                    else return fi.FieldType; // Operate as field.
                }
                else
                {
                    throw new NotSupportedException("Get_Type can be applied only to peopreties and fields.");
                }
            }
        }
    }
}
