// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// Copyright (c) 2019 Volodymyr Podshyvalov

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
