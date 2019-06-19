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

namespace THB_Plugin_Exams
{
    /// <summary>
    /// Interaction logic for TestConstructor.xaml
    /// </summary>
    public partial class TestConstructor : UserControl, UniformClient.Plugins.IPlugin
    {
        public TestConstructor()
        {
            InitializeComponent();
        }

        public MenuItemMeta Meta { get; set; } = new MenuItemMeta() {
            defaultTitle = "Test constructor",
            domain = "30_exams.20_testConstructor",
            titleDictionaryCode = "p_podshyvalov_testConstructor_menuTitle" };

        public void OnStart(object sender)
        {
            WpfHandler.Plugins.API.OpenGUI(this);
        }
    }
}
