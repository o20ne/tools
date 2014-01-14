using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

/// <summary>
/// Summary description for TempIdWithRecordCount
/// </summary>
[DataContract()]
public class TempIdWithRecordCount
{
public long Id { get; set; }
public int RecordCount { get; set; }
}