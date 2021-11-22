$(document).ready(function() {

	/* SELECTORES NIDADOS */
	Consolidado.gestionDOM.initSelectores();
	/* FIN SELECTORES NIDADOS */

	//Invoca la tabla
	$("#cargar").off("click").on("click", Consolidado.gestionDOM.genTable);

	//Habilita o deshabilita el boton
	$("#selectFinca").on("change", Consolidado.gestionDOM.enabledBtn);

	//inicializa el calendario
	Consolidado.gestionDOM.FechasLimitedRankGraph2('calendarPlanning');

	
	if(!( /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent))){
		// se quita la clase para habilitar los eventos en el sidebar nuevamente
		$("#accordion").removeClass("disableEvents");
	}
});