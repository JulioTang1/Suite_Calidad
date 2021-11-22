var Metas = new Object;

Metas.urlConexion = primario.urlConexion;//DataTable
Metas.urlConexionVista = "/apiServices/Metas.asmx";//Vista

Metas.ultimoElementoConPopoever = "";

// BANDERA PARA HABILITAR O NO EDICION
// 0 --> Solo visualizacion
// 1 --> Visualizacion y edicion
Metas.baderaPermisosEdicion = Sidebar.crud_access;

// información de la tabla actual
Metas.$tabla;

/* SELECTORES ANIDADOS DE EDADES INDICADORES */
Metas.filtrosEI = new Object();

Metas.filtrosEI.edad = new Object();
Metas.filtrosEI.edad.data = '0'
Metas.filtrosEI.edad.state = 0;
Metas.filtrosEI.edad.uso = 0;

Metas.filtrosEI.indicador = new Object();
Metas.filtrosEI.indicador.data = '0';
Metas.filtrosEI.indicador.state = 0;
Metas.filtrosEI.indicador.uso = 0;
/* FIN SELECTORES ANIDADOS DE EDADES INDICADORES */