Permisos_Recurso.gestionDOM = new Object();

Permisos_Recurso.gestionDOM.html_edit = function(){
	var btn_add = `
		<div class="contenedor-Opc" id="crearPerfil">
			<i class="material-icons Rol_img">add_circle</i>
			<div class="info">
				<a class="texto">Crear perfil</a>
			</div>
		</div>
	`;

	var btn_edit = `
		<div class="contenedor-Opc" id="Ed_permisos">
			<i class="material-icons Rol_img">groups</i>
			<div class="info">
				<a class="texto">Editar perfiles</a>
			</div>
		</div>
	`;

	$("#ContenedorOpc").prepend(btn_add);
	$("#ContenedorOpc").append(btn_edit);

	$("#crearPerfil").off().on("click", Permisos_Recurso.funciones.crearPerfil);
	$("#Ed_permisos").off().on("click", Permisos_Recurso.gestionDOM.table_P_edit);
}

Permisos_Recurso.gestionDOM.crearPerfil = function(data){

	var Plantas = data.Plantas;
	var Recursos = data.Recursos;
	var NA = data.NA;
	var NA_defecto = data.NA_defecto[0].id;

	Permisos_Recurso.Info_NewPerfil = data;

	var aux_recursos;
	var txt = '';

	//Titulo para modal
	var titulo = "Crear Perfil";

	//Se carga el formulario en divForm	
	var ContenidoHTML = `
	<div id = "divForm">
		<div class="container_InLa C_perfil">
			<input autocomplete="off" type="text" required="" placeholder=" " id="N_perfil">
			<label>Perfil</label>
			<div class="msnError"></div>
		</div>
		<div class="container">
			<div class="custom-control custom-switch">
				<input type="checkbox" class="custom-control-input" id="Apply_to_all">
				<label class="custom-control-label" for="Apply_to_all">Aplicar a todos</label>
			</div>
			<div class="row perfil_recurso">`;
	
	for(x in Plantas){

		ContenidoHTML = `${ContenidoHTML}
				<div class ="col-sm-12 Cat_app mb-3">
					<span>${Plantas[x].nombre}</span>
				</div>`;

		aux_recursos = Recursos.filter(function(e){
			return e.id_Plantas == Plantas[x].id;
		})

		for(y in aux_recursos){
			ContenidoHTML = `${ContenidoHTML}
				<div class="container_InLa col-sm-6 mb-4">
					<input autocomplete="off" type="text" required="" placeholder="" data-id_recurso = "${aux_recursos[y].id}">
					<label>Recurso</label>
				</div>
				<div class="container_Select col-sm-6 mb-4">
					<select data-live-search="true" data-size="4" class='selectpicker' data-id_recurso = "${aux_recursos[y].id}">
						<option disabled value="0"> </option>
					</select>
					<div class="msnError"></div>
					<label>Nivel de acceso</label>
				</div>`;
		}
	}

	ContenidoHTML = `${ContenidoHTML}
			</div>
		</div>
	</div>`;

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

	// Selectores
	gestionBSelect.generic(`#divForm .selectpicker`,"");
	$(`#divForm .selectpicker`).removeAttr("title");

	for(x in NA)
	{
		txt = `${txt}<option value=${NA[x].id}>${NA[x].nombre}</option>`;
	}
	$("#divForm .selectpicker").html(txt).selectpicker('refresh');
	$("#divForm .selectpicker").val(NA_defecto).selectpicker('refresh');

	for(x in Recursos)
	{
		$(`input[data-id_recurso="${Recursos[x].id}"]`).val(Recursos[x].nombre);
	}

	$("#Apply_to_all").off().on("change", Permisos_Recurso.funciones.checked);

	$("#guardarCambios").off().on("click", Permisos_Recurso.funciones.RegistroNewPerfil);
	$("#cancelar").off().on("click", Permisos_Recurso.gestionDOM.cerrarModal); 
}

Permisos_Recurso.gestionDOM.selectores_perfil = function(){

	var id_perfil = parseInt($($("#divForm .selectpicker")[0]).val());
	$("#divForm .selectpicker").val(id_perfil).selectpicker('refresh');
}

Permisos_Recurso.gestionDOM.cerrarModal = function(){
	gestionModal.cerrar();
}

/***********************************************************************************************/ 

Permisos_Recurso.gestionDOM.RegistroNewPerfil = function(){
	gestionModal.confirmacionSinBoton('success','El perfil se creó correctamente.');
	Permisos_Recurso.Info_NewPerfil = null;
}

