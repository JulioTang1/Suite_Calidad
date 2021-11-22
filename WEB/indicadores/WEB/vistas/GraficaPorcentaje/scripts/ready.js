$(document).ready(function() {

	//LLENA EL SELECTOR DE GRUPOS
	GraficaPorcentaje.funciones.bringGrupos();

	/* SELECTORES NIDADOS */
	GraficaPorcentaje.gestionDOM.initSelectores();
	/* FIN SELECTORES NIDADOS */

	//Invoca las graficas
	$(`#selectFinca`).on("change", GraficaPorcentaje.gestionDOM.graph);

	//inicializa el calendario
	GraficaPorcentaje.gestionDOM.FechasLimitedRankGraph2('calendarPlanning');

	if(!( /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent))){
		// se quita la clase para habilitar los eventos en el sidebar nuevamente
		$("#accordion").removeClass("disableEvents");
	}
});