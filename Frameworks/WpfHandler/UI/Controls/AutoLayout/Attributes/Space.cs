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

namespace WpfHandler.UI.Controls.AutoLayout.Attributes
{
    /// <summary>
    /// Adding space between UI elements.
    /// </summary>
    public class Space : Attribute, Interfaces.ILayoutSize
    {
        /// <summary>
        /// Size of the space.
        /// </summary>
        public double Value { get; set; } = double.NaN;

        /// <summary>
        /// Initialize space into 10 points.
        /// </summary>
        public Space()
        {
            Value = 10;
        }

        /// <summary>
        /// Set custom step value.
        /// </summary>
        /// <param name="space">Size of step.</param>
        public Space(float space)
        {
            this.Value = space;
        }
    }
}
