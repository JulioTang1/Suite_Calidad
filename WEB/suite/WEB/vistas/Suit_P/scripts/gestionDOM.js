
Suit_P.gestionDOM = new Object();

Suit_P.gestionDOM.genApps = function(){

	var txt = '';
	var events;
	for (var j = 0; j < Login.CAT_APPS.length; j++) {
		txt = `${txt}
	<div class="master_container" style="background-color: ${Login.CAT_APPS[j].background};">
		<div class="title_cat" style="color: ${Login.CAT_APPS[j].color_letra};">
			<span>${Login.CAT_APPS[j].cat_nombre}</span>
		</div>
		<div class="grid-container">`;

		events = Login.APPS.filter( x => x.cat_nombre === Login.CAT_APPS[j].cat_nombre);

		for(var event of events) {
			txt = `${txt}
			<div class="apps" id="${event.nombre}">
				<div class="apps_login">
					<div class="logo_app">`;
			if(event.Acceso == 0){
				txt = `${txt}
						<i class="material-icons acceso">block</i>`;
			}
			txt = `${txt}
						<img draggable="false" src="${event.url_login_img}">
					</div>
					<div class="nombre_app">
						<span>${event.titulo_login}</span>
					</div>
				</div>
				<div class="icons_app">
					<div class="users_online" title="Usuarios en línea">
						<div class="user_icon">
							<i class="material-icons">group</i>	
						</div>
						<div class="N_online">
							${event.on_line}
						</div>
					</div>
					<div class="access_app">
						<i class="material-icons" title="Información">info</i>
					</div>
				</div>
			</div>`;
		}
		txt = `${txt}
		</div>
	</div>`;
	}
	$("#P-container").html(txt);
	
	$(".apps .apps_login").off().on("click", Suit_P.funciones.login);
	// $(".apps .icons_app .access_app i").off().on("click", Suit_P.funciones.infoApp);

	//tooltip de usuarios en línea y solicitar acceso
	$('.users_online').tooltip();
	$('.access_app i').tooltip();
}


Suit_P.gestionDOM.genInfoApp = function(info){

	var acceso = info.Acceso;

	var txt = `
		<div class="botones">
			<div class="back_app">
				<i class="material-icons">keyboard_backspace</i>
			</div>`;
	if(acceso == 0){
		txt = `${txt}
			<div class="add_app">
				<i class="material-icons">add_circle</i>
			</div>`;
	}
	txt = `${txt}
		</div>

		<div class="Name_User">
			<div class="Name_app">
				<span>${info.titulo_login}</span>
			</div>
			<div class="UserReg" title="Usuarios Registrados">
				<span>${info.user_reg} Usuarios</span>
				<i class="material-icons">person_add</i>	
			</div>
		</div>

		<div class="logoApp">
			<img draggable="false" src="${info.url_login_img}">
		</div>

		<div class="Descripcion">${info.descripcion}</div>

		<div id="Carrusel" class="carousel slide" data-ride="carousel">
			<!-- Indicators -->
			<ul class="carousel-indicators">
				<li data-target="#Carrusel" data-slide-to="0" class="active"></li>
				<li data-target="#Carrusel" data-slide-to="1"></li>
				<li data-target="#Carrusel" data-slide-to="2"></li>
				<li data-target="#Carrusel" data-slide-to="3"></li>
				<li data-target="#Carrusel" data-slide-to="4"></li>
			</ul>

			<!-- The slideshow -->
			<div class="carousel-inner">
				<div class="carousel-item active">
					<img draggable="false" src="imagenes/${info.nombre}/1.png">
				</div>
				<div class="carousel-item">
					<img draggable="false" src="imagenes/${info.nombre}/2.png">
				</div>
				<div class="carousel-item">
					<img draggable="false" src="imagenes/${info.nombre}/3.png">
				</div>
				<div class="carousel-item">
					<img draggable="false" src="imagenes/${info.nombre}/4.png">
				</div>
				<div class="carousel-item">
					<img draggable="false" src="imagenes/${info.nombre}/5.png">
				</div>
			</div>

			<!-- Left and right controls -->
			<a class="carousel-control-prev" href="#Carrusel" data-slide="prev">
				<i class="material-icons">keyboard_arrow_left</i>
			</a>
			<a class="carousel-control-next" href="#Carrusel" data-slide="next">
				<i class="material-icons">keyboard_arrow_right</i>
			</a>
		</div>
	</div>`;

	$(".grid-infoApp").html(txt);
	$('.Name_User .UserReg').tooltip();

	$("#P-container").css("display","none");
	$(".grid-infoApp").css("display","grid");

	//Se le asigna el evento al boton de regresar a la suite 
	$(".grid-infoApp .botones .back_app i").off().on("click", Suit_P.gestionDOM.back_suite);

	//Se le asigna el evento al boton de solicitar acceso a la aplicación
	$(".grid-infoApp .botones .add_app i").off();

	if(acceso == 0){
		Suit_P.gestionDOM.Inactivo_rol();
	}
	
}

