$(document).ready(function() {

	/* SELECTORES NIDADOS */
	Datos.gestionDOM.initSelectores();
	/* FIN SELECTORES NIDADOS */

	//Invoca la tabla
	$("#cargar").off("click").on("click", Datos.gestionDOM.genTable);

	//Habilita o deshabilita el boton
	$("#selectFinca").on("change", Datos.gestionDOM.enabledBtn);

	//inicializa el calendario
	Datos.gestionDOM.FechasLimitedRankGraph2('calendarPlanning');

	//Cuando el popover esta generado (pero oculto):
	$("#contenedorPrincipal").off("shown.bs.popover");
	$("#contenedorPrincipal").on("shown.bs.popover",Datos.gestionDOM.popoverFullyShown);
	
	if(!( /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent))){
		// se quita la clase para habilitar los eventos en el sidebar nuevamente
		$("#accordion").removeClass("disableEvents");
	}
});