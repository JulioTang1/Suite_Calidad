
var Suit_P = new Object();

Suit_P.urlConexion = primario.urlConexion;

//variable para actualizar la información de las aplicaciones en tiempo real (cada 15 segundos)
// se pone login debido a que cuando se cierre la aplicacion se debe limpiar el setTimeOut
Login.updateUserOnline = '';
