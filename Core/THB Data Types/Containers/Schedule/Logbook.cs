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
namespace DatumPoint.Types.Containers.Schedule
{
    /// <summary>
    /// Container that contain long term data about sessions.
    /// </summary>
    [Serializable]
    public class Logbook
    {
        /// <summary>
        /// Hashtable that contain all recorded days.
        /// Key(string) : day key in format dd.mm.yyyy
        /// Value(Day): day data
        /// </summary>
        protected System.Collections.Hashtable recordedDays = new System.Collections.Hashtable();

        /// <summary>
        /// List of pooled orders.
        /// </summary>
        protected List<DatumPoint.Types.Orders.IOrder> orders = new List<Orders.IOrder>();
    }
}
