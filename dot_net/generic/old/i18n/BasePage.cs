using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Resources;
using System.Reflection;
using System.Threading;
using System.Globalization;
using System.Collections.Generic;

/// <summary>
/// Summary description for BasePage
/// </summary>
public class BasePage : Page
{
  private string m_DefaultCulture = ConfigurationManager.AppSettings["locale_" + ConfigurationManager.AppSettings["default_language"]];

  #region InitializeCulture
  protected override void InitializeCulture()
  {
    //call base class
    base.InitializeCulture();

    string culture = m_DefaultCulture;

    HttpCookie cookie = Request.Cookies.Get("locale_lang");
    DateTime cookieExpiration = DateTime.Now.AddDays(30);

    if (Request["lang"] != null && !String.IsNullOrEmpty(Request["lang"]))
    {
      if (cookie != null)
      {
        cookie.Value = Request["lang"];
        Response.Cookies.Set(cookie);
      }
      else
      {
        cookie = new HttpCookie("locale_lang", Request["lang"]);
        cookie.Expires = cookieExpiration;
        Response.Cookies.Add(cookie);
      }
    }

    if (cookie != null && !String.IsNullOrEmpty(cookie.Value))
    {
      culture = cookie.Value;
    }
    
    String strDefaultLocale = ConfigurationManager.AppSettings["default_language"];
    String[] aryStrShortLocales = ConfigurationManager.AppSettings["locale_list"].Split(',');
    Dictionary<String, String> dictLocales = new Dictionary<string, string>();
    foreach (String strShortLocale in aryStrShortLocales)
    {
      if (!dictLocales.ContainsKey(strShortLocale.ToLower()))
      {
        dictLocales.Add(strShortLocale.ToLower(), ConfigurationManager.AppSettings["locale_" + strShortLocale.ToLower()]); // EG. en => en-GB
      }
      if (!dictLocales.ContainsKey(ConfigurationManager.AppSettings["locale_" + strShortLocale.ToLower()].ToLower()))
      {
        dictLocales.Add(ConfigurationManager.AppSettings["locale_" + strShortLocale.ToLower()].ToLower(), ConfigurationManager.AppSettings["locale_" + strShortLocale.ToLower()]); // EG. en-gb => en-GB
      }
    }

    if (dictLocales.ContainsKey(culture.ToLower().Trim()))
    {
      culture = dictLocales[culture.ToLower().Trim()];
    }
    else
    {
      culture = m_DefaultCulture;
    }

    //check whether a culture is stored in the session
    if (!string.IsNullOrEmpty(culture)) Culture = culture;
    else Culture = m_DefaultCulture;

    //set culture to current thread
    Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(culture);
    Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);
  }
  #endregion

  /* 
  //Failed attempt to shorten resource reading
  public string _(String strKey)
  {
    String strFileName = "website.en.resx";

    string resourceFile = "~/App_LocalResources/" + strFileName;
    string filePath = System.AppDomain.CurrentDomain.BaseDirectory.ToString();
    ResourceManager resourceManager = ResourceManager.CreateFileBasedResourceManager(resourceFile, filePath, null);

    return resourceManager.GetString(strKey);
  }
  */

  public static void SetLanguageCookie(String strLanguage)
  {
    HttpCookie cookie = HttpContext.Current.Request.Cookies.Get("locale_lang");
    DateTime cookieExpiration = DateTime.Now.AddDays(30);

    if (cookie != null)
    {
      cookie.Value = strLanguage;
      HttpContext.Current.Response.Cookies.Set(cookie);
    }
    else
    {
      cookie = new HttpCookie("locale_lang", strLanguage);
      cookie.Expires = cookieExpiration;
      HttpContext.Current.Response.Cookies.Add(cookie);
    }
  }

  public static string GetCookieLanguage()
  {
    HttpCookie cookie = HttpContext.Current.Request.Cookies.Get("locale_lang");

    String strLanguage = "";

    if (cookie != null)
    {
      strLanguage = cookie.Value;
    }

    return strLanguage;
  }
}