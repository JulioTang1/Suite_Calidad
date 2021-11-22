$(document).ready(function() {

	/* SELECTORES NIDADOS */
	Nuevo.gestionDOM.initSelectores();
	/* FIN SELECTORES NIDADOS */

	//Se llena el selector de visitas
	$(`#selectFinca`).on("change", Nuevo.funciones.bringVisita);

	//Se trae la informacion de visitas
	$(`#selectVisita`).on("change", Nuevo.funciones.datos);

	//inicializa el calendario
	Nuevo.gestionDOM.FechasLimitedRankGraph2('calendarPlanning');

	if(!( /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent))){
		// se quita la clase para habilitar los eventos en el sidebar nuevamente
		$("#accordion").removeClass("disableEvents");
	}
});