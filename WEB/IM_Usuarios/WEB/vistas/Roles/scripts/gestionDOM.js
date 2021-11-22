Roles.gestionDOM = new Object();

Roles.gestionDOM.html_edit = function(){
	var btn_add = `
		<div class="contenedor-Opc" id="crearRol">
			<i class="material-icons Rol_img">add_circle</i>
			<div class="info">
				<a class="texto">Crear rol</a>
			</div>
		</div>
	`;

	var btn_edit = `
		<div class="contenedor-Opc" id="Ed_permisos_menu">
			<i class="material-icons Rol_img">widgets</i>
			<div class="info">
				<a class="texto">Editar permisos - Menús</a>
			</div>
		</div>

		<div class="contenedor-Opc" id="Ed_permisos_perfil">
			<i class="material-icons Rol_img">wysiwyg</i>
			<div class="info">
				<a class="texto">Editar permisos - Perfiles</a>
			</div>
		</div>

		<div class="contenedor-Opc" id="Ed_permisos_modulo">
		<i class="material-icons Rol_img">qr_code</i>
		<div class="info">
			<a class="texto">Editar permisos - Módulos</a>
		</div>
	</div>
	`;

	$("#ContenedorOpc").prepend(btn_add);
	$("#ContenedorOpc").append(btn_edit);

	$("#crearRol").off().on("click", Roles.funciones.CrearRol);
	$("#Ed_permisos_menu").off().on("click", Roles.funciones.Ed_permisos_menu);
	$("#Ed_permisos_perfil").off().on("click", {DT: 'ROLES_PERFIL'}, Roles.gestionDOM.table_MR_edit);
	$("#Ed_permisos_modulo").off().on("click", {DT: 'ROLES_APP'}, Roles.gestionDOM.table_MR_edit);
}

Roles.gestionDOM.CrearRol = function(data){

	var Perfiles = data.Perfiles;
	var PD = data.Perfil_defecto[0].id;

	Roles.Info_NewRol = data;

	var txt = '';

	//Titulo para modal
	var titulo = "Crear Rol";

	//Se carga el formulario en divForm	
	var ContenidoHTML = `
	<div id = "divForm">
		<div class="container_InLa C_rol">
			<input autocomplete="off" type="text" required="" placeholder=" " id="N_rol">
			<label>Rol</label>
			<div class="msnError"></div>
		</div>
		<div class="container_Select">
			<select data-live-search="true" data-size="4" class='selectpicker' id="N_perfil">
				<option disabled value="0"> </option>
			</select>
			<div class="msnError"></div>
			<label>Perfil</label>
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
	gestionBSelect.generic(`#N_perfil`,"");
	$(`#N_perfil`).removeAttr("title");

	for(x in Perfiles)
	{
		txt = `${txt}<option value=${Perfiles[x].id}>${Perfiles[x].nombre}</option>`;
	}
	$("#N_perfil").html(txt).selectpicker('refresh');
	$("#N_perfil").val(PD).selectpicker('refresh');

	$("#guardarCambios").off().on("click", Roles.funciones.RegistroNewRol);
	$("#cancelar").off().on("click", Roles.gestionDOM.cerrarModal); 
}

Roles.gestionDOM.cerrarModal = function(){
	gestionModal.cerrar();
}

Roles.gestionDOM.RegistroNewRol = function(){
	gestionModal.confirmacionSinBoton('success','El rol se creó correctamente.');
	Roles.Info_NewRol = null;
}

