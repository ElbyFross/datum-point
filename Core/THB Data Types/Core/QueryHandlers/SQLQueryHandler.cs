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
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UniformQueries;
using UniformQueries.Executable;
using DatumPoint.Types.Personality;

namespace DatumPoint.Queries.Handlers
{
    /// <summary>
    /// Query handler specified to processing queries focused on caoonecting SQL server.
    /// </summary>
    public abstract class SQLQueryHandler : IQueryHandler
    {
        /// <summary>
        /// Container that contains meta data of query processing.
        /// </summary>
        public class QueryMeta
        {
            /// <summary>
            /// Id sql operation failed.
            /// </summary>
            public bool sqlDataOperationFailed;

            /// <summary>
            /// Query that was received by server.
            /// </summary>
            public Query entryQuery;

            /// <summary>
            /// Return token registration data.
            /// </summary>
            public AuthorityController.Data.Temporal.TokenInfo TokenInfo
            {
                get
                {
                    // Get token if not found before.
                    if (tokenInfo == null)
                    {
                        // Looking for token in query.
                        if (!entryQuery.TryGetParamValue("token", out QueryPart token))
                        {
                            Console.WriteLine("ERROR : USER INFO GET | Token not found.");
                            return null;
                        }

                        // Getting token info.
                        if (AuthorityController.Session.Current.TryGetTokenInfo(
                            token.PropertyValueString,
                            out AuthorityController.Data.Temporal.TokenInfo ti))
                        {
                            tokenInfo = ti;
                        }
                        else
                        {
                            Console.WriteLine("ERROR : USER INFO GET | Token infor not registred.");
                            return null;
                        }
                    }

                    return tokenInfo;
                }
            }

            /// <summary>
            /// Bufer that contains stored token info.
            /// </summary>
            private AuthorityController.Data.Temporal.TokenInfo tokenInfo;
        }

        /// <summary>
        /// Hashtable that contains pais:
        /// object - Object used as seed to builing sql query.
        /// QueryMeta - meta data of that query. 
        /// </summary>
        protected Hashtable metaTable = new Hashtable();
        
        /// <summary>
        /// Token must has rank higher then described.
        /// </summary>
        public virtual UserRank RankUperThen { get; set; } = UserRank.None;

        /// <summary>
        /// Type of object shared with query, that has sql table descriptor.
        /// </summary>
        public virtual Type TableType { get; set; }

        /// <summary>
        /// Property in query that contains shared object that will be set to db.
        /// </summary>
        public virtual string SharedObjectProperty { get; set; }

        /// <summary>
        /// Rights required from token to operate that query.
        /// Excluding rank's rights.
        /// </summary>
        public virtual string[] RequiredRights { get; set; }


        public SQLQueryHandler()
        {
            // Subscribe.
            UniformDataOperator.Sql.SqlOperatorHandler.SqlErrorOccured += ErrorListener;
        }

        ~SQLQueryHandler()
        {
            // Unsubscribe.
            UniformDataOperator.Sql.SqlOperatorHandler.SqlErrorOccured -= ErrorListener;
        }

        public abstract string Description(string cultureKey);

        public abstract void Execute(object sender, Query query);

        public abstract bool IsTarget(Query query);

        /// <summary>
        /// Validates configiguration of SqlOperator.
        /// Send backward error message in case if operator invalid.
        /// </summary>
        /// <param name="entryQuery">Query received by server from client.</param>
        /// <returns>Is configuration valid?</returns>
        protected bool ValidateSqlSettings(Query entryQuery)
        {
            // Check databasde connection.
            if (UniformDataOperator.Sql.SqlOperatorHandler.Active == null)
            {
                // Send error message.
                UniformServer.BaseServer.SendAnswerViaPP(
                    new Query("ERROR: " +
                    "SqlDataOperator not asigned. " +
                    "Connection to database not possible. " +
                    "Contact administrator or support service."),
                    entryQuery);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Validate token rights
        /// </summary>
        /// <param name="entryQuery">Query received by server.</param>
        /// <param name="requiredRights">Rights required to query processing.</param>
        /// <returns>Is rights enought?</returns>
        protected bool ValidateTokenRights(Query entryQuery, params string[] requiredRights)
        {
            entryQuery.TryGetParamValue("token", out QueryPart token);

            // Check if the user admin or superadmin.
            if (!AuthorityController.API.Tokens.IsHasEnoughRigths(
                token.PropertyValueString,
                out AuthorityController.Data.Temporal.TokenInfo _,
                out string error,
                requiredRights))
            {
                // Log error.
                UniformServer.BaseServer.SendAnswerViaPP(
                    "ERROR: Unauthorized. Details:" + error,
                    entryQuery);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Trying to add meta for pair `sqlSeed`--`query`
        /// </summary>
        /// <param name="sqlSeed">Object used as seed to builing sql query.</param>
        /// <param name="query">Query that will connected to meta data.</param>
        /// <returns>Created meta data.</returns>
        protected QueryMeta RegisterateSQLQuery(object sqlSeed, Query query)
        {
            try
            {
                QueryMeta meta = new QueryMeta()
                {
                    entryQuery = query
                };
                metaTable.Add(sqlSeed, meta);
                return meta;
            }
            catch//(Exception ex)
            {
                // Commented because not all queries has implemented SQLQueryHandler, 
                // so here will no registration of that query. That will cause a spam messages into console.
                //Console.WriteLine("ERROR: SqlQueryHandler | Query not registred." +
                //    "Details: " + ex.Message);

                return null;
            }
        }

        /// <summary>
        /// Trying to unregisterate hashed SQL query's meta data.
        /// </summary>
        /// <param name="sqlSeed">Object used as seed to builing sql query.</param>
        protected void UnregistrateSQLQuery(object sqlSeed)
        {
            try
            {
                metaTable.Remove(sqlSeed);
            }
            catch
            {
                Console.WriteLine("ERROR: SqlQueryHandler | Query registration not found.");
            }
        }

        /// <summary>
        /// Waiting till answer from sql server will received.
        /// </summary>
        /// <param name="dataOperatorTask">Task of data operator that was started to your sql query.</param>
        protected void WaitAsyncSqlServerAnswer(Task dataOperatorTask)
        {
            // If async operation started.
            if (dataOperatorTask != null)
            {
                // Wait until finishing.
                while (!dataOperatorTask.IsCompleted && !dataOperatorTask.IsCanceled)
                {
                    Thread.Sleep(5);
                }
            }
        }

        /// <summary>
        /// Callback that will called if sql server will face error.
        /// </summary>
        /// <param name="sender">Object that was used as seed of sql query building.</param>
        /// <param name="message">Error message.</param>
        protected void ErrorListener(object sender, string message)
        {
            // Try to find meta by sender.
            if(metaTable[sender] is QueryMeta meta)
            {
                // Mark that data receiving failed.
                meta.sqlDataOperationFailed = true;

                // Inform that user not found.
                UniformServer.BaseServer.SendAnswerViaPP("ERROR SQL SERVER: " + message, meta.entryQuery);
            }
            else
            {
                // Log error:
                Console.WriteLine("ERROR: SqlQueryHandler | error callback failed." +
                    " Meta data for sender not found.");
            }
        }
    }
}
