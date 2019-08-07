using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DatumPoint.Controls
{
    /// <summary>
    /// Interaction logic for CatalogButton.xaml
    /// </summary>
    public partial class CatalogButton : UserControl
    {
        #region Dependency properties
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
          "Text", typeof(string), typeof(CatalogButton));

        //public static readonly DependencyProperty HierarchyLevelProperty = DependencyProperty.Register(
        //  "HierarchyLevel", typeof(int), typeof(CatalogButton));

        public static readonly DependencyProperty UnfocusedBackgroundColorProperty = DependencyProperty.Register(
          "UnfocusedBackgroundColor", typeof(Brush), typeof(CatalogButton));

        public static readonly DependencyProperty FocusedBackgroundColorProperty = DependencyProperty.Register(
          "FocusedBackgroundColor", typeof(Brush), typeof(CatalogButton));

        public static readonly DependencyProperty TextColorProperty = DependencyProperty.Register(
          "TextColor", typeof(Brush), typeof(CatalogButton));

        public static readonly DependencyProperty ClickCallbackProperty = DependencyProperty.Register(
          "Click", typeof(RoutedEventHandler), typeof(CatalogButton));
        #endregion

        #region Properties
        /// <summary>
        /// Text that  will be displayed on the button.
        /// </summary>
        public string Text
        {
            get { return (string)this.GetValue(TextProperty); }
            set { this.SetValue(TextProperty, value); }
        }

        /// <summary>
        /// Define offset of the button text. 
        /// </summary>
        public int HierarchyLevel { get; set; } = 1;                

        /// <summary>
        /// Compute margine relative to hierarchy level.
        /// </summary>
        public System.Windows.Thickness AutoMargin
        {
            get
            {
                return new Thickness(HierarchyLevel * 20, Margin.Top - 5, Margin.Right, Margin.Bottom - 5);
            }
        }

        /// <summary>
        /// Collor of button when it unfocused.
        /// </summary>
        public Brush UnfocusedBackgroundColor { get; set; } = Brushes.Transparent;

        /// <summary>
        /// Color of button when it focused.
        /// </summary>
        public Brush FocusedBackgroundColor { get; set; } = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00A8E8"));
        
        /// <summary>
        /// Collor of the text.
        /// </summary>
        public Brush TextColor { get; set; } = Brushes.White;

        /// <summary>
        /// Method that will has been calling during click on button.
        /// </summary>
        public System.Action<object> ClickCallback { get; set; }
        #endregion

        public CatalogButton()
        {
            InitializeComponent();
            DataContext = this;

            // Try to load default style
            if (Application.Current.FindResource("MenuButton") is Style style)
                this.Style = style;
        }

        /// <summary>
        /// Callback that will has been calling when button will be clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CatalogButton_Click(object sender, RoutedEventArgs e)
        {
            // Call target handler if avalaiable.
            ClickCallback?.Invoke(sender);
        }
    }
}
