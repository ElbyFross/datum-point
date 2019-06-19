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

namespace THB_Data_Server
{
    public static class Commands
    {
        /// <summary>
        /// React to the command.
        /// </summary>
        /// <param name="command"></param>
        public static bool CommandResponseProcessor(string command)
        {
            // Skip if command is empty.
            if (string.IsNullOrEmpty(command))
                return false;

            // Split command on parts.
            // 0 element is a command.
            // 1+ atguments.
            string[] commandParts = command.Split(' ');

            // Switch by main command.
            switch (commandParts[0])
            {
                // Show information about application commands.
                case "help":
                    Console.WriteLine();
                    ConsoleDraw.Primitives.DrawLine();
                    Console.WriteLine("\nCOMMANDS LIST:\n{0}\n{1}\n{2}\n{3}\n{4}",
                        "help - List of available commands.",
                        "qhelp - List of available queries with description for each.",
                        "stop - Stoping server and threads. Finishing main loop.",
                        "threads - Return information about started threads.",
                        "threads <int> - Change the count of started server threads.");
                    ConsoleDraw.Primitives.DrawLine();
                    Console.WriteLine();

                    break;

                // Close application.
                case "stop":
                    Server.appTerminated = true;
                    break;

                case "qhelp":

                    Console.WriteLine();
                    ConsoleDraw.Primitives.DrawLine();
                    Console.WriteLine();
                    Console.WriteLine("QUERIES LIST:");
                    Console.WriteLine();
                    foreach (UniformQueries.IQueryHandlerProcessor qp in UniformQueries.API.QueryProcessors)
                    {
                        Console.WriteLine(qp.Description("en"));
                        Console.WriteLine();
                    }
                    ConsoleDraw.Primitives.DrawLine();
                    Console.WriteLine();
                    break;

                case "threads":
                    int requestedThreads;

                    // If include argument.
                    if (commandParts.Length > 1)
                    {
                        if (Int32.TryParse(commandParts[1], out requestedThreads))
                        {
                            Server.ThreadsCount = requestedThreads;
                        }
                        else
                        {
                            Console.WriteLine("INCORRECT COMMAND: Require int argument.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("ACTUAL COUNT OF THREADS: {0}/{1}",
                            Server.ThreadsCount, Environment.ProcessorCount);
                    }
                    break;

                // Inform about inccorect command.
                default:
                    Console.WriteLine("Command not found.");
                    return false;
            }
            return true;
        }
    }
}
