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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniformDataOperator
{
    /// <summary>
    /// Interface that provide uniform API to work with objects.
    /// </summary>
    public interface IUniformDataOperator
    {
        /// <summary>
        /// Active single tone instance of MySQL data provider.
        /// </summary>
        IUniformDataOperator Active { get; }

        /// <summary>
        /// Checing is the storage exist.
        /// </summary>
        /// <param name="args">Shared params that provide posssibility determine target data in storage.
        /// Can has a different format in every implementation. use manuals.</param>
        /// <returns></returns>
        bool IsStorageExist(params object[] args);

        /// <summary>
        /// Create new or update already existed data in the storage.
        /// </summary>
        /// <param name="args">Shared params that provide posssibility determine target data in storage.
        /// Can has a different format in every implementation. use manuals.</param>
        /// <returns></returns>
        bool SetData(params object[] args);

        /// <summary>
        /// Returning data by requested params.
        /// </summary>
        /// <param name="args">Shared params that provide posssibility determine target data in storage.
        /// Can has a different format in every implementation. use manuals.</param>
        /// <returns></returns>
        object GetData(params object[] args);

        /// <summary>
        /// Deliting data from storage by requested params.
        /// </summary>
        /// <param name="args">Shared params that provide posssibility determine target data in storage.
        /// Can has a different format in every implementation. use manuals.</param>
        /// <returns></returns>
        bool DeleteData(params object[] args);
    }
}
