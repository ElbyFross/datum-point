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
using System.IO;
using UniformQueries;

namespace UniformServer.Queries
{
    /// <summary>
    /// Query that request from server public encription key (RSA algorithm).
    /// </summary>
    class GET_PUBLIC_KEY : IQueryHandlerProcessor
    {
        public string Description(string cultureKey)
        {
            switch (cultureKey)
            {
                case "en-US":
                default:
                    return "GET PUBLICKEY\n" +
                            "\tDESCRIPTION: Will return public RSA key of this server," + 
                            "\n\tthat can be used to encrypt message before start transmission.\n" +
                            "\tQUERY FORMAT: q=GET" + API.SPLITTING_SYMBOL + "sq=PUBLICKEY\n";
            }
        }

        public void Execute(QueryPart[] queryParts)
        {
            // Look for token.
            API.TryGetParamValue("token", out QueryPart token, queryParts);

            // Create public key as answer.
            string answer = SecurityAPI.PublicKeyXML;

            // Open answer chanel on server and send message.
            BaseServer.SendAnswer(answer, queryParts);
        }

        public bool IsTarget(QueryPart[] queryParts)
        {
            // Get query header.
            if (!API.TryGetParamValue("q", out QueryPart query, queryParts))
                return false;

            // Get subquery.
            if (!API.TryGetParamValue("sq", out QueryPart subQuery, queryParts))
                return false;

            // Check query.
            if (!query.ParamEqual("GET"))
                return false;

            // Check sub query.
            if (!subQuery.ParamEqual("PUBLICKEY"))
                return false;

            return true;
        }
    }
}
