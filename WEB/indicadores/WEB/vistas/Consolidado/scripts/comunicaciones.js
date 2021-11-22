Consolidado.comunicaciones = new Object();

//Callback de la generacion de la tabla
/* Tiene los siguientes parametros porque viene de un evento de DT */
Consolidado.comunicaciones.tablaCargada  = function(e, settings, json) {
	$(window).resize();
}

/* SELECTORES ANIDADOS */
Consolidado.comunicaciones.consultaSelectores = function(resultado){
	var id = "";
	if(Consolidado.filtros.departamento.state == 1){
		id = `#selectDepartamento`;
	}
	else if(Consolidado.filtros.municipio.state == 1){
		id = `#selectMunicipio`;
	}
	else if(Consolidado.filtros.finca.state == 1){
		id = `#selectFinca`;
	}
	else{}
	Consolidado.gestionDOM.fillSelectores(resultado, id);
}
/* FIN SELECTORES ANIDADOS */