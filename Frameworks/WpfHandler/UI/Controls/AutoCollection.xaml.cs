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
using System.Collections.ObjectModel;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfHandler.UI.AutoLayout;
using WpfHandler.UI.AutoLayout.Interfaces;

namespace WpfHandler.UI.Controls
{
    /// <summary>
    /// Creating UI element for work with binded collections.
    /// </summary>
    public partial class AutoCollection : UserControl, IGUIField
    {
        /// <summary>
        /// If the add button available for an user.
        /// </summary>
        public bool AddButonVisibile { get; set; }

        /// <summary>
        /// Is the remove button available for an user.
        /// </summary>
        public bool RemoveButonVisibile { get; set; }

        /// <summary>
        /// Is the spliting lines between content elements are visible.
        /// </summary>
        public bool ContentSplitersVisibility { get; set; }

        /// <summary>
        /// Orientation of the items into colelction.
        /// </summary>
        public Orientation Orientation { get; set; }

        /// <summary>
        /// UI elemets existing into the current collection.
        /// </summary>
        public ObservableCollection<IGUIField> Elements { get; } = new ObservableCollection<IGUIField>();

        /// <summary>
        /// Source collection applied to the UI.
        /// </summary>
        public object Value
        { 
            get { return source; }
            set
            {
                // Drop if applied source is not IEnumerable.
                if (!(value is IEnumerable))
                    throw new InvalidCastException("Source member mus implement IEnumerable interface.");

                // Updating referece.
                source = value;

                // TODO: Unsubscribe from current handlers.

                // TODO: Clearing the current GUI.

                // TODO: Instiniating new UI elements.

                // Inform subscribers.
                ValueChanged?.Invoke(this);
            }
        }

        private object source;

        /// <summary>
        /// Member from UIDescriptor binded to the UI leement.
        /// </summary>
        public MemberInfo BindedMember { get; set; }

        /// <summary>
        /// Initialize that component.
        /// </summary>
        public AutoCollection()
        {
            InitializeComponent();
            DataContext = this;
        }

        /// <summary>
        /// Will occure when source or one from elements will change.
        /// </summary>
        public event Action<IGUIField> ValueChanged;

        /// <summary>
        /// Configurating collection and binding element to the descriptors handler.
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="args"></param>
        public void OnGUI(ref LayoutLayer layer, params object[] args)
        { 
            // Find required referendes.
            UIDescriptor desc = null;
            MemberInfo member = null;

            // Trying to get shared properties.
            foreach (object obj in args)
            {
                if (obj is UIDescriptor) desc = (UIDescriptor)obj;
                if (obj is MemberInfo) member = (MemberInfo)obj;
            }

            // Try to bind control to auto layout handler. Drop if failed.
            //if (!UIDescriptor.TryToBindControl(this, desc, member)) return;

            // TODO: Update GUI.
        }
    }
}
