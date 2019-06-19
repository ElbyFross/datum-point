// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// Copyright (c) 2019 Volodymyr Podshyvalov

using System;
using System.IO.Pipes;

namespace PipesProvider
{
    /// <summary>
    /// Container that contain meta data about server instance.
    /// </summary>
    public class ServerTransmissionMeta
    {
        /// <summary>
        /// Object that provide access to async connection.
        /// </summary>
        public IAsyncResult connectionMarker;

        /// <summary>
        /// Delegate that will be called when connection will be established.
        /// ServerTransmissionMeta - meta data of transmission.
        /// </summary>
        public System.Action<ServerTransmissionMeta> connectionCallback;

        /// <summary>
        /// Delegate that will be called when server will recive query.
        /// ServerTransmissionMeta - meta data of transmission.
        /// string - shared query.
        /// </summary>
        public System.Action<ServerTransmissionMeta, string> queryHandlerCallback;

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

        /// <summary>
        /// Query that aqtualu in processing. 
        /// 
        /// Attention: Value can be changed if some of handlers will call disconecction or transmission error. 
        /// This situation will lead to establishing new connection that lead to changing of this value.
        /// </summary>
        public string ProcessingQuery { get; set; }

        #region Constructors
        public ServerTransmissionMeta() { }

        public ServerTransmissionMeta(
            IAsyncResult connectionMarker, 
            System.Action<ServerTransmissionMeta> connectionCallback,
            System.Action<ServerTransmissionMeta, string> queryHandlerCallback,
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
        public static ServerTransmissionMeta None
        {
            get { return new ServerTransmissionMeta(); }
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
