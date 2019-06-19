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
using UniformQueries;

namespace THB_Data_Server_Queries
{
    public class GET_DAYS_RANGE : IQueryHandlerProcessor
    {
        public string Description(string cultureKey)
        {
            switch(cultureKey)
            {
                case "en-US":
                default:
                    return "GET DAYSRANGE DD:MM:YYYY DD:MM:YYYY\n" +
                            "\tDESCRIPTION: Will return List of days with shedule reletive to orders.\n" +
                            "\tQUERY FORMAT: q=GET" + API.SPLITTING_SYMBOL + 
                            "sq=DAYSRANGE" + API.SPLITTING_SYMBOL + 
                            "f=DD:MM:YYYY" + API.SPLITTING_SYMBOL + 
                            "t=DD:MM:YYYY\n";
            }
        }

        public void Execute(QueryPart[] queryParts)
        {
            throw new NotImplementedException();
        }

        public bool IsTarget(QueryPart[] queryParts)
        {
            if (API.TryGetParamValue("q", out QueryPart query, queryParts)) return false;

            // Save the time and avoid if query even not hace subquery.
            if (!API.TryGetParamValue("sq", out QueryPart subQuery, queryParts)) return false;

            // Comare q and sq with target.
            bool comparison =
                query.ParamKeyEqual("GET") &&
                query.ParamKeyEqual("DAYSRANGE");

            // Return result of compression.
            return comparison;
        }
    }
}
