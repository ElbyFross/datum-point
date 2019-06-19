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

namespace TeacherHandbook.Core.Containers.Schedule
{
    /// <summary>
    /// Object that describe subject data.
    /// </summary>
    [Serializable]
    public class Subject
    {
        /// <summary>
        /// Unique key of this subject.
        /// </summary>
        public string key = null;

        /// <summary>
        /// Hashtable that contain titles relative to the localization.
        /// Key(string): Language key.
        /// Value (List<string>): List of titles that describe this subject. (Example: Math, Mathematic, H.Math, etc.)
        /// </summary>
        public Hashtable titles = new Hashtable();
    }
}
