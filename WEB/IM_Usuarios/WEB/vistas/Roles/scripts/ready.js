$(document).ready(function() {

	if(Roles.baderaPermisosEdicion){
		Roles.gestionDOM.html_edit();
	}

	//Cuando el popover esta generado (pero oculto):
    $("#contenedorPrincipal").off("shown.bs.popover");
	$("#contenedorPrincipal").on("shown.bs.popover",Roles.gestionDOM.popoverFullyShown);
	
	//Visualizar los permisos
	$("#visualizar_RM").off().on("click", Roles.funciones.visualizar_RM);
	$("#visualizar_RP").off().on("click", {DT: 'ROLES_PERFIL'}, Roles.gestionDOM.table_MR_view);
	$("#visualizar_RA").off().on("click", {DT: 'ROLES_APP'}, Roles.gestionDOM.table_MR_view);


	if(!( /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent))){
		// se quita la clase para habilitar los eventos en el sidebar nuevamente
		$("#accordion").removeClass("disableEvents");
	}

});