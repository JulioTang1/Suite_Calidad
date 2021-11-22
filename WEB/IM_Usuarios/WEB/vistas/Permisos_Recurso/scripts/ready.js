$(document).ready(function() {

	if(Permisos_Recurso.baderaPermisosEdicion){
		Permisos_Recurso.gestionDOM.html_edit();
	}

	//Cuando el popover esta generado (pero oculto):
    $("#contenedorPrincipal").off("shown.bs.popover");
	$("#contenedorPrincipal").on("shown.bs.popover",Permisos_Recurso.gestionDOM.popoverFullyShown);

	//Visualizar los permisos
	$("#visualizar").off().on("click", Permisos_Recurso.gestionDOM.table_P_view);


	if(!( /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent))){
		// se quita la clase para habilitar los eventos en el sidebar nuevamente
		$("#accordion").removeClass("disableEvents");
	}

});