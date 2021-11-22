$(document).ready(function() {

	/* SELECTORES NIDADOS */
	RecorridoFincas.gestionDOM.initSelectores();
	/* FIN SELECTORES NIDADOS */

	//Se oculta el menu que pertenece a el mapa
	$("#selectEdad").parent().css("display", "none");
	$("#selectInfeccion").parent().css("display", "none");
	$("#volver").css("display", "none");

	//Se limpia informacion sobre la visita actual
	$('.TituloCategoria .titulo_opcion').css("display", "none");
	$("#titulo_opcion").empty();

	//Se desaparece el navbar
	$("#nav-wrapper-balance").css("display", "none");
	$(".tab-content").css("display", "none");
	$("#navWrapper").css("display", "none");

	//inicializa el calendario
	RecorridoFincas.gestionDOM.FechasLimitedRankGraph2('calendarPlanning');

	//Habilita o deshabilita el boton
	$("#selectFinca").on("change", RecorridoFincas.gestionDOM.enabledBtn);

	//Refresca el mapa
	$(`#selectEdad`).on("change", RecorridoFincas.funciones.visita);
	$(`#selectInfeccion`).on("change", RecorridoFincas.funciones.visita);

	//volver
	$("#volver").off("click").on("click", RecorridoFincas.gestionDOM.volver);

    //Cuando el popover esta generado (pero oculto):
    $("#contenedorPrincipal").off("shown.bs.popover");
    $("#contenedorPrincipal").on("shown.bs.popover",RecorridoFincas.gestionDOM.popoverFullyShown);

	//Invoca la tabla
	$("#cargar").off("click").on("click", RecorridoFincas.gestionDOM.genTable);

	if(!( /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent))){
		// se quita la clase para habilitar los eventos en el sidebar nuevamente
		$("#accordion").removeClass("disableEvents");
	}
});