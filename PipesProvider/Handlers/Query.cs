﻿//Copyright 2019 Volodymyr Podshyvalov
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
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.IO.Pipes;
using UniformQueries;
using UQAPI = UniformQueries.API;

namespace PipesProvider.Handlers
{
    public static class Query
    {
        /// <summary>
        /// Handler that can be connected as callback to default PipesProvides DNS Handler.
        /// Will validate and decompose querie on parts and send it to target QueryProcessor.
        /// </summary>
        /// <param name="meta"></param>
        /// <param name="query"></param>
        public static async void ProcessingAsync(PipesProvider.Server.ServerTransmissionController _, string query)
        {
            // Detect query parts.
            QueryPart[] queryParts = UQAPI.DetectQueryParts(query);
            QueryPart token = QueryPart.None;

            // Check query format.
            bool queryFormatIsValid =
                UQAPI.QueryParamExist("q", queryParts) &&
                UQAPI.QueryParamExist("guid", queryParts) &&
                UQAPI.TryGetParamValue("token", out token, queryParts);

            // Ignore if requiest not valid.
            if (!queryFormatIsValid)
            {
                Console.WriteLine("INVALID QUERY. LOSED ONE OR MORE PARTS BY SCHEME:{0}\nExample:{1}",
                    "guid=GUID" + UQAPI.SPLITTING_SYMBOL + "token=TOKEN" + UQAPI.SPLITTING_SYMBOL + "q=QUERY",
                    "guid=sharedGUID" + UQAPI.SPLITTING_SYMBOL + "token=clientToken" + UQAPI.SPLITTING_SYMBOL +
                    "q=GET" + UQAPI.SPLITTING_SYMBOL + "sq=DAYSRANGE" + UQAPI.SPLITTING_SYMBOL +
                    "f=07.11.2019" + UQAPI.SPLITTING_SYMBOL + "t=08.11.2019");
                return;
            }

            // Try to detect target query processor.
            bool processorFound = false;
            foreach (UniformQueries.IQueryHandlerProcessor processor in UniformQueries.API.QueryProcessors)
            {
                // Check header
                if (processor.IsTarget(queryParts))
                {
                    // Log.
                    Console.WriteLine("Start execution: [{0}]\n for token: [{1}]",
                        query, token.IsNone ? "token not found" : token.propertyValue);

                    // Execute query as async.
                    await Task.Run(() => processor.Execute(queryParts));

                    // Mark detection as succeed.
                    processorFound = true;

                    // Leave search.
                    break;
                }
            }

            // If not found.
            if (!processorFound)
            {
                Console.WriteLine("POST ERROR: Token: {1} | Handler for query \"{0}\" not implemented.", query, token);
            }
        }
    }
}