Nuevo.gestionDOM = new Object();

/* SELECTORES ANIDADOS */
Nuevo.gestionDOM.initSelectores = function(){
	/* Estilo para selectores */
	gestionBSelect.generic3(`#selectDepartamento`, "#divSelectVista", 'Departamentos');
	gestionBSelect.generic3(`#selectMunicipio`, "#divSelectVista", 'Municipios');
	gestionBSelect.generic3(`#selectFinca`, "#divSelectVista", 'Finca', undefined, false);
	gestionBSelect.generic3(`#selectVisita`, "#divSelectVista", 'Visita');

	//Funciones encargadas de llenar los selectores y mover banderas
	$('button[data-id="selectDepartamento"]').click(Nuevo.funciones.fillDepartamento);
	$('#selectDepartamento').change({select:"selectDepartamento"},Nuevo.funciones.changeFlag);
	$('button[data-id="selectMunicipio"]').click(Nuevo.funciones.fillMunicipio);
	$('#selectMunicipio').change({select:"selectMunicipio"},Nuevo.funciones.changeFlag);
	$('button[data-id="selectFinca"]').click(Nuevo.funciones.fillFinca);
	$('#selectFinca').change({select:"selectFinca"},Nuevo.funciones.changeFlag);

	//Se quita el tooltip por defecto de los selectores
	$(`[data-id="selectDepartamento"]`).removeAttr("title");
	$(`[data-id="selectMunicipio"]`).removeAttr("title");
	$(`[data-id="selectFinca"]`).removeAttr("title");
	$(`[data-id="selectVisita"]`).removeAttr("title");
}

//Se dibujan los valores de los selectores
Nuevo.gestionDOM.fillSelectores = function(resultado, id){
    //Si hay selecciones se guardan
    var options = $(id).val();

	var html = resultado.length > 0 ? "" : "<option value='0' disabled></option>";
	for(var i = 0; i < resultado.length; i++){
		if(resultado[i].id != null){
			html = `${html}
			<option value="${resultado[i].id}">${resultado[i].name}</option>`;
		}
	}

	//SE SELECCIONA LA VISTA O NUEVO INMEDIATAMENTE
	if( id == "#selectVisita" && resultado.length == 1){
		options = resultado[0].id;
	}

	$(id).html(html);
	$(id).selectpicker("refresh");
	$(id).val(options)
	$(id).selectpicker("refresh");

	//Se genera la grafica si las opciones de finca cambiaron
	if( id == "#selectVisita" && (options != $(id).val()) ){
		Nuevo.funciones.datos();
	}

	//Se cierra el modal
	gestionModal.cerrar();
}
/* FIN SELECTORES ANIDADOS */

