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
using UniformQueries;

namespace AuthorityController.Queries
{
    /// <summary>
    /// Set temporaly or permanent ban for user.
    /// </summary>
    public class USER_BAN : IQueryHandlerProcessor
    {
        public static string[] requiredRights = new string[] { "ban" };

        public string Description(string cultureKey)
        {
            throw new NotImplementedException();
        }

        public void Execute(QueryPart[] queryParts)
        {
            // Get requestor token.
            UniformQueries.API.TryGetParamValue("token", out QueryPart token, queryParts);

            // Get target user id or login.
            UniformQueries.API.TryGetParamValue("user", out QueryPart user, queryParts);

            // XML serialized BanInformation. If empty then will shared permanent ban.
            UniformQueries.API.TryGetParamValue("ban", out QueryPart ban, queryParts);

            // Check token rights.
            try
            {
                // Get token of query's sender.
                API.Tokens.IsHasEnoughRigths(token.propertyValue, requiredRights);
            }
            catch (Exception ex)
            {
                // if token not registred.
                if(ex is UnauthorizedAccessException)
                {
                    // Inform that token not registred.
                    UniformServer.BaseServer.SendAnswer("ERROR 401: Invalid token", queryParts);
                    return;
                }
            }


            #region Detect target user
            // Find user for ban.
            Data.User userProfile = null;
            bool userFound = false;

            // Try to parse id from query.
            if (Int32.TryParse(user.propertyValue, out int userId))
            {
                // Try to find user by id.
                if (API.Users.TryToFindUser(userId, out userProfile))
                {
                    userFound = true;
                }
            }

            // if user not found by ID.
            if(!userFound)
            {
                // Try to find user by login.
                if (!API.Users.TryToFindUser(user.propertyValue, out userProfile))
                {
                    // If also not found.
                    UniformServer.BaseServer.SendAnswer("ERROR 404: User not found", queryParts);
                    return;
                }
            }
            #endregion

            #region Apply ban
            // Get ban information.
            Data.BanInformation banInfo;

            // Deserialize ban information from shared xml data.
            if (string.IsNullOrEmpty(ban.propertyValue))
            {
                // Init encoder.
                XmlSerializer xmlSer = new XmlSerializer(typeof(Data.BanInformation));

                // Open stream to XML file.
                using (StringReader fs = new StringReader(ban.propertyValue))
                {
                    try
                    {
                        // Try to deserialize value to ban information.
                        banInfo = (Data.BanInformation)xmlSer.Deserialize(fs);
                    }
                    catch
                    {
                        // If also not found.
                        UniformServer.BaseServer.SendAnswer("ERROR 404: Ban information corrupted.", queryParts);
                        return;
                    }
                }
            }
            else
            {
                // Set auto configurated permanent ban if detail not described.
                banInfo = Data.BanInformation.Permanent;
            }

            // Add ban to user.
            userProfile.bans.Add(banInfo);

            // Update stored profile.
            // in other case ban will losed after session finishing.
            API.Users.SetProfile(userProfile);
            #endregion
        }

        public bool IsTarget(QueryPart[] queryParts)
        { 
            // USER prop.
            if (!UniformQueries.API.QueryParamExist("user", queryParts))
                return false;

            // BAN prop.
            if (!UniformQueries.API.QueryParamExist("ban", queryParts))
                return false;

            return true;
        }
    }
}
