﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using XboxUpdateTool.Models;

namespace XboxUpdateTool
{
    class Program
    {

        static void Main(string[] args)
        {
            if(args.Length < 2)
            {
                Console.WriteLine("Usage: XboxUpdateTool.exe xtoken contentid");
                return;
            }

            string xtoken = String.Empty;
            if (!System.IO.File.Exists(args[0]))
            {
                xtoken = args[0];
            }
            else
            {
                xtoken = System.IO.File.ReadAllText(args[0]);
            }

            var client = new DurangoSystemupdateClient(xtoken);
            CallIsUpdateAvailable(client, args[1], args[2]).GetAwaiter().GetResult();
        }
        
        static void UpdateInfo(UpdateXboxLive update)
        {
            Console.WriteLine($"Update Version: {update.TargetVersionId}\nUpdate Type: {update.UpdateType}");
        }

        static async Task DownloadUpdateAsync(UpdateXboxLive update)
        {
            WebClient downloader = new WebClient();

            Console.WriteLine($"Downloading Update: {update.TargetVersionId} - Size: {update.Files.EstimatedTotalDownloadSize} KB");
            foreach(var file in update.Files.PurpleFiles)
            {
                Console.WriteLine($"Downloading {file.Name} - {file.Size} kb");
                Uri updatepath = new Uri($"http://assets1.xboxlive.com/{file.RelativeUrl}");
                await downloader.DownloadFileTaskAsync(updatepath, file.Name + "_" + update.ContentId);
                Console.WriteLine($"Downloaded {file.Name} as {file.Name}_{update.ContentId}");
            }  
        }

        static async Task CallGetSystemUpdatePackage(DurangoSystemupdateClient client, string contentId)
        {
            var httpResponse = await client.GetSystemUpdatePackage(contentId);
            if (httpResponse.IsSuccessStatusCode)
            {
                string update = await httpResponse.Content.ReadAsStringAsync();
                Console.WriteLine(update);

                var updatejson = UpdateXboxLive.FromJson(update);
                if (updatejson.UpdateType != "None")
                {
                    Console.WriteLine("***Update Found***");
                    UpdateInfo(updatejson);
                    await DownloadUpdateAsync(updatejson);
                }
                else
                {
                    Console.WriteLine("No Update Found.");
                }

            }
            else
            {
                Console.WriteLine($"update.xboxlive.com error - status code {httpResponse.StatusCode.ToString()}");
            }
        }

        static async Task CallGetSpecificSystemVersion(DurangoSystemupdateClient client,
            string unk1, string unk2, string unk3)
        {
            var httpResponse = await client.GetSpecificSystemVersion(unk1, unk2, unk3);
            if (httpResponse.IsSuccessStatusCode)
            {
                string update = await httpResponse.Content.ReadAsStringAsync();
                Console.WriteLine(update);

                var updatejson = UpdateXboxLive.FromJson(update);
                if (updatejson.UpdateType != "None")
                {
                    Console.WriteLine("***Update Found***");
                    UpdateInfo(updatejson);
                    await DownloadUpdateAsync(updatejson);
                }
                else
                {
                    Console.WriteLine("No Update Found.");
                }

            }
            else
            {
                Console.WriteLine($"update.xboxlive.com error - status code {httpResponse.StatusCode.ToString()}");
            }
        }

        static async Task CallIsUpdateAvailable(DurangoSystemupdateClient client, string contentId, string versionId)
        {
            var httpResponse = await client.IsUpdateAvailable(contentId, versionId);
            if (httpResponse.IsSuccessStatusCode)
            {
                string update = await httpResponse.Content.ReadAsStringAsync();
                Console.WriteLine(update);
            }
            else
            {
                Console.WriteLine($"update.xboxlive.com error - status code {httpResponse.StatusCode.ToString()}");
            }
        }
    }
}
