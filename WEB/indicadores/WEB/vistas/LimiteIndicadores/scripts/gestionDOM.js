LimiteIndicadores.gestionDOM = new Object();

/* SELECTORES ANIDADOS */
//Inicializa los selectores
LimiteIndicadores.gestionDOM.initSelectores = function(){
	gestionBSelect.generic3(`#selectEdad`, "#divSelectVista", 'Edades', undefined, false);
    gestionBSelect.generic3(`#selectInfeccion`, "#divSelectVista", 'Indicadores', undefined, false);
    gestionBSelect.generic3(`#selectTipoFinca`, "#divSelectVista", 'Tipo Finca', undefined, false);

   	//Funciones encargadas de llenar los selectores y mover banderas (edades indicadores)
	$('button[data-id="selectEdad"]').click(LimiteIndicadores.funciones.fillEdad);
	$('#selectEdad').change({select:"selectEdad"},LimiteIndicadores.funciones.changeFlagEI);
	$('button[data-id="selectInfeccion"]').click(LimiteIndicadores.funciones.fillInfeccion);
	$('#selectInfeccion').change({select:"selectInfeccion"},LimiteIndicadores.funciones.changeFlagEI);
	$('button[data-id="selectTipoFinca"]').click(LimiteIndicadores.funciones.fillTipoFinca);
	$('#selectTipoFinca').change({select:"selectTipoFinca"},LimiteIndicadores.funciones.changeFlagEI);

	$(`[data-id="selectEdad"]`).removeAttr("title");
	$(`[data-id="selectInfeccion"]`).removeAttr("title");
    $(`[data-id="selectTipoFinca"]`).removeAttr("title");
}

//Se dibujan los valores de los selectores
LimiteIndicadores.gestionDOM.fillSelectores = function(resultado, id){
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

	//Se cierra el modal
	gestionModal.cerrar();
}
/* FIN SELECTORES ANIDADOS */

//Se dibuja el html de la vista
LimiteIndicadores.gestionDOM.info = function(resultado){
    if(resultado.length > 0){
        var html = `
            <table class="table table-bordered table-striped">
                <thead>
                    <tr>
                        <th class="text-center">Rango</th>
                        <th class="text-center">Mínimo</th>
                        <th class="text-center">Máximo</th>
                        <th class="text-center">Color</th>
                    </tr>
                </thead>
                <tbody>`;
            for(var i = 0; i < resultado.length; i++){
                html = `${html}
                    <tr>
                        <td class="text-center">${resultado[i].orden}</td>
                        <td class="text-center">${resultado[i].min}</td>
                        <td class="text-center">${resultado[i].max}</td>
                        <td>
                            <input type="color" class="form-control" value="${resultado[i].color}" disabled>
                        </td>
                    </tr>
                `;
            }
            html = `${html}
                </tbody>
            </table>
        `;
        $("#divTabla").html(html);


        html = `
            <div class="selector ">
                <div class="price-slider">
                    <div id="slider-range" class="ui-slider ui-corner-all ui-slider-horizontal ui-widget ui-widget-content">
                        <div class="ui-slider-range ui-corner-all ui-widget-header"></div>
                        <span tabindex="0" class="ui-slider-handle ui-corner-all ui-state-default"></span>
                        <span tabindex="0" class="ui-slider-handle ui-corner-all ui-state-default"></span>
                    </div>
                    <div class="botones d-flex">
                        <div class="izq">
                            <button id="menosIzq" type="button" class="btn btn-info"><i class="fas fa-minus"></i></button>
                            <button id="masIzq" type="button" class="btn btn-info"><i class="fas fa-plus"></i></button>
                        </div>
                        <div class="der">
                            <button id="menosDer" type="button" class="btn btn-info"><i class="fas fa-minus"></i></i></button>
                            <button id="masDer" type="button" class="btn btn-info"><i class="fas fa-plus"></i></button>
                        </div>
                    </div>
                    <div class="limite">
                        <span id="min-price" class="slider-price">
                            <span class="lim-text">Límite Inferior:</span>
                            <span class="lim-val"> ${resultado[1].min}</span>
                        </span>
                        <span class="seperator">-</span>
                        <span id="max-price" data-max="30" class="slider-price">
                            <span class="lim-text">Límite Superior:</span>
                            <span class="lim-val"> ${resultado[1].max}</span>
                        </span>
                    </div>
                </div> 
            </div>
            <div class="selector ">
                <div class="price-slider">
                    <div id="slider-radio" class="ui-slider ui-corner-all ui-slider-horizontal ui-widget ui-widget-content">
                        <div class="ui-slider-range ui-corner-all ui-widget-header"></div>
                        <span tabindex="0" class="ui-slider-handle ui-corner-all ui-state-default"></span>
                    </div>
                    <div class="limite">
                        <span id="radio" class="slider-price">
                            <span class="lim-text">Radio:</span>
                            <span class="lim-val"> ${resultado[1].radio_mapa}</span>
                        </span>
                    </div>
                </div> 
            </div>
            <div class="containerBtn">
                <button id="guardarCambios" class="btn btn-md btn-success">
                    <i class="far fa-check-circle"></i>
                    <span>Guardar</span>
                </button>
            </div>
        `;
        $("#divRango").html(html);

        //Guarda los cambios en base de datos
        $("#guardarCambios").off("click").on("click", LimiteIndicadores.funciones.save);

        $("#slider-range").slider({
            range: true, 
            min: 0,
            max: 30,
            step: 0.01,
            values: [resultado[1].min, resultado[1].max],
            slide: function( event, ui ) {
            $( "#min-price").html(`<span class="lim-text">Límite Inferior:</span><span class="lim-val" span> ${ui.values[ 0 ]}</span>`);
            $( "#max-price").html(`<span class="lim-text">Límite Superior:</span><span class="lim-val"> ${ui.values[ 1 ]}</span>`);
            }
        });

        $("#slider-radio").slider({
            range: "max",       
            min: 10,
            max: 500,
            step: 10,
            value: resultado[1].radio_mapa,
            slide: function( event, ui ) {
            $( "#radio").html(`<span class="lim-text">Radio:</span><span class="lim-val" span> ${ui.value}</span>`);
            }
        });

        $("#menosIzq").off("click").on("click", LimiteIndicadores.gestionDOM.menosIzq);
        $("#masIzq").off("click").on("click", LimiteIndicadores.gestionDOM.masIzq);
        $("#menosDer").off("click").on("click", LimiteIndicadores.gestionDOM.menosDer);
        $("#masDer").off("click").on("click", LimiteIndicadores.gestionDOM.masDer);

        html = `
            <div class="row">
                <div class="col-6">
                    <label for="color1">Color 1:</label>
                </div>
                <div class="col-6">
                    <input type="color" class="form-control" id="color1" value="${resultado[0].color}">
                </div>
            </div>
            <div class="row">
                <div class="col-6">
                    <label for="color2">Color 2:</label>
                </div>
                <div class="col-6">
                    <input type="color" class="form-control" id="color2" value="${resultado[1].color}">
                </div>
            </div>
            <div class="row">
                <div class="col-6">
                    <label for="color3">Color 3:</label>
                </div>
                <div class="col-6">
                    <input type="color" class="form-control" id="color3" value="${resultado[2].color}">
                </div>
            </div>
        `;
        $("#divColor").html(html);

        //Se muestran los colores y la tabla
        $(".LimiteIndicadoresExterno").css("display", "block");
    }
    else{
        //Se ocultan los colores y la tabla
        $(".LimiteIndicadoresExterno").css("display", "none");

        gestionModal.alertaConfirmacion(
            "BANASAN",
            "No se encuentra un tablero de configuracion para la combinacion de edad e infeccion seleccionada.",
            "info",
            "OK",
            undefined,
            function(){})
    }

    //Se muestra el checkbox y se pone la opcion que trae de base de datos
    $(".oculto").removeClass("oculto");
    $("#semaforoActivo").prop("checked", resultado[0].semaforo_activo == 1 ? true: false);
}

