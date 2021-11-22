$(document).ready(function() {

	/* SELECTORES NIDADOS */
	Precipitaciones.gestionDOM.initSelectores();
	/* FIN SELECTORES NIDADOS */

	//Invoca las graficas
	$(`#selectFinca`).on("change", Precipitaciones.funciones.precipitaciones);

	//inicializa el calendario
	Precipitaciones.gestionDOM.FechasLimitedRankGraph2('calendarPlanning');

	if(!( /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent))){
		// se quita la clase para habilitar los eventos en el sidebar nuevamente
		$("#accordion").removeClass("disableEvents");
	}
});