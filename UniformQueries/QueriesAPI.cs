// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// Copyright (c) 2019 Volodymyr Podshyvalov

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;

namespace UniformQueries
{
    /// <summary>
    /// Class that provide methods for handling of queries.
    /// </summary>
    public static class API
    {
        /// <summary>
        /// Symbol that divide query to parameters array.
        /// </summary>
        public const char SPLITTING_SYMBOL = '&';

        /// <summary>
        /// List that contain references to all query's processors instances.
        /// </summary>
        public static List<IQueryHandlerProcessor> QueryProcessors
        {
            get
            {
                return queryProcessors;
            }
        }
        private static readonly List<IQueryHandlerProcessor> queryProcessors = null;
        
        
        /// <summary>
        /// Load query handlers during first call.
        /// </summary>
        static API()
        {
            queryProcessors = new List<IQueryHandlerProcessor>();

            // Load query's processors.
            System.Reflection.Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            //Console.WriteLine("ASSEMBLIES PROCEED: {0}\n", assemblies.Length);
            Console.WriteLine("\nDETECTED QUERIES:");
            foreach (System.Reflection.Assembly assembly in assemblies)
            {
                // Get all types for assembly.
                foreach (System.Type type in assembly.GetTypes())
                {
                    // Check if this type is subclass of query.
                    if (type.GetInterface("UniformQueries.IQueryHandlerProcessor") != null)
                    {
                        // Instiniating querie processor.
                        UniformQueries.IQueryHandlerProcessor instance = (UniformQueries.IQueryHandlerProcessor)Activator.CreateInstance(type);
                        queryProcessors.Add(instance);
                        Console.WriteLine("{0}", type.Name);
                    }
                }
            }

            // Log
            Console.WriteLine("\nRESUME:\nQueryMonitor established. Session started at {0}\nTotal query processors detected: {1}",
                DateTime.Now.ToString("HH:mm:ss"), queryProcessors.Count);
        }


