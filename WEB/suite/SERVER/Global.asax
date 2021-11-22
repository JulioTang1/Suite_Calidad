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
        //URL SIN SESION
        //-/Services.asmx/user
        //-/Services.asmx/SSOValidation
        //-/Service.asmx/SSO
        //-/Service.asmx/haveSession

        //-/apiServices/Services.asmx/user
        //-/apiServices/Services.asmx/SSOValidation
        //-/apiServices/Service.asmx/SSO
        //-/apiServices/Service.asmx/haveSession

        HttpContext context = HttpContext.Current;
        // CheckSession() inlined

        //URL's excluidas del manejo de la validación de sesión, pero que hacen uso del objeto Session
        if (Context.Request.Url.LocalPath != "/Service.asmx/SSOValidation" &&
            Context.Request.Url.LocalPath != "/Service.asmx/SSO" &&
            Context.Request.Url.LocalPath != "/Service.asmx/haveSession" &&
            Context.Request.Url.LocalPath != "/apiServices/Service.asmx/SSOValidation" &&
            Context.Request.Url.LocalPath != "/apiServices/Service.asmx/SSO" &&
            Context.Request.Url.LocalPath != "/apiServices/Service.asmx/haveSession" &&
            Context.Request.Url.LocalPath != "/apiServices/WS.asmx/updateSession" &&
            Context.Request.Url.LocalPath != "/WS.asmx/updateSession" &&
            Context.Request.Url.LocalPath != "/Service.asmx/SSOBanasan"  &&
            Context.Request.Url.LocalPath != "/apiServices/Service.asmx/SSOBanasan"
)
        {

            //this.Session
            //context.Session
            //HttpApplication.Session
            //HttpContext.Current.Session

            if (HttpContext.Current.Session != null){
                HttpSessionState session = HttpContext.Current.Session;
                if (session["Email"] == null)
                {
                    HttpContext.Current.Response.StatusCode = 602;
                    HttpContext.Current.Response.End();
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
                HttpContext.Current.Response.StatusCode = 602;
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
            HttpContext.Current.Response.StatusCode = 602;
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
                    HttpContext.Current.Response.StatusCode = 602;
                    HttpContext.Current.Response.End();
                }
            }
        }
    }

    void Session_End(object sender, EventArgs e)
    {

    }

</script>
