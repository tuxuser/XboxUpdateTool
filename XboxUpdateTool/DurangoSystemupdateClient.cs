using System;
using System.Text;
using System.Net.Http;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

using XboxUpdateTool.Models;
using System.Net;
using System.Security.Cryptography;

namespace XboxUpdateTool
{
    public class DurangoSystemupdateClient
    {
        public readonly SignaturePolicy TokenSignaturePolicy =
            new SignaturePolicy(1, Int32.MaxValue, new List<string>());

        private readonly string _xToken;
        private readonly HttpClient _httpClient;
        private readonly CorrelationVector _correlationVector;
        public DurangoSystemupdateClient(string xToken)
        {
            _xToken = xToken;
            _httpClient = new HttpClient();
            _correlationVector = new CorrelationVector();
            _correlationVector.Init();
        }

        private Task<HttpResponseMessage> SendHttpMessage(HttpRequestMessage message)
        {
            message.Headers.TryAddWithoutValidation("Authorization", $"XBL3.0 x=-;{_xToken}");
            message.Headers.TryAddWithoutValidation("MS-CV", _correlationVector.Increment());

            var signature = RequestSigner.SignRequest(null, TokenSignaturePolicy, DateTime.Now, message);
            if (signature.Length > 0)
                message.Headers.TryAddWithoutValidation("Signature", signature);

            return _httpClient.SendAsync(message);
        }

        public Task<HttpResponseMessage> IsUpdateAvailable(string contentId, string versionId)
        {
            Uri IsUpdateAvailableUri = new Uri("https://update.xboxlive.com/IsUpdateAvailable");
            IsUpdateAvailableRequest request = new IsUpdateAvailableRequest()
            {
                UpdateMode = 3,
                ContentId = contentId,
                VersionId = versionId
            };

            var message = new HttpRequestMessage(HttpMethod.Post, IsUpdateAvailableUri);
            message.Content = new StringContent(request.ToJson(), Encoding.Default, "application/json");
            return SendHttpMessage(message);
        }

        public Task<HttpResponseMessage> IsUpdateAvailableBatch()
        {
            Uri IsUpdateAvailableBatchUri = new Uri("https://update.xboxlive.com/IsUpdateAvailable/Batch");

            var message = new HttpRequestMessage(HttpMethod.Post, IsUpdateAvailableBatchUri);
            message.Content = new StringContent(" ", Encoding.Default, "application/json");
            return SendHttpMessage(message);
        }

        public Task<HttpResponseMessage> GetSpecificSystemVersion(string unk1, string unk2, string unk3)
        {
            Uri GetSpecificSystemVersionUri = new Uri($"https://update.xboxlive.com/GetSpecificSystemVersion/{unk1}/{unk2}/{unk3}");

            var message = new HttpRequestMessage(HttpMethod.Get, GetSpecificSystemVersionUri);
            return SendHttpMessage(message);
        }

        public Task<HttpResponseMessage> GetCacheGroupId()
        {
            Uri GetCacheGroupIdUri = new Uri("https://update.xboxlive.com/GetCacheGroupId");

            var message = new HttpRequestMessage(HttpMethod.Get, GetCacheGroupIdUri);
            return SendHttpMessage(message);
        }

        public Task<HttpResponseMessage> GetSystemUpdatePackage(string contentid)
        {
            Uri GetSystemUpdatePackageUri = new Uri($"https://update.xboxlive.com/GetSystemUpdatePackage/{contentid}");
            GetSystemUpdatePackageRequest request = new GetSystemUpdatePackageRequest()
            {
                UpdateMode = 3,
                LicenseProtocol = 4,
                FileIncludeFilter = new string[]
                {
                    "updater.xvd"
                },
                IgnoreRootLicense = 0
            };

            var message = new HttpRequestMessage(HttpMethod.Post, GetSystemUpdatePackageUri);
            message.Content = new StringContent(request.ToJson(), Encoding.Default, "application/json");
            return SendHttpMessage(message);
        }
    }
}