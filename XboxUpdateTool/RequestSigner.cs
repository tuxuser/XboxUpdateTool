// Source: https://github.com/blgrossMS-zz/xbox-live-api/blob/17c586336e11f0fa3a2a3f3acd665b18c5487b24/Source/System/auth/request_signer.h

using System;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Security.Cryptography;
using System.Collections.Generic;

namespace XboxUpdateTool
{
    public static class RequestSigner
    {
        public static string SignRequest(object ecdsaValue,
                                         SignaturePolicy signaturePolicy,
                                         DateTime timestamp,
                                         HttpRequestMessage request)
        {
            var hash = HashRequest(signaturePolicy, timestamp, request);

            byte[] signature = new byte[0x40];
            if (ecdsaValue != null)
            {
                /* TODO: Figure out where ecsda is calculated */
                // signature = ecsdaValue.SignHash(hash);
            }

            if (signature.Length > 0)
            {
                byte[] buffer = new byte[12 + signature.Length];

                var versionBuf = RequestSignerHelpers.GetVersionBuffer(signaturePolicy.Version);
                var timestampBuf = RequestSignerHelpers.GetTimestampBuffer(timestamp);

                Array.Copy(versionBuf, 0, buffer, 0, versionBuf.Length);
                Array.Copy(timestampBuf, 0, buffer, 4, timestampBuf.Length);
                Array.Copy(signature, 0, buffer, 12, signature.Length);

                return Convert.ToBase64String(buffer);
            }

            return String.Empty;
        }

        public static byte[] HashRequest(SignaturePolicy signaturePolicy,
                                          DateTime timestamp,
                                          HttpRequestMessage request)
        {
            int _ = 0;
            SHA256 hasher = SHA256.Create();

            /* Signature policy version & Timestamp */
            var buffer = new byte[14];

            byte[] signatureVersionBuf = RequestSignerHelpers.GetVersionBuffer(signaturePolicy.Version);
            Array.Copy(signatureVersionBuf, 0, buffer, 0, signatureVersionBuf.Length);
            buffer[4] = 0; // null byte after the version

            byte[] timestampBuf = RequestSignerHelpers.GetTimestampBuffer(timestamp);
            Array.Copy(timestampBuf, 0, buffer, 5, timestampBuf.Length);
            buffer[13] = 0; // null byte after the timestamp

            hasher.TransformBlock(buffer, 0, buffer.Length, null, _);

            /* HTTP method */
            var httpMethod = Encoding.UTF8.GetBytes(request.Method.Method + "\0");
            hasher.TransformBlock(httpMethod, 0, httpMethod.Length, null, _);

            /* URL Path & query */
            var urlPathAndQuery = Encoding.UTF8.GetBytes(request.RequestUri.PathAndQuery + "\0");
            hasher.TransformBlock(urlPathAndQuery, 0, urlPathAndQuery.Length, null, _);

            /* Authorization-Header */
            string authorizationHeader = RequestSignerHelpers.GetHeaderOrEmptyString(request, "Authorization");
            var authorizationHeaderBuf = Encoding.UTF8.GetBytes(authorizationHeader + "\0");

            hasher.TransformBlock(authorizationHeaderBuf, 0, authorizationHeader.Length, null, _);

            /* Extra headers */
            foreach (var extra in signaturePolicy.ExtraHeaders)
            {
                var extraHeader = RequestSignerHelpers.GetHeaderOrEmptyString(request, extra);
                var extraHeaderBuf = Encoding.UTF8.GetBytes(extraHeader + "\0");

                hasher.TransformBlock(extraHeaderBuf, 0, extraHeaderBuf.Length, null, _);
            }

            /* HTTP content body */
            var bodyBuf = request.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();
            int numBytesToHash = Math.Min(bodyBuf.Length, signaturePolicy.MaxBodyBytes);

            if (numBytesToHash > 0)
            {
                hasher.TransformBlock(bodyBuf, 0, numBytesToHash, null, _);
            }

            /* Last step: Hash null byte */
            hasher.TransformFinalBlock(new byte[1], 0, 1);

            return hasher.Hash;
        }
    }
}