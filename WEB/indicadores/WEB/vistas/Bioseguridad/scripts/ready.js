$(document).ready(function() {

	/* SELECTORES NIDADOS */
	Bioseguridad.gestionDOM.initSelectores();
	/* FIN SELECTORES NIDADOS */

	//Invoca la tabla
	$("#cargar").off("click").on("click", Bioseguridad.gestionDOM.genTable);

	//Habilita o deshabilita el boton
	$("#selectFinca").on("change", Bioseguridad.gestionDOM.enabledBtn);

	//inicializa el calendario
	Bioseguridad.gestionDOM.FechasLimitedRankGraph2('calendarPlanning');

    //Cuando el popover esta generado (pero oculto):
    $("#contenedorPrincipal").off("shown.bs.popover");
    $("#contenedorPrincipal").on("shown.bs.popover",Bioseguridad.gestionDOM.popoverFullyShown);

	//Cerrar popover cuando se de click en el selector
	$(`button[data-id='selectVista']`).on('click',Bioseguridad.gestionDOM.popoverOff);

	if(!( /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent))){
		// se quita la clase para habilitar los eventos en el sidebar nuevamente
		$("#accordion").removeClass("disableEvents");
	}
});