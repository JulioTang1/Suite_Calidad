GraficaEEV.gestionDOM = new Object();

/* SELECTORES ANIDADOS */
GraficaEEV.gestionDOM.initSelectores = function(){
	/* Estilo para selectores */
	gestionBSelect.generic3(`#selectDepartamento`, "#divSelectVista", 'Departamentos');
	gestionBSelect.generic3(`#selectMunicipio`, "#divSelectVista", 'Municipios');
	gestionBSelect.generic3(`#selectFinca`, "#divSelectVista", 'Finca', undefined, false);

	//Funciones encargadas de llenar los selectores y mover banderas
	$('button[data-id="selectDepartamento"]').click(GraficaEEV.funciones.fillDepartamento);
	$('#selectDepartamento').change({select:"selectDepartamento"},GraficaEEV.funciones.changeFlag);
	$('button[data-id="selectMunicipio"]').click(GraficaEEV.funciones.fillMunicipio);
	$('#selectMunicipio').change({select:"selectMunicipio"},GraficaEEV.funciones.changeFlag);
	$('button[data-id="selectFinca"]').click(GraficaEEV.funciones.fillFinca);
	$('#selectFinca').change({select:"selectFinca"},GraficaEEV.funciones.changeFlag);

	//Se quita el tooltip por defecto de los selectores
	$(`[data-id="selectDepartamento"]`).removeAttr("title");
	$(`[data-id="selectMunicipio"]`).removeAttr("title");
	$(`[data-id="selectFinca"]`).removeAttr("title");
}

//Se dibujan los valores de los selectores
GraficaEEV.gestionDOM.fillSelectores = function(resultado, id){
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
		GraficaEEV.gestionDOM.graph();
	}

	//Se cierra el modal
	gestionModal.cerrar();
}
/* FIN SELECTORES ANIDADOS */

//Se plotea la grafica
GraficaEEV.gestionDOM.graph = function(){
	if( $(`#selectFinca`).val().length > 0){
		/*Variable de graficas para hacer multiples peticiones y que se borren las anteriores ya que no encuentran el
		 antiguo contenedor de la grafica*/
		$("#mainGraph").html(`<div id="containerGraph${GraficaEEV.countGraph}" class="container-fluid graficaStyleCss"></div>`);

		/*Importante, este evento debe desactivarse cada que se vaya a cargar una vista que contenga 
		highcharts(gestion interna), puede ir en el ready a no se que se borre html y se haga un 
		llamado a la funcion chart.initGraph por medio de otras funciones sin entrar en el ready*/
		$(window).off('resize');

	    var replace = {
	        fecha_ini: 	GraficaEEV.fechaIniRank,
	        fecha_fin: 	GraficaEEV.fechaFinRank,
			nivel: 		`s`,
			fincas: 	`${$(`#selectFinca`).val()}`,
			edad: 		`6`,//FILTRO DE EDADES SELECCIONADAS
			infeccion: 	`8, 48, 49, 50`//FILTRO DE INDICADOR SELECCIONADO
	    };

		chart.initGraph(
			GraficaEEV.urlConexion, 
			"GraficaEEV",
			`containerGraph${GraficaEEV.countGraph}`, 
			'Exportar', 
			0, 
			replace
		);
		//Resetea la variable
		if(GraficaEEV.countGraph == 100){
			GraficaEEV.countGraph = 0;
		}else{
			GraficaEEV.countGraph++;
		}
	}
	else{
		$("#mainGraph").empty();
	}
}

/*Evento de click en calendario para alterar la forma en la que se abre el 
calendario si se encuentra en dispositivo mobil*/
GraficaEEV.gestionDOM.calendarMobil = function(){
    //REVISAR, AÚN FALTA
	if((/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent))){
		//Se limita el ancho de la ventana de fechas para dispositivos mobiles
		$("div.daterangepicker.opensright").css("top","0");
	}
}

GraficaEEV.gestionDOM.FechasLimitedRankGraph2 = function(tagDiv){
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
        GraficaEEV.fechaIniRank = start.format('YYYY-MM-DD');
        GraficaEEV.fechaFinRank = end.format('YYYY-MM-DD');
        $(`#${tagDiv} span`).html("Rango Visualización &nbsp; &nbsp; &nbsp; ");
        if($(`#selectFinca`).val().length > 0){
			GraficaEEV.gestionDOM.graph();
        }
    }

    $(`#${tagDiv}`).daterangepicker(option, cb);
    cb(start, end);
}