using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfHandler.UI.AutoLayout.Configuration
{
    public class ContentAttribute : GUIContentAttribute, IGUIElement
    {
        public UI.Controls.ILabel BindedLabel { get; protected set; }

        /// <summary>
        /// Auto initialize content with shared title value.
        /// </summary>
        /// <param name="title">Title that will be showed up into the label.</param>
        public ContentAttribute(string title) : base(title) { }

        /// <summary>
        /// Constructor that allow to set title.
        /// </summary>
        /// <param name="title">Title of that element.</param>
        /// <param name="description">Description of that element.</param>
        public ContentAttribute(string title, string description) : base(title, description) { }

        /// <summary>
        /// Initialize all allowed fields.
        /// </summary>
        /// <param name="defaultTitle">Title that would be used by default if localization dictionary not found.</param>
        /// <param name="defaultDescription">Default description if localization dictionary not found.</param>
        /// <param name="decriptionLocalizationResourseKey">Key of description content in localized dynamic dictionary.</param>
        public ContentAttribute(
            string defaultTitle,
            string defaultDescription,
            string decriptionLocalizationResourseKey) :
            base(defaultTitle, defaultDescription, decriptionLocalizationResourseKey)
        { }

        /// <summary>
        /// Initialize all allowed fields.
        /// </summary>
        /// <param name="defaultTitle">Title that would be used by default if localization dictionary not found.</param>
        /// <param name="defaultDescription">Default description if localization dictionary not found.</param>
        /// <param name="titleLocalizationResourseKey">Key of title content in localized dynamic dictionary.</param>
        /// <param name="decriptionLocalizationResourseKey">Key of description content in localized dynamic dictionary.</param>
        public ContentAttribute(
            string defaultTitle,
            string defaultDescription,
            string titleLocalizationResourseKey,
            string decriptionLocalizationResourseKey) :
            base(defaultTitle, defaultDescription, titleLocalizationResourseKey, decriptionLocalizationResourseKey)
        { }

        public override void LanguagesDictionariesUpdated()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Connecting instiniated control with label to localization updates.
        /// </summary>
        /// <param name="layer">Current layout layer.</param>
        /// <param name="args">Must contains ILabel object reference.</param>
        public void OnGUI(ref LayoutLayer layer, params object[] args)
        {
            // Intiniated label.
            UI.Controls.ILabel label = null;
            foreach(object obj in args)
            {
                // Check if argument is label.
                if(obj is UI.Controls.ILabel bufer)
                {
                    label = bufer;
                    break;
                }
            }

            // Throw exception if control not shared.
            if (label == null) 
                throw new NotSupportedException( "Require `" + 
                    typeof(UI.Controls.ILabel).FullName + "` UI control shared via args[].");

            // Udate content.
            LanguagesDictionariesUpdated();
        }
    }
}
