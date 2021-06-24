using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Client.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Client.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public IList<FileModel> Files { get; set; } = new List<FileModel>();

        public IndexModel(
            IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task OnGet()
        {
            var blobServiceClient =
                new BlobServiceClient(_configuration["AZURE_STORAGE_CONNECTION_STRING"]);

            BlobContainerClient blobContainerClient =
                blobServiceClient.GetBlobContainerClient("images");

            Uri uri = null;

            await foreach (var blobItem in blobContainerClient.GetBlobsAsync())
            {
                if (_configuration["SAS_GENERATION_METHOD"] == "logicapp")
                {
                    var url = _configuration["AZURE_LOGIC_APP_URL"];

                    using HttpClient client = new HttpClient();
                    using HttpResponseMessage res = await client.GetAsync(string.Format(_configuration["AZURE_LOGIC_APP_URL"], blobItem.Name));
                    using HttpContent content = res.Content;
                    uri = new Uri(await content.ReadAsStringAsync());
                }
                else
                {
                    BlobClient blobClient = blobContainerClient.GetBlobClient(blobItem.Name);

                    BlobSasBuilder blobSasBuilder = new BlobSasBuilder()
                    {
                        BlobContainerName = blobContainerClient.Name,
                        BlobName = blobItem.Name
                    };

                    if (string.IsNullOrWhiteSpace(_configuration["AZURE_STORAGE_STORED_POLICY_NAME"]))
                    {
                        blobSasBuilder.StartsOn = DateTime.UtcNow;
                        blobSasBuilder.ExpiresOn = DateTime.UtcNow.AddMinutes(Convert.ToInt32(_configuration["AZURE_STORAGE_SAS_TOKEN_DURATION"]));
                        blobSasBuilder.SetPermissions(BlobSasPermissions.Read);
                    }
                    else
                    {
                        blobSasBuilder.Identifier = _configuration["AZURE_STORAGE_STORED_POLICY_NAME"];
                    }

                    uri = blobClient.GenerateSasUri(blobSasBuilder);
                }

                this.Files.Add(new FileModel() { BlobItem = blobItem, Uri = uri });
            }
        }
    }
}
