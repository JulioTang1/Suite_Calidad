GraficaPrecipitaciones.comunicaciones = new Object();

/* SELECTORES ANIDADOS */
GraficaPrecipitaciones.comunicaciones.consultaSelectores = function(resultado){
	var id = "";
	if(GraficaPrecipitaciones.filtros.departamento.state == 1){
		id = `#selectDepartamento`;
	}
	else if(GraficaPrecipitaciones.filtros.municipio.state == 1){
		id = `#selectMunicipio`;
	}
	else if(GraficaPrecipitaciones.filtros.finca.state == 1){
		id = `#selectFinca`;
	}
	else{}
	GraficaPrecipitaciones.gestionDOM.fillSelectores(resultado, id);
}
/* FIN SELECTORES ANIDADOS */