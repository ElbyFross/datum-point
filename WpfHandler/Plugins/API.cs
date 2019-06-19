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
using System.Windows;
using System.Windows.Controls;

namespace WpfHandler.Plugins
{
    /// <summary>
    /// Class that profide simplifyed way to integrate WPF plugins to client.
    /// </summary>
    public static class API
    {
        public static void OpenGUI(UniformClient.Plugins.IPlugin plugin)
        {
            UIElement pluginUI = plugin as UIElement;
            if (pluginUI == null)
                return;

            Window main = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            if (main != null)
            {
                Panel panel = (Panel)main.FindName("canvas");

                if (panel != null)
                {
                    panel.Children.Clear();
                    panel.Children.Add(pluginUI);
                }
            }
        }
    }
}