//Se dibujan los datos de recorrido
Nuevo.gestionDOM.datos = function(resultado){
	var html = `

		<div id="formInterno" class="d-flex  flex-wrap">
			<div>
				<input autocomplete="off" type="text" required placeholder=" " id="precipitacion"
					value="${resultado.PRECIPITACION[0] != undefined ? (resultado.PRECIPITACION[0].valor == null ? "" : resultado.PRECIPITACION[0].valor) : ""}">
				<label>Precipitación D (-1)</label>
			</div>
			<div>
				<input autocomplete="off" type="text" required placeholder=" " id="temperaturaMinima"
					value="${resultado.TEMPERATURAS[0] != undefined ? (resultado.TEMPERATURAS[0].temperatura_minima == null ? "" : resultado.TEMPERATURAS[0].temperatura_minima) : ""}">
				<label>Temperatura Mínima</label>
			</div>
			<div>
				<input autocomplete="off" type="text" required placeholder=" " id="temperatura"
					value="${resultado.TEMPERATURAS[0] != undefined ? (resultado.TEMPERATURAS[0].temperatura == null ? "" : resultado.TEMPERATURAS[0].temperatura) : ""}">
				<label>Temperatura</label>
				<div id="invalidFeedback_precipitacion"></div>
			</div>
			<div>
				<input autocomplete="off" type="text" required placeholder=" " id="temperaturaMaxima"
					value="${resultado.TEMPERATURAS[0] != undefined ? (resultado.TEMPERATURAS[0].temperatura_maxima == null ? "" : resultado.TEMPERATURAS[0].temperatura_maxima) : ""}">
				<label>Temperatura Máxima</label>
			</div>
			<div>
				<input autocomplete="off" type="text" required placeholder=" " id="humedad"
					value="${resultado.TEMPERATURAS[0] != undefined ? (resultado.TEMPERATURAS[0].humedad_relativa == null ? "" : resultado.TEMPERATURAS[0].humedad_relativa) : ""}">
				<label>Humedad Relativa</label>
			</div>
		</div>

		<div id="scroller-left" class="scroller scroller-left"><i class="fas fa-angle-left"></i></div>
		<div id="scroller-right" class="scroller scroller-right"><i class="fas fa-angle-right"></i></div>
		<div id="navWrapper" class="wrapper">
			<nav id='nav-wrapper-balance' class="nav nav-tabs list">
				<a class="nav-tabs nav-link active" data-toggle="tab" href="#pxp">Próxima a Parir</a>
				<a class="nav-tabs nav-link" data-toggle="tab" href="#viisem">7 Semanas</a>
				<a class="nav-tabs nav-link" data-toggle="tab" href="#xsem">10 Semanas</a>
				<a class="nav-tabs nav-link" data-toggle="tab" href="#pfsem">Parcela Fija</a>
			</nav>
		</div>
		<div class="tab-content">

			<div id="pxp" class="tab-pane active">
				<table class="table table-striped table-bordered">
					<thead>
						<tr>
							<th>Planta No</th>
							<th>TH</th>
							<th>YLI</th>
							<th>YLS</th>
							<th>CF</th>
							<th>Lote</th>
						</tr>
					</thead>
					<tbody>`;
					for(var i = 1; i <= 15; i++){
						html = `${html}
						<tr class="row-${i}"
							data-id="${resultado.PP[i-1] != undefined ? (resultado.PP[i-1].ID == null ? "" : resultado.PP[i-1].ID) : ""}">
							<td>
								${i}
							</td>
							<td class="th">
								<input id="pxp_th_${i}" type="text" class="form-control"
									value="${resultado.PP[i-1] != undefined ? (resultado.PP[i-1].TH == null ? "" : resultado.PP[i-1].TH) : ""}">
							</td>
							<td class="yli">
								<input id="pxp_yli_${i}" type="text" class="form-control"
									value="${resultado.PP[i-1] != undefined ? (resultado.PP[i-1].YLI == null ? "" : resultado.PP[i-1].YLI) : ""}">
							</td>
							<td class="yls">
								<input id="pxp_yls_${i}" type="text" class="form-control"
									value="${resultado.PP[i-1] != undefined ? (resultado.PP[i-1].YLS == null ? "" : resultado.PP[i-1].YLS) : ""}">
							</td>
							<td class="cf">
								<i id="pxp_cf_${i}" class='fas fa-camera' 
									data-url="${resultado.PP[i-1] != undefined ? (resultado.PP[i-1].CF == null ? "" : resultado.PP[i-1].CF) : ""}"
									style="${resultado.PP[i-1] != undefined ? (resultado.PP[i-1].CF == null ? "" : (resultado.PP[i-1].CF != "" && resultado.PP[i-1].CF != "0" ? "color: #046d4d": "")) : ""}"></i>
							</td>
							<td class="lote">
								<input id="pxp_lote_${i}" type="text" class="form-control" maxlength="250"
									value="${resultado.PP[i-1] != undefined ? (resultado.PP[i-1].Lote == null ? "" : resultado.PP[i-1].Lote) : ""}">
							</td>
						</tr>`;
					}
						html = `${html}
					</tbody>
				</table>
			</div>

			<div id="viisem" class="tab-pane">
				<table class="table table-striped table-bordered">
					<thead>
						<tr>
							<th>Planta No</th>
							<th>YLS</th>
							<th>CF</th>
							<th>HF</th>
							<th>Lote</th>
						</tr>
					</thead>
					<tbody>`;
					for(var i = 1; i <= 15; i++){
						html = `${html}
						<tr class="row-${i}"
							data-id="${resultado.VIISEM[i-1] != undefined ? (resultado.VIISEM[i-1].ID == null ? "" : resultado.VIISEM[i-1].ID) : ""}">
							<td>
								${i}
							</td>
							<td class="yls">
								<input id="viisem_yls_${i}" type="text" class="form-control"
									value="${resultado.VIISEM[i-1] != undefined ? (resultado.VIISEM[i-1].YLS == null ? "" : resultado.VIISEM[i-1].YLS) : ""}">
							</td>
							<td class="cf">
								<i id="viisem_cf_${i}" class='fas fa-camera' 
									data-url="${resultado.VIISEM[i-1] != undefined ? (resultado.VIISEM[i-1].CF == null ? "" : resultado.VIISEM[i-1].CF) : ""}"
									style="${resultado.VIISEM[i-1] != undefined ? (resultado.VIISEM[i-1].CF == null ? "" : (resultado.VIISEM[i-1].CF != "" && resultado.VIISEM[i-1].CF != "0" ? "color: #046d4d": "")) : ""}"></i>
							</td>
							<td class="hf">
								<input id="viisem_hf_${i}" type="text" class="form-control"
									value="${resultado.VIISEM[i-1] != undefined ? (resultado.VIISEM[i-1].HF == null ? "" : resultado.VIISEM[i-1].HF) : ""}">
							</td>
							<td class="lote">
								<input id="viisem_lote_${i}" type="text" class="form-control" maxlength="250"
									value="${resultado.VIISEM[i-1] != undefined ? (resultado.VIISEM[i-1].Lote == null ? "" : resultado.VIISEM[i-1].Lote) : ""}">
							</td>
						</tr>`;
					}
						html = `${html}
					</tbody>
				</table>
			</div>

			<div id="xsem" class="tab-pane">
				<table class="table table-striped table-bordered">
					<thead>
						<tr>
							<th>Planta No</th>
							<th>YLS</th>
							<th>CF</th>
							<th>HF</th>
							<th>Lote</th>
						</tr>
					</thead>
					<tbody>`;
					for(var i = 1; i <= 15; i++){
						html = `${html}
						<tr class="row-${i}"
							data-id="${resultado.XSEM[i-1] != undefined ? (resultado.XSEM[i-1].ID == null ? "" : resultado.XSEM[i-1].ID) : ""}">
							<td>
								${i}
							</td>
							<td class="yls">
								<input id="xsem_yls_${i}" type="text" class="form-control"
									value="${resultado.XSEM[i-1] != undefined ? (resultado.XSEM[i-1].YLS == null ? "" : resultado.XSEM[i-1].YLS) : ""}">
							</td>
							<td class="cf">
								<i id="xsem_cf_${i}" class='fas fa-camera'
									data-url="${resultado.XSEM[i-1] != undefined ? (resultado.XSEM[i-1].CF == null ? "" : resultado.XSEM[i-1].CF) : ""}"
									style="${resultado.XSEM[i-1] != undefined ? (resultado.XSEM[i-1].CF == null ? "" : (resultado.XSEM[i-1].CF != "" && resultado.XSEM[i-1].CF != "0" ? "color: #046d4d": "")) : ""}"></i>
							</td>
							<td class="hf">
								<input id="xsem_hf_${i}" type="text" class="form-control"
									value="${resultado.XSEM[i-1] != undefined ? (resultado.XSEM[i-1].HF == null ? "" : resultado.XSEM[i-1].HF) : ""}">
							</td>
							<td class="lote">
								<input id="xsem_lote_${i}" type="text" class="form-control" maxlength="250"
									value="${resultado.XSEM[i-1] != undefined ? (resultado.XSEM[i-1].Lote == null ? "" : resultado.XSEM[i-1].Lote) : ""}">
							</td>
						</tr>`;
					}
						html = `${html}
					</tbody>
				</table>
			</div>

			<div id="pfsem" class="tab-pane">
				<table class="table table-striped table-bordered">
					<thead>
						<tr>
							<th>Planta No</th>
							<th>TH</th>
							<th>EFA</th>
							<th>CF</th>
							<th>H2</th>
							<th>H3</th>
							<th>H4</th>
							<th>Lote</th>
						</tr>
					</thead>
					<tbody>`;
					for(var i = 1; i <= 15; i++){
						html = `${html}
						<tr class="row-${i}"
							data-id="${resultado.PF[i-1] != undefined ? (resultado.PF[i-1].ID == null ? "" : resultado.PF[i-1].ID) : ""}">
							<td>
								${i}
							</td>
							<td class="th">
								<input id="pfsem_th_${i}" type="text" class="form-control"
									value="${resultado.PF[i-1] != undefined ? (resultado.PF[i-1].TH == null ? "" : resultado.PF[i-1].TH) : ""}">
							</td>
							<td class="efa">
								<input id="pfsem_efa_${i}" type="text" class="form-control"
									value="${resultado.PF[i-1] != undefined ? (resultado.PF[i-1].EFA == null ? "" : resultado.PF[i-1].EFA) : ""}">
							</td>
							<td class="cf">
								<i id="pfsem_cf_${i}" class='fas fa-camera'
									data-url="${resultado.PF[i-1] != undefined ? (resultado.PF[i-1].CF == null ? "" : resultado.PF[i-1].CF) : ""}"
									style="${resultado.PF[i-1] != undefined ? (resultado.PF[i-1].CF == null ? "" : (resultado.PF[i-1].CF != "" && resultado.PF[i-1].CF != "0" ? "color: #046d4d": "")) : ""}"></i>
							</td>
							<td class="h2">
								<select id="pfsem_h2_${i}" class="selectpicker">
									<option ${resultado.PF[i-1] != undefined ? (resultado.PF[i-1].H2 == null ? "" : (resultado.PF[i-1].H2 == "0" ? "selected" : "")) : ""} value="0">0</option>
									<option ${resultado.PF[i-1] != undefined ? (resultado.PF[i-1].H2 == null ? "" : (resultado.PF[i-1].H2 == "2" ? "selected" : "")) : ""} value="2">1-</option>
									<option ${resultado.PF[i-1] != undefined ? (resultado.PF[i-1].H2 == null ? "" : (resultado.PF[i-1].H2 == "1" ? "selected" : "")) : ""} value="1">1+</option>
									<option ${resultado.PF[i-1] != undefined ? (resultado.PF[i-1].H2 == null ? "" : (resultado.PF[i-1].H2 == "4" ? "selected" : "")) : ""} value="4">2-</option>
									<option ${resultado.PF[i-1] != undefined ? (resultado.PF[i-1].H2 == null ? "" : (resultado.PF[i-1].H2 == "3" ? "selected" : "")) : ""} value="3">2+</option>
									<option ${resultado.PF[i-1] != undefined ? (resultado.PF[i-1].H2 == null ? "" : (resultado.PF[i-1].H2 == "6" ? "selected" : "")) : ""} value="6">3-</option>
									<option ${resultado.PF[i-1] != undefined ? (resultado.PF[i-1].H2 == null ? "" : (resultado.PF[i-1].H2 == "5" ? "selected" : "")) : ""} value="5">3+</option>
								</select>
							</td>
							<td class="h3">
								<select id="pfsem_h3_${i}" class="selectpicker">
									<option ${resultado.PF[i-1] != undefined ? (resultado.PF[i-1].H3 == null ? "" : (resultado.PF[i-1].H3 == "0" ? "selected" : "")) : ""} value="0">0</option>
									<option ${resultado.PF[i-1] != undefined ? (resultado.PF[i-1].H3 == null ? "" : (resultado.PF[i-1].H3 == "2" ? "selected" : "")) : ""} value="2">1-</option>
									<option ${resultado.PF[i-1] != undefined ? (resultado.PF[i-1].H3 == null ? "" : (resultado.PF[i-1].H3 == "1" ? "selected" : "")) : ""} value="1">1+</option>
									<option ${resultado.PF[i-1] != undefined ? (resultado.PF[i-1].H3 == null ? "" : (resultado.PF[i-1].H3 == "4" ? "selected" : "")) : ""} value="4">2-</option>
									<option ${resultado.PF[i-1] != undefined ? (resultado.PF[i-1].H3 == null ? "" : (resultado.PF[i-1].H3 == "3" ? "selected" : "")) : ""} value="3">2+</option>
									<option ${resultado.PF[i-1] != undefined ? (resultado.PF[i-1].H3 == null ? "" : (resultado.PF[i-1].H3 == "6" ? "selected" : "")) : ""} value="6">3-</option>
									<option ${resultado.PF[i-1] != undefined ? (resultado.PF[i-1].H3 == null ? "" : (resultado.PF[i-1].H3 == "5" ? "selected" : "")) : ""} value="5">3+</option>
								</select>
							</td>
							<td class="h4">
								<select id="pfsem_h4_${i}" class="selectpicker">
									<option ${resultado.PF[i-1] != undefined ? (resultado.PF[i-1].H4 == null ? "" : (resultado.PF[i-1].H4 == "0" ? "selected" : "")) : ""} value="0">0</option>
									<option ${resultado.PF[i-1] != undefined ? (resultado.PF[i-1].H4 == null ? "" : (resultado.PF[i-1].H4 == "2" ? "selected" : "")) : ""} value="2">1-</option>
									<option ${resultado.PF[i-1] != undefined ? (resultado.PF[i-1].H4 == null ? "" : (resultado.PF[i-1].H4 == "1" ? "selected" : "")) : ""} value="1">1+</option>
									<option ${resultado.PF[i-1] != undefined ? (resultado.PF[i-1].H4 == null ? "" : (resultado.PF[i-1].H4 == "4" ? "selected" : "")) : ""} value="4">2-</option>
									<option ${resultado.PF[i-1] != undefined ? (resultado.PF[i-1].H4 == null ? "" : (resultado.PF[i-1].H4 == "3" ? "selected" : "")) : ""} value="3">2+</option>
									<option ${resultado.PF[i-1] != undefined ? (resultado.PF[i-1].H4 == null ? "" : (resultado.PF[i-1].H4 == "6" ? "selected" : "")) : ""} value="6">3-</option>
									<option ${resultado.PF[i-1] != undefined ? (resultado.PF[i-1].H4 == null ? "" : (resultado.PF[i-1].H4 == "5" ? "selected" : "")) : ""} value="5">3+</option>
								</select>
							</td>
							<td class="lote">
								<input id="pfsem_lote_${i}" type="text" class="form-control"  maxlength="250"
									value="${resultado.PF[i-1] != undefined ? (resultado.PF[i-1].Lote == null ? "" : resultado.PF[i-1].Lote) : ""}">
							</td>
						</tr>`;
					}
						html = `${html}
					</tbody>
				</table>
			</div>

		</div>
		<div class="d-flex">
			<button id="guardar" type="button" class="btn btn-success ml-auto">Guardar</button>
		</div>
	`;

	$("#main").html(html);
	$("#main").removeClass("spinnerPosition");

	//Imask
	gestionImask.cualquierNumeroDecimal(`#precipitacion`, 0, 0, 999.99, 2);
	gestionImask.cualquierNumeroDecimal(`#temperaturaMinima`, 0, 0, 999.99, 2);
	gestionImask.cualquierNumeroDecimal(`#temperatura`, 0, 0, 999.99, 2);
	gestionImask.cualquierNumeroDecimal(`#temperaturaMaxima`, 0, 0, 999.99, 2);
	gestionImask.cualquierNumeroDecimal(`#humedad`, 0, 0, 999.99, 2);

	for(var i = 1; i <= 15; i++){
		gestionImask.cualquierNumeroDecimal(`#pxp_th_${i}`, 0, 0, 999.99, 2);
		gestionImask.cualquierNumeroDecimal(`#pxp_yli_${i}`, 0, 0, 999.99, 2);
		gestionImask.cualquierNumeroDecimal(`#pxp_yls_${i}`, 0, 0, 999.99, 2);

		gestionImask.cualquierNumeroDecimal(`#viisem_yls_${i}`, 0, 0, 999.99, 2);
		gestionImask.cualquierNumeroDecimal(`#viisem_hf_${i}`, 0, 0, 999.99, 2);

		gestionImask.cualquierNumeroDecimal(`#xsem_yls_${i}`, 0, 0, 999.99, 2);
		gestionImask.cualquierNumeroDecimal(`#xsem_hf_${i}`, 0, 0, 999.99, 2);

		gestionImask.cualquierNumeroDecimal(`#pfsem_th_${i}`, 0, 0, 999.99, 2);
		gestionImask.cualquierNumeroDecimal(`#pfsem_efa_${i}`, 0, 0, 999.99, 2);

		//Aplica el estilo de selectores 
		gestionBSelect.generic3(`#pfsem_h2_${i}`,'#main'," ");
		gestionBSelect.generic3(`#pfsem_h3_${i}`,'#main'," ");
		gestionBSelect.generic3(`#pfsem_h4_${i}`,'#main'," ");
	}

	//Evento de la camara
	$(".fa-camera").off("click").on("click", Nuevo.gestionDOM.camara);

	$(window).off('resize').on('resize', Nuevo.gestionDOM.reAdjust);

	//desplaza el navbar a la derecha
	$(`#scroller-right`).off('click').on('click', Nuevo.gestionDOM.scrollerRight);
	//desplaza el navbar a la izquierda
	$(`#scroller-left`).off('click').on('click', Nuevo.gestionDOM.scrollerLeft);

	Nuevo.gestionDOM.reAdjust();

	//Se guardan las datos
	$("#guardar").off("click").on("click", Nuevo.funciones.guardar);
}

