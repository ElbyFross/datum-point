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
    /// <summary>
    /// Class that provides uniform vai to set data to table basing of simple uniformed format.
    /// </summary>
    public abstract class UniformedSqlSetQueryHandler : SQLQueryHandler
    {
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
        /// Operate query data to set it to SQL server.
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
            if(RequiredRights != null)
            {
                if (!ValidateTokenRights(query, RequiredRights)) return;
            }

            #region Input data
            // Getting shared gender data.
            query.TryGetParamValue(SharedObjectProperty, out QueryPart property);

            // Bufer that would contain shared data.
            object bufer;
            try
            {
                // Trying to get Gender data from binary format.
                bufer = UniformDataOperator.Binary.BinaryHandler.FromByteArray
                    (property.propertyValue);
            }
            catch
            {
                // Log error
                UniformServer.BaseServer.SendAnswerViaPP(
                    new Query(SharedObjectProperty + " value can't be casted to " +
                    "`"+ TableType.FullName + "`"), query);
                return;
            }
            #endregion

            #region Store data
            // Validate SQL operator settings.
            if (!ValidateSqlSettings(query)) return;

            // Registrate sql processing.
            var meta = RegisterateSQLQuery(bufer, query);

            // Write data to server.
            WaitAsyncSqlServerAnswer(
                UniformDataOperator.Sql.SqlOperatorHandler.Active.SetToTableAsync(
                    TableType,
                    System.Threading.CancellationToken.None,
                    bufer)
                );

            // Drop metadata.
            UnregistrateSQLQuery(meta);

            // Drop if oepratio nfailed.
            if (!meta.sqlDataOperationFailed) return;

            // Send received gender data to client.
            UniformServer.BaseServer.SendAnswerViaPP
                (new Query(new QueryPart("success", bufer)),
                query);
            #endregion
        }
    }
}
