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
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.IO.Pipes;
using UniformQueries;
using UQAPI = UniformQueries.API;

namespace PipesProvider.Server
{
    /// <summary>
    /// Class that provide common methods for easy work with pipes' tasks.
    /// </summary>
    public static class ServerAPI
    {
        #region Events
        /// <summary>
        /// Event that will be called when server transmission will be registred or updated.
        /// </summary>
        public static event System.Action<ServerTransmissionController> ServerTransmissionMeta_InProcessing;
        #endregion

        #region Fields
        /// <summary>
        /// Hashtable thast contain references to oppened pipes.
        /// Key (string) pipe_name;
        /// Value (ServerTransmissionMeta) meta data about transmition.
        /// </summary>
        private static readonly Hashtable openedServers = new Hashtable();
        #endregion


        #region Client-Server loops
        /// <summary>
        /// Automaticly create server's pipe that will recive queries from clients.
        /// </summary>
        /// <param name="queryHandlerCallback">Callback that will be called when server will recive query from clinet.</param>
        /// <param name="pipeName">Name of pipe that will created. Client will access this server using that name.</param>
        public static void ClientToServerLoop(
            System.Action<ServerTransmissionController, string> queryHandlerCallback,
            string pipeName, 
            out string guid,
            Security.SecurityLevel securityLevel)
        {
            // Generate GUID.
            guid = pipeName.GetHashCode().ToString();

            // Start loop.
            ServerLoop(
                guid,
                Handlers.DNS.ClientToSereverAsync,
                queryHandlerCallback,
                pipeName,
                PipeDirection.InOut,
                System.IO.Pipes.NamedPipeServerStream.MaxAllowedServerInstances,
                PipeTransmissionMode.Message,
                PipeOptions.Asynchronous | PipeOptions.WriteThrough,
                securityLevel);
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
            System.Action<ServerTransmissionController, string> queryHandlerCallback,
            string pipeName,
            Security.SecurityLevel securityLevel)
        {
            ServerLoop(
                guid,
                Handlers.DNS.ClientToSereverAsync,
                queryHandlerCallback,
                pipeName,
                PipeDirection.InOut,
                System.IO.Pipes.NamedPipeServerStream.MaxAllowedServerInstances,
                PipeTransmissionMode.Message,
                PipeOptions.Asynchronous | PipeOptions.WriteThrough,
                securityLevel);
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
            System.Action<ServerTransmissionController, string> queryHandlerCallback,
            string pipeName,
            int allowedServerInstances,
            Security.SecurityLevel securityLevel)
        {
            ServerLoop(
                guid,
                Handlers.DNS.ClientToSereverAsync,
                queryHandlerCallback,
                pipeName,
                PipeDirection.InOut,
                allowedServerInstances,
                PipeTransmissionMode.Message,
                PipeOptions.Asynchronous | PipeOptions.WriteThrough,
                securityLevel);
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
            System.Action<ServerTransmissionController, string> queryHandlerCallback,
            string pipeName,
            PipeDirection pipeDirection,
            int allowedServerInstances,
            PipeTransmissionMode transmissionMode,
            PipeOptions pipeOptions,
            Security.SecurityLevel securityLevel)
        {
            ServerLoop(
                guid,
                Handlers.DNS.ClientToSereverAsync,
                queryHandlerCallback,
                pipeName,
                pipeDirection,
                allowedServerInstances,
                transmissionMode,
                pipeOptions,
                securityLevel);
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
            out string guid,
            Security.SecurityLevel securityLevel)
        {
            // Generate GUID.
            guid = pipeName.GetHashCode().ToString();

            // Start loop.
            ServerLoop(
                guid,
                Handlers.DNS.ServerToClientAsync,
                null,
                pipeName,
                PipeDirection.InOut,
                1,
                PipeTransmissionMode.Message,
                PipeOptions.Asynchronous | PipeOptions.WriteThrough,
                securityLevel);
        }

        /// <summary>
        /// Automaticly create server's pipe that will send message to client.
        /// </summary>
        /// <param name="queryHandlerCallback">Callback that will be called when server will recive query from clinet.</param>
        /// <param name="pipeName">Name of pipe that will created. Client will access this server using that name.</param>
        public static void ServerToClientLoop(
            string guid,
            string pipeName,
            Security.SecurityLevel securityLevel)
        {
            // Start loop.
            ServerLoop(
                guid,
                Handlers.DNS.ServerToClientAsync,
                null,
                pipeName,
                PipeDirection.InOut,
                1,
                PipeTransmissionMode.Message,
                PipeOptions.Asynchronous | PipeOptions.WriteThrough,
                securityLevel);
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
            System.Action<ServerTransmissionController> connectionCallback,
            System.Action<ServerTransmissionController, string> queryHandlerCallback,
            string pipeName,
            PipeDirection pipeDirection,
            int allowedServerInstances,
            PipeTransmissionMode transmissionMode,
            PipeOptions pipeOptions,
            Security.SecurityLevel securityLevel)
        {
            // Create PipeSecurity relative to requesteed level.
            PipeSecurity pipeSecurity = Security.General.GetRulesForLevels(securityLevel);

            // Try to open pipe server.
            NamedPipeServerStream pipeServer = null;
            try
            {
                pipeServer =
                    new NamedPipeServerStream(pipeName, pipeDirection, allowedServerInstances,
                        transmissionMode, pipeOptions, 0, 0, pipeSecurity);
            }
            catch (Exception ex)
            {
                Console.WriteLine("SERVER LOOP NOT STARTED:\n{0}\n", ex.Message);
                return;
            }

            //Console.WriteLine("{0}: Pipe created", pipeName);

            #region Meta data
            // Meta data about curent transmition.
            ServerTransmissionController meta = ServerTransmissionController.None;
            IAsyncResult connectionMarker = null;

            // Registration or update meta data of oppened transmission.
            if (openedServers.ContainsKey(guid))
            {
                // Load previous meta.
                meta = (ServerTransmissionController)openedServers[guid];
            }
            else
            {
                // Create new meta.
                meta = new ServerTransmissionController(null, connectionCallback, queryHandlerCallback, pipeServer, pipeName);
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
                            Handlers.Service.ConnectionEstablishedCallbackRetranslator, meta);
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
                        transmissionMode, pipeOptions, 0, 0, pipeSecurity);

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
            openedServers.Remove(guid);

            // Finish stream.
            pipeServer.Close();

            Console.WriteLine("{0}: PIPE SERVER CLOSED", meta.name);
        }
        #endregion


        #region Controls
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
                ServerTransmissionController meta = (ServerTransmissionController)openedServers[pipeName];

                // Mark it as expired.
                meta.SetExpired();
            }
        }
        
        /// <summary>
        /// Marking pipe as expired. 
        /// On the next loop tick connections will be disconnect and pipe will close.
        /// </summary>
        /// <param name="pipeName"></param>
        public static void SetExpired(ServerTransmissionController meta)
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
            foreach(ServerTransmissionController meta in openedServers.Values)
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
                ServerTransmissionController meta = (ServerTransmissionController)openedServers[pipeName];

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
        public static void StopServer(ServerTransmissionController meta)
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
            foreach (ServerTransmissionController meta in openedServers.Values)
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
        public static bool TryGetServerTransmissionMeta(string guid, out ServerTransmissionController meta)
        {
            meta = openedServers[guid] as ServerTransmissionController;
            return meta != null;
        }
    }
}