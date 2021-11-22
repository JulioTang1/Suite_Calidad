var GraficaPrecipitaciones = new Object;

GraficaPrecipitaciones.urlConexion = "/apiServices/WS.asmx";//DataTable
GraficaPrecipitaciones.urlConexionVista = "/apiServices/GraficaPrecipitaciones.asmx";//Vista

GraficaPrecipitaciones.fechaIniRank = null;
GraficaPrecipitaciones.fechaFinRank = null;

GraficaPrecipitaciones.countGraph = 0;

/* SELECTORES ANIDADOS */
GraficaPrecipitaciones.filtros = new Object();

GraficaPrecipitaciones.filtros.departamento = new Object();
GraficaPrecipitaciones.filtros.departamento.data = '0'
GraficaPrecipitaciones.filtros.departamento.state = 0;
GraficaPrecipitaciones.filtros.departamento.uso = 0;

GraficaPrecipitaciones.filtros.municipio = new Object();
GraficaPrecipitaciones.filtros.municipio.data = '0';
GraficaPrecipitaciones.filtros.municipio.state = 0;
GraficaPrecipitaciones.filtros.municipio.uso = 0;

GraficaPrecipitaciones.filtros.finca = new Object();
GraficaPrecipitaciones.filtros.finca.data = '0';
GraficaPrecipitaciones.filtros.finca.state = 0;
GraficaPrecipitaciones.filtros.finca.uso = 0;
/* FIN SELECTORES ANIDADOS */