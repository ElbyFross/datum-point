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
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfHandler.UI.AutoLayout;

namespace WpfHandler.UI.Controls
{
    /// <summary>
    /// TODO: Operating by group of the toggles.
    /// </summary>
    public partial class TogglesGroup : UserControl, ILayoutOrientation, ILabel
    {
        /// <summary>
        /// Descriptor of the auto generated GUI for toogle group items.
        /// </summary>
        public class ContentDescriptor : AutoLayout.UIDescriptor
        {
            /// <summary>
            /// Intems binded to the auto-layout GUI.
            /// </summary>
            public readonly List<GUIContent> Items = new List<GUIContent>();
        }

        /// <summary>
        /// Enum type that would be used like a source of values.
        /// </summary>
        public Type SourceEnum { get; set; }

        /// <summary>
        /// TODO: Layout orientation of the UI elements.
        /// </summary>
        public Orientation Orientation { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <summary>
        /// TODO: Provides acess to the label of the list.
        /// </summary>
        public string Label { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <summary>
        /// Width of the prefix lable.
        /// </summary>
        public float LabelWidth { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <summary>
        /// Auto-generated content builded by UI descriptor.
        /// </summary>
        protected ContentDescriptor Descriptor = new ContentDescriptor();

        /// <summary>
        /// Initialize component.
        /// </summary>
        public TogglesGroup()
        {
            InitializeComponent();
            DataContext = this;
        }

        /// <summary>
        /// Callback that will occure when control will be loaded.
        /// Binding auto-generated content to the XAML.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Descriptor.BindTo(root);
        }
    }
}
