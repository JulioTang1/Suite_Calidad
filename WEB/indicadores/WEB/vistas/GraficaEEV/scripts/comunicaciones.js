GraficaEEV.comunicaciones = new Object();

/* SELECTORES ANIDADOS */
GraficaEEV.comunicaciones.consultaSelectores = function(resultado){
	var id = "";
	if(GraficaEEV.filtros.departamento.state == 1){
		id = `#selectDepartamento`;
	}
	else if(GraficaEEV.filtros.municipio.state == 1){
		id = `#selectMunicipio`;
	}
	else if(GraficaEEV.filtros.finca.state == 1){
		id = `#selectFinca`;
	}
	else{}
	GraficaEEV.gestionDOM.fillSelectores(resultado, id);
}
/* FIN SELECTORES ANIDADOS */