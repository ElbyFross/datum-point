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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Threading;
using UniformQueries;

namespace DatumPoint.Networking
{
    /// <summary>
    /// Instance of this class will provide client features.
    /// </summary>
    public class Client : UniformClient.BaseClient
    {
        /// <summary>
        /// Reference to current client provider.
        /// Always return not null result.
        /// </summary>
        public static Client Active
        {
            get
            {
                if (active == null)
                    active = new Client();
                return active;
            }

            protected set { active = value; }
        }

        protected static Client active;


        /// <summary>
        /// Is guest token required.
        /// </summary>
        public bool GuestTokenRequired { get; protected set; }

        public Client()
        {
            // Set as active.
            Active = this;

            // Loading assemblies from lib direcroty.
            LoadAssemblies(AppDomain.CurrentDomain.BaseDirectory + "libs\\");
            
            // Loading assemblies from plugins direcroty.
            LoadAssemblies(AppDomain.CurrentDomain.BaseDirectory + "plugins\\");

            // Load translation for plugins relative to thread culture.
            WpfHandler.Localization.API.LoadXAML_LangDicts(CultureInfo.CurrentCulture, new CultureInfo("en-US"));
        }

        /// <summary>
        /// Trying to receive guest token from server.
        /// </summary>
        /// <param name="serverIP">Ip of server.</param>
        /// <param name="pipeName">Name of the broadcasting pipe that would share tokent for this client.</param>
        /// <param name="timeout">Milisecond before connection terminating.</param>
        public void ReceiveGuestToken(string serverIP, string pipeName, float timeout = 2000)
        {
            // Time when connection would terminated.
            DateTime terminationTime = DateTime.Now.AddMilliseconds(timeout);

            // Open client that will listen server guest chanel broadcasting.
            PipesProvider.Client.TransmissionLine broadcastingLine =
                UniformClient.Standard.SimpleClient.ReciveAnonymousBroadcastMessage(
                serverIP,
                pipeName,
                (PipesProvider.Client.TransmissionLine line, object obj) =>
                {
                    // Validate answer.
                    if (obj is string answer)
                    {
                        Console.WriteLine("GUSET BROAADCASTING CHANEL ANSWER RECIVED: {0}", answer);
                        // Unlock finish blocker.
                        GuestTokenRequired = false;

                        QueryPart[] recivedQuery = UniformQueries.API.DetectQueryParts(answer);

                        // Check token.
                        if (UniformQueries.API.TryGetParamValue("token", out QueryPart tokenQP, recivedQuery) &&
                        !string.IsNullOrEmpty(tokenQP.propertyValue))
                        {
                            token = tokenQP.propertyValue;
                            Console.WriteLine("Guest token: {0}", token);
                        }
                        else
                        {
                            Console.WriteLine("Guest token not detected. Authorization not possible.");
                            return;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Guest token not recived. Incorrect answer format.");
                        return;
                    }
                });

            // Log
            Console.WriteLine("Witing forguest token from server autority system...");

            // Wait for guest token.
            while (GuestTokenRequired)
            {
                // If timeout is reached.
                bool timeoutPassed = DateTime.Compare(DateTime.Now, terminationTime) > 0;
                if (timeoutPassed)
                {
                    // Close trasmission line.
                    broadcastingLine.Close();
                    
                    Console.WriteLine("Connection is out of timeout. Guest token not recived.");
                    return;
                }

                // Whait
                Thread.Sleep(5);
            }
        }

        /// <summary>
        /// Make first start of client.
        /// </summary>
        public static void Init()
        {
            _ = Active;
        }
    }
}