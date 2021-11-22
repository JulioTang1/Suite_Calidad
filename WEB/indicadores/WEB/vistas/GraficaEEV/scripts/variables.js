var GraficaEEV = new Object;

GraficaEEV.urlConexion = "/apiServices/WS.asmx";//DataTable
GraficaEEV.urlConexionVista = "/apiServices/GraficaEEV.asmx";//Vista

GraficaEEV.fechaIniRank = null;
GraficaEEV.fechaFinRank = null;

GraficaEEV.countGraph = 0;

/* SELECTORES ANIDADOS */
GraficaEEV.filtros = new Object();

GraficaEEV.filtros.departamento = new Object();
GraficaEEV.filtros.departamento.data = '0'
GraficaEEV.filtros.departamento.state = 0;
GraficaEEV.filtros.departamento.uso = 0;

GraficaEEV.filtros.municipio = new Object();
GraficaEEV.filtros.municipio.data = '0';
GraficaEEV.filtros.municipio.state = 0;
GraficaEEV.filtros.municipio.uso = 0;

GraficaEEV.filtros.finca = new Object();
GraficaEEV.filtros.finca.data = '0';
GraficaEEV.filtros.finca.state = 0;
GraficaEEV.filtros.finca.uso = 0;
/* FIN SELECTORES ANIDADOS */