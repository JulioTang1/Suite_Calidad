var Roles = new Object;

Roles.urlConexion = "/apiServices/WS.asmx";//data table
Roles.urlConexionVista = "/apiServices/Roles.asmx";//vista
Roles.ultimoElementoConPopoever = "";

// BANDERA PARA HABILITAR O NO EDICION
// 0 --> Solo visualizacion
// 1 --> Visualizacion y edicion
Roles.baderaPermisosEdicion = Sidebar.crud_access;

Roles.$tabla;
Roles.Info_NewRol;