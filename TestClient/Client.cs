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
using System.Threading;
using Microsoft.Win32.SafeHandles;
using PipesProvider.Networking;
using PipesProvider.Security;

namespace TestClient
{
    /// <summary>
    /// Provide example of client-server transmission.
    /// Demostrate ways to use API.
    /// Provide tools for simple tests of local server.
    /// </summary>
    class Client : UniformClient.BaseClient
    {
        /// <summary>
        /// Server that will be used as target for this client.
        /// </summary>
        public static string SERVER_NAME = ".";//"192.168.1.74"; // Dot equal to local.

        /// <summary>
        /// Pipe that will be used to queries of this client.
        /// </summary>
        public static string SERVER_PIPE_NAME = "THB_DS_QM_MAIN_INOUT"; // Pipe openned at server that will recive out queries.


        static void Main(string[] args)
        {
            #region Init
            // React on uniform arguments.
            ArgsReactor(args);

            // Check direcroties
            LoadAssemblies(AppDomain.CurrentDomain.BaseDirectory + "libs\\");

            Console.WriteLine("Preparetion finished. Client strated.");
            #endregion


            // Try to make human clear naming of server. In case of local network we will get the machine name.
            // This is optional and not required for stable work, just little helper for admins.
            PipesProvider.Networking.Info.TryGetHostName(SERVER_NAME, ref SERVER_NAME);


            // Check server exist. When connection will be established will be called shared delegate.
            // Port 445 required for named pipes work.
            PipesProvider.Networking.Info.PingHost(
                SERVER_NAME, 445,
                delegate (string uri, int port)
                {
                    Console.WriteLine("PING COMPLITED | HOST AVAILABLE | {0}:{1}\n", uri, port);

                    // Send few example queries to server.
                    TransmissionsBlock();
                });
            
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
                                SendOneWayQuery("ECHO" + i + "/" + repeaterRequest);
                            }
                        }
                        // Share custom query.
                        else SendOneWayQuery(tmp);
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

        /// <summary>
        /// Method that will send few sample to remote server.
        /// </summary>
        static void TransmissionsBlock()
        {
            // Usew short way to send one way query.
            OpenOutTransmissionLine(SERVER_NAME, SERVER_PIPE_NAME).EnqueueQuery("ECHO");

            // Send sample one way query to server with every step description.
            SendOneWayQuery("ECHO");

            // Get public key for RSA encoding from target server.
            RequestPublicRSAKey();
        }

        #region Queries
        static void SendOneWayQuery(string query)
        {
            #region Authorizing on remote machine
            // Get right to access remote machine.
            //
            // If you use anonymous conection than you need to apply server's LSA (LocalSecurityAuthority) rules:
            // - permit Guest connection over network.
            // - activate Guest user.
            // Without this coonection will terminated by server.
            //
            // Relative to setting of pipes also could be required:
            // - anonymous access to named pipes
            bool logonResult = General.TryLogon(LogonConfig.Anonymous, out SafeAccessTokenHandle safeTokenHandle);
            if (!logonResult)
            {
                Console.WriteLine("Logon failed. Connection not possible.\nPress any key...");
                Console.ReadKey();
                return;
            }
            #endregion

            // Create transmission line.
            TransmissionLine lineProcessor = OpenOutTransmissionLine(SERVER_NAME, SERVER_PIPE_NAME);
            // Set impersonate token.
            lineProcessor.AccessToken = safeTokenHandle;

            // Add sample query to queue. You can use this way if you not need answer from server.
            lineProcessor.EnqueueQuery(query);
        }

        static void RequestPublicRSAKey()
        {
            // Create query that request public RSA key of the server. 
            //This will allow to us encrypt queries and shared data befor transmission in future.
            //
            // Format: param=value&param=value&...
            // "guid", "token" and "q" (query) required.
            //
            // Param "pk" (public key (RSA)) will provide possibility to encrypt of answer on the server side.
            //
            // Using a UniformQueries.API.SPLITTING_SYMBOL to get a valid splitter between your query parts.
            string GetPKQuery = string.Format("guid=WelomeGUID{0}token=InvalidToken{0}q=Get{0}sq=publickey", UniformQueries.API.SPLITTING_SYMBOL);

            // Open duplex chanel. First line processor will send query to server and after that will listen to its andwer.
            // When answer will recived it will redirected to callback.
            EnqueueDuplexQuery(SERVER_NAME, SERVER_PIPE_NAME, GetPKQuery, ServerAnswerHandler_RSAPublicKey);

            // Let the time to transmission line to qompleet the query.
            Thread.Sleep(150);
        }
        #endregion


        #region Server's answer callbacks
        // Create delegate that will recive and procced the server's answer.
        static void ServerAnswerHandler_RSAPublicKey(TransmissionLine tl, object message)
        {
            string messageS = message as string;
            Console.WriteLine("RSA Public Key recived:\n" + (messageS ?? "Message is null"));
        }
        #endregion
    }
}
