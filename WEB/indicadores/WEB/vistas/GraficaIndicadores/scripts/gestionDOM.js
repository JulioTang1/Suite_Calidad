GraficaIndicadores.gestionDOM = new Object();

/* SELECTORES ANIDADOS */
GraficaIndicadores.gestionDOM.initSelectores = function(){
	/* Estilo para selectores */
	gestionBSelect.generic3(`#selectDepartamento`, "#divSelectVista", 'Departamentos');
	gestionBSelect.generic3(`#selectMunicipio`, "#divSelectVista", 'Municipios');	
	gestionBSelect.generic3(`#selectFinca`, "#divSelectVista", 'Finca', undefined, false);

	//Anidados de edades e indicadores
	gestionBSelect.generic3(`#selectEdad`, "#divSelectVista", 'Edades');
	gestionBSelect.generic3(`#selectInfeccion`, "#divSelectVista", 'Indicadores');

	//Funciones encargadas de llenar los selectores y mover banderas
	$('button[data-id="selectDepartamento"]').click(GraficaIndicadores.funciones.fillDepartamento);
	$('#selectDepartamento').change({select:"selectDepartamento"},GraficaIndicadores.funciones.changeFlag);
	$('button[data-id="selectMunicipio"]').click(GraficaIndicadores.funciones.fillMunicipio);
	$('#selectMunicipio').change({select:"selectMunicipio"},GraficaIndicadores.funciones.changeFlag);
	$('button[data-id="selectFinca"]').click(GraficaIndicadores.funciones.fillFinca);
	$('#selectFinca').change({select:"selectFinca"},GraficaIndicadores.funciones.changeFlag);

	//Funciones encargadas de llenar los selectores y mover banderas (edades indicadores)
	$('button[data-id="selectEdad"]').click(GraficaIndicadores.funciones.fillEdad);
	$('#selectEdad').change({select:"selectEdad"},GraficaIndicadores.funciones.changeFlagEI);
	$('button[data-id="selectInfeccion"]').click(GraficaIndicadores.funciones.fillInfeccion);
	$('#selectInfeccion').change({select:"selectInfeccion"},GraficaIndicadores.funciones.changeFlagEI);

	//Se quita el tooltip por defecto de los selectores
	$(`[data-id="selectDepartamento"]`).removeAttr("title");
	$(`[data-id="selectMunicipio"]`).removeAttr("title");
	$(`[data-id="selectFinca"]`).removeAttr("title");

	//Anidados de edades e indicadores
	$(`[data-id="selectEdad"]`).removeAttr("title");
	$(`[data-id="selectInfeccion"]`).removeAttr("title");
}

//Se dibujan los valores de los selectores
GraficaIndicadores.gestionDOM.fillSelectores = function(resultado, id){
    //Si hay selecciones se guardan
    var options = $(id).val();

	var html;
	for(var i = 0; i < resultado.length; i++){
		if(resultado[i].id != null){
			html = `${html}
			<option value="${resultado[i].id}">${resultado[i].name}</option>`;
		}
	}
	$(id).html(html);
	$(id).selectpicker("refresh");
	$(id).val(options)
	$(id).selectpicker("refresh");

	//Se genera la grafica si las opciones de finca cambiaron
	if( id == "#selectFinca" && (options != $(id).val()) ){
		GraficaIndicadores.gestionDOM.graph();
	}

	//Se cierra el modal
	gestionModal.cerrar();
}
/* FIN SELECTORES ANIDADOS */

