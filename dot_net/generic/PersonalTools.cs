/***
* Years of personal tools that I usually used
* @todo: split into multiple classes, classifying them
***/

using System;
using System.Collections.Generic;
using System.Web;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Configuration;
using System.Text;
using System.Net;
using System.Drawing.Imaging;
using System.Collections.Specialized;

/// <summary>
/// Summary description for PersonalTools
/// </summary>
public class PersonalTools
{
  public PersonalTools()
  {
    //
    // TODO: Add constructor logic here
    //
  }

  public enum Method { GET, POST };
  public const string YOUTUBE_IMAGE_URL = "http://i.ytimg.com/vi/{0}/0.jpg";
  public const string YOUTUBE_VIDEO_URL = "http://www.youtube.com/watch?v={0}&feature=player_embedded&wmode=opaque";
  public const string YOUTUBE_EMBED_URL = "http://www.youtube.com/embed/{0}?wmode=opaque";
  public const string YOUTUBE_EMBED_CODE = "<iframe src=\"http://www.youtube.com/embed/{0}?wmode=opaque\" frameborder=\"0\" allowfullscreen></iframe>";

  public static string getYoutubeImgByKey(string strKey)
  {
    return String.Format(YOUTUBE_IMAGE_URL, strKey);
  }

  public static string getYoutubeEmbededUrlByKey(string strKey)
  {
    return String.Format(YOUTUBE_EMBED_URL, strKey);
  }

  public static string getYoutubeEmbededCode(string objId, string strWidth, string strHeight, string strKey)
  {
    return String.Format(YOUTUBE_EMBED_CODE, strKey + "\" id=\"" + objId + "\" width=\"" + strWidth + "\" height=\"" + strHeight);
  }

  public static string getYoutubeEmbededCode(string strKey)
  {
    return String.Format(YOUTUBE_EMBED_CODE, strKey);
  }

  public static string getYoutubeUrl(string strKey)
  {
    return String.Format(YOUTUBE_VIDEO_URL, strKey);
  }

  public static int DayOfWeekStringToInt(string strDayOfWeek)
  {
    int intDayOfWeek = -1;
    switch (strDayOfWeek.ToLower().Trim())
    {
      case "mon":
      case "monday":
        intDayOfWeek = 0;
        break;
      case "tue":
      case "tuesday":
        intDayOfWeek = 1;
        break;
      case "wed":
      case "wednesday":
        intDayOfWeek = 2;
        break;
      case "thu":
      case "thursday":
        intDayOfWeek = 3;
        break;
      case "fri":
      case "friday":
        intDayOfWeek = 4;
        break;
      case "sat":
      case "saturday":
        intDayOfWeek = 5;
        break;
      case "sun":
      case "sunday":
        intDayOfWeek = 6;
        break;
    }

    return intDayOfWeek;
  }

  public static String Number2String(int number, bool isCaps)
  {
    Char c = (Char)((isCaps ? 65 : 97) + (number - 1));

    return c.ToString();
  }

  public static string TimeSpanToHhMm(TimeSpan tsValue, string strDelimiter)
  {
    return String.Format("{0:00}", tsValue.Hours) + strDelimiter + String.Format("{0:00}", tsValue.Minutes);
  }

  public static string CleanString(String strToClean)
  {
    return CleanString(strToClean, "");
  }

  public static string TruncateAtWord(string input, int length)
  {
    return TruncateAtWord(input, length, "...");
  }

  public static string TruncateAtWord(string input, int length, string suffix)
  {
    input = StripTagsCharArray(input);
    if (input == null || input.Length < length)
      return input;
    int iNextSpace = input.LastIndexOf(" ", length);

    return string.Format("{0}" + suffix, input.Substring(0, (iNextSpace > 0) ? iNextSpace : length).Trim());
  }

  public static string rnToLineBreak(String strInput)
  {
    String strOutput = "";
    using (StringReader reader = new StringReader(strInput))
    {
      int lineNo = 0;
      string strLine = "";
      string strCleanedLine = "";
      while ((strLine = reader.ReadLine()) != null)
      {
        strCleanedLine = strLine.Trim().ToLower();
        if(lineNo==0)
        {
          strOutput = strLine;
        }
        else if (strCleanedLine.StartsWith("<param")
            || strCleanedLine.StartsWith("<embed")
            || strCleanedLine.StartsWith("<noembed")
            || strCleanedLine.StartsWith("</"))
        {
          strOutput += strLine;
        }
        else
        {
          strOutput += "<br />" + strLine;
        }

        lineNo++;
      }
    }

    return strOutput;
  }

