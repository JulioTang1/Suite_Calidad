$(document).ready(function() {

	$("#imagen_logo .logo_login").attr("src", primario.imagen_registro);

	Reg_user.funciones.loadInfo();

	// evento cuando se produzca un cambio en el carga de la imagen en el modal
	// de solicitar acceso para modificar el contenedor 
	$("#fileLoad input").off().on("change",Reg_user.gestionDOM.loadPhoto);
	// cuando se cancela o se presiona la x el contenedor que tiene el modal se borra
	$("#CancelarAcceso").off().on("click",Reg_user.gestionDOM.closeSession);

	//evento que crea el usuario en la aplicación
	$("#CrearAcceso").on("click",Reg_user.funciones.createUser);

	// cuando se presione enter en alguno de los inputs se llamara la función para 
	// iniciar sesión
	$("#signin .div-input-md input").keyup(Reg_user.funciones.teclaEnter);

	gestionModal.cerrar();
	
});