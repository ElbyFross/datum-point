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
using System.Collections;

namespace TeacherHandbook.Core.Containers.Schedule
{
    /// <summary>
    /// Object that describe subject data.
    /// </summary>
    [Serializable]
    public class Subject
    {
        /// <summary>
        /// Unique key of this subject.
        /// </summary>
        public string key = null;

        /// <summary>
        /// Hashtable that contain titles relative to the localization.
        /// Key(string): Language key.
        /// Value (List<string>): List of titles that describe this subject. (Example: Math, Mathematic, H.Math, etc.)
        /// </summary>
        public Hashtable titles = new Hashtable();
    }
}
