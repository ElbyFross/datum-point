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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfHandler.UI.Controls.AutoLayout.Interfaces
{
    /// <summary>
    /// Define memebers of all elemtes that affects layout layer's state.
    /// </summary>
    public interface IGUIElement
    {
        /// <summary>
        /// Modify current layer root according to GUI element requirments.
        /// </summary>
        /// <param name="layer">Target GUI layer.</param>
        /// <param name="args">Shared arguments.</param>
        void OnGUI(ref LayoutLayer layer, params object[] args);
    }
}
