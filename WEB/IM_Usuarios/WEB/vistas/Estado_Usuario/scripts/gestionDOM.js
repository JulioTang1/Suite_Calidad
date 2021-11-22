Estado_Usuario.gestionDOM = new Object();

Estado_Usuario.gestionDOM.genTable = function(){

	gestionModal.alertaBloqueante(primario.aplicacion, 'Procesando...');
	Estado_Usuario.gestionDOM.borrarPopover();

	gestionDT.initTable(
		Estado_Usuario.urlConexion,
		"Estado_Usuario", 
		"#T_Estado_Usuario", 
		Estado_Usuario.gestionDOM.LoadTable,
		0
	);

	//Al clickar cualquier elemento tr dentro de tbody 
	$("#T_Estado_Usuario").off().on("click","tbody tr",Estado_Usuario.funciones.clickSelected);
	
}

/*******************************************************************************************************/ 


//Callback de la generacion de la tabla
/* Tiene los siguientes parametros porque viene de un evento de DT */
Estado_Usuario.gestionDOM.LoadTable = function(e, settings, json) {

	var txt = '';
	if( /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)){
		txt = `	
				<button id="D_excel" class="btn btn-success btn-sm">
					<i class="material-icons" style="vertical-align: middle; font-size: 21px;">
						move_to_inbox
					</i>
					<p style="vertical-align: middle; margin: 0; cursor: pointer; display: inherit;">
						Excel usuarios sin actividad
					</p>
				</button>
				<button id="close_user" class="btn btn-danger btn-sm">
					<i class="material-icons" style="vertical-align: middle; font-size: 21px;">
						settings_applications
					</i>
					<p style="vertical-align: middle; margin: 0; cursor: pointer; display: inherit;">
						Bloqueo usuarios sin actividad
					</p>
				</button>
				<button id="upd_msv" class="btn btn-warning btn-sm">
					<i class="material-icons" style="vertical-align: middle; font-size: 21px;">
						groups
					</i>
					<p style="vertical-align: middle; margin: 0; cursor: pointer; display: inherit;">
						Cambio masivo
					</p>
				</button>`;
		$("#divtabla_0 #btn-toggle-option").append(txt);
	}
	else
	{
		txt = `	
				<span class="px-2">
					<button id="D_excel" class="btn btn-success btn-sm">
						<i class="material-icons" style="vertical-align: middle; font-size: 21px;">
							move_to_inbox
						</i>
						<p style="vertical-align: middle; margin: 0; cursor: pointer; display: inherit;">
							Excel usuarios sin actividad
						</p>
					</button>
				</span>
				<span class="px-2">
					<button id="close_user" class="btn btn-danger btn-sm">
						<i class="material-icons" style="vertical-align: middle; font-size: 21px;">
							settings_applications
						</i>
						<p style="vertical-align: middle; margin: 0; cursor: pointer; display: inherit;">
							Bloqueo usuarios sin actividad
						</p>
					</button>
				</span>
				<span class="px-2">
					<button id="upd_msv" class="btn btn-warning btn-sm">
						<i class="material-icons" style="vertical-align: middle; font-size: 21px;">
							groups
						</i>
						<p style="vertical-align: middle; margin: 0; cursor: pointer; display: inherit;">
							Cambio masivo
						</p>
					</button>
				</span>`;
		$("#divtabla_0 .top").append(txt);
	}

	//Evento boton agregar
    if(Estado_Usuario.baderaPermisosEdicion){
		//Evento boton agregar
		$('#close_user').off("click").on("click",Estado_Usuario.funciones.updateStateUser);

		$('#upd_msv').off("click").on("click",Estado_Usuario.funciones.updateStateUserMsv);
	}
	else{ 
		//Se inhabilita el efecto de ondas
		$("#close_user").off();
		//Se le da el color de button disabled 
		$("#close_user").addClass('boton-desabilitado');
		// Tooltip con mensaje para usuario
		$("#close_user").tooltip({title: "No posee permisos para bloquear usuarios", animation: true , placement: "right"}); 

		//Se inhabilita el efecto de ondas
		$("#upd_msv").off();
		//Se le da el color de button disabled 
		$("#upd_msv").addClass('boton-desabilitado');
		// Tooltip con mensaje para usuario
		$("#upd_msv").tooltip({title: "No posee permisos para Editar", animation: true , placement: "right"}); 
	}

	$('#D_excel').on("click",Estado_Usuario.funciones.Excel_user);

	Estado_Usuario.$tabla = new $.fn.dataTable.Api( `#${settings.nTable.id}` ); 
	//Instancia de DataTable para utilizar el Api para los filtros

	//Se define evento para borrar el popover al reordenar
    Estado_Usuario.$tabla.off('draw.dt').on('draw.dt',function(){setTimeout(Estado_Usuario.gestionDOM.borrarPopover,1)});
    //Se define evento al seleccionar una fila de la tabla
    Estado_Usuario.$tabla.off('select').on('select',Estado_Usuario.gestionDOM.popoverDT);
    //Se detruye el popover cuando se haga scroll en el datatable
    $('div.dataTables_scrollBody').scroll(Estado_Usuario.gestionDOM.borrarPopover);

    //CallBack cuando se recargue la tabla
    //Estado_Usuario.$tabla.on("draw",Estado_Usuario.funciones.reloadTable);
    gestionModal.cerrar();
}

