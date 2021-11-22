GraficaPorcentaje.comunicaciones = new Object();

/* SELECTORES ANIDADOS */
GraficaPorcentaje.comunicaciones.consultaSelectores = function(resultado){
	var id = "";
	if(GraficaPorcentaje.filtros.departamento.state == 1){
		id = `#selectDepartamento`;
	}
	else if(GraficaPorcentaje.filtros.municipio.state == 1){
		id = `#selectMunicipio`;
	}
	else if(GraficaPorcentaje.filtros.finca.state == 1){
		id = `#selectFinca`;
	}
	else{}
	GraficaPorcentaje.gestionDOM.fillSelectores(resultado, id);
}
/* FIN SELECTORES ANIDADOS */

//SELECTOR GRUPOS O METAS
GraficaPorcentaje.comunicaciones.bringGrupos = function(resultado){
	GraficaPorcentaje.gestionDOM.bringGrupos(resultado);
}