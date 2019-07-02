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
using PipesProvider.Server;
using UQAPI = UniformQueries.API;

namespace PipesProvider.Handlers
{
    /// <summary>
    /// Handlers that provide transmission between serve and clients.
    /// </summary>
    public static class DNS
    {
        /// <summary>
        /// Code that will work on server loop when connection will be established.
        /// Recoomended to using as default DNS Handler for queries reciving.
        /// </summary>
        public static async void ClientToServerAsync(ServerTransmissionController meta)
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
                        queryBufer = await sr.ReadToEndAsync();
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
                if (string.IsNullOrEmpty(queryBufer))
                {
                    //Console.WriteLine("NULL REQUEST AVOIDED. CONNECTION TERMINATED.");
                    break;
                }

                // Log query before decryption.
                //Console.WriteLine(@"RECIVED QUERY (DNS01): {0}", queryBufer);

                // Try to decrypt. In case of fail decryptor return entry message.
                queryBufer = Security.Crypto.DecryptString(queryBufer);

                // Log query.
                Console.WriteLine(@"RECIVED QUERY (DNS0): {0}", queryBufer);

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
        public static async void ServerToClientAsync(ServerTransmissionController meta)
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
                Console.WriteLine("{0}: Start transmission to client.", meta.name);
                await sw.WriteAsync(sharedQuery);
                await sw.FlushAsync();
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
    }
}