  /// <summary>
  /// Remove HTML tags from string using char array.
  /// Taken from http://www.dotnetperls.com/remove-html-tags
  /// </summary>
  public static string StripTagsCharArray(string source)
  {
    char[] array = new char[source.Length];
    int arrayIndex = 0;
    bool inside = false;

    for (int i = 0; i < source.Length; i++)
    {
      char let = source[i];
      if (let == '<')
      {
        inside = true;
        continue;
      }
      if (let == '>')
      {
        inside = false;
        continue;
      }
      if (!inside)
      {
        array[arrayIndex] = let;
        arrayIndex++;
      }
    }
    return new string(array, 0, arrayIndex);
  }

  public static string CleanString(String strToClean, String ignoredStr)
  {
    strToClean = (strToClean != null ? strToClean.Trim() : "");
    if (!String.IsNullOrEmpty(ignoredStr))
    {
      ignoredStr = "\\" + ignoredStr.Replace("\\", "\\\\");
    }

    strToClean = Regex.Replace(strToClean, @"[^a-zA-Z0-9" + ignoredStr + "]", string.Empty); // Only alphanumeric

    /*
    Regex regex = new Regex("</?(.*)>", RegexOptions.IgnoreCase | RegexOptions.Multiline);
    strToClean = regex.Replace(strToClean, string.Empty);
    strToClean = Regex.Replace(strToClean, @"[^\w\s\.@-]", " ");
    */
    return strToClean.Trim();
  }

  public static string GenerateURL(String strTitle)
  {
    #region Generate SEO Friendly URL based on Title

    // Remove text within ()
    strTitle = Regex.Replace(strTitle, @" ?\(.*?\)", string.Empty);

    //Trim Start and End Spaces.
    strTitle = strTitle.Trim();

    //Trim "-" Hyphen
    strTitle = strTitle.Trim('-');

    strTitle = strTitle.ToLower();
    char[] chars = @"$%#@!*?;:~`+=()[]{}|\'<>,/^&"".".ToCharArray();
    strTitle = strTitle.Replace("c#", "C-Sharp");
    strTitle = strTitle.Replace("vb.net", "VB-Net");
    strTitle = strTitle.Replace("asp.net", "Asp-Net");

    //Replace . with - hyphen
    strTitle = strTitle.Replace(".", "-");

    //Replace Special-Characters
    for (int i = 0; i < chars.Length; i++)
    {
      string strChar = chars.GetValue(i).ToString();
      if (strTitle.Contains(strChar))
      {
        strTitle = strTitle.Replace(strChar, string.Empty);
      }
    }

    //Replace all spaces with one "-" hyphen
    strTitle = strTitle.Replace(" ", "-");

    strTitle = CleanString(strTitle, "-");

    // Limit to 50 chars
    if (strTitle.Length > 50)
    {
      strTitle = strTitle.Remove(50);
    }

    //Replace multiple "-" hyphen with single "-" hyphen.
    while (strTitle.Contains("--"))
    {
      strTitle = strTitle.Replace("--", "-");
    }

    //Run the code again...
    //Trim Start and End Spaces.
    strTitle = strTitle.Trim();

    //Trim "-" Hyphen
    strTitle = strTitle.Trim('-');
    #endregion

    String[] aryStrUnsafeStubs = ConfigurationManager.AppSettings["unsafe_seo_name"].Split(',');
    foreach(String strUnsafeStubs in aryStrUnsafeStubs)
    {
      if (strTitle.ToLower().Equals(strUnsafeStubs.Trim().ToLower()))
      {
        strTitle = strTitle + "-page";
      }
    }

    return strTitle;
  }