//Se mueve el slider izquierdo 1 step en direccion izquierda
LimiteIndicadores.gestionDOM.menosIzq = function(){
    var left = $("#slider-range").slider("values")[0];
    var newLeft = Math.round(($("#slider-range").slider("values")[0] - 0.01 + Number.EPSILON) * 100) / 100;
    var right = $("#slider-range").slider("values")[1];
    if(left > 0){
        $("#slider-range").slider("values", [newLeft, right]);
        $(".lim-val").eq(0).html(newLeft);
    }
}

//Se mueve el slider izquierdo 1 step en direccion derecha
LimiteIndicadores.gestionDOM.masIzq = function(){
    var left = $("#slider-range").slider("values")[0];
    var newLeft = Math.round(($("#slider-range").slider("values")[0] + 0.01 + Number.EPSILON) * 100) / 100;
    var right = $("#slider-range").slider("values")[1];
    if(left < right){
        $("#slider-range").slider("values", [newLeft, right]);
        $(".lim-val").eq(0).html(newLeft);
    }
}

//Se mueve el slider derecho 1 step en direccion izquierda
LimiteIndicadores.gestionDOM.menosDer = function(){
    var left = $("#slider-range").slider("values")[0];
    var right = $("#slider-range").slider("values")[1];
    var newRight = Math.round(($("#slider-range").slider("values")[1] - 0.01 + Number.EPSILON) * 100) / 100;
    if(left < right){
        $("#slider-range").slider("values", [left, newRight]);
        $(".lim-val").eq(1).html(newRight);
    }
}

//Se mueve el slider derecho 1 step en direccion derecha
LimiteIndicadores.gestionDOM.masDer = function(){
    var left = $("#slider-range").slider("values")[0];
    var right = $("#slider-range").slider("values")[1];
    var newRight = Math.round(($("#slider-range").slider("values")[1] + 0.01 + Number.EPSILON) * 100) / 100;
    if(right < 30){
        $("#slider-range").slider("values", [left, newRight]);
        $(".lim-val").eq(1).html(newRight);
    }
}
