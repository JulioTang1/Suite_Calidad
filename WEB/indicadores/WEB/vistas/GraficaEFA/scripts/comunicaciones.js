GraficaEFA.comunicaciones = new Object();

/* SELECTORES ANIDADOS */
GraficaEFA.comunicaciones.consultaSelectores = function(resultado){
	var id = "";
	if(GraficaEFA.filtros.departamento.state == 1){
		id = `#selectDepartamento`;
	}
	else if(GraficaEFA.filtros.municipio.state == 1){
		id = `#selectMunicipio`;
	}
	else if(GraficaEFA.filtros.finca.state == 1){
		id = `#selectFinca`;
	}
	else{}
	GraficaEFA.gestionDOM.fillSelectores(resultado, id);
}
/* FIN SELECTORES ANIDADOS */