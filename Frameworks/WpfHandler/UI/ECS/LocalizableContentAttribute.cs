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

namespace WpfHandler.UI.ECS
{
    /// <summary>
    /// Base attribute that bind UI element to common auto localization system.
    /// </summary>
    public abstract class LocalizableContentAttribute : Attribute
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public LocalizableContentAttribute()
        {
            Dictionaries.API.LanguagesDictionariesUpdated += LanguagesDictionariesUpdated;
        }

        /// <summary>
        /// Unsubscribe from events.
        /// </summary>
        ~LocalizableContentAttribute()
        {
            Dictionaries.API.LanguagesDictionariesUpdated -= LanguagesDictionariesUpdated;
        }

        /// <summary>
        /// Occurs when would reloaded dynamic dictionaries.
        /// </summary>
        public abstract void LanguagesDictionariesUpdated();
    }
}
