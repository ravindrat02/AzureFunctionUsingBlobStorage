using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage.Blob;

namespace AzureFunctionUsingBlobStorage
{
    public static class PersonService
    {
        [FunctionName("CRUD")]
        public static async Task<IActionResult> CRUDUsingBlogStorage(
          [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
          ILogger log, ExecutionContext context)
        {
            bool result = false ,isExist=false;
            dynamic existingData;

            CloudStorageAccount storageAccount = GetCloudStorageAccount(context);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("function-container");
            
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic newData = JsonConvert.DeserializeObject(requestBody);
         
            string Id = newData?.Id;
            isExist = !string.IsNullOrEmpty(Id)?await container.GetBlobReference(Id).ExistsAsync():false;
            if ((object)newData != null)
            {
                if (!string.IsNullOrEmpty(Id) && Guid.TryParse(Id, out _) && isExist)
                {
                    CloudBlob blobs = container.GetBlobReference(Id);

                    existingData = await DownloadFile(container, Id);
                    result = await blobs.DeleteIfExistsAsync();
                    existingData.Name = newData.Name;//? newData?.Name : existingData?.Name;
                    existingData.Content = newData.Content;//newData?.Content ? newData?.Content : existingData?.Content;
                    existingData.LastUpdate = DateTime.Now;
                    existingData.Result = result ? "Record Updated" : "Record Created";

                    existingData = result ? existingData : newData;
                    UploadFile(container, Id, existingData);
                    return new OkObjectResult(existingData);

                }
                else
                {
                    Id = Guid.NewGuid().ToString();

                   // newData = new object();
                    newData.ID = Id;
                    newData.Result = "Record Created";
                    newData.LastUpdate = DateTime.Now;
                    UploadFile(container, Id, newData);

                  

                }
                return new OkObjectResult(newData);
            }
            else
            {
                newData = "Please pass the data as json formet";
                return new OkObjectResult(newData);
            }
        }

        private static CloudStorageAccount GetCloudStorageAccount(ExecutionContext executionContext)
        {
            var config = new ConfigurationBuilder()
                            .SetBasePath(executionContext.FunctionAppDirectory)
                            .AddJsonFile("local.settings.json", true, true)
                            .AddEnvironmentVariables().Build();
          
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(config["CloudStorageAccount"]);
            return storageAccount;
        }
        private static void LoadStreamWithJson(Stream ms, object obj)
        {
            StreamWriter writer = new StreamWriter(ms);
            writer.Write(obj);
            writer.Flush();
            ms.Position = 0;
        }

        private async static void UploadFile(CloudBlobContainer container, string Id, dynamic data)
        {
            CloudBlockBlob blob = container.GetBlockBlobReference(Id);

            var serializeJesonObject = JsonConvert.SerializeObject(new { ID = Id, Name = data?.Name, Content = data?.Content, LastUpdate = DateTime.Now });
            blob.Properties.ContentType = "application/json";

            using (var ms = new MemoryStream())
            { 
                LoadStreamWithJson(ms, serializeJesonObject);
                await blob.UploadFromStreamAsync(ms);
            }
            await blob.SetPropertiesAsync();


        }

        private async static Task<object> DownloadFile(CloudBlobContainer container, string Id)
        {
            CloudBlockBlob cblob = null;
            string file = string.Empty;
            object newData=null;
            using (var stream = new MemoryStream())
            {
               
                    cblob =  container.GetBlockBlobReference(Id);
                    await cblob.DownloadToStreamAsync(stream);
                    file = System.Text.Encoding.UTF8.GetString(stream.ToArray());
                    newData = JsonConvert.DeserializeObject(file);
               
            }

            return newData;

        }


    }
}
