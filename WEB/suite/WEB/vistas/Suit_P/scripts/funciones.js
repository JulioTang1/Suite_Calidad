
Suit_P.funciones = new Object();

Suit_P.funciones.login = function(ev){
	var app = $(ev.currentTarget.parentNode).attr("id");
	var info = Login.APPS.filter(apps => apps.nombre == app)[0];
	var acceso = info.Acceso;

	if(acceso == 0){
		Suit_P.gestionDOM.Inactivo_rol();
	} 
	else if(info.url_website == "" || info.url_website == null)
	{
		Suit_P.gestionDOM.modulos_des();
	}
	else
	{
		clearTimeout(Login.updateUserOnline);
		//Poner lo del iframe
		gestionModal.alertaBloqueante(primario.aplicacion, "Procesando...");
		document.getElementById("iframeApps").src = info.url_website;
	}
}

Suit_P.funciones.loginTop = function(ev){

	var app = $(ev.currentTarget).attr("id");

	if (app == "volver_suite"){
		Suit_P.gestionDOM.back_suiteTop();
	}
	else
	{
		var info = Login.APPS.filter(apps => apps.nombre == app)[0];
		clearTimeout(Login.updateUserOnline);
		//Poner lo del iframe
		gestionModal.alertaBloqueante(primario.aplicacion, "Procesando...");

		if(info.url_website == "" || info.url_website == null)
		{
			Suit_P.gestionDOM.modulos_des();
		}
		else
		{
			document.getElementById("iframeApps").src = info.url_website;
		}
	}
}

Suit_P.funciones.infoApp = function(ev){

	var app = $(ev.target).parent().parent().parent().attr("id");
	var info = Login.APPS.filter(apps => apps.nombre == app)[0];

	Suit_P.gestionDOM.genInfoApp(info);
}


/**********************************************************************************************************/ 

Suit_P.funciones.Update_user_online = function(){

	query.callAjax(
		Suit_P.urlConexion, 
		"updUserOnline", 
		"", 
		Suit_P.comunicaciones.Update_user_online
	);
}

/**********************************************************************************************************/ 

Suit_P.funciones.receiveMessage = function(event){ 

	var info = Login.APPS.filter(apps => apps.url_website == event.origin);

	if (info.length == 1)
	{
		switch(event.data.mensaje){
			case "ready_page":

				var wn = window.frames["iframeApps"].contentWindow;
				var message = {
					id_user: 	Login.ID_user,
					token: 		Login.Token,
					id_app: 	info[0].id,
					navegador: 	Login.navegador,
					url_suite:  info[0].url_website_suite
				};
				wn.postMessage(message, info[0].url_website);
			break;

			case "Listo":

				Suit_P.gestionDOM.pag_lista(info);
			break;

			case "end_session":

				Topbar.gestionDOM.cerrar_sesion();
			break;

			case "copy-to-clipboard":

				Suit_P.funciones.guardarEnPortapapeles(event.data.toCopy);
			break;

			case "TopBar":

				Suit_P.gestionDOM.topbar(info[0].titulo_login, event.data.name_menu);
			break;

			case "Carga_Modulo":

				gestionModal.cerrar();
			break;

			case "cerrar_DD":

				$(".dropdown-menu").dropdown("hide");
			break;

			default:
				gestionModal.alertaConfirmacion(
					primario.aplicacion,
					"No es posible ingresar al módulo, el equipo de soporte ya ha sido notificado para solucionarlo lo más pronto posible. Disculpe las molestias ocasionadas.",
					"error",
					"Ok",
					"#f27474",
					function(){}
				);
			break;
		}
	}
	else if(info.length == 0){

	}
	else
	{
		
	}
}

//Guarda en portapapeles
Suit_P.funciones.guardarEnPortapapeles = function(texto) {
	//Se verifica la existencia de la propiedad "clipboard" para el objeto "navigator"
	if(navigator.clipboard){
		//Se escribe el portapapeles
		navigator.clipboard.writeText(texto)
			.then(function() { 
				//Desarrollo de la funcion en caso exitoso, no se usa
			}, 
			function() {
				//Muesta mensaje de error si falla la lectura del portapapeles
				gestionModal.alertaConfirmacion(
					primario.aplicacion,
					"Por favor habilite el acceso de lectura y escritura al portapapeles.",
					"warning",
					"Ok",
					"#f27474",
					function(){}
				);
			});
	}else{
		//Muestra modal de error si no existe la propiedad "clipboard"
		gestionModal.alertaConfirmacion(
			primario.aplicacion,
			"El acceso de lectura y escritura al portapapeles no es soportado en este navegador.",
			"error",
			"Ok",
			"#f27474",
			function(){}
		);
	}
}
