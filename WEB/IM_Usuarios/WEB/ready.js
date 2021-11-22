$(document).ready(function() {

 	var message = {
 		mensaje: "ready_page"
 	};

 	function Validar_token(resultado){

		if (resultado.length > 0) 
		{	

			Login.id_rol		= resultado[0].id_rol;
			Login.ID_user 		= resultado[0].ID_user;
			Login.id_app		= resultado[0].id_app;

			// se carga el sidebar y el topbar
			$("#Sidebar").load("vistas/Sidebar/Sidebar.html", function(){
				$("#contenedorPrincipal").load("vistas/Pag_construccion/Pag_construccion.html");
			});

			message.mensaje = "Listo";
			primario.window_source.postMessage(message,primario.window_origin);
		} 
		else 
		{
			message.mensaje = "Error";
			primario.window_source.postMessage(message,primario.window_origin);
		}
	}

	function error_url_DB(){ 
		message.mensaje = "error_url_DB";
		window.parent.postMessage(message,document.referrer);
	}

	function receiveMessage(event){ 
		if (event.data.url_suite == event.origin)
		{
			var data = {
				id_user: 	event.data.id_user,
				token: 		event.data.token,
				id_app: 	event.data.id_app,
				navegador: 	event.data.navegador
			};	
			
			primario.window_source = event.source;
			primario.window_origin = event.origin;	
			
			query.callAjax(primario.urlConexionToken, "Validar_token", data, Validar_token);
		}
		else
		{
			var data = {
				App: 		primario.aplicacion,
				url_real: 	event.origin,
				url_BD:		event.data.url_suite
			};
			query.callAjax(primario.urlConexionToken, "error_url_DB", data, error_url_DB);
		}
	}

    window.addEventListener("message", receiveMessage, false);

    if(document.referrer != ""){
		window.parent.postMessage(message,document.referrer);
		
		$("body").off("click").on("click",function(){
			var ev = {
				mensaje: "cerrar_DD"
			};
	
			window.parent.postMessage(ev,document.referrer);
		});
    }
    else{
    	var data = {
    		App: primario.aplicacion
    	};
    	query.callAjax(primario.urlConexionToken, "error_iframe", data, function(){});
	}
});