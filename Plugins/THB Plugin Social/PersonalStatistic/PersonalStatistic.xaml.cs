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
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UniformClient.Plugins;

namespace THB_Plugin_Social.PersonalStatistic
{
    /// <summary>
    /// Interaction logic for PersonalStatistic.xaml
    /// </summary>
    public partial class PersonalStatistic : UserControl, IPlugin
    {
        public PersonalStatistic()
        {
            InitializeComponent();
        }

        public MenuItemMeta Meta { get; set; } = new MenuItemMeta()
        {
            domain = "0_main.20_pStat",
            titleDictionaryCode = "p_podshyvalov_pStat_menuTitle",
            defaultTitle = "Personal statistic"
        };

        public void OnStart(object sender)
        {
            // Request changing of GUI.
            WpfHandler.Plugins.API.OpenGUI(this);
        }
    }
}