Suit_P.gestionDOM.back_suite = function(){

	Suit_P.funciones.Update_user_online();

	$("#P-container").css("display","block");
	$(".grid-infoApp").css("display","none");
	$(".grid-infoApp").empty();
}

/******************************************************************************************************/
Suit_P.gestionDOM.genAppsTop = function(){

	var txt = '';
	var Categorias = Login.CAT_APPS;

	for (var cat of Categorias) {
		txt = `${txt}
			<h5 class="dropdown-header">${cat.cat_nombre}</h5>`;

		modulos = Login.APPS.filter(x => x.cat_nombre === cat.cat_nombre);

		for (var mod of modulos) {
			if(mod.Acceso == 1){
				txt = `${txt}
			<a class="dropdown-item" id="${mod.nombre}">
				<div class="info_apps img">
					<img draggable="false" src="${mod.url_login_img}">
				</div>
				<div class="info_apps txt">
					<span>${mod.titulo_login}</span>
				</div>
			</a>`;
			}
		}
	}
	if(txt == '')
	{
		$('#logo_OPC').prop('disabled', true);
	}
	else
	{
		txt = `${txt}
		<div class="dropdown-divider L_sep" style="display: none;"></div>
		<a class="dropdown-item" id="volver_suite" style="display: none;">
			<div class="info_apps img">
				<img draggable="false" src="${primario.imagen_principal}">
			</div>
			<div class="info_apps txt">
				<span>Ir a inicio</span>
			</div>
		</a>`;

		$('#logo_OPC').css("cursor","pointer");
		$("#menuApp").html(txt);
		$("#menuApp .dropdown-item").off().on("click", Suit_P.funciones.loginTop);
		$('#logo_OPC').yarp({colors: ['#1c202925'],duration: 500});
	}
}


Suit_P.gestionDOM.back_suiteTop = function(){

	Suit_P.funciones.Update_user_online();

	$("#logo_OPC .logo-wrapper").attr("src", primario.imagen_principal);

	$("#iframeApps").css("display","none");
	$("#NameViewTopBar").empty();

	$("#P-container").css("display","block");
	$("#back_APP").css("display","none");
	$(".L_sep").css("display","none");
	$("#volver_suite").css("display","none");
	
	document.getElementById("iframeApps").src = "";
}

/******************************************************************************************************/

Suit_P.gestionDOM.updateInterface = function(info){

	if(info.estado == "detalle"){
		var usuarios = Login.APPS.filter(apps => apps.id == info.id_app)[0].user_reg;
		$(".grid-infoApp .Name_User .UserReg span").html(`${usuarios} Usuarios`);
	}
	else
	{
		Suit_P.gestionDOM.update_user_online();
	}
}

Suit_P.gestionDOM.update_user_online = function(){
	var app;
	var user;

	for (var i = 0; i < Login.APPS.length; i++) {

		app = Login.APPS[i].nombre;
		user = Login.APPS[i].on_line;

		$(`[id='${app}'] .icons_app .users_online .N_online`).html(`${user}`);
	}
}

/******************************************************************************************************/ 

Suit_P.gestionDOM.Inactivo_rol = function(){
	gestionModal.alertaConfirmacion(
		primario.aplicacion,
		"El rol asigando a su usuario no posee permisos para ingresar a este módulo, si esto le presenta inconvenientes, comuniquese con el administrador",
		"warning",
		"Ok",
		"#ff841a",
		function(){}
	);
}


Suit_P.gestionDOM.modulos_des = function(){
	gestionModal.alertaConfirmacion(
		primario.aplicacion,
		"El módulo al que desea ingresar está en desarrollo, pronto estará disponible.",
		"warning",
		"Ok",
		"#ff841a",
		function(){}
	);
}

/****************************************************************************************************************/ 

Suit_P.gestionDOM.Update_user_online = function(){

	Suit_P.gestionDOM.update_user_online();
	Login.updateUserOnline = setTimeout(Suit_P.funciones.Update_user_online, 15000);
}

/****************************************************************************************************************/ 
// funciones - comunicaciones con iframe
Suit_P.gestionDOM.pag_lista = function(info){

	$("#iframeApps").css("display","block");
	$("#P-container").css("display","none");
	$(".grid-infoApp").css("display","none");
	$("#logo_OPC .logo-wrapper").attr("src",info[0].url_login_img);

	if(!( /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent))){
		$("#NameViewTopBar").html(`<span>${info[0].titulo_login}</span>`);
	}

	$("#back_APP").css("display","block");
	$(".L_sep").css("display","block");
	$("#volver_suite").css("display","flex");
}

Suit_P.gestionDOM.topbar = function(nameApp, nameMenu){
	var txt = `<span>${nameApp} - ${nameMenu}</span>`;
	$("#NameViewTopBar").html(txt);

	query.callAjax(Suit_P.urlConexion, "updateSession", "", function(){});
}