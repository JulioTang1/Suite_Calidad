GraficaIndicadores.comunicaciones = new Object();

/* SELECTORES ANIDADOS */
GraficaIndicadores.comunicaciones.consultaSelectores = function(resultado){
	var id = "";
	if(GraficaIndicadores.filtros.departamento.state == 1){
		id = `#selectDepartamento`;
	}
	else if(GraficaIndicadores.filtros.municipio.state == 1){
		id = `#selectMunicipio`;
	}
	else if(GraficaIndicadores.filtros.finca.state == 1){
		id = `#selectFinca`;
	}
	else{}
	GraficaIndicadores.gestionDOM.fillSelectores(resultado, id);
}
/* FIN SELECTORES ANIDADOS */

/* SELECTORES ANIDADOS (edades infecciones) */
GraficaIndicadores.comunicaciones.consultaSelectoresEI = function(resultado){
	var id = "";
	if(GraficaIndicadores.filtrosEI.edad.state == 1){
		id = `#selectEdad`;
	}
	else if(GraficaIndicadores.filtrosEI.indicador.state == 1){
		id = `#selectInfeccion`;
	}
	else{}
	GraficaIndicadores.gestionDOM.fillSelectores(resultado, id);
}
/* FIN SELECTORES ANIDADOS (edades infecciones) */