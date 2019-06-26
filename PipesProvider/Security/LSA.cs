﻿//Copyright 2019 Volodymyr Podshyvalov
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
using System.Security.Principal;
using System.Runtime.InteropServices;
using System.Security;
using System.ComponentModel;

namespace PipesProvider.Security.LSA
{
    using LSA_HANDLE = IntPtr;

    [StructLayout(LayoutKind.Sequential)]
    internal struct LSA_OBJECT_ATTRIBUTES
    {
        internal int Length;
        internal IntPtr RootDirectory;
        internal IntPtr ObjectName;
        internal int Attributes;
        internal IntPtr SecurityDescriptor;
        internal IntPtr SecurityQualityOfService;
    }

    /// 
    /// LSA_UNICODE_STRING structure
    /// 
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct LSA_UNICODE_STRING
    {
        internal ushort Length;
        internal ushort MaximumLength;
        [MarshalAs(UnmanagedType.LPWStr)] internal string Buffer;
    }

    
    /// <summary>
    /// Provide warped way to Add and Rmove rights for user\groups\domains from LSA.
    /// </summary>
    public sealed class LsaSecurityWrapper
    {
        #region DLL Import
        [DllImport("advapi32", CharSet = CharSet.Unicode, SetLastError = true),
         SuppressUnmanagedCodeSecurityAttribute]
        internal static extern uint LsaOpenPolicy(
            LSA_UNICODE_STRING[] SystemName,
            ref LSA_OBJECT_ATTRIBUTES ObjectAttributes,
            int AccessMask,
            out IntPtr PolicyHandle
            );

        [DllImport("advapi32", CharSet = CharSet.Unicode, SetLastError = true),
         SuppressUnmanagedCodeSecurityAttribute]
        internal static extern uint LsaAddAccountRights(
            LSA_HANDLE PolicyHandle,
            IntPtr pSID,
            LSA_UNICODE_STRING[] UserRights,
            int CountOfRights
            );

        [DllImport("advapi32", CharSet = CharSet.Unicode, SetLastError = true),
         SuppressUnmanagedCodeSecurityAttribute]
        internal static extern uint LsaRemoveAccountRights(
            LSA_HANDLE PolicyHandle,
            IntPtr AccountSid,
            bool AllRights,
            LSA_UNICODE_STRING[] UserRights,
            int CountOfRights
            );

        [DllImport("advapi32")]
        internal static extern int LsaClose(IntPtr PolicyHandle);
        #endregion

        private enum Access : int
        {
            POLICY_READ = 0x20006,
            POLICY_ALL_ACCESS = 0x00F0FFF,
            POLICY_EXECUTE = 0X20801,
            POLICY_WRITE = 0X207F8
        }

        #region API
        /// <summary>
        /// Add rights for requested user or group to LSA.
        /// https://docs.microsoft.com/en-us/windows/security/threat-protection/security-policy-settings/user-rights-assignment
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="rights"></param>        
        public static void AddAccountRights(SecurityIdentifier sid, string rights)
        {
            IntPtr lsaHandle;

            LSA_UNICODE_STRING[] system = null;
            LSA_OBJECT_ATTRIBUTES lsaAttr;
            lsaAttr.RootDirectory = IntPtr.Zero;
            lsaAttr.ObjectName = IntPtr.Zero;
            lsaAttr.Attributes = 0;
            lsaAttr.SecurityDescriptor = IntPtr.Zero;
            lsaAttr.SecurityQualityOfService = IntPtr.Zero;
            lsaAttr.Length = Marshal.SizeOf(typeof(LSA_OBJECT_ATTRIBUTES));
            lsaHandle = IntPtr.Zero;

            uint ret = LsaOpenPolicy(system, ref lsaAttr, (int)Access.POLICY_ALL_ACCESS, out lsaHandle);
            if (ret == 0)
            {
                Byte[] buffer = new Byte[sid.BinaryLength];
                sid.GetBinaryForm(buffer, 0);

                IntPtr pSid = Marshal.AllocHGlobal(sid.BinaryLength);
                Marshal.Copy(buffer, 0, pSid, sid.BinaryLength);

                LSA_UNICODE_STRING[] privileges = new LSA_UNICODE_STRING[1];

                LSA_UNICODE_STRING lsaRights = new LSA_UNICODE_STRING();
                lsaRights.Buffer = rights;
                lsaRights.Length = (ushort)(rights.Length * sizeof(char));
                lsaRights.MaximumLength = (ushort)(lsaRights.Length + sizeof(char));

                privileges[0] = lsaRights;

                ret = LsaAddAccountRights(lsaHandle, pSid, privileges, 1);

                LsaClose(lsaHandle);

                Marshal.FreeHGlobal(pSid);

                if (ret != 0)
                {
                    throw new Win32Exception("LsaAddAccountRights failed with error code: " + ret);
                }
            }
            else
            {
                throw new Win32Exception("LsaOpenPolicy failed with error code: " + ret);
            }
        }

        /// <summary>
        /// Remove rights for requested user or group from LSA.
        /// https://docs.microsoft.com/en-us/windows/security/threat-protection/security-policy-settings/user-rights-assignment
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="rights"></param>
        // https://docs.microsoft.com/en-us/windows/security/threat-protection/security-policy-settings/user-rights-assignment
        public static void RemoveAccountRights(SecurityIdentifier sid, string rights)
        {
            IntPtr lsaHandle;

            LSA_UNICODE_STRING[] system = null;
            LSA_OBJECT_ATTRIBUTES lsaAttr;
            lsaAttr.RootDirectory = IntPtr.Zero;
            lsaAttr.ObjectName = IntPtr.Zero;
            lsaAttr.Attributes = 0;
            lsaAttr.SecurityDescriptor = IntPtr.Zero;
            lsaAttr.SecurityQualityOfService = IntPtr.Zero;
            lsaAttr.Length = Marshal.SizeOf(typeof(LSA_OBJECT_ATTRIBUTES));
            lsaHandle = IntPtr.Zero;

            uint ret = LsaOpenPolicy(system, ref lsaAttr, (int)Access.POLICY_ALL_ACCESS, out lsaHandle);
            if (ret == 0)
            {
                Byte[] buffer = new Byte[sid.BinaryLength];
                sid.GetBinaryForm(buffer, 0);

                IntPtr pSid = Marshal.AllocHGlobal(sid.BinaryLength);
                Marshal.Copy(buffer, 0, pSid, sid.BinaryLength);

                LSA_UNICODE_STRING[] privileges = new LSA_UNICODE_STRING[1];

                LSA_UNICODE_STRING lsaRights = new LSA_UNICODE_STRING();
                lsaRights.Buffer = rights;
                lsaRights.Length = (ushort)(rights.Length * sizeof(char));
                lsaRights.MaximumLength = (ushort)(lsaRights.Length + sizeof(char));

                privileges[0] = lsaRights;

                ret = LsaRemoveAccountRights(lsaHandle, pSid, false, privileges, 1);

                LsaClose(lsaHandle);

                Marshal.FreeHGlobal(pSid);

                if (ret != 0)
                {
                    throw new Win32Exception("LsaAddAccountRights failed with error code: " + ret);
                }
            }
            else
            {
                throw new Win32Exception("LsaOpenPolicy failed with error code: " + ret);
            }
        }
        #endregion
    }
}