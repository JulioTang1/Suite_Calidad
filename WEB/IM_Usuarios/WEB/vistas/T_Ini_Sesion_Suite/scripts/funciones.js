
T_Ini_Sesion_Suite.funciones = new Object();

/*****************************************************************************************************/

// Se consultas las fechas minimas y maximas de la tabla calendario_turno para limitar 
// los calendarios
T_Ini_Sesion_Suite.funciones.DatesMin = function(){
	
	query.callAjax(
		T_Ini_Sesion_Suite.urlConexionDates,
		'Sesion_suite_min_date',
		'',
		T_Ini_Sesion_Suite.funciones.DatesMinR
	);
}

// se guardan las fechas maxima y minima de la tabla calendario turnos
T_Ini_Sesion_Suite.funciones.DatesMinR = function(fecha_min){

	var fecha_max = moment().format("DD/MM/YYYY");

	fecha_min = fecha_min == "" ? fecha_max :  moment(fecha_min,"YYYY-MM-DD").format("DD/MM/YYYY");

	gestionDRP.Reporte_uso(
		"fechas_historico",
		fecha_min,
		fecha_max,
		"contenedorT_Ini_Sesion_Suite"
	);

	$('#fechas_historico').on('apply.daterangepicker',T_Ini_Sesion_Suite.gestionDOM.dateSelected);

	T_Ini_Sesion_Suite.gestionDOM.dateSelected();
}