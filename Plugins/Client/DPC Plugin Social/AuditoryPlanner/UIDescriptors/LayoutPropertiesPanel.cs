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
using WpfHandler.UI.AutoLayout.Attributes.Elements;
using WpfHandler.UI.AutoLayout.Attributes.Layout;
using WpfHandler.UI.AutoLayout;

namespace DatumPoint.Plugins.Social.AuditoryPlanner.UIDescriptors
{
    public class LayoutPropertiesPanel : UIDescriptor
    {
        [Header("Test header", "testheader")]
        public string testString = "field one";

        [Order(2)]
        public string TestStringProp { get; set; } = "prop test 2";

        public float testFloat = 4;
    }
}
