var Nuevo = new Object;

Nuevo.urlConexion = "/apiServices/WS.asmx";//DataTable
Nuevo.urlConexionVista = "/apiServices/Nuevo.asmx";//Vista

Nuevo.fechaIniRank = null;

/* SELECTORES ANIDADOS */
Nuevo.filtros = new Object();

Nuevo.filtros.departamento = new Object();
Nuevo.filtros.departamento.data = '0'
Nuevo.filtros.departamento.state = 0;
Nuevo.filtros.departamento.uso = 0;

Nuevo.filtros.municipio = new Object();
Nuevo.filtros.municipio.data = '0';
Nuevo.filtros.municipio.state = 0;
Nuevo.filtros.municipio.uso = 0;

Nuevo.filtros.finca = new Object();
Nuevo.filtros.finca.data = '0';
Nuevo.filtros.finca.state = 0;
Nuevo.filtros.finca.uso = 0;
/* FIN SELECTORES ANIDADOS */