/*****************************************************************************************************/

Estado_Usuario.gestionDOM.popoverOff = function(){
	//Cerrar popover
	$(".popover").popover("dispose");
}

//eventos popover
Estado_Usuario.gestionDOM.popoverDT = function(e, dt, type, indexes){
	//Se borra el popover anterior
	Estado_Usuario.gestionDOM.borrarPopover();
	// Se guardan los datos de la fila de la tabla actualmente seleccionada
	Estado_Usuario.datosFilaTabla = Estado_Usuario.$tabla.rows(indexes).data();
	//Se crea el popoever
	var pop = $(this).children("tbody").children("tr").eq(indexes[0]).popover({
		content: `
					<a id="edit" class="btn"> 
						<i class="fas fa-edit"></i>
					</a>`, 
		html: true, 
		placement: "left",
		animation: true,	
		trigger:"manual"
	});
	//Se posiciona el popover
	Estado_Usuario.gestionDOM.posicionPopoever(pop);
	//Se muestra el popover
	$(this).children("tbody").children("tr").eq(indexes[0]).popover("show");
	//No se muestra el popver hasta asustar posicion
	$(".popover").css("display","none");
	//Se cierra el popover en los botones de filtrar
	$(".filter-option").off("click").on("click",Estado_Usuario.gestionDOM.cerrarPopover);
	//Elemento anterior
   	Estado_Usuario.ultimoElementoConPopoever = $(this).children("tbody").children("tr").eq(indexes[0]);
   	//Se guarda el elemento tabla
   	Estado_Usuario.tabla = $(this);
   	//Cada que se da click en el contenedor se mira que no tenga nada que ver con el popover y si es asi se oculta
   	$("body").off('click').on('click',Estado_Usuario.gestionDOM.cerrarPopoverEnClick);
   	//Vuelve a esconder el popover al dar click en la fila otra vez
   	$(".selected").click(Estado_Usuario.gestionDOM.borrarPopover);
  	//Se definen los eventos de editar y borrar
    if(Estado_Usuario.baderaPermisosEdicion){
	   //Evento boton editar
	   $("#edit").off("click").on("click", Estado_Usuario.funciones.EditarUsuario);
    }
    else{
		//Evento boton editar
		$("#edit").off("click");

        //Se le da el color de button disabled 
        $("#edit").addClass('boton-popover-desabilitado');
        // Tooltip con mensaje para usuario
		$("#edit").tooltip({title: "No posee permisos para Editar", animation: true , placement: "right"});
	}
}

Estado_Usuario.gestionDOM.posicionPopoever = function(pop){
	//Se restan 28px que son la mitad del tamaño del contenedor, como offset
	Estado_Usuario.top = Estado_Usuario.eventoSelected.clientY - 28;
	Estado_Usuario.left = Estado_Usuario.eventoSelected.clientX + 10;
}

Estado_Usuario.gestionDOM.cerrarPopoverEnClick = function(e){
	var container = $(`.popover`);
	if(Estado_Usuario.tabla != undefined){
		if ((!container.is(e.target)) && (!Estado_Usuario.tabla.children("tbody").is(e.target.parentElement.parentElement)) 
		&& (container.has(e.target).length === 0)) { 
				//Se ha pulsado en cualquier lado fuera de los elementos contenidos en la variable container
				Estado_Usuario.gestionDOM.borrarPopover();
		}
	}
}

