using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace PersonalAzure
{
  public class QueueManager
  {
    public string QueueName { get; set; }

    private string StorageConnectionString
    {
      get {
        return CloudConfigurationManager.GetSetting("StorageConnectionString");
      }
    }

    public int CountMessage()
    {
      return RetrieveMessageCount();
    }

    private CloudQueue GetQueue()
    {
      var storageAccount = CloudStorageAccount.Parse(StorageConnectionString);
      var queueClient = storageAccount.CreateCloudQueueClient();
      var queue = queueClient.GetQueueReference(this.QueueName);
      try
      {
        queue.CreateIfNotExist();
      }
      catch (Exception ex)
      { }
      return queue;
    }

    public void AddMessage(string strQueueMessage)
    {
      var queue = GetQueue();
      var message = new CloudQueueMessage(strQueueMessage);
      queue.AddMessage(message);
    }

    public CloudQueueMessage GetMessage()
    {
      var queue = GetQueue();
      return queue.GetMessage();
    }

    public void RemoveLastMessage()
    {
      var queue = GetQueue();
      var retrievedMessage = queue.GetMessage();
      queue.DeleteMessage(retrievedMessage);
    }

    public void RemoveMessage(CloudQueueMessage objMessage)
    {
      var queue = GetQueue();
      queue.DeleteMessage(objMessage);
    }

    public void Clear()
    {
      var queue = GetQueue();

      queue.Clear();
    }

    public int RetrieveMessageCount()
    {
      var queue = GetQueue();
      return queue.RetrieveApproximateMessageCount();
    }
  }
}