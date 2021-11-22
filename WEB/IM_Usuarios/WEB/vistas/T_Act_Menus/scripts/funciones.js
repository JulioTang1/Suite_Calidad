
T_Act_Menus.funciones = new Object();

/*****************************************************************************************************/

// Se consultas las fechas minimas y maximas de la tabla calendario_turno para limitar 
// los calendarios
T_Act_Menus.funciones.DatesMin = function(){
	
	query.callAjax(
		T_Act_Menus.urlConexionDates,
		'Menu_min_date',
		'',
		T_Act_Menus.funciones.DatesMinR
	);
}

// se guardan las fechas maxima y minima de la tabla calendario turnos
T_Act_Menus.funciones.DatesMinR = function(fecha_min){

	var fecha_max = moment().format("DD/MM/YYYY");

	fecha_min = fecha_min == "" ? fecha_max :  moment(fecha_min,"YYYY-MM-DD").format("DD/MM/YYYY");

	gestionDRP.Reporte_uso(
		"fechas_historico",
		fecha_min,
		fecha_max,
		"contenedorT_Act_Menus"
	);

	$('#fechas_historico').on('apply.daterangepicker',T_Act_Menus.gestionDOM.dateSelected);

	T_Act_Menus.gestionDOM.dateSelected();
}

