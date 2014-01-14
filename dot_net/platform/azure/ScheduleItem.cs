using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.WindowsAzure.StorageClient;

namespace PersonalAzure
{
  public class ScheduledItem : TableServiceEntity
  {
    public string Message { get; set; }
    public DateTime Time { get; set; }
    public ScheduledItem(string message, DateTime time)
      : base(string.Empty, time.Ticks.ToString("d19") + "_" + Guid.NewGuid().ToString())
    {
      Message = message;
      Time = time;
    }
    public ScheduledItem() { }
  }
}