Permisos_Recurso.gestionDOM.Perfil_existe = function(Info){

	var data = Permisos_Recurso.Info_NewPerfil;

	var Plantas = data.Plantas;
	var Recursos = data.Recursos;
	var NA = data.NA;
	var NA_defecto = data.NA_defecto[0].id;

	Permisos_Recurso.Info_NewPerfil = data;

	var aux_recursos;
	var txt = '';

	//Titulo para modal
	var titulo = "Crear Perfil";

	//Se carga el formulario en divForm	
	var ContenidoHTML = `
	<div id = "divForm">
		<div class="container_InLa C_perfil">
			<input autocomplete="off" type="text" required="" placeholder=" " id="N_perfil">
			<label>Perfil</label>
			<div class="msnError"></div>
		</div>
		<div class="container">
			<div class="custom-control custom-switch">
				<input type="checkbox" class="custom-control-input" id="Apply_to_all">
				<label class="custom-control-label" for="Apply_to_all">Aplicar a todos</label>
			</div>
			<div class="row perfil_recurso">`;
	
	for(x in Plantas){

		ContenidoHTML = `${ContenidoHTML}
				<div class ="col-sm-12 Cat_app mb-3">
					<span>${Plantas[x].nombre}</span>
				</div>`;

		aux_recursos = Recursos.filter(function(e){
			return e.id_Plantas == Plantas[x].id;
		})

		for(y in aux_recursos){
			ContenidoHTML = `${ContenidoHTML}
				<div class="container_InLa col-sm-6 mb-4">
					<input autocomplete="off" type="text" required="" placeholder="" data-id_recurso = "${aux_recursos[y].id}">
					<label>Recurso</label>
				</div>
				<div class="container_Select col-sm-6 mb-4">
					<select data-live-search="true" data-size="4" class='selectpicker' data-id_recurso = "${aux_recursos[y].id}">
						<option disabled value="0"> </option>
					</select>
					<div class="msnError"></div>
					<label>Nivel de acceso</label>
				</div>`;
		}
	}

	ContenidoHTML = `${ContenidoHTML}
			</div>
		</div>
	</div>`;

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

	// Selectores
	gestionBSelect.generic(`#divForm .selectpicker`,"");
	$(`#divForm .selectpicker`).removeAttr("title");

	for(x in NA)
	{
		txt = `${txt}<option value=${NA[x].id}>${NA[x].nombre}</option>`;
	}
	$("#divForm .selectpicker").html(txt).selectpicker('refresh');
	$("#divForm .selectpicker").val(NA_defecto).selectpicker('refresh');

	for(x in Recursos)
	{
		$(`input[data-id_recurso="${Recursos[x].id}"]`).val(Recursos[x].nombre);
	}

	/**********************************************************************************************/ 

	var recursos_ant = JSON.parse(Info.recursos);
	var nivel_acceso_ant = JSON.parse(Info.nivel_acceso);

	for(x in recursos_ant)
	{
		$(".container_Select").find(`select[data-id_recurso = '${recursos_ant[x]}']`).val(nivel_acceso_ant[x]);
	}
	$("#divForm .selectpicker").selectpicker('refresh');

	$("#N_perfil").val(Info.perfil);

	var mensajeError = `<i class="material-icons md-14">error</i>`;
	var mensaje = "Este nombre ya existe";
	
	$("#N_perfil").addClass("errorInput");
    $("#N_perfil").siblings(".msnError").html(`${mensajeError}<span> ${mensaje}</span>`);

	/**********************************************************************************************/ 


	$("#Apply_to_all").off().on("change", Permisos_Recurso.funciones.checked);

	$("#guardarCambios").off().on("click", Permisos_Recurso.funciones.RegistroNewPerfil);
	$("#cancelar").off().on("click", Permisos_Recurso.gestionDOM.cerrarModal);  
}

/**********************************************************************************************/ 
/**********************************************************************************************/ 
/**********************************************************************************************/ 

