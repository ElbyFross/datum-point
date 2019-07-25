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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;

namespace TeacherHandbook.Networking
{
    /// <summary>
    /// Instance of this class will provide client features.
    /// </summary>
    public class Client : UniformClient.BaseClient
    {
        /// <summary>
        /// Reference to current client provider.
        /// Always return not null result.
        /// </summary>
        public static Client Active
        {
            get
            {
                if (active == null)
                    active = new Client();
                return active;
            }

            protected set { active = value; }
        }

        protected static Client active;

        public Client()
        {
            // Set as active.
            Active = this;

            // Loading assemblies from lib direcroty.
            LoadAssemblies(AppDomain.CurrentDomain.BaseDirectory + "libs\\");
            
            // Loading assemblies from plugins direcroty.
            LoadAssemblies(AppDomain.CurrentDomain.BaseDirectory + "plugins\\");

            // Load translation for plugins relative to thread culture.
            WpfHandler.Localization.API.LoadXAML_LangDicts(CultureInfo.CurrentCulture, new CultureInfo("en-US"));
        }

        /// <summary>
        /// Make first start of client.
        /// </summary>
        public static void Init()
        {
            _ = Active;
        }
    }
}