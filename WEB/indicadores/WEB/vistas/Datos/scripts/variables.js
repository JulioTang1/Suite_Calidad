var Datos = new Object;

Datos.urlConexion = primario.urlConexion;//DataTable
Datos.urlConexionVista = "/apiServices/Datos.asmx";//Vista

Datos.fechaIniRank = null;
Datos.fechaFinRank = null;

// BANDERA PARA HABILITAR O NO EDICION
// 0 --> Solo visualizacion
// 1 --> Visualizacion y edicion
Datos.baderaPermisosEdicion = Sidebar.crud_access;

// información de la tabla actual
Datos.$tabla = new Array();

/* SELECTORES ANIDADOS */
Datos.filtros = new Object();

Datos.filtros.departamento = new Object();
Datos.filtros.departamento.data = '0'
Datos.filtros.departamento.state = 0;
Datos.filtros.departamento.uso = 0;

Datos.filtros.municipio = new Object();
Datos.filtros.municipio.data = '0';
Datos.filtros.municipio.state = 0;
Datos.filtros.municipio.uso = 0;

Datos.filtros.finca = new Object();
Datos.filtros.finca.data = '0';
Datos.filtros.finca.state = 0;
Datos.filtros.finca.uso = 0;
/* FIN SELECTORES ANIDADOS */