Roles.gestionDOM.Rol_existe = function(Info){

	var data = Roles.Info_NewRol;
	var Perfiles = data.Perfiles;

	var txt = '';

	//Titulo para modal
	var titulo = "Crear Rol";

	//Se carga el formulario en divForm	
	var ContenidoHTML = `
	<div id = "divForm">
		<div class="container_InLa C_rol">
			<input autocomplete="off" type="text" required="" placeholder=" " id="N_rol">
			<label>Rol</label>
			<div class="msnError"></div>
		</div>
		<div class="container_Select">
			<select data-live-search="true" data-size="4" class='selectpicker' id="N_perfil">
				<option disabled value="0"> </option>
			</select>
			<div class="msnError"></div>
			<label>Perfil</label>
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
	gestionBSelect.generic(`#N_perfil`,"");
	$(`#N_perfil`).removeAttr("title");

	for(x in Perfiles)
	{
		txt = `${txt}<option value=${Perfiles[x].id}>${Perfiles[x].nombre}</option>`;
	}
	$("#N_perfil").html(txt).selectpicker('refresh');

	/**********************************************************************************************/ 

	$("#N_perfil").val(Info.id_perfil).selectpicker('refresh');

	$("#N_rol").val(Info.rol);

	var mensajeError = `<i class="material-icons md-14">error</i>`;
	var mensaje = "Este nombre ya existe";
	
	$("#N_rol").addClass("errorInput");
    $("#N_rol").siblings(".msnError").html(`${mensajeError}<span> ${mensaje}</span>`);

	/**********************************************************************************************/ 

	$("#guardarCambios").off().on("click", Roles.funciones.RegistroNewRol);
	$("#cancelar").off().on("click", Roles.gestionDOM.cerrarModal); 
}

/**********************************************************************************************/ 
/**********************************************************************************************/ 
/**********************************************************************************************/ 

Roles.gestionDOM.table_MR_view = function(ev){

	var DT = ev.data.DT;
	var Perfil = false;
	var text;

	gestionModal.alertaBloqueante(primario.aplicacion, 'Procesando...');

	switch(DT){
		case "ROLES_GRUPO":
			text = `	<div id="Titulo">
							<a>Ver roles:</a>
							<a>Grupos</a>
						</div>`;
		break;

		case "ROLES_MENU":
			text = `	<div id="Titulo">
							<a>Ver roles:</a>
							<a>Menús</a>
						</div>`;
		break;

		case "ROLES_SUBMENU":
			text = `	<div id="Titulo">
							<a>Ver roles:</a>
							<a>Sub menús</a>
						</div>`;
		break;

		case "ROLES_APP":
			text = `	<div id="Titulo">
							<a>Ver roles:</a>
							<a>Módulos</a>
						</div>`;
			Perfil = true;
		break;

		default:
			text = `	<div id="Titulo">
							<a>Ver roles:</a>
							<a>Perfiles</a>
						</div>`;
			Perfil = true;
		break;
	}

	$("#info_Opciones").hide();

	$("#cont_info_rol").html(`
		${text}
		<div class="container_DataTable">
			<div id="divtabla_0" class="dataTableStyle tab-pane c_shadow rounded">		
				<!--para configurar opciones con los data--> 
				<!-- Por defecto siempre se organiza por la primera columna asc -->
				<table id="T_Roles" class="table-striped table-bordered" data-order='[[ 0, "asc" ]]' data-page-length='100'>
				</table> <!-- el id de la tabla si se puede cambiar -->
			</div>
		</div>
	`);

	if(Perfil){
		gestionDT.initTable(
			Roles.urlConexion,
			DT,
			"#T_Roles", 
			Roles.gestionDOM.LoadPerfil_App,
			0
		);
	}
	else
	{
		gestionDT.initTable(
			Roles.urlConexion,
			DT,
			"#T_Roles", 
			Roles.gestionDOM.LoadMenu,
			0
		);
	}
}

//Callback de la generacion de la tabla
/* Tiene los siguientes parametros porque viene de un evento de DT */
Roles.gestionDOM.LoadPerfil_App = function(e, settings, json) {

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

	$("#B_Back").off().on("click", Roles.gestionDOM.B_Back);

	Roles.$tabla = new $.fn.dataTable.Api( `#${settings.nTable.id}` );

	$("#cont_info_rol").resize();

    gestionModal.cerrar();
}

