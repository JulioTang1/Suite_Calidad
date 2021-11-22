Sidebar.gestionDOM = new Object();

// si es un dispositivo movil, estas funciones abren y cierran el sidebar y habilitan y deshabilita los eventos
// de ella, con el fin de evitar que cuando se abra se seleccione una opción

// funcion que abre y cierra el sidebar con deslizar el menu
Sidebar.gestionDOM.TouchToggle = function(){
	if(Sidebar.status == 1){
		Sidebar.gestionDOM.CloseSidebar();
		$("#accordion").addClass("disableEvents");
	}
	else if(Sidebar.status == 0){
		Sidebar.gestionDOM.OpenSidebar();
	}
}

Sidebar.gestionDOM.CloseSidebarTouch =  function(){
	if(Sidebar.status == 1){
		Sidebar.gestionDOM.CloseSidebar();
		$("#accordion").addClass("disableEvents");
	}
}

Sidebar.gestionDOM.OpenSidebarTouch = function(){
	if(Sidebar.status == 0){
		Sidebar.gestionDOM.OpenSidebar();
	}
}

// Abertura y cierre de la barra lateral haciendo Dash en pantallas de dispositivos
// moviles
Sidebar.gestionDOM.DashToggle = function(e){
    var swipe = e.originalEvent.touches,
    start = swipe[0].pageX;

    $("#sidebar-wrapper").on('touchmove', function(e) {

        var contact = e.originalEvent.touches,
        end = contact[0].pageX,
        distance = end-start;

        if (distance < -100 && Sidebar.status == 1){
        	Sidebar.gestionDOM.CloseSidebarTouch();
        }
        else if (distance > 100 && Sidebar.status == 0){
        	Sidebar.gestionDOM.OpenSidebar();
        }
	}).one('touchend', function() {
    	$(this).off('touchmove touchend');
	});
}

// Funciones para abrir y cerrar el sidebar
Sidebar.gestionDOM.OpenSidebar = function(){
	// si la barra esta abierta esta no volvera a abrir
	if(!Sidebar.status){

		// Estado de la barra, Abierto
		Sidebar.status = 1;

		Sidebar.gestionDOM.closeBoostrapSelect();

		// metodo para cerrar los collapse del Topbar
		// Topbar.gestionDOM.closeOpc();

		Sidebar.gestionDOM.animationSelected();
		Sidebar.gestionDOM.changeMarginPlus();
		Sidebar.gestionDOM.animationOpen();
		Sidebar.gestionDOM.visibleText();

		// esta función cambia la margen del buscador del Topbar
		// Topbar.gestionDOM.widthBarOpen();
	}
}

// funcion que cierra los selectores de boostrap select si se encuentran
// abiertos cuando se abre el sidebar
Sidebar.gestionDOM.closeBoostrapSelect = function(){
	$('.selectpicker').trigger('click.bs.dropdown.data-api');
}

Sidebar.gestionDOM.CloseSidebar = function(){
	// si la barra esta cerrada esta no volvera a cerrar
	if(Sidebar.status){
		// Estado de la barra, Cerrado
		Sidebar.status = 0;

		Sidebar.gestionDOM.invisibleText();
		Sidebar.gestionDOM.ArrowClose();
		Sidebar.gestionDOM.animationClose();
		Sidebar.gestionDOM.changeMarginLess();
		Sidebar.gestionDOM.closeTooltip();

		// esta función cambia la margen del buscador del Topbar
		// Topbar.gestionDOM.widthBarClose();
	}
}

// funcion para cerrar los tooltip, por si quedan abiertos
Sidebar.gestionDOM.closeTooltip = function(){
	$('.tooltip').tooltip('hide');
}

//Se cambia el tamaño de la margen cuando se abre y se cierra el sidebar
// para que cuando se seleccione un elemento el icono se vea centrado en 
// el gradiante
Sidebar.gestionDOM.changeMarginPlus = function(){
	$(".titulosOpc").css("margin-right","16px");
}
Sidebar.gestionDOM.changeMarginLess = function(){
	$(".titulosOpc").css("margin-right","7px");
}


// Animacion de abertura en el sidebar
Sidebar.gestionDOM.animationOpen = function(){
	$("#sidebar-wrapper").stop();
	$("#sidebar-wrapper").animate({width: '260px'}, 280, "swing", Sidebar.gestionDOM.visibleArrow);
}

