// Source: https://github.com/blgrossMS-zz/xbox-live-api/blob/17c586336e11f0fa3a2a3f3acd665b18c5487b24/Source/System/auth/request_signer_helpers.cpp

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace XboxUpdateTool
{
    public static class RequestSignerHelpers
    {
        // Get version in network order / big endian
        public static byte[] GetVersionBuffer(int version)
        {
            return BitConverter.GetBytes(version).Reverse().ToArray();
        }

        // Get timestamp (FILETIME format) in network order / big endian
        public static byte[] GetTimestampBuffer(DateTime dt)
        {
            var filetimeUtc = dt.ToFileTimeUtc();
            return GetTimestampBuffer(filetimeUtc);
        }

        // Get timestamp (FILETIME format) in network order / big endian
        public static byte[] GetTimestampBuffer(long filetime)
        {
            return BitConverter.GetBytes(filetime).Reverse().ToArray();
        }

        public static string GetHeaderOrEmptyString(HttpRequestMessage request, string headerName)
        {
            var headers = request.Headers.GetValues(headerName);
            if (headers.Count() > 0)
                return headers.First();
            
            return String.Empty;
        }
    }
}