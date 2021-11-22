
T_Hist_Est_Usuario.funciones = new Object();

/*****************************************************************************************************/

// Se consultas las fechas minimas y maximas de la tabla calendario_turno para limitar 
// los calendarios
T_Hist_Est_Usuario.funciones.DatesMin = function(){
	
	query.callAjax(
		T_Hist_Est_Usuario.urlConexionDates,
		'Hist_min_date',
		'',
		T_Hist_Est_Usuario.funciones.DatesMinR
	);
}

// se guardan las fechas maxima y minima de la tabla calendario turnos
T_Hist_Est_Usuario.funciones.DatesMinR = function(fecha_min){

	var fecha_max = moment().format("DD/MM/YYYY");

	fecha_min = fecha_min == "" ? fecha_max :  moment(fecha_min,"YYYY-MM-DD").format("DD/MM/YYYY");

	gestionDRP.Reporte_uso(
		"fechas_historico",
		fecha_min,
		fecha_max,
		"contenedorT_Hist_Est_Usuario"
	);

	$('#fechas_historico').on('apply.daterangepicker',T_Hist_Est_Usuario.gestionDOM.dateSelected);

	T_Hist_Est_Usuario.gestionDOM.dateSelected();
}

