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
using System.IO;
using System.Xml.Serialization;
using System.Xml;

namespace AuthorityController.Data
{
    /// <summary>
    /// Object that contain data for setup of authority controller.
    /// </summary>
    [System.Serializable]
    public class Config
    {
        #region Constants
        /// <summary>
        /// Directory that will contain serialized instance of this class.
        /// </summary>
        public const string DIRECTORY = "//resources//ac//";

        /// <summary>
        /// Name of the file that will be loaded as config.
        /// </summary>
        public const string CONFIG_FILE_NAME = "config.xml";        
        #endregion

        #region Single tone
        /// <summary>
        /// Reference to current configs.
        /// Auto load one from resources if exist by DIRECTORY.
        /// </summary>
        [XmlIgnore]
        public static Config Active
        {
            get
            {
                if(active == null)
                {
                    // Try to load config from directory.
                    if(!TryToLoad<Config>(DIRECTORY, CONFIG_FILE_NAME, out active))
                    {
                        // Create new one if failed.
                        active = new Config();

                        // Save to resources.
                        active.SaveAs(DIRECTORY, CONFIG_FILE_NAME);

                    }
                }
                return active;
            }
        }
        [XmlIgnore]
        private static Config active;
        #endregion

        #region Salt
        /// <summary>
        /// Salt loaded from file.
        /// </summary>
        [XmlIgnore]
        public byte[] Salt
        {
            get
            {
                // If not found
                if (salt == null)
                {
                    // Try to load from resources.
                    if (!TryToLoad<byte[]>(DIRECTORY, CONFIG_FILE_NAME, out salt))
                    {
                        // Generate new salt.

                        // Save to resources.
                    }
                }
                return salt;
            }
        }
        [XmlIgnore]
        private byte[] salt;
        #endregion


        #region Serialized fields
        /// <summary>
        /// Directory to folder that will contain users data.
        /// </summary>
        public string UsersStorageDirectory = "//Resorces//users//";

        /// <summary>
        /// Name of the file that will contain salt.
        /// </summary>
        public string SALT_FILE_NAME = ".salt";

        /// <summary>
        /// How many minutes token is valid.
        /// </summary>
        public int tokenValidTimeMinutes = 1440;
        #endregion


        #region Constructors
        public Config()
        {
            // Set as active.
            active = this;
        }
        #endregion

        #region API
        /// <summary>
        /// Saving config file to directory.
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="fileName"></param>
        public void SaveAs(string directory, string fileName)
        {
            // Check directory exist.
            if (!Directory.Exists(directory))
            {
                // Create new if not exist.
                Directory.CreateDirectory(directory);
            }

            // Convert table to XML file.
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                XmlSerializer serializer = new XmlSerializer(typeof(Config));
                using (MemoryStream stream = new MemoryStream())
                {
                    serializer.Serialize(stream, this);
                    stream.Position = 0;
                    xmlDocument.Load(stream);
                    xmlDocument.Save(directory + fileName + ".xml");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Auth control error (ACC 10): Not serialized. Reason:\n{0}", ex.Message);
            }
        }
        
        /// <summary>
        /// Trying to deserialize object from XML file.
        /// </summary>
        /// <typeparam name="T">Required type</typeparam>
        /// <param name="directory">Folder where file stored.</param>
        /// <param name="fileName">Name of the file incliding extension.</param>
        /// <param name="result">Deserizlised object.</param>
        /// <returns></returns>
        public static bool TryToLoad<T>(string directory, string fileName, out T result)
        { // Check file exist.
            if (!File.Exists(directory))
            {
                result = default(T);
                return false;
            }

            // Init encoder.
            XmlSerializer xmlSer = new XmlSerializer(typeof(T));

            // Open stream to XML file.
            using (FileStream fs = new FileStream(directory + fileName, FileMode.Open))
            {
                try
                {
                    // Try to deserialize object from file.
                    result = (T)xmlSer.Deserialize(fs);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Auth control error (ACC 20): File reading failed. Reason:\n{0}\n", ex.Message);
                    result = default(T);
                    return false;
                }
            }

            return true;

        }
        #endregion
    }
}
