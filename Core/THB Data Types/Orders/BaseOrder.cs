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

namespace DatumPoint.Types.Orders
{
    public abstract class BaseOrder : IOrder
    {
        /// <summary>
        /// Date and session whe order will started.
        /// </summary>
        public OrderDate StartDate { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <summary>
        /// Date and session when order will expire inpact.
        /// </summary>
        public OrderDate ExpireDate { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <summary>
        /// User that place this order.
        /// </summary>
        public string OrderPublisher => throw new NotImplementedException();

        /// <summary>
        /// When this order was pooled to stack.
        /// </summary>
        public DateTime PooledTime => throw new NotImplementedException();
    }
}
