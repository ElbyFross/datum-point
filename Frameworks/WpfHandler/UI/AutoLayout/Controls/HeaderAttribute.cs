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
using WpfHandler.UI.AutoLayout;

namespace WpfHandler.UI.AutoLayout.Controls
{
    /// <summary>
    /// Added header block element to UI.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class HeaderAttribute : GUIContentAttribute, IGUIElement
    {
        /// <summary>
        /// Auto initialize content with shared title value.
        /// </summary>
        /// <param name="title">Title that will be showed up into the label.</param>
        public HeaderAttribute(string title) : base(title) { }

        /// <summary>
        /// Constructor that allow to set title.
        /// </summary>
        /// <param name="title">Title of that element.</param>
        /// <param name="description">Description of that element.</param>
        public HeaderAttribute(string title, string description) : base(title, description) { }

        /// <summary>
        /// Initialize all allowed fields.
        /// </summary>
        /// <param name="defaultTitle">Title that would be used by default if localization dictionary not found.</param>
        /// <param name="defaultDescription">Default description if localization dictionary not found.</param>
        /// <param name="decriptionLocalizationResourseKey">Key of description content in localized dynamic dictionary.</param>
        public HeaderAttribute(
            string defaultTitle,
            string defaultDescription,
            string decriptionLocalizationResourseKey) :
            base(defaultTitle, defaultDescription, decriptionLocalizationResourseKey) { }

        /// <summary>
        /// Initialize all allowed fields.
        /// </summary>
        /// <param name="defaultTitle">Title that would be used by default if localization dictionary not found.</param>
        /// <param name="defaultDescription">Default description if localization dictionary not found.</param>
        /// <param name="titleLocalizationResourseKey">Key of title content in localized dynamic dictionary.</param>
        /// <param name="decriptionLocalizationResourseKey">Key of description content in localized dynamic dictionary.</param>
        public HeaderAttribute(
            string defaultTitle,
            string defaultDescription,
            string titleLocalizationResourseKey,
            string decriptionLocalizationResourseKey) :
            base(defaultTitle, defaultDescription, titleLocalizationResourseKey, decriptionLocalizationResourseKey) { }
        
        /// <summary>
        /// Spawning Header UI elements un shared layer. Connecting to the shared member.
        /// </summary>
        /// <param name="layer">Target UI layer.</param>
        /// <param name="args">Must contains: @UIDescriptor and @MemberInfo</param>
        public virtual void OnGUI(ref LayoutLayer layer, params object[] args)
        {
            // Instiniate header UI.
            var header = new UI.Controls.Header()
            {
                GUIContent = Content
            };

            // Call GUI processing.
            header.OnGUI(ref layer, args);
        }
        
        /// <summary>
        /// TODO: Callback that occurs when content dictionaries are reloaded.
        /// Updating header's content.
        /// </summary>
        public override void LanguagesDictionariesUpdated()
        {
            throw new NotImplementedException();
        }
    }
}
