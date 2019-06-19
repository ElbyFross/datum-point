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
    public abstract class BaseOrder : IOrder
    {
        /// <summary>
        /// Date and session whe order will started.
        /// </summary>
        public OrderDate StartDate { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <summary>
        /// Date and session when order will expire inpact.
        /// </summary>
        public OrderDate ExpireDate { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <summary>
        /// User that place this order.
        /// </summary>
        public string OrderPublisher => throw new NotImplementedException();

        /// <summary>
        /// When this order was pooled to stack.
        /// </summary>
        public DateTime PooledTime => throw new NotImplementedException();
    }
}
