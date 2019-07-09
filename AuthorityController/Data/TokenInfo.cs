﻿//Copyright 2019 Volodymyr Podshyvalov
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
    /// Container that contain data about token.
    /// </summary>
    [System.Serializable]
    public struct TokenInfo
    {
        /// <summary>
        /// Return empty info.
        /// </summary>
        public static TokenInfo Anonymous
        {
            get { return new TokenInfo() { userId = -1 }; }
        }

        /// <summary>
        /// Token string.
        /// </summary>
        public string token;

        /// <summary>
        /// Operation system that request this token.
        /// Using for machine stamp.
        /// </summary>
        public string operationSystem;

        /// <summary>
        /// Mac adress of machine that request access.
        /// Using for machine stamp.
        /// </summary>
        public string machineMac;

        /// <summary>
        /// Id of user that recive this token.
        /// </summary>
        public int userId;

        /// <summary>
        /// Time when token was allocated in system.
        /// </summary>
        public DateTime allocationTime;

        /// <summary>
        /// Array that contain rights provided to this token.
        /// </summary>
        public string[] rights;
    }
}