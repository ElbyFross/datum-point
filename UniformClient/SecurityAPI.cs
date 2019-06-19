//Copyright 2019 Volodymyr Podshyvalov
//
//Licensed under the Apache License, Version 2.0 (the "License");
//you may not use this file except in compliance with the License.
//You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//Unless required by applicable law or agreed to in writing, software
//distributed under the License is distributed on an "AS IS" BASIS,
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//See the License for the specific language governing permissions and
//limitations under the License.

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