// cuando se termina la animación de abrir el sidebar se pueda visualizar las flechas
// no se remueve la clase al comienzo de la animación porque se verían las flechas moviendose
Sidebar.gestionDOM.visibleArrow = function(){
	$(".icono-animado").removeClass("desactive");
	// Cuando se esta trabajando en una pantalla tactil, este evento habilita los eventos de la barra
	// lateral pero solo hasta que abra por completo, esto evita seleccionar algo miestras la barra
	// esta cerrada
	$("#accordion").removeClass("disableEvents");
}

// Animacion de cierre en el sidebar
Sidebar.gestionDOM.animationClose = function(){
	$("#sidebar-wrapper").stop();
	$("#sidebar-wrapper").animate({width: '64px'}, 250 , "swing", Sidebar.gestionDOM.collapseClose);
}

//cierre de los menus cuando termine la animación del sidebar
Sidebar.gestionDOM.collapseClose = function() {
	$("#sidebar-wrapper .collapse.show").collapse('hide');
}

// se muestran los textos y se quitan el icono de puntos cuando la barra se expande
Sidebar.gestionDOM.visibleText = function(){
	$("#sidebar-wrapper span").css("visibility","visible");
	$("#sidebar-wrapper #accordion .puntos").css("display","none");
}

// se quitan los textos y se muestran el icono de puntos cuando la barra se contrae
Sidebar.gestionDOM.invisibleText = function(){
	$("#sidebar-wrapper span").css("visibility","hidden");
	$("#sidebar-wrapper #accordion .puntos").css("display","inline");
	$(".icono-animado").addClass("desactive");
}

// cuando se selecciona una opción principal se pone un background-color en todo
// el los hijos de este
Sidebar.gestionDOM.backgroundSelected = function(e){
	if (Sidebar.menu == 'light'){
		$(".desplegableLight").removeClass("desplegableLight");
		$(e.currentTarget.parentNode).addClass("desplegableLight");
	}
	else
	{
		$(".desplegableDark").removeClass("desplegableDark");
		$(e.currentTarget.parentNode).addClass("desplegableDark");
	}
	// flagAnimation estara en 1 si se encuentra la animación de collapse en curso, y cuando se termine
	// pasara a estar en 0
	if(Sidebar.flagAnimation == 0){
		Sidebar.gestionDOM.animationArrowPrincipal(e);
		Sidebar.flagAnimation = 1;
	}
}

Sidebar.gestionDOM.flagAnimation = function(){
	Sidebar.flagAnimation = 0;
}

// funcion para la animacion de las flechas de las opciones principales
Sidebar.gestionDOM.animationArrowPrincipal = function(e){
	var flagActive = $(".icono-animado.active").length;
	$(".icono-animado.active").removeClass("active");
	
	Sidebar.gestionDOM.closeSubAccordion();

	if((flagActive == 0)||(Sidebar.flagPosition!=e.currentTarget)){
		$(e.currentTarget).addClass("active");
		Sidebar.flagPosition = e.currentTarget;
	}
	else
	{
		Sidebar.flagPosition = e.currentTarget;
	}
}

//mio
// funcion para cerrar todos los sub-acordeones, en esta función se ingresan las funciones los
// metodos de collapse con los data-parent respectivos
Sidebar.gestionDOM.closeSubAccordion =  function(){
	// $("[data-parent='#SubAccordion1'].collapse").collapse("hide");
	// $("[data-parent='#SubAccordion2'].collapse").collapse("hide");
	// $("[data-parent='#SubAccordion3'].collapse").collapse("hide");
	// $("[data-parent='#SubAccordion4'].collapse").collapse("hide");
}

// background-color para el sub-acordeon
Sidebar.gestionDOM.OpcionSelected = function(e){
	if (Sidebar.menu == 'light'){
		$(".subDesplegableLight").removeClass("subDesplegableLight");
		$(e.currentTarget.parentNode).addClass("subDesplegableLight");
	}
	else
	{
		$(".subDesplegableDark").removeClass("subDesplegableDark");
		$(e.currentTarget.parentNode).addClass("subDesplegableDark");
	}
}

// funcion para la animacion de las flechas de las opciones secundarias
Sidebar.gestionDOM.animationArrowActiveSecondary = function(e){
	var id = e.currentTarget.id;
	$(`[href = '#${id}']`).addClass("active");
}

Sidebar.gestionDOM.animationArrowDesactiveSecondary = function(e){
	var id = e.currentTarget.id;
	$(`[href = '#${id}']`).removeClass("active");
}


// funcion para que las flechas vuelvan a apuntar a la derecha cuando el sidebar cierre
Sidebar.gestionDOM.ArrowClose = function(){
	$(".icono-animado").removeClass("active");
}

