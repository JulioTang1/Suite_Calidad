$(document).ready(function() {

	gestionModal.alertaBloqueante(primario.aplicacion, "Procesando...");

	moment.locale('es');
	T_Act_Menus.funciones.DatesMin();

	if(!( /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent))){
		// se quita la clase para habilitar los eventos en el sidebar nuevamente
		$("#accordion").removeClass("disableEvents");
	}
	
});