Permisos_Recurso.gestionDOM.table_P_view = function(){

	gestionModal.alertaBloqueante(primario.aplicacion, 'Procesando...');

	var text = `	
		<div id="Titulo">
			<a>Ver Permisos_Recurso:</a>
			<a>Perfiles</a>
		</div>`;

	$("#info_Opciones").hide();

	$("#cont_info_perfil").html(`
		<div id="Titulo">
			<a>Ver:</a>
			<a>Perfiles</a>
		</div>
		<div class="container_DataTable">
			<div id="divtabla_0" class="dataTableStyle tab-pane c_shadow rounded">		
				<!--para configurar opciones con los data--> 
				<!-- Por defecto siempre se organiza por la primera columna asc -->
				<table id="T_Permisos_Recurso" class="table-striped table-bordered" data-order='[[ 0, "asc" ]]' data-page-length='100'>
				</table> <!-- el id de la tabla si se puede cambiar -->
			</div>
		</div>
	`);

	gestionDT.initTable(
		Permisos_Recurso.urlConexion,
		"PERFILES",
		"#T_Permisos_Recurso", 
		Permisos_Recurso.gestionDOM.LoadPerfil,
		0
	);

}

//Callback de la generacion de la tabla
/* Tiene los siguientes parametros porque viene de un evento de DT */
Permisos_Recurso.gestionDOM.LoadPerfil = function(e, settings, json) {

	var txt = '';
	if( /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)){
		txt = `
				<button id="B_Back" class="btn btn-danger btn-sm">
					<i class="material-icons" style="vertical-align: middle; font-size: 21px;">
						replay
					</i>
					<p style="vertical-align: middle; margin: 0; cursor: pointer; display: inherit;">
						Ir a inicio
					</p>
				</button>`;
		$("#divtabla_0 #btn-toggle-option").append(txt);
	}
	else
	{
		txt = `
				<span class="px-2">
					<button id="B_Back" class="btn btn-danger btn-sm">
						<i class="material-icons" style="vertical-align: middle; font-size: 21px;">
							replay
						</i>
						<p style="vertical-align: middle; margin: 0; cursor: pointer; display: inherit;">
							Ir a inicio
						</p>
					</button>
				</span>
				`;
		$("#divtabla_0 .top").append(txt);
	}

	$("#B_Back").off().on("click", Permisos_Recurso.gestionDOM.B_Back);

	Permisos_Recurso.$tabla = new $.fn.dataTable.Api( `#${settings.nTable.id}` );

	$("#cont_info_perfil").resize();

    gestionModal.cerrar();
}

/********************************************************************************************************** */
/********************************************************************************************************** */
/********************************************************************************************************** */

Permisos_Recurso.gestionDOM.table_P_edit = function(){

	gestionModal.alertaBloqueante(primario.aplicacion, 'Procesando...');

	$("#info_Opciones").hide();

	$("#cont_info_perfil").html(`
		<div id="Titulo">
			<a>Editar:</a>
			<a>Perfiles</a>
		</div>
		<div class="container_DataTable">
			<div id="divtabla_0" class="dataTableStyle tab-pane c_shadow rounded">		
				<!--para configurar opciones con los data--> 
				<!-- Por defecto siempre se organiza por la primera columna asc -->
				<table id="T_Permisos_Recurso" class="table-striped table-bordered" data-order='[[ 0, "asc" ]]' data-page-length='100'>
				</table> <!-- el id de la tabla si se puede cambiar -->
			</div>
		</div>
	`);

	//Al clickar cualquier elemento tr dentro de tbody 
	$("#T_Permisos_Recurso").off().on("click","tbody tr",Permisos_Recurso.funciones.clickSelected);

	gestionDT.initTable(
		Permisos_Recurso.urlConexion,
		"PERFILES",
		"#T_Permisos_Recurso", 
		Permisos_Recurso.gestionDOM.LoadPerfil_edit,
		0
	);
}