Sidebar.gestionDOM.animationSelected = function(){
	if (Sidebar.open == 0) 
	{	
		// se define el color, la velocidad del ripple y se agrega el hover
		// dependiendo del tipo de menu

		if(Sidebar.menu == 'light'){
			Sidebar.gestionDOM.HoverLight();
			var color = '#1c202925';
		}
		else
		{
			Sidebar.gestionDOM.HoverDark();
			var color = '#00000042';
		}
		var speed = 500;

		// gestionRippler.rippler = function(element, color, speed) //
		gestionRippler.rippler('.titulosOpc', color, speed);
		gestionRippler.rippler('.titulosOpcSec', color, speed);

		Sidebar.open = 1;
	}
}

// cuando se selecciona una opción, esta función agrega un gradiente y un borde lateral dependiendo el caso
Sidebar.gestionDOM.optionSelected = function(e){

	$(".borde_left_principal").removeClass("borde_left_principal");
	$(".borde_left_secondary").removeClass("borde_left_secondary");

	if($(e.currentTarget).parents(".collapse").siblings(".titulosOpcSec").length){
		$(e.currentTarget).parents(".collapse").siblings(".titulosOpcSec").addClass("borde_left_secondary");
	}

	if($(e.currentTarget).parents(".collapse").siblings(".titulosOpc").length){
		$(e.currentTarget).parents(".collapse").siblings(".titulosOpc").addClass("borde_left_principal");
	}

	$(".item_selected").removeClass("item_selected");
	$(e.currentTarget).addClass("item_selected");
}

// funcion para configurar el esquema de colores dependiendo el tipo de menu variando
// el background-color y el color en CSS
Sidebar.gestionDOM.Menu = function(type){
	if (type == 'dark'){
		$("#sidebar-wrapper").css("background-color","#2c323f");
		$("#sidebar-wrapper").css("color","white");
	}
	else if( type == 'light')
	{
		$("#sidebar-wrapper").css("background-color","#ffffff");
		$("#sidebar-wrapper").css("color","black");
	}
}

// hover cuando el menu sea dark o ligth
Sidebar.gestionDOM.HoverDark = function(){
	$("#sidebar-wrapper #accordion .card .card-body li").hover(function(){
			$(this).addClass("HoverActiveDark");
		}, function(){
			$(this).removeClass("HoverActiveDark");
	});
	$(".titulosOpcSec").hover(function(){
			$(this).addClass("HoverActiveDark");
		}, function(){
			$(this).removeClass("HoverActiveDark");
	});
	$(".titulosOpc").hover(function(){
			$(this).addClass("HoverActiveDark");
		}, function(){
			$(this).removeClass("HoverActiveDark");
	});
}

Sidebar.gestionDOM.HoverLight = function(){
	$("#sidebar-wrapper #accordion .card .card-body li").hover(function(){
			$(this).addClass("HoverActiveLight");
		}, function(){
			$(this).removeClass("HoverActiveLight");
	});
	$(".titulosOpcSec").hover(function(){
			$(this).addClass("HoverActiveLight");
		}, function(){
			$(this).removeClass("HoverActiveLight");
	});
	$(".titulosOpc").hover(function(){
			$(this).addClass("HoverActiveLight");
		}, function(){
			$(this).removeClass("HoverActiveLight");
	});
}

// funcion para añadir el scrollbar
Sidebar.gestionDOM.scrollBar = function(){
	new PerfectScrollbar('#scrollbar',{
      suppressScrollX: true
    });
}


// funcion que modifica la imagen de perfil
Sidebar.gestionDOM.ImageAndTitle = function(){

	var txt1 = ` <img draggable="false" class="size_width" src="${Login.urlImgSideBar}" alt="${Login.tituloSidebar} Logo">`;
	var txt2 = ` <span class="logo-text">${Login.tituloSidebar}</span>`;

    $("#imagenSidebar").html(txt1);
    $("#tituloSidebar").html(txt2);
}

Sidebar.gestionDOM.errorConexion = function(){
	gestionModal.alertaConfirmacion(
		primario.aplicacion,
		"No posee acceso a ningún menu, por favor comuniquese con el administrador",
		"error",
		"Ok",
		"#f27474",
		function(){}
	);
}


