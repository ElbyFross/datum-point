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
using System.Threading;
using System.IO;
using System.IO.Pipes;

namespace TestClient
{
    class Client : UniformClient.BaseClient
    {
        static void Main(string[] args)
        {
            #region Init
            // React on uniform arguments.
            ArgsReactor(args);

            // Check direcroties
            LoadAssemblies(AppDomain.CurrentDomain.BaseDirectory + "libs\\");

            Console.WriteLine("Preparetion finished. Client strated.");
            #endregion

            #region Create sample duplex query
            // Create transmission line.
            PipesProvider.TransmissionLine lineProcessor = OpenTransmissionLine(
                ".", "THB_DS_QM_MAIN_INOUT", UniformQueryPostHandler);

            #region Query tip & description
            // Create query that request public RSA key of the server. 
            //This will allow to us encrypt queries and shared data befor transmission in future.
            //
            // Format: param=value&param=value&...
            // "guid", "token" and "q" (query) required.
            // param "pk" (public key (RSA)) will provide possibility to encrypt of answeron the server side.
            #endregion
            string THB_DS_QM_MAIN_INOUT_Query = "guid=WelomeGUID&token=InvalidToken&q=Get&sq=publickey";

            // Add our query to line processor queue.
            lineProcessor.EnqueueQuery(THB_DS_QM_MAIN_INOUT_Query);
            
            // Open backward chanel to recive answer from server.
            ReciveAnswer(lineProcessor, THB_DS_QM_MAIN_INOUT_Query, ServerPKProcessor);

            // Let the time to transmission line to qompleet the query.
            Thread.Sleep(150);
            #endregion

            #region Main loop
            while (true)
            {
                if (Console.KeyAvailable)
                {
                    Console.Write("Enter command: ");
                    string tmp = Console.ReadLine();

                    // Skip empty requests.
                    if (string.IsNullOrEmpty(tmp)) continue;

                    // Close application by command.
                    if (tmp == "close") break;
                    else
                    {
                        // If included counter then spawn a echo in loop.
                        if (Int32.TryParse(tmp, out int repeaterRequest))
                        {
                            for (int i = 1; i < repeaterRequest + 1; i++)
                            {
                                lineProcessor.EnqueueQuery("ECHO" + i + "/" + repeaterRequest);
                            }
                        }
                        // Share custom query.
                        else lineProcessor.EnqueueQuery(tmp);
                    }
                }
            }
            #endregion

            // Close all active lines. Without this operation thread will be hanged.
            PipesProvider.API.CloseAllTransmissionLines();

            // Whait until close.
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
        
        // Create delegate that will recive and procced the server's answer.
        static void ServerPKProcessor(PipesProvider.TransmissionLine tl, object message)
        {
            string messageS = message as string;
            Console.WriteLine(messageS ?? "Message is null");
        }
    }
}
