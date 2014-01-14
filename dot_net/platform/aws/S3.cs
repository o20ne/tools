/***
* Last edited Nov 2012, should have better ways to do this
***/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.IO;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Amazon.S3.Transfer;

namespace PersonalAws
{
  public class S3
  {
    private String mAWSKeyId = ConfigurationManager.AppSettings["AWSAccessKey"];
    private String mAWSKeySecret = ConfigurationManager.AppSettings["AWSSecretKey"];
    private String mAWSBucketName = ConfigurationManager.AppSettings["AWSS3BucketName"];
    private String mAWSServiceUrl = ConfigurationManager.AppSettings["AWSS3ServiceUrl"];

    static AmazonS3 client;

    // Summary:
    //     Initialize Amazon S3 Bucket
    //     View region @ http://docs.aws.amazon.com/general/latest/gr/rande.html#s3_region
    public S3()
    {
      AmazonS3Config S3Config = new AmazonS3Config
      {
        ServiceURL = mAWSServiceUrl,
        CommunicationProtocol = Amazon.S3.Model.Protocol.HTTPS
      };
      client = Amazon.AWSClientFactory.CreateAmazonS3Client(mAWSKeyId, mAWSKeySecret, S3Config);
    }

    // Summary:
    //     List files in Amazon S3 Bucket
    //
    // Parameters:
    //   strPrefix:
    //     Prefix for paths & files
    public List<string> ListFiles(string strPrefix)
    {
      List<string> listFiles = new List<string>();
      
      ListObjectsRequest request = new ListObjectsRequest();
      request = new ListObjectsRequest();
      if(!string.IsNullOrEmpty(strPrefix))
      {
        request.Prefix = strPrefix;
      }
      request.BucketName = mAWSBucketName;

      do
      {
        ListObjectsResponse response = client.ListObjects(request);

        // Process response.
        foreach (S3Object entry in response.S3Objects)
        {
          listFiles.Add(entry.Key);
          //Console.WriteLine("key = {0} size = {1}", entry.Key, entry.Size);
        }

        // If response is truncated, set the marker to get the next 
        // set of keys.
        if (response.IsTruncated)
        {
          request.Marker = response.NextMarker;
        }
        else
        {
          request = null;
        }
      } while (request != null);

      return listFiles;
    }


    // Summary:
    //     Upload file to Amazon S3 Bucket
    //
    // Parameters:
    //   sourcePath:
    //     File path of uploaded file
    //
    //   filePath:
    //     Path of file in S3 Bucket
    public void UploadFile(string sourcePath, string filePath)
    {
      TransferUtility transfer = new TransferUtility(client);

      TransferUtilityUploadRequest request = new TransferUtilityUploadRequest();
      request.WithTimeout(10 * 60 * 1000)
          .WithBucketName(mAWSBucketName)
          .WithKey(filePath)
          .WithAutoCloseStream(true)
          .WithFilePath(sourcePath)
          .AddHeader("x-amz-acl", "public-read");

      transfer.Upload(request);
    }

    // Summary:
    //     Upload file to Amazon S3 Bucket
    //
    // Parameters:
    //   fileStream:
    //     File stream of uploaded file
    //
    //   filePath:
    //     Path of file in S3 Bucket
    public void UploadFile(Stream fileStream, string filePath)
    {
      TransferUtility transfer = new TransferUtility(client);

      TransferUtilityUploadRequest request = new TransferUtilityUploadRequest();
      request.WithTimeout(10 * 60 * 1000)
          .WithBucketName(mAWSBucketName)
          .WithKey(filePath)
          .WithAutoCloseStream(true)
          .WithInputStream(fileStream)
          .AddHeader("x-amz-acl", "public-read");

      transfer.Upload(request);
    }

    // Summary:
    //     Upload file to Amazon S3 Bucket
    //
    // Parameters:
    //   fileStream:
    //     File stream of uploaded file
    //
    //   filePath:
    //     Path of file in S3 Bucket
    //
    //   boolAutoCloseStream:
    //     Boolean to determine if file stream should be auto close
    public void UploadFile(Stream fileStream, string filePath, bool boolAutoCloseStream)
    {
      TransferUtility transfer = new TransferUtility(client);

      TransferUtilityUploadRequest request = new TransferUtilityUploadRequest();
      request.WithTimeout(10 * 60 * 1000)
          .WithBucketName(mAWSBucketName)
          .WithKey(filePath)
          .WithAutoCloseStream(boolAutoCloseStream)
          .WithInputStream(fileStream)
          .AddHeader("x-amz-acl", "public-read");


      transfer.Upload(request);
    }

    // Summary:
    //     Upload file to Amazon S3 Bucket
    //
    // Parameters:
    //   fileStream:
    //     File stream of uploaded file
    //
    //   filePath:
    //     Path of file in S3 Bucket
    //
    //   contentType:
    //     Content Type for target file
    //     Default to auto detect by Amazon
    public void UploadFile(Stream fileStream, string filePath, string contentType)
    {
      TransferUtility transfer = new TransferUtility(client);

      TransferUtilityUploadRequest request = new TransferUtilityUploadRequest();
      request.WithTimeout(10 * 60 * 1000)
          .WithContentType(contentType)
          .WithBucketName(mAWSBucketName)
          .WithKey(filePath)
          .WithAutoCloseStream(true)
          .WithInputStream(fileStream)
          .AddHeader("x-amz-acl", "public-read");

      // Add a header to the request.
      request.AddHeaders(AmazonS3Util.CreateHeaderEntry("ContentType", contentType));

      transfer.Upload(request);
    }

    // Summary:
    //     Upload file to Amazon S3 Bucket
    //
    // Parameters:
    //   fileStream:
    //     File stream of uploaded file
    //
    //   filePath:
    //     Path of file in S3 Bucket
    //
    //   contentType:
    //     Content Type for target file
    //     Default to auto detect by Amazon
    //
    //   boolAutoCloseStream:
    //     Boolean to determine if file stream should be auto close
    public void UploadFile(Stream fileStream, string filePath, string contentType, bool boolAutoCloseStream)
    {
      TransferUtility transfer = new TransferUtility(client);

      TransferUtilityUploadRequest request = new TransferUtilityUploadRequest();
      request.WithTimeout(10 * 60 * 1000)
          .WithContentType(contentType)
          .WithBucketName(mAWSBucketName)
          .WithKey(filePath)
          .WithAutoCloseStream(boolAutoCloseStream)
          .WithInputStream(fileStream)
          .AddHeader("x-amz-acl", "public-read");

      // Add a header to the request.
      request.AddHeaders(AmazonS3Util.CreateHeaderEntry("ContentType", contentType));

      transfer.Upload(request);
    }

    // Summary:
    //     Update file on Amazon S3 Bucket with Headers
    //
    // Parameters:
    //   filePath:
    //     Path of file in S3 Bucket
    public void UpdateFile(string filePath)
    {
      CopyObjectRequest request = new Amazon.S3.Model.CopyObjectRequest()
                                 .WithDirective(Amazon.S3.Model.S3MetadataDirective.REPLACE)
                                 .WithSourceBucket(mAWSBucketName)
                                 .WithSourceKey(filePath)
                                 .WithDestinationBucket(mAWSBucketName)
                                 .WithDestinationKey(filePath);

      request.AddHeader("x-amz-acl", "public-read");

      client.CopyObject(request);
    }
  }
}