  public static string SHAHash(string valueToHash, string salt)
  {
    SHA1 hasher = SHA1.Create();
    Byte[] valueToHashAsByte = System.Text.Encoding.UTF8.GetBytes(string.Concat(valueToHash, salt));
    Byte[] returnBytes = hasher.ComputeHash(valueToHashAsByte);
    hasher.Clear();

    return Convert.ToBase64String(returnBytes);
  }

  /** C# SHA1 to match PHP SHA1 taken from http://stackoverflow.com/questions/790232/c-sha-1-vs-php-sha-1-different-results **/
  public static string PhpSha1Hash(string valueToHash)
  {
    string strHashedValue = "";

    ASCIIEncoding encoding = new ASCIIEncoding();
    byte[] HashValue, MessageBytes = encoding.GetBytes(valueToHash);
    SHA1Managed SHhash = new SHA1Managed();
    HashValue = SHhash.ComputeHash(MessageBytes);
    foreach (byte b in HashValue)
    {
      strHashedValue += String.Format("{0:x2}", b);
    }

    return strHashedValue;
  }

  public static String getIpAddress()
  {
    HttpRequest Request = HttpContext.Current.Request;
    String ipAddress = "";

    ipAddress = String.IsNullOrEmpty(Request.ServerVariables["HTTP_X_FORWARDED_FOR"]) ? Request.ServerVariables["REMOTE_ADDR"] : Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

    return ipAddress;
  }

  public static string GetSiteProtocol()
  {
    string protocol = System.Web.HttpContext.Current.Request.ServerVariables["SERVER_PORT_SECURE"];
    if (protocol == null || protocol == "0")
      protocol = "http://";
    else
      protocol = "https://";

    return protocol;
  }

  public static string GetSiteRoot()
  {
    string port = System.Web.HttpContext.Current.Request.ServerVariables["SERVER_PORT"];
    if (port == null || port == "80" || port == "443")
      port = "";
    else
      port = ":" + port;

    string protocol = GetSiteProtocol();

    string sOut = protocol + System.Web.HttpContext.Current.Request.ServerVariables["SERVER_NAME"] + port + System.Web.HttpContext.Current.Request.ApplicationPath;

    if (sOut.EndsWith("/"))
    {
      sOut = sOut.Substring(0, sOut.Length - 1);
    }

    return sOut;
  }

  public static bool tryParseDate(String strDay, String strMth, String strYr, out DateTime dateDob)
  {
    return tryParseDate(strDay, strMth, strYr, "00", "00", out dateDob);
  }

  public static bool tryParseDate(String strDay, String strMth, String strYr, String strHr, String strMin, out DateTime dateDob)
  {
    bool isValid = false;
    dateDob = DateTime.Now;
    try
    {
      int day = int.Parse(strDay);
      int month = int.Parse(strMth);
      int year = int.Parse(strYr);
      int hour = int.Parse(strHr);
      int minute = int.Parse(strMin);
      dateDob = new DateTime(year, month, day, hour, minute, 0);
      isValid = true;
    }
    catch (Exception ex)
    {
      isValid = false;
    }

    return isValid;
  }

  #region DateTime to/from Unix Timestamp
  public static long GetCurrentUnixTimestampMillis()
  {
    DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    return (long)(DateTime.UtcNow - UnixEpoch).TotalMilliseconds;
  }

  public static DateTime DateTimeFromUnixTimestampMillis(long millis)
  {
    DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    return UnixEpoch.AddMilliseconds(millis);
  }

  public static long GetCurrentUnixTimestampSeconds()
  {
    DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    return (long)(DateTime.UtcNow - UnixEpoch).TotalSeconds;
  }

  public static long GetUnixTimestampSeconds(DateTime dt)
  {
    DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    TimeSpan ts = dt - UnixEpoch;
    return (long)ts.TotalSeconds;
  }

  public static DateTime DateTimeFromUnixTimestampSeconds(long seconds)
  {
    DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    return UnixEpoch.AddSeconds(seconds);
  }
  #endregion

