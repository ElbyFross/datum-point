// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// Copyright (c) 2019 Volodymyr Podshyvalov

using System;

namespace TeacherHandbook.Core.Containers.Schedule
{
    /// <summary>
    /// Sigle data that contain shedule.
    /// </summary>
    [Serializable]
    public class Day
    {
        /// <summary>
        /// Unitye key of this day in logbook.
        /// format dd.mm.yyyy
        /// </summary>
        public string key = null;

        /// <summary>
        /// List of sessions during this day.
        /// </summary>
        public Session[] sessions = null;
    }
}