Estado_Usuario.gestionDOM.borrarPopover = function(){
	$(Estado_Usuario.ultimoElementoConPopoever).popover("dispose");
}

//Cuando el popover esta generado (pero oculto):
Estado_Usuario.gestionDOM.popoverFullyShown = function(){
	//Se ajusta la posicion del popover con las cooernadas del click
    $(".popover").css({"transform":`translate(${Estado_Usuario.left}px,${Estado_Usuario.top}px)`});
    //Se muetra el popover despues de acomodado
	$(".popover").css("display","block");
	//Se acomoda la flecha del popoever
	$(".arrow").css("top",`${parseInt(Estado_Usuario.eventoSelected.clientY - ($(".popover").offset().top + 14))}px`);
}

/*****************************************************************************************************/

//Se crea el contenedor divForm Y en el se carga el formulario
Estado_Usuario.gestionDOM.EditarUsuario = function(data){

	var info_rol = data.Roles;
	var info_estados = data.Estado;
	var info_usuario = data.E_R;
	var app_habilitada = data.app_habilitada[0].app_habilitada;
	var contrasena_app = data.app_habilitada[0].contrasena_app == null ? "" : data.app_habilitada[0].contrasena_app;
	var txt;

	//Titulo para modal
	var titulo = "Cambiar estado y/o rol";

	//Se carga el formulario en divForm	
	var ContenidoHTML = `
	<div id='divForm'>
		<div class="container">
			<div class="row mb-4">
				<div class = "col-sm-12">
					<div>
						<span>Estado:</span>
					</div>
					<!-- selector -->
					<div id = "selectorE" class="border-r-select">
						<select data-live-search="true" id="S_Estado" class='selectpicker'>
							<option disabled value="0"> </option>
						</select>
					</div>

					<div>
						<span>Rol:</span>
					</div>
					<!-- selector -->
					<div id = "selectorR" class="border-r-select">
						<select data-live-search="true" id="S_Rol" class='selectpicker'>
							<option disabled value="0"> </option>
						</select>
					</div>

					<div>
						<span>Habilitar App Móvil:</span>
					</div>
					<div class="row">
						<div class="col-md-6">
							<input id="app" type="checkbox" name="set-name" class="switch-input">
							<label for="app" class="switch-label">
								<span class="toggle--on">Habilitar App</span>
								<span class="toggle--off">Deshabilitar App</span>
							</label>
						</div>
						<div class="col-sm-6">
						    <div class="div-input-md margin-top-app ">      
							  <input id="contrasena" type="text" required autofocus 
							  value="${contrasena_app}" class="caja">
						      <label class="caja">Contraseña APP</label>
						    </div>
						    <!-- Contenedor para mensajes de error -->
				    		<div id="invalidFeedback_contrasena"></div> 
						</div>
					</div>
				</div>
			</div>
		</div>
	</div>
	`;

	var botonHTML = 
	`<div id='divBoton' class="pr-3 pt-3 pb-5">
		<div>
			<a id="cancelar" class="boton red p-2 mr-2"> Cancelar </a>
		</div>	
		<div>
			<a id="guardarCambios" class="boton blue p-2 mr-2" disabled> Guardar cambios </a>	
		</div>											
	</div>`;
	
	//Se agrega efecto de onda al boton
	swal(
	{
		title: titulo,
		html: ContenidoHTML,
		showCloseButton: true,
		showConfirmButton: false,
		allowOutsideClick: false
	}
	);

	//Se dinuja el boton, dentro del modal, pero fuera del modal content, como footer
	$(".swal2-content").after(botonHTML);

	$(".swal2-modal").css("width","700px");

	gestionBSelect.generic(`#S_Rol`,"");
	gestionBSelect.generic(`#S_Estado`,"");
	$(`#S_Rol`).removeAttr("title");
	$(`#S_Estado`).removeAttr("title");

	//Se enciende o apaga el checkbox si tiene la app habilitada
	$("#app").prop("checked", app_habilitada == 1 ? true : false);
	//Se habilita o deshabilita la caja de contraseña
	$("#contrasena").prop("disabled", app_habilitada == 1 ? false : true);
	gestionImask.CodigoAlfaNumerico("#contrasena");
	//Evento para habilitar contraseña en funcion del checkbox
	$("#app").off("change").on("change", Estado_Usuario.gestionDOM.change);

	txt= '';

	for(x in info_rol)
	{
		txt = `${txt}<option value=${info_rol[x].id}>${info_rol[x].nombre}</option>`;
	}
	$("#S_Rol").html(txt).selectpicker('refresh');

	txt= '';

	for(x in info_estados)
	{
		txt = `${txt}<option value=${info_estados[x].id}>${info_estados[x].estado}</option>`;
	}
	$("#S_Estado").html(txt).selectpicker('refresh');

	$("#S_Rol").val(info_usuario[0].id_rol).selectpicker('refresh');
	$("#S_Estado").val(info_usuario[0].id_estado_opcion).selectpicker('refresh');

	$("#guardarCambios").off().on("click", Estado_Usuario.funciones.upd_EU);
	$("#cancelar").off().on("click", Estado_Usuario.gestionDOM.cerrarModal); 

}

