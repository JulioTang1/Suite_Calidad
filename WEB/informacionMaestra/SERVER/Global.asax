<%@ Application Language="C#" %>

<script runat="server">

    bool globalError = false;
    bool callRedirect = false;

    void Application_Start(object sender, EventArgs e)
    {
        //Captura global de errores
        //Cualquier error no controlado, es capturado por esta excepción
        AppDomain.CurrentDomain.FirstChanceException += (senderCatch, eventArgs) =>{
            //string host = HttpContext.Current.Request.Url.Host;
            //Mail.SendEmail(eventArgs.Exception, host, "");           
        };
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        Exception ex = Server.GetLastError();

        if (ex != null)
        {
            globalError = true;
        }
    }

    void Application_AcquireRequestState(object sender, EventArgs e)
    {
        HttpContext Context = HttpContext.Current;

        //URL's excluidas del manejo de la validación de sesión, pero que hacen uso del objeto Session
        if (Context.Request.Url.LocalPath != "/token.asmx/Validar_token" &&
            Context.Request.Url.LocalPath != "/apiServices/token.asmx/Validar_token" &&
            Context.Request.Url.LocalPath != "/SERVER/token.asmx/Validar_token" &&
            Context.Request.Url.LocalPath != "/token.asmx/error_iframe" &&
            Context.Request.Url.LocalPath != "/apiServices/token.asmx/error_iframe" &&
            Context.Request.Url.LocalPath != "/SERVER/token.asmx/error_iframe" &&
            Context.Request.Url.LocalPath != "/token.asmx/error_url_DB" &&
            Context.Request.Url.LocalPath != "/apiServices/token.asmx/error_url_DB" &&
            Context.Request.Url.LocalPath != "/SERVER/token.asmx/error_url_DB")
        {

            if (Context.Session != null){
                HttpSessionState session = Context.Session;
                if (session["Email"] == null)
                {
                    HttpContext.Current.Response.StatusCode = 601;
                    HttpContext.Current.Response.End();
                }
                else
                {
                    session SS = new session();
                    SS.UpdateSession();
                }
            }
        }
    }

    protected void Application_BeginRequest(object sender, EventArgs e)
    {
        //Verificación de errores globales.
        if (globalError)
        {
            if (HttpContext.Current != null)
            {
                //Redireccionamiento 404 para cualquier error encontrado en la plataforma
                HttpContext.Current.Response.StatusCode = 601;
                HttpContext.Current.Response.End();
            }
        }
    }

    void Application_End(object sender, EventArgs e)
    {
        //Verificación de errores globales.
        if (globalError)
        {
            //Redireccionamiento 404 para cualquier error encontrado en la plataforma
            HttpContext.Current.Response.StatusCode = 601;
            HttpContext.Current.Response.End();
        }
    }

    void Application_Error(object sender, EventArgs e)
    {
        //Captura global de errores
        //Cualquier error no controlado, es capturado por esta excepción
        Exception exc = Server.GetLastError();

        if (exc is HttpUnhandledException)
        {
            if (exc.InnerException != null)
            {
                if (HttpContext.Current != null)
                {
                    //En caso de que se requiera una notificación global de errores no controlados
                    exc = new Exception(exc.InnerException.Message);
                    string host = HttpContext.Current.Request.Url.Host;
                    //Mail.SendEmail(exc, host, "");
                    Server.ClearError();
                    HttpContext.Current.Response.StatusCode = 601;
                    HttpContext.Current.Response.End();
                }
            }
        }
    }

    void Session_End(object sender, EventArgs e)
    {

    }

</script>
