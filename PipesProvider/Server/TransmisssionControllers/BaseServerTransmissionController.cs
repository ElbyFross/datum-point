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
using System.IO.Pipes;

namespace PipesProvider.Server.TransmissionControllers
{
    /// <summary>
    /// Container that contain meta data about server instance.
    /// </summary>
    public class BaseServerTransmissionController
    {
        /// <summary>
        /// Object that provide access to async connection.
        /// </summary>
        public IAsyncResult connectionMarker;

        /// <summary>
        /// Delegate that will be called when connection will be established.
        /// ServerTransmissionMeta - meta data of transmission.
        /// </summary>
        public System.Action<BaseServerTransmissionController> connectionCallback;

        /// <summary>
        /// Delegate that will be called when server will recive query.
        /// ServerTransmissionMeta - meta data of transmission.
        /// string - shared query.
        /// </summary>
        public System.Action<BaseServerTransmissionController, string> queryHandlerCallback;

        /// <summary>
        /// Reference to created pipe.
        /// </summary>
        public NamedPipeServerStream pipe;

        /// <summary>
        /// Name of this connection.
        /// </summary>
        public string name;

        /// <summary>
        /// Marker that show does current transmission is relevant.
        /// When it'll become true this pipe connection will be desconected.
        /// </summary>
        public bool Expired { get; protected set; }

        /// <summary>
        /// Marker that show does this transmition stoped.
        /// </summary>
        public bool Stoped { get; protected set; }
        
        #region Constructors
        public BaseServerTransmissionController() { }

        public BaseServerTransmissionController(
            IAsyncResult connectionMarker, 
            System.Action<BaseServerTransmissionController> connectionCallback,
            System.Action<BaseServerTransmissionController, string> queryHandlerCallback,
            NamedPipeServerStream pipe, string pipeName)
        {
            this.connectionMarker = connectionMarker;
            this.connectionCallback = connectionCallback;
            this.queryHandlerCallback = queryHandlerCallback;
            this.pipe = pipe;
            this.name = pipeName;
            Expired = false;
            Stoped = false;
        }

        /// <summary>
        /// Return instance that not contain initialized fields.
        /// </summary>
        public static BaseServerTransmissionController None
        {
            get { return new BaseServerTransmissionController(); }
        }
        #endregion

        /// <summary>
        /// Maeking transmission as expired. Line will be remaked.
        /// </summary>
        public void SetExpired()
        {
            Expired = true;

            Console.WriteLine("{0}: PIPE SERVER MANUALY EXPIRED", name);
        }

        /// <summary>
        /// Marking transmission as expired and stoped for full exclusion 
        /// from automatic server operations.
        /// </summary>
        public void SetStoped()
        {
            Stoped = true;
            Expired = true;

            Console.WriteLine("{0}: PIPE SERVER MANUALY STOPED", name);
        }
    }
}
