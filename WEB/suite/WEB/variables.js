
var Login = new Object();
var primario = new Object();

// Se pasa selecciona el tipo de menu que se quiere, 'dark' para un menu oscuro 
// y 'light' para un menu claro, si se deja vacío o con otro string, por defecto estara en dark.
primario.menu = 'light';

//Conexión al servidor
primario.urlConexion="/apiServices/WS.asmx";
primario.urlConexionLogin="/apiServices/Service.asmx";

//Aplicación
primario.aplicacion = "Suite Banasan";

//Imagenes
primario.imagen_principal = "imagenes/banasanTituloSinFondo.png"
primario.imagen_registro = "imagenes/banasanTituloSinFondo.png"

primario.banderaParaEdicionAtributo = 0;
primario.IdFilaTabla = 0;

primario.editTimes = 0;
primario.editValores = 0;

primario.correo = '';
primario.user = '';
primario.nombre = '';
primario.apellido = '';
primario.pais = '';
primario.departamento = '';
primario.ciudad = '';
primario.cargo = '';
primario.nombre_usuario = '';

//Variable que almacenara la función de actualización de las tarjetas
// despues de la sincronización en la vista de registro de tiempos
primario.refresh = null;
