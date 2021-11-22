$(document).ready(function() {

	$("#logo_OPC .logo-wrapper").attr("src", primario.imagen_principal);

	Topbar.gestionDOM.Menu(Topbar.menu);

	Topbar.gestionDOM.ImgUser();
	
	// background al contenedor cuando se le hace focus al input del buscador
	$(".search-input").off("focus").on("focus",Topbar.gestionDOM.backgroundSearchActive);
	$(".search-input").off("blur").on("blur",Topbar.gestionDOM.backgroundSearchInactive);

	// Generación y limpieza de flecha de etiquetas en los dropdown's del TopBar	
	$(".OpcMenuUser").off("click").on("click",Topbar.gestionDOM.ArrowLowerUser);
	$(".OpcMenuApp").off("click").on("click",Topbar.gestionDOM.ArrowLowerApp);
	$("#Topbar-wrapper #MenuUser").on("hide.bs.dropdown", Topbar.gestionDOM.CleanArrowLowerUser);
	$("#Topbar-wrapper #MenuApp").on("hide.bs.dropdown", Topbar.gestionDOM.CleanArrowLowerApp);

	//Evento que genera la X de cierre cuando se abre el buscador en una pantalla de baja resolución
	$("#Topbar-wrapper .collapse").on('show.bs.collapse',Topbar.gestionDOM.SearchBar);

	//Evento que genera focus  en el input cuando se abre el buscador
	$("#Topbar-wrapper .collapse").on('shown.bs.collapse',Topbar.gestionDOM.FocusSearchBar);

	//evento que desparece la x cuando se cierra el collapse cuando se presiona la x
	$("#Topbar-wrapper .search-icono .close").on("click",Topbar.gestionDOM.SearchBarClean);

	//evento que verifica el cambio en la orientación de la pantalla, verifica si se hace responsive
	// y si es asi cierre la barra de busqueda
	$(window).on("orientationchange", Topbar.gestionDOM.orientationChange);

	// cuando se presione la techa enter en la barra de busqueda, cierre la barra y hace la busqueda
	$("#Topbar-wrapper .search-box .search-input").on("keyup",Topbar.funciones.searchBarEvent);

	// Seleccion de una opción del autocompletar en el searchbar
	$("#autocomplete-result").on("click","li",Topbar.funciones.selectListSearchBar);

	// cuando se le hace click a la lipa en la barra de busqueda, cierre la barra y hace la busqueda
	$("#search-icono").on("click",Topbar.funciones.SearchAndClean);

	// Se se encuentra en un PC de escritorio o un dispositivo con una resolución grande, 
	// cuando se haga click en el contenedor principal se cerrara el contenedor del autocompletar
	// se usa así porque si se esta en un dispositivo movil este mismo evento genera el cierre de la
	// barra lateral
	if(!(/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent))){
		$("#contenedorPrincipal").off('click').on('click',Topbar.gestionDOM.SearchBarClean);
	}

	// marcar la vista actual como favorito
	$("#addFavorite .OpcMenuFavorite").on("click",Topbar.funciones.toggleFavorite);


	// evento de cerrar sesión
	$("#close_APP").on("click",Topbar.gestionDOM.cerrar_sesion);

	// Evento que creara un modal con toda la información del usuario cuando se termine la consulta
	$("#editarPerfil").on("click",Topbar.gestionDOM.editarPerfil);

	// cuando se cancela o se presiona la x el contenedor que tiene el modal se borra de edición de perfil
	$("#ModalEdit").on("click","[data-dismiss='modal']",Topbar.gestionDOM.deleteModal);

	// evento que genera un modal para poder visualizar la foto de usuario
	$("#editarFoto").on("click",Topbar.gestionDOM.editarFoto);

	// cuando se edite la foto de perfil, este evento ejecuta la función 
	// para visualizar la nueva imagen cargada cuando se edita la foto de perfil
	$("#ModalEdit").on("change","#inputFile",Topbar.funciones.URLandPhoto);

	// evento que verfica la carga de la imagen nueva, si no muestra un mensaje de error, se se
	// cargo, actualiza la imagen en el servidor, cambia la url de la imagen y actualiza
	// la imagen en el topbar
	$("#createModalEdit").on("click","#EditPhoto",Topbar.funciones.updatePhoto);

	// evento que ejecuta la funcion de limpiar el mensaje de error en la validación de la carga
	// la foto de perfil
	$("#createModalEdit").on("change","#fileLoad input",Topbar.gestionDOM.cleanModalPhoto);
	
});