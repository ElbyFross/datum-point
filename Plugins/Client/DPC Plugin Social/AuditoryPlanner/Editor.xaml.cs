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
using WpfHandler.Plugins;

namespace DatumPoint.Plugins.Social.AuditoryPlanner
{
    /// <summary>
    /// Visual constructor for creating of the auditorium's shemas.
    /// </summary>
    public partial class Editor : UserControl, WpfHandler.Plugins.IPlugin
    {
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title", typeof(string), typeof(Editor));

        public static readonly DependencyProperty SchemaProperty = DependencyProperty.Register(
            "Schema", typeof(Types.AuditoryPlanner.Schema), typeof(Editor));

        /// <summary>
        /// Title of the schema.
        /// </summary>
        public string Title
        {
            get { return (string)this.GetValue(TitleProperty); }
            set { this.SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// Conected schema.
        /// </summary>
        public Types.AuditoryPlanner.Schema Schema
        {
            get { return (Types.AuditoryPlanner.Schema)this.GetValue(SchemaProperty); }
            set { this.SetValue(SchemaProperty, value); }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Editor()
        {
            InitializeComponent();
            DataContext = this; // Setup values to meta.

            Meta.domain = "0_main.900_shemaEditor";
            Meta.titleDictionaryCode = "p_podshyvalov_shemaEditor_menuTitle";
        }

        public MenuItemMeta Meta { get; set; } = new MenuItemMeta();

        /// <summary>
        /// Callback that will be called when plugin will be started by user.
        /// Use to init data.
        /// </summary>
        /// <param name="sender"></param>
        public void OnStart(object sender)
        {
            // Request changing of GUI.
            API.OpenGUI(this);
        }

        /// <summary>
        /// Callback for info button.
        /// </summary>
        /// <param name="sender"></param>
        public void InfoCallback(object sender)
        {
            #region Enable animation
            // Block input.
            helpOverlayBackplate.IsHitTestVisible = true;

            var duration = new TimeSpan(0, 0, 0, 0, 200);

            WpfHandler.UI.Animations.Blur.BlurApply(
                workspace, 7, new TimeSpan(0, 0, 0, 0, 100), new TimeSpan(),
                System.Windows.Media.Animation.FillBehavior.HoldEnd);

            // Activate backplate
            WpfHandler.UI.Animations.Float.FloatAniamtion(
                this,
                helpOverlayBackplate.Name,
                new PropertyPath(Control.OpacityProperty),
                duration,
                System.Windows.Media.Animation.FillBehavior.HoldEnd,
                0, 0.6f);

            // Activate UI.
            WpfHandler.UI.Animations.Float.FloatAniamtion(
                this,
                helpOverlay.Name,
                new PropertyPath(Control.OpacityProperty),
                duration,
                System.Windows.Media.Animation.FillBehavior.HoldEnd,
                0, 1);
            #endregion

            this.MouseDown += Editor_MouseDown;
        }

        private void Editor_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.MouseDown -= Editor_MouseDown;

            #region Enable animation
            // Block input.
            helpOverlayBackplate.IsHitTestVisible = false;

            var duration = new TimeSpan(0, 0, 0, 0, 200);

            WpfHandler.UI.Animations.Blur.BlurDisable(
                workspace, duration, new TimeSpan(), null);

            // Activate backplate
            WpfHandler.UI.Animations.Float.FloatAniamtion(
                this,
                helpOverlayBackplate.Name,
                new PropertyPath(Control.OpacityProperty),
                duration,
                System.Windows.Media.Animation.FillBehavior.HoldEnd,
                0.6f, 0);

            // Activate UI.
            WpfHandler.UI.Animations.Float.FloatAniamtion(
                this,
                helpOverlay.Name,
                new PropertyPath(Control.OpacityProperty),
               duration,
                System.Windows.Media.Animation.FillBehavior.HoldEnd,
                1, 0);
            #endregion
        }
    }
}
