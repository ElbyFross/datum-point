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

namespace TeacherHandbook.Core.Containers.Entities
{
    /// <summary>
    /// Group of students.
    /// </summary>
    [Serializable]
    public class Group
    {
        /// <summary>
        /// Unique DB key of this group.
        /// </summary>
        public string key = null;

        /// <summary>
        /// Title that will be displayed into UI.
        /// </summary>
        public string title = "New group";

        /// <summary>
        /// Hashtable that contrain descriptions of group. 
        /// Key (string) - language key;
        /// Value (string) - description relative to language.
        /// </summary>
        public Hashtable descriptions = new Hashtable();
    }
}
