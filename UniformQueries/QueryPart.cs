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
using System.Text;

namespace UniformQueries
{
    public struct QueryPart
    {
        /// <summary>
        /// Key for access
        /// </summary>
        public string key;

        /// <summary>
        /// Property that will be shared via query.
        /// </summary>
        public string property;

        /// <summary>
        /// If this struct not initialized.
        /// </summary>
        public bool IsNone
        {
            get
            {
                if (string.IsNullOrEmpty(key))
                    return true;

                if (string.IsNullOrEmpty(property))
                    return true;

                return false;
            }
        }

        public static QueryPart None
        {
            get { return new QueryPart(); }
        }

        /// <summary>
        /// Base constructor.
        /// </summary>
        /// <param name="key">String key that allow to find this part in query.</param>
        /// <param name="property">String property that will be available to  find by key.</param>
        public QueryPart(string key, string property)
        {
            this.key = key;
            this.property = property;
        }

        /// <summary>
        /// Return part in query format.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return key + "=" + property;
        }

        /// <summary>
        /// Convert QueryPart to string.
        /// </summary>
        /// <param name="qp"></param>
        public static implicit operator string(QueryPart qp)
        {
            return qp.key + "=" + qp.property;
        }

        /// <summary>
        /// Convert string to Qury Part.
        /// </summary>
        /// <param name="buildedPart"></param>
        public static explicit operator QueryPart(string buildedPart)
        {
            // Drop invalid request.
            if (string.IsNullOrEmpty(buildedPart))
            {
                Console.WriteLine("QueryPart converting error: provided part is null or empty");
                return new QueryPart();
            }

            // Split string by spliter.
            string[] parts = buildedPart.Split('=');

            // If splided as require.
            if (parts.Length == 2)
            {
                return new QueryPart(parts[0], parts[1]);
            }
            else
            {
                Console.WriteLine("QueryPart converting error: Need string format key=property | Requested: {0}", buildedPart);
                return new QueryPart("error", buildedPart);
            }
        }

        /// <summary>
        /// Convert array to query string.
        /// </summary>
        /// <param name="queryParts"></param>
        /// <returns></returns>
        public static string QueryPartsArrayToString(QueryPart[] queryParts)
        {
            string query = "";

            // Processing of every part.
            for (int i = 0; i < queryParts.Length; i++)
            {
                // Add query part.
                query += queryParts[i];

                // Add splitter.
                if (i < queryParts.Length - 1)
                    query += API.SPLITTING_SYMBOL;
            }

            return query;
        }
        
        /// <summary>
        /// Check does this query's key equals to target.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public bool ParamKeyEqual(string key)
        {
            return this.key.Equals(key, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Check does this query's parameter equals to target.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public bool ParamEqual(string param)
        {
            return this.property.Equals(param, StringComparison.OrdinalIgnoreCase);
        }       

        /// <summary>
        /// Try to get domain to backward connection by entry query.
        /// </summary>
        /// <param name="queryParts">Query that was reciverd from client.</param>
        /// <param name="domain">Domain that will return in case if build is possible.</param>
        /// <returns></returns>
        public static bool TryGetBackwardDomain(QueryPart[] queryParts, out string domain)
        {
            domain = null;

            // Get query GUID.
            if (!API.TryGetParamValue("guid", out QueryPart guid, queryParts)) return false;

            // Get client token.
            if (!API.TryGetParamValue("token", out QueryPart token, queryParts)) return false;

            // Build domain.
            domain = guid.property.GetHashCode() + "_" + token.property.GetHashCode();

            return true;
        }
    }
}
