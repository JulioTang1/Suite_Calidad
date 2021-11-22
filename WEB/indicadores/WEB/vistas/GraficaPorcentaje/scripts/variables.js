var GraficaPorcentaje = new Object;

GraficaPorcentaje.urlConexion = "/apiServices/WS.asmx";//DataTable
GraficaPorcentaje.urlConexionVista = "/apiServices/GraficaPorcentaje.asmx";//Vista

GraficaPorcentaje.fechaIniRank = null;
GraficaPorcentaje.fechaFinRank = null;

GraficaPorcentaje.countGraph = 0;

/* SELECTORES ANIDADOS */
GraficaPorcentaje.filtros = new Object();

GraficaPorcentaje.filtros.departamento = new Object();
GraficaPorcentaje.filtros.departamento.data = '0'
GraficaPorcentaje.filtros.departamento.state = 0;
GraficaPorcentaje.filtros.departamento.uso = 0;

GraficaPorcentaje.filtros.municipio = new Object();
GraficaPorcentaje.filtros.municipio.data = '0';
GraficaPorcentaje.filtros.municipio.state = 0;
GraficaPorcentaje.filtros.municipio.uso = 0;

GraficaPorcentaje.filtros.finca = new Object();
GraficaPorcentaje.filtros.finca.data = '0';
GraficaPorcentaje.filtros.finca.state = 0;
GraficaPorcentaje.filtros.finca.uso = 0;
/* FIN SELECTORES ANIDADOS */