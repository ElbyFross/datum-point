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
    /// Single session binded to the groups and teacher.
    /// </summary>
    [Serializable]
    public class Session
    {
        /// <summary>
        /// Hashtable that contain data pairs: 
        /// subject (string key) -> groups (string key)
        /// </summary>
        public Hashtable subjects = new Hashtable();
    }
}