        /// <summary>
        /// Handler that can be connected as callback to default PipesProvides DNS Handler.
        /// Will validate and decompose querie on parts and send it to target QueryProcessor.
        /// </summary>
        /// <param name="meta"></param>
        /// <param name="query"></param>
        public static async void PPReceivedQueryHandlerAsync(PipesProvider.ServerTransmissionMeta meta, string query)
        {
            // Detect query parts.
            QueryPart[] queryParts = API.DetectQueryParts(query);
            QueryPart token = QueryPart.None;

            // Check query format.
            bool queryFormatIsValid =
                QueryParamExist("q", queryParts)     &&
                QueryParamExist("guid", queryParts)  &&
                TryGetParamValue("token", out token, queryParts);

            // Ignore if requiest not valid.
            if (!queryFormatIsValid)
            {
                Console.WriteLine("INVALID QUERY. LOSED ONE OR MORE PARTS BY SCHEME:{0}\nExample:{1}",
                    "guid=GUID" + SPLITTING_SYMBOL + "token=TOKEN" + SPLITTING_SYMBOL + "q=QUERY",
                    "guid=sharedGUID" + SPLITTING_SYMBOL + "token=clientToken" + SPLITTING_SYMBOL +
                    "q=GET" + SPLITTING_SYMBOL + "sq=DAYSRANGE" + SPLITTING_SYMBOL + 
                    "f=07.11.2019" + SPLITTING_SYMBOL + "t=08.11.2019");
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
                        query, token.IsNone ? "token not found" : token.property);

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


        /// <summary>
        /// Check existing of param in query parts.
        /// Example entry query part: "q=Get", where target param is "q".
        /// </summary>
        /// <param name="param"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public static bool QueryParamExist(string param, string query)
        {
            return QueryParamExist(param, query.Split(SPLITTING_SYMBOL));
        }

        /// <summary>
        /// Check existing of param in query parts.
        /// Example entry query part: "q=Get", where target param is "q".
        /// </summary>
        /// <param name="param"></param>
        /// <param name="queryParts"></param>
        /// <returns></returns>
        public static bool QueryParamExist(string param, params string[] queryParts)
        {
            // Try to find target param
            foreach (string part in queryParts)
            {
                // If target param
                if (part.StartsWith(param + "=")) return true;
            }
            return false;
        }

        /// <summary>
        /// Check existing of param in query parts.
        /// </summary>
        /// <param name="param"></param>
        /// <param name="queryParts"></param>
        /// <returns></returns>
        public static bool QueryParamExist(string param, params QueryPart[] queryParts)
        {
            // Try to find target param
            foreach (QueryPart part in queryParts)
            {
                // If target param
                if (part.ParamKeyEqual(param))
                    return true;
            }
            return false;
        }


        /// <summary>
        /// Try to find requested param's value in query. 
        /// </summary>
        /// <param name="param"></param>
        /// <param name="value"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public static bool TryGetParamValue(string param, out string value, string query)
        {
            return TryGetParamValue(param, out value, query.Split(SPLITTING_SYMBOL));
        }

        /// <summary>
        /// Try to find requested param's value among query parts. 
        /// </summary>
        /// <param name="param"></param>
        /// <param name="value"></param>
        /// <param name="queryParts"></param>
        /// <returns></returns>
        public static bool TryGetParamValue(string param, out string value, params string[] queryParts)
        {
            // Try to find target param
            foreach (string part in queryParts)
            {
                // If target param
                if (part.StartsWith(param + "="))
                {
                    // Get value.
                    value = part.Substring(param.Length + 1);
                    // Mark as success.
                    return true;
                }
            }

            // Inform that param not found.
            value = null;
            return false;
        }

        /// <summary>
        /// Try to find requested param's value among query parts. 
        /// </summary>
        /// <param name="param"></param>
        /// <param name="value"></param>
        /// <param name="queryParts"></param>
        /// <returns></returns>
        public static bool TryGetParamValue(string param, out QueryPart value, params QueryPart[] queryParts)
        {
            // Try to find target param
            foreach (QueryPart part in queryParts)
            {
                // If target param
                if (part.ParamKeyEqual(param))
                {
                    // Get value.
                    value = part;
                    // Mark as success.
                    return true;
                }
            }

            // Inform that param not found.
            value = QueryPart.None;
            return false;
        }


        /// <summary>
        /// Try to find requested all param's value among query parts by requested param name. 
        /// </summary>
        /// <param name="param"></param>
        /// <param name="value"></param>
        /// <param name="queryParts"></param>
        /// <returns></returns>
        public static List<string> GetParamValues(string param, params string[] queryParts)
        {
            List<string> value = new List<string>();
            // Try to find target param
            foreach (string part in queryParts)
            {
                // If target param
                if (part.StartsWith(param + "="))
                {
                    // Get value.
                    value.Add(part.Substring(param.Length + 1));
                }
            }
            return value;
        }

        /// <summary>
        /// Try to find requested all param's value among query parts by requested param name. 
        /// </summary>
        /// <param name="param"></param>
        /// <param name="value"></param>
        /// <param name="queryParts"></param>
        /// <returns></returns>
        public static List<QueryPart> GetParamValues(string param, params QueryPart[] queryParts)
        {
            List<QueryPart> value = new List<QueryPart>();
            // Try to find target param
            foreach (QueryPart part in queryParts)
            {
                // If target param
                if (part.ParamKeyEqual(param))
                {
                    // Get value.
                    value.Add(part);
                }
            }
            return value;
        }


        /// <summary>
        /// Build query string with requested parts and core data.
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="token"></param>
        /// <param name="queryParams"></param>
        /// <returns></returns>
        public static string MakeQuery(string guid, string token, params QueryPart[] queryParams)
        {
            string query = "";

            // Build core data.
            query += new QueryPart("guid", guid);
            query += SPLITTING_SYMBOL;
            query += new QueryPart("token", token);

            // Add parts.
            foreach(QueryPart part in queryParams)
                query += SPLITTING_SYMBOL + part;

            return query;
        }

        /// <summary>
        /// Convert query's string to array of query parts.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public static QueryPart[] DetectQueryParts(string query)
        {
            // Get query parts in string format.
            string[] splitedQuery = query.Split(SPLITTING_SYMBOL);

            // Init list.
            QueryPart[] parts = new QueryPart[splitedQuery.Length];

            // Add parts to array. Will auto converted from string to QueryPart.
            for (int i = 0; i < splitedQuery.Length; i++)
            {
                parts[i] = (QueryPart)splitedQuery[i];
            }

            return parts;
        }
    }
}