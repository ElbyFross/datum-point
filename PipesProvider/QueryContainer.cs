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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipesProvider
{
    /// <summary>
    /// Privide unvormation about query.
    /// </summary>
    public struct QueryContainer
    {
        /// <summary>
        /// Query that will be shared.
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// Delegate that will be called when anser transmition will be recived.
        /// </summary>
        public System.Action<TransmissionLine, string> AnswerHandler;

        /// <summary>
        /// Return empty contaier.
        /// </summary>
        public static QueryContainer Empty { get; } = new QueryContainer();

        public QueryContainer(string query, System.Action<TransmissionLine, string> AnswerHandler)
        {
            this.Query = query;
            this.AnswerHandler = AnswerHandler;
            this.Attempts = 0;
        }

        /// <summary>
        /// Validate container.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                if (string.IsNullOrEmpty(Query))
                    return true;
                return false;
            }
        }

        /// <summary>
        /// Return copy of source container.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static QueryContainer Copy(QueryContainer source)
        {
            return new QueryContainer(
                string.Copy(source.Query),
                source.AnswerHandler != null ? source.AnswerHandler.Clone() as System.Action<TransmissionLine, string> : null);

        }

        /// <summary>
        /// How many attemts was applied to this query processing.
        /// </summary>
        public int Attempts { get; private set; }

        /// <summary>
        /// Incremet of attempts count.
        /// </summary>
        /// <param name="contaier"></param>
        /// <returns></returns>
        public static QueryContainer operator ++(QueryContainer contaier)
        {
            contaier.Attempts++;
            return contaier;
        }

        public static explicit operator string(QueryContainer container)
        {
            return container.Query;
        }

        public override string ToString()
        {
            return Query;
        }
    }
}
