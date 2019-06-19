// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// Copyright (c) 2019 Volodymyr Podshyvalov

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeacherHandbook.Core.Enums;

namespace TeacherHandbook.Core.Containers.Entities
{
    /// <summary>
    /// Base class implemented the API for users.
    /// </summary>
    [Serializable]
    public abstract class BaseUser : IUser
    {
        #region Public properties
        /// <summary>
        /// Rights provided to current user
        /// </summary>
        public UserRights Rigts { get { return rights; } }

        /// <summary>
        /// Hashtable that contain names relative to languages.
        /// Key (string) -> language key
        /// Value (NameContainer) -> localized data.
        /// </summary>
        public Hashtable Name { get { return name; } set { name = value; } }
        #endregion

        #region Protected fields
        /// <summary>
        /// Hashtable that contain names relative to languages.
        /// Key (string) -> language key
        /// Value (NameContainer) -> localized data.
        /// </summary>
        protected System.Collections.Hashtable name = new System.Collections.Hashtable();

        /// <summary>
        /// Rights provided to current user
        /// </summary>
        protected UserRights rights;
        #endregion
    }
}
