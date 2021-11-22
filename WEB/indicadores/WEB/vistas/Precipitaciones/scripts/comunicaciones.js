Precipitaciones.comunicaciones = new Object();

/* SELECTORES ANIDADOS */
Precipitaciones.comunicaciones.consultaSelectores = function(resultado){
	var id = "";
	if(Precipitaciones.filtros.departamento.state == 1){
		id = `#selectDepartamento`;
	}
	else if(Precipitaciones.filtros.municipio.state == 1){
		id = `#selectMunicipio`;
	}
	else if(Precipitaciones.filtros.finca.state == 1){
		id = `#selectFinca`;
	}
	else{}
	Precipitaciones.gestionDOM.fillSelectores(resultado, id);
}
/* FIN SELECTORES ANIDADOS */

//Se traen las precipitaciones
Precipitaciones.comunicaciones.precipitaciones = function(resultado){
	Precipitaciones.gestionDOM.precipitaciones(resultado);
}

//Se guardan las precipitaciones
Precipitaciones.comunicaciones.guardarPrecipitaciones = function(){
	//Abre el modal de exitoso
	gestionModal.confirmacionSinBoton('success','Cambios guardados');
}