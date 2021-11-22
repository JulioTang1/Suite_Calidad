
Topbar.gestionDOM = new Object();

// animación del buscador que se desplaza cuando se abre y se cierra la barra lateral
Topbar.gestionDOM.widthBarOpen = function(){
	// esta funcion limpia la ventana de autocompletar cuando se abre el sidebar
	$("#Topbar-wrapper .search-box").stop();
	// la funcion de retorno es para cerrar el menu de autocompletar, se pone al final de la
	// animación porque si por error el usuario pasa el mouse un momento por el sidebar
	$("#Topbar-wrapper .search-box").animate({"margin-left": '201px'}, 280, "swing", Topbar.gestionDOM.SearchBarClean);
}

Topbar.gestionDOM.widthBarClose = function(){
	$("#Topbar-wrapper .search-box").stop();
	$("#Topbar-wrapper .search-box").animate({"margin-left": '5px'}, 250);
}

// cuando se observa desde un dispositivo movil esta funcion cierra una opción
// cuando se abre otra
Topbar.gestionDOM.closeOpc = function(){
	$("#Topbar-wrapper .collapse.show").collapse("hide");
}


// funcion para configurar el esquema de colores dependiendo el tipo de menu variando
// el background-color y el color en CSS
Topbar.gestionDOM.Menu = function(type){
	if (type == 'dark'){
		Topbar.gestionDOM.colorDark();
		Topbar.gestionDOM.HoverDark();
		Topbar.gestionDOM.RipplerDark();

	}
	else if( type == 'light')
	{
		Topbar.gestionDOM.colorLight();
		Topbar.gestionDOM.HoverLight();
		Topbar.gestionDOM.RipplerLight();
		
	}
}

// se define el color, la velocidad del ripple dependiendo del tipo de menu
Topbar.gestionDOM.RipplerDark = function(){
	var color = '#00000042';
	gestionRippler.rippler('#Topbar-wrapper .navbar-brand', color, 900);
	gestionRippler.rippler('#Topbar-wrapper .searchTopBar', color, 900);
	gestionRippler.rippler('#Topbar-wrapper .OpcTopBar', color, 900);
	gestionRippler.rippler('#Topbar-wrapper .navbar-nav .nav-item .nav-link', color, 800);
}

Topbar.gestionDOM.RipplerLight = function(){
	var color = '#1c202925';
	gestionRippler.rippler('#Topbar-wrapper .navbar-brand', color, 900);
	gestionRippler.rippler('#Topbar-wrapper .searchTopBar', color, 900);
	gestionRippler.rippler('#Topbar-wrapper .OpcTopBar', color, 900);
	gestionRippler.rippler('#Topbar-wrapper .navbar-nav .nav-item .nav-link', color, 800);
}

// funcion para configurar el esquema de colores dependiendo el tipo de menu variando
// el background-color y el color en CSS
Topbar.gestionDOM.colorDark = function(){
	$("#Topbar-wrapper").css("background-color","#2c323f");
	$("#Topbar-wrapper").css("color","white");
}

Topbar.gestionDOM.colorLight = function(){
	$("#Topbar-wrapper").css("background-color","#ffffff");
	$("#Topbar-wrapper").css("color","black");
	$("#Topbar-wrapper .navbar-collapse").css("background-color","#ffffff");
	$("#Topbar-wrapper .navbar-nav .nav-item .nav-link").css("color","black");
}

// hover cuando el menu sea dark o ligth
Topbar.gestionDOM.HoverLight = function(){
	$("#Topbar-wrapper .navbar-nav .nav-item .nav-link").hover(function(){
			$(this).addClass("HoverActiveLight");
		}, function(){
			$(this).removeClass("HoverActiveLight");
	});
	$("#Topbar-wrapper .navbar-brand").hover(function(){
			$(this).addClass("HoverActiveLight");
		}, function(){
			$(this).removeClass("HoverActiveLight");
	});
	$("#Topbar-wrapper .navbar-toggler").hover(function(){
			$(this).addClass("HoverActiveLight");
		}, function(){
			$(this).removeClass("HoverActiveLight");
	});
}
	