// generación del Sidebar dinamico
Sidebar.gestionDOM.SidebarDynamic = function(data){

	resultado_sidebar 	= data.SIDEBAR;
	resultado_header 	= data.HEADER;
	resultado_one_level = data.ONE_LEVEL;
	resultado_two_levels = data.TWO_LEVELS;

	var dataConexion;
	var txt = '';
	// arreglos para saber la cantidad de header que hay en el sidebar
	headerId 	= new Array();

	for (x in resultado_sidebar)
	{
		headerId[x] = resultado_sidebar[x].ID_header;
	}
	// se quitan los valores repetidos
	headerId = headerId.filter((valor, indiceActual, arreglo) => arreglo.indexOf(valor) === indiceActual);

	txt = `<!-- la clase puntos es necesaria para que cuando se abra el sidebar este elemento desaparezca y viceversa-->`
	for (x in resultado_header)
	{
		for (y in headerId)
		{
			if(resultado_header[x].ID_header == headerId[y]){
				txt = ` ${txt}   
				<h2 class="tituloP">
		            <i class="material-icons md-22 puntos">more_horiz</i>
		            <span>${resultado_header[x].header}</span>
		        </h2>
		        <div id="acordeon_${resultado_header[x].ID_header}"></div>`;
	        }
    	}
	}
	$("#accordion").html(txt);

	OneLevel 	= new Array();
	TwoLevels 	= new Array();


	// Agregar html para el primer nivel del menu del sidebar
	// son los primeros niveles sin y con submenus
	// OneLevel es sin submenus, twolevels tiene submenus 

	for (x in resultado_one_level)
	{
		OneLevel[x] = resultado_one_level[x].ID_primer_nivel;
	}

	for (x in resultado_two_levels)
	{
		TwoLevels[x] = resultado_two_levels[x].ID_primer_nivel;
	}
	TwoLevels = TwoLevels.filter((valor, indiceActual, arreglo) => arreglo.indexOf(valor) === indiceActual);

	primerNivelSelect   = new Array();
	primerNivelCollapse = new Array();

	primerNivelSelect 	= OneLevel;
	primerNivelCollapse = TwoLevels;
	primerNivelCollapse = primerNivelCollapse.filter((valor, indiceActual, arreglo) => arreglo.indexOf(valor) === indiceActual);

	// for para generar el html con sus clases y eventos para los elementos del primer nivel del sidebar
	for(y in headerId)
	{
		txt = '';
		for (x in resultado_sidebar)
		{
			// opciones de primer nivel con collapse que agrega las clases para la animación de las flechas
			if((resultado_sidebar[x].ID_header == headerId[y]) && ($.inArray(resultado_sidebar[x].ID_menu, primerNivelCollapse) !== -1))
			{
				if(resultado_sidebar[x].es_vista){
					txt =` ${txt}
					<div class="card">
				  		<div class="titulosOpc icono-animado desactive" data-toggle="collapse" data-tool="tooltip" 
				  		href="#collapse${resultado_sidebar[x].ID_menu}" id="menu_${resultado_sidebar[x].ID_menu}"
				  		title="${resultado_sidebar[x].tooltip}" data-placement="right">
				      		<i class="material-icons md-22 iconoP">${resultado_sidebar[x].icono}</i>
				      		<span>${resultado_sidebar[x].nombre_sidebar}</span>
				  		</div>
				  		<div id="collapse${resultado_sidebar[x].ID_menu}" class="collapse" data-parent="#accordion"></div>
				  	</div>`;
				  	dataConexion = {
				  			url: 		`${resultado_sidebar[x].url_vista}`,
				  			id_vista: 	`${resultado_sidebar[x].ID_menu}`,
				  			nombre: 	`${resultado_sidebar[x].nombre_sidebar}`
				  	};
				  	$(`#acordeon_${headerId[y]}`).on("click",`#menu_${resultado_sidebar[x].ID_menu}`,dataConexion, Sidebar.gestionDOM.loadView);
			  	}
			  	else
			  	{
			  		txt =` ${txt}
					<div class="card">
				  		<div class="titulosOpc icono-animado desactive" data-toggle="collapse" data-tool="tooltip" 
				  		href="#collapse${resultado_sidebar[x].ID_menu}" id="menu_${resultado_sidebar[x].ID_menu}"
				  		title="${resultado_sidebar[x].tooltip}" data-placement="right">
				      		<i class="material-icons md-22 iconoP">${resultado_sidebar[x].icono}</i>
				      		<span>${resultado_sidebar[x].nombre_sidebar}</span>
				  		</div>
				  		<div id="collapse${resultado_sidebar[x].ID_menu}" class="collapse" data-parent="#accordion"></div>
				  	</div>`;
			  	}
		  	}
		  	// opciones de primer nivel sin collapse que agrega las clases del gradiente para la selección
		  	else if((resultado_sidebar[x].ID_header == headerId[y]) && ($.inArray(resultado_sidebar[x].ID_menu, primerNivelSelect) !== -1))
			{
				txt =` ${txt}
				<div class="card">
			  		<div class="titulosOpc picked" data-toggle="collapse" data-tool="tooltip"
			  		href="#collapse${resultado_sidebar[x].ID_menu}" id="menu_${resultado_sidebar[x].ID_menu}" 
			  		title="${resultado_sidebar[x].tooltip}" data-placement="right">
			      		<i class="material-icons md-22 iconoP">${resultado_sidebar[x].icono}</i>
			      		<span>${resultado_sidebar[x].nombre_sidebar}</span>
			  		</div>
			  		<div id="collapse${resultado_sidebar[x].ID_menu}" class="collapse" data-parent="#accordion"></div>
				</div>`;
			  	dataConexion = {
			  			url: 		`${resultado_sidebar[x].url_vista}`,
			  			id_vista: 	`${resultado_sidebar[x].ID_menu}`,
			  			nombre: 	`${resultado_sidebar[x].nombre_sidebar}`
			  	};
	  			$(`#acordeon_${headerId[y]}`).on("click",`#menu_${resultado_sidebar[x].ID_menu}`,dataConexion, Sidebar.gestionDOM.loadView);
		  	}
		}
		$(`#acordeon_${headerId[y]}`).html(txt);
	}
	
	TwoLevelsSelect = new Array();
	TwoLevelsSelect = TwoLevels;

	// for para generar el html con sus clases y eventos para los elementos del segundo nivel del sidebar sin collapse
	for(x in TwoLevelsSelect)
	{
		txt = '	<div class="card-body">';
		for (y in resultado_sidebar)
		{
			for (z in resultado_two_levels)
			{
				if((resultado_sidebar[y].ID_menu == resultado_two_levels[z].ID_segundo_nivel) && (resultado_two_levels[z].ID_primer_nivel == TwoLevelsSelect[x]))
				{
					txt =` ${txt}
					<li class="picked" id="menu_${resultado_sidebar[y].ID_menu}" data-tool="tooltip"
					title="${resultado_sidebar[y].tooltip}" data-placement="right">
						<i class="material-icons md-14 iconoT">${resultado_sidebar[y].icono}</i>
						<span>${resultado_sidebar[y].nombre_sidebar}</span>
					</li>`;
				  	dataConexion = {
				  			url: 		`${resultado_sidebar[y].url_vista}`,
				  			id_vista: 	`${resultado_sidebar[y].ID_menu}`,
				  			nombre: 	`${resultado_sidebar[y].nombre_sidebar}`
				  	};
					$(`#collapse${TwoLevelsSelect[x]}`).on("click",`#menu_${resultado_sidebar[y].ID_menu}`,dataConexion, Sidebar.gestionDOM.loadView);
				}
			}
		}
		$(`#collapse${TwoLevelsSelect[x]}`).html(txt);
	}
	
	// mostrar tooltip cuando es escritorio, para no mostrarlo en el celular
	if(!( /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent))){
		$(`#accordion [data-tool="tooltip"]`).tooltip({delay: {show: 500, hide: 0}, trigger: 'hover'});
	}

	var message = {
		mensaje: "Carga_Modulo"
	};
	primario.window_source.postMessage(message,primario.window_origin);

}

