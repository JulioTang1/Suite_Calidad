Datos.comunicaciones = new Object();

//Callback de la generacion de la tabla
/* Tiene los siguientes parametros porque viene de un evento de DT */
Datos.comunicaciones.tablaCargada  = function(e, settings, json) {
	var id_tabla = ["tablaSigatoka", "tablaVasculares", "tablaCulturales", "tablaPrecipitaciones"];
	index = id_tabla.indexOf($(e.target).attr("id"));
	Datos.$tabla[index] = new $.fn.dataTable.Api( `#${settings.nTable.id}` ); 

	//Se define evento para borrar el popover al reordenar
	Datos.$tabla[index].off('draw.dt').on('draw.dt',function(){setTimeout(Datos.gestionDOM.borrarPopover,1)});
	//Se define evento al seleccionar una fila de la tabla
	Datos.$tabla[index].off('select').on('select', {index: index},Datos.gestionDOM.popoverEditar);
	//Se detruye el popover cuando se haga scroll en el datatable
	$('div.dataTables_scrollBody').scroll(Datos.gestionDOM.borrarPopover);
	Datos.$tabla[index].on("draw",Datos.gestionDOM.ContadorCerrarModal);

	$(window).resize();
}

/* SELECTORES ANIDADOS */
Datos.comunicaciones.consultaSelectores = function(resultado){
	var id = "";
	if(Datos.filtros.departamento.state == 1){
		id = `#selectDepartamento`;
	}
	else if(Datos.filtros.municipio.state == 1){
		id = `#selectMunicipio`;
	}
	else if(Datos.filtros.finca.state == 1){
		id = `#selectFinca`;
	}
	else{}
	Datos.gestionDOM.fillSelectores(resultado, id);
}
/* FIN SELECTORES ANIDADOS */

//Se confirma n lo cambios guardados
Datos.comunicaciones.save = function(resultado, index){
	//Se esconde y se elimina el modal
	$('#modal').modal("hide");
	$("#modal").remove();
	//Se recarga la tabla
	Datos.$tabla[index].ajax.reload();
	//Abre el modal de exitoso
	gestionModal.confirmacionSinBoton('success','Cambios guardados');
}