Topbar.gestionDOM.HoverDark = function(){
	$("#Topbar-wrapper .navbar-nav .nav-item .nav-link").hover(function(){
			$(this).addClass("HoverActiveDark");
		}, function(){
			$(this).removeClass("HoverActiveDark");
	});
	$("#Topbar-wrapper .navbar-brand").hover(function(){
			$(this).addClass("HoverActiveDark");
		}, function(){
			$(this).removeClass("HoverActiveDark");
	});
	$("#Topbar-wrapper .navbar-toggler").hover(function(){
			$(this).addClass("HoverActiveDark");
		}, function(){
			$(this).removeClass("HoverActiveDark");
	});
}

// background blanco al contenedor cuando se le hace focus al input del buscador y hace
// la consulta de las vistas en el buscador con el contenido que tenga
Topbar.gestionDOM.backgroundSearchActive = function(){
	$(".search-box").addClass("backgroundSearch");

	var search_data = $("#Topbar-wrapper .search-box .search-input").val();
	if(search_data == '')
	{
		$("#autocomplete-result").empty();
		Topbar.searchBarList = -1;
	}
	else
	{
		data = {
			busqueda: search_data,
			Rol: Login.Rol
		};
		// callAjax modificado para hacer dos consultas para la barra de busqueda
		query.callAjaxSearchBar(Topbar.urlConexion,"autocomplete",data,Topbar.comunicaciones.searchBarResult);
	}

}

// background gris al contenedor cuando se le quita el focus al input del buscador
// y cierra la ventana de autocompletar 
Topbar.gestionDOM.backgroundSearchInactive = function(){

	$(".search-box").removeClass("backgroundSearch");
}

// Generación y limpieza de flecha de etiquetas en el dropdown del menu de aplicaciones
Topbar.gestionDOM.ArrowLowerApp = function(){
	$(".ArrowTopApp").css("display","block");
	$(".ArrowTopShadowApp").css("display","block");
}

Topbar.gestionDOM.CleanArrowLowerApp = function() {
	$(".ArrowTopApp").css("display","none");
	$(".ArrowTopShadowApp").css("display","none");
}

// Generación y limpieza de flecha de etiquetas en el dropdown del menu de usuario
Topbar.gestionDOM.ArrowLowerUser = function(){
	$(".ArrowTopUser").css("display","block");
	$(".ArrowTopShadowUser").css("display","block");
}

Topbar.gestionDOM.CleanArrowLowerUser = function() {
	$(".ArrowTopUser").css("display","none");
	$(".ArrowTopShadowUser").css("display","none");
}

//se hace visible la x de cierre en la barra de busqueda
Topbar.gestionDOM.SearchBar = function(){
	$("#Topbar-wrapper .search-icono .close").css("display","inline");
}

// Se hace focus al input de la barra de busqueda
Topbar.gestionDOM.FocusSearchBar = function(){
	$("#Topbar-wrapper .search-input")[0].focus();
}

//Se cierra el collapse de la barra de busqueda y se hace desaparece la X de cierre en
// pantallas de moviles, ademas limpia el search bar y cierra la ventana de autocompletar
Topbar.gestionDOM.SearchBarClean = function(){
	$("#Topbar-wrapper .collapse.show").collapse("hide");
	$(".search-icono .close").css("display","none");
	$("#autocomplete-result").empty();
	$(".search-input").blur();
	Topbar.searchBarList = -1;
}

//cuando se esta en un dispositivo movil y se rota la pantalla, se cierra el buscador
Topbar.gestionDOM.orientationChange = function(){
	if($(window).width() >= 768){
		Topbar.gestionDOM.SearchBarClean();
	}
}

// Funcion que muestra la barra de busqueda con solo conicidencias en las vistas
Topbar.gestionDOM.autocompleteP = function(resultado) {
	var txt = '';
	txt = ` 
		<div class="search-box autocomplete">
			<ul class="list-autocomplete">
				<div class="dropdown-header">Coincidencias Exactas</div>`;
	for(x in resultado)
	{
		txt = `${txt}
				<li>
					<i class="material-icons">${resultado[x].Icono}</i>
					<span data-id="${resultado[x].ID}">
						${resultado[x].Name}
					</span>
				</li>`;
	}
	txt = `${txt}
			</ul>
		</div>`;
	$("#autocomplete-result").html(txt);
}

