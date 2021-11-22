$(document).ready(function() {
	//Se carga el data table
	Metas.gestionDOM.genTable();
	
    //Cuando el popover esta generado (pero oculto):
    $("#contenedorPrincipal").off("shown.bs.popover");
    $("#contenedorPrincipal").on("shown.bs.popover",Metas.gestionDOM.popoverFullyShown);

	//Cerrar popover cuando se de click en el selector
	$(`button[data-id='selectVista']`).on('click',Metas.gestionDOM.popoverOff);

	if(!( /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent))){
		// se quita la clase para habilitar los eventos en el sidebar nuevamente
		$("#accordion").removeClass("disableEvents");
	}
});