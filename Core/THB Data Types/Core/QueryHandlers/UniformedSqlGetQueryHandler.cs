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
using System.Text;
using UniformQueries;
using DatumPoint.Types.Personality;

namespace DatumPoint.Queries.Handlers
{
    public abstract class UniformedSqlGetQueryHandler : SQLQueryHandler
    {
        /// <summary>
        /// Delegate that could be used to shring data.
        /// </summary>
        /// <param name="sharedData">Object in processing.</param>
        /// <param name="meta">Meta data stored for target query.</param>
        /// <returns>Array of strings that will be used during query.</returns>
        public delegate string[] ObjectHander(object sharedData, QueryMeta meta);

        /// <summary>
        /// Delegate that must return array of target 
        /// SQL properties that will used in WHERE block.
        /// </summary>
        public virtual ObjectHander Where { get; } = null;

        /// <summary>
        /// Delegate that must return array of target 
        /// SQL properties that will used in SELECT block.
        /// </summary>
        public virtual ObjectHander Select { get; } = null;

        /// <summary>
        /// Delegate that will has calling before send success answer to client.
        /// Use to modify data.
        /// 
        /// object - Received data.
        /// QueryMeta - Meta that cotains information about query,
        /// </summary>
        public virtual Action<object, QueryMeta> PreComplete { get; } = null;

        public override string Description(string cultureKey)
        {
            throw new NotImplementedException();
        }

        public override void Execute(object sender, Query query)
        {
            Operate(query);
        }

        public override bool IsTarget(Query query)
        {
            return false;
        }

        /// <summary>
        /// Operate query data to get data from SQL server.
        /// Validate rights and configuration.
        /// </summary>
        /// <param name="query">Query received by server.</param>
        protected void Operate(Query query)
        {
            // Check rank rights.
            if (RankUperThen != UserRank.None)
            {
                // Is has enought ritghts.
                if (!ValidateTokenRights(query, ">rank=" + (int)RankUperThen)) return;
            }

            // Check additive rights.
            if (RequiredRights != null)
            {
                if (!ValidateTokenRights(query, RequiredRights)) return;
            }

            #region Get params.
            query.TryGetParamValue(SharedObjectProperty, out QueryPart binaryData);
            #endregion

            // Object that will contains deserizlised instance of object setting shared via 
            // the query in binary format.
            object bufer;
            try
            {
                // Try tp deserizlise gender object from binary.
                bufer = UniformDataOperator.Binary.BinaryHandler.FromByteArray(binaryData.propertyValue);
            }
            catch (Exception ex)
            {
                // Send error message in case of data damage.
                UniformServer.BaseServer.SendAnswerViaPP(
                    new Query("ERROR: Binary data demaged. Details: " + ex.Message),
                    query);
                return;
            }

            // Validate SQL operator settings.
            if (!ValidateSqlSettings(query)) return;

            // Registrate sql processing.
            var meta = RegisterateSQLQuery(bufer, query);

            // Receive props lists.
            var selectProps = Select?.Invoke(bufer, meta);
            var whereProps = Where?.Invoke(bufer, meta);

            // Look for gender in db by id if defined. Look by title in other case.
            WaitAsyncSqlServerAnswer(
                UniformDataOperator.Sql.SqlOperatorHandler.Active.SetToObjectAsync(
                    TableType,
                    System.Threading.CancellationToken.None,
                    bufer,
                    selectProps ?? new string[0],
                    whereProps ?? new string[0])
                );

            // Drop metadata.
            UnregistrateSQLQuery(meta);

            // Drop if oepration failed.
            if (meta.sqlDataOperationFailed)
            {
                // Log error.
                UniformServer.BaseServer.SendAnswerViaPP
                    (new Query(new QueryPart("error 401", "Data not found.")),
                    query);
                return;
            }

            // Modify data if required.
            PreComplete?.Invoke(bufer, meta);

            // Send received gender data to client.
            UniformServer.BaseServer.SendAnswerViaPP
                (new Query(new QueryPart("data", bufer)),
                query);
        }
    }
}
