var Bioseguridad = new Object;

Bioseguridad.urlConexion = "/apiServices/WS.asmx";//DataTable
Bioseguridad.urlConexionVista = "/apiServices/Bioseguridad.asmx";//Vista

Bioseguridad.fechaIniRank = null;
Bioseguridad.fechaFinRank = null;

Bioseguridad.edicion = 0;

Bioseguridad.ultimoElementoConPopoever = "";

// BANDERA PARA HABILITAR O NO EDICION
// 0 --> Solo visualizacion
// 1 --> Visualizacion y edicion
Bioseguridad.baderaPermisosEdicion = Sidebar.crud_access;

// información de la tabla actual
Bioseguridad.$tabla;

/* SELECTORES ANIDADOS */
Bioseguridad.filtros = new Object();

Bioseguridad.filtros.departamento = new Object();
Bioseguridad.filtros.departamento.data = '0'
Bioseguridad.filtros.departamento.state = 0;
Bioseguridad.filtros.departamento.uso = 0;

Bioseguridad.filtros.municipio = new Object();
Bioseguridad.filtros.municipio.data = '0';
Bioseguridad.filtros.municipio.state = 0;
Bioseguridad.filtros.municipio.uso = 0;

Bioseguridad.filtros.finca = new Object();
Bioseguridad.filtros.finca.data = '0';
Bioseguridad.filtros.finca.state = 0;
Bioseguridad.filtros.finca.uso = 0;
/* FIN SELECTORES ANIDADOS */