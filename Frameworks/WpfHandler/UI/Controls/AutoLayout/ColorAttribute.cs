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
using System.Windows.Media;

namespace WpfHandler.UI.Controls.AutoLayout
{
    /// <summary>
    /// Base class for color attributes that provides unioform convertion API for color mamangment relative to the WPF.
    /// </summary>
    public abstract class ColorAttribute : Attribute
    {
        /// <summary>
        /// Applying color to hte brush.
        /// </summary>
        public Color Color
        {
            set { Brush = new SolidColorBrush(value); }
        }

        /// <summary>
        /// Trying to apply string color code as brush by using `System.Windows.Media.ColorConverter' rules.
        /// Not throw excption in case if color's code invalid to prevent UI crash.
        /// </summary>
        public string StringColor
        {
            set
            {
                try
                {
                    // Trying to get color from the string.
                    var bufer = (Color)ColorConverter.ConvertFromString(value);
                    // Applying color if converting passed without exception.
                    this.Color = bufer;
                }
                catch { return; }
            }
        }

        /// <summary>
        /// Brush that will applied to GUI element.
        /// </summary>
        public Brush Brush
        {
            get { return _Brush; }
            set { _Brush = value; }
        }

        /// <summary>
        /// Bufer that contains generated or shared Brush.
        /// </summary>
        private Brush _Brush;
    }
}
