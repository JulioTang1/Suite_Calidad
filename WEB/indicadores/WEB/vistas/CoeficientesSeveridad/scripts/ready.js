$(document).ready(function() {
	//Se carga el data table
	CoeficientesSeveridad.gestionDOM.genTable();
	
    //Cuando el popover esta generado (pero oculto):
    $("#contenedorPrincipal").off("shown.bs.popover");
    $("#contenedorPrincipal").on("shown.bs.popover",CoeficientesSeveridad.gestionDOM.popoverFullyShown);

	if(!( /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent))){
		// se quita la clase para habilitar los eventos en el sidebar nuevamente
		$("#accordion").removeClass("disableEvents");
	}
});