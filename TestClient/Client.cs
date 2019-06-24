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

using System.Diagnostics;
using System.Security.Principal;
using System.Security.Permissions;
using Microsoft.Win32.SafeHandles;
using System.Runtime.ConstrainedExecution;
using System.Security;
using System.Net;
using System.Runtime.InteropServices;

namespace TestClient
{
    /// <summary>
    /// Provide example of client-server transmission.
    /// Demostrate ways to use API.
    /// Provide tools for simple tests of local server.
    /// </summary>
    class Client : UniformClient.BaseClient
    {
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool LogonUser(String lpszUsername, String lpszDomain, String lpszPassword,
        int dwLogonType, int dwLogonProvider, out SafeAccessTokenHandle phToken);

        /// <summary>
        /// Server that will be used as target for this client.
        /// </summary>
        public static string SERVER_NAME = "192.168.1.74"; // Dot queal to local.

        /// <summary>
        /// Pipe that will be used to queries of this client.
        /// </summary>
        public static string SERVER_PIPE_NAME = "THB_DS_QM_MAIN_INOUT"; // Pipe openned at server that will recive out queries.

        static void Main(string[] args)
        {
            if (!SERVER_NAME.Equals("."))
                SERVER_NAME = PipesProvider.Networking.GetHostName(SERVER_NAME);
                       
            PipesProvider.Networking.PingHost(
                SERVER_NAME, 445,
                delegate(string uri, int port) 
                { Console.WriteLine("PING COMPLITED | HOST AVAILABLE | {0}:{1}", uri, port); });


            #region Impersonated remote user
            SafeAccessTokenHandle safeTokenHandle;
            string userName, domainName;
            // Get the user token for the specified user, domain, and password using the
            // unmanaged LogonUser method.
            // The local machine name can be used for the domain name to impersonate a user on this machine.
            Console.Write("Enter the name of the domain on which to log on: ");
            domainName = "workgroup";// SERVER_NAME;

            Console.Write("Enter the login of a user on {0} that you wish to impersonate: ", domainName);
            userName = "remoteUserName";// Console.ReadLine();

            Console.Write("Enter the password for {0}: ", userName);

            const int LOGON32_PROVIDER_WINNT50 = 3;
            //This parameter causes LogonUser to create a primary token.
            const int LOGON_TYPE_NEW_CREDENTIALS = 9;


            // Call LogonUser to obtain a handle to an access token.
            bool returnValue = LogonUser(userName, domainName, "remoteUserPassword",//Console.ReadLine(),
                LOGON_TYPE_NEW_CREDENTIALS, LOGON32_PROVIDER_WINNT50,
                out safeTokenHandle);
            if (false == returnValue)
            {
                int ret = Marshal.GetLastWin32Error();
                Console.WriteLine("LogonUser failed with error code : {0}", ret);
                throw new System.ComponentModel.Win32Exception(ret);
            }
            Console.WriteLine("Did LogonUser Succeed? " + (returnValue ? "Yes" : "No"));
            // Check the identity.  
            Console.WriteLine("Before impersonation: " + WindowsIdentity.GetCurrent().Name);
            #endregion

            // Make anonimys user.
            safeTokenHandle = WindowsIdentity.GetAnonymous().AccessToken;

            WindowsIdentity.RunImpersonated(safeTokenHandle, () =>
            {
                #region Init
                // React on uniform arguments.
                ArgsReactor(args);

                // Check direcroties
                LoadAssemblies(AppDomain.CurrentDomain.BaseDirectory + "libs\\");

                Console.WriteLine("Preparetion finished. Client strated.");
                #endregion

                // Usew short wway to send one way query.
                OpenOutTransmissionLine(SERVER_NAME, SERVER_PIPE_NAME).EnqueueQuery("ECHO");

                // Send sample one way query to server with every step description.
                SendOneWayQuery("ECHO");

                // Get public key for RSA encoding from target server.
                RequestPublicRSAKey();

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
            });
        }


        #region Queries
        static void SendOneWayQuery(string query)
        {
            // Create transmission line.
            PipesProvider.TransmissionLine lineProcessor = OpenOutTransmissionLine(SERVER_NAME, SERVER_PIPE_NAME);

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
        static void ServerAnswerHandler_RSAPublicKey(PipesProvider.TransmissionLine tl, object message)
        {
            string messageS = message as string;
            Console.WriteLine("RSA Public Key recived:\n" + (messageS ?? "Message is null"));
        }
        #endregion
    }
}
