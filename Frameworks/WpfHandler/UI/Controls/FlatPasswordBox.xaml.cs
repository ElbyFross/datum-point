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
using WpfHandler.UI.Controls.AutoLayout.Interfaces;
using System.Reflection;
using WpfHandler.UI.Controls.AutoLayout;

namespace WpfHandler.UI.Controls
{
    /// <summary>
    /// Interaction logic for FlatPasswordBox.xaml
    /// </summary>
    public partial class FlatPasswordBox : UserControl, ILayoutControl
    {
        #region Dependency properties
        public static readonly DependencyProperty LableProperty = DependencyProperty.Register(
          "Lable", typeof(string), typeof(FlatPasswordBox));

        public static readonly DependencyProperty LableWidthProperty = DependencyProperty.Register(
          "LableWidth", typeof(float), typeof(FlatPasswordBox));

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
          "Text", typeof(string), typeof(FlatPasswordBox));
        
        public static readonly DependencyProperty TextBoxForegroundProperty = DependencyProperty.Register(
          "TextBoxForeground", typeof(Brush), typeof(FlatPasswordBox));

        public static readonly DependencyProperty TextBoxBackgroundProperty = DependencyProperty.Register(
          "TextBoxBackground", typeof(Brush), typeof(FlatPasswordBox));
        #endregion

        #region Properties
        /// <summary>
        /// Event that will occure in case if value of the field will be changed.
        /// Will cause updating of the BindedMember value.
        /// </summary>
        public event Action<ILayoutControl> ValueChanged;

        /// <summary>
        /// Memeber that will be used as source\target for the value into UI.
        /// </summary>
        public MemberInfo BindedMember { get; set; }

        /// <summary>
        /// Text in lable field.
        /// </summary>
        public string Lable
        {
            get { return (string)this.GetValue(LableProperty); }
            set { this.SetValue(LableProperty, value); }
        }

        /// <summary>
        /// Width of lable field.
        /// </summary>
        public float LableWidth
        {
            get { return (float)this.GetValue(LableWidthProperty); }
            set { this.SetValue(LableWidthProperty, value); }
        }

        /// <summary>
        /// Text in textbox.
        /// </summary>
        public string Text
        {
            get { return passwordBox.Password; }
            set { passwordBox.Password = value; }
        }

        /// <summary>
        /// Color of the text in textbox.
        /// </summary>
        public Brush TextBoxForeground
        {
            get { return (Brush)this.GetValue(TextBoxForegroundProperty); }
            set { this.SetValue(TextBoxForegroundProperty, value); }
        }

        /// <summary>
        /// Collor of the text box backplate.
        /// </summary>
        public Brush TextBoxBackground
        {
            get { return (Brush)this.GetValue(TextBoxBackgroundProperty); }
            set { this.SetValue(TextBoxBackgroundProperty, value); }
        }

        /// <summary>
        /// Uniformaed value of the field.
        /// Allow only strings.
        /// </summary>
        public object Value
        {
            get { return Text; }
            set
            {
                if (value is string s)
                {
                    Text = s;
                }
            }
        }
        #endregion

        /// <summary>
        /// Defalut constructor.
        /// 
        /// Trying to load `FlatPasswordBox` as @Style resource.
        /// </summary>
        public FlatPasswordBox()
        {
            InitializeComponent();
            DataContext = this;

            // Try to load default style
            try
            {
                if (Application.Current.FindResource("FlatPasswordBox") is Style style)
                {
                    this.Style = style;
                }
            }
            catch
            {
                // Not found in dictionary. Not important.}
            }
        }

        /// <summary>
        /// Callback that will occure when password will changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            // Inform autolayout handler about changes.
            ValueChanged?.Invoke(this);
        }

        public void OnGUI(ref LayoutLayer layer, params object[] args)
        {
            throw new NotImplementedException();
        }
    }
}