// Funcion que muestra la barra de busqueda con solo conicidencias en los sinonimos
Topbar.gestionDOM.autocompleteS = function(resultado) {
	var txt = '';
	txt = ` 
		<div class="search-box autocomplete">
			<ul class="list-autocomplete">
				<div class="dropdown-header">Coincidencias Relativas</div>`;
	for(x in resultado)
	{
		txt = `${txt}
				<li>
					<i class="material-icons">${resultado[x].Icono}</i>
					<span data-id="${resultado[x].ID}">
						${resultado[x].Name}
					</span>
				</li>`;
	}
	txt = `${txt}
			</ul>
		</div>`;
	$("#autocomplete-result").html(txt);
}

// Funcion que muestra la barra de busqueda con conicidencias en las vistas y
// en los sinonimos
Topbar.gestionDOM.autocompleteFull = function(resultado_P,resultado_S) {
	var txt = '';
	txt = ` 
		<div class="search-box autocomplete">
			<ul class="list-autocomplete">
				<div class="dropdown-header">Coincidencias Exactas</div>`;
	for(x in resultado_P)
	{
		txt = `${txt}
				<li>
					<i class="material-icons">${resultado_P[x].Icono}</i>
					<span data-id="${resultado_P[x].ID}">
						${resultado_P[x].Name}
					</span>
				</li>`;
	}
	txt = `${txt}
				<div class="dropdown-divider"></div>
				<div class="dropdown-header">Coincidencias Relativas</div>`;
	for(x in resultado_S)
	{
		txt = `${txt}
				<li>
					<i class="material-icons">${resultado_S[x].Icono}</i>
					<span data-id="${resultado_S[x].ID}">
						${resultado_S[x].Name}
					</span>
				</li>`;
	}
	txt = `${txt}
			</ul>
		</div>`;
	$("#autocomplete-result").html(txt);
}

// Lista del autocompletar, funciones para movimientos con el teclado
// flecha abajo
Topbar.gestionDOM.ListDown = function(){
	// verificar la posición de la lista en la que se encuentra
	// si esta en la ultima posición y se le da flecha abajo, vuelve a la primera posición
	if(Topbar.searchBarList >= ($("#autocomplete-result li").length-1)){
		Topbar.searchBarList = 0;
	}
	else
	{
		Topbar.searchBarList++;
	}
	Topbar.gestionDOM.ListEffects();
}

// flecha arriba
Topbar.gestionDOM.ListTop = function(){
	// verificar la posición de la lista en la que se encuentra
	// si esta en la primera posición y se le da flecha arriba, vuelve a la ultima posicion
	if(Topbar.searchBarList <= 0){
		Topbar.searchBarList = $("#autocomplete-result li").length-1;
	}
	else
	{
		Topbar.searchBarList--;	
	}
	Topbar.gestionDOM.ListEffects();
}

// Esta funcion agrega la clase al elemento donde se encuentre el select, toma el valor
// del string del span y se lo pone al input
Topbar.gestionDOM.ListEffects = function(){
	$(".autocomplete_keyboard").removeClass("autocomplete_keyboard");
	$($("#autocomplete-result li")[Topbar.searchBarList]).addClass("autocomplete_keyboard");

	var id 		= $("#autocomplete-result li span")[Topbar.searchBarList].attributes["data-id"].nodeValue;
	var value 	= $("#autocomplete-result li span")[Topbar.searchBarList].innerText;

	$("#Topbar-wrapper .search-input").val(value);
	$("#Topbar-wrapper .search-input").data('id',id);
}

// Funcion que cambiara la vista del contenedor principal
Topbar.gestionDOM.busqueda = function(){

	var view_ID = $("#Topbar-wrapper .search-input").data('id');
	// con el id de la vista se sabe si puede ver o editar
	var viewID = parseInt(view_ID);
	var access = Sidebar.Acceso[Sidebar.menus_ID.indexOf(viewID)];

	// nivel de edición
	if (access == 2)
	{
		Sidebar.crud_access = 1;
	}
	// nivel de visualización
	else if (access == 1)
	{
		Sidebar.crud_access = 0;
	}
	// cerrar popover cuando abre una vista si es que esta abierto
	$(".popover").popover("dispose");

	// se quitan los background del sidebar cuando se busca la vista por el menu
	// y se le da click a la vista para que se le apliquen los estilos al sidebar
	$(".desplegableLight").removeClass("desplegableLight");
	$(".desplegableDark").removeClass("desplegableDark");
	$(`#menu_${view_ID}`).click();

	// como el evento click abre el sidebar lo abre, por eso se pone a ejecutar la funcion
	// de cerrarlo, es muy rapido y no se puede notar ningún cambio, se usa la función click
	// para que se asignen los estilos en la opción del sidebar que se busco en el buscador
	Sidebar.gestionDOM.CloseSidebar();
}