Permisos_Recurso.gestionDOM.LoadPerfil_edit = function(e, settings, json) {

	var txt = '';
	if( /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)){
		txt = `
				<button id="upd_msv" class="btn btn-warning btn-sm">
					<i class="material-icons" style="vertical-align: middle; font-size: 21px;">
						groups
					</i>
					<p style="vertical-align: middle; margin: 0; cursor: pointer; display: inherit;">
						Cambio masivo
					</p>
				</button>

				<button id="B_Back" class="btn btn-danger btn-sm">
					<i class="material-icons" style="vertical-align: middle; font-size: 21px;">
						replay
					</i>
					<p style="vertical-align: middle; margin: 0; cursor: pointer; display: inherit;">
						Ir a inicio
					</p>
				</button>`;
		$("#divtabla_0 #btn-toggle-option").append(txt);
	}
	else
	{
		txt = `
				<span class="px-2">
					<button id="upd_msv" class="btn btn-warning btn-sm">
						<i class="material-icons" style="vertical-align: middle; font-size: 21px;">
							groups
						</i>
						<p style="vertical-align: middle; margin: 0; cursor: pointer; display: inherit;">
							Cambio masivo
						</p>
					</button>
				</span>
				<span class="px-2">
					<button id="B_Back" class="btn btn-danger btn-sm">
						<i class="material-icons" style="vertical-align: middle; font-size: 21px;">
							replay
						</i>
						<p style="vertical-align: middle; margin: 0; cursor: pointer; display: inherit;">
							Ir a inicio
						</p>
					</button>
				</span>`;
		$("#divtabla_0 .top").append(txt);
	}

	//Evento boton agregar
	if(Permisos_Recurso.baderaPermisosEdicion){

		$('#upd_msv').off("click").on("click",Permisos_Recurso.funciones.updatePerfilMsv);
	}
	else{ 

		//Se inhabilita el efecto de ondas
		$("#upd_msv").off();
		//Se le da el color de button disabled 
		$("#upd_msv").addClass('boton-desabilitado');
		// Tooltip con mensaje para usuario
		$("#upd_msv").tooltip({title: "No posee permisos para Editar", animation: true , placement: "right"}); 
	}

    $("#B_Back").off().on("click", Permisos_Recurso.gestionDOM.B_Back);
	
	Permisos_Recurso.$tabla = new $.fn.dataTable.Api( `#${settings.nTable.id}` ); 
	//Instancia de DataTable para utilizar el Api para los filtros

	//Se define evento para borrar el popover al reordenar
    Permisos_Recurso.$tabla.off('draw.dt').on('draw.dt',function(){setTimeout(Permisos_Recurso.gestionDOM.borrarPopover,1)});
    //Se define evento al seleccionar una fila de la tabla
    Permisos_Recurso.$tabla.off('select').on('select',Permisos_Recurso.gestionDOM.popoverDT);
    //Se detruye el popover cuando se haga scroll en el datatable
    $('div.dataTables_scrollBody').scroll(Permisos_Recurso.gestionDOM.borrarPopover);

	$("#cont_info_perfil").resize();
	
    gestionModal.cerrar();
}

/*****************************************************************************************************/
/*****************************************************************************************************/
/*****************************************************************************************************/

Permisos_Recurso.gestionDOM.popoverOff = function(){
	//Cerrar popover
	$(".popover").popover("dispose");
}

