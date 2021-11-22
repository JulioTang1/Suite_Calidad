var GraficaIndicadores = new Object;

GraficaIndicadores.urlConexion = "/apiServices/WS.asmx";//DataTable
GraficaIndicadores.urlConexionVista = "/apiServices/GraficaIndicadores.asmx";//Vista

GraficaIndicadores.fechaIniRank = null;
GraficaIndicadores.fechaFinRank = null;

GraficaIndicadores.countGraph = 0;

/* SELECTORES ANIDADOS */
GraficaIndicadores.filtros = new Object();

GraficaIndicadores.filtros.departamento = new Object();
GraficaIndicadores.filtros.departamento.data = '0'
GraficaIndicadores.filtros.departamento.state = 0;
GraficaIndicadores.filtros.departamento.uso = 0;

GraficaIndicadores.filtros.municipio = new Object();
GraficaIndicadores.filtros.municipio.data = '0';
GraficaIndicadores.filtros.municipio.state = 0;
GraficaIndicadores.filtros.municipio.uso = 0;

GraficaIndicadores.filtros.finca = new Object();
GraficaIndicadores.filtros.finca.data = '0';
GraficaIndicadores.filtros.finca.state = 0;
GraficaIndicadores.filtros.finca.uso = 0;
/* FIN SELECTORES ANIDADOS */

/* SELECTORES ANIDADOS DE EDADES INDICADORES */
GraficaIndicadores.filtrosEI = new Object();

GraficaIndicadores.filtrosEI.edad = new Object();
GraficaIndicadores.filtrosEI.edad.data = '0'
GraficaIndicadores.filtrosEI.edad.state = 0;
GraficaIndicadores.filtrosEI.edad.uso = 0;

GraficaIndicadores.filtrosEI.indicador = new Object();
GraficaIndicadores.filtrosEI.indicador.data = '0';
GraficaIndicadores.filtrosEI.indicador.state = 0;
GraficaIndicadores.filtrosEI.indicador.uso = 0;
/* FIN SELECTORES ANIDADOS DE EDADES INDICADORES */