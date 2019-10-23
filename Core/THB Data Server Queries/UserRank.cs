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

namespace DatumPoint.Queries
{
    /// <summary>
    /// Rank of user rights.
    /// </summary>
    public enum UserRank
    {
        /// <summary>
        /// Not required.
        /// </summary>
        None = -1,
        /// <summary>
        /// Require guest rights.
        /// </summary>
        Guest = 0,
        /// <summary>
        /// Require user rights.
        /// </summary>
        User = 1,
        /// <summary>
        /// Require privileged user rights.
        /// </summary>
        PrivilegedUser = 2,
        /// <summary>
        /// Require moderator rights.
        /// </summary>
        Moderator = 4,
        /// <summary>
        /// Require admin rights.
        /// </summary>
        Admin = 8,
        /// <summary>
        /// Require super admin rights.
        /// </summary>
        SuperAdmin = 16
    }
}
