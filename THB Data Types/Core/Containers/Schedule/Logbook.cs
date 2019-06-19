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
namespace TeacherHandbook.Core.Containers.Schedule
{
    /// <summary>
    /// Container that contain long term data about sessions.
    /// </summary>
    [Serializable]
    public class Logbook
    {
        /// <summary>
        /// Hashtable that contain all recorded days.
        /// Key(string) : day key in format dd.mm.yyyy
        /// Value(Day): day data
        /// </summary>
        protected System.Collections.Hashtable recordedDays = new System.Collections.Hashtable();

        /// <summary>
        /// List of pooled orders.
        /// </summary>
        protected List<TeacherHandbook.Core.Orders.IOrder> orders = new List<Orders.IOrder>();
    }
}
