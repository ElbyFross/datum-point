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
    public class ServerAnswerTransmissionController : BaseServerTransmissionController
    {
        /// <summary>
        /// Query that actualy in processing. 
        /// 
        /// Attention: Value can be changed if some of handlers will call disconecction or transmission error. 
        /// This situation will lead to establishing new connection that lead to changing of this value.
        /// </summary>
        public string ProcessingQuery { get; set; }

        // Set uniform constructor.
        public ServerAnswerTransmissionController(
           IAsyncResult connectionMarker,
           System.Action<BaseServerTransmissionController> connectionCallback,
           System.Action<BaseServerTransmissionController, string> queryHandlerCallback,
           NamedPipeServerStream pipe,
           string pipeName) : base(
                connectionMarker,
                connectionCallback,
                queryHandlerCallback,
                pipe,
                pipeName)
        { }
    }
}
