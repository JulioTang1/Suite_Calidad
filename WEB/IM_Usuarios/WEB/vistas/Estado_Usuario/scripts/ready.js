$(document).ready(function() {

	/*******************************************************************************************************/ 

	//Cerrar popover cuando se de click en el selector
	$(`button[data-id='selectVista']`).on('click',Estado_Usuario.gestionDOM.popoverOff);

	 //Cuando el popover esta generado (pero oculto):
    $("#contenedorPrincipal").off("shown.bs.popover");
    $("#contenedorPrincipal").on("shown.bs.popover",Estado_Usuario.gestionDOM.popoverFullyShown);

	if(!( /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent))){
		// se quita la clase para habilitar los eventos en el sidebar nuevamente
		$("#accordion").removeClass("disableEvents");
	}

	Estado_Usuario.gestionDOM.genTable();

});