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
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace UniformClient
{
    /// <summary>
    /// Provide client sequrity part.
    /// </summary>
    class SecurityAPI
    {
        /// <summary>
        /// Encrypt string message and make it ready to trasmition to the server.
        /// </summary>
        /// <param name="message">Message that will be encrypted.</param>
        /// <param name="serverPublicKey">Public encrypt key that was shered by target server.</param>
        /// <returns></returns>
        public static string EncryptString(string message, RSAParameters serverPublicKey)
        {
            // Conver message to byte array.
            byte[] bytedMessage = Encoding.UTF8.GetBytes(message);

            // Encrypt byte array.
            byte[] encryptedMessage = RSAEncrypt(bytedMessage, serverPublicKey, false);

            // Create encrypted string.
            return Encoding.UTF8.GetString(encryptedMessage);
        }

        /// <summary>
        /// Encrypt byte array by public server RSA key.
        /// </summary>
        /// <param name="DataToEncrypt">Data that will be encrypted.</param>
        /// <param name="serverPublicKey">Public encrypt key of target server.</param>
        /// <param name="DoOAEPPadding"></param>
        /// <returns></returns>
        public static byte[] RSAEncrypt(byte[] DataToEncrypt, RSAParameters serverPublicKey, bool DoOAEPPadding)
        {
            try
            {
                byte[] encryptedData;
                //Create a new instance of RSACryptoServiceProvider.
                using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
                {

                    //Import the RSA Key information. This only needs
                    //toinclude the public key information.
                    RSA.ImportParameters(serverPublicKey);

                    //Encrypt the passed byte array and specify OAEP padding.  
                    //OAEP padding is only available on Microsoft Windows XP or
                    //later.  
                    encryptedData = RSA.Encrypt(DataToEncrypt, DoOAEPPadding);
                }
                return encryptedData;
            }
            //Catch and display a CryptographicException  
            //to the console.
            catch (CryptographicException e)
            {
                Console.WriteLine(e.Message);

                return null;
            }
        }
    }
}
