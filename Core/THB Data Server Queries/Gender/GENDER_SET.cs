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

namespace DatumPoint.Queries.Gender
{
    /// <summary>
    /// Set new or update already existed gender settings.
    /// </summary>
    class GENDER_SET : IQueryHandler
    {
        public string Description(string cultureKey)
        {
            return "SET GENDER=[binary]\n" +
                "\tDESCRIPTION:" +
                "Set new or update existed gender settings.\n" +
                "\tQUERY FORMAT: gender property must contain binary serialized " +
                "`DatumPoint.Types.Personality.Gender` object that will applied to the database ot local storage.\n" +
                "\tREQUIRMENTS: User must has `envConfig` right.";
        }

        public void Execute(object sender, Query query)
        {
            #region Validate rights
            #endregion

            #region Input data
            // Getting shared gender data.
            query.TryGetParamValue("gender", out QueryPart genderBinary);

            // Bufer that would contain shared gender data.
            Types.Personality.Gender gender = null;

            try
            {
                // Trying to get Gender data from binary format.
                gender = UniformDataOperator.Binary.BinaryHandler.FromByteArray
                    <Types.Personality.Gender>(genderBinary.propertyValue);
            }
            catch
            {
                // Log error
                UniformServer.BaseServer.SendAnswerViaPP(
                    new Query("GENDER value can't be casted to " +
                    "`DatumPoint.Types.Personality.Gender`"), query);
                return;
            }
            #endregion

            #region Store data
            // SQL server connected.
            if (UniformDataOperator.Sql.SqlOperatorHandler.Active != null)
            {

            }
            // Store in local storage.
            else
            {

            }
            #endregion
        }

        public bool IsTarget(Query query)
        {
            if (!query.QueryParamExist("gender")) return false;
            if (!query.QueryParamExist("set")) return false;
            return true;
        }
    }
}
