using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;
using System.Configuration;
using Newtonsoft.Json.Linq;
using System.Web.Helpers;
using Newtonsoft.Json;

/// <summary>
/// Summary description for LinkedInTools
/// </summary>
public class LinkedInTools
{
  public enum Method { GET, POST };

  // URLs taken from http://developer.linkedin.com/documents/authentication
  public const String AUTHORIZE_URL = "https://www.linkedin.com/uas/oauth2/authorization";
  public const String CODE_TO_TOKEN_URL = "https://www.linkedin.com/uas/oauth2/accessToken";

  #region Properties value
  // Configurations in web.config taken as default
  private String _ApiKey = ""; // web.config LinkedIn_ApiKey
  private String _ApiSecret = ""; // web.config LinkedIn_ApiSecret
  private String _RedirectUri = ""; // web.config LinkedIn_RedirectUri

  // Scope reference - http://developer.linkedin.com/documents/authentication#granting
  private String[] _Scope = new string[] { }; // web.config LinkedIn_Scope

  private String _State = "";
  private String _UserAuthorizationCode = "";
  private String _UserAccessToken = "";
  #endregion

  #region Properties
  public String ApiKey
  {
    get
    {
      if (_ApiKey.Length == 0)
      {
        _ApiKey = ConfigurationManager.AppSettings["LinkedIn_ApiKey"];
      }
      return _ApiKey;
    }
    set { _ApiKey = value; }
  }

  public String ApiSecret
  {
    get
    {
      if (_ApiSecret.Length == 0)
      {
        _ApiSecret = ConfigurationManager.AppSettings["LinkedIn_ApiSecret"];
      }
      return _ApiSecret;
    }
    set { _ApiSecret = value; }
  }

  public String RedirectUri
  {
    get
    {
      if (_RedirectUri.Length == 0)
      {
        _RedirectUri = ConfigurationManager.AppSettings["LinkedIn_RedirectUri"];
      }
      return _RedirectUri;
    }
    set { _RedirectUri = value; }
  }

  public String[] Scope
  {
    get
    {
      if (_Scope.Length == 0)
      {
        string strScope = ConfigurationManager.AppSettings["LinkedIn_Scope"];
        _Scope = strScope.Split(',').Select(strVal => strVal.Trim(',', ' ')).ToArray();
      }
      return _Scope;
    }
    set { _Scope = value; }
  }
  
  //  A long unique string value of your choice that is hard to guess. Used to prevent CSRF
  public String State { 
    get {
      System.Web.SessionState.HttpSessionState Session = HttpContext.Current.Session;
      if (!Session.IsNewSession)
      {
        if (Session["LinkedIn_ApiState"] != null)
        {
          _State = (string)Session["LinkedIn_ApiState"];
        }
      }

      return _State;
    }

    set {
      _State = value;
      System.Web.SessionState.HttpSessionState Session = HttpContext.Current.Session;
      Session["LinkedIn_ApiState"] = _State;
    }
  } 

  public String UserAuthorizationCode { get; set; }
  public String UserAccessToken { get; set; }
  #endregion

  public LinkedInTools()
  {
  }

  /// <summary>
  /// Get the link to LinkedIn's authorization page for this application.
  /// </summary>
  /// <returns>The url with a valid request token, or a null string.</returns>
  public string AuthorizationLinkGet(string strState)
  {
    this.State = strState;
    
    return string.Format("{0}?response_type=code&client_id={1}&scope={2}&state={3}&redirect_uri={4}", AUTHORIZE_URL, this.ApiKey, String.Join(" ", this.Scope), this.State, this.RedirectUri);
  }

  public string ExchangeCodeForToken(String strUserAccessCode)
  {
    this.UserAuthorizationCode = strUserAccessCode;

    string strTokenParams = string.Format("grant_type=authorization_code&code={0}&client_id={1}&client_secret={2}&redirect_uri={3}", this.UserAuthorizationCode, this.ApiKey, this.ApiSecret, this.RedirectUri);

    string strJsonWebResponse = PersonalTools.WebRequest(PersonalTools.Method.POST, CODE_TO_TOKEN_URL, strTokenParams);

    JObject jObjResponseData = JObject.Parse(strJsonWebResponse);
    JToken jtValue = null;
    if (jObjResponseData.TryGetValue("access_token", out jtValue))
    {
      this.UserAccessToken = jObjResponseData["access_token"].ToString().Replace("\"", "");
    }

    return this.UserAccessToken;
  }

