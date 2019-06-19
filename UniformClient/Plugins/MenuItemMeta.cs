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
using System.Text;
using System.Linq;

namespace UniformClient.Plugins
{
    /// <summary>
    /// Contain information that will be used for connection to the automatic navigation menu in client application 
    /// </summary>
    [System.Serializable]
    public class MenuItemMeta
    {
        /// <summary>
        /// Path of item in menu hierarchy.
        /// 
        /// Domain fragment format: [prority]_domainPart.
        /// Priority will be used for sorting of plugins in menu. 
        /// If not defined then will be auto changed to SUBDOMAIN.GetHashcode().
        /// 
        /// Attention: 
        /// 0_DomainName != DomainName
        /// 0_DomainName.SubdomainName != 10_DomainName.SubdomainName
        /// 
        /// Example of plugins menu map:
        /// 0_main
        ///     0_main.0_plugin_1
        ///     0_main.1_plugin_2
        ///     
        /// big_plugin_3
        ///     big_plugin_3.10_plugin1
        ///     big_plugin_3.20_pl_2
        ///         big_plugin_3.20_pl_2.minor_plugin
        ///     big_plugin_3.p_3
        ///     
        /// big_plugin_3 // Dublicated plugin's domain. Will be added to menu but all childs will applied to first entry plugin.
        /// </summary>
        public string domain = "0_main.0_new_plugin";

        /// <summary>
        /// Code of resource in language xaml dictionary that will contain translated title.
        /// For avoidance of conflicts recommended naming format is: "p_" + author + "_" + plugin_name + "_" + title.
        /// </summary>
        public string titleDictionaryCode = "p_author_myPlugin_title";

        /// <summary>
        /// Title that will be showed in case if dictionary not found.
        /// </summary>
        public string defaultTitle = null;


        #region Constructors
        public MenuItemMeta() { }

        public MenuItemMeta(string domain, string titleDictionaryCode)
        {
            this.domain = domain;
            this.titleDictionaryCode = titleDictionaryCode;
        }
        #endregion
    }
}