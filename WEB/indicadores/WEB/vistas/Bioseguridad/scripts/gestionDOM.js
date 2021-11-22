Bioseguridad.gestionDOM = new Object();

/*****************************POPOVER***************************************/
Bioseguridad.gestionDOM.popoverOff = function(){
	//Cerrar popover
	$(".popover").popover("dispose");
}

//eventos popover
Bioseguridad.gestionDOM.popoverEditar = function(e, dt, type, indexes){
	//Se borra el popover anterior
	Bioseguridad.gestionDOM.borrarPopover();
	// Se guardan los datos de la fila de la tabla actualmente seleccionada
	Bioseguridad.datosFilaTabla = Bioseguridad.$tabla.rows(indexes).data();
	//Se crea el popoever
	var pop = $(this).children("tbody").children("tr").eq(indexes[0]).popover({
		content: `	<a id="edit" class="btn"> 
						<i id="icon" class="fas fa-eye"></i>
					</a>
					<a id="delete" class="btn"> 
						<i id="icon" class="fas fa-trash-alt"></i>
					</a>`, 
		html: true, 
		placement: "left",
		animation: true,	
		trigger:"manual"
	});
	//Se posiciona el popover
	Bioseguridad.gestionDOM.posicionPopoever(pop);
	//Se muestra el popover
	$(this).children("tbody").children("tr").eq(indexes[0]).popover("show");
	//No se muestra el popver hasta asustar posicion
	$(".popover").css("display","none");
	//Se cierra el popover en los botones de filtrar
	$(".filter-option").off("click").on("click",Bioseguridad.gestionDOM.cerrarPopover);
	//Elemento anterior
   	Bioseguridad.ultimoElementoConPopoever = $(this).children("tbody").children("tr").eq(indexes[0]);
   	//Se guarda el elemento tabla
   	Bioseguridad.tabla = $(this);
   	//Cada que se da click en el contenedor se mira que no tenga nada que ver con el popover y si es asi se oculta
   	$("body").off('click').on('click',Bioseguridad.gestionDOM.cerrarPopoverEnClick);
   	//Vuelve a esconder el popover al dar click en la fila otra vez
   	$(".selected").click(Bioseguridad.gestionDOM.borrarPopover);
  	//Se definen los eventos de editar y borrar
    if(Bioseguridad.baderaPermisosEdicion){
	   //Evento boton editar
	   $("#edit").off("click").on("click", Bioseguridad.funciones.visita);
		//Evento boton borrrar
		$("#delete").off("click").on("click", function(){gestionModal.alertaOpciones("Control Fitosanitario",
		"¿Está seguro que desea eliminar esta visita?",
		"question",
		Bioseguridad.funciones.delete,
		function(){})});
    }
    else{ 
        //Se le da el color de button disabled 
        $("#edit").addClass('boton-popover-desabilitado');
        // Tooltip con mensaje para usuario
        $("#edit").tooltip({title: "No posee permisos para Editar", animation: true , placement: "right"});
		
		//Se le da el color de button disabled 
		$("#delete").addClass('boton-popover-desabilitado');
		// Tooltip con mensaje para usuario
		$("#delete").tooltip({title: "No posee permisos para Editar", animation: true , placement: "right"}); 
    }
}

Bioseguridad.gestionDOM.posicionPopoever = function(pop){
	//Se restan 28px que son la mitad del tamaño del contenedor, como offset
	Bioseguridad.top = Bioseguridad.eventoSelected.clientY - 28;
	Bioseguridad.left = Bioseguridad.eventoSelected.clientX + 10;
}

Bioseguridad.gestionDOM.cerrarPopoverEnClick = function(e){
	var container = $(`.popover`);
	if(Bioseguridad.tabla != undefined){
		if ((!container.is(e.target)) && (!Bioseguridad.tabla.children("tbody").is(e.target.parentElement.parentElement)) 
		&& (container.has(e.target).length === 0)) { 
				//Se ha pulsado en cualquier lado fuera de los elementos contenidos en la variable container
				Bioseguridad.gestionDOM.borrarPopover();
		}
	}
}

Bioseguridad.gestionDOM.borrarPopover = function(){
	$(Bioseguridad.ultimoElementoConPopoever).popover("dispose");
}

