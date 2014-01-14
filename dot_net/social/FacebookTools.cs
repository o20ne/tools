using System;
using System.Collections.Generic;
using System.Web;
using System.Net;
using System.Collections.Specialized;
using System.IO;
using System.Configuration;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using System.Linq;
using System.Web.Helpers;

public class FacebookTools
{
  public const string AUTHORIZE = "https://graph.facebook.com/oauth/authorize";
  public const string ACCESS_TOKEN = "https://graph.facebook.com/oauth/access_token";
  public const string GRAPH_API_URI = "https://graph.facebook.com/";

  private string _AppId = "";
  private string _AppSecret = "";
  private string _AccessToken = "";
  private string _CallbackUrl = "";
  private string _Permissions = ConfigurationManager.AppSettings["FacebookPermissions"];
  private string _SignedRequest = "";

  public String signed_params_json = ""; 

  #region Properties

  public string AppId
  {
    get
    {
      if (_AppId.Length == 0)
      {
        _AppId = ConfigurationManager.AppSettings["FacebookAppId"]; //Your application ID
      }
      return _AppId;
    }
    set { _AppId = value; }
  }

  public string CallbackUrl
  {
    get
    {
      if (_CallbackUrl.Length == 0)
      {
        _CallbackUrl = ConfigurationManager.AppSettings["FacebookCallback"]; //Your call back url
      }
      return _CallbackUrl;
    }
    set { _CallbackUrl = value; }
  }

  public string AppSecret
  {
    get
    {
      if (_AppSecret.Length == 0)
      {
        _AppSecret = ConfigurationManager.AppSettings["FacebookAppSecret"];  //Your application secret
      }
      return _AppSecret;
    }
    set { _AppSecret = value; }
  }

  public string AccessToken { get { return _AccessToken; } set { _AccessToken = value; } }

  public string Permission { get { return _Permissions; } set { _Permissions = value; } }

  public string SignedRequest { get { return _SignedRequest; } set { _SignedRequest = value; } }

  #endregion


  /// <summary>
  /// Get the link to Facebook's authorization page for this application.
  /// </summary>
  /// <returns>The url with a valid request token, or a null string.</returns>
  public string AuthorizationLinkGet()
  {
    return string.Format("{0}?client_id={1}&scope={2}&redirect_uri={3}", AUTHORIZE, this.AppId, this.Permission, this.CallbackUrl);
  }


  /// <summary>
  /// Decrypt the Facebook "signed_request" to an access token.
  /// </summary>
  /// <param name="signedRequest">The oauth_token or "signed_request" is supplied by Facebook.</param>
  public void SignedRequestAccessTokenGet(string signedRequest)
  {
    this.SignedRequest = signedRequest;
    char[] charSplit = { '.' };
    string[] signed_data = this.SignedRequest.Split(charSplit, 2);
    string encoded_sig = signed_data[0];
    string payload = signed_data[1];

    string sig = Base64UrlDecode(encoded_sig);

    using (HMACSHA256 cryto = new HMACSHA256(Encoding.UTF8.GetBytes(this.AppSecret)))
    {
      string hash = Convert.ToBase64String(cryto.ComputeHash(Encoding.UTF8.GetBytes(payload)));
      string hashDecoded = Base64UrlDecode(hash);
      if (hashDecoded.Equals(sig)) // Verifying the secret key crypted signature
      {
        string payloadJson = Encoding.UTF8.GetString(Convert.FromBase64String(Base64UrlDecode(payload)));
        signed_params_json = payloadJson;
        JObject jObjData = JObject.Parse(payloadJson);
        JToken jtValue = null;
        if (jObjData.TryGetValue("oauth_token", out jtValue))
        {
          this.AccessToken = jObjData["oauth_token"].ToString().Replace("\"", "");
        }
      }
    }
  }

  public string ConvertSignedRequestToJson(string signedRequest)
  {
    String signed_params_json = "";
    char[] charSplit = { '.' };
    string[] signed_data = signedRequest.Split(charSplit, 2);
    string encoded_sig = signed_data[0];
    string payload = signed_data[1];

    string sig = Base64UrlDecode(encoded_sig);

    using (HMACSHA256 cryto = new HMACSHA256(Encoding.UTF8.GetBytes(this.AppSecret)))
    {
      string hash = Convert.ToBase64String(cryto.ComputeHash(Encoding.UTF8.GetBytes(payload)));
      string hashDecoded = Base64UrlDecode(hash);
      if (hashDecoded.Equals(sig)) // Verifying the secret key crypted signature
      {
        string payloadJson = Encoding.UTF8.GetString(Convert.FromBase64String(Base64UrlDecode(payload)));
        signed_params_json = payloadJson;
      }
    }

    return signed_params_json;
  }

