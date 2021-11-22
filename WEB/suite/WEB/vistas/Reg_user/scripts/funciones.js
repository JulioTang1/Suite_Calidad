
Reg_user.funciones = new Object();

Reg_user.funciones.loadInfo = function(){
	$("#UserReg").val(primario.user);
	//$("#departamento").val(primario.departamento);
	$("#email").val(primario.correo);
	$("#nombres").val(primario.nombre);
	$("#apellidos").val(primario.apellido);
	//$("#cargo").val(primario.cargo);
	//$("#pais").val(primario.pais);
	//$("#ciudad").val(primario.ciudad);
	$("#nombre_usuario").val(primario.nombre_usuario);
	//$("#UserAzure").val(primario.AzureUser);
}

// al solicitar acceso y el usurio esta registrado, contienen información ya se procede a enviar 
// la información para que sea almacenada a la base de datos
Reg_user.funciones.createUser = function(){

	gestionModal.alertaBloqueante(primario.aplicacion,"Procesando...");

	//imagen en el formulario en el modal
	var foto = $("#fileLoad input").val();

	if(foto == '')
	{
		var formData = new FormData();
		formData.append("ext","0");
	}
	else
	{
		// se extrae la extensión de la imagen
       	var nameImg = $("#fileLoad input").val().split(".");
       	var ext = nameImg[nameImg.length - 1];

       	// se organizan la imagen y la información para el registro
		var formElement = $("#fileLoad")[0];

		var formData = new FormData(formElement);
		formData.append("ext",ext);
	}

	// se agrega la información para enviarla junto a la imagen
	query.callAjaxUser(Reg_user.urlConexion, "Reg_User", formData, Reg_user.gestionDOM.RegCorrecto);
}

