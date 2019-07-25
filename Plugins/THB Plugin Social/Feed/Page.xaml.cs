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
using WpfHandler.Plugins;

namespace TeacherHandbook.Plugins.Feed
{
    /// <summary>
    /// Interaction logic for Feed.xaml
    /// </summary>
    public partial class Page : UserControl, IPlugin
    {
        public Page()
        {
            InitializeComponent();

            // Setup values to meta.
            Meta.domain = "0_main";
            Meta.titleDictionaryCode = "p_podshyvalov_feed_menuTitle";
        }

        public MenuItemMeta Meta { get; set; } = new MenuItemMeta();

        public void OnStart(object sender)
        {
            // Request changing of GUI.
            API.OpenGUI(this);
        }
    }
}