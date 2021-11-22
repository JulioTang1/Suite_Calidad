LimiteIndicadores.comunicaciones = new Object();

//Se trae la informacion de los limites de indicadores
LimiteIndicadores.comunicaciones.info = function(resultado){
    LimiteIndicadores.gestionDOM.info(resultado);
}

//Callback de save
LimiteIndicadores.comunicaciones.save = function(){
    LimiteIndicadores.funciones.info();
    //Abre el modal de exitoso
	gestionModal.confirmacionSinBoton('success','Cambios guardados');
}

/* SELECTORES ANIDADOS (edades infecciones) */
LimiteIndicadores.comunicaciones.consultaSelectoresEI = function(resultado){
	var id = "";
	if(LimiteIndicadores.filtrosEI.edad.state == 1){
		id = `#selectEdad`;
	}
	else if(LimiteIndicadores.filtrosEI.indicador.state == 1){
		id = `#selectInfeccion`;
	}
	else if(LimiteIndicadores.filtrosEI.tipoFinca.state == 1){
		id = `#selectTipoFinca`;
	}
	else{}
	LimiteIndicadores.gestionDOM.fillSelectores(resultado, id);
}
/* FIN SELECTORES ANIDADOS (edades infecciones) */