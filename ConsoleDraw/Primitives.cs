// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// Copyright (c) 2019 Volodymyr Podshyvalov

using System;

namespace ConsoleDraw
{
    public static class Primitives
    {
        /// <summary>
        /// Drawing line from selected char.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="length"></param>
        public static void DrawLine(char element = '=', int length = 80)
        {
            for (int i = 0; i < length; i++)
                Console.Write(element);
        }

        /// <summary>
        /// Drawing line from selected char.
        /// Set space line befor and after separator.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="length"></param>
        public static void DrawSpacedLine(char element = '=', int length = 80)
        {
            Console.WriteLine();
            for (int i = 0; i < length; i++)
                Console.Write(element);
            Console.WriteLine();
        }
    }
}
