
Login.gestionDOM = new Object();

// modal de carga
Login.gestionDOM.CargaPag = function(){
	gestionModal.alertaBloqueante(primario.aplicacion,"Procesando...");
}

// esta función sera el retorno de la consulta con el nombre del titulo, la url de la imagen 
// para la animación y la url de la imagen del logo
Login.gestionDOM.ImageAndTitle = function(resultado){
	
	Login.tituloSidebar = resultado[0].titulo_sidebar;
	Login.urlImgSideBar = resultado[0].url_sidebar_img;

	var txt = '';
	Login.tituloPag = resultado[0].titulo_pag;
	txt = `<span>${resultado[0].titulo_login}</span>`;
	$(".titulo").html(txt);

	txt = `<img draggable="false" draggable="false" src="${resultado[0].url_login_img}" class="logo_login">`;
	$("#imagen_logo").html(txt);

	// txt = `	<title>${resultado[0].titulo_pag}</title>
	// 		<link rel="icon" type="image/png" sizes="16x16" href="${resultado[0].url_favicon_img}">`;
	// $("head").append(txt);
}

Login.gestionDOM.errorConexion = function(){
    gestionModal.alertaConfirmacion(
        primario.aplicacion,
        "Error en la consulta, posiblemente es una falla en la conexión a Internet, intentelo de nuevo más tarde",
        "error",
        "Ok",
        "#f27474",
        function(){}
    );
}