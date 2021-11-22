Nuevo.comunicaciones = new Object();

/* SELECTORES ANIDADOS */
Nuevo.comunicaciones.consultaSelectores = function(resultado){
	var id = "";
	if(Nuevo.filtros.departamento.state == 1){
		id = `#selectDepartamento`;
	}
	else if(Nuevo.filtros.municipio.state == 1){
		id = `#selectMunicipio`;
	}
	else if(Nuevo.filtros.finca.state == 1){
		id = `#selectFinca`;
	}
	else{}
	Nuevo.gestionDOM.fillSelectores(resultado, id);
}
/* FIN SELECTORES ANIDADOS */

Nuevo.comunicaciones.bringVisita = function(resultado, activeEdicion){
	//Se elimina ele elemento Nuevo si viene con mas opciones
	if(resultado.length > 1){
		var salida = new Array();
		var j = 0;
		for(var i = 0; i < resultado.length; i++){
			if(resultado[i].id != 0){
				salida[j] = resultado[i];
				j++;
			}
		}
	}else{
		var salida = resultado;
	}
	Nuevo.gestionDOM.fillSelectores(salida, `#selectVisita`);

	if(activeEdicion != 1){
		//Se piden los datos de la visita
		Nuevo.funciones.datos();
	}
}

//Se traen los datos
Nuevo.comunicaciones.datos = function(resultado){
	Nuevo.gestionDOM.datos(resultado);
}

//Se guardan los datos
Nuevo.comunicaciones.guardar = function(resultado, activeEdicion){
	Nuevo.funciones.bringVisita(undefined, activeEdicion);
	//Abre el modal de exitoso
	gestionModal.confirmacionSinBoton('success','Cambios guardados');
}