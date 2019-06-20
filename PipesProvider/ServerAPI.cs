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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.IO.Pipes;

namespace PipesProvider
{
    /// <summary>
    /// Class that provide common methods for easy work with pipes' tasks.
    /// </summary>
    public static partial class API
    {
        #region Events
        /// <summary>
        /// Event that will be called when server transmission will be registred or updated.
        /// </summary>
        public static event System.Action<ServerTransmissionMeta> ServerTransmissionMeta_InProcessing;
        #endregion

        /// <summary>
        /// Hashtable thast contain references to oppened pipes.
        /// Key (string) pipe_name;
        /// Value (ServerTransmissionMeta) meta data about transmition.
        /// </summary>
        private static readonly Hashtable openedServers = new Hashtable();

        #region Client-Server loops
        /// <summary>
        /// Automaticly create server's pipe that will recive queries from clients.
        /// </summary>
        /// <param name="queryHandlerCallback">Callback that will be called when server will recive query from clinet.</param>
        /// <param name="pipeName">Name of pipe that will created. Client will access this server using that name.</param>
        public static void ClientToServerLoop(
            System.Action<ServerTransmissionMeta, string> queryHandlerCallback,
            string pipeName, 
            out string guid)
        {
            // Generate GUID.
            guid = pipeName.GetHashCode().ToString();

            // Start loop.
            ServerLoop(
                guid,
                DNSHandler_ClientToSerever_Async,
                queryHandlerCallback,
                pipeName,
                PipeDirection.InOut,
                System.IO.Pipes.NamedPipeServerStream.MaxAllowedServerInstances,
                PipeTransmissionMode.Message,
                PipeOptions.Asynchronous | PipeOptions.WriteThrough);
        }

        /// <summary>
        /// Automaticly create server's pipe.
        /// Allow to customise GUID.
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="queryHandlerCallback"></param>
        /// <param name="pipeName"></param>
        public static void ClientToServerLoop(
            string guid,
            System.Action<ServerTransmissionMeta, string> queryHandlerCallback,
            string pipeName)
        {
            ServerLoop(
                guid,
                DNSHandler_ClientToSerever_Async,
                queryHandlerCallback,
                pipeName,
                PipeDirection.InOut,
                System.IO.Pipes.NamedPipeServerStream.MaxAllowedServerInstances,
                PipeTransmissionMode.Message,
                PipeOptions.Asynchronous | PipeOptions.WriteThrough);
        }

        /// <summary>
        /// Automaticly create server's pipe.
        /// Allow to customise GUID.
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="queryHandlerCallback"></param>
        /// <param name="pipeName"></param>
        /// <param name="pipeName"></param>
        public static void ClientToServerLoop(
            string guid,
            System.Action<ServerTransmissionMeta, string> queryHandlerCallback,
            string pipeName,
            int allowedServerInstances)
        {
            ServerLoop(
                guid,
                DNSHandler_ClientToSerever_Async,
                queryHandlerCallback,
                pipeName,
                PipeDirection.InOut,
                allowedServerInstances,
                PipeTransmissionMode.Message,
                PipeOptions.Asynchronous | PipeOptions.WriteThrough);
        }

        /// <summary>
        /// Server loop 
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="queryHandlerCallback"></param>
        /// <param name="pipeName"></param>
        /// <param name="pipeDirection"></param>
        /// <param name="allowedServerInstances"></param>
        /// <param name="transmissionMode"></param>
        /// <param name="pipeOptions"></param>
        public static void ClientToServerLoop(
            string guid,
            System.Action<ServerTransmissionMeta, string> queryHandlerCallback,
            string pipeName,
            PipeDirection pipeDirection,
            int allowedServerInstances,
            PipeTransmissionMode transmissionMode,
            PipeOptions pipeOptions)
        {
            ServerLoop(
                guid,
                DNSHandler_ClientToSerever_Async,
                queryHandlerCallback,
                pipeName,
                pipeDirection,
                allowedServerInstances,
                transmissionMode,
                pipeOptions);
        }
        #endregion

        #region Server-Client loops
        /// <summary>
        /// Automaticly create server's pipe that will send message to client.
        /// </summary>
        /// <param name="queryHandlerCallback">Callback that will be called when server will recive query from clinet.</param>
        /// <param name="pipeName">Name of pipe that will created. Client will access this server using that name.</param>
        public static void ServerToClientLoop(
            string pipeName,
            out string guid)
        {
            // Generate GUID.
            guid = pipeName.GetHashCode().ToString();

            // Start loop.
            ServerLoop(
                guid,
                DNSHandler_ServerToClient_Async,
                null,
                pipeName,
                PipeDirection.InOut,
                1,
                PipeTransmissionMode.Message,
                PipeOptions.Asynchronous | PipeOptions.WriteThrough);
        }

