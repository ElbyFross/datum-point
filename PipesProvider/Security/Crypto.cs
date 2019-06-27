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

namespace PipesProvider.Security
{
    /// <summary>
    /// Class that provide metods for providing server transmission sequrity.
    /// </summary>
    public static class Crypto
    {
        #region Enums
        /// <summary>
        /// Enum  that describe type of SHA hash algorithm.
        /// </summary>
        public enum SHATypes
        {
            SHA1,
            SHA256,
            SHA384,
            SHA512
        }
        #endregion


        #region RSA
        /// <summary>
        /// Current crypto service provider. Using RSA algortihm with 2048 bit key.
        /// </summary>
        public static RSACryptoServiceProvider CryptoServiceProvider_RSA
        {
            get
            {
                // Create new provider if not found.
                if (_CryptoServiceProvider_RSA == null)
                    _CryptoServiceProvider_RSA = new RSACryptoServiceProvider(2048);
                return _CryptoServiceProvider_RSA;
            }
        }
        private static RSACryptoServiceProvider _CryptoServiceProvider_RSA;

        /// <summary>
        /// Public RSA key that must b used to encrypt of message befor send.
        /// </summary>
        public static RSAParameters PublicKey
        {
            get
            {
                return CryptoServiceProvider_RSA.ExportParameters(false);
            }
        }

        /// <summary>
        /// Return Public key in XML format.
        /// </summary>
        public static string PublicKeyXML
        {
            get
            {
                var sw = new StringWriter();
                var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
                xs.Serialize(sw, PublicKey);
                return sw.ToString();
            }
        }

        /// <summary>
        /// Return private RSA key that can be used to decode message.
        /// </summary>
        public static RSAParameters PrivateKey
        {
            get
            {
                return CryptoServiceProvider_RSA.ExportParameters(true);
            }
        }

        public static byte[] RSADecrypt(byte[] DataToDecrypt, bool DoOAEPPadding)
        {
            try
            {
                byte[] decryptedData;
                //Create a new instance of RSACryptoServiceProvider.
                using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
                {
                    //Import the RSA Key information. This needs
                    //to include the private key information.
                    RSA.ImportParameters(PrivateKey);

                    //Decrypt the passed byte array and specify OAEP padding.  
                    //OAEP padding is only available on Microsoft Windows XP or
                    //later.  
                    decryptedData = RSA.Decrypt(DataToDecrypt, DoOAEPPadding);
                }
                return decryptedData;
            }
            //Catch and display a CryptographicException  
            //to the console.
            catch (CryptographicException e)
            {
                Console.WriteLine(e.ToString());

                return null;
            }
        }
        #endregion


        #region Hash
        /// <summary>
        /// Return the hash of the string.
        /// Use SHA256 as default.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string StringToSHA(string input)
        {
            return StringToSHA(input, SHATypes.SHA256);
        }

        /// <summary>
        /// Return hash of string.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string StringToSHA(string input, SHATypes type)
        {
            byte[] hashValue = null;
            HashAlgorithm hashAlgorithm = null;

            // Select algorithm
            switch (type)
            {
                case SHATypes.SHA1:
                    hashAlgorithm = SHA1.Create();
                    break;
                case SHATypes.SHA256:
                    hashAlgorithm = SHA256.Create();
                    break;
                case SHATypes.SHA384:
                    hashAlgorithm = SHA384.Create();
                    break;
                case SHATypes.SHA512:
                    hashAlgorithm = SHA512.Create();
                    break;
            }


            // Compute the hash of the fileStream.
            try
            {
                hashValue = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));
            }
            catch (Exception ex)
            {
                Console.WriteLine("HASH COMPUTING ERROR: {0}", ex.Message);
            }

            // Dispose unmanaged resource.
            hashAlgorithm.Clear();

            // Convert byte array to string.
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hashValue.Length; i++)
            {
                builder.Append(hashValue[i].ToString("x2"));
            }
            return builder.ToString();
        }
        #endregion


        #region Generators
        /// <summary>
        /// TODO Create binnary file with unique 256 bit key that can be used as seed to session providing.
        /// </summary>
        /// <param name="targetDirectory"></param>
        public static void GenerateUniqueKey256Bit(string targetDirectory)
        {
            // Create directory if not exist.
            if (!Directory.Exists(targetDirectory))
                Directory.CreateDirectory(targetDirectory);
        }
        #endregion
    }
}