  /// <summary>
  /// Exchange the Facebook "code" for an access token.
  /// </summary>
  /// <param name="authToken">The oauth_token or "code" is supplied by Facebook's authorization page following the callback.</param>
  public void CodeAccessTokenGet(string authToken)
  {
    this.AccessToken = authToken;
    string accessTokenUrl = string.Format("{0}?client_id={1}&redirect_uri={2}&client_secret={3}&code={4}",
    ACCESS_TOKEN, this.AppId, this.CallbackUrl, this.AppSecret, authToken);

    string response = PersonalTools.WebRequest(PersonalTools.Method.GET, accessTokenUrl, String.Empty);

    if (response.Length > 0)
    {
      //Store the returned access_token
      NameValueCollection qs = HttpUtility.ParseQueryString(response);

      if (qs["access_token"] != null)
      {
        this.AccessToken = qs["access_token"];
      }
    }
  }

  public String AppAccessTokenGet()
  {
    String strQuery = String.Format("client_id={0}&client_secret={1}&grant_type=client_credentials", this.AppId, this.AppSecret);

    String[] strResponse = PersonalTools.WebRequest(PersonalTools.Method.POST, ACCESS_TOKEN, strQuery).Split('=');

    return strResponse[strResponse.Length - 1];
  }

  public string GraphSearchUrl(String strQuery, String strFields, String strAccessToken)
  {
    strQuery = String.Format("q={0}&access_token={1}", HttpUtility.UrlEncode(strQuery), strAccessToken);
    if (!String.IsNullOrEmpty(strFields))
    {
      strQuery = strQuery + String.Format("&fields={0}", strFields);
    }
    
    return GRAPH_API_URI + "search?" + strQuery;
  }

  public static DateTime ConvertFBTimeString(string datetime_str)
  {
    string[] parts = datetime_str.Split('T');
    string[] d_parts = parts[0].Split('-');
    string[] t_parts = parts[1].Split('+');
    string[] t_parts2 = t_parts[0].Split(':');
    DateTime dt = new DateTime(Convert.ToInt16(d_parts[0]), Convert.ToInt16(d_parts[1]), Convert.ToInt16(d_parts[2]), Convert.ToInt16(t_parts2[0]), Convert.ToInt16(t_parts2[1]), Convert.ToInt16(t_parts2[2]));
    return dt;
  }

  /// <summary>
  /// Converts the base 64 url encoded string to standard base 64 encoding.
  /// </summary>
  /// <param name="encodedValue">The encoded value.</param>
  /// <returns>The base 64 string.</returns>
  private static string Base64UrlDecode(string encodedValue)
  {
    encodedValue = encodedValue.Replace('+', '-').Replace('/', '_').Trim();
    int pad = encodedValue.Length % 4;
    if (pad > 0)
    {
      pad = 4 - pad;
    }

    encodedValue = encodedValue.PadRight(encodedValue.Length + pad, '=');
    return encodedValue;
  }

  public static string FQLQueryJson(string strFQL, string strOAuthToken)
  {
    String strQuery = String.Format("fql?q={0}&access_token={1}", HttpUtility.UrlEncode(strFQL), strOAuthToken);

    return PersonalTools.WebRequest(PersonalTools.Method.GET, GRAPH_API_URI + strQuery, "");
  }

  public static string WallPost(String strOAuthToken, String strFbid, String strLink, String strPictureUrl, String strMessage, String strDescription, String strCaption)
  {
    String fbPostParams = "";
    if (!String.IsNullOrEmpty(strLink))
    {
      fbPostParams += "&link=" + strLink;
    }

    if (!String.IsNullOrEmpty(strPictureUrl))
    {
      fbPostParams += "&picture=" + strPictureUrl;
    }

    if (!String.IsNullOrEmpty(strMessage))
    {
      fbPostParams += "&message=" + strMessage;
    }

    if (!String.IsNullOrEmpty(strDescription))
    {
      fbPostParams += "&description=" + strDescription;
    }

    if (!String.IsNullOrEmpty(strCaption))
    {
      fbPostParams += "&caption=" + strCaption;
    }

    fbPostParams = fbPostParams.TrimStart('&');

    return PersonalTools.WebRequest(PersonalTools.Method.POST, GRAPH_API_URI + strFbid + "/feed?access_token=" + strOAuthToken, fbPostParams);
  }

