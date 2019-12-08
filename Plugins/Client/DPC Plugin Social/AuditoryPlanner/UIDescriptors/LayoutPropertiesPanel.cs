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

using System.Windows.Controls;
using WpfHandler.UI.AutoLayout.Configuration;
using WpfHandler.UI.AutoLayout.Controls;
using WpfHandler.UI.AutoLayout.Options;
using WpfHandler.UI.AutoLayout;
using WpfHandler.UI;

namespace DatumPoint.Plugins.Social.AuditoryPlanner.UIDescriptors
{
    [Foreground("WhiteSmoke")]
    [AutoCollectionProperties(BackplateBackground = "#00A8E8", SpliterColor = "#007ea7")]
    public class LayoutPropertiesPanel : UIDescriptor
    {
        public enum Modes
        {
            Normal,
            Advanced,
            Pro
        }

        public enum State
        {
            On,
            Off
        }

        public string disorderedField = "s-ad2";

        [Order(0)]
        public string a = "abs";

        [Order(10)]
        [EndGroup]
        [Header("Test header", "testheader")]
        public string testString2 = "field one";

        [Order(20)]
        [Space]
        [Content("TEST CUSTOM")]
        public string TestStringProp { get; set; } = "prop test 2";

        [Order(30)]
        [Height(40)]
        [LabelWidth(50)]
        public float testFloat = 4;


        [Order(110)]
        [EndGroup]
        [Header("TEST HEADER 2", "testheader2", DefaultState = false)]
        public float testFloat223sd = 4;

        [Foreground("Yellow")]
        public Modes enumField = Modes.Advanced;

        [Content]
        [Orientation(Orientation.Horizontal)]
        public Modes enumFieldRaw = Modes.Pro;

        [Orientation(Orientation.Horizontal)]
        [Content("STATE")]
        [Content("Вкл")]
        public State state = State.Off;

        [EndGroup]
        [Header("Collections test", "testheader2")]
        [Order(-3)]
        public List<object> objCollection = new List<object>();

        [Order(-2)]
        [AutoCollectionProperties(AddButtonVisibile = false)]
        public List<int> intCollection = new List<int>();

        [Order(-1)]
        [AutoCollectionProperties(
            BackplateBackground = "WhiteSmoke",
            SpliterColor = "LightGray", 
            SplitersDraw = false)]
        public string[] stringArray = new string[] { "1", "2", "3", "4", "5" };

        public List<object> callbackHandling = new List<object>();

        public LayoutPropertiesPanel()
        {
            objCollection.Add(Modes.Pro);
            objCollection.Add(43);
            objCollection.Add("String");

            Loaded += LayoutPropertiesPanel_Loaded;
        }

        private void LayoutPropertiesPanel_Loaded(UIDescriptor obj)
        {
            var field = GetFieldByMember("callbackHandling");

            var ac = field as WpfHandler.UI.Controls.AutoCollection;
            ac.OnAddClick += delegate (object sender)
            {
                System.Windows.MessageBox.Show("Custom add");
            };

            ac.OnRemoveClick += delegate (object sender)
            {
                System.Windows.MessageBox.Show("Custom remove");
            };
        }
    }
}