//Se muestran las imagenes
Nuevo.gestionDOM.camara = function(){
	html = `
	<div id="demo" class="carousel slide" data-ride="carousel">

		<!-- Indicators -->
		<ul class="carousel-indicators">
			<li data-target="#demo" data-slide-to="0" class="${$(this).data("url") == "" || $(this).data("url") == "1" ? "active" : ""}"></li>
			<li data-target="#demo" data-slide-to="1" class="${$(this).data("url") == "2" ? "active" : ""}"></li>
			<li data-target="#demo" data-slide-to="2" class="${$(this).data("url") == "3" ? "active" : ""}"></li>
			<li data-target="#demo" data-slide-to="3" class="${$(this).data("url") == "4" ? "active" : ""}"></li>
		</ul>

		<!-- The slideshow -->
		<div class="carousel-inner" style="max-width: 30vw">
			<div class="carousel-item ${$(this).data("url") == "" || $(this).data("url") == "1" ? "active" : ""}">
				<img data-img="1" src="vistas/Nuevo/imagenes/CF_ideal.jpeg" height="auto" style="width: 30vw">
			</div>
			<div class="carousel-item ${$(this).data("url") == "2" ? "active" : ""}">
				<img data-img="2" src="vistas/Nuevo/imagenes/CF_Sintomas_Arrepollamiento.jpeg" height="auto" style="width: 30vw">
			</div>
			<div class="carousel-item ${$(this).data("url") == "3" ? "active" : ""}">
				<img data-img="3" src="vistas/Nuevo/imagenes/CF_Sintomas_Arrepollamiento_2.jpeg" height="auto" style="width: 30vw">
			</div>
			<div class="carousel-item ${$(this).data("url") == "4" ? "active" : ""}">
				<img data-img="4" src="vistas/Nuevo/imagenes/CF_Sintomas_Arrepollamiento_3.jpeg" height="auto" style="width: 30vw">
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

	//Modificacion de estilo a modal de sweet alert
	$(".swal2-modal").css("width","unset");

	//Se guarda el id de la imagen en el data url y se pinta el borde de la imagen
	$("img").off("click").on("click", {id: $(this).prop("id")}, Nuevo.gestionDOM.imgSelected);

	//Se marca la imagen seleccionada previamente
	$(`[data-img="${$(this).data("url")}"]`).css("border", "6px solid #007BFF");
}

//Se guarda el id de la imagen en el data url y se pinta el borde de la imagen
Nuevo.gestionDOM.imgSelected = function(e){
	var id = e.data.id;

	$('[data-img="1"]').css("border", "none");
	$('[data-img="2"]').css("border", "none");
	$('[data-img="3"]').css("border", "none");
	$('[data-img="4"]').css("border", "none");

	$(`#${id}`).css("color", "#046d4d");
	$(`#${id}`).data("url", $(this).data("img"));
	$(this).css("border", "6px solid #007BFF");
}

