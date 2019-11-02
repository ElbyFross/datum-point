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
using System.Xml.Serialization;
using UniformDataOperator.Sql.Attributes;
using UniformDataOperator.Sql.MySql.Attributes;
using PipesProvider.Client;
using PipesProvider.Networking.Routing;
using UniformClient;

namespace DatumPoint.Plugins.Social.Types.AuditoryPlanner
{
    /// <summary>
    /// Class that describe certain auditory settings.
    /// </summary>
    [Serializable]
    [Table("datum-point", "Auditorium")]
    public class Auditorium
    {
        #region Fields & properties
        /// <summary>
        /// Uniquem index of that auditory.
        /// </summary>
        [Column("auditoriumid", System.Data.DbType.Int32), IsPrimaryKey, IsNotNull]
        [MySqlDBTypeOverride(MySql.Data.MySqlClient.MySqlDbType.Int32, "INT")]
        public int id = -1;

        /// <summary>
        /// Uniqum identifier of building where that auditory contains.
        /// </summary>
        [Column("buildingid", System.Data.DbType.Int32), IsPrimaryKey, IsNotNull]
        [IsForeignKey("datum-point", "buiding", "buildingid")]
        public int buildingid = 0;

        /// <summary>
        /// If of seats' schema applied to that auditory.
        /// </summary>
        public int schemaid = -1;

        /// <summary>
        /// Schema received from server relative to id described in class.
        /// </summary>
        [XmlIgnore]
        public Schema SeatsSchema
        { 
            get
            {
                if(_SeatsSchema == null)
                {
                    // TODO Request schema from server.
                }
                return _SeatsSchema;
            }
        }
        [XmlIgnore]
        protected Schema _SeatsSchema;
        
        /// <summary>
        /// Bufer that cotnain user profile loaded from server.
        /// </summary>
        [XmlIgnore]
        private DatumPoint.Types.Personality.User _ResponsibleUser;

        /// <summary>
        /// Id of user that responcible for that class.
        /// </summary>
        public uint responsibleUserId;
        #endregion

        #region API
        /// <summary>
        /// Requesting responsible user's profile from server.
        /// </summary>
        /// <param name="forceUpdate">If true then will drop current data and request new.</param>
        /// <returns></returns>
        public async Task<DatumPoint.Types.Personality.User> GetResponsibleUserAsync(bool forceUpdate = false)
        {
            // Drop if user undefined.
            if (responsibleUserId == 0) return null;

            // Return already received data.
            if (!forceUpdate && _ResponsibleUser != null) return _ResponsibleUser;

            #region Define instruction 
            // Detecting routing instruction suitable for user's queries.
            BaseClient.routingTable.TryGetRoutingInstruction(
                new UniformQueries.Query(
                    new UniformQueries.QueryPart("info"),
                    new UniformQueries.QueryPart("get"),
                    new UniformQueries.QueryPart("user"),
                    new UniformQueries.QueryPart("token"),
                    new UniformQueries.QueryPart("guid")),
                out Instruction instruction);

            if (!(instruction is PartialAuthorizedInstruction pai)) return null;
            #endregion

            #region Define token
            string token = null;
            // If instruction is full authorized.
            if (instruction is AuthorizedInstruction authorizedInstruction &&
                authorizedInstruction.IsFullAuthorized)
            {
                // Apply full authorized token.
                token = authorizedInstruction.AuthorizedToken;
            }
            else
            {
                // Authorize if not authorized yet.
                if (!pai.IsPartialAuthorized)
                {
                    await pai.TryToGetGuestTokenAsync(BaseClient.TerminationTokenSource.Token);
                }
                token = pai.GuestToken;
            }
            #endregion

            #region Receive data
            // Query that was received from query.
            UniformQueries.Query receivedQuery = null;

            // Requies user profile from server.
            BaseClient.EnqueueDuplexQueryViaPP(
                // Define routing.
                instruction.routingIP, instruction.pipeName,
                // Building query.
                new UniformQueries.Query(
                    new UniformQueries.Query.EncryptionInfo(),
                    new UniformQueries.QueryPart("token", token),
                    new UniformQueries.QueryPart("guid", "GetBuilingContacts"),
                    new UniformQueries.QueryPart("user", new DatumPoint.Types.Personality.User() { id = responsibleUserId }),
                    new UniformQueries.QueryPart("get"),
                    new UniformQueries.QueryPart("info")),
                // Managing answer from server.
                delegate (TransmissionLine tl, UniformQueries.Query answer)
                {
                    receivedQuery = answer;
                });

            // Wait till answer from server.
            while (receivedQuery == null)
            {
                await Task.Delay(5);
            }
            #endregion

            #region Decode data
            try
            {
                // Trying to get user data from answer.
                _ResponsibleUser = UniformDataOperator.Binary.BinaryHandler.
                FromByteArray<DatumPoint.Types.Personality.User>(receivedQuery.First.propertyValue);
            }
            catch
            {
                // Set empty user to log error.
                //_ResponsibleUser = new DatumPoint.Types.Personality.User()
                //{ firstName = "User not found" };
            }
            #endregion

            return _ResponsibleUser;
        }
        #endregion
    }
}
