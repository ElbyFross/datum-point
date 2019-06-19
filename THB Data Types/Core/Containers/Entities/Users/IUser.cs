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

namespace TeacherHandbook.Core.Containers.Entities
{
    public interface IUser
    {
        /// <summary>
        /// Rights that give an access to features or restrict it.
        /// </summary>
        TeacherHandbook.Core.Enums.UserRights Rigts { get; }

        /// <summary>
        /// Hashtable that contain names relative to languages.
        /// Key (string) -> language key
        /// Valuse (NameContainer) -> localized data.
        /// </summary>
        System.Collections.Hashtable Name { get; set; }
    }
}
