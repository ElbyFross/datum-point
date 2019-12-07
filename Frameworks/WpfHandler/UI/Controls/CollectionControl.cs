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
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfHandler.UI.AutoLayout;
using WpfHandler.UI.AutoLayout.Configuration;

namespace WpfHandler.UI.Controls
{
    /// <summary>
    /// Abstract class that provides common API for implementin collection controls.
    /// </summary>
    /// <remarks>
    /// Fully compatible with <see cref="UIDescriptor"/> and auto layout handlers.
    /// </remarks>
    public abstract class CollectionControl : UserControl, IGUIField
    {
        #region Dependency properties
        /// <summary>
        /// Property that bridging control's property between XAML and code.
        /// </summary>
        public static readonly DependencyProperty DragAllowedProperty = DependencyProperty.Register(
          "DragAllowed", typeof(bool), typeof(CollectionControl),
          new PropertyMetadata(true));
        #endregion

        #region Public properties
        /// <summary>
        /// Is list allows to drag elements.
        /// </summary>
        /// <remarks>
        /// </remarks>
        public virtual bool DragAllowed
        {
            get { return (bool)this.GetValue(DragAllowedProperty); }
            set 
            { 
                // Applying the value.
                this.SetValue(DragAllowedProperty, value);

                // Reconfigurating a style of the list.
                ConfigurateStyles();
            }
        }

        /// <summary>
        /// Reference to the list box that will contains spawned elements.
        /// </summary>
        public abstract ListBox ListContent { get; }

        /// <summary>
        /// Source collection applied to the UI.
        /// </summary>
        /// <remarks>
        /// Object must has implemented <see cref="IList"/> interface.
        /// </remarks>
        public object Value
        {
            get { return source; }
            set
            {
                // Drop if applied source is not IEnumerable.
                if (!(value is IList list))
                    throw new InvalidCastException("Source member mus implement IEnumerable interface.");

                // Updating referece.
                source = list;

                // Inform subscribers.
                ValueChanged?.Invoke(this);
            }
        }

        /// <summary>
        /// Member from UIDescriptor binded to the UI leement.
        /// </summary>
        public MemberInfo BindedMember { get; set; }
        
        /// <summary>
        /// UI elemets existing into the current collection.
        /// </summary>
        public ObservableCollection<FrameworkElement> Elements { get; } = new ObservableCollection<FrameworkElement>();
        #endregion

        #region Events
        /// <summary>
        /// Will occure when source or one from elements will change.
        /// </summary>
        public event Action<IGUIField> ValueChanged;
        #endregion

        #region Protected members
        /// <summary>
        /// Mapp of GUI elements binding.
        /// <see cref="IGUIField"/> - key
        /// <see cref="int"/> - index in the list.
        /// </summary>
        protected readonly Hashtable map = new Hashtable();

        /// <summary>
        /// Bufer that contains source sollection.
        /// </summary>
        protected IList source;
        #endregion


        /// <summary>
        /// Default constructor.
        /// </summary>
        public CollectionControl()
        {
            // Subcribing on the after loading event.
            this.Loaded += delegate (object sender, RoutedEventArgs e)
            {
                // Apply a suitable style.
                ConfigurateStyles();

                // Recomputing UI.
                UpdateElementsWidth(null, null);

                // Subscribing of the size update.
                ListContent.SizeChanged += UpdateElementsWidth;
            };
        }
        
        /// <summary>
        /// Configurating collection and binding element to the descriptors handler.
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="args"></param>
        public virtual void OnLayout(ref LayoutLayer layer, params object[] args)
        {
            // Clearing data.
            ListContent.Items.Clear();
            Elements.Clear();
            
            // Update GUI.
            for (int i = 0; i < source.Count; i++)
            {
                // Instinating an element.
                var element = ItemRegistration(i);
                
                // Adding element to the list.
                if (element != null) Elements.Add(element);
            }

            // Applying elements as source.
            ListContent.ItemsSource = Elements;
        }


        #region API
        /// <summary>
        /// TODO: Clearing the collection and UI.
        /// </summary>
        public virtual void Clear()
        {
            new NotImplementedException();
        }

        /// <summary>
        /// TODO: Adding source to the collection.
        /// </summary>
        /// <param name="source">An object that will adde dto the collection as item. Also wil applied to the GUI.</param>
        public virtual void Add(object source)
        {
            new NotImplementedException();
        }

        /// <summary>
        /// TODO: Reordering two elements.
        /// </summary>
        /// <param name="firstElementIndex">An index of an element in the collection.</param>
        /// <param name="secondElementIndex">An index of an element in the collection.</param>
        public virtual void Reorder(int firstElementIndex, int secondElementIndex)
        {
            // Getting sources.
            var fSource = source[firstElementIndex];
            var sSource = source[secondElementIndex];

            // Getting binded indexes.
            var fElementIndex = (int)map[fSource];
            var sElementIndex = (int)map[sSource];
            new NotImplementedException();
        }

