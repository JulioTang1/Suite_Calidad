var Fincas = new Object;

Fincas.urlConexion = primario.urlConexion;//DataTable
Fincas.urlConexionVista = "/apiServices/Fincas.asmx";//Vista
Fincas.id = 0;
Fincas.edicion = 0;

Fincas.ultimoElementoConPopoever = "";

// BANDERA PARA HABILITAR O NO EDICION
// 0 --> Solo visualizacion
// 1 --> Visualizacion y edicion
Fincas.baderaPermisosEdicion = Sidebar.crud_access;

// información de la tabla actual
Fincas.$tabla;