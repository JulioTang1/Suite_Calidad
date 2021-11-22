var RecorridoFincas = new Object;

RecorridoFincas.urlConexion = "/apiServices/WS.asmx";//DataTable
RecorridoFincas.urlConexionVista = "/apiServices/RecorridoFincas.asmx";//Vista

RecorridoFincas.fechaIniRank = null;
RecorridoFincas.fechaFinRank = null;

// BANDERA PARA HABILITAR O NO EDICION
// 0 --> Solo visualizacion
// 1 --> Visualizacion y edicion
RecorridoFincas.baderaPermisosEdicion = Sidebar.crud_access;

// información de la tabla actual
RecorridoFincas.$tabla;

// visita seleccionada actualmente
RecorridoFincas.idVisita = 0;

// ultimo mapa pedido
RecorridoFincas.mapRequest = 0;

/* SELECTORES ANIDADOS */
RecorridoFincas.filtros = new Object();

RecorridoFincas.filtros.departamento = new Object();
RecorridoFincas.filtros.departamento.data = '0'
RecorridoFincas.filtros.departamento.state = 0;
RecorridoFincas.filtros.departamento.uso = 0;

RecorridoFincas.filtros.municipio = new Object();
RecorridoFincas.filtros.municipio.data = '0';
RecorridoFincas.filtros.municipio.state = 0;
RecorridoFincas.filtros.municipio.uso = 0;

RecorridoFincas.filtros.finca = new Object();
RecorridoFincas.filtros.finca.data = '0';
RecorridoFincas.filtros.finca.state = 0;
RecorridoFincas.filtros.finca.uso = 0;
/* FIN SELECTORES ANIDADOS */

/* SELECTORES ANIDADOS DE EDADES INDICADORES */
RecorridoFincas.filtrosEI = new Object();

RecorridoFincas.filtrosEI.edad = new Object();
RecorridoFincas.filtrosEI.edad.data = '0'
RecorridoFincas.filtrosEI.edad.state = 0;
RecorridoFincas.filtrosEI.edad.uso = 0;

RecorridoFincas.filtrosEI.indicador = new Object();
RecorridoFincas.filtrosEI.indicador.data = '0';
RecorridoFincas.filtrosEI.indicador.state = 0;
RecorridoFincas.filtrosEI.indicador.uso = 0;
/* FIN SELECTORES ANIDADOS DE EDADES INDICADORES */