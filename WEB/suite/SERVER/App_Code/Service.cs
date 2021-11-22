using Newtonsoft.Json.Linq;
using Saml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Xml;
using System.Security.Cryptography;

/// <summary>
/// Summary description for Service
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class Service : System.Web.Services.WebService
{

    public Service()
    {
        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    //String donde se creara el token
    internal static readonly char[] chars =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();

    //Función que genera el token
    public static string GetUniqueKey()
    {
        int size = 32;
        byte[] data = new byte[4 * size];
        using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
        {
            crypto.GetBytes(data);
        }
        StringBuilder result = new StringBuilder(size);
        for (int i = 0; i < size; i++)
        {
            var rnd = BitConverter.ToUInt32(data, i * 4);
            var idx = rnd % chars.Length;

            result.Append(chars[idx]);
        }

        return result.ToString();
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public void haveSession(string navegador)
    {
        JObject result = new JObject();

        if (Session["Email"] != null)
        {

            WS consulta = new WS();

            Session["navegador"] = navegador;

            string info = consulta.Login();

            result["STATE"] = "true";
            result["MESSAGE"] = "Consulta realizada con éxito.";
            result["RESULT"] = info;
            Context.Response.Write(result.ToString());
        }
        else
        {
            result["STATE"] = "false";
            result["MESSAGE"] = "Consulta realizada con éxito.";
            result["RESULT"] = new JArray();
            Context.Response.Write(result.ToString());
        }
    }

    [WebMethod(EnableSession = true)]
    public void SSOValidation()
    {
        // The Client ID is used by the application to uniquely identify itself to Azure AD.
        string clientId = System.Configuration.ConfigurationManager.AppSettings["ClientId"];

        // RedirectUri is the URL where the user will be redirected to after they sign in.
        string redirectUri = System.Configuration.ConfigurationManager.AppSettings["RedirectUri"];

        //URL for authority
        var samlEndpoint = String.Format(System.Configuration.ConfigurationManager.AppSettings["Authority"]);

        var request = new AuthRequest(
            clientId, //put your app's "unique ID" here
            redirectUri //assertion Consumer Url - the redirect URL where the provider will send authenticated users
            );

        //generate the provider URL
        string url = request.GetRedirectUrl(samlEndpoint);

        HttpContext.Current.Response.Redirect(url);
    }

    /* Provisional */
    [WebMethod(EnableSession = true)]
    public void SSOBanasan()
    {
        // The Client ID is used by the application to uniquely identify itself to Azure AD.
        string clientId = System.Configuration.ConfigurationManager.AppSettings["ClientId"];

        // RedirectUri is the URL where the user will be redirected to after they sign in.
        string redirectUri = System.Configuration.ConfigurationManager.AppSettings["RedirectUri"];

        //URL for authority
        var samlEndpoint = String.Format(System.Configuration.ConfigurationManager.AppSettings["Authority_Banasan"]);

        var request = new AuthRequest(
            clientId, //put your app's "unique ID" here
            redirectUri //assertion Consumer Url - the redirect URL where the provider will send authenticated users
            );

        //generate the provider URL
        string url = request.GetRedirectUrl(samlEndpoint);

        HttpContext.Current.Response.Redirect(url);
    }

    [WebMethod(EnableSession = true)]
    //[ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public void SSO()
    {
        JObject result = new JObject();

        string xmlMail, xmlLastName, xmlName;

        //Email
        xmlMail = "";
        //Primer Apellido
        xmlLastName = "";
        //Primer Nombre
        xmlName = "";

        string data = "";

        try
        {
            //Sin certificado
            string samlCertificate = @"-----BEGIN CERTIFICATE-----
                MIIC8DCCAdigAwIBAgIQLoQCG44XBr1GrP29RwNMzzANBgkqhkiG9w0BAQsFADA0MTIwMAYDVQQD
                EylNaWNyb3NvZnQgQXp1cmUgRmVkZXJhdGVkIFNTTyBDZXJ0aWZpY2F0ZTAeFw0yMDAzMjYxMTEz
                MzNaFw0yMzAzMjYxMTEzMzNaMDQxMjAwBgNVBAMTKU1pY3Jvc29mdCBBenVyZSBGZWRlcmF0ZWQg
                U1NPIENlcnRpZmljYXRlMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAqpMIz2RsvRoE
                QfM3EU+xJn4uteTk1DqdXWMq5vqD8/3UpxmB89CgxaULB2CJD/rm6oJZdcvRstBqz2H4BRhQwhFw
                /6Wkblw13r8bKwfydAqDj+V4gFPX9zMUr0OiO1FRn1MbPq40TXL54Q3RXfqygfy0htyF387xmtmd
                CtbhRffd8ciYAqkPkUJODHOxxoO9I3FInogW9vZiViUS2U15APDGSMBUwq9W9OJ1p7pjFtcDy8m0
                Cm9nI+0ng2hkTecLLNuAWYnHQhlzHMm5aDMxfPQYAi9KV3cyP3nJbWp9svDCj5b5FypoNtTk5uJs
                FEZ4rt369EVDu9RyJhtVZ1YhyQIDAQABMA0GCSqGSIb3DQEBCwUAA4IBAQCck2Eg6jHRlawyZ7Sh
                GYW5dbxZcz/ZI7eApgn8aUpq+5x/AjdA8z7vEZqbwu0HHcqbWdolu9AMAt2X8/1O4uyIKV4erMQW
                LvKI+7AEcOQWxwCIeIEyj7U8MEvP2+q2DDT1ikJ4nFbzLoY6ENK7YZX5gPgy0PQVuaZBuvly/kNF
                cf3IGpX8iU3rR7AGmXQsFRJ3MBB65grNXcANSbH1wlQREjYAe8Vj1IUJ10u/01MB8zsjcHh4P7Ed
                CyUsWGTJOyVqph5Ikzic0v4RBBddjq063hktqiwTZvQi73txUCnyfLKwsYjBqEVVVbrzoIgGPRYl
                t0Aa01wuwWRmUSBRtniJ
                -----END CERTIFICATE-----";
            HttpContext.Current.Request.InputStream.Position = 0;

            data = new StreamReader(HttpContext.Current.Request.InputStream).ReadToEnd();
            data = HttpContext.Current.Server.UrlDecode(data.Replace("SAMLResponse=", ""));

            byte[] dataBytes = Convert.FromBase64String(data);
            string decodedString = Encoding.UTF8.GetString(dataBytes);

            //Respuesta en formato XML
            XmlDocument xmResponse = new XmlDocument();
            xmResponse.LoadXml(decodedString);

            //Se valida el inicio de sesión afirmativo
            String statusResponse = xmResponse.GetElementsByTagName("saml2p:StatusCode")[0].Attributes[0].Value.ToString();

            if (statusResponse == "urn:oasis:names:tc:SAML:2.0:status:Success")
            {
                //Respues en objeto XML
                XmlNodeList elemList = xmResponse.GetElementsByTagName("saml2:Attribute");
                string attrVal;
                for (int i = 0; i < elemList.Count; i++)
                {
                    attrVal = elemList[i].Attributes["Name"].Value;
                    Console.WriteLine(attrVal);
                    switch (attrVal)
                    {
                        case "Apellidos":
                            //Apellido
                            xmlLastName = elemList[i].InnerText;
                            break;
                        case "Nombres":
                            //nombre
                            xmlName = elemList[i].InnerText;
                            break;
                        case "correo":
                            //Nombre completo
                            xmlMail = elemList[i].InnerText;
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        catch (Exception e)
        {
            string host = HttpContext.Current.Request.Url.Host;
            Mail.SendEmail(e, host, data);
        }

        string token = GetUniqueKey();

        //Se genera la objeto sesion
        Session["Email"] = xmlMail;
        Session["LastName"] = xmlLastName;
        Session["Name"] = xmlName;

        Session["navegador"] = "";

        Session["token"] = token;

        //Redireccionamiento base https
        string Redireccionamiento = System.Configuration.ConfigurationManager.AppSettings["Redireccionamiento"];
        HttpContext.Current.Response.Redirect(Redireccionamiento);
    }
}
