var Consolidado = new Object;

Consolidado.urlConexion = primario.urlConexion;//DataTable
Consolidado.urlConexionVista = "/apiServices/Consolidado.asmx";//Vista

Consolidado.fechaIniRank = null;
Consolidado.fechaFinRank = null;

// BANDERA PARA HABILITAR O NO EDICION
// 0 --> Solo visualizacion
// 1 --> Visualizacion y edicion
Consolidado.baderaPermisosEdicion = Sidebar.crud_access;

// información de la tabla actual
Consolidado.$tabla = new Array();

/* SELECTORES ANIDADOS */
Consolidado.filtros = new Object();

Consolidado.filtros.departamento = new Object();
Consolidado.filtros.departamento.data = '0'
Consolidado.filtros.departamento.state = 0;
Consolidado.filtros.departamento.uso = 0;

Consolidado.filtros.municipio = new Object();
Consolidado.filtros.municipio.data = '0';
Consolidado.filtros.municipio.state = 0;
Consolidado.filtros.municipio.uso = 0;

Consolidado.filtros.finca = new Object();
Consolidado.filtros.finca.data = '0';
Consolidado.filtros.finca.state = 0;
Consolidado.filtros.finca.uso = 0;
/* FIN SELECTORES ANIDADOS */