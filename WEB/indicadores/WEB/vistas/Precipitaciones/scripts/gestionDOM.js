Precipitaciones.gestionDOM = new Object();

/* SELECTORES ANIDADOS */
Precipitaciones.gestionDOM.initSelectores = function(){
	/* Estilo para selectores */
	gestionBSelect.generic3(`#selectDepartamento`, "#divSelectVista", 'Departamentos');
	gestionBSelect.generic3(`#selectMunicipio`, "#divSelectVista", 'Municipios');
	gestionBSelect.generic3(`#selectFinca`, "#divSelectVista", 'Finca', undefined, false);

	//Funciones encargadas de llenar los selectores y mover banderas
	$('button[data-id="selectDepartamento"]').click(Precipitaciones.funciones.fillDepartamento);
	$('#selectDepartamento').change({select:"selectDepartamento"},Precipitaciones.funciones.changeFlag);
	$('button[data-id="selectMunicipio"]').click(Precipitaciones.funciones.fillMunicipio);
	$('#selectMunicipio').change({select:"selectMunicipio"},Precipitaciones.funciones.changeFlag);
	$('button[data-id="selectFinca"]').click(Precipitaciones.funciones.fillFinca);
	$('#selectFinca').change({select:"selectFinca"},Precipitaciones.funciones.changeFlag);

	//Se quita el tooltip por defecto de los selectores
	$(`[data-id="selectDepartamento"]`).removeAttr("title");
	$(`[data-id="selectMunicipio"]`).removeAttr("title");
	$(`[data-id="selectFinca"]`).removeAttr("title");
}

//Se dibujan los valores de los selectores
Precipitaciones.gestionDOM.fillSelectores = function(resultado, id){
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
		Precipitaciones.funciones.precipitaciones();
	}

	//Se cierra el modal
	gestionModal.cerrar();
}
/* FIN SELECTORES ANIDADOS */

//Se dibujan las precipitaciones
Precipitaciones.gestionDOM.precipitaciones = function(resultado){
	var html = `
		<table class="table table-striped table-bordered">
			<thead>
				<tr>
					<th>Día</th>
					<th>Fecha</th>
					<th>Precipitación [mm]</th>
				</tr>
			</thead>
			<tbody>`;

			for(var i = 0; i < resultado.length; i++){
				html = `${html}
				<tr>
					<td>${resultado[i].dias}</td>
					<td>${resultado[i].fecha.substring(0,10)}</td>
					<td>
						<input type="text" class="form-control" id="${resultado[i].id}" value="${resultado[i].valor}" ${resultado[i].editable == 1 ? "" : "disabled"}>
					</td>
				</tr>`;
			}
				html = `${html}
				<tr>
					<td> </td>
					<td><b>Total Sem</b></td>
					<td>
						<input type="text" class="form-control" id="total" disabled>
					</td>
				</tr>
			</tbody>
		</table>
		<div class="d-flex">
			<button id="guardar" type="button" class="btn btn-success ml-auto">Guardar</button>
		</div>
	`;

	$("#precipitaciones").html(html);

	//Se calcula el total
	Precipitaciones.gestionDOM.calcTotal();
	$("#lunes, #martes, #miercoles, #jueves, #viernes, #sabado, #domingo").on("change", Precipitaciones.gestionDOM.calcTotal);

	gestionImask.cualquierNumeroDecimal("#lunes", 0, 0, 999.99, 2);
	gestionImask.cualquierNumeroDecimal("#martes", 0, 0, 999.99, 2);
	gestionImask.cualquierNumeroDecimal("#miercoles", 0, 0, 999.99, 2);
	gestionImask.cualquierNumeroDecimal("#jueves", 0, 0, 999.99, 2);
	gestionImask.cualquierNumeroDecimal("#viernes", 0, 0, 999.99, 2);
	gestionImask.cualquierNumeroDecimal("#sabado", 0, 0, 999.99, 2);
	gestionImask.cualquierNumeroDecimal("#domingo", 0, 0, 999.99, 2);

	//Se guardan las nuevas precipitaciones
	$("#guardar").off("click").on("click", {resultado:resultado}, Precipitaciones.funciones.guardarPrecipitaciones);
}

//Se calcula el total
Precipitaciones.gestionDOM.calcTotal = function(){
	var total = parseFloat($("#lunes").val().replaceAll('.', '').replaceAll(',','.')) + parseFloat($("#martes").val().replaceAll('.', '').replaceAll(',','.')) +
	parseFloat($("#miercoles").val().replaceAll('.', '').replaceAll(',','.')) + parseFloat($("#jueves").val().replaceAll('.', '').replaceAll(',','.')) +
	parseFloat($("#viernes").val().replaceAll('.', '').replaceAll(',','.')) + parseFloat($("#sabado").val().replaceAll('.', '').replaceAll(',','.')) +
	parseFloat($("#domingo").val().replaceAll('.', '').replaceAll(',','.'));
	$("#total").val(Intl.NumberFormat("de-DE").format(total));
}

/*Evento de click en calendario para alterar la forma en la que se abre el 
calendario si se encuentra en dispositivo mobil*/
Precipitaciones.gestionDOM.calendarMobil = function(){
    //REVISAR, AÚN FALTA
	if((/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent))){
		//Se limita el ancho de la ventana de fechas para dispositivos mobiles
		$("div.daterangepicker.opensright").css("top","0");
	}
}

Precipitaciones.gestionDOM.FechasLimitedRankGraph2 = function(tagDiv){
	var start = moment();
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
		showISOWeekNumbers: true,
        startDate: start,
    	alwaysShowCalendars: true,
    	singleDatePicker:true
    }

    function cb(start) {
        Precipitaciones.fechaIniRank = start.format('YYYY-MM-DD');
        $(`#${tagDiv} span`).html("Rango Visualización &nbsp; &nbsp; &nbsp; ");
        if($(`#selectFinca`).val().length > 0){
			Precipitaciones.funciones.precipitaciones();
        }
    }

    $(`#${tagDiv}`).daterangepicker(option, cb);
    cb(start);
}