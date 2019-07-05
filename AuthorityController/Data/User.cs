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

namespace AuthorityController.Data
{
    /// <summary>
    /// Object that contain relevant data about user.
    /// </summary>
    [System.Serializable]
    public class User
    {
        /// <summary>
        /// Unique id of this user to allow services access.
        /// </summary>
        public int id;

        /// <summary>
        /// Login of this user to access the system.
        /// </summary>
        public string login;

        /// <summary>
        /// Hashed and salted password that confirm user rights to use this account.
        /// </summary>
        public string hashedPassword;

        /// <summary>
        /// Name of the user that will displayed in profile.
        /// </summary>
        public string firstName;

        /// <summary>
        /// Secondary name that will be displayed in profile.
        /// </summary>
        public string secondName;

        // TODO Local configs like prefered language.
    }
}
