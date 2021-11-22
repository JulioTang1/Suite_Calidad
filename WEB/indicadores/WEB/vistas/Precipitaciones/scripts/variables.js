var Precipitaciones = new Object;

Precipitaciones.urlConexion = "/apiServices/WS.asmx";//DataTable
Precipitaciones.urlConexionVista = "/apiServices/Precipitaciones.asmx";//Vista

Precipitaciones.fechaIniRank = null;

Precipitaciones.countGraph = 0;

/* SELECTORES ANIDADOS */
Precipitaciones.filtros = new Object();

Precipitaciones.filtros.departamento = new Object();
Precipitaciones.filtros.departamento.data = '0'
Precipitaciones.filtros.departamento.state = 0;
Precipitaciones.filtros.departamento.uso = 0;

Precipitaciones.filtros.municipio = new Object();
Precipitaciones.filtros.municipio.data = '0';
Precipitaciones.filtros.municipio.state = 0;
Precipitaciones.filtros.municipio.uso = 0;

Precipitaciones.filtros.finca = new Object();
Precipitaciones.filtros.finca.data = '0';
Precipitaciones.filtros.finca.state = 0;
Precipitaciones.filtros.finca.uso = 0;
/* FIN SELECTORES ANIDADOS */