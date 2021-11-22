using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Net.Mime;

/// <summary>
/// Summary description for Mail
/// </summary>
public class Mail
{
    
    public Mail()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public static void SendEmail(Exception ex, String dominio, String parametros)
    {
        String mail;
        String password;
        String nombreRemitente;
        String asunto;

        ConexionSQL conexion = new ConexionSQL();
        conexion.openConexion();
        SqlDataAdapter adapter = new SqlDataAdapter(String.Format(@"
            SELECT
                Cuenta.mail,
                Cuenta.password,
                Cuenta.nombreRemitente,
                Cuenta.asunto

            FROM
                {0}.cuentasCorreos Cuenta
            WHERE
                Cuenta.nombre = 'EXCEPCION'
        ", Abi_maestro.esquema), conexion.getConexion());
        DataSet ds = new DataSet();
        adapter.Fill(ds);
        DataTable dt = ds.Tables[0];
        DataRow row = dt.Rows[0];

        mail = row["mail"].ToString();
        password = row["password"].ToString();
        nombreRemitente = row["nombreRemitente"].ToString();
        asunto = row["asunto"].ToString();


        MailAddress toAddress;
        var fromAddress = new MailAddress(mail, nombreRemitente);
        MailMessage message;
        message = new MailMessage();
                
        adapter = new SqlDataAdapter(String.Format(@"
        SELECT
            Correo.correo
        FROM
            {0}.correosExcepcion Correo
        ", Abi_maestro.esquema), conexion.getConexion());

        ds = new DataSet();
        adapter.Fill(ds);
        dt = ds.Tables[0];
        row = dt.Rows[0];

        toAddress = new MailAddress(row[0].ToString());
        message = new MailMessage();
        message.From = fromAddress;
        message.Bcc.Add(toAddress);
        int i = 0;
        foreach (DataRow auxrow in dt.Rows)
        {
            if (i > 0)
            {
                message.Bcc.Add(auxrow[0].ToString());
            }
            i++;
        }

        message.Subject = asunto + ": " + dominio;
        StringBuilder stringBuilder = new StringBuilder(); stringBuilder.Append("<div class=\"vZone\" style=\"width: 100%;height: 100vh;font-family: 'Droid Sans', sans-serif;overflow: auto !important;\">");
        stringBuilder.Append("<div class=\"vcZone\" style=\"-webkit-flex-direction: column;-ms-flex-direction: column;flex-direction: column;box-sizing: border-box;min-height: 100%;\">");
        stringBuilder.Append("<div class=\"vrZone\" style=\"display: flex;-webkit-justify-content: center;-ms-flex-pack: center;justify-content: center;-webkit-align-items: center;-ms-flex-align: center;align-items: center;width: 100%;-webkit-justify-content: flex-start;-ms-flex-pack: start;justify-content: flex-start;\">");
        stringBuilder.Append("<img src =\"http://opc.com.co/wp-content/uploads/2016/01/logo-292x139.png\" style=\"padding:10px; \"></div>");
        stringBuilder.Append("<div class=\"vrZone\" style=\"display: flex;-webkit-justify-content: center;-ms-flex-pack: center;justify-content: center;-webkit-align-items: center;-ms-flex-align: center;align-items: center;width: 100%;\">");
        stringBuilder.Append("<div class=\"sT1\" style=\"font-size: 26px;font-weight: 600;background: #535355;color: #FFF;width: 100%;padding: 10px 5px;box-sizing: border-box;\">Reporte de Excepciones Servicio Web</div></div>");
        stringBuilder.Append("<div class=\"vrZone\" style=\"display: flex;-webkit-justify-content: center;-ms-flex-pack: center;justify-content: center;-webkit-align-items: center;-ms-flex-align: center;align-items: center;width: 100%;\">");
        stringBuilder.Append("<table style=\"border-collapse: collapse;width: 100%;\">");
        stringBuilder.Append("<tr style=\"background: #e3c5b5;\">");
        stringBuilder.Append("<th style=\"width: 25%;font-size: 15px;font-weight: 600;padding: 3px;padding-left: 5px;box-sizing: border-box;border: 1px solid #ce9a7e;text-align: left;\">Información</th>");
        stringBuilder.Append("<th style=\"width: 25%;font-size: 15px;font-weight: 600;padding: 3px;padding-left: 5px;box-sizing: border-box;border: 1px solid #ce9a7e;text-align: left;\">Dato</th>");
        stringBuilder.Append("</tr>");
        stringBuilder.Append("<tr style=\"background: #FFF;\">");
        stringBuilder.Append("<td style=\"padding: 3px;padding-left: 5px;box-sizing: border-box;width: 25%;font-size: 15px;border: 1px solid #E1C1B0;\">Tipo Exception</td>");
        stringBuilder.Append("<td style=\"padding: 3px;padding-left: 5px;box-sizing: border-box;width: 25%;font-size: 15px;border: 1px solid #E1C1B0;\">" + ex.GetType() + "</td>");
        stringBuilder.Append("</tr>");
        stringBuilder.Append("<tr style=\"background: #FBE4D6;\">");
        stringBuilder.Append("<td style=\"padding: 3px;padding-left: 5px;box-sizing: border-box;width: 25%;font-size: 15px;border: 1px solid #E1C1B0;\">Mensaje Exception</td>");
        stringBuilder.Append("<td style=\"padding: 3px;padding-left: 5px;box-sizing: border-box;width: 25%;font-size: 15px;border: 1px solid #E1C1B0;\">" + ex.Message + "</td>");
        stringBuilder.Append("</tr>");
        stringBuilder.Append("<tr style=\"background: #FFF;\">");
        stringBuilder.Append("<td style=\"padding: 3px;padding-left: 5px;box-sizing: border-box;width: 25%;font-size: 15px;border: 1px solid #E1C1B0;\">Stack trace</td>");
        stringBuilder.Append("<td style=\"padding: 3px;padding-left: 5px;box-sizing: border-box;width: 25%;font-size: 15px;border: 1px solid #E1C1B0;\">" + ex.StackTrace + "</td>");
        stringBuilder.Append("</tr>");

        if (ex.InnerException != null)
        {
            stringBuilder.Append("<tr style=\"background: #FFF;\">");
            stringBuilder.Append("<td style=\"padding: 3px;padding-left: 5px;box-sizing: border-box;width: 25%;font-size: 15px;border: 1px solid #E1C1B0;\">Excepcion Interna</td>");
            stringBuilder.Append("<td style=\"padding: 3px;padding-left: 5px;box-sizing: border-box;width: 25%;font-size: 15px;border: 1px solid #E1C1B0;\"></td>");
            stringBuilder.Append("</tr>");
            stringBuilder.Append("<tr style=\"background: #FFF;\">");
            stringBuilder.Append("<td style=\"padding: 3px;padding-left: 5px;box-sizing: border-box;width: 25%;font-size: 15px;border: 1px solid #E1C1B0;\">Tipo Exception</td>");
            stringBuilder.Append("<td style=\"padding: 3px;padding-left: 5px;box-sizing: border-box;width: 25%;font-size: 15px;border: 1px solid #E1C1B0;\">" + ex.InnerException.GetType() + "</td>");
            stringBuilder.Append("</tr>");
            stringBuilder.Append("<tr style=\"background: #FBE4D6;\">");
            stringBuilder.Append("<td style=\"padding: 3px;padding-left: 5px;box-sizing: border-box;width: 25%;font-size: 15px;border: 1px solid #E1C1B0;\">Mensaje Exception</td>");
            stringBuilder.Append("<td style=\"padding: 3px;padding-left: 5px;box-sizing: border-box;width: 25%;font-size: 15px;border: 1px solid #E1C1B0;\">" + ex.InnerException.Message + "</td>");
            stringBuilder.Append("</tr>");
            stringBuilder.Append("<tr style=\"background: #FFF;\">");
            stringBuilder.Append("<td style=\"padding: 3px;padding-left: 5px;box-sizing: border-box;width: 25%;font-size: 15px;border: 1px solid #E1C1B0;\">Stack trace</td>");
            stringBuilder.Append("<td style=\"padding: 3px;padding-left: 5px;box-sizing: border-box;width: 25%;font-size: 15px;border: 1px solid #E1C1B0;\">" + ex.InnerException.StackTrace + "</td>");
            stringBuilder.Append("</tr>");
        }
        stringBuilder.Append("<tr style=\"background: #FFF;\">");
        stringBuilder.Append("<td style=\"padding: 3px;padding-left: 5px;box-sizing: border-box;width: 25%;font-size: 15px;border: 1px solid #E1C1B0;\">Parametros</td>");
        stringBuilder.Append("<td style=\"padding: 3px;padding-left: 5px;box-sizing: border-box;width: 25%;font-size: 15px;border: 1px solid #E1C1B0;\">" + parametros + "</td>");
        stringBuilder.Append("</tr>");
        stringBuilder.Append("</table>");



        stringBuilder.Append("</div>");
        stringBuilder.Append("<div class=\"vrZone\" style=\"display: flex;-webkit-justify-content: center;-ms-flex-pack: center;justify-content: center;-webkit-align-items: center;-ms-flex-align: center;align-items: center;width: 100%;background: #535355;-webkit-justify-content: flex-start;-ms-flex-pack: start;justify-content: flex-start;margin-top: auto;\">");
        stringBuilder.Append("<div class=\"vcZone\" style=\"-webkit-justify-content: center;-ms-flex-pack: center;justify-content: center;-webkit-flex-direction: column;-ms-flex-direction: column;flex-direction: column;box-sizing: border-box;\">");

        stringBuilder.Append("<div class=\"vrZone\" style=\"display: flex;-webkit-justify-content: center;-ms-flex-pack: center;justify-content: center;-webkit-align-items: center;-ms-flex-align: center;align-items: center;width: 100%;-webkit-justify-content: flex-start;-ms-flex-pack: start;justify-content: flex-start;\">");
        stringBuilder.Append("<div class=\"sT2\" style=\"font-size: 15px;font-weight: 600;color: #FFF;width: 100%;padding: 5px 10px;box-sizing: border-box;\">OptiPlant Consultores S.A.S.</div>");
        stringBuilder.Append("</div>");
        stringBuilder.Append("<div class=\"vrZone\" style=\"display: flex;-webkit-justify-content: center;-ms-flex-pack: center;justify-content: center;-webkit-align-items: center;-ms-flex-align: center;align-items: center;width: 100%;-webkit-justify-content: flex-start;-ms-flex-pack: start;justify-content: flex-start;padding-left: 10px;\">");
        stringBuilder.Append("<div class=\"\" style=\"border-top: 2px solid #BA5B2C;max-width: 245px;width: 245px;\"></div>");
        stringBuilder.Append("</div>");

        stringBuilder.Append("<div class=\"vrZone\" style=\"display: flex;justify-content: center;align-items: center;width: 100%;justify-content: flex-start;\">");
        stringBuilder.Append("<div class=\"sT3\" style=\"font-size: 11px;color: #FFF;width: 100%;padding: 5px 15px;box-sizing: border-box;\">Armenia, Quindío, Colombia</div>");
        stringBuilder.Append("</div>");
        stringBuilder.Append("<div class=\"vrZone\" style=\"display: flex;justify-content: center;align-items: center;width: 100%;justify-content: flex-start;\">");
        //stringBuilder.Append("<div class=\"sT3\" style=\"font-size: 18px;color: #FFF;width: 100%;padding: 5px 15px;box-sizing: border-box;\">info@opc.com.co</div>");
        stringBuilder.Append("<div class=\"sT3\" style=\"font-size: 18px;color: #FFF;width: 100%;padding: 5px 15px;box-sizing: border-box;\">soporte@opc.com.co</div>");
        stringBuilder.Append("</div>");
        stringBuilder.Append("<div class=\"vrZone\" style=\"display: flex;justify-content: center;align-items: center;width: 100%;justify-content: flex-start;\">");
        stringBuilder.Append("<div class=\"sT3\" style=\"font-size: 11px;color: #FFF;width: 100%;padding: 5px 15px;box-sizing: border-box;\">(+57) 320 314 32 89 - (+57 6) 7 35 55 47</div>");
        stringBuilder.Append("</div>");


        stringBuilder.Append("</div>");
        stringBuilder.Append("</div>");
        message.Body = stringBuilder.ToString();
        message.Priority = MailPriority.High;
        message.IsBodyHtml = true;

        var smtp = new SmtpClient
        {
            Host = "smtp.gmail.com",
            Port = 587, //25
            EnableSsl = true, // false
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = true, // false
            Credentials = new NetworkCredential(mail, password)

        };
        conexion.closeConexion();
        smtp.Send(message);
    }

    public static void SendEmail_Error(String dominio, String App)
    {
        String mail;
        String password;
        String nombreRemitente;
        String asunto;

        ConexionSQL conexion = new ConexionSQL();
        conexion.openConexion();
        SqlDataAdapter adapter = new SqlDataAdapter(String.Format(@"
            SELECT
                Cuenta.mail,
                Cuenta.password,
                Cuenta.nombreRemitente,
                Cuenta.asunto

            FROM
                {0}.cuentasCorreos Cuenta
            WHERE
                Cuenta.nombre = 'EXCEPCION'
        ", Abi_maestro.esquema), conexion.getConexion());
        DataSet ds = new DataSet();
        adapter.Fill(ds);
        DataTable dt = ds.Tables[0];
        DataRow row = dt.Rows[0];

        mail = row["mail"].ToString();
        password = row["password"].ToString();
        nombreRemitente = row["nombreRemitente"].ToString();
        asunto = row["asunto"].ToString();


        MailAddress toAddress;
        var fromAddress = new MailAddress(mail, nombreRemitente);
        MailMessage message;
        message = new MailMessage();

        adapter = new SqlDataAdapter(String.Format(@"
        SELECT
            Correo.correo
        FROM
            {0}.correosExcepcion Correo
        ", Abi_maestro.esquema), conexion.getConexion());

        ds = new DataSet();
        adapter.Fill(ds);
        dt = ds.Tables[0];
        row = dt.Rows[0];

        toAddress = new MailAddress(row[0].ToString());
        message = new MailMessage();
        message.From = fromAddress;
        message.Bcc.Add(toAddress);
        int i = 0;
        foreach (DataRow auxrow in dt.Rows)
        {
            if (i > 0)
            {
                message.Bcc.Add(auxrow[0].ToString());
            }
            i++;
        }

        message.Subject = asunto + ": " + dominio;
        StringBuilder stringBuilder = new StringBuilder(); stringBuilder.Append("<div class=\"vZone\" style=\"width: 100%;height: 100vh;font-family: 'Droid Sans', sans-serif;overflow: auto !important;\">");
        stringBuilder.Append("<div class=\"vcZone\" style=\"-webkit-flex-direction: column;-ms-flex-direction: column;flex-direction: column;box-sizing: border-box;min-height: 100%;\">");
        stringBuilder.Append("<div class=\"vrZone\" style=\"display: flex;-webkit-justify-content: center;-ms-flex-pack: center;justify-content: center;-webkit-align-items: center;-ms-flex-align: center;align-items: center;width: 100%;-webkit-justify-content: flex-start;-ms-flex-pack: start;justify-content: flex-start;\">");
        stringBuilder.Append("<img src =\"http://opc.com.co/wp-content/uploads/2016/01/logo-292x139.png\" style=\"padding:10px; \"></div>");
        stringBuilder.Append("<div class=\"vrZone\" style=\"display: flex;-webkit-justify-content: center;-ms-flex-pack: center;justify-content: center;-webkit-align-items: center;-ms-flex-align: center;align-items: center;width: 100%;\">");
        stringBuilder.Append("<div class=\"sT1\" style=\"font-size: 26px;font-weight: 600;background: #535355;color: #FFF;width: 100%;padding: 10px 5px;box-sizing: border-box;\">Reporte de Excepciones Servicio Web</div></div>");
        stringBuilder.Append("<div class=\"vrZone\" style=\"display: flex;-webkit-justify-content: center;-ms-flex-pack: center;justify-content: center;-webkit-align-items: center;-ms-flex-align: center;align-items: center;width: 100%;\">");
        stringBuilder.Append("<table style=\"border-collapse: collapse;width: 100%;\">");
        stringBuilder.Append("<tr style=\"background: #e3c5b5;\">");
        stringBuilder.Append("<th style=\"width: 25%;font-size: 15px;font-weight: 600;padding: 3px;padding-left: 5px;box-sizing: border-box;border: 1px solid #ce9a7e;text-align: left;\">Información</th>");
        stringBuilder.Append("<th style=\"width: 25%;font-size: 15px;font-weight: 600;padding: 3px;padding-left: 5px;box-sizing: border-box;border: 1px solid #ce9a7e;text-align: left;\">Dato</th>");
        stringBuilder.Append("</tr>");
        stringBuilder.Append("<tr style=\"background: #FFF;\">");
        stringBuilder.Append("<td style=\"padding: 3px;padding-left: 5px;box-sizing: border-box;width: 25%;font-size: 15px;border: 1px solid #E1C1B0;\">Tipo Exception</td>");
        stringBuilder.Append("<td style=\"padding: 3px;padding-left: 5px;box-sizing: border-box;width: 25%;font-size: 15px;border: 1px solid #E1C1B0;\">Acceso indebido de la aplicación a través del iframe</td>");
        stringBuilder.Append("</tr>");
        stringBuilder.Append("<tr style=\"background: #FBE4D6;\">");
        stringBuilder.Append("<td style=\"padding: 3px;padding-left: 5px;box-sizing: border-box;width: 25%;font-size: 15px;border: 1px solid #E1C1B0;\">Aplicación</td>");
        stringBuilder.Append("<td style=\"padding: 3px;padding-left: 5px;box-sizing: border-box;width: 25%;font-size: 15px;border: 1px solid #E1C1B0;\">" + App + "</td>");
        stringBuilder.Append("</tr>");
        stringBuilder.Append("</table>");



        stringBuilder.Append("</div>");
        stringBuilder.Append("<div class=\"vrZone\" style=\"display: flex;-webkit-justify-content: center;-ms-flex-pack: center;justify-content: center;-webkit-align-items: center;-ms-flex-align: center;align-items: center;width: 100%;background: #535355;-webkit-justify-content: flex-start;-ms-flex-pack: start;justify-content: flex-start;margin-top: auto;\">");
        stringBuilder.Append("<div class=\"vcZone\" style=\"-webkit-justify-content: center;-ms-flex-pack: center;justify-content: center;-webkit-flex-direction: column;-ms-flex-direction: column;flex-direction: column;box-sizing: border-box;\">");

        stringBuilder.Append("<div class=\"vrZone\" style=\"display: flex;-webkit-justify-content: center;-ms-flex-pack: center;justify-content: center;-webkit-align-items: center;-ms-flex-align: center;align-items: center;width: 100%;-webkit-justify-content: flex-start;-ms-flex-pack: start;justify-content: flex-start;\">");
        stringBuilder.Append("<div class=\"sT2\" style=\"font-size: 15px;font-weight: 600;color: #FFF;width: 100%;padding: 5px 10px;box-sizing: border-box;\">OptiPlant Consultores S.A.S.</div>");
        stringBuilder.Append("</div>");
        stringBuilder.Append("<div class=\"vrZone\" style=\"display: flex;-webkit-justify-content: center;-ms-flex-pack: center;justify-content: center;-webkit-align-items: center;-ms-flex-align: center;align-items: center;width: 100%;-webkit-justify-content: flex-start;-ms-flex-pack: start;justify-content: flex-start;padding-left: 10px;\">");
        stringBuilder.Append("<div class=\"\" style=\"border-top: 2px solid #BA5B2C;max-width: 245px;width: 245px;\"></div>");
        stringBuilder.Append("</div>");

        stringBuilder.Append("<div class=\"vrZone\" style=\"display: flex;justify-content: center;align-items: center;width: 100%;justify-content: flex-start;\">");
        stringBuilder.Append("<div class=\"sT3\" style=\"font-size: 11px;color: #FFF;width: 100%;padding: 5px 15px;box-sizing: border-box;\">Armenia, Quindío, Colombia</div>");
        stringBuilder.Append("</div>");
        stringBuilder.Append("<div class=\"vrZone\" style=\"display: flex;justify-content: center;align-items: center;width: 100%;justify-content: flex-start;\">");
        //stringBuilder.Append("<div class=\"sT3\" style=\"font-size: 18px;color: #FFF;width: 100%;padding: 5px 15px;box-sizing: border-box;\">info@opc.com.co</div>");
        stringBuilder.Append("<div class=\"sT3\" style=\"font-size: 18px;color: #FFF;width: 100%;padding: 5px 15px;box-sizing: border-box;\">soporte@opc.com.co</div>");
        stringBuilder.Append("</div>");
        stringBuilder.Append("<div class=\"vrZone\" style=\"display: flex;justify-content: center;align-items: center;width: 100%;justify-content: flex-start;\">");
        stringBuilder.Append("<div class=\"sT3\" style=\"font-size: 11px;color: #FFF;width: 100%;padding: 5px 15px;box-sizing: border-box;\">(+57) 320 314 32 89 - (+57 6) 7 35 55 47</div>");
        stringBuilder.Append("</div>");


        stringBuilder.Append("</div>");
        stringBuilder.Append("</div>");
        message.Body = stringBuilder.ToString();
        message.Priority = MailPriority.High;
        message.IsBodyHtml = true;

        var smtp = new SmtpClient
        {
            Host = "smtp.gmail.com",
            Port = 587, //25
            EnableSsl = true, // false
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = true, // false
            Credentials = new NetworkCredential(mail, password)

        };
        conexion.closeConexion();
        smtp.Send(message);
    }

    public static void Error_URL(String dominio, String App, String url_real, String url_BD)
    {
        String mail;
        String password;
        String nombreRemitente;
        String asunto;

        ConexionSQL conexion = new ConexionSQL();
        conexion.openConexion();
        SqlDataAdapter adapter = new SqlDataAdapter(String.Format(@"
            SELECT
                Cuenta.mail,
                Cuenta.password,
                Cuenta.nombreRemitente,
                Cuenta.asunto

            FROM
                {0}.cuentasCorreos Cuenta
            WHERE
                Cuenta.nombre = 'EXCEPCION'
        ", Abi_maestro.esquema), conexion.getConexion());
        DataSet ds = new DataSet();
        adapter.Fill(ds);
        DataTable dt = ds.Tables[0];
        DataRow row = dt.Rows[0];

        mail = row["mail"].ToString();
        password = row["password"].ToString();
        nombreRemitente = row["nombreRemitente"].ToString();
        asunto = row["asunto"].ToString();


        MailAddress toAddress;
        var fromAddress = new MailAddress(mail, nombreRemitente);
        MailMessage message;
        message = new MailMessage();

        adapter = new SqlDataAdapter(String.Format(@"
        SELECT
            Correo.correo
        FROM
            {0}.correosExcepcion Correo
        ", Abi_maestro.esquema), conexion.getConexion());

        ds = new DataSet();
        adapter.Fill(ds);
        dt = ds.Tables[0];
        row = dt.Rows[0];

        toAddress = new MailAddress(row[0].ToString());
        message = new MailMessage();
        message.From = fromAddress;
        message.Bcc.Add(toAddress);
        int i = 0;
        foreach (DataRow auxrow in dt.Rows)
        {
            if (i > 0)
            {
                message.Bcc.Add(auxrow[0].ToString());
            }
            i++;
        }

        message.Subject = asunto + ": " + dominio;
        StringBuilder stringBuilder = new StringBuilder(); stringBuilder.Append("<div class=\"vZone\" style=\"width: 100%;height: 100vh;font-family: 'Droid Sans', sans-serif;overflow: auto !important;\">");
        stringBuilder.Append("<div class=\"vcZone\" style=\"-webkit-flex-direction: column;-ms-flex-direction: column;flex-direction: column;box-sizing: border-box;min-height: 100%;\">");
        stringBuilder.Append("<div class=\"vrZone\" style=\"display: flex;-webkit-justify-content: center;-ms-flex-pack: center;justify-content: center;-webkit-align-items: center;-ms-flex-align: center;align-items: center;width: 100%;-webkit-justify-content: flex-start;-ms-flex-pack: start;justify-content: flex-start;\">");
        stringBuilder.Append("<img src =\"http://opc.com.co/wp-content/uploads/2016/01/logo-292x139.png\" style=\"padding:10px; \"></div>");
        stringBuilder.Append("<div class=\"vrZone\" style=\"display: flex;-webkit-justify-content: center;-ms-flex-pack: center;justify-content: center;-webkit-align-items: center;-ms-flex-align: center;align-items: center;width: 100%;\">");
        stringBuilder.Append("<div class=\"sT1\" style=\"font-size: 26px;font-weight: 600;background: #535355;color: #FFF;width: 100%;padding: 10px 5px;box-sizing: border-box;\">Reporte de Excepciones Servicio Web</div></div>");
        stringBuilder.Append("<div class=\"vrZone\" style=\"display: flex;-webkit-justify-content: center;-ms-flex-pack: center;justify-content: center;-webkit-align-items: center;-ms-flex-align: center;align-items: center;width: 100%;\">");
        stringBuilder.Append("<table style=\"border-collapse: collapse;width: 100%;\">");
        stringBuilder.Append("<tr style=\"background: #e3c5b5;\">");
        stringBuilder.Append("<th style=\"width: 25%;font-size: 15px;font-weight: 600;padding: 3px;padding-left: 5px;box-sizing: border-box;border: 1px solid #ce9a7e;text-align: left;\">Información</th>");
        stringBuilder.Append("<th style=\"width: 25%;font-size: 15px;font-weight: 600;padding: 3px;padding-left: 5px;box-sizing: border-box;border: 1px solid #ce9a7e;text-align: left;\">Dato</th>");
        stringBuilder.Append("</tr>");
        stringBuilder.Append("<tr style=\"background: #FFF;\">");
        stringBuilder.Append("<td style=\"padding: 3px;padding-left: 5px;box-sizing: border-box;width: 25%;font-size: 15px;border: 1px solid #E1C1B0;\">Tipo Exception</td>");
        stringBuilder.Append("<td style=\"padding: 3px;padding-left: 5px;box-sizing: border-box;width: 25%;font-size: 15px;border: 1px solid #E1C1B0;\">La URL real de la suite y la URL en base de datos no corresponden</td>");
        stringBuilder.Append("</tr>");
        stringBuilder.Append("<tr style=\"background: #FBE4D6;\">");
        stringBuilder.Append("<td style=\"padding: 3px;padding-left: 5px;box-sizing: border-box;width: 25%;font-size: 15px;border: 1px solid #E1C1B0;\">Módulo</td>");
        stringBuilder.Append("<td style=\"padding: 3px;padding-left: 5px;box-sizing: border-box;width: 25%;font-size: 15px;border: 1px solid #E1C1B0;\">" + App + "</td>");
        stringBuilder.Append("</tr>");

        stringBuilder.Append("<tr style=\"background: #FFF;\">");
        stringBuilder.Append("<td style=\"padding: 3px;padding-left: 5px;box-sizing: border-box;width: 25%;font-size: 15px;border: 1px solid #E1C1B0;\">URL Real de la suite</td>");
        stringBuilder.Append("<td style=\"padding: 3px;padding-left: 5px;box-sizing: border-box;width: 25%;font-size: 15px;border: 1px solid #E1C1B0;\">" + url_real + "</td>"); 
        stringBuilder.Append("</tr>");

        stringBuilder.Append("<tr style=\"background: #FBE4D6;\">");
        stringBuilder.Append("<td style=\"padding: 3px;padding-left: 5px;box-sizing: border-box;width: 25%;font-size: 15px;border: 1px solid #E1C1B0;\">URL Registrada en base de datos</td>");
        stringBuilder.Append("<td style=\"padding: 3px;padding-left: 5px;box-sizing: border-box;width: 25%;font-size: 15px;border: 1px solid #E1C1B0;\">" + url_BD + "</td>");
        stringBuilder.Append("</tr>");

        stringBuilder.Append("</table>");



        stringBuilder.Append("</div>");
        stringBuilder.Append("<div class=\"vrZone\" style=\"display: flex;-webkit-justify-content: center;-ms-flex-pack: center;justify-content: center;-webkit-align-items: center;-ms-flex-align: center;align-items: center;width: 100%;background: #535355;-webkit-justify-content: flex-start;-ms-flex-pack: start;justify-content: flex-start;margin-top: auto;\">");
        stringBuilder.Append("<div class=\"vcZone\" style=\"-webkit-justify-content: center;-ms-flex-pack: center;justify-content: center;-webkit-flex-direction: column;-ms-flex-direction: column;flex-direction: column;box-sizing: border-box;\">");

        stringBuilder.Append("<div class=\"vrZone\" style=\"display: flex;-webkit-justify-content: center;-ms-flex-pack: center;justify-content: center;-webkit-align-items: center;-ms-flex-align: center;align-items: center;width: 100%;-webkit-justify-content: flex-start;-ms-flex-pack: start;justify-content: flex-start;\">");
        stringBuilder.Append("<div class=\"sT2\" style=\"font-size: 15px;font-weight: 600;color: #FFF;width: 100%;padding: 5px 10px;box-sizing: border-box;\">OptiPlant Consultores S.A.S.</div>");
        stringBuilder.Append("</div>");
        stringBuilder.Append("<div class=\"vrZone\" style=\"display: flex;-webkit-justify-content: center;-ms-flex-pack: center;justify-content: center;-webkit-align-items: center;-ms-flex-align: center;align-items: center;width: 100%;-webkit-justify-content: flex-start;-ms-flex-pack: start;justify-content: flex-start;padding-left: 10px;\">");
        stringBuilder.Append("<div class=\"\" style=\"border-top: 2px solid #BA5B2C;max-width: 245px;width: 245px;\"></div>");
        stringBuilder.Append("</div>");

        stringBuilder.Append("<div class=\"vrZone\" style=\"display: flex;justify-content: center;align-items: center;width: 100%;justify-content: flex-start;\">");
        stringBuilder.Append("<div class=\"sT3\" style=\"font-size: 11px;color: #FFF;width: 100%;padding: 5px 15px;box-sizing: border-box;\">Armenia, Quindío, Colombia</div>");
        stringBuilder.Append("</div>");
        stringBuilder.Append("<div class=\"vrZone\" style=\"display: flex;justify-content: center;align-items: center;width: 100%;justify-content: flex-start;\">");
        //stringBuilder.Append("<div class=\"sT3\" style=\"font-size: 18px;color: #FFF;width: 100%;padding: 5px 15px;box-sizing: border-box;\">info@opc.com.co</div>");
        stringBuilder.Append("<div class=\"sT3\" style=\"font-size: 18px;color: #FFF;width: 100%;padding: 5px 15px;box-sizing: border-box;\">soporte@opc.com.co</div>");
        stringBuilder.Append("</div>");
        stringBuilder.Append("<div class=\"vrZone\" style=\"display: flex;justify-content: center;align-items: center;width: 100%;justify-content: flex-start;\">");
        stringBuilder.Append("<div class=\"sT3\" style=\"font-size: 11px;color: #FFF;width: 100%;padding: 5px 15px;box-sizing: border-box;\">(+57) 320 314 32 89 - (+57 6) 7 35 55 47</div>");
        stringBuilder.Append("</div>");


        stringBuilder.Append("</div>");
        stringBuilder.Append("</div>");
        message.Body = stringBuilder.ToString();
        message.Priority = MailPriority.High;
        message.IsBodyHtml = true;

        var smtp = new SmtpClient
        {
            Host = "smtp.gmail.com",
            Port = 587, //25
            EnableSsl = true, // false
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = true, // false
            Credentials = new NetworkCredential(mail, password)

        };
        conexion.closeConexion();
        smtp.Send(message);
    }

    public static void SendEmailState(string nombre, string email, string estado, string rol)
    {
        String mail;
        String password;
        String nombreRemitente;
        String asunto;

        ConexionSQL conexion = new ConexionSQL();
        conexion.openConexion();
        SqlDataAdapter adapter = new SqlDataAdapter(String.Format(@"

            SELECT
                Cuenta.mail,
                Cuenta.password,
                Cuenta.nombreRemitente,
                Cuenta.asunto

            FROM
                {0}.cuentasCorreos Cuenta
            WHERE
                Cuenta.nombre = 'NOTIFICACIÓN'
        ", Abi_maestro.esquema), conexion.getConexion());
        DataSet ds = new DataSet();
        adapter.Fill(ds);
        DataTable dt = ds.Tables[0];
        DataRow row = dt.Rows[0];

        mail = row["mail"].ToString();
        password = row["password"].ToString();
        nombreRemitente = row["nombreRemitente"].ToString();
        asunto = row["asunto"].ToString();

        MailAddress toAddress;
        var fromAddress = new MailAddress(mail, nombreRemitente);
        MailMessage message;
        message = new MailMessage();

        toAddress = new MailAddress(email);
        message = new MailMessage();
        message.From = fromAddress;
        message.Bcc.Add(toAddress);

        message.Subject = asunto + ": Actualización de ESTADO";
        StringBuilder stringBuilder = new StringBuilder();

        stringBuilder.Append("<html>");
        stringBuilder.Append("<body>");
        stringBuilder.Append("<div style=\"display: block;font-family: Heveltica, Arial, sans-serif;font-weight: normal;font-style: normal;line-height: 1.5;width: calc(100% - 30px);margin: 30px 5px;background-color: #f5f5f5;color: #000000;border-radius: 15px;\">");
        stringBuilder.Append("<div style=\"width: 100%;height: auto;\">");
        stringBuilder.Append("<span style=\"text-align: center;font-weight: 1000;font-size: 1.5rem;line-height: 1;letter-spacing: 0;padding: .25em 0 .325em;display: block;text-shadow: 0 0 80px rgba(255, 255, 255, .5);\">");
        stringBuilder.Append("Actualización de ESTADO - Banasan");
        stringBuilder.Append("</span>");
        stringBuilder.Append("</div>");
        stringBuilder.Append("<div class=\"img_text\" style=\"display: flex;\">");
        stringBuilder.Append("<center class=\"tam_cont\" style=\"width: 50%;height: auto;\">");
        stringBuilder.Append("<img src=\"https://desarrollosuitebanasan.demo.com.co/imagenes/banasanTituloSinFondo.png\" style =\"max-width: 80%;max-height: 90%;margin: auto;\">");
        stringBuilder.Append("</center>");
        stringBuilder.Append("<div class=\"tam_cont\" style=\"width: 50%;height: auto;\">");
        stringBuilder.Append("<span style=\"font-size: 1rem;line-height: 1;letter-spacing: 0;padding: 1.25em .325em;display: block;text-shadow: 0 0 80px rgba(255, 255, 255, .5);\">");
        stringBuilder.Append("Hola <strong>" + nombre + "</strong>,<br><br>El estado y rol de su cuenta han sido cambiados a:<br><br>");
        stringBuilder.Append("Estado:&nbsp;&nbsp;");
        stringBuilder.Append("<strong>" + estado + "</strong><br>");
        stringBuilder.Append("Rol:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;");
        stringBuilder.Append("<strong>" + rol + "</strong><br><br>");
        stringBuilder.Append("Muchas gracias.");
        stringBuilder.Append("</span>");
        stringBuilder.Append("</div>");
        stringBuilder.Append("</div>");
        stringBuilder.Append("</div>");
        stringBuilder.Append("</body>");
        stringBuilder.Append("</html>");

        ContentType mimeType = new System.Net.Mime.ContentType("text/html");

        AlternateView alternate = AlternateView.CreateAlternateViewFromString(stringBuilder.ToString(), mimeType);
        message.AlternateViews.Add(alternate);

        message.Priority = MailPriority.High;
        message.IsBodyHtml = true;

        var smtp = new SmtpClient
        {
            Host = "smtp.gmail.com",
            Port = 587, //25
            EnableSsl = true, // false
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = true, // false
            Credentials = new NetworkCredential(mail, password)

        };
        conexion.closeConexion();
        smtp.Send(message);
    }

    public static void SendEmailNewUser(string nombres, string apellidos, string email, string user)
    {
        String mail;
        String password;
        String nombreRemitente;
        String asunto;

        ConexionSQL conexion = new ConexionSQL();
        conexion.openConexion();
        SqlDataAdapter adapter = new SqlDataAdapter(String.Format(@"
            SELECT
                Cuenta.mail,
                Cuenta.password,
                Cuenta.nombreRemitente,
                Cuenta.asunto

            FROM
                {0}.cuentasCorreos Cuenta
            WHERE
                Cuenta.nombre = 'NOTIFICACIÓN'
        ",Abi_maestro.esquema), conexion.getConexion());
        DataSet ds = new DataSet();
        adapter.Fill(ds);
        DataTable dt = ds.Tables[0];
        DataRow row = dt.Rows[0];

        mail = row["mail"].ToString();
        password = row["password"].ToString();
        nombreRemitente = row["nombreRemitente"].ToString();
        asunto = row["asunto"].ToString();

        MailAddress toAddress;
        var fromAddress = new MailAddress(mail, nombreRemitente);
        MailMessage message;
        message = new MailMessage();

        adapter = new SqlDataAdapter(String.Format(@"
            SELECT
	            Usuario.nombres,
	            Usuario.correo
            FROM
	            {0}.usuario Usuario
            INNER JOIN {0}.estado_usuario EstadoUsuario ON EstadoUsuario.id_usuario = Usuario.id
            INNER JOIN {0}.rol Rol ON EstadoUsuario.id_rol = Rol.id
            INNER JOIN {0}.permisos_rol_menu Permisos ON Permisos.id_rol = Rol.id
            INNER JOIN {0}.menu Menus ON Permisos.id_menu = Menus.id
            WHERE
	            Menus.edit_state = 1
            AND Permisos.id_nivel_acceso = 2
        ", Abi_maestro.esquema), conexion.getConexion());

        ds = new DataSet();
        adapter.Fill(ds);
        dt = ds.Tables[0];

        for(int x = 0; x < dt.Rows.Count; x++)
        {

            row = dt.Rows[x];

            toAddress = new MailAddress(row["correo"].ToString());
            message = new MailMessage();
            message.From = fromAddress;
            message.Bcc.Add(toAddress);

            message.Subject = asunto + ": Solicitud de Acceso";
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append("<html>");

            stringBuilder.Append("<body>");
            stringBuilder.Append("<div style=\"display: block;font-family: Heveltica, Arial, sans-serif;font-weight: normal;font-style: normal;line-height: 1.5;width: calc(100% - 30px);margin: 30px 5px;background-color: #f5f5f5;color: #000000;border-radius: 15px;\">");

            stringBuilder.Append("<div style=\"width: 100%;height: auto;\">");
            stringBuilder.Append("<span style=\"text-align: center;font-weight: 1000;font-size: 1.5rem;line-height: 1;letter-spacing: 0;padding: .25em 0 .325em;display: block;text-shadow: 0 0 80px rgba(255, 255, 255, .5);\">");
            stringBuilder.Append("Solicitud de Acceso - Packaging 360");
            stringBuilder.Append("</span>");
            stringBuilder.Append("</div>");

            stringBuilder.Append("<div class=\"img_text\" style=\"display: flex;\">");
            stringBuilder.Append("<center class=\"tam_cont\" style=\"width: 50%;height: auto;\">");
            stringBuilder.Append("<img src=\"https://desarrollosuitebanasan.demo.com.co/imagenes/banasanTituloSinFondo.png\" style =\"max-width: 80%;max-height: 90%;margin: auto;\">");
            stringBuilder.Append("</center>");
            stringBuilder.Append("<div class=\"tam_cont\" style=\"width: 50%;height: auto;\">");
            stringBuilder.Append("<span style=\"font-size: 1rem;line-height: 1;letter-spacing: 0;padding: 1.25em .325em;display: block;text-shadow: 0 0 80px rgba(255, 255, 255, .5);\">");
            stringBuilder.Append("Hola " + row["nombres"] + ",<br><br>El siguiente usuario ha solicitado acceso a P360 Web:<br><br>");
            stringBuilder.Append("- " + nombres + " " + apellidos + "<br>");
            stringBuilder.Append("- " + user + "<br>");
            stringBuilder.Append("- " + email + "<br><br>");
            stringBuilder.Append("Por favor ingresa a P360 Web en el meú de gestión de usuarios para activarlo y asignarle un rol.<br><br>Muchas gracias.");
            stringBuilder.Append("</span>");
            stringBuilder.Append("</div>");
            stringBuilder.Append("</div>");

            stringBuilder.Append("</div>");

            stringBuilder.Append("</body>");
            stringBuilder.Append("</html>");

            ContentType mimeType = new System.Net.Mime.ContentType("text/html");

            AlternateView alternate = AlternateView.CreateAlternateViewFromString(stringBuilder.ToString(), mimeType);
            message.AlternateViews.Add(alternate);

            message.Priority = MailPriority.High;
            message.IsBodyHtml = true;

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587, //25
                EnableSsl = true, // false
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = true, // false
                Credentials = new NetworkCredential(mail, password)

            };
            smtp.Send(message);
        }
        conexion.closeConexion();
    }

    public static string SendEmail(string[] To, string Subject, string[] Titles, string Head, string[] Files)
    {

        try
        {
            MailMessage message = new MailMessage();

            String mail;
            String password;
            String nombreRemitente;
            String asunto;

            ConexionSQL conexion = new ConexionSQL();
            conexion.openConexion();
            SqlDataAdapter adapter = new SqlDataAdapter(String.Format(@"
                SELECT
                    Cuenta.mail,
                    Cuenta.password,
                    Cuenta.nombreRemitente,
                    Cuenta.asunto

                FROM
                    {0}.cuentasCorreos Cuenta
                WHERE
                    Cuenta.nombre = 'EXCEPCION'
            ", Abi_maestro.esquema), conexion.getConexion());
            DataSet ds = new DataSet();
            adapter.Fill(ds);
            DataTable dt = ds.Tables[0];
            DataRow row = dt.Rows[0];

            mail = row["mail"].ToString();
            password = row["password"].ToString();
            nombreRemitente = row["nombreRemitente"].ToString();
            asunto = row["asunto"].ToString();


            message.From = new MailAddress(mail, nombreRemitente);
            message.Bcc.Add(string.Join(",", To));
            message.Subject = Subject;

            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append("<div class=\"vZone\" style=\"width: 100%;font-family: 'Droid Sans', sans-serif;overflow: auto !important;\">");
            stringBuilder.Append("<div class=\"vcZone\" style=\"-webkit-flex-direction: column;-ms-flex-direction: column;flex-direction: column;box-sizing: border-box;min-height: 100%;\">");
            stringBuilder.Append("<div class=\"vrZone\" style=\"display: flex;-webkit-justify-content: center;-ms-flex-pack: center;justify-content: center;-webkit-align-items: center;-ms-flex-align: center;align-items: center;width: 100%;-webkit-justify-content: flex-start;-ms-flex-pack: start;justify-content: flex-start;\">");
            //stringBuilder.Append("<img src =\"http://opc.com.co/wp-content/uploads/2016/01/logo-292x139.png\" style=\"padding:10px; \"></div>");
            stringBuilder.Append("</div>");
            stringBuilder.Append("<div class=\"vrZone\" style=\"display: flex;-webkit-justify-content: center;-ms-flex-pack: center;justify-content: center;-webkit-align-items: center;-ms-flex-align: center;align-items: center;width: 100%;\">");
            stringBuilder.Append("<div class=\"sT1\" style=\"font-size: 26px;font-weight: 600;background: #535355;color: #FFF;width: 100%;padding: 10px 5px;box-sizing: border-box;\">" + Head + "</div></div>");
            stringBuilder.Append("<div class=\"vrZone\" style=\"display: flex;-webkit-justify-content: center;-ms-flex-pack: center;justify-content: center;-webkit-align-items: center;-ms-flex-align: center;align-items: center;width: 100%;\">");
            stringBuilder.Append("<table style=\"border-collapse: collapse;width: 100%;\">");

            stringBuilder.Append("<tr style=\"background: #e3c5b5;\">");
            stringBuilder.Append("<th style=\"width: 25%;font-size: 15px;font-weight: 600;padding: 3px;padding-left: 5px;box-sizing: border-box;border: 1px solid #ce9a7e;text-align: left;\">Información</th>");
            //stringBuilder.Append("<th style=\"width: 25%;font-size: 15px;font-weight: 600;padding: 3px;padding-left: 5px;box-sizing: border-box;border: 1px solid #ce9a7e;text-align: left;\">Dato</th>");
            stringBuilder.Append("</tr>");

            // Agregando el contenido
            for (int i = 0; i < Titles.Length; i++)
            {
                if (i % 2 == 0)
                {
                    stringBuilder.Append("<tr style=\"background: #FFF;\">");
                }
                else
                {
                    stringBuilder.Append("<tr style=\"background: #FBE4D6;\">");
                }

                stringBuilder.Append("<td style=\"padding: 3px;padding-left: 5px;box-sizing: border-box;width: 25%;font-size: 15px;border: 1px solid #E1C1B0;\">" + Titles[i] + "</td>");
                //stringBuilder.Append("<td style=\"padding: 3px;padding-left: 5px;box-sizing: border-box;width: 25%;font-size: 15px;border: 1px solid #E1C1B0;\">" + Values[i] + "</td>");
                stringBuilder.Append("</tr>");

            }

            stringBuilder.Append("</table>");
            stringBuilder.Append("</div>");
            stringBuilder.Append("<div class=\"vrZone\" style=\"display: flex;-webkit-justify-content: center;-ms-flex-pack: center;justify-content: center;-webkit-align-items: center;-ms-flex-align: center;align-items: center;width: 100%;background: #535355;-webkit-justify-content: flex-start;-ms-flex-pack: start;justify-content: flex-start;margin-top: auto;\">");
            stringBuilder.Append("<div class=\"vcZone\" style=\"-webkit-justify-content: center;-ms-flex-pack: center;justify-content: center;-webkit-flex-direction: column;-ms-flex-direction: column;flex-direction: column;box-sizing: border-box;\">");

            stringBuilder.Append("<div class=\"vrZone\" style=\"display: flex;-webkit-justify-content: center;-ms-flex-pack: center;justify-content: center;-webkit-align-items: center;-ms-flex-align: center;align-items: center;width: 100%;-webkit-justify-content: flex-start;-ms-flex-pack: start;justify-content: flex-start;\">");
            stringBuilder.Append("<div class=\"sT2\" style=\"font-size: 15px;font-weight: 600;color: #FFF;width: 100%;padding: 5px 10px;box-sizing: border-box;\">OptiPlant Consultores S.A.S.</div>");
            stringBuilder.Append("</div>");
            stringBuilder.Append("<div class=\"vrZone\" style=\"display: flex;-webkit-justify-content: center;-ms-flex-pack: center;justify-content: center;-webkit-align-items: center;-ms-flex-align: center;align-items: center;width: 100%;-webkit-justify-content: flex-start;-ms-flex-pack: start;justify-content: flex-start;padding-left: 10px;\">");
            stringBuilder.Append("<div class=\"\" style=\"border-top: 2px solid #BA5B2C;max-width: 245px;width: 245px;\"></div>");
            stringBuilder.Append("</div>");

            stringBuilder.Append("<div class=\"vrZone\" style=\"display: flex;justify-content: center;align-items: center;width: 100%;justify-content: flex-start;\">");
            stringBuilder.Append("<div class=\"sT3\" style=\"font-size: 11px;color: #FFF;width: 100%;padding: 5px 15px;box-sizing: border-box;\">Armenia, Quindío, Colombia</div>");
            stringBuilder.Append("</div>");
            stringBuilder.Append("<div class=\"vrZone\" style=\"display: flex;justify-content: center;align-items: center;width: 100%;justify-content: flex-start;\">");
            stringBuilder.Append("<div class=\"sT3\" style=\"font-size: 18px;color: #FFF;width: 100%;padding: 5px 15px;box-sizing: border-box;\">soporte@opc.com.co</div>");
            stringBuilder.Append("</div>");
            stringBuilder.Append("<div class=\"vrZone\" style=\"display: flex;justify-content: center;align-items: center;width: 100%;justify-content: flex-start;\">");
            stringBuilder.Append("<div class=\"sT3\" style=\"font-size: 11px;color: #FFF;width: 100%;padding: 5px 15px;box-sizing: border-box;\">(+57) 320 314 32 89 - (+57 6) 7 35 55 47</div>");

            stringBuilder.Append("</div>");
            stringBuilder.Append("</div>");
            stringBuilder.Append("</div>");

            message.Body = stringBuilder.ToString();
            message.Priority = MailPriority.High;
            message.IsBodyHtml = true;

            // Archivos adjuntos
            if (Files.Count() == 1 && Files[0] == "")
            {
                //no hay archivos para enviar
            }
            else
            {

                foreach (string Pathfile in Files)
                {
                    Attachment data = new Attachment(Pathfile);
                    message.Attachments.Add(data);
                }
            }
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587, //25
                EnableSsl = true, // false
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = true, // false
                Credentials = new NetworkCredential(mail, password)

            };

            smtp.Send(message);

            return "TRUE";

        }
        catch (Exception e)
        {
            return e.ToString();
        }

    }

    public static string SendEmail_Notificacion(String grupo_archivo, string tabla_insertar_registros)
    {
        String mail;
        String password;
        String nombreRemitente;
        String asunto;

        ConexionSQL conexion = new ConexionSQL();
        conexion.openConexion();
        SqlDataAdapter adapter = new SqlDataAdapter(String.Format(@"
            SELECT
                Cuenta.mail,
                Cuenta.password,
                Cuenta.nombreRemitente,
                Cuenta.asunto

            FROM
                {0}.cuentasCorreos Cuenta
            WHERE
                Cuenta.nombre = 'NOTIFICACIÓN'
        ", Abi_maestro.esquema), conexion.getConexion());

        ////////////////////
        DataSet ds = new DataSet();
        adapter.Fill(ds);
        DataTable dt = ds.Tables[0];
        DataRow row = dt.Rows[0];

        mail = row["mail"].ToString();
        password = row["password"].ToString();
        nombreRemitente = row["nombreRemitente"].ToString();
        asunto = row["asunto"].ToString();


        MailAddress toAddress;
        var fromAddress = new MailAddress(mail, nombreRemitente);
        MailMessage message;
        message = new MailMessage();

        adapter = new SqlDataAdapter(@"
                                        SELECT correo FROM correosNotificacion

                                      ", conexion.getConexion());

        ds = new DataSet();
        adapter.Fill(ds);
        dt = ds.Tables[0];
        row = dt.Rows[0];

        toAddress = new MailAddress(row[0].ToString());
        message = new MailMessage();
        message.From = fromAddress;
        message.Bcc.Add(toAddress);
        int J = 0;
        foreach (DataRow auxrow in dt.Rows)
        {
            if (J > 0)
            {
                message.Bcc.Add(auxrow[0].ToString());
            }
            J++;
        }

        try
        {
            message.Subject = asunto;

            adapter = new SqlDataAdapter(String.Format
            (@" 
            ---------------------------------------------------------------
                DECLARE @GRUPO_ARCHIVO VARCHAR(255)= '{0}'
                SELECT 
	                S.NOMBRE,
	                S.FechaInsercion,
	                @GRUPO_ARCHIVO AS GRUPO_ARCHIVO,
	                CASE 
		                WHEN S.FechaInsercion = 'NO SE INSERTARON DATOS' 
			                THEN 'FALLO' 
			                ELSE 'SE INSERTARON ' + CAST(COUNT(S.NOMBRE) AS VARCHAR(255)) + ' REGISTROS' 
	                END AS Estado
                FROM
                (
	                SELECT 	
		                Pais.NOMBRE,
		                ISNULL(CAST(FechaInsercion.fecha AS VARCHAR(255)), 'NO SE INSERTARON DATOS') AS FechaInsercion	
	                FROM Pais
	                LEFT JOIN {1} AS DEMANDA ON  Demanda.Id_pais = Pais.Id
	                LEFT JOIN FechaInsercion ON FechaInsercion.Id = Demanda.Id_FechaInsercion
	                --GROUP BY ID_PAIS
                ) AS S
                GROUP BY s.NOMBRE,S.FechaInsercion
            --------------------------------------------------------------------------
            ",grupo_archivo, tabla_insertar_registros), conexion.getConexion());

            ds = new DataSet();
            adapter.Fill(ds);
            DataTable tb = ds.Tables[0];
            StringBuilder stringBuilder = new StringBuilder();
            int i = 0;
            stringBuilder.Append("<div class=\"vZone\" style=\"width: 100%;height: 100vh;font-family: 'Droid Sans', sans-serif;overflow: auto !important;\">");
            stringBuilder.Append("<div class=\"vcZone\" style=\"-webkit-flex-direction: column;-ms-flex-direction: column;flex-direction: column;box-sizing: border-box;min-height: 100%;\">");
            stringBuilder.Append("<div class=\"vrZone\" style=\"display: flex;-webkit-justify-content: center;-ms-flex-pack: center;justify-content: center;-webkit-align-items: center;-ms-flex-align: center;align-items: center;width: 100%;-webkit-justify-content: flex-start;-ms-flex-pack: start;justify-content: flex-start;\">");
            stringBuilder.Append("<img src =\"http://opc.com.co/wp-content/uploads/2016/01/logo-292x139.png\" style=\"padding:10px; \"></div>");
            stringBuilder.Append("<div class=\"vrZone\" style=\"display: flex;-webkit-justify-content: center;-ms-flex-pack: center;justify-content: center;-webkit-align-items: center;-ms-flex-align: center;align-items: center;width: 100%;\">");
            stringBuilder.Append("<div class=\"sT1\" style=\"font-size: 26px;font-weight: 600;background: #535355;color: #FFF;width: 100%;padding: 10px 5px;box-sizing: border-box;\">Reporte de carga de archivos API ABI</div></div>");
            stringBuilder.Append("<div class=\"vrZone\" style=\"display: flex;-webkit-justify-content: center;-ms-flex-pack: center;justify-content: center;-webkit-align-items: center;-ms-flex-align: center;align-items: center;width: 100%;\">");
            stringBuilder.Append("<table style=\"border-collapse: collapse;width: 100%;\">");
            stringBuilder.Append("<tr style=\"background: #e3c5b5;\">");
            stringBuilder.Append("<th style=\"width: 25%;font-size: 15px;font-weight: 600;padding: 3px;padding-left: 5px;box-sizing: border-box;border: 1px solid #ce9a7e;text-align: left;\">PAÍS</th>");
            stringBuilder.Append("<th style=\"width: 25%;font-size: 15px;font-weight: 600;padding: 3px;padding-left: 5px;box-sizing: border-box;border: 1px solid #ce9a7e;text-align: left;\">FECHA INSERCIÓN</th>");
            stringBuilder.Append("<th style=\"width: 25%;font-size: 15px;font-weight: 600;padding: 3px;padding-left: 5px;box-sizing: border-box;border: 1px solid #ce9a7e;text-align: left;\">GRUPO ARCHIVO</th>");
            stringBuilder.Append("<th style=\"width: 25%;font-size: 15px;font-weight: 600;padding: 3px;padding-left: 5px;box-sizing: border-box;border: 1px solid #ce9a7e;text-align: left;\">ESTADO</th>");
            stringBuilder.Append("</tr>");
            foreach (DataRow auxrow in tb.Rows)
            {
                if ((i + 1) % 2 == 0)
                {
                    stringBuilder.Append("<tr style=\"background: #FBE4D6;\">");
                    stringBuilder.Append("<td style=\"padding: 3px;padding-left: 5px;box-sizing: border-box;width: 25%;font-size: 15px;border: 1px solid #E1C1B0;\">" + auxrow[0].ToString() + "</td>");
                    stringBuilder.Append("<td style=\"padding: 3px;padding-left: 5px;box-sizing: border-box;width: 25%;font-size: 15px;border: 1px solid #E1C1B0;\">" + auxrow[1].ToString() + "</td>");
                    stringBuilder.Append("<td style=\"padding: 3px;padding-left: 5px;box-sizing: border-box;width: 25%;font-size: 15px;border: 1px solid #E1C1B0;\">" + auxrow[2].ToString() + "</td>");
                    stringBuilder.Append("<td style=\"padding: 3px;padding-left: 5px;box-sizing: border-box;width: 25%;font-size: 15px;border: 1px solid #E1C1B0;\">" + auxrow[3].ToString() + "</td>");
                    stringBuilder.Append("</tr>");
                }
                else
                {
                    stringBuilder.Append("<tr style=\"background: #FFF;\">");
                    stringBuilder.Append("<td style=\"padding: 3px;padding-left: 5px;box-sizing: border-box;width: 25%;font-size: 15px;border: 1px solid #E1C1B0;\">" + auxrow[0].ToString() + "</td>");
                    stringBuilder.Append("<td style=\"padding: 3px;padding-left: 5px;box-sizing: border-box;width: 25%;font-size: 15px;border: 1px solid #E1C1B0;\">" + auxrow[1].ToString() + "</td>");
                    stringBuilder.Append("<td style=\"padding: 3px;padding-left: 5px;box-sizing: border-box;width: 25%;font-size: 15px;border: 1px solid #E1C1B0;\">" + auxrow[2].ToString() + "</td>");
                    stringBuilder.Append("<td style=\"padding: 3px;padding-left: 5px;box-sizing: border-box;width: 25%;font-size: 15px;border: 1px solid #E1C1B0;\">" + auxrow[3].ToString() + "</td>");
                    stringBuilder.Append("</tr>");

                }
                i++;
            }
            stringBuilder.Append("</table>");



            stringBuilder.Append("</div>");
            stringBuilder.Append("<div class=\"vrZone\" style=\"display: flex;-webkit-justify-content: center;-ms-flex-pack: center;justify-content: center;-webkit-align-items: center;-ms-flex-align: center;align-items: center;width: 100%;background: #535355;-webkit-justify-content: flex-start;-ms-flex-pack: start;justify-content: flex-start;margin-top: auto;\">");
            stringBuilder.Append("<div class=\"vcZone\" style=\"-webkit-justify-content: center;-ms-flex-pack: center;justify-content: center;-webkit-flex-direction: column;-ms-flex-direction: column;flex-direction: column;box-sizing: border-box;\">");

            stringBuilder.Append("<div class=\"vrZone\" style=\"display: flex;-webkit-justify-content: center;-ms-flex-pack: center;justify-content: center;-webkit-align-items: center;-ms-flex-align: center;align-items: center;width: 100%;-webkit-justify-content: flex-start;-ms-flex-pack: start;justify-content: flex-start;\">");
            stringBuilder.Append("<div class=\"sT2\" style=\"font-size: 15px;font-weight: 600;color: #FFF;width: 100%;padding: 5px 10px;box-sizing: border-box;\">OptiPlant Consultores S.A.S.</div>");
            stringBuilder.Append("</div>");
            stringBuilder.Append("<div class=\"vrZone\" style=\"display: flex;-webkit-justify-content: center;-ms-flex-pack: center;justify-content: center;-webkit-align-items: center;-ms-flex-align: center;align-items: center;width: 100%;-webkit-justify-content: flex-start;-ms-flex-pack: start;justify-content: flex-start;padding-left: 10px;\">");
            stringBuilder.Append("<div class=\"\" style=\"border-top: 2px solid #BA5B2C;max-width: 245px;width: 245px;\"></div>");
            stringBuilder.Append("</div>");

            stringBuilder.Append("<div class=\"vrZone\" style=\"display: flex;justify-content: center;align-items: center;width: 100%;justify-content: flex-start;\">");
            stringBuilder.Append("<div class=\"sT3\" style=\"font-size: 11px;color: #FFF;width: 100%;padding: 5px 15px;box-sizing: border-box;\">Armenia, Quindío, Colombia</div>");
            stringBuilder.Append("</div>");
            stringBuilder.Append("<div class=\"vrZone\" style=\"display: flex;justify-content: center;align-items: center;width: 100%;justify-content: flex-start;\">");
            stringBuilder.Append("<div class=\"sT3\" style=\"font-size: 18px;color: #FFF;width: 100%;padding: 5px 15px;box-sizing: border-box;\">soporte@opc.com.co</div>");
            stringBuilder.Append("</div>");
            stringBuilder.Append("<div class=\"vrZone\" style=\"display: flex;justify-content: center;align-items: center;width: 100%;justify-content: flex-start;\">");
            stringBuilder.Append("<div class=\"sT3\" style=\"font-size: 11px;color: #FFF;width: 100%;padding: 5px 15px;box-sizing: border-box;\">(+57) 320 314 32 89 - (+57 6) 7 35 55 47</div>");
            stringBuilder.Append("</div>");


            stringBuilder.Append("</div>");
            stringBuilder.Append("</div>");


            message.Body = stringBuilder.ToString();
            message.Priority = MailPriority.High;
            message.IsBodyHtml = true;
            
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587, //25
                EnableSsl = true, // false
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = true, // false
                Credentials = new NetworkCredential(mail, password)
            };


            smtp.Send(message);
            conexion.closeConexion();
            return "TRUE";

        }
        catch (Exception e)
        {
            string host = HttpContext.Current.Request.Url.Host;
            Mail.SendEmail(e, host, tabla_insertar_registros);
            return e.ToString();
        }

    }
}