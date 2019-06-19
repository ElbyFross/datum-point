﻿// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// Copyright (c) 2019 Volodymyr Podshyvalov

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniformQueries
{
    /// <summary>
    /// All classes that implements this interface 
    /// will automaticly detected by QueriesAPI via first use and connected to queries processing.
    /// </summary>
    public interface IQueryHandlerProcessor
    {
        /// <summary>
        /// Methods that process query.
        /// </summary>
        /// <param name="queryParts"></param>
        void Execute(QueryPart[] queryParts);

        /// <summary>
        /// Check by the entry params does it target Query Handler.
        /// </summary>
        /// <param name="queryParts"></param>
        /// <returns></returns>
        bool IsTarget(QueryPart[] queryParts);

        /// <summary>
        /// Return the description relative to the lenguage code or default if not found.
        /// </summary>
        /// <param name="cultureKey"></param>
        /// <returns></returns>
        string Description(string cultureKey);
    }
}
