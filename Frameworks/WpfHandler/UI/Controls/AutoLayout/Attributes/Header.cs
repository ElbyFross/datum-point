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

namespace WpfHandler.UI.Controls.AutoLayout.Attributes
{
    /// <summary>
    /// Added header block element to UI.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class Header : LayoutAttribute, Interfaces.IGUIElement
    {
        /// <summary>
        /// Content applied to that GUI element.
        /// </summary>
        public GUIContent Content { get; set; }

        /// <summary>
        /// Auto initialize content with shared title value.
        /// </summary>
        /// <param name="title">Title that will be showed up into the lable.</param>
        public Header(string title) : base()
        {
            Content = new GUIContent(title);
        }

        /// <summary>
        /// Initialize header with shared content.
        /// </summary>
        /// <param name="content">Content descriptor with configurated show up options.</param>
        public Header(GUIContent content) : base()
        {
            Content = content;
        }

        /// <summary>
        /// Spawning Header UI elements un shared layer. Connecting to the shared member.
        /// </summary>
        /// <param name="layer">Target UI layer.</param>
        /// <param name="args">Must contains: @UIDescriptor and @MemberInfo</param>
        public void OnGUI(ref LayoutLayer layer, params object[] args)
        {
            // Instiniate header UI.
            var header = new Controls.Header();

            // Call GUI processing.
            header.OnGUI(ref layer, args);
        }
    }
}