        /// <summary>
        /// Automaticly create server's pipe that will send message to client.
        /// </summary>
        /// <param name="queryHandlerCallback">Callback that will be called when server will recive query from clinet.</param>
        /// <param name="pipeName">Name of pipe that will created. Client will access this server using that name.</param>
        public static void ServerToClientLoop(
            string guid,
            string pipeName)
        {
            // Start loop.
            ServerLoop(
                guid,
                DNSHandler_ServerToClient_Async,
                null,
                pipeName,
                PipeDirection.InOut,
                1,
                PipeTransmissionMode.Message,
                PipeOptions.Asynchronous | PipeOptions.WriteThrough);
        }
        #endregion


        #region Core configurable loop
        /// <summary>
        /// Provide base server loop that control pipe.
        /// Have ability to full controll all handlers.
        /// 
        /// Warrning: Use only if undersend how it work. Overwise use simplived ClientToServerLoop or ServerToClientLoop
        /// </summary>
        /// <param name="connectionCallback">Delegate that will be called when connection will be established.</param>
        /// <param name="pipeName">Name of pipe that will be used to acces by client.</param>
        /// <param name="pipeDirection">Dirrection of the possible transmission.</param>
        /// <param name="allowedServerInstances">How many server pipes can be started with the same name.</param>
        /// <param name="transmissionMode">Type of transmission.</param>
        /// <param name="pipeOptions">Configuration of the pipe.</param>
        public static void ServerLoop(
            string guid,
            System.Action<ServerTransmissionMeta> connectionCallback,
            System.Action<ServerTransmissionMeta, string> queryHandlerCallback,
            string pipeName,
            PipeDirection pipeDirection,
            int allowedServerInstances,
            PipeTransmissionMode transmissionMode,
            PipeOptions pipeOptions)
        {
            // Try to oppen pipe server.
            NamedPipeServerStream pipeServer = null;
            try
            {
                pipeServer =
                    new NamedPipeServerStream(pipeName, pipeDirection, allowedServerInstances,
                        transmissionMode, pipeOptions, 0, 0);
            }
            catch (Exception ex)
            {
                Console.WriteLine("SERVER LOOP NOT STARTED:\n{0}\n", ex.Message);
                return;
            }

            //Console.WriteLine("{0}: Pipe created", pipeName);

            #region Meta data
            // Meta data about curent transmition.
            ServerTransmissionMeta meta = ServerTransmissionMeta.None;
            IAsyncResult connectionMarker = null;

            // Registration or update meta data of oppened transmission.
            if (openedServers.ContainsKey(guid))
            {
                // Load previous meta.
                meta = (ServerTransmissionMeta)openedServers[guid];
            }
            else
            {
                // Create new meta.
                meta = new ServerTransmissionMeta(null, connectionCallback, queryHandlerCallback, pipeServer, pipeName);
                openedServers.Add(guid, meta);
            }

            try
            {
                // Inform subscribers about new pass with this transmission to give possibility correct data.
                ServerTransmissionMeta_InProcessing?.Invoke(meta);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Server Loop. Transmission event fail. (SLTEF0): {0}", ex.Message);
            }
            #endregion

            #region Main loop
            while (!meta.Expired)
            {
                // Wait for a client to connect
                if ((connectionMarker == null || connectionMarker.IsCompleted) &&
                    !pipeServer.IsConnected)
                {
                    try
                    {
                        // Start async waiting of connection.
                        connectionMarker = pipeServer.BeginWaitForConnection(
                            ConnectionEstablishedCallbackRetranslator, meta);
                        /// Update data.
                        meta.connectionMarker = connectionMarker;

                        Console.Write("{0}: Waiting for client connection...\n", pipeName);
                    }
                    catch //(Exception ex)
                    {
                        //Console.WriteLine("SERVER LOOP ERROR. SERVER RESTORED: {0}", ex.Message);

                        // Close actual pipe.
                        pipeServer.Close();

                        // Establish new server.
                        pipeServer = new NamedPipeServerStream(pipeName, pipeDirection, allowedServerInstances,
                        transmissionMode, pipeOptions, 0, 0);

                        // Update meta data.
                        meta.pipe = pipeServer;
                    }
                    //Console.WriteLine("TRANSMITION META HASH: {0}", meta.GetHashCode());
                }

                // Turn order to other threads.
                Thread.Sleep(150);
            }
            #endregion

            // Finalize server.
            if (!meta.Stoped)
            {
                StopServer(meta);
            }

            // Discharge existing in hashtable.
            openedServers.Remove(meta.name);

            // Finish stream.
            pipeServer.Close();

            Console.WriteLine("{0}: PIPE SERVER CLOSED", meta.name);
        }
        #endregion


