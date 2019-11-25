﻿//Copyright 2019 Volodymyr Podshyvalov
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
using WpfHandler.UI.AutoLayout;
using WpfHandler.UI.ECS;
using WpfHandler.UI.AutoLayout.Interfaces;
using WpfHandler.UI.ECS;

namespace WpfHandler.UI.AutoLayout
{
    /// <summary>
    /// Provides common methods for layout controls.
    /// </summary>
    public static class LayoutHandler
    {
        /// <summary>
        /// Contains declared bindings from types to default layout controls.
        /// Key - Source System.Type
        /// Value - Binded System.Type of suitable ILayoutControl.
        /// </summary>
        private static readonly Hashtable DefaultControlBindings = new Hashtable();

        /// <summary>
        /// Table that contains all registread value update callbacks.
        /// 
        /// Key = Type of the binded @ILayoutControl
        /// </summary>
        private static readonly Hashtable RegistredCallbacks = new Hashtable();

        /// <summary>
        /// Scaning asseblies to find descriptions of controls.
        /// </summary>
        static LayoutHandler()
        {
            RescanAssemblies();
        }

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

        //public static void RegistrateField(this ILayoutControl control, ref object bindingTarget)
        //{
        //    // Apply default value.
        //    control.Value = bindingTarget;

        //    // Instiniate UI field update callback for property members.
        //    Action<ILayoutControl> changeCallback = delegate (ILayoutControl _)
        //    {
        //        // Try to set value.
        //        try { bindingTarget = control.Value; } catch { };
        //    };

        //    // To to registrate control into handler.
        //    try { RegistredCallbacks.Add(control, handler); }
        //    catch { throw new NotSupportedException("@ILayoutControl could be registred only once."); }
            
        //    // Subscribe on value change.
        //    control.ValueChanged += propChangeCallback;
        //}

        /// <summary>
        /// Registrating bool property into auto layout ui.
        /// </summary>
        /// <param name="control">Instiniated layout control.</param>
        /// <param name="descriptor">Descriptor that hold fields or properties.</param>
        /// <param name="member">Member in descriptor instance that will be used as target for value update.</param>
        /// <param name="defautltValue">Value that will be setted by default.</param>
        public static void RegistrateField(this IGUIField control, UIDescriptor descriptor, MemberInfo member, object defautltValue)
        {
            #region Declaretion & Initializtion
            // Apply default value.
            control.Value = defautltValue;

            // Declaring registration params.
            Action<IGUIField> handler = null;
            PropertyInfo propMember = null;
            FieldInfo fieldMember = null;
            #endregion

            #region Handlers
            // Instiniate UI field update callback for property members.
            void PropChangeCallback(IGUIField _)
            {
                // Try to set value.
                try { propMember.SetValue(descriptor, control.Value); } catch { };
            }
            // Instiniate UI field update callback for fields members.
            void FieldChangeCallback(IGUIField _)
            {
                // Try to set value.
                try { fieldMember.SetValue(descriptor, control.Value); } catch { };
            }
            #endregion

            #region Configuration
            // Configurating registration params and handlers.
            if (member is PropertyInfo prop)
            {
                propMember = prop;
                handler = PropChangeCallback;
            }
            else if (member is FieldInfo field)
            {
                fieldMember = field;
                handler = FieldChangeCallback;
            }
            #endregion

            #region Registration
            // To to registrate control into handler.
            try { RegistredCallbacks.Add(control, handler); }
            catch { throw new NotSupportedException("@ILayoutControl could be registred only once."); }

            // Subscribe on value change.
            control.ValueChanged += PropChangeCallback;
            #endregion
        }

        /// <summary>
        /// Unbind layout control from auto layout handler.
        /// </summary>
        /// <param name="control">Target layout control.</param>
        public static void UnregistrateField(this IGUIField control)
        {
            try
            {
                // Unregistreting of registred callback.
                control.ValueChanged -= (Action<IGUIField>)RegistredCallbacks[control];
            }
            catch
            {
                // Log error.
                //MessageBox.Show("You trying to unregistred layout control " +
                //    "that was not registred into the auto layout handler.\n" +
                //    "Use `LayoutHandler.RegistrateField` before.");
            }
        }

        /// <summary>
        /// Rescaning solution for Layout controls bindings.
        /// </summary>
        public static void RescanAssemblies()
        {
            // Clearign current meta.
            DefaultControlBindings.Clear();

            // Load query's processors.
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                #region Operation with types
                // Get all types for assembly.
                foreach (Type type in assembly.GetTypes())
                {
                    #region Validate
                    // Skip if it's absctract. 
                    if (type.IsAbstract) continue;
                    // Skip if it's interface.
                    if (type.IsInterface) continue;
                    // Skip if not Implement @ILayoutControl.
                    if (type.GetInterface(typeof(IGUIField).FullName) == null) continue;
                    #endregion

                    #region Performing all existing descriptors.
                    // Trying to find descriptor to types binding.
                    var bindingDescriptors = type.GetCustomAttributes<Attributes.Configuration.TypesCompatible>();

                    foreach (var desc in bindingDescriptors)
                    {
                        // Applying all shared bindings.
                        foreach (var bindedType in desc.CompatibleWith)
                        {
                            // Bind type to control.
                            BindLayoutControlToType(type, bindedType);
                        }
                    }
                    #endregion
                }
                #endregion
            }
        }

        /// <summary>
        /// Binds @ILayoutControl to the source type to using into auto generate ui panels based on @UIDescriptor content.
        /// </summary>
        /// <param name="controlType">Type with implemented @ILayoutControl interface.</param>
        /// <param name="sourceType">Type that will cause spawning of binded @ILayoutControl during building of auto-generated UIs.</param>
        public static void BindLayoutControlToType(Type controlType, Type sourceType)
        {
            if (DefaultControlBindings[sourceType] is IGUIField)
            {
                // If control already defined for that type then override it.
                DefaultControlBindings[sourceType] = controlType;
            }
            else
            {
                // Registrate new binding if not registred yet.
                DefaultControlBindings.Add(sourceType, controlType);
            }
        }

        /// <summary>
        /// Looking got control binded to the source type.
        /// </summary>
        /// <param name="sourceType">Type of the member that will be applied to the UI control.</param>
        /// <param name="isInherited">Should it look into entire inheritance hierarchy of the source type till not found the binded one?</param>
        /// <returns>Type of found control. Null if not found.</returns>
        public static Type GetBindedControl(Type sourceType, bool isInherited)
        { 
            // Check if requested type has binding.
            if(DefaultControlBindings[sourceType] is Type control)
            {
                // Return binded control if found.
                return control;
            }
            else
            {
                // Looking into hierarchy.
                if (isInherited && // Is looking in depth requested.
                    sourceType.BaseType != null) // is still not on the ground level.
                {
                    
                    return GetBindedControl(sourceType.BaseType, true);
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