/*Esta funcion ajusta el navbar despues de moverse como tabn ajusta el legend y el navbar si se mueve 
el ancho de la pantalla. tambien se encarga del ancho de cada culumna de filtros o .section
Solo esta enfocada al modo de uso de una grafica por link*/
Nuevo.gestionDOM.reAdjust = function(){
	//navbar
	if ( !(Nuevo.gestionDOM.widthOfList()-$(`#main`).width()-(-1*Nuevo.gestionDOM.getLeftPosi()) < 1) ) {
		$(`#scroller-right`).fadeIn(0);
	}
	else {
		$(`#scroller-right`).fadeOut(0);
	}

	if (Nuevo.gestionDOM.getLeftPosi() < 0) {
		$(`#scroller-left`).fadeIn(0);
	}
	else {
	$(`.wrapper nav .item`).animate({left:"-="+Nuevo.gestionDOM.getLeftPosi()+"px"},0);
		$(`#scroller-left`).fadeOut(0);
	}
}

//Funcion que retorna el ancho de la lista navbar
Nuevo.gestionDOM.widthOfList = function(){
	var itemsWidth = 0;
	$(`.wrapper .list a`).each(function(){
		var itemWidth = $(this).outerWidth();
		itemsWidth+=itemWidth;
	});
	return itemsWidth;
}

