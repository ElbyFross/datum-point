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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DatumPoint.Networking;
using WpfHandler.Plugins;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using PipesProvider.Client;
using UniformClient;
using PipesProvider.Networking.Routing;
using AuthorityController.Queries;

namespace DatumPoint.UI.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Public properties
        /// <summary>
        /// List that contain all loaded plugins.
        /// </summary>
        public ObservableCollection<IPlugin> Plugins { get; set; }

        /// <summary>
        /// List that contain menu's controls.
        /// </summary>
        public ObservableCollection<FrameworkElement> MenuButtons { get; set; } = new ObservableCollection<FrameworkElement>();

        /// <summary>
        /// Compute panel width relative to thw window's size.
        /// Implemented for old monitors' cases (640x480 etc.) in the poor countries.
        /// </summary>
        public double ControlPanelWidth
        {
            get
            {
                double width = 280.5d;

                double appWidth = ActualWidth;

                if (appWidth < 840)
                {
                    width = Math.Max(150, appWidth / 3);

                    width = Math.Round(width);
                    width += 0.5d;
                }

                return width;
            }
        }
        #endregion

        #region Constructors & destructors
        public MainWindow()
        {
            #region WPF Init            
            InitializeComponent();
            DataContext = this;

            // Subscribe on events
            SizeChanged += MainWindow_SizeChanged; // Window size changing.
            logonScreen.LoginCallback += LogonScreen_LoginButton; // Login button
            #endregion

            #region UniformClient Init
            // Initialize client. Also will load assemblies.
            Client.Init();

            // Load plugins.
            Plugins = API.LoadPluginsCollection();

            // Sort plugins
            API.SortByDomains(Plugins);
            #endregion

            #region Load main menu      
            // Add hardcoded UI to collection.
            foreach (FrameworkElement fe in MainMenu.Items)
            {
                MenuButtons.Add(fe);
            }

            // Connect all plugins to main menu to provide access via UI.
            foreach (IPlugin plugin in Plugins)
            {
                if (plugin.Meta != null)
                {
                    // Compute hierarchy level
                    int _hierarchyLevel = plugin.Meta.domain.Split('.').Length;

                    // Add space before paragraph.
                    if (_hierarchyLevel <= 1)
                    {
                        MenuButtons.Add(new ItemsControl() { Height = 20 });
                    }

                    // Try to load name from dictionary.
                    string title = null;
                    try
                    {
                        // load title from dictionary.
                        title = FindResource(plugin.Meta.titleDictionaryCode) as string;
                    }
                    catch
                    {
                        // Set default title or dict code if title not found.
                        title = plugin.Meta.defaultTitle ?? plugin.Meta.titleDictionaryCode;
                    }

                    // Create button by meta.
                    MenuButtons.Add(
                    new WpfHandler.UI.Controls.CatalogButton()
                    {
                        Text = title,
                        // Set uniformed text offset in hierarchy tree.
                        HierarchyLevel = _hierarchyLevel,
                        // Set root level as bool, others as thin.
                        FontWeight = _hierarchyLevel > 1 ? FontWeights.Thin : FontWeights.SemiBold,
                        // Setup plugin activator
                        ClickCallback = plugin.OnStart
                    });
                }
            }

            // Clear previos collection.
            MainMenu.Items.Clear();
            // Apply plugins to item source.
            MainMenu.ItemsSource = MenuButtons;
            #endregion
        }

        ~MainWindow()
        {
            // Unsubscribe from events.
            SizeChanged -= MainWindow_SizeChanged;
            logonScreen.LoginCallback -= LogonScreen_LoginButton;
        }
        #endregion

        #region Callbacks
        /// <summary>
        /// Callback that will has been calling when widow size will be changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Update size of control panel.
            BindingOperations.GetBindingExpression(controlPanelColumn, ColumnDefinition.WidthProperty).UpdateTarget();
        }

        /// <summary>
        /// Callback that will has been calling when main window would be loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Open first plugin.
            WpfHandler.Plugins.API.OpenGUI(Plugins[0]);
        }

        /// <summary>
        /// Handle logon process.
        /// </summary>
        /// <param name="sender"></param>
        private void LogonScreen_LoginButton(object sender)
        {
            // Lock screen until lockon confiramtion. 
            overlay.Lock("Authorization", main);//, controlPanel, canvas, logonScreen);

            // Detecting routing instruction suitable for user's queries.
            Client.Active.RoutingTable.TryGetRoutingInstruction("token&guid&user&logon", out Instruction instruction);
            if (!(instruction is PartialAuthorizedInstruction queriesChanelInstruction))
            {
                // Enable routing error message.
                MessageBox.Show("Routing instruction for LOGON query not found.\n" +
                    "Please bw sure that you has PartialAuthorizedInstruction that allow to share queries with user&logon parts");
                return;
            }

            // Create new processor that provide logon operation.
            USER_LOGON.LogonProcessor processor = new USER_LOGON.LogonProcessor();

            // Sign up on callback that would be called when logon operation would be passed.
            processor.ProcessingFinished += LogonCallback;

            // Request logon.
            processor.TryToLogonAsync(
                queriesChanelInstruction.GuestToken,
                logonScreen.Login,
                logonScreen.Password,
                queriesChanelInstruction.routingIP,
                queriesChanelInstruction.pipeName);

            // Callback that would be called when server returns answer.
            void LogonCallback(
                UniformQueries.Executable.QueryProcessor _,
                bool result,
                object message)
            {
                // Unsubscribe.
                processor.ProcessingFinished -= LogonCallback;

                // Unlock overlay.
                overlay.Unlock();

                // If success logoned.
                if (result)
                {
                    //message as string;


                    // TODO Disable logon menu.
                }
                else
                {
                    // TODO Show up error message.
                }
            };
        }
        #endregion
    }
}
