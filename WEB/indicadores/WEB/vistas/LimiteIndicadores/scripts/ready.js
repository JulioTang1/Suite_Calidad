$(document).ready(function() {

	//Se trae la informacion de los limites de indicadores
	$(`#selectEdad`).off("change").on("change", LimiteIndicadores.funciones.info);
	$(`#selectInfeccion`).off("change").on("change", LimiteIndicadores.funciones.info);
	$(`#selectTipoFinca`).off("change").on("change", LimiteIndicadores.funciones.info);

	//Inicializa los selectores
	LimiteIndicadores.gestionDOM.initSelectores();

	//Se ocultan los colores y la tabla
	$(".LimiteIndicadoresExterno").css("display", "none");

	if(!( /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent))){
		// se quita la clase para habilitar los eventos en el sidebar nuevamente
		$("#accordion").removeClass("disableEvents");
	}
});