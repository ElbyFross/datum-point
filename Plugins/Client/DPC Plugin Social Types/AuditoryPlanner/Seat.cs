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

namespace DatumPoint.Plugins.Social.Types.AuditoryPlanner
{
    /// <summary>
    /// Class that describe certain seatt in the auditory or on the schema.
    /// Provides API that allow to manage it.
    /// </summary>
    [Serializable]
    public class Seat
    {
        /// <summary>
        /// Describe the current state of seat that defines managment options.
        /// </summary>
        public enum State
        { 
            /// <summary>
            /// Uniform state that could be used in unspecified editors like shema buildor etc.
            /// </summary>
            Undefined,
            /// <summary>
            /// Seat can be reserved.
            /// </summary>
            Free,
            /// <summary>
            /// Seat reserved by someone.
            /// </summary>
            Reserved,
            /// <summary>
            /// Seat will be hidded from schema in normal mode. Will visible only during edit.
            /// </summary>
            Hidded,
            /// <summary>
            /// Seat will not access to reservation.
            /// </summary>
            Blocked
        }

        /// <summary>
        /// Current state of the place.
        /// </summary>
        public State state = State.Undefined;

        /// <summary>
        /// Index of the place in the auditorium.
        /// </summary>
        public int index = -1;
    }
}
