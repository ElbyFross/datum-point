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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfHandler.UI.AutoLayout.Attributes.Options
{
    /// <summary>
    /// Define vertical align of the GUI element.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class VerticalAlign : Attribute, Interfaces.IGUILayoutOption
    {
        /// <summary>
        /// Alignment that will applied to GUI element.
        /// </summary>
        public VerticalAlignment Alignment { get; set; }

        /// <summary>
        /// Define vertical align of the GUI element.
        /// </summary>
        /// <param name="element">Shared UI element.</param>
        public void ApplyLayoutOption(FrameworkElement element)
        {
            element.VerticalAlignment = Alignment;
        }
    }
}
