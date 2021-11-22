var Estado_Usuario = new Object;

Estado_Usuario.urlConexion = "/apiServices/WS.asmx";//data table
Estado_Usuario.urlConexionVista = "/apiServices/Estado_Usuario.asmx";//vista
Estado_Usuario.ultimoElementoConPopoever = "";

// BANDERA PARA HABILITAR O NO EDICION
// 0 --> Solo visualizacion
// 1 --> Visualizacion y edicion
Estado_Usuario.baderaPermisosEdicion = Sidebar.crud_access;

Estado_Usuario.$tabla;