        #region Controls
        /// <summary>
        /// Callback that will react on connection esstablishing.
        /// Will close waiting async operation and call shared delegate with server loop's code.
        /// </summary>
        /// <param name="result"></param>
        private static async void ConnectionEstablishedCallbackRetranslator(IAsyncResult result)
        {
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-us");
            // Load transmission meta data.
            ServerTransmissionMeta meta = (ServerTransmissionMeta)result.AsyncState;
            
            // Stop connection waiting.
            try
            {
                // Close connection if not conplited.
                //if (!meta.connectionMarker.IsCompleted)
                {
                    meta.pipe.EndWaitForConnection(meta.connectionMarker);
                }
            }
            catch (Exception ex)
            {
                // Log if error caused not just by closed pipe.
                if (!(ex is ObjectDisposedException))
                {
                    Console.WriteLine("CONNECTION ERROR (CECR EWFC): {0} ", ex.Message);
                }
                // Connection failed. Drop.
                return;
            }

            try
            {
                if(!meta.pipe.IsConnected)
                    await meta.pipe.WaitForConnectionAsync();
            }
            catch(Exception ex)
            {
                Console.WriteLine("CONNECTION ERROR (CECR EWFC 2): {0} {1}", meta.name, ex.Message);
            }

            //Console.WriteLine("\nAsync compleated:{0} {1}\nPipe connected:{2}\n", result.IsCompleted, result.CompletedSynchronously, meta.pipe.IsConnected);

            // Log about success.
            if (meta.pipe.IsConnected)
            {
                Console.WriteLine("\n{0}: Client connected.", meta.name);
            }
            else
            {
                Console.WriteLine("\n{0}: Connection waiting was terminated", meta.name);
            }

            // Call handler.
            //Console.WriteLine("Connected: {0}\tCallback valid: {1}", meta.pipe.IsConnected, meta.connectionCallback != null);
            if (meta.pipe.IsConnected)  meta.connectionCallback?.Invoke(meta);
        }

        /// <summary>
        /// Marking pipe as expired. 
        /// On the next loop tick connections will be disconnect and pipe will close.
        /// </summary>
        /// <param name="pipeName"></param>
        public static void SetExpired(string pipeName)
        {
            // Check meta data existing.
            if (openedServers.ContainsKey(pipeName))
            {
                // Load meta data.
                ServerTransmissionMeta meta = (ServerTransmissionMeta)openedServers[pipeName];

                // Mark it as expired.
                meta.SetExpired();
            }
        }
        
        /// <summary>
        /// Marking pipe as expired. 
        /// On the next loop tick connections will be disconnect and pipe will close.
        /// </summary>
        /// <param name="pipeName"></param>
        public static void SetExpired(ServerTransmissionMeta meta)
        {
            // Mark it as expired.
            meta.SetExpired();
        }

        /// <summary>
        /// Markin all pipes as expired. 
        /// Connection will be terminated.
        /// </summary>
        public static void SetExpiredAll()
        {
            foreach(ServerTransmissionMeta meta in openedServers.Values)
            {
                meta.SetExpired();
            }
        }

        /// <summary>
        /// Stop server by pipe name.
        /// </summary>
        /// <param name="pipeName"></param>
        public static void StopServer(string pipeName)
        {
            // Check meta data existing.
            if (openedServers.ContainsKey(pipeName))
            {
                // Load meta data.
                ServerTransmissionMeta meta = (ServerTransmissionMeta)openedServers[pipeName];

                // Stop server relative to meta data.
                StopServer(meta);

                // Discharge existing in hashtable.
                openedServers.Remove(meta.name);
            }
        }

        /// <summary>
        /// Stop server by relative meta data.
        /// </summary>
        /// <param name="meta"></param>
        public static void StopServer(ServerTransmissionMeta meta)
        {
            // If transmission has been opening.
            if (meta != null)
            {
                // Disconnect and close pipe.
                try
                {
                    // Disconnects clients.
                    if (meta.pipe.IsConnected)
                    {
                        meta.pipe.Disconnect();
                    }

                    // Closing pipe.
                    meta.pipe.Close();
                }
                catch (Exception ex)
                {
                    /// Log error.
                    Console.WriteLine("SERVER STOP FAILED: {0}", ex.Message);
                }

                Console.WriteLine("PIPE CLOSED: {0}", meta.name);
                return;
            }

            Console.WriteLine("META NOT FOUND");
        }

