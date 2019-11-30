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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfHandler.UI.AutoLayout;
using WpfHandler.UI.AutoLayout.Configuration;

namespace WpfHandler.UI.Controls
{
    /// <summary>
    /// TODO: Operating by group of the toggles.
    /// </summary>
    [TypesCompatible(typeof(Object))]
    [EnumsCompatible]
    public partial class FlatTogglesGroup : UserControl, ILayoutOrientation, ILabel, IGUIField
    {
        /// <summary>
        /// Property that bridging control's property between XAML and code.
        /// </summary>
        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(
          "Label", typeof(string), typeof(FlatTogglesGroup));

        /// <summary>
        /// Property that bridging control's property between XAML and code.
        /// </summary>
        public static readonly DependencyProperty LabelWidthProperty = DependencyProperty.Register(
          "LabelWidth", typeof(float), typeof(FlatTogglesGroup));


        /// <summary>
        /// Enum type that would be used like a source of values.
        /// </summary>
        public Type SourceEnum { get; set; }

        /// <summary>
        /// Layout orientation of the UI elements.
        /// </summary>
        public Orientation Orientation
        {
            get { return _Orientation; }
            set
            {
                // Updating value.
                _Orientation = value;

                // TODO: Updating layout.
            }
        }


        /// <summary>
        /// Text in label field.
        /// </summary>
        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        /// <summary>
        /// Width of label field.
        /// </summary>
        public float LabelWidth
        {
            get { return (float)GetValue(LabelWidthProperty); }
            set
            {
                // Buferize requested value.
                _LabelWidth = value;

                // Set value but apply at least 25 point to input field.
                float appliedSize = (float)Math.Min(_LabelWidth, ActualWidth - 25);

                // Appling value.
                SetValue(LabelWidthProperty, appliedSize);
            }
        }

        /// <summary>
        /// Value of that control.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Memeber that will be used as source\target for the value into UI.
        /// </summary>
        public MemberInfo BindedMember { get; set; }

        /// <summary>
        /// Bufer that contains las requested label width.
        /// </summary>
        protected float _LabelWidth;

        /// <summary>
        /// Bufer that contains current layout orientation.
        /// </summary>
        protected Orientation _Orientation = Orientation.Vertical;

        /// <summary>
        /// Event that will occure in case if value of the field will be changed.
        /// Will cause updating of the BindedMember value.
        /// 
        /// IGUIField - sender.
        /// </summary>
        public event Action<IGUIField> ValueChanged;

        /// <summary>
        /// Initialize component.
        /// </summary>
        public FlatTogglesGroup()
        {
            InitializeComponent();
            DataContext = this;
        }

        /// <summary>
        /// Modify current layer's layout according to GUI element requirments.
        /// Calls once during UI spawn.
        /// </summary>
        /// <param name="layer">Target GUI layer.</param>
        /// <param name="args">Shared arguments. Must contins <see cref="UIDescriptor"/> and</param>
        public void OnGUI(ref LayoutLayer layer, params object[] args)
        {
            #region Getting shared data
            // Find required referendes.
            UIDescriptor desc = null;
            MemberInfo member = null;

            // Trying to get shared properties.
            foreach (object obj in args)
            {
                if (obj is UIDescriptor) desc = (UIDescriptor)obj;
                if (obj is MemberInfo) member = (MemberInfo)obj;
            }
            #endregion
            
            #region Bind as member
            Type memberType = UIDescriptor.MembersHandler.GetSpecifiedMemberType(member);
            #endregion
        }
    }
}
