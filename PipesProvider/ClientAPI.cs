﻿// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// Copyright (c) 2019 Volodymyr Podshyvalov

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
        /// <summary>
        /// Hashtable thast contain references to oppened pipes.
        /// Key (string) server_name.pipe_name;
        /// Value (LineProcessor) meta data about transmition.
        /// </summary>
        private static readonly Hashtable openedClients = new Hashtable();
        
        /// <summary>
        /// Start saftely async waiting connection operation.
        /// </summary>
        /// <param name="pipeClient"></param>
        /// <param name="lineProcessor"></param>
        async static void ConnectToServerAsync(
            NamedPipeClientStream pipeClient,
            TransmissionLine lineProcessor)
        {
            try
            {
                // Wait until connection.
                await pipeClient.ConnectAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0}/{1}: Connection not possible. {2}", lineProcessor.ServerPipeName, lineProcessor.ServerPipeName, ex.Message);
            }
        }


        #region Public methods
        /// <summary>
        /// Provide stable client loop controlled by data of LineProcessor.
        /// </summary>
        /// <param name="lineProcessor"></param>
        /// <param name="pipeDirection"></param>
        /// <param name="pipeOptions"></param>
        public static void ClientLoop(
            TransmissionLine lineProcessor,
            PipeDirection pipeDirection,
            PipeOptions pipeOptions
            )
        {
            // Loop will work until this proceesor line not closed.
            while (!lineProcessor.Closed)
            {
                /// If queries not placed then wait.
                if (!lineProcessor.HasQueries || !lineProcessor.TryDequeQuery(out string query))
                {
                    Thread.Sleep(50);
                    continue;
                }

                // Open pipe.
                using (NamedPipeClientStream pipeClient =
                    new NamedPipeClientStream(lineProcessor.ServerName, lineProcessor.ServerPipeName, pipeDirection, pipeOptions))
                {
                    // Update meta data.
                    lineProcessor.pipeClient = pipeClient;

                    // Log.
                    Console.WriteLine("{0}/{1}: Connection to server.", lineProcessor.ServerName, lineProcessor.ServerPipeName);

                    // Request connection.
                    ConnectToServerAsync(pipeClient, lineProcessor);

                    // Sleep until connection.
                    while (!pipeClient.IsConnected)
                    {
                        Thread.Sleep(50);
                    }

                    // Log about establishing.
                    Console.WriteLine("{0}/{1}: Connection established.", lineProcessor.ServerName, lineProcessor.ServerPipeName);

                    try
                    {
                        // Execute target query.
                        lineProcessor.queryProcessor?.Invoke(lineProcessor);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("{0}/{1}: EXECUTION TIME ERROR. {2}", lineProcessor.ServerName, lineProcessor.ServerPipeName, ex.Message);
                    }

                    // Log about establishing.
                    Console.WriteLine("{0}/{1}: Transmission finished at {2}.", lineProcessor.ServerName, lineProcessor.ServerPipeName, DateTime.Now.ToString("HH:mm:ss.fff"));

                    // Remove not relevant meta data.
                    lineProcessor.pipeClient.Dispose();
                    lineProcessor.DropMeta();

                    Console.WriteLine();
                }

                // Let other threads time for processing before next query.
                Thread.Sleep(50);
            }
        }

        /// <summary>
        /// Try to find out registred transmission line by GUID.
        /// If client not strted then return false.
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="lineProcessor"></param>
        /// <returns></returns>
        public static bool TryGetTransmissionLineByGUID(
            string guid, 
            out TransmissionLine lineProcessor)
        {
            lineProcessor = openedClients[guid] as TransmissionLine;
            return lineProcessor != null;
        }

        /// <summary>
        /// Trying to register transmission line in common table by key:
        /// ServerName.PipeLine
        /// 
        /// If exist return false.
        /// Retrun sycces if added to table.
        /// </summary>
        /// <param name="lineProcessor"></param>
        /// <returns></returns>
        public static bool TryToRegisterTransmissionLine(TransmissionLine lineProcessor)
        {
            // Build pipe domain.
            string lineDomain = lineProcessor.ServerName + "." + lineProcessor.ServerPipeName;

            // Reject if already registred.
            if (openedClients[lineDomain] is TransmissionLine)
            {
                Console.WriteLine("LINE PROCESSOR \"{0}\" ALREADY EXIST.", lineDomain);
                return false;
            }

            // Add line to table.
            openedClients.Add(lineDomain, lineProcessor);
            return true;
        }
        #endregion
    }
}