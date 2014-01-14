using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace PersonalAzure
{
  public class AzureBlob
  {
    private CloudBlobClient blobClient;
    private CloudBlobContainer container;

    public AzureBlob()
    {
      CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
      blobClient = storageAccount.CreateCloudBlobClient();
      container = blobClient.GetContainerReference(CloudConfigurationManager.GetSetting("StorageBlobContainer"));
      container.CreateIfNotExist();
      container.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
    }

    public string Host { get { return container.Uri.Host; } }
    public string AbsolutePath { get { return container.Uri.AbsolutePath; } }
    public string DnsSafeHost { get { return container.Uri.DnsSafeHost; } }
    public string AbsoluteUri { get { return container.Uri.AbsoluteUri; } }

    /*
    // Summary:
    //     Upload file to Azure Blob Storage
    //
    // Parameters:
    //   strDirectory:
    //     Directory structure for blob
    //
    //   strFilePaths:
    //     Paths of file locally to be uploaded
    public void CreateBlob(string strDirectory, string[] strFilePaths)
    {
      foreach (string strFilePath in strFilePaths)
      {
        if (File.Exists(strFilePath))
        {
          string blobName = Path.GetFileName(strFilePath);
          CloudBlob cloudBlob = container.GetBlobReference(strDirectory.Trim('/') + "/" + blobName);

          cloudBlob.UploadFile(strFilePath);
          cloudBlob.Metadata["Filename"] = Path.GetFileName(strFilePath);
          cloudBlob.SetMetadata();
        }
      }
    }
    */

    // Summary:
    //     Upload file to Azure Blob Storage
    //
    // Parameters:
    //   strDirectory:
    //     Directory structure for blob
    //
    //   strFilePaths:
    //     Paths of file locally to be uploaded
    //
    //   strContentTypes:
    //     Content Type of files
    public void CreateBlob(string strDirectory, string[] strFilePaths, string[] strContentTypes)
    {
      int intCount = 0;
      foreach (string strFilePath in strFilePaths)
      {
        if (File.Exists(strFilePath))
        {
          string blobName = Path.GetFileName(strFilePath);
          CloudBlob cloudBlob = container.GetBlobReference(strDirectory.Trim('/') + "/" + blobName);

          cloudBlob.UploadFile(strFilePath);
          cloudBlob.Properties.ContentType = strContentTypes[intCount];
          cloudBlob.Metadata["Filename"] = Path.GetFileName(strFilePath);
          cloudBlob.SetMetadata();
          cloudBlob.SetProperties();
        }

        intCount++;
      }
    }

    // Summary:
    //     Upload file to Azure Blob Storage
    //
    // Parameters:
    //   strDirectory:
    //     Directory structure for blob
    //
    //   fileStream:
    //     File stream of uploaded file
    //
    //   blobName:
    //     File name of uploaded file
    public void CreateBlob(string strDirectory, Stream fileStream, string blobName)
    {
      CloudBlob cloudBlob = container.GetBlobReference(strDirectory.Trim('/') + "/" + blobName);

      cloudBlob.UploadFromStream(fileStream);
      cloudBlob.Metadata["Filename"] = blobName;
      cloudBlob.SetMetadata();
    }

    // Summary:
    //     Read Azure Blob Storage as byte[]
    //
    // Parameters:
    //   strPathBlob:
    //     Path & File name of uploaded file
    public byte[] RetrieveBlob(string strPathBlob)
    {
      CloudBlob cloudBlob = container.GetBlobReference(strPathBlob);

      MemoryStream ms = new MemoryStream();
      cloudBlob.DownloadToStream(ms);
      ms.Seek(0, SeekOrigin.Begin);
      byte[] byteData = ms.ToArray();
      ms.Dispose();

      return byteData;
    }
  }

}