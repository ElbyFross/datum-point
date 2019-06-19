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
    /// <summary>
    /// Interface that devlare the base properties fo orders.
    /// </summary>
    public interface IOrder
    {
        /// <summary>
        /// Date and session whe order will started.
        /// </summary>
        OrderDate StartDate { get; set; }

        /// <summary>
        /// Date and session when order will expire inpact.
        /// </summary>
        OrderDate ExpireDate { get; set; }

        /// <summary>
        /// User that place this order.
        /// </summary>
        string OrderPublisher { get; }

        /// <summary>
        /// When this order was pooled to stack.
        /// </summary>
        DateTime PooledTime { get; }
    }
}
