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
using PipesProvider.Networking.Routing;

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
        /// <summary>
        /// Bufer that contains reference to current active client.
        /// </summary>
        protected static Client active;
        
        /// <summary>
        /// Routing tables loaded to that client.
        /// </summary>
        public RoutingTable RoutingTable
        {
            get
            {
                return routingTable;
            }
        }

        /// <summary>
        /// Instiniate client object. Loadign dlls and plugins.
        /// </summary>
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

            // Loading routing data.
            InitRoutingTables();
        }

        /// <summary>
        /// Instiniate client object. Loadign dlls and plugins.
        /// Initialize client with routing table shared from diferent obejct.
        /// </summary>
        /// <param name="table">Pre-initialized routing table.</param>
        public Client(RoutingTable table)
        {
            // Set as active.
            Active = this;

            // Loading assemblies from lib direcroty.
            LoadAssemblies(AppDomain.CurrentDomain.BaseDirectory + "libs\\");

            // Loading assemblies from plugins direcroty.
            LoadAssemblies(AppDomain.CurrentDomain.BaseDirectory + "plugins\\");

            // Load translation for plugins relative to thread culture.
            WpfHandler.Localization.API.LoadXAML_LangDicts(CultureInfo.CurrentCulture, new CultureInfo("en-US"));

            // Aplly routing data.
            routingTable = table;
        }

        /// <summary>
        /// Inisitalize interanal routing table. 
        /// 
        /// Loading data from local filesystem.
        /// Trying to logon if reuqires partial authorization.
        /// </summary>
        public void InitRoutingTables()
        {
            // Loading roting tables to detect servers.
            LoadRoutingTables(AppDomain.CurrentDomain.BaseDirectory + "plugins\\");

            // Logon all partial authorized instruction.
            foreach (Instruction instruction in routingTable.intructions)
            {
                // If instruction require guest token.
                if (instruction is PartialAuthorizedInstruction pai)
                {
                    // Trying to recive guest token from server.
                    pai.TryToGetGuestTokenAsync(null,
                        AuthorityController.Session.Current.TerminationTokenSource.Token); // Using Seestion termination token as uniform 
                                                                               //to provide possibility to stop all async operation before application exit.
                }
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