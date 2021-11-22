$(document).ready(function() {		

	gestionModal.alertaBloqueante(primario.aplicacion, "Procesando...");

	// funcion que carga la vista del login para redireccionar a la aplicación
	// externa de autenticación
	function PremierLogin(){
		$("#Login").empty();
		$("#Login").load("vistas/Login/Login.html");
	}
	
	// si se entra en esta funcion el usuario esta registrado tanto en la aplicación
	// externa como en la nuestra, se cargan sus datos ademas del nombre de la pagina,
	// del sidebar y las url del favicon y la imagen del sidebar y se carga la aplicación
	function InicioSeccion(result){

		if(result.RESULTADO["ESTADO"] == "TRUE"){
			// información del usuario
			Login.urlImgTopBar 	= result.RESULTADO["USER"][0].Foto;
			Login.User			= result.RESULTADO["USER"][0].usuario;
			Login.Name 			= result.RESULTADO["USER"][0].nombres;
			Login.lastName 		= result.RESULTADO["USER"][0].apellidos;
			Login.ID_user 		= result.RESULTADO["USER"][0].ID_user;
			Login.Email 		= result.RESULTADO["USER"][0].email;
			Login.nombre_usuario= result.RESULTADO["USER"][0].nombre_usuario;
			Login.Token 		= result.RESULTADO["USER"][0].Token;

			Login.navegador 	= primario.getBrowserInfo();

			// información de la aplicación 
			Login.APPS = result.RESULTADO["APP"];
			Login.CAT_APPS = result.RESULTADO["CAT_APP"];

			$("#Login").empty();
			$("#contenedorPrincipal").load("vistas/Suit_P/Suit_P.html");
			$("#Topbar").load("vistas/Topbar/Topbar.html");
		}
		else
		{
			primario.correo 		= result.RESULTADO["USER"][0].email;
			primario.user   		= result.RESULTADO["USER"][0].usuario;
			primario.nombre 		= result.RESULTADO["USER"][0].nombres;
			primario.apellido 		= result.RESULTADO["USER"][0].apellidos;
			primario.nombre_usuario = result.RESULTADO["USER"][0].nombre_usuario;
			
			$("#Login").empty();
			$("#Login").load("vistas/Reg_user/Reg_user.html")
		}
	}

	// función de retorno cuando el usuario tiene una sesión iniciada y se consulto si el 
	// usuario esta registrado en nuestra base de datos, dependiendo del resultado
	// (activo, bloqueado, inactivo y sin registrar) se redirige a su respectiva función
	function UserReg(resultado){
		result = JSON.parse(resultado);
		InicioSeccion(result);
	}

	//función para determinar el nombre del navegador y la versión de este
	primario.getBrowserInfo = function() {
	    var ua= navigator.userAgent, tem, 
	    M= ua.match(/(opera|chrome|safari|firefox|msie|trident(?=\/))\/?\s*(\d+)/i) || [];
	    if(/trident/i.test(M[1])){
	        tem=  /\brv[ :]+(\d+)/g.exec(ua) || [];
	        return 'IE '+(tem[1] || '');
	    }
	    if(M[1]=== 'Chrome'){
	        tem= ua.match(/\b(OPR|Edge)\/(\d+)/);
	        if(tem!= null) return tem.slice(1).join(' ').replace('OPR', 'Opera');
	    }
	    M= M[2]? [M[1], M[2]]: [navigator.appName, navigator.appVersion, '-?'];
	    if((tem= ua.match(/version\/(\d+)/i))!= null) M.splice(1, 1, tem[1]);
	    return M.join(' ');
	};

	var data = {
		navegador: primario.getBrowserInfo()
	};
	// función que verifica si la sesión esta iniciada
	query.callAjaxLogin(primario.urlConexionLogin,"haveSession",data,PremierLogin,UserReg);

});