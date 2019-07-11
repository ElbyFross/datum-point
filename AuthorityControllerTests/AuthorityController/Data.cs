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
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AuthorityController.Tests
{
    [TestClass]
    public class Data
    {
        /// <summary>
        /// Create new config file.
        /// </summary>
        [TestMethod]
        public void ConfigInit()
        {
        }

        /// <summary>
        /// Try to load valid config data file.
        /// </summary>
        [TestMethod]
        public void ConfigLoad_ValidData()
        {
        }

        /// <summary>
        /// Trying to load corrupted config dile.
        /// </summary>
        [TestMethod]
        public void ConfigLoad_CoruptedData()
        {
        }

        /// <summary>
        /// Creating new user profile and saving to storage.
        /// </summary>
        [TestMethod]
        public void NewUser()
        {
        }
        
        /// <summary>
        /// Update already existed profile.
        /// </summary>
        [TestMethod]
        public void UpdateUser()
        {
        }

        /// <summary>
        /// Remove profile from storage.
        /// </summary>
        [TestMethod]
        public void RemoveUser()
        {
        }

        /// <summary>
        /// Try to load a huge pool of users. Some users can be corrupted.
        /// Need to finish operation without freezing of other threads and without crush due corupted data.
        /// </summary>
        [TestMethod]
        public void ThreadingLoadProfiles()
        {
        }

        /// <summary>
        /// Generate big pool of users ID and check that conut of registred equal the cound of requsets.
        /// If any user will losed - data corupted.
        /// </summary>
        [TestMethod]
        public void GenerateUserID()
        {
        }

        /// <summary>
        /// Creating a new salt data.
        /// </summary>
        [TestMethod]
        public void SaltInit()
        {
        }

        /// <summary>
        /// Validate loading of already created salt
        /// </summary>
        [TestMethod]
        public void SaltLoading()
        {
        }
    }
}