//Funcion que retorna el ancho de la lista navbar
Nuevo.gestionDOM.widthOfList = function(){
	var itemsWidth = 0;
	$(`.wrapper .list a`).each(function(){
		var itemWidth = $(this).outerWidth();
		itemsWidth+=itemWidth;
	});
	return itemsWidth;
}

//funcion que retorna la posicion a la izquierda del navbar (inicialmente es cero por que no se ha movido a la derecha)
Nuevo.gestionDOM.getLeftPosi = function(){
	return $(`.wrapper .list`).length > 0 ? $(`.wrapper .list`).position().left : 0;
}

/*Scroll horizontal*/
//Evento de desplazamiento del navbar a la izquierda
Nuevo.gestionDOM.scrollerRight = function(e) {
	//variable de cantidad a desplazar en el scroll horizontal
	var cant;
	cant = Nuevo.gestionDOM.widthOfList()-$(`#main`).width()-(-1*Nuevo.gestionDOM.getLeftPosi());
	if( cant >= 300){
		$(`#scroller-right`).off('click');
		$(`.wrapper .list`).animate({left:'+=-300px'},300);
		window.setTimeout("Nuevo.gestionDOM.onScrollerRight()", 350);
	}
	else{
		$(`#scroller-right`).off('click');
		$(`.wrapper .list`).animate({left:`+=-${cant+20}px`},300);
		window.setTimeout("Nuevo.gestionDOM.onScrollerRight()", 350);
	}
}

