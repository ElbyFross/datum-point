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

namespace DatumPoint.Types.Schedule
{
    /// <summary>
    /// Object that describe day's time block as part of schedule.
    /// </summary>
    [System.Serializable]
    public class Day
    {
        /// <summary>
        /// Date of this day.
        /// </summary>
        public DateTime date;

        /// <summary>
        /// When will has been starting the firts lesson.
        /// Require to back log of shedule, cause start time can'be changed by orders.
        /// </summary>
        public DateTime startTime;

        /// <summary>
        /// How many minuts take one lesson.
        /// </summary>
        public int lessonDuration;

        /// <summary>
        /// Array that contain lessons binded to this day.
        /// Index equal the row nomber of lesson at that day.
        /// </summary>
        public Lesson[] lessonIds;
    }
}
