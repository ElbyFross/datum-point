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
using UniformQueries.Executable;
using DatumPoint.Types.Personality;

namespace DatumPoint.Queries.Statistic
{
    /// <summary>
    /// Set assessment to the certain user.
    /// </summary>
    public class USER_SET_ASSESSMENT : Handlers.SQLQueryHandler
    {
        public override UserRank RankUperThen { get; set; } = UserRank.Guest;
        public override string SharedObjectProperty { get; set; } = "set";
        public override Type TableType { get; set; } = typeof(Types.Personality.Gender);
        public override string[] RequiredRights { get; set; } = null;

        public override string Description(string cultureKey)
        {
            return "USER=[binary] ASSESSMENT SET=[binary]\n" +
                "\tDESCRIPTION:" +
                "Set new or update existed assesment to certein user.\n" +
                "\tQUERY FORMAT: gender property must contain binary serialized " +
                "`" + TableType.FullName + "` object that will applied to the database ot local storage.\n" +
                "\tREQUIRMENTS: User must has at least `admin` right.";
        }

        public override void Execute(object sender, Query query)
        {
            // Check rank rights.
            if (RankUperThen != UserRank.None)
            {
                // Is has enought ritghts.
                if (!ValidateTokenRights(query, ">rank=" + RankUperThen.ToString())) return;
            }

            // Check additive rights.
            if (RequiredRights == null)
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
                    "`" + TableType.FullName + "`"), query);
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

        public override bool IsTarget(Query query)
        {
            if (!query.QueryParamExist("assessment")) return false;
            if (!query.QueryParamExist("user")) return false;
            if (!query.QueryParamExist("set")) return false;
            return true;
        }
    }
}