//Ajusta la pantalla segun corresponda despues del evento de mover navbar a la derecha
Nuevo.gestionDOM.onScrollerRight = function(){
	$(`#scroller-right`).off('click').on('click', Nuevo.gestionDOM.scrollerRight);
	Nuevo.gestionDOM.reAdjust();
}

//Evento de desplazamiento del navbar a la derecha
Nuevo.gestionDOM.scrollerLeft = function(e) {
	//variable de cantidad a desplazar en el scroll horizontal
	var cant;
	if (Nuevo.gestionDOM.getLeftPosi() < 0) {
		cant = -1*Nuevo.gestionDOM.getLeftPosi();
		if( cant > 300 ){
			$(`#scroller-left`).off('click');
			$(`.wrapper .list`).animate({left:'+=300px'},300);
			window.setTimeout("Nuevo.gestionDOM.onScrollerLeft()", 350);
		}
		else{
			$(`#scroller-left`).off('click');
			$(`.wrapper .list`).animate({left:`+=${cant}px`},300);
			window.setTimeout("Nuevo.gestionDOM.onScrollerLeft()", 350);
		}
	}
}

//Ajusta la pantalla segun corresponda despues del evento de mover navbar a la izquierda
Nuevo.gestionDOM.onScrollerLeft = function(){
	$(`#scroller-left`).off('click').on('click', Nuevo.gestionDOM.scrollerLeft);
	Nuevo.gestionDOM.reAdjust();
}
/*Scroll horizontal fin*/

Nuevo.gestionDOM.FechasLimitedRankGraph2 = function(tagDiv){
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
        Nuevo.fechaIniRank = start.format('YYYY-MM-DD');
        $(`#${tagDiv} span`).html("Rango Visualización &nbsp; &nbsp; &nbsp; ");
        if($(`#selectVisita`).val().length > 0){
			Nuevo.funciones.bringVisita();
        }
    }

    $(`#${tagDiv}`).daterangepicker(option, cb);
    cb(start);
}

/*Evento de click en calendario para alterar la forma en la que se abre el 
calendario si se encuentra en dispositivo mobil*/
Nuevo.gestionDOM.calendarMobil = function(){
    //REVISAR, AÚN FALTA
	if((/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent))){
		//Se limita el ancho de la ventana de fechas para dispositivos mobiles
		$("div.daterangepicker.opensright").css("top","0");
	}
}