//Cuando el popover esta generado (pero oculto):
Bioseguridad.gestionDOM.popoverFullyShown = function(){
	//Se ajusta la posicion del popover con las cooernadas del click
    $(".popover").css({"transform":`translate(${Bioseguridad.left}px,${Bioseguridad.top}px)`});
    //Se muetra el popover despues de acomodado
	$(".popover").css("display","block");
	//Se acomoda la flecha del popoever
	$(".arrow").css("top",`${parseInt(Bioseguridad.eventoSelected.clientY - ($(".popover").offset().top + 14))}px`);
}

//Borra el popover al reordenar las filas
Bioseguridad.gestionDOM.borrarPopoverOnOrder = function(e, settings){
	Bioseguridad.gestionDOM.borrarPopover();
}

/*****************************FIN*POPOVER***************************************/

//Se redibuja la tabla cuando se cambia la seleccion
Bioseguridad.gestionDOM.genTable = function(){
	if($(`#selectFinca`).val().length > 0){
		$("#cargar").prop("disabled", true);
		//Cerrar popover
		Bioseguridad.gestionDOM.popoverOff();

		//Se limpia el contenedor
		$('#tableBioseguridad').empty();

		var txt = `
		<div id="divtabla_0" class="c_shadow rounded dataTableStyle">		
			<!--para configurar opciones con los data-->
			<!-- Por defecto siempre se organiza por la primera columna asc -->
			<table id="tablaBioseguridad" class="table-striped table-bordered" data-order='[[ 0, "asc" ]]' data-page-length='100'>
			</table> <!-- el id de la tabla si se puede cambiar -->
		</div>
		`;

		//Se dibuja el div de data table
		$('#tableBioseguridad').append(txt);

		//Al clickar cualquier elemento tr dentro de tbody 
		$("#tablaBioseguridad").on("click","tbody tr",Bioseguridad.funciones.clickSelected);

		$("#principal").css("display","none");

		Bioseguridad.funciones.genTable();
	}
	else{
		//Cerrar popover
		Bioseguridad.gestionDOM.popoverOff();

		//Se limpia el contenedor
		$('#tableBioseguridad').empty();
	}
}

/* SELECTORES ANIDADOS */
Bioseguridad.gestionDOM.initSelectores = function(){
	/* Estilo para selectores */
	gestionBSelect.generic3(`#selectDepartamento`, "#divSelectVista", 'Departamentos');
	gestionBSelect.generic3(`#selectMunicipio`, "#divSelectVista", 'Municipios');
	gestionBSelect.generic3(`#selectFinca`, "#divSelectVista", 'Fincas');

	//Funciones encargadas de llenar los selectores y mover banderas
	$('button[data-id="selectDepartamento"]').click(Bioseguridad.funciones.fillDepartamento);
	$('#selectDepartamento').change({select:"selectDepartamento"},Bioseguridad.funciones.changeFlag);
	$('button[data-id="selectMunicipio"]').click(Bioseguridad.funciones.fillMunicipio);
	$('#selectMunicipio').change({select:"selectMunicipio"},Bioseguridad.funciones.changeFlag);
	$('button[data-id="selectFinca"]').click(Bioseguridad.funciones.fillFinca);
	$('#selectFinca').change({select:"selectFinca"},Bioseguridad.funciones.changeFlag);

	//Se quita el tooltip por defecto de los selectores
	$(`[data-id="selectDepartamento"]`).removeAttr("title");
	$(`[data-id="selectMunicipio"]`).removeAttr("title");
	$(`[data-id="selectFinca"]`).removeAttr("title");
}