// se cambia el icono de favoritos
Topbar.gestionDOM.addFavorite = function(ev){
	var txt = '';
	if(ev.currentTarget.outerText == "star_border"){
		txt = `	<i class="material-icons" style="color: #007bff;">star</i>`;
	}
	else if(ev.currentTarget.outerText == "star"){
		txt = `	<i class="material-icons">star_border</i>`;
	}
	$("#addFavorite .OpcMenuFavorite .navbar-brand").empty();
	$("#addFavorite .OpcMenuFavorite .navbar-brand").html(txt);
}

// función que carga la imagen del usuario en la parte derecha del topbar
Topbar.gestionDOM.ImgUser = function(){
	if(Login.urlImgTopBar != "")
	{
		query.callAjaxImg(Login.urlImgTopBar,Topbar.gestionDOM.successImg, Topbar.gestionDOM.errorImg);
	}
	$("#TopName span").html(Login.nombre_usuario);
}

Topbar.gestionDOM.successImg = function(){
	var	txt = `<img draggable="false" src="${Login.urlImgTopBar}" alt="Img_User">`;
	$("#MenuUser .OpcMenuUser a").empty();
	$("#MenuUser .OpcMenuUser a").html(txt);
}

Topbar.gestionDOM.errorImg = function(){
	var	txt = `<img draggable="false" src="vistas/Topbar/imagenes/user.png" alt="Img_User">`;
	$("#MenuUser .OpcMenuUser a").empty();
	$("#MenuUser .OpcMenuUser a").html(txt);
}

// Generación del Modal para el registro de los datos para solicitar acceso
Topbar.gestionDOM.editarPerfil = function(resultado){
	txt = `
		<div class="modal-content">

		<!-- Modal Header -->
			<div class="modal-header">
				<h4 class="modal-title">Perfil</h4>
				<button type="button" class="close" data-dismiss="modal">&times;</button>
			</div>

			<!-- Modal body -->
			<div class="modal-body">

				<div class="row">
					<div class="col-md-12">
						<div class="div-input-md">
							<input id="UserReg" type="text" autocomplete="off" required disabled>
							<label>Usuario</label>
						</div> 
					</div>
					<div class="col-md-6">
						<div class="div-input-md">
							<input id="nombres" type="text" autocomplete="off" required disabled>
							<label>Nombre</label>
						</div> 
					</div>
					<div class="col-md-6">
						<div class="div-input-md">
							<input id="apellidos" type="text" autocomplete="off" required disabled>
							<label>Apellido</label>
						</div> 
					</div>
					<div class="col-md-12">
						<div class="div-input-md">
							<input id="nombre_usuario" type="text" autocomplete="off" required disabled>
							<label>Nombre completo</label>
						</div> 
					</div>
					<div class="col-md-12">
						<div class="div-input-md">
							<input id="email" type="text" autocomplete="off" required disabled>
							<label>Correo electrónico</label>
						</div> 
					</div>
				</div>

			</div>

		</div>`;
	$("#createModalEdit").html(txt);

	// se cargan los valores en el modal
	$("#UserReg").val(Login.User);
	$("#email").val(Login.Email);
	$("#nombres").val(Login.Name);
	$("#apellidos").val(Login.lastName);
	$("#nombre_usuario").val(Login.nombre_usuario);

	$("#ModalEdit").modal({backdrop: "static"});
	gestionModal.cerrar();
	
}

// función que muestra un modal indicando que se modificarón los datos
// satisfactoriamente
Topbar.gestionDOM.editResponse = function(){
	gestionModal.alertaConfirmacion(
		primario.aplicacion,
		"Sus datos han sido modificados satisfactoriamente",
		"success",
		"Ok",
		"#28a745",
		Topbar.gestionDOM.deleteModal
	);
	$("#TopName span").html(Topbar.nombre);
	$("#TopCargo span").html(Topbar.cargo);
}


