
var Sidebar = new Object();

// variable para indicar cuando el sidebar se abra por primera vez
Sidebar.open = 0;

//indica el estado de la barra, 0 --> Cerrado, 1 --> Abierto
Sidebar.status = 0;

// Se pasa selecciona el tipo de menu que se quiere, 'dark' para un menu oscuro 
// y 'light' para un menu claro, si se deja vacío o con otro string, por defecto estara en dark.
Sidebar.menu = primario.menu;

// variables para saber si esta en curso la animación y en que contenedor se esta produciendo la
// animación
Sidebar.flagAnimation = 0;
Sidebar.flagPosition = '';

//Conexión al servidor
Sidebar.urlConexion = primario.urlConexion;
Sidebar.urlConexionSession = "/apiServices/session.asmx";

// nivel de acceso en los menus
Sidebar.menus_ID = new Array();
Sidebar.Acceso 	 = new Array();
Sidebar.crud_access = 0;

Sidebar.menuIDActual;
Sidebar.menuNombreActual;