//Se dibujan los valores de los selectores
Bioseguridad.gestionDOM.fillSelectores = function(resultado, id){
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

//Se dibuja la visita de bioseguridad
Bioseguridad.gestionDOM.visita = function(resultado){

	var html = `<div class='c_shadow rounded'>
		<h6 class="text-center">Componente de infraestructura</h6>

		<table class="table table-bordered table-striped">
			<thead>
				<tr>
					<th class="text-center">#</th>
					<th class="text-center">Pregunta</th>
					<th class="text-center">Respuesta</th>
					<th class="text-center">Foto</th>
				</tr>
			</thead>
			<tbody>
	`;
		for(var i = 0; i < 14; i++){
			html = `${html}
			<tr data-id="${resultado[i].id}">
				<td class="text-center"><b>${resultado[i].numero}</b></td>
				<td>${resultado[i].pregunta}</td>
				<td class="text-center"><i>${resultado[i].respuesta}</i></td>
				<td class="text-center">${resultado[i].foto == 1 ? "<i class='fas fa-camera'></i>" : ""}</td>
			</tr>
			`;
		}
	html = `${html}
			</tbody>
		</table>
	</div>`;

	html = `${html}<div class='c_shadow rounded'>
		<h6 class="text-center">Componente operativo</h6>

		<table class="table table-bordered table-striped">
			<thead>
				<tr>
					<th class="text-center">#</th>
					<th class="text-center">Pregunta</th>
					<th class="text-center">Respuesta</th>
					<th class="text-center">Foto</th>
				</tr>
			</thead>
			<tbody>
	`;
		for(var i = 14; i < 21; i++){
			html = `${html}
			<tr data-id="${resultado[i].id}">
				<td class="text-center"><b>${resultado[i].numero}</b></td>
				<td>${resultado[i].pregunta}</td>
				<td class="text-center"><i>${resultado[i].respuesta}</i></td>
				<td class="text-center">${resultado[i].foto == 1 ? "<i class='fas fa-camera'></i>" : ""}</td>
			</tr>
			`;
		}
			html = `${html}
			</tbody>
		</table>
	</div>`;

	html = `${html}<div class='c_shadow rounded'>
		<h6 class="text-center">Componente químico</h6>

		<table class="table table-bordered table-striped">
			<thead>
				<tr>
					<th class="text-center">#</th>
					<th class="text-center">Pregunta</th>
					<th class="text-center">Respuesta</th>
					<th class="text-center">Foto</th>
				</tr>
			</thead>
			<tbody>
	`;
		for(var i = 21; i < 25; i++){
			html = `${html}
			<tr data-id="${resultado[i].id}">
				<td class="text-center"><b>${resultado[i].numero}</b></td>
				<td>${resultado[i].pregunta}</td>
				<td class="text-center"><i>${resultado[i].respuesta}</i></td>
				<td class="text-center">${resultado[i].foto == 1 ? "<i class='fas fa-camera'></i>" : ""}</td>
			</tr>
			`;
		}
			html = `${html}
			</tbody>
		</table>
	</div>`;

	html = `${html}<div class='c_shadow rounded'>
		<h6 class="text-center">Componente informativo</h6>

		<table class="table table-bordered table-striped">
			<thead>
				<tr>
					<th class="text-center">#</th>
					<th class="text-center">Pregunta</th>
					<th class="text-center">Respuesta</th>
					<th class="text-center">Foto</th>
				</tr>
			</thead>
			<tbody>
	`;
		for(var i = 25; i < 30; i++){
			html = `${html}
			<tr data-id="${resultado[i].id}">
				<td class="text-center"><b>${resultado[i].numero}</b></td>
				<td>${resultado[i].pregunta}</td>
				<td class="text-center"><i>${resultado[i].respuesta}</i></td>
				<td class="text-center">${resultado[i].foto == 1 ? "<i class='fas fa-camera'></i>" : ""}</td>
			</tr>
			`;
		}
			html = `${html}
			</tbody>
		</table>
	</div>`;

	html = `${html}<div class='c_shadow rounded'>
			<p><b>${resultado[30].pregunta}:</b> ${resultado[30].respuesta}</p>
	</div>`;

	html = `${html}<div class='c_shadow rounded'>
			<p><b>${resultado[31].pregunta}:</b> ${resultado[31].respuesta}</p>
	</div>`;

	html = `${html}<div class='c_shadow rounded'>
			<p><b>${resultado[32].pregunta}:</b> ${resultado[32].respuesta}</p>
	</div>
	
	<div class="divBoton">
		<button id="volver" class="btn btn-md btn-primary">
			<i class="far fa-arrow-alt-circle-left"></i>
			<span>Regresar</span>		      	
		</button>
	</div>
	`;

	$("#main").html(html);
	
	//volver
	$("#volver").off("click").on("click", Bioseguridad.gestionDOM.volver);

	//Evento de la camara
	$(".fa-camera").off("click").on("click", Bioseguridad.funciones.camara);
}

//sale de la visita y vuelve a la tabla
Bioseguridad.gestionDOM.volver = function(){
	$("#main").css("display", "none");
	$("#tableBioseguridad").css("display", "block");
	$("#divSelectVista").attr("style", "display: flex!important");
}

/*Evento de click en calendario para alterar la forma en la que se abre el 
calendario si se encuentra en dispositivo mobil*/
Bioseguridad.gestionDOM.calendarMobil = function(){
    //REVISAR, AÚN FALTA
	if((/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent))){
		//Se limita el ancho de la ventana de fechas para dispositivos mobiles
		$("div.daterangepicker.opensright").css("top","0");
	}
}