Roles.gestionDOM.LoadMenu = function(e, settings, json) {

	var txt = '';
	if( /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)){
		txt = `
				<button id="B_Ini" class="btn btn-danger btn-sm">
					<i class="material-icons" style="vertical-align: middle; font-size: 21px;">
						replay
					</i>
					<p style="vertical-align: middle; margin: 0; cursor: pointer; display: inherit;">
						Ir a inicio
					</p>
				</button>
				<button id="B_Back" class="btn btn-secondary btn-sm">
					<i class="material-icons" style="vertical-align: middle; font-size: 21px;">
						arrow_back
					</i>
					<p style="vertical-align: middle; margin: 0; cursor: pointer; display: inherit;">
						volver
					</p>
				</button>`;
		$("#divtabla_0 #btn-toggle-option").append(txt);
	}
	else
	{
		txt = `
				<span class="px-2">
					<button id="B_Ini" class="btn btn-danger btn-sm">
						<i class="material-icons" style="vertical-align: middle; font-size: 21px;">
							replay
						</i>
						<p style="vertical-align: middle; margin: 0; cursor: pointer; display: inherit;">
							Ir a inicio
						</p>
					</button>
				</span>
				<span class="px-2">
					<button id="B_Back" class="btn btn-secondary btn-sm">
						<i class="material-icons" style="vertical-align: middle; font-size: 21px;">
							arrow_back
						</i>
						<p style="vertical-align: middle; margin: 0; cursor: pointer; display: inherit;">
							volver
						</p>
					</button>
				</span>`;
		$("#divtabla_0 .top").append(txt);
	}

	$("#B_Back").off().on("click", Roles.funciones.visualizar_RM);
	$("#B_Ini").off().on("click", Roles.gestionDOM.B_Back);
    
	Roles.$tabla = new $.fn.dataTable.Api( `#${settings.nTable.id}` );

	$("#cont_info_rol").resize();

    gestionModal.cerrar();
}

/***********************************************************************************************************/
/***********************************************************************************************************/


Roles.gestionDOM.TiposMenus = function(){
	var Contenido = `
	<div id="ContenedorRol">
		<div class="contenedor-Rol" id="T_grupo">
		<img draggable="false" class="Roles_img" src="imagenes/T_grupo.png">
			<div class="info">
				<a class="texto">Grupo</a>
			</div>
		</div>
		<div class="contenedor-Rol" id="T_menu">
			<img draggable="false" class="Roles_img" src="imagenes/T_menu.png">
			<div class="info">
				<a class="texto">Menú</a>
			</div>
		</div>
		<div class="contenedor-Rol" id="T_submenu">
			<img draggable="false" class="Roles_img" src="imagenes/T_submenu.png">
			<div class="info">
				<a class="texto">Submenú</a>
			</div>
		</div>
		<div class="contenedor-Rol" id="T_volver">
			<img draggable="false" class="Roles_img" src="imagenes/T_volver.png">
			<div class="info">
				<a class="texto">Volver</a>
			</div>
		</div>
	</div>`;

	Contenido = `${Contenido}
		</div>`;

	$("#info_Opciones").hide();
	$("#cont_info_rol").html(Contenido);

	$("#T_volver").off().on("click", Roles.gestionDOM.B_Back);
}

/********************************************************************************************************** */
/********************************************************************************************************** */
/********************************************************************************************************** */

