using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfHandler.UI.Controls
{
    public abstract class LayoutSizeAttribute : Attribute, ILayoutSize
    {
        /// <summary>
        /// Value that will be used in the element's propeties.
        /// </summary>
        public double Size { get; set; } = double.NaN;

        /// <summary>
        /// Default constructor.
        /// Use auto width.
        /// </summary>
        public LayoutSizeAttribute() { }

        /// <summary>
        /// Set requested width as Size.
        /// </summary>
        /// <param name="width"></param>
        public LayoutSizeAttribute(double width)
        {
            Size = width;
        }
    }
}