// Generación del Modal para el cambio de foto de perfil
Topbar.gestionDOM.editarFoto = function(resultado){
	txt = `
		<div class="modal-content">

		<!-- Modal Header -->
			<div class="modal-header">
				<h4 class="modal-title">Editar foto de perfil</h4>
				<button type="button" class="close" data-dismiss="modal">&times;</button>
			</div>

			<!-- Modal body -->
			<div class="modal-body">
				
				<div class="col-md-12">
					<img draggable="false" src="${Login.urlImgTopBar}" id="profile-img-tag"/>
				</div>

				<div class="col-md-12">
					<div class="cargarFoto">
						<form id="fileLoad" enctype ="multipart/form-data" method="POST">
							<input type="file" class="" name="File" id="inputFile"
							accept="image/*, image/heic, image/heif" hidden>
							<label id="subirFoto" for="inputFile"> 
								<i class="material-icons">add_a_photo</i>
								<span>Subir foto</span>
							</label>
						</form>
						<div class="msnError"></div>
					</div>
				</div>

			</div>
			<!-- Modal footer -->
			<div class="modal-footer">
				<button type="button" class="btn btn-success" id="EditPhoto">
					Confirmar
				</button>
				<button type="button" class="btn btn-danger" data-dismiss="modal">Cancelar</button>
			</div>

		</div>`;
	$("#createModalEdit").html(txt);

	$("#ModalEdit").modal({backdrop: "static"});
	
}

// funcion para limpiar el mensaje de error cuando se cargue una foto en el modal
Topbar.gestionDOM.cleanModalPhoto = function(){
	$(".errorInput").removeClass("errorInput");
	$(".msnError").empty();
}

// funcion que cambia el estilo del boton de carga cuando hay una una imagen cargada
// y cuando no se carga una imagen, se visualiza la que ya existe
Topbar.gestionDOM.loadPhoto = function(){
	if($("#fileLoad input").val() != ''){
		$("#subirFoto").addClass("fotoSeleccionada");
		$("#subirFoto span").html("Foto cargada");
		$("#subirFoto i").html("done_all");
	}
	else
	{
		$('#profile-img-tag').attr('src', Login.urlImgTopBar);
		$("#subirFoto").removeClass("fotoSeleccionada");	
		$("#subirFoto span").html("Subir foto");
		$("#subirFoto i").html("add_a_photo");
	}
}

// modal de confirmación sobre la actualización de datos del usuario
Topbar.gestionDOM.RegCorrecto = function(data){
	gestionModal.alertaConfirmacion(
		primario.aplicacion,
		"Su foto de perfil ha sido actualizada correctamente",
		"success",
		"Ok",
		"#28a745",
		Topbar.gestionDOM.deleteModal
	);
	Login.urlImgTopBar = data[0].url_foto;

	var txt = `<img draggable="false" src="${Login.urlImgTopBar}">`;
	$("#MenuUser .OpcMenuUser a").html(txt);
}

// funcion que cierra el modal y borra el modal, con fines de segurida para que no
// envíe información al servidor sin la autorización
Topbar.gestionDOM.deleteModal = function(){
	$("#ModalEdit").modal("hide");
	$("#createModalEdit").empty();
}

// se dibuja el menu de favoritos cuando se inicia sesion y cuando se agrega y quita menus de favoritos
Topbar.gestionDOM.menuFavoritos = function(resultado){
	// Se limpian los eventos cuando se redibuje el menu de opciones cuando agregue y quite favoritos
	$("#menuFav").empty();
	$("#menuFav").off();
	var id_menu;
	var txt = `	<div style="font-weight: bold; text-align: center;">
					Mis favoritos
				</div>
				<div id="grid">`;
	for (x in resultado){
		txt = `${txt}
					<div class="item-application" id="menuFav_${resultado[x].id_menu}"
					title="${resultado[x].nombre_menu}"
					data-toolFav="tooltip">
						<i class="material-icons icono-application">${resultado[x].icono}</i>
					</div>`;
		id_menu = {
			id_vista: 	`${resultado[x].id_menu}`
		};
		$("#menuFav").on("click",`#menuFav_${resultado[x].id_menu}`,id_menu,Topbar.gestionDOM.loadViewFav);
	}
	txt = `${txt} 
				</div>`;
	$("#menuFav").html(txt);

	// comparador que cambia el overflow-y cuando son menos de 3 favoritos, 
	// quitandolo para que el tooltip se vea bien
	Topbar.gestionDOM.overflowFav();

	//función para crear listas y grillas clasificables utilizando la API nativa de arrastrar y soltar
	// y evento cuando sucede un evento de la cuadricula.
	$('#grid').sortable().bind('sortupdate', Topbar.funciones.ordenMenuFav);

	// mostrar tooltip cuando es escritorio, para no mostrarlo en el celular
	if(!( /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent))){
		$(`#MenuApp [data-toolFav="tooltip"]`).tooltip({delay: {show: 500, hide: 0}, trigger: 'hover'});
	}
}