//habilitar contraseña en funcion del checkbox
Estado_Usuario.gestionDOM.change = function(){
	$("#contrasena").val("");
	$("#contrasena").prop("disabled", !$(this).prop("checked"));
}

Estado_Usuario.gestionDOM.cerrarModal = function(){
	gestionModal.cerrar();
}

Estado_Usuario.gestionDOM.UPD_Success = function(){

	//Se cargan la tabla con los nuevos datos
	Estado_Usuario.$tabla.ajax.reload();

	//Modal de registro satisfactorio
	gestionModal.confirmacionSinBoton('success','Actualización exitosa');
}

/* ************************************************************************************************** */

//Se crea el contenedor divForm Y en el se carga el formulario
Estado_Usuario.gestionDOM.updateStateUserMsv = function(data){

	var info_rol = data.Roles;
	var info_estados = data.Estado;
	var txt;

	//Titulo para modal
	var titulo = "Cambiar estado y/o rol";

	//Se carga el formulario en divForm	
	var ContenidoHTML = `
	<div id='divForm'>
		<div class="container">
			<div class="row mb-4">
				<div class = "col-sm-12">
					<div>
						<span>Estado:</span>
					</div>
					<!-- selector -->
					<div id = "selectorE" class="border-r-select">
						<select data-live-search="true" id="S_Estado" class='selectpicker'>
							<option disabled value="0"> </option>
						</select>
					</div>

					<div>
						<span>Rol:</span>
					</div>
					<!-- selector -->
					<div id = "selectorR" class="border-r-select">
						<select data-live-search="true" id="S_Rol" class='selectpicker'>
							<option disabled value="0"> </option>
						</select>
					</div>
				</div>
			</div>
		</div>
	</div>
	`;

	var botonHTML = 
	`<div id='divBoton' class="pr-3 pt-3 pb-5">
		<div>
			<a id="cancelar" class="boton red p-2 mr-2"> Cancelar </a>
		</div>	
		<div>
			<a id="guardarCambios" class="boton blue p-2 mr-2" disabled> Guardar cambios </a>	
		</div>											
	</div>`;
	
	//Se agrega efecto de onda al boton
	swal(
	{
		title: titulo,
		html: ContenidoHTML,
		showCloseButton: true,
		showConfirmButton: false,
		allowOutsideClick: false
	}
	);

	//Se dinuja el boton, dentro del modal, pero fuera del modal content, como footer
	$(".swal2-content").after(botonHTML);

	$(".swal2-modal").css("width","500px");

	gestionBSelect.generic(`#S_Rol`,"");
	gestionBSelect.generic(`#S_Estado`,"");
	$(`#S_Rol`).removeAttr("title");
	$(`#S_Estado`).removeAttr("title");

	txt= '';

	for(x in info_rol)
	{
		txt = `${txt}<option value=${info_rol[x].id}>${info_rol[x].nombre}</option>`;
	}
	$("#S_Rol").html(txt).selectpicker('refresh');

	txt= '';

	for(x in info_estados)
	{
		txt = `${txt}<option value=${info_estados[x].id}>${info_estados[x].estado}</option>`;
	}
	$("#S_Estado").html(txt).selectpicker('refresh');

	$("#guardarCambios").off().on("click", Estado_Usuario.funciones.upd_EU_msv);
	$("#cancelar").off().on("click", Estado_Usuario.gestionDOM.cerrarModal); 

}