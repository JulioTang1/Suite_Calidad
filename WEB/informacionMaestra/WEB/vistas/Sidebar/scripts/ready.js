$(document).ready(function() {

	$("#accordion").off();
	//función que carga el menu del sidebar dinamicamente
	Sidebar.funciones.SidebarDynamic();

	// función que carga la imagen superior y el titulo del sidebar desde base de datos
	// Sidebar.gestionDOM.ImageAndTitle();

	// Se pasa un string con el tipo de menu que se quiere, 'dark' para un menu oscuro 
	// y 'light' para un menu claro
	Sidebar.gestionDOM.Menu(Sidebar.menu);

	// funcion para añadir el scrollBar
	Sidebar.gestionDOM.scrollBar();

	// Animaciones del SideBar
	// Eventos para abrir y cerrar el Sidebar
	// esta comparación es para detectar si es un dispositivo movil o es un
	// computador de escritorio para generar los eventos

	if( /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)){

		

   		$("#accordion").addClass("disableEvents");

   		// Se abre el sidebar cuando se hace touch en el sidebar-nav
   		$("#sidebar-wrapper .sidebar-nav").off("touchstart").on("touchstart",Sidebar.gestionDOM.OpenSidebarTouch);
   		// Se cierra cuando se hace touch en el contenedor principal
		$("#contenedorPrincipal").off("click").on("click",Sidebar.gestionDOM.CloseSidebarTouch);

		//Se abre o se cierra el sidebar si se presiona en el logo
		// $("#sidebar-wrapper .brand-sidebar .logo-wrapper").off("click").on("click",Sidebar.gestionDOM.TouchToggle);
		
		//Se abre o se cierra el sidebar deslizando a la derecha o izquierda la barra
		$("#sidebar-wrapper").on('touchstart', Sidebar.gestionDOM.DashToggle);
	}
	else
	{
		$("#sidebar-wrapper").off();
		$("#sidebar-wrapper").on("mouseenter",Sidebar.gestionDOM.OpenSidebar);
		$("#sidebar-wrapper").on("mouseleave",Sidebar.gestionDOM.CloseSidebar);
	}

	// evento para sombrear el contenedor donde se selecciono una opción principal del sidebar
	$("#accordion").on("click",".titulosOpc",Sidebar.gestionDOM.backgroundSelected);

	// evento para sombrear el contenedor donde se selecciono una opcion secundaria del sidebar
	$("#accordion").on("click",".titulosOpcSec",Sidebar.gestionDOM.OpcionSelected);

	// eventos para la animación de las flechas del acordeon principal
	$("#accordion").on("webkitTransitionEnd otransitionend oTransitionEnd msTransitionEnd transitionend", ".collapse", Sidebar.gestionDOM.flagAnimation);

	// eventos para la animación de las flechas del primer acordeon secundario
	// $("#accordion").on('show.bs.collapse', "[data-parent='#SubAccordion1'].collapse", Sidebar.gestionDOM.animationArrowActiveSecondary);
	// $("#accordion").on('hide.bs.collapse', "[data-parent='#SubAccordion1'].collapse", Sidebar.gestionDOM.animationArrowDesactiveSecondary);

	// // eventos para la animación de las flechas del segundo acordeon secundario
	// $("#accordion").on('show.bs.collapse', "[data-parent='#SubAccordion2'].collapse", Sidebar.gestionDOM.animationArrowActiveSecondary);
	// $("#accordion").on('hide.bs.collapse', "[data-parent='#SubAccordion2'].collapse", Sidebar.gestionDOM.animationArrowDesactiveSecondary);

	// // eventos para la animación de las flechas del tercer acordeon secundario
	// $("#accordion").on('show.bs.collapse', "[data-parent='#SubAccordion3'].collapse", Sidebar.gestionDOM.animationArrowActiveSecondary);
	// $("#accordion").on('hide.bs.collapse', "[data-parent='#SubAccordion3'].collapse", Sidebar.gestionDOM.animationArrowDesactiveSecondary);

	// // eventos para la animación de las flechas del cuarto acordeon secundario
	// $("#accordion").on('show.bs.collapse', "[data-parent='#SubAccordion4'].collapse", Sidebar.gestionDOM.animationArrowActiveSecondary);
	// $("#accordion").on('hide.bs.collapse', "[data-parent='#SubAccordion4'].collapse", Sidebar.gestionDOM.animationArrowDesactiveSecondary);
	//Se debe poner estos dos eventos con el respectivo data-parent de cada sub-acordeon


	// evento para el background del la opción seleccionada
	$("#accordion").on("click", ".picked", Sidebar.gestionDOM.optionSelected);

	//carga de acordeones primarios en el html principal 
	// $("#acordeon1").load("vistas/Sidebar/HTML/Acordeones/acordeon1.html");
	// $("#acordeon2").load("vistas/Sidebar/HTML/Acordeones/acordeon2.html");
	// $("#acordeon3").load("vistas/Sidebar/HTML/Acordeones/acordeon3.html");
	// $("#acordeon4").load("vistas/Sidebar/HTML/Acordeones/acordeon4.html");

	//Si se desea utilizar sub-acordeones se tiene que habilitar la craga del javascript 
	//(ready) en el ultimo Html cargado y en ese documento modificar el contenedor donde 
	//se quiere cargar el sub-acordeon y el nombre de este.
	// $("#subAcordeon1").load("vistas/Sidebar/HTML/Sub_acordeones/subAcordeon1.html");

	gestionModal.cerrar();
});