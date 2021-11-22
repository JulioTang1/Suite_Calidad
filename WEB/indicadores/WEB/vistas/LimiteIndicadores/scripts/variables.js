var LimiteIndicadores = new Object;

LimiteIndicadores.urlConexion = "/apiServices/WS.asmx";//DataTable
LimiteIndicadores.urlConexionVista = "/apiServices/LimiteIndicadores.asmx";//Vista

/* SELECTORES ANIDADOS DE EDADES INDICADORES */
LimiteIndicadores.filtrosEI = new Object();

LimiteIndicadores.filtrosEI.edad = new Object();
LimiteIndicadores.filtrosEI.edad.data = '0'
LimiteIndicadores.filtrosEI.edad.state = 0;
LimiteIndicadores.filtrosEI.edad.uso = 0;

LimiteIndicadores.filtrosEI.indicador = new Object();
LimiteIndicadores.filtrosEI.indicador.data = '0';
LimiteIndicadores.filtrosEI.indicador.state = 0;
LimiteIndicadores.filtrosEI.indicador.uso = 0;

LimiteIndicadores.filtrosEI.tipoFinca = new Object();
LimiteIndicadores.filtrosEI.tipoFinca.data = '0';
LimiteIndicadores.filtrosEI.tipoFinca.state = 0;
LimiteIndicadores.filtrosEI.tipoFinca.uso = 0;
/* FIN SELECTORES ANIDADOS DE EDADES INDICADORES */