  public static List<KeyValuePair<long, string>> GetUserFriends(String oAuthToken, String fid)
  {
    List<KeyValuePair<long, string>> listFriends = new List<KeyValuePair<long, string>>();

    if (!String.IsNullOrEmpty(fid) && !String.IsNullOrEmpty(oAuthToken))
    {
      string strGraphApiUrl = GRAPH_API_URI + fid + "/friends?format=json&access_token=" + oAuthToken;
      string strResponseJson = PersonalTools.WebRequest(PersonalTools.Method.GET, strGraphApiUrl, String.Empty);
      JObject jObjResponseData = JObject.Parse(strResponseJson);

      if (jObjResponseData["data"] != null)
      {
        foreach (JObject jObjFriend in jObjResponseData["data"].Children())
        {
          try
          {
            string strFriendFbid = jObjFriend["id"].ToString().Replace("\"", "");
            long longFriendFbid = long.Parse(strFriendFbid);
            string strFriendName = jObjFriend["name"].ToString().Replace("\"", "");
            KeyValuePair<long, string> kvpFriend = new KeyValuePair<long, string>(longFriendFbid, strFriendName);
            listFriends.Add(kvpFriend);
          }
          catch (Exception friendEx)
          {
          }
        }
      }
    }
    return listFriends;
  }

  public static List<Dictionary<string, string>> GetAlbums(String oAuthToken, String fid, String fields)
  {
    List<Dictionary<string, string>> listAlbumData = new List<Dictionary<string, string>>();

    if (!String.IsNullOrEmpty(fid) && !String.IsNullOrEmpty(oAuthToken))
    {
      string strGraphApiUrl = GRAPH_API_URI + fid + "/albums?limit=100&access_token=" + oAuthToken + (!String.IsNullOrEmpty(fields) ? "&fields=" + fields : "");

      string strResponseJson = PersonalTools.WebRequest(PersonalTools.Method.GET, strGraphApiUrl, String.Empty);
      JObject jObjResponseData = JObject.Parse(strResponseJson);


      if (jObjResponseData["data"] != null)
      {
        foreach (JObject jObjAlbum in jObjResponseData["data"].Children())
        {
          Dictionary<string, string> dictData = new Dictionary<string, string>();
          foreach (KeyValuePair<string, JToken> jObjAlbumData in jObjAlbum)
          {
            dictData.Add(jObjAlbumData.Key.Replace("\"", ""), jObjAlbumData.Value.ToString().Replace("\"", ""));
          }
          listAlbumData.Add(dictData);
        }
      }
    }

    return listAlbumData;
  }

  public static List<string> ValidateFriendsId(string oAuthToken, List<string> listFriendsId)
  {
    List<string> listValidFriendsId = new List<string>();
    string strValFriendsFql = "SELECT uid2 FROM friend WHERE uid1 = me() AND uid2 IN (" + String.Join(",", listFriendsId) + ")";

    string strValFriendJsonResponse = FacebookTools.FQLQueryJson(strValFriendsFql, oAuthToken);
    JObject jValFriendData = JObject.Parse(strValFriendJsonResponse);

    if (jValFriendData["data"] != null)
    {
      foreach (JObject objFriendsId in jValFriendData["data"].Children())
      {
        listValidFriendsId.Add(objFriendsId["uid2"].ToString().Replace("\"", ""));
      }
    }

    return listValidFriendsId;
  }

  public static bool haveFanLike(String oAuthToken, String fid, String fanPageId)
  {
    bool like = false;

    if (!String.IsNullOrEmpty(fid) && !String.IsNullOrEmpty(oAuthToken))
    {
      String url = GRAPH_API_URI + fid + "/likes?target_id=" + fanPageId + "&format=json&access_token=" + oAuthToken;
      string strJsonResponse = PersonalTools.WebRequest(PersonalTools.Method.GET, url, String.Empty);
      JObject jValPageData = JObject.Parse(strJsonResponse);
      
    if (jValPageData["data"] != null && jValPageData["data"].Children().Count() > 0)
      {
        like = true;
      }
    }

    return like;
  }

  public static Dictionary<string, string> getProfile(String oAuthToken)
  {
    Dictionary<string, string> profileData = new Dictionary<string, string>();

    try
    {
      string strJson = PersonalTools.WebRequest(PersonalTools.Method.GET, GRAPH_API_URI + "me?access_token=" + oAuthToken, String.Empty);
      JObject jUserFbData = JObject.Parse(strJson);
      foreach (KeyValuePair<string, JToken> jObjData in jUserFbData)
      {
        profileData.Add(jObjData.Key.Replace("\"", ""), jObjData.Value.ToString().Replace("\"", ""));
      }

    }
    catch (Exception ex)
    {
      throw;
    }

    return profileData;
  }

  public static string getName(string strFbid)
  {
    string strName = "";
    try
    {
      string strJson = PersonalTools.WebRequest(PersonalTools.Method.GET, GRAPH_API_URI + strFbid, String.Empty);
      JObject jUserFbData = JObject.Parse(strJson);
      strName = jUserFbData["name"].ToString().Replace("\"", "");
    }
    catch (Exception ex)
    {

    }

    return strName;
  }
}