
Suit_P.comunicaciones = new Object();

Suit_P.comunicaciones.Update_user_online = function(resultado){
	if(resultado.length > 0){
		Login.APPS = resultado;
		Suit_P.gestionDOM.Update_user_online();
	}
}