        /// <summary>
        /// TODO: Removing element form collection by index.
        /// </summary>
        /// <param name="index">An index of an element.</param>
        public virtual void RemoveAt(int index)
        {
            new NotImplementedException();
        }

        /// <summary>
        /// TODO: Remove collection element if exist.
        /// </summary>
        /// <param name="source">An element of the collection.</param>
        public virtual void Remove(object source)
        {
            new NotImplementedException();
        }
        #endregion

        #region Local
        /// <summary>
        /// Requiesting recomputing of the elements width.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void UpdateElementsWidth(object sender, SizeChangedEventArgs e)
        {
            // Drop if the list not loaded yet.
            if(ListContent.IsLoaded == false)
            {
                // Subscribing on the after loadign event.
                ListContent.Loaded += delegate (object sender, RoutedEventArgs e)
                {
                    // Recal layout computing.
                    UpdateElementsWidth(sender, null);
                };
                // Drop.
                return;
            }

            // Update width of an every element.
            foreach (FrameworkElement field in Elements)
            {
                if (field != null)
                {
                    field.Width = ListContent.ActualWidth - 12;
                }
            }
        }

        /// <summary>
        /// Configurating styles applyed to the controls.
        /// </summary>
        protected void ConfigurateStyles()
        {
            // Defining contaier style.
            Style itemContainerStyle = new Style(typeof(ListBoxItem));

            // Enable drop possibility.
            if (AllowDrop)
            {
                itemContainerStyle.Setters.Add(new Setter(ListBoxItem.AllowDropProperty, true));
                itemContainerStyle.Setters.Add(new EventSetter(
                    ListBoxItem.PreviewMouseLeftButtonDownEvent,
                    new MouseButtonEventHandler(OnPreviewMouseLeftButtonDown)));
                itemContainerStyle.Setters.Add(new EventSetter(
                    ListBoxItem.DropEvent,
                    new DragEventHandler(OnItemDrop)));
            }
            // Disable drop possibility.
            else
            {
                itemContainerStyle.Setters.Add(new Setter(ListBoxItem.AllowDropProperty, false));
            }

            // Applying style to the list.
            ListContent.ItemContainerStyle = itemContainerStyle;
        }
        #endregion

        #region Callbacks
        /// <summary>
        /// Regestrating element for source collection.
        /// Allow to create bachward binding of UI to the source object.
        /// </summary>
        /// <param name="index">Index of the element into the <see cref="source"/> collection.</param>
        protected virtual FrameworkElement ItemRegistration(int index)
        {
            // Getting data.
            var obj = source[index];

            // Gettign type of the binded UI element.
            var controlType = LayoutHandler.GetBindedControl(obj.GetType(), true);

            // Drop if control not available.
            if (controlType == null) return null;

            // Instiniating UI element.
            var element = (IGUIField)Activator.CreateInstance(controlType);

            // Applying default value.
            element.Value = obj;

            // Adding ellement to the maping table.
            map.Add(element, index);
            
            // Subscribing on the index of the value changing.
            element.ValueChanged += CollectionElementValueChanged;

            return (FrameworkElement)element;
        }

        /// <summary>
        /// Occurs when field's valu will be changed via UI element.
        /// </summary>
        /// <param name="obj">The GUI element that initialize event.</param>
        protected virtual void CollectionElementValueChanged(IGUIField obj)
        {
            if (map[obj] is int index)
            {
                source[index] = obj.Value;
                
                // Inform subscribers.
                ValueChanged?.Invoke(this);
            }
            else
            {
                MessageBox.Show("IGUIField not binded to the list.");
            }
        }

        /// <summary>
        /// Occurs when item is selected by LMB click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Drop if selected element is not a common grid or selectable border.
            if (!e.OriginalSource.GetType().Equals(typeof(Border))
                && !e.OriginalSource.GetType().Equals(typeof(Grid)))
            {
                return;
            }

            if (sender is ListBoxItem)
            {
                ListBoxItem draggedItem = sender as ListBoxItem;
                DragDrop.DoDragDrop(draggedItem, draggedItem.DataContext, DragDropEffects.Move);
                draggedItem.IsSelected = true;
            }
        }

        /// <summary>
        /// Occurs when an element was droped after drag.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnItemDrop(object sender, DragEventArgs e)
        {
            var droppedData = e.Data.GetData(e.Data.GetFormats()[0]) as FrameworkElement;
            var target = ((ListBoxItem)(sender)).DataContext as FrameworkElement;

            int removedIdx = ListContent.Items.IndexOf(droppedData);
            int targetIdx = ListContent.Items.IndexOf(target);

            if (removedIdx < targetIdx)
            {
                Elements.Insert(targetIdx + 1, droppedData);
                Elements.RemoveAt(removedIdx);
            }
            else
            {
                int remIdx = removedIdx + 1;
                if (Elements.Count + 1 > remIdx)
                {
                    Elements.Insert(targetIdx, droppedData);
                    Elements.RemoveAt(remIdx);
                }
            }
        }
        #endregion
    }
}
