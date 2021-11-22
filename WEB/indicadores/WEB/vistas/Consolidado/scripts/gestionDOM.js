Consolidado.gestionDOM = new Object();

//Se redibuja la tabla cuando se cambia la seleccion
Consolidado.gestionDOM.genTable = function(){
	//Se limpia el contenedor
	$('#tableConsolidado').empty();

	var txt = `
	<div id="scroller-left" class="scroller scroller-left"><i class="fas fa-angle-left"></i></div>
	<div id="scroller-right" class="scroller scroller-right"><i class="fas fa-angle-right"></i></div>
	<div id="navWrapper" class="wrapper">
		<nav id='nav-wrapper-balance' class="nav nav-tabs list">
			<a class="nav-tabs nav-link active" data-toggle="tab" href="#promedio">Promedio</a>
			<a class="nav-tabs nav-link" data-toggle="tab" href="#parametros">Parametros</a>
		</nav>
	</div>
	<div class="tab-content">
		<div id="promedio" class="tab-pane active">
			<div id="divtabla_1" class="dataTableStyle">		
				<!--para configurar opciones con los data-->
				<!-- Por defecto siempre se organiza por la primera columna asc -->
				<table id="tablaPromedio" class="table-striped table-bordered" data-order='[[ 0, "asc" ]]' data-page-length='100'>
				</table> <!-- el id de la tabla si se puede cambiar -->
			</div>
		</div>
		<div id="parametros" class="tab-pane">
			<div id="divtabla_0" class="dataTableStyle">		
				<!--para configurar opciones con los data-->
				<!-- Por defecto siempre se organiza por la primera columna asc -->
				<table id="tablaParametros" class="table-striped table-bordered" data-order='[[ 0, "asc" ]]' data-page-length='100'>
				</table> <!-- el id de la tabla si se puede cambiar -->
			</div>
		</div>
	</div>
	`;

	//Se dibuja el div de data table
	$('#tableConsolidado').append(txt);

	$("#principal").css("display","none");

	//evento risize cada que se cambia de tabla
	$(".nav-tabs").off('click').on('click', Consolidado.gestionDOM.resize)

	Consolidado.funciones.genTable1();
	Consolidado.funciones.genTable2();
}

/* SELECTORES ANIDADOS */
Consolidado.gestionDOM.initSelectores = function(){
	/* Estilo para selectores */
	gestionBSelect.generic3(`#selectDepartamento`, "#divSelectVista", 'Departamentos');
	gestionBSelect.generic3(`#selectMunicipio`, "#divSelectVista", 'Municipios');
	gestionBSelect.generic3(`#selectFinca`, "#divSelectVista", 'Fincas');

	//Funciones encargadas de llenar los selectores y mover banderas
	$('button[data-id="selectDepartamento"]').click(Consolidado.funciones.fillDepartamento);
	$('#selectDepartamento').change({select:"selectDepartamento"},Consolidado.funciones.changeFlag);
	$('button[data-id="selectMunicipio"]').click(Consolidado.funciones.fillMunicipio);
	$('#selectMunicipio').change({select:"selectMunicipio"},Consolidado.funciones.changeFlag);
	$('button[data-id="selectFinca"]').click(Consolidado.funciones.fillFinca);
	$('#selectFinca').change({select:"selectFinca"},Consolidado.funciones.changeFlag);

	//Se quita el tooltip por defecto de los selectores
	$(`[data-id="selectDepartamento"]`).removeAttr("title");
	$(`[data-id="selectMunicipio"]`).removeAttr("title");
	$(`[data-id="selectFinca"]`).removeAttr("title");
}

//Se dibujan los valores de los selectores
Consolidado.gestionDOM.fillSelectores = function(resultado, id){
    //Si hay selecciones se guardan
    var options = $(id).val();

	var html ="<option value='0' disabled></option>";
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

	//Se cierra el modal
	gestionModal.cerrar();
}
/* FIN SELECTORES ANIDADOS */

//Habilita o deshabilita el boton
Consolidado.gestionDOM.enabledBtn = function(){
	if($(`#selectFinca`).val().length > 0){
		$("#cargar").prop("disabled", false);
	}else{
		$("#cargar").prop("disabled", true);
	}
}

/*Evento de click en calendario para alterar la forma en la que se abre el 
calendario si se encuentra en dispositivo mobil*/
Consolidado.gestionDOM.calendarMobil = function(){
    //REVISAR, AÚN FALTA
	if((/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent))){
		//Se limita el ancho de la ventana de fechas para dispositivos mobiles
		$("div.daterangepicker.opensright").css("top","0");
	}
}

Consolidado.gestionDOM.FechasLimitedRankGraph2 = function(tagDiv){
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
        Consolidado.fechaIniRank = start.format('YYYY-MM-DD');
        Consolidado.fechaFinRank = end.format('YYYY-MM-DD');
        $(`#${tagDiv} span`).html("Rango Visualización &nbsp; &nbsp; &nbsp; ");
    }

    $(`#${tagDiv}`).daterangepicker(option, cb);
    cb(start, end);
}

//ejcuta evento risize a la ventana para ajustar el datatable
Consolidado.gestionDOM.resize = function(){
	$(window).resize();
}

//********* Mensajes de error *************/
Consolidado.gestionDOM.mensajeError = function(tag_id, contenidoHTML){
	$(`${tag_id}`).append(contenidoHTML);	
}