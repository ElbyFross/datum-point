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

namespace TeacherHandbook.Core.Orders
{
    /// <summary>
    /// Interface that devlare the base properties fo orders.
    /// </summary>
    public interface IOrder
    {
        /// <summary>
        /// Date and session whe order will started.
        /// </summary>
        OrderDate StartDate { get; set; }

        /// <summary>
        /// Date and session when order will expire inpact.
        /// </summary>
        OrderDate ExpireDate { get; set; }

        /// <summary>
        /// User that place this order.
        /// </summary>
        string OrderPublisher { get; }

        /// <summary>
        /// When this order was pooled to stack.
        /// </summary>
        DateTime PooledTime { get; }
    }
}