//eventos popover
Permisos_Recurso.gestionDOM.popoverDT = function(e, dt, type, indexes){

	var DT = Permisos_Recurso.$tabla.ajax.params().nombre;

	//Se borra el popover anterior
	Permisos_Recurso.gestionDOM.borrarPopover();
	// Se guardan los datos de la fila de la tabla actualmente seleccionada
	Permisos_Recurso.datosFilaTabla = $tabla.rows(indexes).data();
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
	Permisos_Recurso.gestionDOM.posicionPopoever(pop);
	//Se muestra el popover
	$(this).children("tbody").children("tr").eq(indexes[0]).popover("show");
	//No se muestra el popver hasta asustar posicion
	$(".popover").css("display","none");
	//Se cierra el popover en los botones de filtrar
	$(".filter-option").off("click").on("click",Permisos_Recurso.gestionDOM.cerrarPopover);
	//Elemento anterior
   	Permisos_Recurso.ultimoElementoConPopoever = $(this).children("tbody").children("tr").eq(indexes[0]);
   	//Se guarda el elemento tabla
   	Permisos_Recurso.tabla = $(this);
   	//Cada que se da click en el contenedor se mira que no tenga nada que ver con el popover y si es asi se oculta
   	$("body").off('click').on('click',Permisos_Recurso.gestionDOM.cerrarPopoverEnClick);
   	//Vuelve a esconder el popover al dar click en la fila otra vez
   	$(".selected").click(Permisos_Recurso.gestionDOM.borrarPopover);
	
	
	//Se definen los eventos de editar y borrar
    if(Permisos_Recurso.baderaPermisosEdicion){

		$("#edit").off("click").on("click", Permisos_Recurso.funciones.Editar_Perfil);
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

Permisos_Recurso.gestionDOM.posicionPopoever = function(pop){
	//Se restan 28px que son la mitad del tamaño del contenedor, como offset
	Permisos_Recurso.top = Permisos_Recurso.eventoSelected.clientY - 28;
	Permisos_Recurso.left = Permisos_Recurso.eventoSelected.clientX + 10;
}

Permisos_Recurso.gestionDOM.cerrarPopoverEnClick = function(e){
	var container = $(`.popover`);
	if(Permisos_Recurso.tabla != undefined){
		if ((!container.is(e.target)) && (!Permisos_Recurso.tabla.children("tbody").is(e.target.parentElement.parentElement)) 
		&& (container.has(e.target).length === 0)) { 
				//Se ha pulsado en cualquier lado fuera de los elementos contenidos en la variable container
				Permisos_Recurso.gestionDOM.borrarPopover();
		}
	}
}

Permisos_Recurso.gestionDOM.borrarPopover = function(){
	$(Permisos_Recurso.ultimoElementoConPopoever).popover("dispose");
}

//Cuando el popover esta generado (pero oculto):
Permisos_Recurso.gestionDOM.popoverFullyShown = function(){
	//Se ajusta la posicion del popover con las cooernadas del click
    $(".popover").css({"transform":`translate(${Permisos_Recurso.left}px,${Permisos_Recurso.top}px)`});
    //Se muetra el popover despues de acomodado
	$(".popover").css("display","block");
	//Se acomoda la flecha del popoever
	$(".arrow").css("top",`${parseInt(Permisos_Recurso.eventoSelected.clientY - ($(".popover").offset().top + 14))}px`);
}

/*****************************************************************************************************/
/*****************************************************************************************************/
/*****************************************************************************************************/

//Se crea el contenedor divForm Y en el se carga el formulario
Permisos_Recurso.gestionDOM.Editar_Perfil = function(data){

	var info_NA = data.NA;
	var info_PPR = data.PPR;
	var txt;

	//Titulo para modal
	var titulo = "Cambiar Nivel de Acceso";

	//Se carga el formulario en divForm	
	var ContenidoHTML = `
	<div id='divForm'>
		<div class="container">
			<div class="row mb-4">
				<div class = "col-sm-12">
					<div>
						<span>Nivel de Acceso:</span>
					</div>
					<!-- selector -->
					<div id = "selectorP" class="border-r-select">
						<select data-live-search="true" id="S_NA" class='selectpicker'>
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

	gestionBSelect.generic(`#S_NA`,"");
	$(`#S_NA`).removeAttr("title");

	txt= '';

	for(x in info_NA)
	{
		txt = `${txt}<option value=${info_NA[x].id}>${info_NA[x].nombre}</option>`;
	}
	$("#S_NA").html(txt).selectpicker('refresh');

	$("#S_NA").val(info_PPR[0].id).selectpicker('refresh');

	$("#guardarCambios").off().on("click", Permisos_Recurso.funciones.upd_P);
	$("#cancelar").off().on("click", Permisos_Recurso.gestionDOM.cerrarModal); 
}

Permisos_Recurso.gestionDOM.updatePerfilMsv =  function(data){

	var info_NA = data.NA;
	var txt;

	//Titulo para modal
	var titulo = "Cambiar Nivel de Acceso";

	//Se carga el formulario en divForm	
	var ContenidoHTML = `
	<div id='divForm'>
		<div class="container">
			<div class="row mb-4">
				<div class = "col-sm-12">
					<div>
						<span>Nivel de Acceso:</span>
					</div>
					<!-- selector -->
					<div id = "selectorP" class="border-r-select">
						<select data-live-search="true" id="S_NA" class='selectpicker'>
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

	gestionBSelect.generic(`#S_NA`,"");
	$(`#S_NA`).removeAttr("title");

	txt= '';

	for(x in info_NA)
	{
		txt = `${txt}<option value=${info_NA[x].id}>${info_NA[x].nombre}</option>`;
	}
	$("#S_NA").html(txt).selectpicker('refresh');

	$("#guardarCambios").off().on("click", Permisos_Recurso.funciones.upd_P_msv);
	$("#cancelar").off().on("click", Permisos_Recurso.gestionDOM.cerrarModal); 
}

/*****************************************************************************************************************/ 
/*****************************************************************************************************************/ 
/*****************************************************************************************************************/ 

Permisos_Recurso.gestionDOM.B_Back = function(){

    Permisos_Recurso.gestionDOM.borrarPopover();
    $("#cont_info_perfil").empty();
    $("#info_Opciones").show(); 
}

/*****************************************************************************************************************/ 
/*****************************************************************************************************************/ 
/*****************************************************************************************************************/ 

Permisos_Recurso.gestionDOM.UPD_Success = function(){

	//Se cargan la tabla con los nuevos datos
	Permisos_Recurso.$tabla.ajax.reload();

	//Modal de registro satisfactorio
	gestionModal.confirmacionSinBoton('success','Actualización exitosa');
}
