
Reg_user.gestionDOM = new Object();

Reg_user.gestionDOM.closeSession = function(){

	delete Login;
	$("#Login").empty();

	gestionModal.alertaBloqueante(primario.aplicacion, "Procesando...");
	query.callAjax(primario.urlConexion,"closeSession",'',Reg_user.gestionDOM.loadLogin);
}

Reg_user.gestionDOM.loadLogin = function(){
	$("#Login").load("vistas/Login/Login.html");
}


// Función para cambiar el estilo del label si la imagen esta cargado o no 
// en el formulario de Solicitud de acceso
Reg_user.gestionDOM.loadPhoto = function(){
	if($("#fileLoad input").val() != ''){
		$("#subirFoto").addClass("fotoSeleccionada");
		$("#subirFoto span").html("Foto cargada");
		$("#subirFoto i").html("done_all");
	}
	else
	{
		$("#subirFoto").removeClass("fotoSeleccionada");	
		$("#subirFoto span").html("Subir foto");
		$("#subirFoto i").html("add_a_photo");
	}
}

Reg_user.gestionDOM.RegCorrecto = function(result){

	// información del usuario
	Login.urlImgTopBar 	= result["USER"][0].Foto;
	Login.User			= result["USER"][0].usuario;
	Login.Name 			= result["USER"][0].nombres;
	Login.lastName 		= result["USER"][0].apellidos;
	Login.ID_user 		= result["USER"][0].ID_user;
	Login.Email 		= result["USER"][0].email;
	Login.nombre_usuario= result["USER"][0].nombre_usuario;
	Login.Token 		= result["USER"][0].Token;

	Login.navegador 	= primario.getBrowserInfo();

	// información de la aplicación 
	Login.APPS = result["APP"];
	Login.CAT_APPS = result["CAT_APP"];

	$("#Login").empty();
	$("#contenedorPrincipal").load("vistas/Suit_P/Suit_P.html");
	$("#Topbar").load("vistas/Topbar/Topbar.html");
}