//Se plotea la grafica
GraficaIndicadores.gestionDOM.graph = function(){
	if( $(`#selectFinca`).val().length > 0 && $(`#selectEdad`).val().length > 0 && $(`#selectInfeccion`).val().length > 0 ){
		/*Variable de graficas para hacer multiples peticiones y que se borren las anteriores ya que no encuentran el
		 antiguo contenedor de la grafica*/
		$("#mainGraph").html(`<div id="containerGraph${GraficaIndicadores.countGraph}" class="container-fluid graficaStyleCss"></div>`);

		/*Importante, este evento debe desactivarse cada que se vaya a cargar una vista que contenga 
		highcharts(gestion interna), puede ir en el ready a no se que se borre html y se haga un 
		llamado a la funcion chart.initGraph por medio de otras funciones sin entrar en el ready*/
		$(window).off('resize');

	    var replace = {
	        fecha_ini: 	GraficaIndicadores.fechaIniRank,
	        fecha_fin: 	GraficaIndicadores.fechaFinRank,
			nivel: 		`s`,
			fincas: 	`${$(`#selectFinca`).val()}`,
			edad: 		`${$(`#selectEdad`).val()}`,
			infeccion: 	`${$(`#selectInfeccion`).val()}`
	    };

		chart.initGraph(
			GraficaIndicadores.urlConexion, 
			"GraficaIndicadores",
			`containerGraph${GraficaIndicadores.countGraph}`, 
			'Exportar', 
			0, 
			replace
		);
		//Resetea la variable
		if(GraficaIndicadores.countGraph == 100){
			GraficaIndicadores.countGraph = 0;
		}else{
			GraficaIndicadores.countGraph++;
		}
	}
	else{
		$("#mainGraph").empty();
	}
}

/*Evento de click en calendario para alterar la forma en la que se abre el 
calendario si se encuentra en dispositivo mobil*/
GraficaIndicadores.gestionDOM.calendarMobil = function(){
    //REVISAR, AÚN FALTA
	if((/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent))){
		//Se limita el ancho de la ventana de fechas para dispositivos mobiles
		$("div.daterangepicker.opensright").css("top","0");
	}
}

GraficaIndicadores.gestionDOM.FechasLimitedRankGraph2 = function(tagDiv){
	var start = moment().add(-7, 'days');
 	var end = moment();
    // Se configura las opciones para configurar el idioma 
    var option = {
    	showDropdowns: true,
    	locale: {
	        "applyLabel": "Aplicar",
	        "cancelLabel": "Cancelar",
	        "fromLabel": "Desde",
	        "toLabel": "Hasta",
	        "customRangeLabel": "Rango",
	        "weekLabel": "S",
	        "daysOfWeek": [
	            "Do",
	            "Lu",
	            "Ma",
	            "Mi",
	            "Ju",
	            "Vi",
	            "Sa"
	        ],
	        "monthNames": [
	            "Enero",
	            "Febrero",
	            "Marzo",
	            "Abril",
	            "Mayo",
	            "Junio",
	            "Julio",
	            "Agosto",
	            "Septiembre",
	            "Octubre",
	            "Noviembre",
	            "Diciembre"
	        ],
	        "firstDay": 1
    	},
        ranges: {
	        'Hoy'				: [moment(), moment()],
	        'Ayer'				: [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
	        'Últimos 7 Días'	: [moment().subtract(6, 'days'), moment()],
	        'Últimos 30 Días'	: [moment().subtract(29, 'days'), moment()],
	        'Este Mes'			: [moment().startOf('month'), moment().endOf('month')],
	        'Último Mes'		: [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')]
		},
		showISOWeekNumbers: true,
        startDate: start,
        endDate: end,
    	alwaysShowCalendars: true,
    	singleDatePicker:false
    }

    function cb(start, end) {
        GraficaIndicadores.fechaIniRank = start.format('YYYY-MM-DD');
        GraficaIndicadores.fechaFinRank = end.format('YYYY-MM-DD');
        $(`#${tagDiv} span`).html("Rango Visualización &nbsp; &nbsp; &nbsp; ");
        if($(`#selectFinca`).val().length > 0 ){
			GraficaIndicadores.gestionDOM.graph();
        }
    }

    $(`#${tagDiv}`).daterangepicker(option, cb);
    cb(start, end);
}