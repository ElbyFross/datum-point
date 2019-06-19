// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// Copyright (c) 2019 Volodymyr Podshyvalov

using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Threading;
using System.IO;
using System.Diagnostics;

namespace UniformClient
{
    /// <summary>
    /// Class that provide base client features and envirounment static API.
    /// </summary>
    public abstract class BaseClient
    {
        #region Static fields and properties
        /// <summary>
        /// Imported method that allo to controll console window state.
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="nCmdShow"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        protected static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        /// <summary>
        /// Inported method that allow acces to console window.
        /// </summary>
        /// <returns></returns>
        [DllImport("Kernel32")]
        protected static extern IntPtr GetConsoleWindow();

        /// <summary>
        /// Argument that will hide console window.
        /// </summary>
        protected const int SW_HIDE = 0;

        /// <summary>
        /// Agrument that will show console window.
        /// </summary>
        protected const int SW_SHOW = 5;

        /// <summary>
        /// How many milisseconds will sleep thread after tick.
        /// </summary>
        protected static int threadSleepTime = 150;

        /// <summary>
        /// If true then will stop main loop.
        /// </summary>
        public static bool appTerminated;
        #endregion

        /// <summary>
        /// Reference to thread that host this server.
        /// </summary>
        public Thread thread;

        /// <summary>
        /// Loading assemblies from requested path.
        /// </summary>
        /// <param name="path"></param>
        protected static void LoadAssemblies(string path)
        {
            // Validate directory.
            bool dirExist = Directory.Exists(path);
            if (!dirExist)
            {
                Console.WriteLine("Libs directory not found. Creating new one...\n{0}", path);
                Directory.CreateDirectory(path);
                Console.WriteLine("");
            }

            // Search files in directory.
            string[] dllFiles = Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories);

            // Loading assemblies.
            if (dllFiles.Length > 0)
            {
                Console.WriteLine("ASSEMBLIES DETECTED:");
            }
            foreach (string _path in dllFiles)
            {
                try
                {
                    Assembly.LoadFrom(_path);
                    Console.WriteLine(_path.Substring(_path.LastIndexOf("\\") + 1));
                }
                catch(Exception ex)
                {
                    Console.WriteLine("DLL \"{0}\" LOADING FAILED: {1}", 
                        _path.Substring(_path.LastIndexOf("\\") + 1), 
                        ex.Message);
                }
            }

            if (dllFiles.Length > 0)
            {
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Method that will configurate application and server relative to the uniform arguments.
        /// </summary>
        /// <param name="args"></param>
        protected static void ArgsReactor(string[] args)
        {
            // Get a pointer to this console.
            IntPtr hwnd = GetConsoleWindow();

            // Change window state.
            ShowWindow(hwnd, SW_SHOW);

            // Check every argument.
            foreach (string s in args)
            {
                // Hide application from tray.
                if (s == "hide")
                {
                    ShowWindow(hwnd, SW_HIDE);
                    continue;
                }
            }
        }

        /// <summary>
        /// Method that starting client thread.
        /// </summary>
        /// <param name="threadName"></param>
        /// <param name="sharebleParam"></param>
        /// <param name="clientLoop"></param>
        /// <returns></returns>
        protected virtual Thread StartClientThread(
            string threadName,
            object sharebleParam,
            ParameterizedThreadStart clientLoop)
        {
            // Abort started thread if exits.
            if(thread != null && thread.IsAlive)
            {
                Console.WriteLine("THREAD MANUAL ABORTED (BC_SCT_0): {0}", thread.Name);
                thread.Abort();
            }

            // Initialize queries monitor thread.
            thread = new Thread(clientLoop)
            {
                Name = threadName
            };

            // Start thread
            thread.Start(sharebleParam);

            // Let it proceed first run.
            Thread.Sleep(threadSleepTime);

            return thread;
        }


        /// <summary>
        /// Load plugins from assembly and instiniate them to list.
        /// </summary>
        /// <returns></returns>
        public static System.Collections.Generic.IEnumerable<Plugins.IPlugin> LoadPluginsEnumerable()
        {
            // Load query's processors.
            System.Reflection.Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            //Console.WriteLine("ASSEMBLIES PROCEED: {0}\n", assemblies.Length);
            Console.WriteLine("\nDETECTED PLUGINS:");
            foreach (System.Reflection.Assembly assembly in assemblies)
            {
                // Get all types for assembly.
                foreach (System.Type type in assembly.GetTypes())
                {
                    // Check if this type is subclass of query.
                    if (type.GetInterface("IPlugin") != null)
                    {
                        // Instiniating querie processor.
                        Plugins.IPlugin instance = (Plugins.IPlugin)Activator.CreateInstance(type);
                        Console.WriteLine("{0}", type.Name);
                        yield return instance;
                    }
                }
            }
        }

        /// <summary>
        /// Load plugins from assembly and instiniate them to list.
        /// </summary>
        /// <param name="list"></param>
        public static System.Collections.ObjectModel.ObservableCollection<Plugins.IPlugin> LoadPluginsCollection()
        {
            System.Collections.ObjectModel.ObservableCollection<Plugins.IPlugin> collection = new System.Collections.ObjectModel.ObservableCollection<Plugins.IPlugin>();

            // Load query's processors.
            System.Reflection.Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            //Console.WriteLine("ASSEMBLIES PROCEED: {0}\n", assemblies.Length);
            Console.WriteLine("\nDETECTED PLUGINS:");
            foreach (System.Reflection.Assembly assembly in assemblies)
            {
                // Get all types for assembly.
                foreach (System.Type type in assembly.GetTypes())
                {
                    // Check if this type is subclass of query.
                    if (type.GetInterface("IPlugin") != null)
                    {
                        // Instiniating querie processor.
                        Plugins.IPlugin instance = (Plugins.IPlugin)Activator.CreateInstance(type);
                        collection.Add(instance);
                        Console.WriteLine("{0}", type.Name);
                    }
                }
            }
            return collection;
        }
        
        /// <summary>
        /// Provide complex initalization of all relative systems. 
        /// Build meta data, regitrate line in common table.
        /// Start new thread to avoid freezes.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="guid"></param>
        /// <param name="serverName"></param>
        /// <param name="pipeName"></param>
        /// <param name="callback"></param>
        /// <returns>Opened transmission line. Use line.Enqueue to add your query.</returns>
        public static PipesProvider.TransmissionLine OpenTransmissionLine(
            BaseClient client,
            string guid,
            string serverName,
            string pipeName,
            System.Action<object> callback)
        {
            // Validate client.
            if(client == null)
            {
                Console.WriteLine("CLIENT is NULL (BC_OTL_0). Unable to open new transmission line.");
                return null;
            }

            // Create new line processor.
            PipesProvider.TransmissionLine trnsLine = new PipesProvider.TransmissionLine(
                guid,
                serverName,
                pipeName,
                callback
                );

            // Put line proccesor to the new client loop.
            client.StartClientThread(
                guid,
                trnsLine,
                PipesProvider.TransmissionLine.ThreadLoop);

            // Return oppened line.
            return trnsLine;
        }
    }
}