Roles.gestionDOM.table_MR_edit = function(ev){

	var DT = ev.data.DT;
	var Perfil = 0;
	var text;

	gestionModal.alertaBloqueante(primario.aplicacion, 'Procesando...');

	switch(DT){
		case "ROLES_GRUPO":
			text = `	<div id="Titulo">
							<a>Ver roles:</a>
							<a>Grupos</a>
						</div>`;
		break;

		case "ROLES_MENU":
			text = `	<div id="Titulo">
							<a>Ver roles:</a>
							<a>Menús</a>
						</div>`;
		break;

		case "ROLES_SUBMENU":
			text = `	<div id="Titulo">
							<a>Ver roles:</a>
							<a>Sub menús</a>
						</div>`;
		break;

		case "ROLES_APP":
			text = `	<div id="Titulo">
							<a>Ver roles:</a>
							<a>Módulos</a>
						</div>`;
			Perfil = 1;
		break;

		default:
			text = `	<div id="Titulo">
							<a>Ver roles:</a>
							<a>Perfiles</a>
						</div>`;
			Perfil = 2;
		break;
	}

	$("#info_Opciones").hide();

	$("#cont_info_rol").html(`
		${text}
		<div class="container_DataTable">
			<div id="divtabla_0" class="dataTableStyle tab-pane c_shadow rounded">		
				<!--para configurar opciones con los data--> 
				<!-- Por defecto siempre se organiza por la primera columna asc -->
				<table id="T_Roles" class="table-striped table-bordered" data-order='[[ 0, "asc" ]]' data-page-length='100'>
				</table> <!-- el id de la tabla si se puede cambiar -->
			</div>
		</div>
	`);

	//Al clickar cualquier elemento tr dentro de tbody 
	$("#T_Roles").off().on("click","tbody tr",Roles.funciones.clickSelected);

	if(Perfil == 1){
		gestionDT.initTable(
			Roles.urlConexion,
			DT,
			"#T_Roles", 
			Roles.gestionDOM.LoadApp_edit,
			0,
			"",
			"",
			"",
			{ SelectCells: 0}
		);
	}
	else if(Perfil == 2)
	{
		gestionDT.initTable(
			Roles.urlConexion,
			DT,
			"#T_Roles", 
			Roles.gestionDOM.LoadPerfil_edit,
			0,
			"",
			"",
			"",
			{ SelectCells: 0}
		);
	}
	else
	{
		gestionDT.initTable(
			Roles.urlConexion,
			DT,
			"#T_Roles", 
			Roles.gestionDOM.LoadMenu_edit,
			0,
			"",
			"",
			"",
			{ SelectCells: 0}
		);
	}
}

Roles.gestionDOM.LoadApp_edit = function(e, settings, json) {

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
	if(Roles.baderaPermisosEdicion){

		$('#upd_msv').off("click").on("click",Roles.funciones.updateRolAppMsv);
	}
	else{ 

		//Se inhabilita el efecto de ondas
		$("#upd_msv").off();
		//Se le da el color de button disabled 
		$("#upd_msv").addClass('boton-desabilitado');
		// Tooltip con mensaje para usuario
		$("#upd_msv").tooltip({title: "No posee permisos para Editar", animation: true , placement: "right"}); 
	}

    $("#B_Back").off().on("click", Roles.gestionDOM.B_Back);
	
	Roles.$tabla = new $.fn.dataTable.Api( `#${settings.nTable.id}` ); 
	//Instancia de DataTable para utilizar el Api para los filtros

	//Se define evento para borrar el popover al reordenar
    Roles.$tabla.off('draw.dt').on('draw.dt',function(){setTimeout(Roles.gestionDOM.borrarPopover,1)});
    //Se define evento al seleccionar una fila de la tabla
    Roles.$tabla.off('select').on('select',Roles.gestionDOM.popoverDT);
    //Se detruye el popover cuando se haga scroll en el datatable
    $('div.dataTables_scrollBody').scroll(Roles.gestionDOM.borrarPopover);

    $("#cont_info_rol").resize();
    gestionModal.cerrar();
}

Roles.gestionDOM.LoadPerfil_edit = function(e, settings, json) {

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
	if(Roles.baderaPermisosEdicion){

		$('#upd_msv').off("click").on("click",Roles.funciones.updateRolPerfilMsv);
	}
	else{ 

		//Se inhabilita el efecto de ondas
		$("#upd_msv").off();
		//Se le da el color de button disabled 
		$("#upd_msv").addClass('boton-desabilitado');
		// Tooltip con mensaje para usuario
		$("#upd_msv").tooltip({title: "No posee permisos para Editar", animation: true , placement: "right"}); 
	}

    $("#B_Back").off().on("click", Roles.gestionDOM.B_Back);
	
	Roles.$tabla = new $.fn.dataTable.Api( `#${settings.nTable.id}` ); 
	//Instancia de DataTable para utilizar el Api para los filtros

	//Se define evento para borrar el popover al reordenar
    Roles.$tabla.off('draw.dt').on('draw.dt',function(){setTimeout(Roles.gestionDOM.borrarPopover,1)});
    //Se define evento al seleccionar una fila de la tabla
    Roles.$tabla.off('select').on('select',Roles.gestionDOM.popoverDT);
    //Se detruye el popover cuando se haga scroll en el datatable
    $('div.dataTables_scrollBody').scroll(Roles.gestionDOM.borrarPopover);

    $("#cont_info_rol").resize();
    gestionModal.cerrar();
}

