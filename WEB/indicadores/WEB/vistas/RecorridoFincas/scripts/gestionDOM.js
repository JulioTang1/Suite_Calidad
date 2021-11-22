RecorridoFincas.gestionDOM = new Object();

/*****************************POPOVER***************************************/
RecorridoFincas.gestionDOM.popoverOff = function(){
	//Cerrar popover
	$(".popover").popover("dispose");
}

//eventos popover
RecorridoFincas.gestionDOM.popoverEditar = function(e, dt, type, indexes){
	//Se borra el popover anterior
	RecorridoFincas.gestionDOM.borrarPopover();
	// Se guardan los datos de la fila de la tabla actualmente seleccionada
	RecorridoFincas.datosFilaTabla = RecorridoFincas.$tabla.rows(indexes).data();
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
	RecorridoFincas.gestionDOM.posicionPopoever(pop);
	//Se muestra el popover
	$(this).children("tbody").children("tr").eq(indexes[0]).popover("show");
	//No se muestra el popver hasta asustar posicion
	$(".popover").css("display","none");
	//Se cierra el popover en los botones de filtrar
	$(".filter-option").off("click").on("click",RecorridoFincas.gestionDOM.cerrarPopover);
	//Elemento anterior
   	RecorridoFincas.ultimoElementoConPopoever = $(this).children("tbody").children("tr").eq(indexes[0]);
   	//Se guarda el elemento tabla
   	RecorridoFincas.tabla = $(this);
   	//Cada que se da click en el contenedor se mira que no tenga nada que ver con el popover y si es asi se oculta
   	$("body").off('click').on('click',RecorridoFincas.gestionDOM.cerrarPopoverEnClick);
   	//Vuelve a esconder el popover al dar click en la fila otra vez
   	$(".selected").click(RecorridoFincas.gestionDOM.borrarPopover);
  	//Se definen los eventos de editar y borrar
    if(RecorridoFincas.baderaPermisosEdicion){
	   //Evento boton editar
	   $("#edit").off("click").on("click", RecorridoFincas.funciones.visita);
	   //Evento boton borrrar
	   $("#delete").off("click").on("click", function(){gestionModal.alertaOpciones("Control Fitosanitario",
	   "¿Está seguro que desea eliminar esta visita?",
	   "question",
	   RecorridoFincas.funciones.delete,
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

RecorridoFincas.gestionDOM.posicionPopoever = function(pop){
	//Se restan 28px que son la mitad del tamaño del contenedor, como offset
	RecorridoFincas.top = RecorridoFincas.eventoSelected.clientY - 28;
	RecorridoFincas.left = RecorridoFincas.eventoSelected.clientX + 10;
}

RecorridoFincas.gestionDOM.cerrarPopoverEnClick = function(e){
	var container = $(`.popover`);
	if(RecorridoFincas.tabla != undefined){
		if ((!container.is(e.target)) && (!RecorridoFincas.tabla.children("tbody").is(e.target.parentElement.parentElement)) 
		&& (container.has(e.target).length === 0)) { 
				//Se ha pulsado en cualquier lado fuera de los elementos contenidos en la variable container
				RecorridoFincas.gestionDOM.borrarPopover();
		}
	}
}

RecorridoFincas.gestionDOM.borrarPopover = function(){
	$(RecorridoFincas.ultimoElementoConPopoever).popover("dispose");
}

//Cuando el popover esta generado (pero oculto):
RecorridoFincas.gestionDOM.popoverFullyShown = function(){
	//Se ajusta la posicion del popover con las cooernadas del click
    $(".popover").css({"transform":`translate(${RecorridoFincas.left}px,${RecorridoFincas.top}px)`});
    //Se muetra el popover despues de acomodado
	$(".popover").css("display","block");
	//Se acomoda la flecha del popoever
	$(".arrow").css("top",`${parseInt(RecorridoFincas.eventoSelected.clientY - ($(".popover").offset().top + 14))}px`);
}

//Borra el popover al reordenar las filas
RecorridoFincas.gestionDOM.borrarPopoverOnOrder = function(e, settings){
	RecorridoFincas.gestionDOM.borrarPopover();
}

/*****************************FIN*POPOVER***************************************/

/* SELECTORES ANIDADOS */
RecorridoFincas.gestionDOM.initSelectores = function(){
    /* Estilo para selectores */
    gestionBSelect.generic3(`#selectDepartamento`, "#divSelectVista", 'Departamentos');
    gestionBSelect.generic3(`#selectMunicipio`, "#divSelectVista", 'Municipios');
    gestionBSelect.generic3(`#selectFinca`, "#divSelectVista", 'Fincas');
    gestionBSelect.generic3(`#selectVisita`, "#divSelectVista", 'Visitas');

    //Anidados de edades e indicadores
    gestionBSelect.generic3(`#selectEdad`, "#divSelectVista", 'Edades');
    gestionBSelect.generic3(`#selectInfeccion`, "#divSelectVista", 'Indicadores');

    //Funciones encargadas de llenar los selectores y mover banderas
    $('button[data-id="selectDepartamento"]').click(RecorridoFincas.funciones.fillDepartamento);
    $('#selectDepartamento').change({select:"selectDepartamento"},RecorridoFincas.funciones.changeFlag);
    $('button[data-id="selectMunicipio"]').click(RecorridoFincas.funciones.fillMunicipio);
    $('#selectMunicipio').change({select:"selectMunicipio"},RecorridoFincas.funciones.changeFlag);
    $('button[data-id="selectFinca"]').click(RecorridoFincas.funciones.fillFinca);
    $('#selectFinca').change({select:"selectFinca"},RecorridoFincas.funciones.changeFlag);

	//Funciones encargadas de llenar los selectores y mover banderas (edades indicadores)
	$('button[data-id="selectEdad"]').click(RecorridoFincas.funciones.fillEdad);
	$('#selectEdad').change({select:"selectEdad"},RecorridoFincas.funciones.changeFlagEI);
	$('button[data-id="selectInfeccion"]').click(RecorridoFincas.funciones.fillInfeccion);
	$('#selectInfeccion').change({select:"selectInfeccion"},RecorridoFincas.funciones.changeFlagEI);

    //Se quita el tooltip por defecto de los selectores
    $(`[data-id="selectDepartamento"]`).removeAttr("title");
    $(`[data-id="selectMunicipio"]`).removeAttr("title");
    $(`[data-id="selectFinca"]`).removeAttr("title");
    $(`[data-id="selectVisita"]`).removeAttr("title");

    //Anidados de edades e indicadores
    $(`[data-id="selectEdad"]`).removeAttr("title");
    $(`[data-id="selectInfeccion"]`).removeAttr("title");
}

//Se dibujan los valores de los selectores
RecorridoFincas.gestionDOM.fillSelectores = function(resultado, id){
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

//Se redibuja la tabla cuando se cambia la seleccion
RecorridoFincas.gestionDOM.genTable = function(){
	$("#cargar").prop("disabled", true);
	//Cerrar popover
	RecorridoFincas.gestionDOM.popoverOff();

	//Se limpia el contenedor
	$('#tableRecorridoFincas').empty();

	var txt = `
	<div id="divtabla_0" class="c_shadow rounded dataTableStyle">		
		<!--para configurar opciones con los data-->
		<!-- Por defecto siempre se organiza por la primera columna asc -->
		<table id="tablaRecorridoFincas" class="table-striped table-bordered" data-order='[[ 0, "asc" ]]' data-page-length='100'>
		</table> <!-- el id de la tabla si se puede cambiar -->
	</div>
	`;

	//Se dibuja el div de data table
	$('#tableRecorridoFincas').append(txt);

	//Al clickar cualquier elemento tr dentro de tbody 
	$("#tablaRecorridoFincas").on("click","tbody tr",RecorridoFincas.funciones.clickSelected);

	$("#principal").css("display","none");

	RecorridoFincas.funciones.genTable();
}

//Habilita o deshabilita el boton
RecorridoFincas.gestionDOM.enabledBtn = function(){
	if($(`#selectFinca`).val().length > 0){
		$("#cargar").prop("disabled", false);
	}else{
		$("#cargar").prop("disabled", true);
	}
}

//sale de la visita y vuelve a la tabla
RecorridoFincas.gestionDOM.volver = function(){
	//Se limpia informacion sobre la visita actual
	$('.TituloCategoria .titulo_opcion').css("display", "none");
	$("#titulo_opcion").empty();

	//Se desaparece el navbar
	$("#nav-wrapper-balance").css("display", "none");
	$(".tab-content").css("display", "none");
	$("#navWrapper").css("display", "none");
	
	$("#tableRecorridoFincas").css("display", "block");

	$("#selectEdad").parent().css("display", "none");
	$("#selectInfeccion").parent().css("display", "none");
	$("#volver").css("display", "none");

	$("#calendarPlanning").css("display", "block");
	$("#selectDepartamento").parent().css("display", "block");
	$("#selectMunicipio").parent().css("display", "block");
	$("#selectFinca").parent().css("display", "block");
	$("#cargar").css("display", "block");

	$(window).resize()
}

//Se dibuja la visita en el mapa
RecorridoFincas.gestionDOM.visita = function(resultado){
	$(`#map`).empty();
    var loader = new google.maps.plugins.loader.Loader({
        apiKey: "AIzaSyA1HS-BuQUunSljqBT3unkzR4-X3VEzM2I",
        libraries: ["visualization"]
    });
    
    if (!loader.done) {
        loader.load().then(RecorridoFincas.gestionDOM.initMap(resultado));
    }
    else {
        RecorridoFincas.gestionDOM.initMap(resultado);
    }
}

RecorridoFincas.gestionDOM.initMap = function(resultado){
	try{
		//carga mapa en div
		var map = new google.maps.Map(document.getElementById("map"), {
		center: resultado.RECORRIDO[0],
		zoom: 16,
		mapTypeId: 'satellite' //Tipos de mapas: 'roadmap' (por defecto), 'satellite', 'hybrid', 'terrain'
		});

		//posicionamiento de marca estandar en el mapa
		var miPosicion = new google.maps.Marker({
			position: resultado.RECORRIDO[0],
			title: "Inicio",
			icon: '../../imagenes/markerGreen.png',
			map: map			
		});

		//posicionamiento de marca estandar en el mapa
		var destino = new google.maps.Marker({
			position: resultado.RECORRIDO[resultado.RECORRIDO.length-1],
			title: "Fin",
			icon: '../../imagenes/markerRed.png',
			map: map
		});

		//posicionamiento de perimetro en mapa
		var perimeter = new google.maps.Polyline({
			path: resultado.RECORRIDO,
			geodesic: true,
			strokeColor: "#DAA520",
			strokeOpacity: 1.0,
			strokeWeight: 8,
			map: map
		});

		// YLS YLI
		var circle = new Array();
		for (var i = 0; i < resultado.INDICADOR.length; i++) {
			var coordenada = {
				lat: resultado.INDICADOR[i].lat, 
				lng: resultado.INDICADOR[i].lng
			}

			//Circulos para indicadores sigatoka
			if(resultado.INDICADOR[i].color != null && resultado.INDICADOR[i].semaforo_activo == 1){
				circle[i] = new google.maps.Circle({
					strokeColor: resultado.INDICADOR[i].color,
					strokeOpacity: 0.8,
					strokeWeight: 2,
					fillColor: resultado.INDICADOR[i].color,
					fillOpacity: 0.4,
					map: map,
					center: coordenada,
					radius: resultado.INDICADOR[i].radio_mapa,
					title: resultado.INDICADOR[i].nombre
				});
				circle[i].addListener(
					'mouseover',
					function(){
						this.set('fillOpacity', 0.8);
						this.set('strokeOpacity', 1);
	
						this.getMap()
							.getDiv()						
							.setAttribute('title', this.get('title'));
	
						this.getMap()
							.getDiv()
							.setAttribute('fillOpacity', this.get('fillOpacity'));
	
						this.getMap()
							.getDiv()
							.setAttribute('strokeOpacity', this.get('strokeOpacity'));
					}
				);
				circle[i].addListener(
					'mouseout',
					function(){
						this.set('fillOpacity', 0.4);
						this.set('strokeOpacity', 0.8);
	
						this.getMap()
							.getDiv()
							.removeAttribute('title');
	
						this.getMap()
							.getDiv()
							.setAttribute('fillOpacity', this.get('fillOpacity'));
	
						this.getMap()
							.getDiv()
							.setAttribute('strokeOpacity', this.get('strokeOpacity'));
					}
				);
			}
			//Marcadores para los demas indicadores
			else{
				circle[i] = new google.maps.Marker({
					position: coordenada,
					title: resultado.INDICADOR[i].nombre,
					icon: '../../imagenes/markerYellow.png',
					map: map
				});
			}		
		}

		var circle2 = new Array();
		for (var i = 0; i < resultado.INDICADOR.length; i++) {
			var coordenada = {
				lat: resultado.INDICADOR[i].lat, 
				lng: resultado.INDICADOR[i].lng
			}
			//Circulos para indicadores sigatoka
			if(resultado.INDICADOR[i].color != null && resultado.INDICADOR[i].semaforo_activo == 1){
				circle2[i] = new google.maps.Circle({
					strokeColor: "#000000",
					strokeOpacity: 1,
					strokeWeight: 1,
					fillColor: "#000000",
					fillOpacity: 1,
					map: map,
					center: coordenada,
					radius: 1,
					zIndex: 100
				});
			}
		}

	}catch{
		setTimeout(RecorridoFincas.gestionDOM.visita, 500, resultado);
	}
}

/*Evento de click en calendario para alterar la forma en la que se abre el 
calendario si se encuentra en dispositivo mobil*/
RecorridoFincas.gestionDOM.calendarMobil = function(){
    //REVISAR, AÚN FALTA
	if((/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent))){
		//Se limita el ancho de la ventana de fechas para dispositivos mobiles
		$("div.daterangepicker.opensright").css("top","0");
	}
}

RecorridoFincas.gestionDOM.FechasLimitedRankGraph2 = function(tagDiv){
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
        RecorridoFincas.fechaIniRank = start.format('YYYY-MM-DD');
        RecorridoFincas.fechaFinRank = end.format('YYYY-MM-DD');
        $(`#${tagDiv} span`).html("Rango Visualización &nbsp; &nbsp; &nbsp; ");
    }

    $(`#${tagDiv}`).daterangepicker(option, cb);
    cb(start, end);
}

//Se genera el html de la tabla
RecorridoFincas.gestionDOM.genDatos = function(resultado){
	var html = `
	<div id="divdatos">
	<div id="divdatos_int">
			<div class='c_shadow rounded'>
				<h6 class="text-center">Sigatoka</h6>

				<table id="sigatoka" class="table table-bordered table-striped">
					<thead>
						<tr>
							<th class="text-center">Fecha y Hora</th>
							<th class="text-center">Edad</th>
							<th class="text-center">TH</th>
							<th class="text-center">YLI</th>
							<th class="text-center">YLS</th>
							<th class="text-center">CF</th>
							<th class="text-center">HF</th>
							<th class="text-center">H2</th>
							<th class="text-center">H3</th>
							<th class="text-center">H4</th>
							<th class="text-center">EFA</th>
							<th class="text-center">Lote</th>
						</tr>
					</thead>
					<tbody>`;
					
				for(var i = 0; i < resultado.SIGATOKA.length; i++){
					html = `${html}
					<tr data-id="${resultado.SIGATOKA[i]["id"]}">
						<td class="text-center">${resultado.SIGATOKA[i]["Fecha y Hora"].replaceAll('T', ' ')}</td>
						<td class="text-center">${resultado.SIGATOKA[i]["Edad"]}</td>
						<td class="text-center">${resultado.SIGATOKA[i]["TH"] == null ? "" : resultado.SIGATOKA[i]["TH"]}</td>
						<td class="text-center">${resultado.SIGATOKA[i]["YLI"] == null ? "" : resultado.SIGATOKA[i]["YLI"]}</td>
						<td class="text-center">${resultado.SIGATOKA[i]["YLS"] == null ? "" : resultado.SIGATOKA[i]["YLS"]}</td>
						<td class="text-center" data-indicador="${resultado.SIGATOKA[i]["CF"]}">${resultado.SIGATOKA[i]["CF"] == 0 ? "" : "<i class='fas fa-camera'></i>"}</td>
						<td class="text-center">${resultado.SIGATOKA[i]["HF"] == null ? "" : resultado.SIGATOKA[i]["HF"]}</td>
						<td class="text-center">${resultado.SIGATOKA[i]["H2"] == null ? "" : resultado.SIGATOKA[i]["H2"]}</td>
						<td class="text-center">${resultado.SIGATOKA[i]["H3"] == null ? "" : resultado.SIGATOKA[i]["H3"]}</td>
						<td class="text-center">${resultado.SIGATOKA[i]["H4"] == null ? "" : resultado.SIGATOKA[i]["H4"]}</td>
						<td class="text-center">${resultado.SIGATOKA[i]["EFA"] == null ? "" : resultado.SIGATOKA[i]["EFA"]}</td>
						<td class="text-center">${resultado.SIGATOKA[i]["Lote"] == null ? "" : resultado.SIGATOKA[i]["Lote"]}</td>
					</tr>`;
				}

			html = `${html}
					</tbody>
				</table>
			</div>

			<div class='c_shadow rounded'>
				<h6 class="text-center">Enfermedades vasculares</h6>

				<table id="vasculares" class="table table-bordered table-striped">
					<thead>
						<tr>
							<th class="text-center">Fecha y Hora</th>
							<th class="text-center">Fusarium</th>
							<th class="text-center">Moko</th>
							<th class="text-center">Erwinia</th>
						</tr>
					</thead>
					<tbody>`;
					
					for(var i = 0; i < resultado.VASCULARES.length; i++){
						html = `${html}
						<tr data-id="${resultado.VASCULARES[i]["id"]}">
							<td class="text-center">${resultado.VASCULARES[i]["Fecha y Hora"].replaceAll('T', ' ')}</td>
							<td class="text-center">${resultado.VASCULARES[i]["Fusarium"]}</td>
							<td class="text-center">${resultado.VASCULARES[i]["Moko"]}</td>
							<td class="text-center">${resultado.VASCULARES[i]["Erwinia"]}</td>
						</tr>`;
					}
		
				html = `${html}
					</tbody>
				</table>
			</div>

			<div class='c_shadow rounded'>
				<h6 class="text-center">Condiciones culturales</h6>

				<table id="culturales" class="table table-bordered table-striped">
					<thead>
						<tr>
							<th class="text-center">Fecha y Hora</th>
							<th class="text-center">NF</th>
							<th class="text-center">Foto NF</th>
							<th class="text-center">FIT</th>
							<th class="text-center">Foto FIT</th>
							<th class="text-center">Comentario FIT</th>
							<th class="text-center">RTI</th>
							<th class="text-center">Comentario RTI</th>
						</tr>
					</thead>
					<tbody>`;
					
					for(var i = 0; i < resultado.CULTURALES.length; i++){
						html = `${html}
						<tr data-id="${resultado.CULTURALES[i]["id"]}">
							<td class="text-center">${resultado.CULTURALES[i]["Fecha y Hora"].replaceAll('T', ' ')}</td>
							<td class="text-center">${resultado.CULTURALES[i]["NF"]}</td>
							<td class="text-center" data-indicador="12"><i class='fas fa-camera'></i></td>
							<td class="text-center">${resultado.CULTURALES[i]["FIT"]}</td>
							<td class="text-center" data-indicador="13"><i class='fas fa-camera'></i></td>
							<td class="text-center">${resultado.CULTURALES[i]["Comentario FIT"] == null ? "" : resultado.CULTURALES[i]["Comentario FIT"]}</td>
							<td class="text-center">${resultado.CULTURALES[i]["RTI"]}</td>
							<td class="text-center">${resultado.CULTURALES[i]["Comentario RTI"] == null ? "" : resultado.CULTURALES[i]["Comentario RTI"]}</td>
						</tr>`;
					}
		
				html = `${html}
					</tbody>
				</table>
			</div>
		</div>
	</div>`;

	$("#tablas").html(html);

	//Evento de la camara
	$(".fa-camera").off("click").on("click", RecorridoFincas.funciones.camara);
}

//Se muestra informacion sobre la visita actual
RecorridoFincas.gestionDOM.infoVisita = function(){
    $('.TituloCategoria .titulo_opcion').html(`Finca: ${RecorridoFincas.datosFilaTabla[0]["Nombre Finca"]} - Fecha: ${RecorridoFincas.datosFilaTabla[0].Fecha.replaceAll('T', ' ')}`);
	$('.TituloCategoria .titulo_opcion').css("display", "flex");
}

//Se muestran las imagenes
RecorridoFincas.gestionDOM.camara = function(resultado){
	var html;
	if(resultado.length == 1){
		html = `
		<div id="demo" class="carousel slide" data-ride="carousel">

			<!-- Indicators -->
			<ul class="carousel-indicators">
				<li data-target="#demo" data-slide-to="0" class="active"></li>
			</ul>

			<!-- The slideshow -->
			<div class="carousel-inner" style="max-width: 30vw">
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
			<div class="carousel-inner" style="max-width: 30vw">
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
			<div class="carousel-inner" style="max-width: 30vw">
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
	//Modificacion de estilo a modal de sweet alert
	$(".swal2-modal").css("width","unset");
}

//Se muestran las imagenes de CF
RecorridoFincas.gestionDOM.camaraCF = function(resultado){
	var html;
	if(resultado.length == 1){
		html = `
		<div id="demo" class="carousel slide" data-ride="carousel">

			<!-- Indicators -->
			<ul class="carousel-indicators">
				<li data-target="#demo" data-slide-to="0" class="active"></li>
			</ul>

			<!-- The slideshow -->
			<div class="carousel-inner" style="max-width: 30vw">
				<div class="carousel-item active">
					<img src="${resultado[0].url}" width="auto" style="height: CALC(100vh - 100px)">
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
			<div class="carousel-inner" style="max-width: 30vw">
				<div class="carousel-item active">
					<img src="${resultado[0].url}" width="auto" style="height: CALC(100vh - 100px)">
				</div>
				<div class="carousel-item">
					<img src="${resultado[1].url}" width="auto" style="height: CALC(100vh - 100px)">
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
			<div class="carousel-inner" style="max-width: 30vw">
				<div class="carousel-item active">
					<img src="${resultado[0].url}" width="auto" style="height: CALC(100vh - 100px)">
				</div>
				<div class="carousel-item">
					<img src="${resultado[1].url}" width="auto" style="height: CALC(100vh - 100px)">
				</div>
				<div class="carousel-item">
					<img src="${resultado[2].url}" width="auto" style="height: CALC(100vh - 100px)">
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
	//Modificacion de estilo a modal de sweet alert
	$(".swal2-modal").css("width","unset");
}