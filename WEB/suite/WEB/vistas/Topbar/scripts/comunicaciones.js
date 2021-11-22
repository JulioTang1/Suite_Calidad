
Topbar.comunicaciones = new Object();

Topbar.comunicaciones.searchBarResult = function(resultado_P,resultado_S){

	// Respuesta del callAjax, dependiendo si tiene resultados absolutos, relativos o ambos, se generara un html
	// dinamico diferente, si no hay coincidencias, borrara el contenedor del autocompletar

	if((resultado_P.length > 0)&&(resultado_S.length > 0))
	{
		Topbar.gestionDOM.autocompleteFull(resultado_P,resultado_S);
	}
	else if((resultado_P.length > 0)&&(resultado_S.length == 0))
	{
		Topbar.gestionDOM.autocompleteP(resultado_P);
	}
	else if((resultado_P.length == 0)&&(resultado_S.length > 0))
	{
		Topbar.gestionDOM.autocompleteS(resultado_S);
	}
	else
	{
		$("#autocomplete-result").empty();
		Topbar.searchBarList = -1;
	}
}

// funcion de retorno cuando quiere editar su perfil le retorna su información
Topbar.comunicaciones.editarPerfil = function(resultado){
	if(resultado.length > 0)
	{
		Topbar.gestionDOM.editarPerfil(resultado);
	}
	else
	{
		Login.gestionDOM.errorConexion();
	}
}

// función que le retorna el menu de favoritos que tenga un determinado usuario y los carga
// en el menu de favoritos
Topbar.comunicaciones.menuFavoritos = function(resultado){
	if(resultado.length > 0)
	{
		Topbar.gestionDOM.menuFavoritos(resultado);
	}
	else
	{
		Topbar.gestionDOM.menuSinFavoritos();
	}
}

// funcion de retorno que cuando se carga una vista se hace la consulta y se retorna si es o no
// es una vista favorita para cambiar la apariencia de la estrella en el topbar
Topbar.comunicaciones.favoriteView = function(resultado){
	if(resultado.length > 0)
	{
		Topbar.gestionDOM.ViewFav();
	}
	else
	{
		Topbar.gestionDOM.ViewNotFav();
	}
}