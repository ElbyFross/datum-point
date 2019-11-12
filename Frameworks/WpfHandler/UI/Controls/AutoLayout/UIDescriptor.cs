using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Reflection;
using System.Windows.Controls;

namespace WpfHandler.UI.Controls.AutoLayout
{
    /// <summary>
    /// Class that provides adopting of members by AutoLayout UI.
    /// </summary>
    public abstract class UIDescriptor
    {
        /// <summary>
        /// Contains data about layout layer to relative element.
        /// </summary>
        private class LayoutLayer
        {
            /// <summary>
            /// Element on that will contains child elements.
            /// </summary>
            public IAddChild root;

            /// <summary>
            /// Parent layer that contains that element.
            /// </summary>
            public LayoutLayer Parent
            {
                get { return _Parent; }
                protected set
                {
                    // Drop recursive reference.
                    if (value.Equals(this)) return;

                    // update value,
                    _Parent = value;
                }
            }

            /// <summary>
            /// Check does the layer has a parent.
            /// </summary>
            public bool HasParent
            { 
                get { return _Parent != null; }
            }

            /// <summary>
            /// Bufer that containes reference to the parent layer.
            /// </summary>
            private LayoutLayer _Parent;

            /// <summary>
            /// Orientation of that UI layer.
            /// </summary>
            public Orientation orientation = Orientation.Vertical;

            /// <summary>
            /// Create next UI layer and set it as active.
            /// </summary>
            /// <param name="nextLayerRoot">Element that will be root of the new layer.</param>
            public LayoutLayer GoDeeper(IAddChild nextLayerRoot)
            {
                // Validate shared reference.
                if(nextLayerRoot == null)
                {
                    throw new NullReferenceException("Root element can't be null.");
                }

                // Add shared root as child on current layer.
                root.AddChild(nextLayerRoot);

                // Configurate new layer.
                var newLayer = new LayoutLayer()
                {
                    root = nextLayerRoot, // Set shared element as root
                    Parent = this // Set reference to current active layer like on the parent.
                };

                return newLayer;
            }

            /// <summary>
            /// Change current layer to previous one.
            /// </summary>
            public LayoutLayer GoUpper()
            {
                // Check if current layout has a parent.
                if (HasParent)
                {
                    // Return parent if exist.
                    return Parent;
                }

                // Return this if that higher layer.
                return this;
            }
        }

        /// <summary>
        /// Insiniate UI by descriptor's attributes map and add it as child to parent element.
        /// </summary>
        /// <param name="parent">UI element that would contain instiniated UI elemets.</param>
        public void BindTo(IAddChild parent)
        {
            // Get all memebers.
            var members = GetType().GetMembers();

            // Instiniate first UILayer.
            var activeLayer = new LayoutLayer()
            {
                root = parent // Thet binding target as root for cuurent layer.
            };

            // Perform all descriptor map.
            foreach (MemberInfo member in members)
            {
                // Try to get attribute that depends layer settings.
                var layerAttr = member.GetCustomAttribute(typeof(Interfaces.ILayerAttribute), true);

                #region Group begin
                // Start new UI group if requested.
                if (layerAttr is Interfaces.ILayerBeginAttribute)
                {
                    // Element that will contains childs of next UI group.
                    IAddChild newGroup = null;

                    // Start new horizontal group.
                    if (layerAttr is Attributes.BeginHorizontalGroup)
                    {
                        // Create grid.
                        newGroup = new Grid();

                    }
                    // Start new vertical group.
                    else
                    {
                        // Create stack panel.
                        newGroup = new StackPanel()
                        { Orientation = Orientation.Vertical };
                    }

                    // Set new layer.
                    activeLayer = activeLayer.GoDeeper(newGroup);
                }
                #endregion

                // Perform header if requested.
                var header = member.GetCustomAttribute(typeof(Attributes.Header), true);
                if (header != null)
                {

                }


                #region End group
                // Ending UI group if requested.
                if (layerAttr is Interfaces.ILayerEndAttribute)
                {
                    // Close current layer.
                    activeLayer = activeLayer.GoUpper();
                }
                #endregion
            }
        }
    }
}
