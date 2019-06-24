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
using System.Threading;
using System.IO;
using System.IO.Pipes;
using System.Security.AccessControl;
using System.Security.Principal;

namespace PipesProvider
{
    /// <summary>
    /// Class that contain methods for working with sequrity systems.
    /// </summary>
    public static class Security
    {
        /// <summary>
        /// Anonymous - not require logon. 
        /// Require Guest user on server.
        /// Require allownce to network access via Guest accounts.
        /// 
        /// RemoteLogon - Require authentication via one of the profile on server.
        /// 
        /// Local - Pipe will be accessed only on the local machine.
        /// 
        /// Administrator - access to pipe will provided only for administrators. By default allowed via remote authentication.
        /// 
        /// Internal - pipe will controlled only be server application and system.
        /// Any external coonection will be blocked.
        /// </summary>
        public enum SecurityLevel
        {
            Anonymous = 2,
            RemoteLogon = 4,
            Local = 8,
            Administrator = 16,
            Internal = 32
        }

        /// <summary>
        /// Configurate pipe squrity relative to requested level.
        ///
        /// You can request more then one level via using format:
        /// SecurityLevel | SecurityLevel | ...
        /// 
        /// Internal level will applyed by default to allow system and application control created pipes.
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public static PipeSecurity GetRulesForLevels(SecurityLevel level)
        {
            // Get core base of rules that euqual Internal level.
            PipeSecurity rules = DefaultInternalPipeScurity;

            string rulesLog = "";

            // Add Anonymous rule
            if (level.HasFlag(SecurityLevel.Anonymous))
            {
                // Add to log.
                rulesLog += (rulesLog.Length > 0 ? " | " : "") + "WorldSid";

                // Add owner rights to control the pipe.
                rules.AddAccessRule(
                    new PipeAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null),
                    PipeAccessRights.ReadWrite, AccessControlType.Allow));
            }

            // Add Authenticated rule
            if (level.HasFlag(SecurityLevel.RemoteLogon))
            {
                // Add to log.
                rulesLog += (rulesLog.Length > 0 ? " | " : "") + "RemoteLogonIdSid";

                // Add owner rights to control the pipe.
                rules.AddAccessRule(
                    new PipeAccessRule(new SecurityIdentifier(WellKnownSidType.RemoteLogonIdSid, null),
                    PipeAccessRights.ReadWrite, AccessControlType.Allow));
            }

            // Add Local rule
            if (level.HasFlag(SecurityLevel.Local))
            {
                // Add to log.
                rulesLog += (rulesLog.Length > 0 ? " | " : "") + "LocalSystemSid";


                // Add owner rights to control the pipe.
                rules.AddAccessRule(
                    new PipeAccessRule(new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null),
                    PipeAccessRights.ReadWrite, AccessControlType.Allow));
            }

            // Add Administrator rule
            if (level.HasFlag(SecurityLevel.Administrator))
            {
                // Add to log.
                rulesLog += (rulesLog.Length > 0 ? " | " : "") + "BuiltinAdministratorsSid";


                // Add owner rights to control the pipe.
                rules.AddAccessRule(
                    new PipeAccessRule(new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null),
                    PipeAccessRights.ReadWrite, AccessControlType.Allow));
            }

            // Show logs.
            Console.WriteLine("APPLIED RULES: system | self | " + rulesLog);

            return rules;
        }

        /// <summary>
        /// Return pipe security situable for internal use.
        /// </summary>
        public static PipeSecurity DefaultInternalPipeScurity
        {
            get
            {
                // Set common sequrity.
                PipeSecurity pipeSecurity = new PipeSecurity();
                
                // Add system rights to control the pipe.
                pipeSecurity.AddAccessRule(
                    new PipeAccessRule("SYSTEM",
                    PipeAccessRights.FullControl, AccessControlType.Allow));

                // Add owner rights to control the pipe.
                pipeSecurity.AddAccessRule(
                 new PipeAccessRule(new SecurityIdentifier(WellKnownSidType.AuthenticatedUserSid, null),
                 PipeAccessRights.FullControl, AccessControlType.Allow));

                return pipeSecurity;
            }
        }
    }
}
