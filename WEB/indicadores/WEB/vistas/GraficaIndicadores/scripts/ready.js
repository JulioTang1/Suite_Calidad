$(document).ready(function() {

	/* SELECTORES NIDADOS */
	GraficaIndicadores.gestionDOM.initSelectores();
	/* FIN SELECTORES NIDADOS */

	//Invoca las graficas
	$(`#selectFinca`).on("change", GraficaIndicadores.gestionDOM.graph);
	$(`#selectEdad`).on("change", GraficaIndicadores.gestionDOM.graph);
	$(`#selectInfeccion`).on("change", GraficaIndicadores.gestionDOM.graph);

	//inicializa el calendario
	GraficaIndicadores.gestionDOM.FechasLimitedRankGraph2('calendarPlanning');

	if(!( /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent))){
		// se quita la clase para habilitar los eventos en el sidebar nuevamente
		$("#accordion").removeClass("disableEvents");
	}
});