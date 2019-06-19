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

namespace TeacherHandbook.Core.Localization
{
    /// <summary>
    /// Binary container that contain hash table with keys
    /// </summary>
    [Serializable]
    public class LanguageContaier
    {
        #region Public fields
        /// <summary>
        /// Unique key of this language.
        /// </summary>
        public string key = null;

        /// <summary>
        /// Title that will be displayed in settings form.
        /// </summary>
        public string title = "New language";

        /// <summary>
        /// Hash table that contain pairs string->string, where key is unique value for access to translation.
        /// </summary>
        public System.Collections.Hashtable map = new System.Collections.Hashtable();
        #endregion
    }
}
