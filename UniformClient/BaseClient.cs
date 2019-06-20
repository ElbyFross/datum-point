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



        #region Transmission API
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
            System.Action<PipesProvider.TransmissionLine> callback)
        {
            // Validate client.
            if (client == null)
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


        /// <summary>
        /// Handler that send last dequeued query to server when connection will be established.
        /// </summary>
        /// <param name="sharedObject">
        /// Normaly is a PipesProvider.TransmissionLine that contain information about actual transmission.</param>
        public static void UniformQueryPostHandler(object sharedObject)
        { 
            // Drop as invalid in case of incorrect transmitted data.
            if (!(sharedObject is PipesProvider.TransmissionLine lineProcessor))
            {
                Console.WriteLine("TRANSMISSION ERROR (UQPP0): INCORRECT TRANSFERD DATA TYPE. PERMITED ONLY \"LineProcessor\"");
                return;
            }
            
            /// If queries not placed then wait.
            while (!lineProcessor.HasQueries || !lineProcessor.TryDequeQuery(out PipesProvider.QueryContainer query))
            {
                Thread.Sleep(50);
                continue;
            }

            // Open stream writer.
            StreamWriter sw = new StreamWriter(lineProcessor.pipeClient);
            try
            {
                sw.Write(lineProcessor.LastQuery);
                sw.Flush();
                Console.WriteLine("TRANSMITED: {0}", lineProcessor.LastQuery);
                //sw.Close();
            }
            // Catch the Exception that is raised if the pipe is broken or disconnected.
            catch (Exception e)
            {
                Console.WriteLine("DNS HANDLER ERROR ({1}): {0}", e.Message, lineProcessor.pipeClient.GetHashCode());

                // Retry transmission.
                if (lineProcessor.LastQuery.Attempts < 10)
                {
                    // Add to queue.
                    lineProcessor.EnqueueQuery(lineProcessor.LastQuery);

                    // Add attempt.
                    PipesProvider.QueryContainer qcl = lineProcessor.LastQuery;
                    qcl++;
                }
                else
                {
                    // If transmission attempts over the max count.
                }
            }

            lineProcessor.Processing = false;
        }


        /// <summary>
        /// Handler that will recive message from the server.
        /// </summary>
        /// <param name="sharedObject">
        /// Normaly is a PipesProvider.TransmissionLine that contain information about actual transmission.</param>
        public static async void UniformServerAnswerHandler(object sharedObject)
        {
            // Drop as invalid in case of incorrect transmitted data.
            if (!(sharedObject is PipesProvider.TransmissionLine lineProcessor))
            {
                Console.WriteLine("TRANSMISSION ERROR (UQPP0): INCORRECT TRANSFERD DATA TYPE. PERMITED ONLY \"LineProcessor\"");
                return;
            }

            lineProcessor.Processing = true;

            //Console.WriteLine("{0}/{1}: ANSWER READING STARTED.",
            //     lineProcessor.ServerName, lineProcessor.ServerPipeName);
            
            // Open stream reader.
            StreamReader sr = new StreamReader(lineProcessor.pipeClient);
            //StreamReader sr = new StreamReader(lineProcessor.pipeClient, System.Text.Encoding.UTF8, true, 128, true);
            try
            {
                string message = null;
                while (string.IsNullOrEmpty(message))
                {
                    // Avoid an error caused to disconection of client.
                    try
                    {
                        //Console.WriteLine("{0}/{1}: READ Started: {2}", lineProcessor.ServerName, lineProcessor.ServerPipeName, DateTime.Now.ToString("HH:mm:ss.fff"));
                        message = await sr.ReadToEndAsync();
                        //Console.WriteLine("{0}/{1}: READ Finished: {2}", lineProcessor.ServerName, lineProcessor.ServerPipeName, DateTime.Now.ToString("HH:mm:ss.fff"));
                    }
                    // Catch the Exception that is raised if the pipe is broken or disconnected.
                    catch (Exception e)
                    {
                        Console.WriteLine("DNS HANDLER ERROR (USAH0): {0}", e.Message);
                        lineProcessor.Processing = false;
                        return;
                    }
                }

                //Console.WriteLine("{0}/{1}: ANSWER READING FINISH.\nMESSAGE: {2}", 
                //    lineProcessor.ServerName, lineProcessor.ServerPipeName, message);
            }
            // Catch the Exception that is raised if the pipe is broken or disconnected.
            catch (Exception e)
            {
                Console.WriteLine("DNS HANDLER ERROR ({1}): {0}", e.Message, lineProcessor.pipeClient.GetHashCode());
            }

            lineProcessor.Processing = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="line">Line that was used to transmition</param>
        /// <param name="answerHandler">Delegate that will be called as handler for answer processing. 
        /// TransmissionLine contain data about actual transmission.
        /// object contain recived query (usualy string or byte[]).</param>
        /// <param name="entryQueryParts">Parts of query that was recived from client. 
        /// Method will detect core part and establish backward connection.</param>
        /// <returns></returns>
        public static bool ReciveAnswer(
            PipesProvider.TransmissionLine line,
            UniformQueries.QueryPart[] entryQueryParts, 
            System.Action<PipesProvider.TransmissionLine, object> answerHandler)
        {
            // Try to compute bacward domaint to contact with client.
            if (!UniformQueries.QueryPart.TryGetBackwardDomain(entryQueryParts, out string domain))
            {
                Console.WriteLine("Unable to buid backward domain. QUERY: {0}", 
                    UniformQueries.QueryPart.QueryPartsArrayToString(entryQueryParts));
                return false;
            }

            // Create transmission line.
            PipesProvider.TransmissionLine lineProcessor = OpenTransmissionLine(
                new SimpleClient(),
                domain,
                line.ServerName, domain,
                UniformServerAnswerHandler
                );

            // Skip line
            Console.WriteLine();
            return true;
        }
        #endregion
    }
}