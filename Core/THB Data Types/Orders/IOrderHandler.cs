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

namespace DatumPoint.Types.Orders
{
    /// <summary>
    /// Interface that can be implemented by order's handler that would process data received from database.
    /// </summary>
    public interface IOrderHandler
    {
        /// <summary>
        /// Checking does this handler suitable to processign of order.
        /// </summary>
        /// <param name="order">Target order.</param>
        /// <returns></returns>
        bool IsTarget(Order order);

        /// <summary>
        /// Execute order via this handler.
        /// </summary>
        /// <param name="order"></param>
        void Execute(Order order);
    }
}
