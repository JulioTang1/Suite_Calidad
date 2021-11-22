
Login.comunicaciones = new Object();

// funcion de callback que contiene el titulo de la pagina, el titulo del inicio de sesión
// las url del favicon y la imagen de inicio de sesión 
Login.comunicaciones.ImageAndTitle = function(resultado){
	
	if(resultado.length > 0)
	{
		Login.gestionDOM.ImageAndTitle(resultado);
	}
}