  public static bool ValidateSGNric(String strNric)
  {
    bool isValid = true;

    if (strNric.Length != 9)
    {
      isValid = false;
    }
    else
    {
      String[] icArray = new String[9];
      int[] inicArray = new int[9];
      for (int i = 0; i < 9; i++)
      {
        icArray[i] = strNric.Substring(i, 1).ToString();
      }

      inicArray[1] = int.Parse(icArray[1]) * 2;
      inicArray[2] = int.Parse(icArray[2]) * 7;
      inicArray[3] = int.Parse(icArray[3]) * 6;
      inicArray[4] = int.Parse(icArray[4]) * 5;
      inicArray[5] = int.Parse(icArray[5]) * 4;
      inicArray[6] = int.Parse(icArray[6]) * 3;
      inicArray[7] = int.Parse(icArray[7]) * 2;

      int weight = 0;
      for (int i = 1; i < 8; i++)
      {
        weight += inicArray[i];
      }

      int offset = (icArray[0].ToUpper() == "T" || icArray[0].ToUpper() == "G") ? 4 : 0;
      int temp = (offset + weight) % 11;

      String[] st = { "J", "Z", "I", "H", "G", "F", "E", "D", "C", "B", "A" };
      String[] fg = { "X", "W", "U", "T", "R", "Q", "P", "N", "M", "L", "K" };

      String theAlpha = String.Empty;
      if (icArray[0].ToUpper() == "S" || icArray[0].ToUpper() == "T") { theAlpha = st[temp]; }
      else if (icArray[0].ToUpper() == "F" || icArray[0].ToUpper() == "G") { theAlpha = fg[temp]; }


      isValid = icArray[8] != theAlpha ? false : true;
    }

    return isValid;
  }

  public static bool ValidateEmail(String strEmail)
  {
    string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
          @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
          @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
    Regex re = new Regex(strRegex);
    if (re.IsMatch(strEmail))
      return true;
    else
      return false;
  }

  public static bool ValidateSGContactNum(string strContactNum)
  {
    Regex reg = new Regex((@"^[9,8,6]\d{7}$"));

    return reg.IsMatch(strContactNum);
  }

  public static bool ValidateImageFile(HttpPostedFile objPostedFile)
  {
    List<string> lstExtensions = new List<string>() { "jpeg", "jpg", "gif", "png", "bmp" };
    List<string> lstContentType = new List<string>() { "image/gif", "image/jpeg", "image/png", "image/x-png", "image/jpg", "image/pjpeg" };

    return ValidateFileExtension(objPostedFile, lstExtensions);
  }

  public static Boolean ValidateContentType(HttpPostedFile objPostedFile, List<string> lstContentType)
  {
    lstContentType = lstContentType.ConvertAll(d => d.ToLower());
    String strContentType = objPostedFile.ContentType.ToLower(); ;

    return lstContentType.Contains(strContentType);
  }

  public static bool ValidateFileExtension(HttpPostedFile objPostedFile, List<string> lstExtensions)
  {
    // Should add check content-type?
    String ext = Path.GetExtension(objPostedFile.FileName);
    ext = ext.ToLower().Replace(".", "");
    lstExtensions = lstExtensions.ConvertAll(d => d.ToLower());

    return lstExtensions.Contains(ext);
  }

  /*
    strFileMaxSize examples: 1024, 1kb, 2MB, 2097152
  */
  public static bool ValidateFileSize(HttpPostedFile objPostedFile, string strFileMaxSize)
  {
    bool isValid = false;

    strFileMaxSize = strFileMaxSize.Replace(" ", "").ToUpper();

    String strUnit = "";
    String strSize = strFileMaxSize;

    if (strFileMaxSize.Length > 2)
    {
      strUnit = strFileMaxSize.Substring(strFileMaxSize.Length - 2);
      strSize = strFileMaxSize.Substring(0, strFileMaxSize.Length - 2);
    }

    int intMaxSizeByte = 0;

    switch (strUnit)
    {
      case "KB":
        int.TryParse(strSize, out intMaxSizeByte);
        intMaxSizeByte = intMaxSizeByte * 1024;
        break;
      case "MB":
        int.TryParse(strSize, out intMaxSizeByte);
        intMaxSizeByte = intMaxSizeByte * 1024 * 1024;
        break;
      case "GB":
        int.TryParse(strSize, out intMaxSizeByte);
        intMaxSizeByte = intMaxSizeByte * 1024 * 1024 * 1024;
        break;
      default:
        int.TryParse(strFileMaxSize, out intMaxSizeByte);
        break;
    }


    if (intMaxSizeByte > objPostedFile.ContentLength)
    {
      isValid = true;
    }

    return isValid;
  }

