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

namespace TeacherHandbook.Core.Containers.Entities
{
    public interface IUser
    {
        /// <summary>
        /// Rights that give an access to features or restrict it.
        /// </summary>
        TeacherHandbook.Core.Enums.UserRights Rigts { get; }

        /// <summary>
        /// Hashtable that contain names relative to languages.
        /// Key (string) -> language key
        /// Valuse (NameContainer) -> localized data.
        /// </summary>
        System.Collections.Hashtable Name { get; set; }
    }
}
