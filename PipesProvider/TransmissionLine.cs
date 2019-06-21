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
using System.IO.Pipes;
using System.Threading;

namespace PipesProvider
{
    /// <summary>
    /// Class that provide information about line between client and server.
    /// Provide API to easy control.
    /// Provide automatic services.
    /// </summary>
    public class TransmissionLine
    {
        #region Public properties
        /// <summary>
        /// Unique GUID for this pipe.
        /// </summary>
        public string GUID
        {
            get
            {
                // Generate GUID if not found.
                if (string.IsNullOrEmpty(guid))
                    guid = GenerateGUID(ServerName, ServerPipeName);
                return guid;
            }
        }
        private string guid = null;

        /// <summary>
        /// Name of server pipe that will be using for transmission via current processor.
        /// </summary>
        public string ServerPipeName
        {
            get;
            protected set;
        }

        /// <summary>
        /// Name of server pipe that will be using for transmission via current processor.
        /// </summary>
        public string ServerName
        {
            get;
            protected set;
        }

        /// <summary>
        /// If true then this line will be closed on the next client tick.
        /// </summary>
        public bool Closed
        {
            get;
            protected set;
        }

        /// <summary>
        /// True if async operation started and not finished.
        /// </summary>
        public bool Processing
        {
            get;
            set;
        }

        /// <summary>
        /// Return tthe query that was dequeue at last.
        /// </summary>
        public QueryContainer LastQuery
        {
            get
            {
                return lastQuery.IsEmpty ? QueryContainer.Empty : QueryContainer.Copy(lastQuery);
            }
            protected set
            {
                lastQuery = value;
            }
        }
        #endregion

        #region Public fields
        /// <summary>
        /// Reference to the current oppened pipe.
        /// </summary>
        public NamedPipeClientStream pipeClient;

        /// <summary>
        /// This delegate will be callback when connection wfor qury will be established.
        /// </summary>
        public System.Action<TransmissionLine> queryProcessor;
        #endregion

        #region Protected fields
        /// <summary>
        /// Field that contain last dequeued query.
        /// </summary>
        protected QueryContainer lastQuery = QueryContainer.Empty;

        /// <summary>
        /// List of queries that will wait its order to access transmission via this line.
        /// </summary>
        protected Queue<QueryContainer> queries = new Queue<QueryContainer>();
        #endregion


        #region Constructors
        /// <summary>
        /// Create new instance of LineProcessor taht can be registread in static services.
        /// Contain information about transmission between client and server.
        /// </summary>
        /// <param name="guid">Unique value that will be used to access this prossor.</param>
        /// <param name="serverName">Name of server into the network. If local than place "."</param>
        /// <param name="serverPipeName">Name of the pipe that will be used for transmitiong.</param>
        /// <param name="queryProcessor">Delegat that will be called when connection will be established.</param>
        public TransmissionLine(string serverName, string serverPipeName, System.Action<TransmissionLine> queryProcessor)
        {
            // Set fields.
            ServerName = serverName;
            ServerPipeName = serverPipeName;
            this.queryProcessor = queryProcessor;

            // Registrate at hashtable.
            API.TryToRegisterTransmissionLine(this);
        }
        #endregion

        #region API
        /// <summary>
        /// Enqueue query to order. Query will be posted to server as soon as will possible.
        /// </summary>
        /// <param name="query"></param>
        public void EnqueueQuery(string query)
        {
            queries.Enqueue(new QueryContainer(query, null));
        }

        /// <summary>
        /// Enqueue query to order. Query will be posted to server as soon as will possible.
        /// </summary>
        /// <param name="query"></param>
        public void EnqueueQuery(QueryContainer query)
        {
            queries.Enqueue(query);
        }

        /// <summary>
        /// Try to get a new query in turn.
        /// 
        /// Will return false if query not found.
        /// Will return false in case if LineProccessor has status InProgress.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public bool TryDequeQuery(out QueryContainer query)
        {
            // If some query already started then reject operation.
            if (Processing)
            {
                query = QueryContainer.Empty;
                return false;
            }

            try
            {
                // Dequeue query
                QueryContainer dequeuedQuery = queries.Dequeue();

                // Buferize at last.
                LastQuery = dequeuedQuery;

                // Initialize answer.
                query = dequeuedQuery;

                // Mark processor as busy.
                Processing = true;

                Console.WriteLine("QUERY DEQUEUED: {0}", LastQuery);

                // inform about success.
                return true;
            }
            catch (Exception ex)
            {
                // Inform about error during request.
                Console.WriteLine("LINE PROCESSOR ERROR (GUID: \"{3}\" ADDRESS: {0}/{1}): {2}",
                    ServerName, ServerPipeName, ex.Message, GUID);

                // Drop relative data.
                LastQuery = QueryContainer.Empty;
                query = QueryContainer.Empty;

                // Infor about failure.
                return false;
            }

        }

        /// <summary>
        /// Mark line as closed. Thread will be terminated on the next client tick.
        /// </summary>
        public void Close()
        {
            // Mark as closed.
            Closed = true;

            // Drop processing marker to allow loop drop waiting to async operrations.
            Processing = false;

            // Remove from table.
            API.TryToUnregisterTransmissionLine(GUID);
        }

        /// <summary>
        /// Drop meta data relative only per one session.
        /// </summary>
        public void DropMeta()
        {
            pipeClient = null;
            Processing = false;
        }

        /// <summary>
        /// Return true if queue contain queries.
        /// </summary>
        public bool HasQueries
        { get {  return queries.Count > 0; } }
        #endregion

        /// <summary>
        /// Method that can be started as thread. Will start client loop.
        /// </summary>
        /// <param name="lineProcessor"></param>
        public static void ThreadLoop(object lineProcessor)
        {
            // Drop if incorrect argument.
            if (!(lineProcessor is TransmissionLine lp))
            {
                Console.WriteLine("THREAD NOT STARTED. INVALID ARGUMENT.");
                return;
            }

            // Change thread cuture.
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-us");
            Console.WriteLine("THREAD STARTED: {0}", Thread.CurrentThread.Name);

            // Start client loop.
            PipesProvider.API.ClientLoop(
                lp,
                System.IO.Pipes.PipeDirection.InOut,
                PipeOptions.Asynchronous | PipeOptions.WriteThrough);
        }

        /// <summary>
        /// Generate GUID of this transmission line relative to pipe params.
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="pipeName"></param>
        /// <returns></returns>
        public static string GenerateGUID(string serverName, string pipeName)
        {
            if(string.IsNullOrEmpty(serverName))
            {
                Console.WriteLine("EROOR (TL GUID): Server name can't be null or empty.");
                return null;
            }

            if (string.IsNullOrEmpty(pipeName))
            {
                Console.WriteLine("EROOR (TL GUID): Pipe name can't be null or empty.");
                return null;
            }
            //return serverName.GetHashCode() + "_" + pipeName.GetHashCode();
            return serverName + "." + pipeName;
        }


        /// <summary>
        /// Incremet of attempts count.
        /// </summary>
        /// <param name="contaier"></param>
        /// <returns></returns>
        public static TransmissionLine operator ++(TransmissionLine line)
        {
            line.lastQuery++;
            return line;
        }
    }
}