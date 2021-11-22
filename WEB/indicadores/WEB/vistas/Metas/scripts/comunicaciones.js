Metas.comunicaciones = new Object();

//Callback de la generacion de la tabla
/* Tiene los siguientes parametros porque viene de un evento de DT */
Metas.comunicaciones.tablaCargada  = function(e, settings, json) {
	var txt = '';
	if( /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)){
		txt = `	<button id="add" class="btn btn-success btn-sm">
					<i class="material-icons" style="vertical-align: middle; font-size: 21px;">
						add_circle
					</i>
					<p style="vertical-align: middle; margin: 0; cursor: pointer; display: inherit;">
						Crear meta
					</p>
				</button>`;
		$("#divtabla_0 #btn-toggle-option").append(txt);
	}
	else
	{
		txt = `	<span class="px-2">
					<button id="add" class="btn btn-success btn-sm">
						<i class="material-icons" style="vertical-align: middle; font-size: 21px;">
							add_circle
						</i>
						<p style="vertical-align: middle; margin: 0; cursor: pointer; display: inherit;">
							Crear meta
						</p>
					</button>
				</span>`;
		$("#divtabla_0 .top").append(txt);
	}
	//Evento boton agregar
    if(Metas.baderaPermisosEdicion){
       //Evento boton agregar
       $('#add').on("click",Metas.gestionDOM.htmlForm);
    }
    else{ 
        //Se inhabilita el efecto de ondas
        $("#add").off();
        //Se le da el color de button disabled 
        $("#add").addClass('boton-desabilitado');
        // Tooltip con mensaje para usuario
        $("#add").tooltip({title: "No posee permisos para Editar", animation: true , placement: "right"});
    }

	Metas.$tabla = new $.fn.dataTable.Api( `#${settings.nTable.id}` ); 
	//Instancia de DataTable para utilizar el Api para los filtros

	//Se define evento para borrar el popover al reordenar
    Metas.$tabla.off('draw.dt').on('draw.dt', function(){setTimeout(Metas.gestionDOM.borrarPopoverOnOrder,1)});
    //Se define evento al seleccionar una fila de la tabla
    Metas.$tabla.off('select').on('select', Metas.gestionDOM.popoverEditar);
    //Se detruye el popover cuando se haga scroll en el datatable
    $('div.dataTables_scrollBody').scroll(Metas.gestionDOM.borrarPopover);
	$(window).resize();
}

//CALLBACK insert meta
Metas.comunicaciones.insertMeta = function(resultado){
	//Si resultado esta indefinido es por que no hubo codigos o nombres repetidos
	if(resultado == undefined){
		//Pone la vista de la tabla y desaparece el formulario
		Metas.gestionDOM.cancelar();
		//Se recarga la tabla
		Metas.$tabla.ajax.reload();
		//Abre el modal de exitoto
		gestionModal.confirmacionSinBoton('success','Cambios guardados');
	}
	else{
		Metas.funciones.validateCodes(resultado);
	}
}

//CALLBACK delete meta
Metas.comunicaciones.deleteMeta = function(){
	//Se recarga la tabla
	Metas.$tabla.ajax.reload();
	//Abre el modal de exitoto
	gestionModal.confirmacionSinBoton('success','Cambios guardados');
}

/* SELECTORES ANIDADOS (edades infecciones) */
Metas.comunicaciones.consultaSelectoresEI = function(resultado){
	var id = "";
	if(Metas.filtrosEI.edad.state == 1){
		id = `#selectEdad`;
	}
	else if(Metas.filtrosEI.indicador.state == 1){
		id = `#selectInfeccion`;
	}
	else{}
	Metas.gestionDOM.fillSelectores(resultado, id);
}
/* FIN SELECTORES ANIDADOS (edades infecciones) */