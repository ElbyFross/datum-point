// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// Copyright (c) 2019 Volodymyr Podshyvalov

using System;

namespace TeacherHandbook.Core.Localization
{
    /// <summary>
    /// Binary container that contain hash table with keys
    /// </summary>
    [Serializable]
    public class LanguageContaier
    {
        #region Public fields
        /// <summary>
        /// Unique key of this language.
        /// </summary>
        public string key = null;

        /// <summary>
        /// Title that will be displayed in settings form.
        /// </summary>
        public string title = "New language";

        /// <summary>
        /// Hash table that contain pairs string->string, where key is unique value for access to translation.
        /// </summary>
        public System.Collections.Hashtable map = new System.Collections.Hashtable();
        #endregion
    }
}
