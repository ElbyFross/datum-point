// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// Copyright (c) 2019 Volodymyr Podshyvalov

using System;

namespace TeacherHandbook.Core.Orders
{
    [Serializable]
    public struct OrderDate
    {
        /// <summary>
        /// Date of order releasing.
        /// </summary>
        public DateTime date;

        /// <summary>
        /// Nuber of target session like a part of entire day.
        /// </summary>
        public int session;

        public OrderDate(DateTime date)
        {
            this.date = date;
            session = 1;
        }

        public OrderDate(int sesstion)
        {
            this.date = DateTime.Now;
            this.session = sesstion;
        }

        public OrderDate(DateTime date, int session)
        {
            this.date = date;
            this.session = session;
        }
    }
}
