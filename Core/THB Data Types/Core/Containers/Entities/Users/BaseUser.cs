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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeacherHandbook.Core.Enums;

namespace TeacherHandbook.Core.Containers.Entities
{
    /// <summary>
    /// Base class implemented the API for users.
    /// </summary>
    [Serializable]
    public abstract class BaseUser : IUser
    {
        #region Public properties
        /// <summary>
        /// Rights provided to current user
        /// </summary>
        public UserRights Rigts { get { return rights; } }

        /// <summary>
        /// Hashtable that contain names relative to languages.
        /// Key (string) -> language key
        /// Value (NameContainer) -> localized data.
        /// </summary>
        public Hashtable Name { get { return name; } set { name = value; } }
        #endregion

        #region Protected fields
        /// <summary>
        /// Hashtable that contain names relative to languages.
        /// Key (string) -> language key
        /// Value (NameContainer) -> localized data.
        /// </summary>
        protected System.Collections.Hashtable name = new System.Collections.Hashtable();

        /// <summary>
        /// Rights provided to current user
        /// </summary>
        protected UserRights rights;
        #endregion
    }
}
