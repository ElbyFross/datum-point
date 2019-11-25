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
using WpfHandler.UI.AutoLayout;
using WpfHandler.UI.AutoLayout.Attributes;
using WpfHandler.UI.AutoLayout.Attributes.Elements;

namespace DatumPoint.Plugins.Social.AuditoryPlanner.UIDescriptors
{
    /// <summary>
    /// Class that describe UI member of Auditory members panel.
    /// </summary>
    public class EditingModesPanel : UIDescriptor
    {
        public enum EditingModes
        { 
            Normal,
            Hide,
            Block
        }

        [HeaderAttribute("MODES", "p_podshyvalov_shemaEditor_editingModesPanel_Header")]
        public EditingModes mode;
    }
}