  public static void SendMail(bool isHtml, MailAddress toAddress, MailAddress fromAddress, String subject, String body)
  {
    SmtpClient smtpClient = new SmtpClient();
    MailMessage mail = new MailMessage();
    mail.BodyEncoding = System.Text.Encoding.UTF8;
    mail.SubjectEncoding = System.Text.Encoding.UTF8;
    mail.IsBodyHtml = isHtml;
    mail.To.Add(toAddress);
    mail.From = fromAddress;
    mail.Subject = subject;
    mail.Body = body;

    try
    {
      smtpClient.Send(mail);
    }
    catch (Exception mail_ex)
    {
      throw mail_ex;
    }
  }
  /*
  public static void SendMail(bool isHtml, MailAddress toAddress, MailAddress fromAddress, String subject, String body, String strUsername, String strPassword)
  {
    SmtpClient smtpClient = new SmtpClient();
    MailMessage mail = new MailMessage();
    mail.BodyEncoding = System.Text.Encoding.UTF8;
    mail.SubjectEncoding = System.Text.Encoding.UTF8;
    mail.IsBodyHtml = isHtml;
    mail.To.Add(toAddress);
    mail.From = fromAddress;
    mail.Subject = subject;
    mail.Body = body;

    try
    {
      smtpClient.Send(mail);
    }
    catch (Exception mail_ex)
    {
      throw mail_ex;
    }
  }

  public static void SendMail(bool isHtml, MailAddress toAddress, MailAddress fromAddress, String subject, String body)
  {
    string strUsername = ConfigurationManager.AppSettings["sendgrid_username"];
    string strPassword = ConfigurationManager.AppSettings["sendgrid_password"];

    SendMail(isHtml, toAddress, fromAddress, subject, body, strUsername, strPassword);
  }
  */

  public static string CreateRandomString(int StringLength)
  {
    String strCharacterDictionary = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ23456789";

    return CreateRandomString(StringLength, strCharacterDictionary);
  }

  public static string CreateRandomString(int StringLength, String strCharacterDictionary)
  {
    Byte[] randomBytes = new Byte[StringLength];
    RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
    rng.GetBytes(randomBytes);
    char[] chars = new char[StringLength];
    int allowedCharCount = strCharacterDictionary.Length;

    for (int i = 0; i < StringLength; i++)
    {
      chars[i] = strCharacterDictionary[(int)randomBytes[i] % allowedCharCount];
    }

    return new string(chars);
  }

  public static DateTime GetFileLastModifiedTime(string strFile)
  {
    DateTime dtLastModified = DateTime.MinValue;
    
    string strFilePath = strFile;
    if (strFile.Contains("/"))
    {
      strFilePath = HttpContext.Current.Server.MapPath(strFile);
    }

    if (File.Exists(strFilePath))
    {
      dtLastModified = File.GetLastWriteTime(strFilePath);
    }

    return dtLastModified;
  }

  public static void CreateDirectory(String strDirectory)
  {
    String[] aryStrDirectories = strDirectory.Split('\\');
    String strIncrementDirectory = "";
    foreach (String strSingleDirectory in aryStrDirectories)
    {
      strIncrementDirectory += strSingleDirectory + "\\";
      if (!Directory.Exists(strIncrementDirectory))
      {
        Directory.CreateDirectory(strIncrementDirectory);
      }
    }
  }

  public static void DownloadFromWeb(string strSrcUrl, string strLocalLocation, bool boolOverwrite)
  {
    if (File.Exists(strLocalLocation) && boolOverwrite)
    {
      File.Delete(strLocalLocation);
    }
    
    if (!File.Exists(strLocalLocation))
    {
      Uri uriResult = new Uri("http://www.yisheng.yap");
      if (Uri.TryCreate(strSrcUrl, UriKind.Absolute, out uriResult))
      {
        // Url is absolute
        using (WebClient Client = new WebClient())
        {
          Client.DownloadFile(strSrcUrl, strLocalLocation);
        }
      }
      else
      {
        // Url is relative, local
        /*string strSrcFilename = Path.GetFileName(strSrcUrl);
        string strSrcPath = Path.GetDirectoryName(strSrcUrl);*/
        strSrcUrl = (strSrcUrl.StartsWith("/") ? "" : "/") + strSrcUrl;
        String strSrcDirectory = System.Web.Hosting.HostingEnvironment.MapPath("~" + strSrcUrl);
        File.Copy(strSrcDirectory, strLocalLocation);
      }
    }
  }

