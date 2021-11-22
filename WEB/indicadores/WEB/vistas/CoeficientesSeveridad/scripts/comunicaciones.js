CoeficientesSeveridad.comunicaciones = new Object();

//Callback de la generacion de la tabla
/* Tiene los siguientes parametros porque viene de un evento de DT */
CoeficientesSeveridad.comunicaciones.tablaCargada  = function(e, settings, json) {
	CoeficientesSeveridad.$tabla = new $.fn.dataTable.Api( `#${settings.nTable.id}` ); 
	//Instancia de DataTable para utilizar el Api para los filtros

	//Se define evento para borrar el popover al reordenar
    CoeficientesSeveridad.$tabla.off('draw.dt').on('draw.dt', function(){setTimeout(CoeficientesSeveridad.gestionDOM.borrarPopoverOnOrder,1)});
    //Se define evento al seleccionar una fila de la tabla
    CoeficientesSeveridad.$tabla.off('select').on('select', CoeficientesSeveridad.gestionDOM.popoverEditar);
    //Se detruye el popover cuando se haga scroll en el datatable
    $('div.dataTables_scrollBody').scroll(CoeficientesSeveridad.gestionDOM.borrarPopover);
	$(window).resize();
}

//CALLBACK update finca 
CoeficientesSeveridad.comunicaciones.edit = function(){
	//Pone la vista de la tabla y desaparece el formulario
	CoeficientesSeveridad.gestionDOM.cancelar();
	//Se recarga la tabla
	CoeficientesSeveridad.$tabla.ajax.reload();
	//Abre el modal de exitoso
	gestionModal.confirmacionSinBoton('success','Cambios guardados');
}