//Habilita o deshabilita el boton
Bioseguridad.gestionDOM.enabledBtn = function(){
	if($(`#selectFinca`).val().length > 0){
		$("#cargar").prop("disabled", false);
	}else{
		$("#cargar").prop("disabled", true);
	}
}

//Se muestran las imagenes
Bioseguridad.gestionDOM.camara = function(resultado){
	var html;
	if(resultado.length == 1){
		html = `
		<div id="demo" class="carousel slide" data-ride="carousel">

			<!-- Indicators -->
			<ul class="carousel-indicators">
				<li data-target="#demo" data-slide-to="0" class="active"></li>
			</ul>

			<!-- The slideshow -->
			<div class="carousel-inner">
				<div class="carousel-item active">
					<img src="SERVER/Fotos/${resultado[0].url}" width="auto" style="height: CALC(100vh - 100px)">
				</div>
			</div>

		</div>
		`;
		swal({
			html:html,
			showCloseButton: true,
			showConfirmButton: false,
			allowOutsideClick: false
		});
	}
	else if(resultado.length == 2){
		html = `
		<div id="demo" class="carousel slide" data-ride="carousel">

			<!-- Indicators -->
			<ul class="carousel-indicators">
				<li data-target="#demo" data-slide-to="0" class="active"></li>
				<li data-target="#demo" data-slide-to="1"></li>
			</ul>

			<!-- The slideshow -->
			<div class="carousel-inner">
				<div class="carousel-item active">
					<img src="SERVER/Fotos/${resultado[0].url}" width="auto" style="height: CALC(100vh - 100px)">
				</div>
				<div class="carousel-item">
					<img src="SERVER/Fotos/${resultado[1].url}" width="auto" style="height: CALC(100vh - 100px)">
				</div>
			</div>

			<!-- Left and right controls -->
			<a class="carousel-control-prev" href="#demo" data-slide="prev">
				<span class="carousel-control-prev-icon"></span>
			</a>
			<a class="carousel-control-next" href="#demo" data-slide="next">
				<span class="carousel-control-next-icon"></span>
			</a>
		</div>
		`;
		swal({
			html:html,
			showCloseButton: true,
			showConfirmButton: false,
			allowOutsideClick: false
		});
	}
	else if(resultado.length == 3){
		html = `
		<div id="demo" class="carousel slide" data-ride="carousel">

			<!-- Indicators -->
			<ul class="carousel-indicators">
				<li data-target="#demo" data-slide-to="0" class="active"></li>
				<li data-target="#demo" data-slide-to="1"></li>
				<li data-target="#demo" data-slide-to="2"></li>
			</ul>

			<!-- The slideshow -->
			<div class="carousel-inner">
				<div class="carousel-item active">
					<img src="SERVER/Fotos/${resultado[0].url}" width="auto" style="height: CALC(100vh - 100px)">
				</div>
				<div class="carousel-item">
					<img src="SERVER/Fotos/${resultado[1].url}" width="auto" style="height: CALC(100vh - 100px)">
				</div>
				<div class="carousel-item">
					<img src="SERVER/Fotos/${resultado[2].url}" width="auto" style="height: CALC(100vh - 100px)">
				</div>
			</div>

			<!-- Left and right controls -->
			<a class="carousel-control-prev" href="#demo" data-slide="prev">
				<span class="carousel-control-prev-icon"></span>
			</a>
			<a class="carousel-control-next" href="#demo" data-slide="next">
				<span class="carousel-control-next-icon"></span>
			</a>
		</div>
		`;
		swal({
			html:html,
			showCloseButton: true,
			showConfirmButton: false,
			allowOutsideClick: false
		});
	}
}

Bioseguridad.gestionDOM.FechasLimitedRankGraph2 = function(tagDiv){
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
        Bioseguridad.fechaIniRank = start.format('YYYY-MM-DD');
        Bioseguridad.fechaFinRank = end.format('YYYY-MM-DD');
        $(`#${tagDiv} span`).html("Rango Visualización &nbsp; &nbsp; &nbsp; ");
    }

    $(`#${tagDiv}`).daterangepicker(option, cb);
    cb(start, end);
}