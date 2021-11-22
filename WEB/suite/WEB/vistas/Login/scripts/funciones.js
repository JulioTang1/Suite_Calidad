
Login.funciones = new Object();

// funcion que hace la validación de los inputs para el inició de sesión, si estos
// contienen información ya se procede a enviar la información al WS externo
Login.funciones.InicioSesion = function(){
	
	gestionModal.alertaBloqueante(primario.aplicacion, "Procesando...");

	// Redirección para la validación del usuario
	window.location.href = Login.urlConexionLogin + "/SSOValidation";
}

/* Provisional */
Login.funciones.InicioSesionBanasan = function(){
	
	gestionModal.alertaBloqueante(primario.aplicacion, "Procesando...");

	// Redirección para la validación del usuario
	window.location.href = Login.urlConexionLogin + "/SSOBanasan";
}

