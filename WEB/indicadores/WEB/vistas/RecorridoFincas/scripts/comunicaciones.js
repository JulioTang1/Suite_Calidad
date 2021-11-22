RecorridoFincas.comunicaciones = new Object();

/* SELECTORES ANIDADOS */
RecorridoFincas.comunicaciones.consultaSelectores = function(resultado){
	var id = "";
	if(RecorridoFincas.filtros.departamento.state == 1){
		id = `#selectDepartamento`;
	}
	else if(RecorridoFincas.filtros.municipio.state == 1){
		id = `#selectMunicipio`;
	}
	else if(RecorridoFincas.filtros.finca.state == 1){
		id = `#selectFinca`;
	}
	else{}
	RecorridoFincas.gestionDOM.fillSelectores(resultado, id);
}
/* FIN SELECTORES ANIDADOS */

/* SELECTORES ANIDADOS (edades infecciones) */
RecorridoFincas.comunicaciones.consultaSelectoresEI = function(resultado){
	var id = "";
	if(RecorridoFincas.filtrosEI.edad.state == 1){
		id = `#selectEdad`;
	}
	else if(RecorridoFincas.filtrosEI.indicador.state == 1){
		id = `#selectInfeccion`;
	}
	else{}
	RecorridoFincas.gestionDOM.fillSelectores(resultado, id);
}
/* FIN SELECTORES ANIDADOS (edades infecciones) */

//Callback de la generacion de la tabla
/* Tiene los siguientes parametros porque viene de un evento de DT */
RecorridoFincas.comunicaciones.tablaCargada  = function(e, settings, json) {
	RecorridoFincas.$tabla = new $.fn.dataTable.Api( `#${settings.nTable.id}` ); 
	//Instancia de DataTable para utilizar el Api para los filtros

	//Se define evento para borrar el popover al reordenar
    RecorridoFincas.$tabla.off('draw.dt').on('draw.dt', function(){setTimeout(RecorridoFincas.gestionDOM.borrarPopoverOnOrder,1)});
    //Se define evento al seleccionar una fila de la tabla
    RecorridoFincas.$tabla.off('select').on('select', RecorridoFincas.gestionDOM.popoverEditar);
    //Se detruye el popover cuando se haga scroll en el datatable
    $('div.dataTables_scrollBody').scroll(RecorridoFincas.gestionDOM.borrarPopover);
	$(window).resize();

	$("#cargar").prop("disabled", false);

	//Se limpia informacion sobre la visita actual
	$('.TituloCategoria .titulo_opcion').css("display", "none");
	$("#titulo_opcion").empty();

	//Se desaparece el navbar
	$("#nav-wrapper-balance").css("display", "none");
	$(".tab-content").css("display", "none");
	$("#navWrapper").css("display", "none");
	
	$("#tableRecorridoFincas").css("display", "block");
}

/* INFORMACION DE VISITA SELECCIONADA */
RecorridoFincas.comunicaciones.visita = function(resultado, request){
	//dibuja mapa solo cuando llegue la respuesta de la ultima peticion
	if(request == RecorridoFincas.mapRequest){
		RecorridoFincas.gestionDOM.visita(resultado);
	}
}

//CALLBACK con datos de la visita para mostrar en tablas
RecorridoFincas.comunicaciones.genDatos = function(resultado){
	RecorridoFincas.gestionDOM.genDatos(resultado);
}

//Se reciben las urls de las fotos
RecorridoFincas.comunicaciones.camara = function(resultado){
	RecorridoFincas.gestionDOM.camara(resultado);
}

//CALLBACK delete meta
RecorridoFincas.comunicaciones.delete = function(){
	//Se recarga la tabla
	RecorridoFincas.$tabla.ajax.reload();
	//Abre el modal de exitoto
	gestionModal.confirmacionSinBoton('success','Cambios guardados');
}