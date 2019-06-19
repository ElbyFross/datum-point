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

namespace UniformClient.Plugins
{
    /// <summary>
    /// Interface that allow to create a plugin that can be connect to client application.
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        /// Meta data that contain description for main menu integration.
        /// </summary>
        MenuItemMeta Meta{ get; set; }

        /// <summary>
        /// Method that will be called when menu item will be pressed.
        /// There you can set your controls as source for executable application's zone
        /// </summary>
        void OnStart(object sender);
    }
}
