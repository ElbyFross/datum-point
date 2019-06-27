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
using Microsoft.Win32.SafeHandles;
using PipesProvider.Networking;
using PipesProvider.Client;

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
        public static bool AppTerminated { get; set; }

        /// <summary>
        /// Table that contain delegatds subscribed to beckward lines in duplex queries.
        /// 
        /// Key string - backward domain
        /// Value System.Action<TransmissionLine, object> - answer processing delegat.
        /// </summary>
        protected static Hashtable DuplexBackwardCallbacks = new Hashtable();
        #endregion

        #region Public fields
        /// <summary>
        /// Reference to thread that host this server.
        /// </summary>
        public Thread thread;

        /// <summary>
        /// Token that will used to autorizing on the server.
        /// By default setted up to anonymous.
        /// </summary>
        public SafeAccessTokenHandle AccessToken
        { get; set; } = System.Security.Principal.WindowsIdentity.GetAnonymous().AccessToken;
        #endregion


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
        /// Oppening transmition line that will able to send querie to described server's pipe.
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="pipeName"></param>
        /// <returns></returns>
        public static TransmissionLine OpenOutTransmissionLine(
           string serverName,
           string pipeName)
        {
            return OpenTransmissionLine(serverName, pipeName, UniformQueryPostHandler);
        }

        /// <summary>
        /// Automaticly create Transmission line or lokking for previos one.
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="pipeName"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static TransmissionLine OpenTransmissionLine(
           string serverName,
           string pipeName,
           System.Action<TransmissionLine> callback)
        {
            string guid = serverName.GetHashCode() + "_" + pipeName.GetHashCode();
            return OpenTransmissionLine(new SimpleClient(), serverName, pipeName, callback);
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
        public static TransmissionLine OpenTransmissionLine(
            BaseClient client,
            string serverName,
            string pipeName,
            System.Action<TransmissionLine> callback)
        {
            // Validate client.
            if (client == null)
            {
                Console.WriteLine("CLIENT is NULL (BC_OTL_0). Unable to open new transmission line.");
                return null;
            }

            // Get target GUID.
            string guid = TransmissionLine.GenerateGUID(serverName, pipeName);

            // Try to load  trans line by GUID.
            if (ClientAPI.TryGetTransmissionLineByGUID(guid, out TransmissionLine trnsLine))
            {
                // If not obsolterd transmission line then drop operation.
                if (!trnsLine.Closed)
                {
                    //Console.WriteLine("OTL {0} | FOUND", guid);
                    return trnsLine;
                }
                else
                {
                    // Unregister line and recall method.
                    ClientAPI.TryToUnregisterTransmissionLine(guid);

                    //Console.WriteLine("OTL {0} | RETRY", guid);

                    // Retry.
                    return OpenTransmissionLine(client, serverName, pipeName, callback); 
                }
            }
            // If full new pipe.
            else
            {
                // Create new if not registred.
                trnsLine = new TransmissionLine(
                    serverName,
                    pipeName,
                    callback,
                    client.AccessToken);

                // Put line proccesor to the new client loop.
                client.StartClientThread(
                    guid,
                    trnsLine,
                    TransmissionLine.ThreadLoop);


                //Console.WriteLine("OTL {0} | CREATED", guid);
            }

            // Return oppened line.
            return trnsLine;
        }


        /// <summary>
        /// Handler that send last dequeued query to server when connection will be established.
        /// </summary>
        /// <param name="sharedObject">
        /// Normaly is a TransmissionLine that contain information about actual transmission.</param>
        public async static void UniformQueryPostHandler(object sharedObject)
        { 
            // Drop as invalid in case of incorrect transmitted data.
            if (!(sharedObject is TransmissionLine lineProcessor))
            {
                Console.WriteLine("TRANSMISSION ERROR (UQPP0): INCORRECT TRANSFERD DATA TYPE. PERMITED ONLY \"LineProcessor\"");
                return;
            }
            /// If queries not placed then wait.
            while (!lineProcessor.HasQueries || !lineProcessor.TryDequeQuery(out _))
            {
                Thread.Sleep(50);
                continue;
            }

            // Open stream writer.
            StreamWriter sw = new StreamWriter(lineProcessor.pipeClient);
            try
            {
                await sw.WriteAsync(lineProcessor.LastQuery.Query);
                await sw.FlushAsync();
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
                    lineProcessor++;
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
        /// Normaly is a TransmissionLine that contain information about actual transmission.</param>
        public static async void UniformServerAnswerHandler(object sharedObject)
        {
            // Drop as invalid in case of incorrect transmitted data.
            if (!(sharedObject is TransmissionLine lineProcessor))
            {
                Console.WriteLine("TRANSMISSION ERROR (UQPP0): INCORRECT TRANSFERD DATA TYPE. PERMITED ONLY \"LineProcessor\"");
                return;
            }

            // Mark line as busy to avoid calling of next query, cause this handler is async.
            lineProcessor.Processing = true;

            // Open stream reader.
            StreamReader sr = new StreamReader(lineProcessor.pipeClient);
            try
            {
                #region Reciving message
                Console.WriteLine("{0}/{1}: WAITING FOR MESSAGE",
                        lineProcessor.ServerName, lineProcessor.ServerPipeName);

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
                        // Log error.
                        Console.WriteLine("DNS HANDLER ERROR (USAH0): {0}", e.Message);

                        // Stop processing merker to pass async block.
                        lineProcessor.Processing = false;

                        // Close processor case this line already deprecated on the server side as single time task.
                        lineProcessor.Close();
                        return;
                    }
                }

                Console.WriteLine("{0}/{1}: MESSAGE RECIVED",
                        lineProcessor.ServerName, lineProcessor.ServerPipeName);
                #endregion

                #region Processing message
                // Try to call answer handler.
                string tableGUID = lineProcessor.ServerName + "\\" + lineProcessor.ServerPipeName;
                // Look for delegate in table.
                if (DuplexBackwardCallbacks[tableGUID] is
                    System.Action<TransmissionLine, object> registredCallback)
                {
                    if (registredCallback != null)
                    {
                        // Invoke delegate if found and has dubscribers.
                        registredCallback.Invoke(lineProcessor, message);
                    }
                    else
                    {
                        Console.WriteLine("{0}/{1}: ANSWER CALLBACK HAS NO SUBSCRIBERS",
                            lineProcessor.ServerName, lineProcessor.ServerPipeName);
                    }
                }
                else
                {
                    Console.WriteLine("{0}/{1}: ANSWER HANDLER NOT FOUND BY {2}",
                        lineProcessor.ServerName, lineProcessor.ServerPipeName, tableGUID);
                }

                //Console.WriteLine("{0}/{1}: ANSWER READING FINISH.\nMESSAGE: {2}",
                //    lineProcessor.ServerName, lineProcessor.ServerPipeName, message);
                #endregion
            }
            // Catch the Exception that is raised if the pipe is broken or disconnected.
            catch (Exception e)
            {
                Console.WriteLine("DNS HANDLER ERROR ({1}): {0}", e.Message, lineProcessor.pipeClient.GetHashCode());
            }

            // Stop processing merker to pass async block.
            lineProcessor.Processing = false;

            // Close processor case this line already deprecated on the server side as single time task.
            lineProcessor.Close();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="line">Line that was used to transmition</param>
        /// <param name="answerHandler">Delegate that will be called as handler for answer processing. 
        /// TransmissionLine contain data about actual transmission.
        /// object contain recived query (usualy string or byte[]).</param>
        /// <param name="decodedQuery">Query that sent to server and must recive answer. Must be not encoded.</param>
        /// <returns></returns>
        public static bool ReciveAnswer(
            TransmissionLine line,
            string decodedQuery,
            System.Action<TransmissionLine, object> answerHandler)
        {
            return ReciveAnswer(
                line, 
                UniformQueries.API.DetectQueryParts(decodedQuery),
                answerHandler);
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
            TransmissionLine line,
            UniformQueries.QueryPart[] entryQueryParts, 
            System.Action<TransmissionLine, object> answerHandler)
        {
            #region Create backward domain
            // Try to compute bacward domaint to contact with client.
            if (!UniformQueries.QueryPart.TryGetBackwardDomain(entryQueryParts, out string domain))
            {
                Console.WriteLine("ERROR (BCRA0): Unable to buid backward domain. QUERY: {0}", 
                    UniformQueries.QueryPart.QueryPartsArrayToString(entryQueryParts));
                return false;
            }
            #endregion

            #region Addind answer handler to backward table.
            // Try to load registred callback to overriding.
            if (DuplexBackwardCallbacks[domain] is
                System.Action<TransmissionLine, object> registredCallback)
            {
                DuplexBackwardCallbacks[domain] = answerHandler;
            }
            else
            {
                // Add colback to table as new.
                DuplexBackwardCallbacks.Add(line.ServerName + "\\" + domain, answerHandler);
            }
            #endregion

            #region Opening transmition line
            // Create transmission line.
            TransmissionLine lineProcessor = OpenTransmissionLine(
                new SimpleClient(),
                line.ServerName, domain,
                UniformServerAnswerHandler
                );
            #endregion

            // Skip line
            Console.WriteLine();
            return true;
        }


        /// <summary>
        /// Add query to queue. 
        /// Open backward line that will call answer handler.
        /// </summary>
        /// <param name="line">Line proccessor that control queries posting to target server.</param>
        /// <param name="query">Query that will sent to server.</param>
        /// <param name="answerHandler">Callback that will recive answer.</param>
        public static void EnqueueDuplexQuery(
            TransmissionLine line,
            string query,
            System.Action<TransmissionLine, object> answerHandler)
        {
            // Add our query to line processor queue.
            line.EnqueueQuery(query);

            // Open backward chanel to recive answer from server.
            ReciveAnswer(line, query, answerHandler);
        }

        /// <summary>
        /// Add query to queue. 
        /// Open backward line that will call answer handler.
        /// </summary>
        /// <param name="serverName">Name of the server. "." if local.</param>
        /// <param name="serverPipeName">Name of pipe provided by server.</param>
        /// <param name="query">Query that will sent to server.</param>
        /// <param name="answerHandler">Callback that will recive answer.</param>
        public static void EnqueueDuplexQuery(
            string serverName,
            string serverPipeName,
            string query,
            System.Action<TransmissionLine, object> answerHandler)
        {
            // Open transmission line.
            TransmissionLine line = OpenOutTransmissionLine(serverName, serverPipeName);

            // Equeue query to line.
            EnqueueDuplexQuery(line, query, answerHandler);
        }
        #endregion
    }
}