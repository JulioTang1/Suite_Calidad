using System;
using System.Web;
using System.Web.Services;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;

public class CoeficientesSeveridad : WebService
{
    //SE ACTUALIZA LA INFORMACION DE M_ESTADO
    [WebMethod(EnableSession = true)]
    public string edit(int id, float dos, float tres, float cuatro)
    {
        //Aca se guarda el JObject consultas
        JObject result = new JObject();
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();
        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                //Se actualiza codigo y nombre de la finca
                adapter = new SqlDataAdapter(String.Format(@"

                        UPDATE dbo.M_estado
                        SET
	                        II = @dos,
	                        III = @tres,
                            IV = @cuatro
                        WHERE id = @id

                    "), conexion.getConexion());
                adapter.SelectCommand.Parameters.AddWithValue("@id", id);
                adapter.SelectCommand.Parameters.AddWithValue("@dos", dos);
                adapter.SelectCommand.Parameters.AddWithValue("@tres", tres);
                adapter.SelectCommand.Parameters.AddWithValue("@cuatro", cuatro);
                adapter.SelectCommand.ExecuteScalar();

                result["ESTADO"] = "TRUE";
                result["MENSAJE"] = "Consulta Correcta.";
                conexion.closeConexion();
            }
            catch (Exception e)
            {
                result["ESTADO"] = "FALSE";
                result["MENSAJE"] = "ERROR";
                string host = HttpContext.Current.Request.Url.Host;
                Mail.SendEmail(e, host, string.Format(@"id={0}, dos={1}, tres={2}, cuatro={3}", id, dos, tres, cuatro));
                conexion.closeConexion();
            }
        }
        else
        {
            result["ESTADO"] = "FALSE";
            result["MENSAJE"] = "ERROR DE LA CONEXIÓN";
            conexion.closeConexion();
        }

        Context.Response.Output.Write(result);
        Context.Response.End();
        return result.ToString();
    }
}
