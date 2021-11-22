var Permisos_Recurso = new Object;

Permisos_Recurso.urlConexion = "/apiServices/WS.asmx";//data table
Permisos_Recurso.urlConexionVista = "/apiServices/Perfiles.asmx";//vista
Permisos_Recurso.ultimoElementoConPopoever = "";

// BANDERA PARA HABILITAR O NO EDICION
// 0 --> Solo visualizacion
// 1 --> Visualizacion y edicion
Permisos_Recurso.baderaPermisosEdicion = Sidebar.crud_access;

Permisos_Recurso.$tabla;
Permisos_Recurso.Info_NewPerfil;