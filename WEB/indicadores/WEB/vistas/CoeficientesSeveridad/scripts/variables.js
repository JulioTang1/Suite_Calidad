var CoeficientesSeveridad = new Object;

CoeficientesSeveridad.urlConexion = primario.urlConexion;//DataTable
CoeficientesSeveridad.urlConexionVista = "/apiServices/CoeficientesSeveridad.asmx";//Vista

CoeficientesSeveridad.ultimoElementoConPopoever = "";

// BANDERA PARA HABILITAR O NO EDICION
// 0 --> Solo visualizacion
// 1 --> Visualizacion y edicion
CoeficientesSeveridad.baderaPermisosEdicion = Sidebar.crud_access;

// información de la tabla actual
CoeficientesSeveridad.$tabla;