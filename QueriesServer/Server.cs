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
using PipesProvider.Security;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Threading;

namespace QueriesServer
{
    /// <summary>
    /// Public servers that allow anonimous connection.
    /// Reciving clients' queries and redirect in to target infrastructure servers by the comutation table.
    /// Reciving answer from servers and redirect it to target clients.
    /// </summary>
    class Server : UniformServer.BaseServer
    {
        static void Main(string[] args)
        {
            #region Detect processes conflicts
            // Get GUID of this assebly.
            string guid = Marshal.GetTypeLibGuidForAssembly(Assembly.GetExecutingAssembly()).ToString();

            // Create Mutex for this app instance.
            mutexObj = new Mutex(true, guid, out bool newApp);

            // Check does this instance a new single app, or same app already runned.
            if (!newApp)
            {
                // Log error.
                Console.WriteLine("\"THB Data Server\" already started. Application not allow multiple instances at single moment.\nGUID: " + guid);
                // Wait a time until exit.
                Thread.Sleep(2000);
                return;
            }
            #endregion

            // Request anonymous configuration for system.
            General.SetLocalSecurityAuthority(SecurityLevel.Anonymous);

            #region Set default data \ load DLLs \ appling arguments
            // Set default thread count. Can be changed via args or command.
            threadsCount = Environment.ProcessorCount;
            longTermServerThreads = new Server[threadsCount];

            // React on uniform arguments.
            ArgsReactor(args);

            // Check direcroties
            LoadAssemblies(AppDomain.CurrentDomain.BaseDirectory + "libs\\");
            #endregion


            #region Load routing tables
            // Load routing tables
            PipesProvider.Networking.RoutingTable routingTable = null;
            // From system folders.
            routingTable += PipesProvider.Networking.RoutingTable.LoadRoutingTables(AppDomain.CurrentDomain.BaseDirectory + "resources\\routing\\");
            // From plugins.
            routingTable += PipesProvider.Networking.RoutingTable.LoadRoutingTables(AppDomain.CurrentDomain.BaseDirectory + "plugins\\");

            // If routing table not found.
            if(routingTable.intructions.Count == 0)
            {
                // Log error.
                Console.WriteLine("ROUTING TABLE NOT FOUND: Create default table by directory \\resources\\routing\\ROUTING.xml");

                // Set default intruction.
                routingTable.intructions.Add(new PipesProvider.Networking.RoutingTable.RoutingInstruction()
                {
                    logonConfig = PipesProvider.Security.LogonConfig.Anonymous,
                    queryPatterns = new string[] { "$q&$guid&$token" },
                    routingIP = "localhost"
                });

                // Save sample routing table to application files.
                PipesProvider.Networking.RoutingTable.SaveRoutingTable(routingTable, AppDomain.CurrentDomain.BaseDirectory + "resources\\routing\\", "ROUTING");
            }
            else
            {
                // Log error.
                Console.WriteLine("ROUTING TABLE: Detected {0} instructions.", routingTable.intructions.Count);
            }
            #endregion

            #region Loaded query handler processors
            /// Draw line
            ConsoleDraw.Primitives.DrawSpacedLine();
            // Initialize Queue monitor.
            try
            {
                var _qp_loader = UniformQueries.API.QueryProcessors;
            }
            catch (Exception ex)
            {
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                Console.WriteLine("QUERY HANDLER PROCESSORS LOADINT TERMINATED:\n{0}", ex.Message);
            }
            ConsoleDraw.Primitives.DrawSpacedLine();
            Console.WriteLine();
            #endregion

            #region Start queries monitor threads
            for (int i = 0; i < threadsCount; i++)
            {
                // Instiniate server.
                Server serverBufer = new Server();
                longTermServerThreads[i] = serverBufer;

                // Set fields.
                serverBufer.pipeName = "THB_QUERY_SEREVER";

                // Starting server loop.
                serverBufer.StartServerThread(
                    "Queries chanel #" + i, serverBufer,
                    ThreadingServerLoop_OpenChanel);

                // Change thread culture.
                serverBufer.thread.CurrentUICulture = new System.Globalization.CultureInfo("en-us");

                // Skip line
                Console.WriteLine();
            }
            #endregion

            
            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }
    }
}