// funcion que carga la vista y verifica que nivel de acceso tiene el rol en esa vista
Sidebar.gestionDOM.loadView = function(ev){

	Sidebar.menuNombreActual = ev.data.nombre;
	
	// con el id de la vista se sabe si puede ver o editar
	var viewID = parseInt(ev.data.id_vista);
	var access = Sidebar.Acceso[Sidebar.menus_ID.indexOf(viewID)];
	Sidebar.menuIDActual = viewID;

	//Función que envía la petición de crear el registro de trasabilidad del menu
	Sidebar.funciones.T_menu(viewID);

	// Cuando se esta en la versión de escritorio, en el topbar mostrara el nombre de la vista actual
	if(!( /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent))){
		Sidebar.funciones.NombreVistaTopBar(Sidebar.menuNombreActual);
		// se agrega la clase para deshabilitar los eventos en el sidebar, se vuelven a 
		// habilitar cuando la tabla este dibujada
		$("#accordion").addClass("disableEvents");
	}

	// Cuando se esta en la versión movil, cuando se seleccione una opcion del sidebar, este se cerrara
	if(/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)){
		Sidebar.gestionDOM.CloseSidebarTouch();
	}

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

	$(".tooltip").tooltip("dispose");
	$(window).off('resize');
	$(".popover").popover("dispose");

	$("#contenedorPrincipal").empty();
	$("#contenedorPrincipal").load(ev.data.url);
}

