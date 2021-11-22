Bioseguridad.comunicaciones = new Object();

//Callback de la generacion de la tabla
/* Tiene los siguientes parametros porque viene de un evento de DT */
Bioseguridad.comunicaciones.tablaCargada  = function(e, settings, json) {
	Bioseguridad.$tabla = new $.fn.dataTable.Api( `#${settings.nTable.id}` ); 
	//Instancia de DataTable para utilizar el Api para los filtros

	//Se define evento para borrar el popover al reordenar
    Bioseguridad.$tabla.off('draw.dt').on('draw.dt', function(){setTimeout(Bioseguridad.gestionDOM.borrarPopoverOnOrder,1)});
    //Se define evento al seleccionar una fila de la tabla
    Bioseguridad.$tabla.off('select').on('select', Bioseguridad.gestionDOM.popoverEditar);
    //Se detruye el popover cuando se haga scroll en el datatable
    $('div.dataTables_scrollBody').scroll(Bioseguridad.gestionDOM.borrarPopover);
	$(window).resize();

	$("#cargar").prop("disabled", false);

	$("#main").css("display", "none");
	$("#tableBioseguridad").css("display", "block");
	$("#divSelectVista").attr("style", "display: flex!important");
}

/* SELECTORES ANIDADOS */
Bioseguridad.comunicaciones.consultaSelectores = function(resultado){
	var id = "";
	if(Bioseguridad.filtros.departamento.state == 1){
		id = `#selectDepartamento`;
	}
	else if(Bioseguridad.filtros.municipio.state == 1){
		id = `#selectMunicipio`;
	}
	else if(Bioseguridad.filtros.finca.state == 1){
		id = `#selectFinca`;
	}
	else{}
	Bioseguridad.gestionDOM.fillSelectores(resultado, id);
}
/* FIN SELECTORES ANIDADOS */

/* INFORMACION DE VISITA SELECCIONADA */
Bioseguridad.comunicaciones.visita = function(resultado){
	Bioseguridad.gestionDOM.visita(resultado);
}

//Se reciben las urls de las fotos
Bioseguridad.comunicaciones.camara = function(resultado){
	Bioseguridad.gestionDOM.camara(resultado);
}

//CALLBACK delete meta
Bioseguridad.comunicaciones.delete = function(){
	//Se recarga la tabla
	Bioseguridad.$tabla.ajax.reload();
	//Abre el modal de exitoto
	gestionModal.confirmacionSinBoton('success','Cambios guardados');
}