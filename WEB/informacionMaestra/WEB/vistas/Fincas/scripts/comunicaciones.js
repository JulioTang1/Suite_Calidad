Fincas.comunicaciones = new Object();

//Callback de la generacion de la tabla
/* Tiene los siguientes parametros porque viene de un evento de DT */
Fincas.comunicaciones.tablaCargada  = function(e, settings, json) {
	var txt = '';
	if( /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)){
		txt = `	<button id="add" class="btn btn-success btn-sm">
					<i class="material-icons" style="vertical-align: middle; font-size: 21px;">
						add_circle
					</i>
					<p style="vertical-align: middle; margin: 0; cursor: pointer; display: inherit;">
						Crear finca
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
							Crear finca
						</p>
					</button>
				</span>`;
		$("#divtabla_0 .top").append(txt);
	}
	//Evento boton agregar
    if(Fincas.baderaPermisosEdicion){
       //Evento boton agregar
       $('#add').on("click",Fincas.funciones.bringSelects);
    }
    else{ 
        //Se inhabilita el efecto de ondas
        $("#add").off();
        //Se le da el color de button disabled 
        $("#add").addClass('boton-desabilitado');
        // Tooltip con mensaje para usuario
        $("#add").tooltip({title: "No posee permisos para Editar", animation: true , placement: "right"});
    }

	Fincas.$tabla = new $.fn.dataTable.Api( `#${settings.nTable.id}` ); 
	//Instancia de DataTable para utilizar el Api para los filtros

	//Se define evento para borrar el popover al reordenar
    Fincas.$tabla.off('draw.dt').on('draw.dt', function(){setTimeout(Fincas.gestionDOM.borrarPopoverOnOrder,1)});
    //Se define evento al seleccionar una fila de la tabla
    Fincas.$tabla.off('select').on('select', Fincas.gestionDOM.popoverEditar);
    //Se detruye el popover cuando se haga scroll en el datatable
    $('div.dataTables_scrollBody').scroll(Fincas.gestionDOM.borrarPopover);
	$(window).resize();
}

//CALLBACK informacion de selectores del formulario
Fincas.comunicaciones.bringSelects = function(resultado){
	Fincas.gestionDOM.htmlForm(resultado);
}

//CALLBACK informacion de municipios
Fincas.comunicaciones.bringMunicipios = function(resultado){
	Fincas.gestionDOM.bringMunicipios(resultado);
}

//CALLBACK inset finca 
Fincas.comunicaciones.insertFinca = function(resultado){
	//Si resultado esta indefinido es por que no hubo codigos o nombres repetidos
	if(resultado == undefined){
		//Pone la vista de la tabla y desaparece el formulario
		Fincas.gestionDOM.cancelar();
		//Se recarga la tabla
		Fincas.$tabla.ajax.reload();
		//Abre el modal de exitoto
		gestionModal.confirmacionSinBoton('success','Cambios guardados');
	}
	else{
		Fincas.funciones.validateCodes(resultado);
	}
}

//CALLBACK informacion de selectores del formulario
Fincas.comunicaciones.bringSelectsEdit = function(resultado){
	Fincas.gestionDOM.htmlFormEdit(resultado);
}

//CALLBACK update finca 
Fincas.comunicaciones.editFinca = function(resultado){
	//Si resultado esta indefinido es por que no hubo codigos repetidos
	if(resultado == undefined){
		//Pone la vista de la tabla y desaparece el formulario
		Fincas.gestionDOM.cancelar();
		//Se recarga la tabla
		Fincas.$tabla.ajax.reload();
		//Abre el modal de exitoso
		gestionModal.confirmacionSinBoton('success','Cambios guardados');
	}
	else{
		Fincas.funciones.validateCodes(resultado);
	}
}