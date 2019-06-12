using System;
using System.Net.Http;
using System.Threading.Tasks;

using XboxUpdateTool.Models;

namespace XboxUpdateTool
{
    public class DurangoSystemupdateClient
    {
        private readonly HttpClient _httpClient;
        public DurangoSystemupdateClient(string authToken)
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authToken);
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
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
            StringContent content = new StringContent(request.ToJson());
            return _httpClient.PostAsync(IsUpdateAvailableUri, content);
        }

        public Task<HttpResponseMessage> IsUpdateAvailableBatch()
        {
            Uri IsUpdateAvailableBatchUri = new Uri("https://update.xboxlive.com/IsUpdateAvailable/Batch");
            return _httpClient.GetAsync(IsUpdateAvailableBatchUri);
        }

        public Task<HttpResponseMessage> GetSpecificSystemVersion(string unk1, string unk2, string unk3)
        {
            Uri GetSpecificSystemVersionUri = new Uri($"https://update.xboxlive.com/GetSpecificSystemVersion/{unk1}/{unk2}/{unk3}");
            return _httpClient.GetAsync(GetSpecificSystemVersionUri);
        }

        public Task<HttpResponseMessage> GetCacheGroupId()
        {
            Uri GetCacheGroupIDUri = new Uri("https://update.xboxlive.com/GetCacheGroupId");
            return _httpClient.GetAsync(GetCacheGroupIDUri);
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
            StringContent content = new StringContent(request.ToJson());
            return _httpClient.PostAsync(GetSystemUpdatePackageUri, content);
        }
    }
}