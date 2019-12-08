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
    /// Class that describes building in corporate infrastructure.
    /// </summary>
    [Serializable]
    [Table("datum-point", "building")]
    public class Building
    {
        #region Fields & properties
        /// <summary>
        /// Unique id of the building in corporate infrastructure.
        /// </summary>
        [Column("buildingid", System.Data.DbType.Int32), IsPrimaryKey, IsNotNull]
        [MySqlDBTypeOverride(MySql.Data.MySqlClient.MySqlDbType.Int32, "INT")]
        public int id = -1;

        /// <summary>
        /// TODO Logo of building in binary format.
        /// </summary>
        [Column("logo", System.Data.DbType.Binary)]
        [MySqlDBTypeOverride(MySql.Data.MySqlClient.MySqlDbType.TinyBlob, "TINYBLOB")]
        public byte[] LogoBlob
        {
            get; set;
        }

        /// <summary>
        /// Description of that builing. 
        /// Include history, as example.
        /// </summary>
        [Column("description", System.Data.DbType.String)]
        [MySqlDBTypeOverride(MySql.Data.MySqlClient.MySqlDbType.VarChar, "VARCHAR(2500)")]
        public string description = null;

        /// <summary>
        /// Bufer that contains data with users' profiles received from server.
        /// </summary>
        [XmlIgnore]
        public List<DatumPoint.Types.Personality.User> _ContactListProfiles;

        /// <summary>
        /// Contats list that contains user ids in binary format.
        /// </summary>
        [XmlIgnore]
        [Column("contactslist", System.Data.DbType.Binary)]
        [MySqlDBTypeOverride(MySql.Data.MySqlClient.MySqlDbType.TinyBlob, "TINYBLOB")]
        public byte[] ContactsListBlob
        {
            get
            {
                if (ContactsIdList == null) return null;
                return UniformDataOperator.Binary.BinaryHandler.ToByteArray(ContactsIdList);
            }
            set
            {
                if (value == null)
                {
                    ContactsIdList = null;
                    return;
                }

                try
                {
                    ContactsIdList = UniformDataOperator.Binary.BinaryHandler.FromByteArray<List<uint>>(value);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("BUilding data damaged. Contact list not defined. Details:" + ex.Message);
                    ContactsIdList = null;
                }
            }
        }

        /// <summary>
        /// List that cotnains id of contact persons.
        /// </summary>
        public List<uint> ContactsIdList;

        /// <summary>
        /// List that contain id's of images files into repository in binary format
        /// </summary>
        public byte[] ImagesIdListBlob
        {
            get
            {
                if (ImagesIdList == null) return null;
                return UniformDataOperator.Binary.BinaryHandler.ToByteArray(ImagesIdList);
            }
            set
            {
                // Skip if contenct not attached.
                if (value == null)
                {
                    ImagesIdList = null;
                    return;
                }

                try
                {
                    // try to get data from binary container.
                    ImagesIdList = UniformDataOperator.Binary.BinaryHandler.FromByteArray<List<int>>(value);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Building data damaged. Images list not defined. Details:" + ex.Message);
                    ImagesIdList = null;
                }
            }
        }

        /// <summary>
        /// List that contains id's of repository files that would be tried to displayed as images.
        /// </summary>
        public List<int> ImagesIdList;
        #endregion

        #region API
        /// <summary>
        /// Contact list that contain of responsible persons.
        /// </summary>
        /// <param name="forceUpdate">Drop current data and load new if true.</param>
        public async Task<List<DatumPoint.Types.Personality.User>> GetContactListProfilesAsync(bool forceUpdate = false)
        {
            // Return existed list if data already downloaded from server.
            if (!forceUpdate && // Skip if force update mode requested.
                _ContactListProfiles != null)
            {
                return _ContactListProfiles;
            }

            // Load user profiles from server.
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

            // Drop if instruction invalid.
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

            #region Requiesting users' profiles
            // Lock changing requiest list.
            lock (ContactsIdList)
            {
                // Requiest all id's
                for (int i = 0; i < ContactsIdList.Count; i++)
                {
                    var userId = ContactsIdList[i];

                    // Skip if user undefined.
                    if(userId == 0)
                    {
                        // Add empty user to list for logging error.
                        _ContactListProfiles.Add(new DatumPoint.Types.Personality.User()
                        {
                            firstName = "User not found"
                        });

                        continue;
                    }

                    // Requies user profile from server.
                    BaseClient.EnqueueDuplexQueryViaPP(
                        // Define routing.
                        instruction.routingIP, instruction.pipeName,
                        // Building query.
                        new UniformQueries.Query(
                            new UniformQueries.Query.EncryptionInfo(),
                            new UniformQueries.QueryPart("token", token),
                            new UniformQueries.QueryPart("guid", "GetBuilingContacts"),
                            new UniformQueries.QueryPart("user", new DatumPoint.Types.Personality.User() { id = userId }),
                            new UniformQueries.QueryPart("get"),
                            new UniformQueries.QueryPart("info")),
                        // Managing answer from server.
                        delegate (TransmissionLine tl, UniformQueries.Query answer)
                        {
                            try
                            {
                                // Trying to get user data from answer.
                                var receiverUser = UniformDataOperator.Binary.BinaryHandler.
                                FromByteArray<DatumPoint.Types.Personality.User>(answer.First.propertyValue);

                                // Addign user to the list.
                                _ContactListProfiles.Add(receiverUser);
                            }
                            catch
                            {
                                // Add empty user to list for logging error.
                                _ContactListProfiles.Add(new DatumPoint.Types.Personality.User()
                                {
                                    firstName = "User not found"
                                });
                            }
                        });
                }
            }
            #endregion

            #region Waiting for results
            // Wait for all data.
            while (_ContactListProfiles.Count != ContactsIdList.Count)
            {
                await Task.Delay(5);
            }
            #endregion

            return _ContactListProfiles;
        }
        #endregion
    }
}
