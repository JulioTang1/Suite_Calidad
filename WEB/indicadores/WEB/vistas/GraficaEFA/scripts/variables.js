var GraficaEFA = new Object;

GraficaEFA.urlConexion = "/apiServices/WS.asmx";//DataTable
GraficaEFA.urlConexionVista = "/apiServices/GraficaEFA.asmx";//Vista

GraficaEFA.fechaIniRank = null;
GraficaEFA.fechaFinRank = null;

GraficaEFA.countGraph = 0;

/* SELECTORES ANIDADOS */
GraficaEFA.filtros = new Object();

GraficaEFA.filtros.departamento = new Object();
GraficaEFA.filtros.departamento.data = '0'
GraficaEFA.filtros.departamento.state = 0;
GraficaEFA.filtros.departamento.uso = 0;

GraficaEFA.filtros.municipio = new Object();
GraficaEFA.filtros.municipio.data = '0';
GraficaEFA.filtros.municipio.state = 0;
GraficaEFA.filtros.municipio.uso = 0;

GraficaEFA.filtros.finca = new Object();
GraficaEFA.filtros.finca.data = '0';
GraficaEFA.filtros.finca.state = 0;
GraficaEFA.filtros.finca.uso = 0;
/* FIN SELECTORES ANIDADOS */