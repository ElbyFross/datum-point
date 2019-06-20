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
            // React on uniform arguments.
            ArgsReactor(args);

            // Check direcroties
            LoadAssemblies(AppDomain.CurrentDomain.BaseDirectory + "libs\\");

            Console.WriteLine("Preparetion finished. Client strated.");

            // Create transmission line.
            PipesProvider.TransmissionLine lineProcessor = OpenTransmissionLine(
                new Client(),
                "DataServer #0",
                ".", "THB_DS_QM_MAIN_INOUT",
                UniformQueryPostHandler
                );

            // Sent test message.
            lineProcessor.EnqueueQuery("guid=WelomeGUID&token=InvalidToken&q=Get&sq=publickey");

            System.Action<PipesProvider.TransmissionLine, object> answerProcessor =
                delegate (PipesProvider.TransmissionLine tl, object message)
            {
                string messageS = message as string;

                Console.WriteLine("DELEGATE");
                Console.WriteLine(messageS ?? "Message is null");
            };

            ReciveAnswer(
                lineProcessor,
                UniformQueries.API.DetectQueryParts("guid=WelomeGUID&token=InvalidToken&q=Get&sq=publickey"),
                answerProcessor);

            Thread.Sleep(150);

            while(true)
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

            // Close line. Withot this thread will be hanged.
            lineProcessor.Close();

            // Whait until close.
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        /// <summary>
        /// Query processor that send last dequeued query to server when connection will be established.
        /// </summary>
        /// <param name="lineProcessor"></param>
        //static void UniformQueryPOSTProcessor(object sharedObject)
        //{
        //    // Drop as invalid in case of incorrect transmitted data.
        //    if (!(sharedObject is PipesProvider.TransmissionLine lineProcessor))
        //    {
        //        Console.WriteLine("TRANSMISSION ERROR (UQPP0): INCORRECT TRANSFERD DATA TYPE. PERMITED ONLY \"LineProcessor\"");
        //        return;
        //    }

        //    // Open stream reader.
        //    StreamWriter sw = new StreamWriter(lineProcessor.pipeClient);
        //    try
        //    {
        //        sw.WriteLine(lineProcessor.LastQuery);
        //        sw.Flush();
        //        Console.WriteLine("TRANSMITED: {0}", lineProcessor.LastQuery);
        //        //sw.Close();
        //    }
        //    // Catch the Exception that is raised if the pipe is broken or disconnected.
        //    catch (Exception e)
        //    {
        //        Console.WriteLine("DNS HANDLER ERROR ({1}): {0}", e.Message, lineProcessor.pipeClient.GetHashCode());

        //        // Retry transmission.
        //        if (lineProcessor.LastQuery.Attempts < 10)
        //        {
        //            // Add to queue.
        //            lineProcessor.EnqueueQuery(lineProcessor.LastQuery);

        //            // Add attempt.
        //            PipesProvider.QueryContainer qcl = lineProcessor.LastQuery;
        //            qcl++;
        //        }
        //        else
        //        {
        //            // If transmission attempts over the max count.
        //        }
        //    }

        //    // If requested data in answer.
        //    if(lineProcessor.LastQuery.Query.StartsWith("GET", StringComparison.OrdinalIgnoreCase))
        //    {
        //        // Create answer reciving line.
        //        PipesProvider.TransmissionLine reciveAnswer = OpenTransmissionLine(
        //            new Client(),
        //            "DataServer #0 a" + lineProcessor.LastQuery.Query.Split(' '),
        //            ".", "THB_DS_QM_MAIN_INOUT",
        //            UniformQueryPOSTProcessor
        //            );
        //    }
        //}

    }
}
