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

namespace TeacherHandbook.Core.Containers.Schedule
{
    /// <summary>
    /// Sigle data that contain shedule.
    /// </summary>
    [Serializable]
    public class Day
    {
        /// <summary>
        /// Unitye key of this day in logbook.
        /// format dd.mm.yyyy
        /// </summary>
        public string key = null;

        /// <summary>
        /// List of sessions during this day.
        /// </summary>
        public Session[] sessions = null;
    }
}