        /// <summary>
        /// Stoping all regirated servers.
        /// </summary>
        public static void StopAllServers()
        {
            // Log statistic.
            Console.WriteLine("TRANSMISSIONS TO CLOSE: {0}", openedServers.Count);

            // Stop every registred server.
            foreach (ServerTransmissionMeta meta in openedServers.Values)
            {
                // Log about target to close.
                //Console.WriteLine("STOPING SERVER: {0}", meta.name);

                // Mark as stoped.
                meta.SetStoped();

                // Stop server described by meta.
                StopServer(meta);
            }

            // Clear hashtable with terminated servers.
            openedServers.Clear();

            Console.WriteLine();
        }
        #endregion

        /// <summary>
        /// Try to find opened servert to client transmisssion meta data in common table.
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="meta"></param>
        /// <returns></returns>
        public static bool TryGetServerTransmissionMeta(string guid, out ServerTransmissionMeta meta)
        {
            meta = openedServers[guid] as ServerTransmissionMeta;
            return meta != null;
        }


        #region Handlers
        /// <summary>
        /// Code that will work on server loop when connection will be established.
        /// Recoomended to using as default DNS Handler for queries reciving.
        /// </summary>
        public static async void DNSHandler_ClientToSerever_Async(PipesProvider.ServerTransmissionMeta meta)
        {
            // Open stream reader.
            StreamReader sr = new StreamReader(meta.pipe);
            string queryBufer;
            DateTime sessionTime = DateTime.Now.AddSeconds(5000);

            // Read until trasmition exits not finished.
            while (meta.pipe.IsConnected)
            {
                queryBufer = null;
                // Read line from stream.
                while (queryBufer == null)
                {
                    // Avoid an error caused to disconection of client.
                    try
                    {
                        queryBufer = await sr.ReadLineAsync();
                    }
                    // Catch the Exception that is raised if the pipe is broken or disconnected.
                    catch (Exception e)
                    {
                        Console.WriteLine("DNS HANDLER ERROR: {0}", e.Message);
                        return;
                    }

                    if (DateTime.Compare(sessionTime, DateTime.Now) < 0)
                    {
                        Console.WriteLine("Connection terminated cause allowed time has expired.");
                        /// Avoid disconnectin error.
                        try
                        {
                            meta.pipe.Disconnect();
                        }
                        catch { throw; }

                        return;
                    }
                }

                // Disconnect user if query recived.
                if (meta.pipe.IsConnected)
                {
                    meta.pipe.Disconnect();
                }

                // Remove temporal data.
                meta.pipe.Dispose();

                // Drop if stream is over.
                if (queryBufer == null)
                {
                    //Console.WriteLine("NULL REQUEST AVOIDED. CONNECTION TERMINATED.");
                    break;
                }

                // Log query.
                Console.WriteLine("RECIVED QUERY: {0}", queryBufer);

                // Redirect handler.
                meta.queryHandlerCallback?.Invoke(meta, queryBufer);
            }

            // Log about transmission finish.
            Console.WriteLine("TRANSMISSION FINISHED AT {0}", DateTime.Now.ToString("HH:mm:ss.fff"));
        }

        /// <summary>
        /// Code that will work on server loop when connection will be established.
        /// Recoomended to using as default DNS Handler for message sending.
        /// </summary>
        public static async void DNSHandler_ServerToClient_Async(PipesProvider.ServerTransmissionMeta meta)
        {
            // Open stream reader.
            StreamWriter sw = new StreamWriter(meta.pipe);
            //StreamWriter sw = new StreamWriter(meta.pipe, Encoding.UTF8, 128, true);

            // Buferise query before calling of async operations.
            string sharedQuery = meta.ProcessingQuery;

            // Read until trasmition exits not finished.
            // Avoid an error caused to disconection of client.
            try
            {
                // Write message to stream.
                await sw.WriteAsync(sharedQuery);
                sw.Flush();
            }
            // Catch the Exception that is raised if the pipe is broken or disconnected.
            catch (Exception e)
            {
                Console.WriteLine("DNS HANDLER ERROR (StC0): {0}", e.Message);
                return;
            }

            // Disconnect user if query recived.
            if (meta.pipe.IsConnected)
            {
                meta.pipe.Disconnect();
            }

            // Remove temporal data.
            meta.pipe.Dispose();

            // Stop this transmission line.
            meta.SetStoped();

            // Log about transmission finish.
            Console.WriteLine("TRANSMISSION FINISHED AT {0}", DateTime.Now.ToString("HH:mm:ss.fff"));
        }
        #endregion
    }
}