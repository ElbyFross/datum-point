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
    [Serializable]
    public struct OrderDate
    {
        /// <summary>
        /// Date of order releasing.
        /// </summary>
        public DateTime date;

        /// <summary>
        /// Nuber of target session like a part of entire day.
        /// </summary>
        public int session;

        public OrderDate(DateTime date)
        {
            this.date = date;
            session = 1;
        }

        public OrderDate(int sesstion)
        {
            this.date = DateTime.Now;
            this.session = sesstion;
        }

        public OrderDate(DateTime date, int session)
        {
            this.date = date;
            this.session = session;
        }
    }
}
