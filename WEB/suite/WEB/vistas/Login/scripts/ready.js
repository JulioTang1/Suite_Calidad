$(document).ready(function() {

	$("#imagen_logo .logo_login").attr("src", primario.imagen_principal);

	// evento cuando se quiere iniciar sesión
	$(".login").off().on("click",Login.funciones.InicioSesion);

	/* Provisional */
	$(".L_banasan").off().on("click",Login.funciones.InicioSesionBanasan);

	gestionModal.cerrar();
});