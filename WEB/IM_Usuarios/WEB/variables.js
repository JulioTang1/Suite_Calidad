var primario = new Object();
var Login = new Object();

// Se pasa selecciona el tipo de menu que se quiere, 'dark' para un menu oscuro 
// y 'light' para un menu claro, si se deja vacío o con otro string, por defecto estara en dark.
primario.menu = 'light';

//Conexión al servidor
primario.urlConexion = "/apiServices/WS.asmx";
primario.urlConexionToken = "/apiServices/token.asmx";


//Aplicación
primario.aplicacion = "IM Gestion Usuarios";

primario.banderaParaEdicionAtributo = 0;
primario.IdFilaTabla = 0;

primario.editTimes = 0;
primario.editValores = 0;

/**************************************************************/ 

primario.window_source = "";
primario.window_origin = "";		
