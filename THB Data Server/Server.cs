// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// Copyright (c) 2019 Volodymyr Podshyvalov

using System;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Threading;
using System.IO;
using System.Diagnostics;

namespace THB_Data_Server
{
    /// <summary>
    /// Server that provide and manage data by queries.
    /// </summary>
    class Server : UniformServer.BaseServer
    {
        
        /// <summary>
        /// Main loop.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args) 
        {
            #region Detect processes conflicts
            // Get GUID of this assebly.
            string guid = Marshal.GetTypeLibGuidForAssembly(Assembly.GetExecutingAssembly()).ToString();

            // Create Mutex for this app instance.
            mutexObj = new Mutex(true, guid, out bool newApp);

            // Check does this instance a new single app, or same app already runned.
            if(!newApp)
            {
                // Log error.
                Console.WriteLine("\"THB Data Server\" already started. Application not allow multiple instances at single moment.\nGUID: " + guid);
                // Wait a time until exit.
                Thread.Sleep(2000);
                return;
            }
            #endregion

            #region Set default data \ load DLLs \ appling arguments
            // Set default thread count. Can be changed via args or command.
            threadsCount = Environment.ProcessorCount;
            longTermServerThreads = new Server[threadsCount];

            // React on uniform arguments.
            ArgsReactor(args);

            // Check direcroties
            LoadAssemblies(AppDomain.CurrentDomain.BaseDirectory + "libs\\");
            #endregion

            // Log about succeed run of the server.
            Console.WriteLine("Data server started.\nGUID: " + guid);

            #region Loaded query handler processors.
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
                serverBufer.pipeName = "THB_DS_QM_MAIN_INOUT";

                // Starting server loop.
                serverBufer.StartServerThread(
                    "Queries monitor #" + i, serverBufer, 
                    ThreadingServerLoop_OpenChanel);

                // Change thread culture.
                serverBufer.thread.CurrentUICulture = new System.Globalization.CultureInfo("en-us");

                // Skip line
                Console.WriteLine();
            }

            /// Draw line
            ConsoleDraw.Primitives.DrawLine();
            Console.WriteLine();
            #endregion

            /// Show help.
            Commands.CommandResponseProcessor("help");

            #region Main loop
            // Main loop that will provide server services until application close.
            while (!appTerminated)
            {
                // Check input
                if(Console.KeyAvailable)
                {
                    // Log responce.
                    Console.Write("\nEnter command: ");
                    // Read command.
                    string command = Console.ReadLine();
                    // Processing of entered command.
                    bool validCommand = Commands.CommandResponseProcessor(command);
                    if (validCommand)
                    {
                        //Console.WriteLine();
                    }
                }
                Thread.Sleep(threadSleepTime);
            }
            #endregion

            #region Finalize
            Console.WriteLine();

            // Stop started servers.
            PipesProvider.API.StopAllServers();

            // Aborting threads.
            foreach (Server st in longTermServerThreads)
            {
                st.thread.Abort();
                Console.WriteLine("THREAD ABORTED: {0}", st.thread.Name);
            }
            Console.WriteLine();

            // Whait until close.
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            #endregion
        }
    }
}