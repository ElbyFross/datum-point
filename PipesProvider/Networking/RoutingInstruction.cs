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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;
using System.Xml;

namespace PipesProvider.Networking
{
    /// <summary>
    /// Object that contain routing instgructions.
    /// Provide API to work with file system.
    /// </summary>
    [System.Serializable]
    public class RoutingTable
    {
        #region Structs
        /// <summary>
        /// Struct that contain instruction about target adress by relative query params.
        /// Allow using of several servers via one public.
        /// 
        /// Example:
        ///                          <--> Authification server
        /// Client <--> Query server <--> Data server 1
        ///                          <--> Data server 2
        /// </summary>
        [System.Serializable]
        public struct RoutingInstruction
        {
            /// <summary>
            /// Address that will be ised for routing
            /// </summary>
            public string routingIP;

            /// <summary>
            /// Logon config recuired to server connection.
            /// </summary>
            public Security.LogonConfig logonConfig;

            /// <summary>
            /// Array that contain querie's body that need to be routed by this instruction.
            /// 
            /// Format:
            /// property=value&property=value&... etc. - Encount all properties that need to be a part of query by splitting with UniformQueries.API.SPLITTING_SYMBOL ('&' by default).
            /// !property - this property must be out of query.
            /// $property - this property must exist in query.
            /// 
            /// Example:
            /// targetQueries[0] = "q=GET&sq="PUBLICKEY";   // All queries that contain GET query and PUBLICKEY sub-query will routed.
            /// targetQueries[1] = "q=GET&!pk";             // All queries that request data from server but has no RSA public keys for backward encription will wouted.
            /// targetQueries[1] = "$customProp";           // All queries that have "customProp" property in query will be routed.
            /// </summary>
            public string[] queryPatterns;

            public bool IsRoutingTarget(string recivedQuery)
            {
                // Get query patrs.
                UniformQueries.QueryPart[] splitedQuery = UniformQueries.API.DetectQueryParts(recivedQuery);

                // Declere variables out of loops for avoid additive allocating.
                bool valid = true;
                char instructionOperator;

                // Check every pattern.
                foreach (string pattern in queryPatterns)
                {
                    // Marker that shoved up checking up result.
                    valid = true;

                    // Split pattern to instructions.
                    UniformQueries.QueryPart[] patternParts = UniformQueries.API.DetectQueryParts(pattern);

                    // Compare every instruction.
                    foreach (UniformQueries.QueryPart pp in patternParts)
                    {
                        // If instruction.
                        #region Instuction processing
                        if (string.IsNullOrEmpty(pp.propertyValue))
                        {
                            instructionOperator = pp.propertyValue[0];

                            switch (instructionOperator)
                            {
                                // Not contain instruction.
                                case '!':
                                    // Check parameter existing.
                                    if (UniformQueries.API.QueryParamExist(pp.propertyValue.Substring(1), splitedQuery))
                                    {
                                        // Mark as invalid if found.
                                        valid = false;
                                    }
                                    break;

                                // Property exist instruction.
                                case '$':
                                default:
                                    // Check parameter existing.
                                    if (!UniformQueries.API.QueryParamExist(pp.propertyValue.Substring(1), splitedQuery))
                                    {
                                        // Mark as invalid if not found.
                                        valid = false;
                                    }
                                    break;
                            }
                        }
                        #endregion
                        // If full query part.
                        #region Query part processing
                        else
                        {
                            // Try to get requested value.
                            if (UniformQueries.API.TryGetParamValue(pp.propertyName, out UniformQueries.QueryPart propertyBufer, splitedQuery))
                            {
                                // Check param value.
                                if (!propertyBufer.ParamValueEqual(pp.propertyValue))
                                {
                                    // Mark as invalide.
                                    valid = false;
                                }
                            }
                            else
                            {
                                // Mark as invalide if param not found.
                                valid = false;
                            }
                        }
                        #endregion

                        // Drop if instruction already failed.
                        if (!valid) break;
                    }

                    // Drop if instruction validated.
                    if (valid) break;
                }

                // Return validation result.
                return valid;
            }
        }
        #endregion


        /// <summary>
        /// List that contain routing instructions.
        /// </summary>
        public List<RoutingInstruction> intructions = new List<RoutingInstruction>();

        /// <summary>
        /// Path from was loaded tis table.
        /// </summary>
        public string SourcePath { get; set; }

        /// <summary>
        /// Trying to load all routing tables from durectory.
        /// You could have several XML serialized routing tables. This way allow to share it via plugins.
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        public static RoutingTable LoadRoutingTables(string directory)
        {
            // Create new empty table.
            RoutingTable table = new RoutingTable();

            // Check directory exist.
            if (!Directory.Exists(directory))
            {
                // Create if not found.
                Directory.CreateDirectory(directory);
            }

            // Detect all xml files in directory.
            string[] xmlFiles = Directory.GetFiles(directory, "*.xml", SearchOption.AllDirectories);

            // Init encoder.
            XmlSerializer xmlSer = new XmlSerializer(typeof(RoutingTable));

            // Deserialize every file to table if possible.
            foreach (string fileDir in xmlFiles)
            {
                // Open stream to XML file.
                using (FileStream fs = new FileStream(fileDir, FileMode.Open))
                {
                    RoutingTable tableBufer = null;
                    try
                    {
                        // Try to deserialize routing table from file.
                        tableBufer = xmlSer.Deserialize(fs) as RoutingTable;

                        // Buferize directory to backward access.
                        tableBufer.SourcePath = fileDir;

                        // Add to  common table.
                        table += tableBufer;
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine("ROUTING TABLE ERROR: File reading failed. Reason:\n{0}\n", ex.Message);
                    }

                }
            }

            return table;
        }

        public static void SaveRoutingTable(RoutingTable table, string directory, string name)
        {
            // Check directory exist.
            if (!Directory.Exists(directory))
            {
                // Create if not found.
                Directory.CreateDirectory(directory);
            }

            // Convert table to XML file.
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                XmlSerializer serializer = new XmlSerializer(typeof(RoutingTable));
                using (MemoryStream stream = new MemoryStream())
                {
                    serializer.Serialize(stream, table);
                    stream.Position = 0;
                    xmlDocument.Load(stream);
                    xmlDocument.Save(directory + name + ".xml");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ROUTING TABLE ERROR: Not serialized. Reason:\n{0}", ex.Message);
            }
        }

        /// <summary>
        /// Adding instruction from second table to first one.
        /// </summary>
        /// <param name="table0"></param>
        /// <param name="table1"></param>
        /// <returns></returns>
        public static RoutingTable operator + (RoutingTable table0, RoutingTable table1)
        {
            // Validate first table.
            if (table0 == null)
            {
                // init new table.
                table0 = new RoutingTable();
            }

            // Validate second table.
            if (table1 == null)
            {
                // Operation not possible. Return first table.
                return table0;
            }

            // Find every instruction in loaded table.
            foreach (RoutingInstruction instruction in table1.intructions)
            {
                // Copy instructions to output table.
                table0.intructions.Add(instruction);
            }

            return table0;
        }
    }
}
