// Source: https://github.com/blgrossMS-zz/xbox-live-api/blob/17c586336e11f0fa3a2a3f3acd665b18c5487b24/Source/System/auth/signature_policy.cpp

using System;
using System.Collections.Generic;

namespace XboxUpdateTool
{
    public class SignaturePolicy
    {
        public int Version { get; }
        public int MaxBodyBytes { get; }
        public IEnumerable<string> ExtraHeaders { get; }

        public SignaturePolicy(int version, int maxBodyBytes, IEnumerable<string> extraHeaders)
        {
            Version = version;
            MaxBodyBytes = maxBodyBytes;
            ExtraHeaders = extraHeaders;
        }
    }
}