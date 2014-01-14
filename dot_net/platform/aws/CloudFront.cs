/***
* Last edited Nov 2012, should have better ways to do this
***/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using Amazon.CloudFront;
using Amazon.CloudFront.Model;

namespace PersonalAws
{
  public class CloudFront
  {
    private String mAWSKeyId = ConfigurationManager.AppSettings["AWSAccessKey"];
    private String mAWSKeySecret = ConfigurationManager.AppSettings["AWSSecretKey"];
    private String mAWSCloudFrontId = ConfigurationManager.AppSettings["AWSCloudfrontId"];
    static AmazonCloudFront client;

    public CloudFront()
    {
      client = Amazon.AWSClientFactory.CreateAmazonCloudFrontClient(mAWSKeyId, mAWSKeySecret);
    }

    // Summary:
    //     Invalidate Amazon CloudFront files
    //
    // Parameters:
    //   strPathsInput:
    //     The file paths to invalidate
    //     Seperated by line breaks
    public void Invalidate(string strPathsInput)
    {
      CreateInvalidationRequest createInvalidationRequest = new CreateInvalidationRequest();
      createInvalidationRequest.DistributionId = mAWSCloudFrontId;
      
      List<string> listFiles = strPathsInput.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).ToList();
      List<string> tempListFiles = new List<string>();

      String strRandom = PersonalTools.CreateRandomString(50);
      
      while(listFiles.Count > 1000)
      {
        Paths paths = new Paths();
        paths.WithQuantity(tempListFiles.Count);
        paths.WithItems(tempListFiles);

        strRandom = PersonalTools.CreateRandomString(10);
        tempListFiles = listFiles.GetRange(0, 1000);
        createInvalidationRequest.InvalidationBatch = new InvalidationBatch();
        createInvalidationRequest.InvalidationBatch.CallerReference = strRandom;
        createInvalidationRequest.InvalidationBatch.Paths = paths;
        client.CreateInvalidation(createInvalidationRequest);

        listFiles.RemoveRange(0, 1000);
      }

      Paths paths2 = new Paths();
      paths2.WithQuantity(listFiles.Count);
      paths2.WithItems(listFiles);

      strRandom = PersonalTools.CreateRandomString(10);
      createInvalidationRequest.InvalidationBatch = new InvalidationBatch();
      createInvalidationRequest.InvalidationBatch.CallerReference = strRandom;
      createInvalidationRequest.InvalidationBatch.Paths = paths2;
      client.CreateInvalidation(createInvalidationRequest);
    }
  }
}