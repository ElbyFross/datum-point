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
        public ObservableCollection<FrameworkElement> MenuButtons { get; set; } = 
            new ObservableCollection<FrameworkElement>();

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
            logonScreen.LogonPanel_LoginCallback += LogonScreen_LoginButton; // Login button
            logonScreen.RegPanel_ContinueCallback += LogonScreen_RegistrationButton; // Registration button
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
        #endregion

        #region Winsow callbacks
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
        /// Callback that will has been calling during widow closing.
        /// Will terminate all registred async operations, stop transmissions, stop server instances.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Unsubscribe from events.
            SizeChanged -= MainWindow_SizeChanged;
            try { logonScreen.LogonPanel_LoginCallback -= LogonScreen_LoginButton; } catch { }
            try { logonScreen.RegPanel_ContinueCallback -= LogonScreen_RegistrationButton; } catch { }

            // Close lines.
            ClientAPI.CloseAllTransmissionLines();
            // Close servers.
            PipesProvider.Server.ServerAPI.StopAllServers();

            // Terminate async operations.
            AuthorityController.Session.Current.TerminationTokenSource.Cancel();
            BaseClient.TerminationTokenSource.Cancel();
        }
        #endregion

        #region Logon screen callbacks
        /// <summary>
        /// Handle logon process.
        /// </summary>
        /// <param name="sender"></param>
        private async void LogonScreen_LoginButton(object sender)
        {
            // Lock screen until lockon confiramtion. 
            overlay.Lock("Authorization", main);//, controlPanel, canvas, logonScreen);

            // Detecting routing instruction suitable for user's queries.
            BaseClient.routingTable.TryGetRoutingInstruction(
                new UniformQueries.Query(
                    new UniformQueries.QueryPart("logon"),
                    new UniformQueries.QueryPart("user"),
                    new UniformQueries.QueryPart("token"),
                    new UniformQueries.QueryPart("guid")),
                out Instruction instruction);
            if (!(instruction is AuthorizedInstruction queriesChanelInstruction))
            {
                // Enable routing error message.
                MessageBox.Show("Routing instruction for LOGON query not found.\n" +
                    "Please bw sure that you has PartialAuthorizedInstruction that allow to share queries with user&logon parts");
                return;
            }

            // Set entry data.
            queriesChanelInstruction.authLogin = logonScreen.logonPanel.Login;
            queriesChanelInstruction.authPassword = logonScreen.logonPanel.Password;

            // Request logon
            queriesChanelInstruction.TryToLogonAsync(null, AuthorityController.Session.Current.TerminationTokenSource.Token);
            queriesChanelInstruction.LogonHandler.ProcessingFinished += LogonFinishedCallback;

            bool answerReceived = false;
            bool logonResult = false;
            object sharedMessage = null;

            // Callback that would be called when server returns answer.
            void LogonFinishedCallback(UniformQueries.Executable.QueryProcessor _, bool result, object message)
            {
                // Unsubscribe from events.
                queriesChanelInstruction.LogonHandler.ProcessingFinished -= LogonFinishedCallback;

                // Buferize result.
                logonResult = result;
                sharedMessage = message;

                // Unlocking thread.
                answerReceived = true;

            }

            // Waiting thread.
            while(!answerReceived)
            {
                await Task.Delay(5);
                //System.Threading.Thread.Sleep(5);
            }

           
            // Clear data.
            queriesChanelInstruction.authLogin = null;
            queriesChanelInstruction.authPassword = null;
            
            // Unlock overlay. Drop if operation was canceled
            try { overlay.Unlock(); } catch { return; }

            // Wait till end of blue animation.
            await Task.Delay(overlay.lockAnimationDuration.Add(overlay.lockAnimationDuration));

            // If success logoned.
            if (queriesChanelInstruction.IsFullAuthorized)
            {
                await DisableLogonScreenAsync();

                // TODO Impersionate like authorized user.
            }
            else
            {
                // Drop auth data.
                logonScreen.logonPanel.Password = "";
            }            

            // Operate shared message if exist.
            if (sharedMessage != null)
            {
                #region Try to convert message to string
                string stringMessage = null;
                if (sharedMessage is UniformQueries.QueryPart messageQP)
                {
                    stringMessage = messageQP.PropertyValueString;
                }
                else if(sharedMessage is string messageS)
                {
                    stringMessage = messageS;
                }
                #endregion

                #region Log message
                if (!string.IsNullOrEmpty(stringMessage))
                {
                    // If received error.
                    if (stringMessage.StartsWith("error", StringComparison.OrdinalIgnoreCase))
                    {
                        int messageStartIndex = stringMessage.IndexOf(':');
                        if (messageStartIndex != -1 && stringMessage.Length > messageStartIndex + 1)
                        {
                            logonScreen.logonPanel.ErrorMessage = stringMessage.Substring(messageStartIndex + 1).Trim();
                        }
                        else
                        {
                            logonScreen.logonPanel.ErrorMessage = stringMessage;
                        }
                    }
                }
                #endregion
            }
        }

        /// <summary>
        /// Handle registration process.
        /// </summary>
        /// <param name="sender"></param>
        private async void LogonScreen_RegistrationButton(object sender)
        {
            #region Validate data
            bool IsFieledsFilled = true; // Marker that contain surfacly data validation to prevent braking of base rules.

            if (string.IsNullOrEmpty(logonScreen.registrationPanel.Login)) IsFieledsFilled = false;
            if (string.IsNullOrEmpty(logonScreen.registrationPanel.Password)) IsFieledsFilled = false;
            if (string.IsNullOrEmpty(logonScreen.registrationPanel.PasswordConfirmation)) IsFieledsFilled = false;
            if (string.IsNullOrEmpty(logonScreen.registrationPanel.FirstName)) IsFieledsFilled = false;
            if (string.IsNullOrEmpty(logonScreen.registrationPanel.LastName)) IsFieledsFilled = false;

            // Enable/disable fields fill error.
            logonScreen.registrationPanel.FillAllFieldErrorLable = !IsFieledsFilled;

            // Enable/disable passwords matching error.
            bool passwordsValid = logonScreen.registrationPanel.IsPasswordsTheSame;
            logonScreen.registrationPanel.PasswordNotMatchErrorLable = !passwordsValid;

            // Drop if invalid.
            if(!passwordsValid || !IsFieledsFilled) return;
            #endregion

            #region Registrate user
            // Lock screen until lockon confiramtion. 
            overlay.Lock("Authorization", main);//, controlPanel, canvas, logonScreen);

            // Detecting routing instruction suitable for user's queries.
            BaseClient.routingTable.TryGetRoutingInstruction(
                new UniformQueries.Query(
                    new UniformQueries.QueryPart("new"),
                    new UniformQueries.QueryPart("user"),
                    new UniformQueries.QueryPart("token"),
                    new UniformQueries.QueryPart("guid")),
                out Instruction instruction);
            if (!(instruction is AuthorizedInstruction queriesChanelInstruction))
            {
                // Enable routing error message.
                MessageBox.Show("Routing instruction for LOGON query not found.\n" +
                    "Please be sure that you has PartialAuthorizedInstruction that allow to share queries with user&logon parts");
                return;
            }

            // Check if instruction is has authorized guest token.
            if (!queriesChanelInstruction.IsPartialAuthorized)
            {
                // Try to logon.
                if (await queriesChanelInstruction.TryToGetGuestTokenAsync(
                   AuthorityController.Session.Current.TerminationTokenSource.Token))
                {
                    await SendRegistrationCallback();
                }
            }
            else
            {
                await SendRegistrationCallback();
            }

            // Unlock overlay.
            overlay.Unlock();
            #endregion
            
            #region Send registration query
            // Method that build and send query with registration data to server.
            async Task SendRegistrationCallback()
            {
                // bufer that would contain received answer from server.
                UniformQueries.Query receivedAnswer = null;

                try
                {
                    // Send query to server.
                    BaseClient.EnqueueDuplexQueryViaPP(
                        queriesChanelInstruction.routingIP,
                        queriesChanelInstruction.pipeName,
                        new UniformQueries.Query(
                            // Enable encryption
                            new UniformQueries.Query.EncryptionInfo(),

                            // Add query stamp.
                            new UniformQueries.QueryPart("token", queriesChanelInstruction.GuestToken),
                            new UniformQueries.QueryPart("guid", "logScr_RegReq"),

                            // Add core data.
                            new UniformQueries.QueryPart("new"),
                            new UniformQueries.QueryPart("user"),
                            new UniformQueries.QueryPart("login", logonScreen.registrationPanel.Login),
                            new UniformQueries.QueryPart("password", logonScreen.registrationPanel.Password),

                            // Add personal info.
                            new UniformQueries.QueryPart("fn", logonScreen.registrationPanel.FirstName),
                            new UniformQueries.QueryPart("ln", logonScreen.registrationPanel.LastName),

                            // Add device stamp.
                            new UniformQueries.QueryPart("os", Environment.OSVersion.VersionString),
                            new UniformQueries.QueryPart("mac", PipesProvider.Networking.Info.MacAdsress),
                            new UniformQueries.QueryPart("stamp", DateTime.Now.ToBinary().ToString())
                        ),
                        delegate (TransmissionLine line, UniformQueries.Query answer)
                        {
                            receivedAnswer = answer;
                        });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                // Waiting for server answer.
                while (receivedAnswer == null)
                {
                    await Task.Delay(5);
                }

                #region Log error
                // Check if error received.
                string errorMessage = null;
                try { errorMessage = receivedAnswer.First.PropertyValueString; } catch { }

                // If received error.
                if (errorMessage != null &&
                    errorMessage.StartsWith("error", StringComparison.OrdinalIgnoreCase))
                {
                    int messageStartIndex = errorMessage.IndexOf(':');
                    if (messageStartIndex != -1 && errorMessage.Length > messageStartIndex + 1)
                    {
                        logonScreen.registrationPanel.ErrorMessage = errorMessage.Substring(messageStartIndex + 1);
                    }
                    else
                    {
                        logonScreen.registrationPanel.ErrorMessage = errorMessage;
                    }
                }
                #endregion
                else
                {
                    // Disable logon screen.
                    await DisableLogonScreenAsync();

                    // TODO Impersionate like authorized user.
                }
            }
            #endregion
        }
        #endregion

        /// <summary>
        /// Disabling logen screen and open acess to main window.
        /// </summary>
        protected async Task DisableLogonScreenAsync()
        {
            // Drop auth data.
            logonScreen.Clear();

            //Disable logon menu.
            //logonScreen.IsHitTestVisible = false;

            // Hide panel.
            WpfHandler.UI.Animations.Thinkness.ThinknessAniamtion(
                this,
                logonScreen.Name,
                new PropertyPath(Control.MarginProperty),
                new TimeSpan(0, 0, 0, 0, 500),
                new Thickness(0, -0.5f, 0, 0),
                new Thickness(0, -0.5f - main.ActualHeight - 5, 0, main.ActualHeight + 5),
                FillBehavior.HoldEnd);

            await Task.Delay(new TimeSpan(0, 0, 0, 0, 250));

            // Hide curtain
            WpfHandler.UI.Animations.Float.FloatAniamtion(
                this,
                curtain.Name,
                new PropertyPath(Control.OpacityProperty),
                new TimeSpan(0, 0, 0, 0, 350),
                FillBehavior.HoldEnd,
                1, 0);
        }
    }
}