  public static int GetAge(DateTime dtDob)
  {
    DateTime dtNow = DateTime.Now;

    int age = dtNow.Year - dtDob.Year;
    if (dtDob > dtNow.AddYears(-age)) age--;

    return age;
  }

  public static String HttpUploadFile(string url, Stream fileStream, string paramName, String fileName, string contentType)
  {
    string output = "";
    string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
    byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

    HttpWebRequest wr = (HttpWebRequest)System.Net.WebRequest.Create(url);
    wr.ContentType = "multipart/form-data; boundary=" + boundary;
    wr.Method = "POST";
    wr.KeepAlive = true;
    wr.Credentials = System.Net.CredentialCache.DefaultCredentials;

    Stream rs = wr.GetRequestStream();

    NameValueCollection nvc = new NameValueCollection();
    nvc.Add("submit", "Upload");

    string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
    foreach (string key in nvc.Keys)
    {
      rs.Write(boundarybytes, 0, boundarybytes.Length);
      string formitem = string.Format(formdataTemplate, key, nvc[key]);
      byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
      rs.Write(formitembytes, 0, formitembytes.Length);
    }
    rs.Write(boundarybytes, 0, boundarybytes.Length);

    string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
    string header = string.Format(headerTemplate, paramName, fileName, contentType);
    byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
    rs.Write(headerbytes, 0, headerbytes.Length);

    byte[] buffer = new byte[4096];
    int bytesRead = 0;
    while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
    {
      rs.Write(buffer, 0, bytesRead);
    }
    fileStream.Close();

    byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
    rs.Write(trailer, 0, trailer.Length);
    rs.Close();

    WebResponse wresp = null;
    try
    {
      wresp = wr.GetResponse();
      Stream stream2 = wresp.GetResponseStream();
      StreamReader reader2 = new StreamReader(stream2);
      output = reader2.ReadToEnd();
    }
    catch (Exception ex)
    {
      throw ex;
    }
    finally
    {
      if (wresp != null)
      {
        wresp.Close();
        wresp = null;
      }
      wr = null;
    }

    return output;
  }

  /// <summary>
  /// Web Request Wrapper
  /// </summary>
  /// <param name="method">Http Method</param>
  /// <param name="url">Full url to the web resource</param>
  /// <param name="postData">Data to post in querystring format</param>
  /// <returns>The web server response.</returns>
  public static string WebRequest(Method method, string url, string postData)
  {
    HttpWebRequest webRequest = null;
    StreamWriter requestWriter = null;
    string responseData = "";

    webRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;
    webRequest.Method = method.ToString();
    webRequest.ServicePoint.Expect100Continue = false;
    webRequest.Timeout = 20000;

    if (method == Method.POST)
    {
      webRequest.ContentType = "application/x-www-form-urlencoded";

      //POST the data.
      requestWriter = new StreamWriter(webRequest.GetRequestStream());

      try
      {
        requestWriter.Write(postData);
      }
      catch
      {
        throw;
      }

      finally
      {
        requestWriter.Close();
        requestWriter = null;
      }
    }

    responseData = WebResponseGet(webRequest);
    webRequest = null;
    return responseData;
  }

  /// <summary>
  /// Process the web response.
  /// </summary>
  /// <param name="webRequest">The request object.</param>
  /// <returns>The response data.</returns>
  public static string WebResponseGet(HttpWebRequest webRequest)
  {
    StreamReader responseReader = null;
    string responseData = "";

    try
    {
      responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream());
      responseData = responseReader.ReadToEnd();
    }
    catch
    {
      throw;
    }
    finally
    {
      webRequest.GetResponse().GetResponseStream().Close();
      responseReader.Close();
      responseReader = null;
    }

    return responseData;
  }
}