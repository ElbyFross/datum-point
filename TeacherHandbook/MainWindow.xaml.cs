// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// Copyright (c) 2019 Volodymyr Podshyvalov

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

namespace TeacherHandbook
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
        public ObservableCollection<UniformClient.Plugins.IPlugin> Plugins { get; set; }

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
            SizeChanged += MainWindow_SizeChanged;
            #endregion

            #region UniformClient Init
            // Initialize client. Also will load assemblies.
            Client.Init();

            // Load plugins.
            Plugins = Client.LoadPluginsCollection();

            // Sort plugins
            UniformClient.Plugins.API.SortByDomains(Plugins);
            #endregion

            #region Load main menu      
            // Add hardcoded UI to collection.
            foreach (FrameworkElement fe in MainMenu.Items)
            {
                MenuButtons.Add(fe);
            }

            // Connect all plugins to main menu to provide access via UI.
            foreach (UniformClient.Plugins.IPlugin plugin in Plugins)
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
                    new Controls.CatalogButton()
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
        }
        #endregion


        #region Callbacks
        /// <summary>
        /// Callback that will be called when widow size will be changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Update size of control panel.
            BindingOperations.GetBindingExpression(controlPanelColumn, ColumnDefinition.WidthProperty).UpdateTarget();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Open first plugin.
            WpfHandler.Plugins.API.OpenGUI(Plugins[0]);
        }
        #endregion
    }
}