// comparador que cambia el overflow-y cuando son menos de 3 favoritos, 
// quitandolo para que el tooltip se vea bien
Topbar.gestionDOM.overflowFav = function(){
   	if($("#grid div").length > 3)
   	{
   		$("#grid").css("overflow-y","auto");
   	}
   	else
   	{
   		$("#grid").css("overflow-y","initial");	
   	}
}

Topbar.gestionDOM.menuSinFavoritos = function(){
	$("#menuFav").empty();
	var txt = ` <span style="vertical-align: middle; font-weight: bold;">
					No hay favoritos
				</span>`;
	$("#menuFav").html(txt);
	$("#menuFav").css({"text-align": "center"});
}

Topbar.gestionDOM.loadViewFav = function(ev){
	var viewID = parseInt(ev.data.id_vista);
	var access = Sidebar.Acceso[Sidebar.menus_ID.indexOf(viewID)];

	// nivel de edición
	if (access == 2)
	{
		Sidebar.crud_access = 1;
	}
	// nivel de visualización
	else if (access == 1)
	{
		Sidebar.crud_access = 0;
	}

	// se quitan los background del sidebar cuando se busca la vista por el menu
	// y se le da click a la vista para que se le apliquen los estilos al sidebar
	$(".desplegableLight").removeClass("desplegableLight");
	$(".desplegableDark").removeClass("desplegableDark");
	$(`#menu_${viewID}`).click();

	// como el evento click abre el sidebar lo abre, por eso se pone a ejecutar la funcion
	// de cerrarlo, es muy rapido y no se puede notar ningún cambio, se usa la función click
	// para que se asignen los estilos en la opción del sidebar que se busco en el menu
	// de favoritos
	Sidebar.gestionDOM.CloseSidebar();
}

// funciones que despues de la consulta si el menu es favorito o no, pone la estrella o solo el borde
Topbar.gestionDOM.ViewFav = function(){
	// variable que indica si la vista actual pertenece a favoritos o no
	// 1 --> favoritos, 0 --> No es de favoritos
	Topbar.viewFav = 1;
	var txt = `	<i class="material-icons" style="color: #007bff;">star</i>`;
	$("#addFavorite .OpcMenuFavorite .navbar-brand").empty();
	$("#addFavorite .OpcMenuFavorite .navbar-brand").html(txt);
}

Topbar.gestionDOM.ViewNotFav = function(){
	// variable que indica si la vista actual pertenece a favoritos o no
	// 1 --> favoritos, 0 --> No es de favoritos
	Topbar.viewFav = 0;
	var txt = `	<i class="material-icons">star_border</i>`;
	$("#addFavorite .OpcMenuFavorite .navbar-brand").empty();
	$("#addFavorite .OpcMenuFavorite .navbar-brand").html(txt);
}

Topbar.gestionDOM.NombreVistaTopBar = function(nombre){
	var txt = `<span>${nombre}</span>`;
	$("#NameViewTopBar").html(txt);
}

// funcion que cierra sesión, borra los objetos en el sidebar, topbar y Login
// que sin ellas no funcionaran las vistas, se vacían los contenedores y se 
// carga nuevamente el login
Topbar.gestionDOM.cerrar_sesion = function(){

	clearTimeout(Login.updateUserOnline);
	delete Sidebar;
	delete Topbar;
	delete Login;
	$("#Sidebar").empty();
	$("#Topbar").empty();
	$("#Login").empty();
	$("#contenedorPrincipal").empty();

	gestionModal.alertaBloqueante(primario.aplicacion, "Procesando...");
	query.callAjax(primario.urlConexion,"closeSession",'',Topbar.gestionDOM.loadLogin);
}

Topbar.gestionDOM.loadLogin = function(){
	$("#Login").load("vistas/Login/Login.html");
}