Roles.gestionDOM.LoadMenu_edit = function(e, settings, json) {

	var TM = settings.ajax.data.nombre;

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

				<button id="B_Ini" class="btn btn-danger btn-sm">
					<i class="material-icons" style="vertical-align: middle; font-size: 21px;">
						replay
					</i>
					<p style="vertical-align: middle; margin: 0; cursor: pointer; display: inherit;">
						Ir a inicio
					</p>
				</button>
				
				<button id="B_Back" class="btn btn-secondary btn-sm">
					<i class="material-icons" style="vertical-align: middle; font-size: 21px;">
						arrow_back
					</i>
					<p style="vertical-align: middle; margin: 0; cursor: pointer; display: inherit;">
						volver
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
					<button id="B_Ini" class="btn btn-danger btn-sm">
						<i class="material-icons" style="vertical-align: middle; font-size: 21px;">
							replay
						</i>
						<p style="vertical-align: middle; margin: 0; cursor: pointer; display: inherit;">
							Ir a inicio
						</p>
					</button>
				</span>
				
				<span class="px-2">
					<button id="B_Back" class="btn btn-secondary btn-sm">
						<i class="material-icons" style="vertical-align: middle; font-size: 21px;">
							arrow_back
						</i>
						<p style="vertical-align: middle; margin: 0; cursor: pointer; display: inherit;">
							volver
						</p>
					</button>
				</span>`;

		$("#divtabla_0 .top").append(txt);
	}

	//Evento boton agregar
	if(Roles.baderaPermisosEdicion){

		var DM;

		switch(TM){
			case "ROLES_GRUPO":
				DM = 1;
			break;
	
			case "ROLES_MENU":
				DM = 2;
			break;
	
			default:
				DM = 3;
			break;
		}

		var DataM = {
			DM: DM
		};

		$('#upd_msv').on("click", DataM, Roles.funciones.updateRolMenuMsv);
	}
	else{ 

		//Se inhabilita el efecto de ondas
		$("#upd_msv").off();
		//Se le da el color de button disabled 
		$("#upd_msv").addClass('boton-desabilitado');
		// Tooltip con mensaje para usuario
		$("#upd_msv").tooltip({title: "No posee permisos para Editar", animation: true , placement: "right"}); 
	}

	$("#B_Back").off().on("click", Roles.funciones.Ed_permisos_menu);
	$("#B_Ini").off().on("click", Roles.gestionDOM.B_Back);
	
	Roles.$tabla = new $.fn.dataTable.Api( `#${settings.nTable.id}` ); 
	//Instancia de DataTable para utilizar el Api para los filtros

	//Se define evento para borrar el popover al reordenar
    Roles.$tabla.off('draw.dt').on('draw.dt',function(){setTimeout(Roles.gestionDOM.borrarPopover,1)});
    //Se define evento al seleccionar una fila de la tabla
    Roles.$tabla.off('select').on('select',Roles.gestionDOM.popoverDT);
    //Se detruye el popover cuando se haga scroll en el datatable
    $('div.dataTables_scrollBody').scroll(Roles.gestionDOM.borrarPopover);

    $("#cont_info_rol").resize();
    gestionModal.cerrar();
}

/*****************************************************************************************************/
/*****************************************************************************************************/
/*****************************************************************************************************/

Roles.gestionDOM.popoverOff = function(){
	//Cerrar popover
	$(".popover").popover("dispose");
}

