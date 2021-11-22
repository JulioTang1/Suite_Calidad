$(document).ready(function() {

	Suit_P.gestionDOM.genApps();
	Suit_P.gestionDOM.genAppsTop();

	$("#back_APP").off().on("click",Suit_P.gestionDOM.back_suiteTop);

	Login.updateUserOnline = setTimeout(Suit_P.funciones.Update_user_online, 15000);

	window.addEventListener("message", Suit_P.funciones.receiveMessage, false);

	gestionModal.cerrar();
	
});