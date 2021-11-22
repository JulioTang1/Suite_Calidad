$(document).ready(function() {

	/* SELECTORES NIDADOS */
	GraficaPrecipitaciones.gestionDOM.initSelectores();
	/* FIN SELECTORES NIDADOS */

	//Invoca las graficas
	$(`#selectFinca`).on("change", GraficaPrecipitaciones.gestionDOM.graph);
	$(`#selectTiempo`).on("change", GraficaPrecipitaciones.gestionDOM.graph);

	//inicializa el calendario
	GraficaPrecipitaciones.gestionDOM.FechasLimitedRankGraph2('calendarPlanning');

	if(!( /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent))){
		// se quita la clase para habilitar los eventos en el sidebar nuevamente
		$("#accordion").removeClass("disableEvents");
	}
});