//eventos popover
Roles.gestionDOM.popoverDT = function(e, dt, type, indexes){

	var DT = Roles.$tabla.ajax.params().nombre;

	//Se borra el popover anterior
	Roles.gestionDOM.borrarPopover();
	// Se guardan los datos de la fila de la tabla actualmente seleccionada
	Roles.datosFilaTabla = Roles.$tabla.rows(indexes).data();
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
	Roles.gestionDOM.posicionPopoever(pop);
	//Se muestra el popover
	$(this).children("tbody").children("tr").eq(indexes[0]).popover("show");
	//No se muestra el popver hasta asustar posicion
	$(".popover").css("display","none");
	//Se cierra el popover en los botones de filtrar
	$(".filter-option").off("click").on("click",Roles.gestionDOM.cerrarPopover);
	//Elemento anterior
   	Roles.ultimoElementoConPopoever = $(this).children("tbody").children("tr").eq(indexes[0]);
   	//Se guarda el elemento tabla
   	Roles.tabla = $(this);
   	//Cada que se da click en el contenedor se mira que no tenga nada que ver con el popover y si es asi se oculta
   	$("body").off('click').on('click',Roles.gestionDOM.cerrarPopoverEnClick);
   	//Vuelve a esconder el popover al dar click en la fila otra vez
   	$(".selected").click(Roles.gestionDOM.borrarPopover);
	
	
	//Se definen los eventos de editar y borrar
    if(Roles.baderaPermisosEdicion){
		//Evento boton editar
		if(DT == "ROLES_PERFIL")
		{
			$("#edit").off("click").on("click", Roles.funciones.Editar_Perfil);
		}
		else if(DT == "ROLES_APP")
		{
			$("#edit").off("click").on("click", Roles.funciones.Editar_Modulo);
		}
		else
		{
			var DataM = {
				DT: DT
			};
			$("#edit").off("click").on("click", DataM, Roles.funciones.Editar_Menu);
		}
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

Roles.gestionDOM.posicionPopoever = function(pop){
	//Se restan 28px que son la mitad del tamaño del contenedor, como offset
	Roles.top = Roles.eventoSelected.clientY - 28;
	Roles.left = Roles.eventoSelected.clientX + 10;
}

Roles.gestionDOM.cerrarPopoverEnClick = function(e){
	var container = $(`.popover`);
	if(Roles.tabla != undefined){
		if ((!container.is(e.target)) && (!Roles.tabla.children("tbody").is(e.target.parentElement.parentElement)) 
		&& (container.has(e.target).length === 0)) { 
				//Se ha pulsado en cualquier lado fuera de los elementos contenidos en la variable container
				Roles.gestionDOM.borrarPopover();
		}
	}
}

Roles.gestionDOM.borrarPopover = function(){
	$(Roles.ultimoElementoConPopoever).popover("dispose");
}

//Cuando el popover esta generado (pero oculto):
Roles.gestionDOM.popoverFullyShown = function(){
	//Se ajusta la posicion del popover con las cooernadas del click
    $(".popover").css({"transform":`translate(${Roles.left}px,${Roles.top}px)`});
    //Se muetra el popover despues de acomodado
	$(".popover").css("display","block");
	//Se acomoda la flecha del popoever
	$(".arrow").css("top",`${parseInt(Roles.eventoSelected.clientY - ($(".popover").offset().top + 14))}px`);
}

/*****************************************************************************************************/
/*****************************************************************************************************/
/*****************************************************************************************************/

//Se crea el contenedor divForm Y en el se carga el formulario
Roles.gestionDOM.Editar_Modulo = function(data){

	var info_NA = data.NA;
	var id_NA = data.R_A[0].id_nivel_acceso;
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
	$("#S_NA").val(id_NA).selectpicker('refresh');

	$("#guardarCambios").off().on("click", Roles.funciones.upd_RA);
	$("#cancelar").off().on("click", Roles.gestionDOM.cerrarModal); 
}

Roles.gestionDOM.updateRolAppMsv =  function(data){
	
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

	$("#guardarCambios").off().on("click", Roles.funciones.upd_RA_msv);
	$("#cancelar").off().on("click", Roles.gestionDOM.cerrarModal); 
}

/*****************************************************************************************************/
/*****************************************************************************************************/
/*****************************************************************************************************/

//Se crea el contenedor divForm Y en el se carga el formulario
Roles.gestionDOM.Editar_Perfil = function(data){
	var info_perfil = data.Perfil;
	var info_RP = data.R_P;
	var txt;

	//Titulo para modal
	var titulo = "Cambiar perfil";

	//Se carga el formulario en divForm	
	var ContenidoHTML = `
	<div id='divForm'>
		<div class="container">
			<div class="row mb-4">
				<div class = "col-sm-12">
					<div>
						<span>Perfil:</span>
					</div>
					<!-- selector -->
					<div id = "selectorP" class="border-r-select">
						<select data-live-search="true" id="S_Perfil" class='selectpicker'>
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

	gestionBSelect.generic(`#S_Perfil`,"");
	$(`#S_Perfil`).removeAttr("title");

	txt= '';

	for(x in info_perfil)
	{
		txt = `${txt}<option value=${info_perfil[x].id}>${info_perfil[x].nombre}</option>`;
	}
	$("#S_Perfil").html(txt).selectpicker('refresh');

	$("#S_Perfil").val(info_RP[0].id_perfil).selectpicker('refresh');

	$("#guardarCambios").off().on("click", Roles.funciones.upd_RP);
	$("#cancelar").off().on("click", Roles.gestionDOM.cerrarModal); 
}

Roles.gestionDOM.updateRolPerfilMsv =  function(data){
	var info_perfil = data.Perfil;
	var txt;

	//Titulo para modal
	var titulo = "Cambiar perfil";

	//Se carga el formulario en divForm	
	var ContenidoHTML = `
	<div id='divForm'>
		<div class="container">
			<div class="row mb-4">
				<div class = "col-sm-12">
					<div>
						<span>Perfil:</span>
					</div>
					<!-- selector -->
					<div id = "selectorP" class="border-r-select">
						<select data-live-search="true" id="S_Perfil" class='selectpicker'>
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

	gestionBSelect.generic(`#S_Perfil`,"");
	$(`#S_Perfil`).removeAttr("title");

	txt= '';

	for(x in info_perfil)
	{
		txt = `${txt}<option value=${info_perfil[x].id}>${info_perfil[x].nombre}</option>`;
	}
	$("#S_Perfil").html(txt).selectpicker('refresh');

	$("#guardarCambios").off().on("click", Roles.funciones.upd_RP_msv);
	$("#cancelar").off().on("click", Roles.gestionDOM.cerrarModal); 
}

/*****************************************************************************************************************/ 
/*****************************************************************************************************************/ 
/*****************************************************************************************************************/ 

Roles.gestionDOM.Editar_Menu = function(data, info){ 

	var info_NA = data.NA;
	var nivel_acceso = info.NA;
	var id_NA = undefined;
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

		if(info_NA[x].nombre == nivel_acceso){
			id_NA = info_NA[x].id;
		}
	}
	$("#S_NA").html(txt).selectpicker('refresh');

	if(id_NA != undefined){
		$("#S_NA").val(id_NA).selectpicker('refresh');
	}
	else
	{
		$(".swal2-contentwrapper #divBoton").before(`
			<p style="font-weight: 600;">
				Los permisos asignados son diferentes entre sí.
			</p>`
		);
	}

	$("#guardarCambios").off().on("click", info, Roles.funciones.upd_RM);
	$("#cancelar").off().on("click", Roles.gestionDOM.cerrarModal); 
}

Roles.gestionDOM.updateRolMenuMsv = function(data, DM){

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

	$("#guardarCambios").off().on("click", {DM: DM}, Roles.funciones.upd_RM_msv);
	$("#cancelar").off().on("click", Roles.gestionDOM.cerrarModal); 
}

/*****************************************************************************************************************/ 
/*****************************************************************************************************************/ 
/*****************************************************************************************************************/ 

Roles.gestionDOM.B_Back = function(){

    Roles.gestionDOM.borrarPopover();
    $("#cont_info_rol").empty();
    $("#info_Opciones").show(); 
}

/*****************************************************************************************************************/ 
/*****************************************************************************************************************/ 
/*****************************************************************************************************************/ 

Roles.gestionDOM.UPD_Success = function(){

	//Se cargan la tabla con los nuevos datos
	Roles.$tabla.ajax.reload();

	//Modal de registro satisfactorio
	gestionModal.confirmacionSinBoton('success','Actualización exitosa');
}