  public static dynamic GetUserProfile(string strUserAccessToken)
  {
    string strUrl = string.Format("https://api.linkedin.com/v1/people/~?oauth2_access_token={0}&format=json", strUserAccessToken);

    string strJsonWebResponse = PersonalTools.WebRequest(PersonalTools.Method.GET, strUrl, "");

    //return strJsonWebResponse;
    /*
    {
      "firstName": "Firstname", 
      "headline": "Role at Company", 
      "lastName": "Lastname", 
      "siteStandardProfileRequest": {"url": "http://www.linkedin.com/profile/view?id="} 
    }
    */
    return Json.Decode(strJsonWebResponse);
  }

  // Iterate through pagination
  public static string IterateResponsePaginationData(string strBaseUrl, string strMainPropertyName)
  {
    try
    {
      string strJsonWebResponse = PersonalTools.WebRequest(PersonalTools.Method.GET, strBaseUrl, "");
      JObject jMain1 = (JObject)JsonConvert.DeserializeObject(strJsonWebResponse);
      JObject j1 = jMain1;
      if (!String.IsNullOrEmpty(strMainPropertyName))
      {
        j1 = (JObject)JsonConvert.DeserializeObject(jMain1.GetValue(strMainPropertyName).ToString());
      }

      strJsonWebResponse = JsonConvert.SerializeObject(j1);

      int intTotal = int.Parse(j1.Value<string>("_total") ?? "0");
      int intCount = int.Parse(j1.Value<string>("_count") ?? "0");
      int intStart = int.Parse(j1.Value<string>("_start") ?? "0");
      int intCurrentMax = intCount + intStart;

      while (intTotal > intCurrentMax && intCurrentMax > 0)
      {
        string strFullUrl = strBaseUrl + "&start=" + intCurrentMax.ToString();
        
        string strJsonWebResponsePaging = PersonalTools.WebRequest(PersonalTools.Method.GET, strFullUrl, "");

        JObject jMain2 = (JObject)JsonConvert.DeserializeObject(strJsonWebResponsePaging);
        JObject j2 = jMain2;
        if (!String.IsNullOrEmpty(strMainPropertyName))
        {
          j2 = (JObject)JsonConvert.DeserializeObject(jMain2.GetValue(strMainPropertyName).ToString());
        }

        intCount = int.Parse(j2.Value<string>("_count") ?? "0");
        intStart = int.Parse(j2.Value<string>("_start") ?? "0");
        if (intCurrentMax == intCount + intStart)
        {
          break;
        }
        intCurrentMax = intCount + intStart;

        if (j2["values"] != null)
        {
          var jArray = new JArray(j1, j2);
          var str = jArray.ToString();

          strJsonWebResponse = JsonConvert.SerializeObject(
                          new { _total = j1["_total"], values = j1["values"].Union(j2["values"]) },
                          Newtonsoft.Json.Formatting.Indented);
          j1 = (JObject)JsonConvert.DeserializeObject(strJsonWebResponse);
        }
      }

      return strJsonWebResponse;
    }
    catch (Exception ex)
    {
      String strError = "ex.Message=" + ex.Message + "\r\n"
                      + "ex.Source=" + ex.Source + "\r\n"
                      + "ex.StackTrace=" + ex.StackTrace + "\r\n";

      return strError;
    }
  }

  public static dynamic GetUserConnections(string strUserAccessToken)
  {
    string strBaseUrl = string.Format("https://api.linkedin.com/v1/people/~/connections?oauth2_access_token={0}&format=json", strUserAccessToken);
    string strJsonWebResponse = IterateResponsePaginationData(strBaseUrl, "");

    //return strJsonWebResponse;
    return Json.Decode(strJsonWebResponse);
  }

  public static dynamic GetUserGroups(string strUserAccessToken)
  {
    string strBaseUrl = string.Format("https://api.linkedin.com/v1/people/~/group-memberships?oauth2_access_token={0}&format=json", strUserAccessToken);
    string strJsonWebResponse = IterateResponsePaginationData(strBaseUrl, "");

    //return strJsonWebResponse;
    return Json.Decode(strJsonWebResponse);
  }

  public static dynamic GetUserFollowedCompanies(string strUserAccessToken)
  {
    string strBaseUrl = string.Format("https://api.linkedin.com/v1/people/~/following/companies?oauth2_access_token={0}&format=json", strUserAccessToken);
    string strJsonWebResponse = IterateResponsePaginationData(strBaseUrl, "");

    //return strJsonWebResponse;
    return Json.Decode(strJsonWebResponse);
  }

  public static dynamic GetUserPositions(string strUserAccessToken)
  {
    string strBaseUrl = string.Format("https://api.linkedin.com/v1/people/~/positions?oauth2_access_token={0}&format=json", strUserAccessToken);
    string strJsonWebResponse = IterateResponsePaginationData(strBaseUrl, "");

    // return strJsonWebResponse;
    return Json.Decode(strJsonWebResponse);
  }
}