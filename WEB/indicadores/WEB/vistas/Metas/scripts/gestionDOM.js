Metas.gestionDOM = new Object();

/*****************************POPOVER***************************************/
Metas.gestionDOM.popoverOff = function(){
	//Cerrar popover
	$(".popover").popover("dispose");
}

//eventos popover
Metas.gestionDOM.popoverEditar = function(e, dt, type, indexes){
	//Se borra el popover anterior
	Metas.gestionDOM.borrarPopover();
	// Se guardan los datos de la fila de la tabla actualmente seleccionada
	Metas.datosFilaTabla = Metas.$tabla.rows(indexes).data();
	//Se crea el popoever
	var pop = $(this).children("tbody").children("tr").eq(indexes[0]).popover({
		content: `	<a id="delete" class="btn"> 
						<i id="icon" class="fas fa-trash-alt"></i>
					</a>`, 
		html: true, 
		placement: "left",
		animation: true,	
		trigger:"manual"
	});
	//Se posiciona el popover
	Metas.gestionDOM.posicionPopoever(pop);
	//Se muestra el popover
	$(this).children("tbody").children("tr").eq(indexes[0]).popover("show");
	//No se muestra el popver hasta asustar posicion
	$(".popover").css("display","none");
	//Se cierra el popover en los botones de filtrar
	$(".filter-option").off("click").on("click",Metas.gestionDOM.cerrarPopover);
	//Elemento anterior
   	Metas.ultimoElementoConPopoever = $(this).children("tbody").children("tr").eq(indexes[0]);
   	//Se guarda el elemento tabla
   	Metas.tabla = $(this);
   	//Cada que se da click en el contenedor se mira que no tenga nada que ver con el popover y si es asi se oculta
   	$("body").off('click').on('click',Metas.gestionDOM.cerrarPopoverEnClick);
   	//Vuelve a esconder el popover al dar click en la fila otra vez
   	$(".selected").click(Metas.gestionDOM.borrarPopover);
  	//Se definen los eventos de editar y borrar
    if(Metas.baderaPermisosEdicion){
	   //Evento boton editar
	   $("#delete").off("click").on("click", function(){gestionModal.alertaOpciones("Control Fitosanitario",
	   "¿Está seguro que desea eliminar esta meta?",
	   "question",
	   Metas.funciones.deleteMeta)});
    }
    else{ 
        //Se le da el color de button disabled 
        $("#delete").addClass('boton-popover-desabilitado');
        // Tooltip con mensaje para usuario
        $("#delete").tooltip({title: "No posee permisos para Editar", animation: true , placement: "right"}); 
    }
}

Metas.gestionDOM.posicionPopoever = function(pop){
	//Se restan 28px que son la mitad del tamaño del contenedor, como offset
	Metas.top = Metas.eventoSelected.clientY - 28;
	Metas.left = Metas.eventoSelected.clientX + 10;
}

Metas.gestionDOM.cerrarPopoverEnClick = function(e){
	var container = $(`.popover`);
	if(Metas.tabla != undefined){
		if ((!container.is(e.target)) && (!Metas.tabla.children("tbody").is(e.target.parentElement.parentElement)) 
		&& (container.has(e.target).length === 0)) { 
				//Se ha pulsado en cualquier lado fuera de los elementos contenidos en la variable container
				Metas.gestionDOM.borrarPopover();
		}
	}
}

Metas.gestionDOM.borrarPopover = function(){
	$(Metas.ultimoElementoConPopoever).popover("dispose");
}

//Cuando el popover esta generado (pero oculto):
Metas.gestionDOM.popoverFullyShown = function(){
	//Se ajusta la posicion del popover con las cooernadas del click
    $(".popover").css({"transform":`translate(${Metas.left}px,${Metas.top}px)`});
    //Se muetra el popover despues de acomodado
	$(".popover").css("display","block");
	//Se acomoda la flecha del popoever
	$(".arrow").css("top",`${parseInt(Metas.eventoSelected.clientY - ($(".popover").offset().top + 14))}px`);
}

//Borra el popover al reordenar las filas
Metas.gestionDOM.borrarPopoverOnOrder = function(e, settings){
	Metas.gestionDOM.borrarPopover();
}

/*****************************FIN*POPOVER***************************************/

//Se redibuja la tabla cuando se cambia la seleccion
Metas.gestionDOM.genTable = function(){
	//Cerrar popover
	Metas.gestionDOM.popoverOff();

	//Se limpia el contenedor
	$('#tableMetas').empty();

	var txt = `
	<div id="divtabla_0" class="c_shadow rounded dataTableStyle">		
		<!--para configurar opciones con los data-->
		<!-- Por defecto siempre se organiza por la primera columna asc -->
		<table id="tablaMetas" class="table-striped table-bordered" data-order='[[ 0, "asc" ]]' data-page-length='100'>
		   </table> <!-- el id de la tabla si se puede cambiar -->
	</div>
	`;

	//Se dibuja el div de data table
	$('#tableMetas').append(txt);

	//Al clickar cualquier elemento tr dentro de tbody 
    $("#tablaMetas").on("click","tbody tr",Metas.funciones.clickSelected);

	$("#principal").css("display","none");

	Metas.funciones.genTable();
}

//Creacion del formulario
Metas.gestionDOM.htmlForm = function(){
	/* SELECTORES ANIDADOS DE EDADES INDICADORES */
	Metas.filtrosEI = new Object();

	Metas.filtrosEI.edad = new Object();
	Metas.filtrosEI.edad.data = '0'
	Metas.filtrosEI.edad.state = 0;
	Metas.filtrosEI.edad.uso = 0;

	Metas.filtrosEI.indicador = new Object();
	Metas.filtrosEI.indicador.data = '0';
	Metas.filtrosEI.indicador.state = 0;
	Metas.filtrosEI.indicador.uso = 0;
	/* FIN SELECTORES ANIDADOS DE EDADES INDICADORES */
	
	var txt;
	txt =`
	<div class="TituloCategoria">
		<span class="titulo_opcion"></span>
	</div>

	<div class="infoFinca row">
		<div class="container_Select col-lg-4 col-md-6 mb-4">
			<select id="selectEdad" class='selectpicker'>			
				<option disabled></option>
			</select>
			<label>Edad</label>
			<!-- Contenedor para mensajes de error -->
			<div id="invalidFeedback_selectEdad"></div>			
		</div>
		<div class="container_Select col-lg-4 col-md-6 mb-4">
			<select id="selectInfeccion" class='selectpicker'>
				<option disabled></option>
			</select>
			<label>Indicador</label>
			<!-- Contenedor para mensajes de error -->
			<div id="invalidFeedback_selectInfeccion"></div>				
		</div>
		<div class="contenedorInput col-lg-4 col-md-6 mb-4">
			<input type="text" id="meta" required>
			<label>Meta</label>
			<!-- Contenedor para mensajes de error -->
			<div id="invalidFeedback_meta"></div>			
		</div>
	</div>		

	<div class="container_btn_control">
		<button id="cancelar" class="btn btn-md btn-primary">
			<i class="far fa-arrow-alt-circle-left"></i>
			<span>Regresar</span>		      	
		</button>
		<button id="guardarCambios" class="btn btn-md btn-success">
			<i class="far fa-check-circle"></i>
			<span></span>
		</button>
	</div>
	`;

	//Se limpia el contenedor y se dibuja el formulario
	$("#principal").empty();
	$("#principal").append(txt);

	gestionImask.cualquierNumeroEntero("#meta");

	//Aplica el estilo de selectores 
	gestionBSelect.generic3(`#selectEdad`, "#contenedorMetas", '');
	gestionBSelect.generic3(`#selectInfeccion`, "#contenedorMetas", '');

	/* SELECTORES NIDADOS */
	Metas.gestionDOM.initSelectores();
	/* FIN SELECTORES NIDADOS */

	//Se remueven los tooltip de BSelect
    $(`#selectEdad`).removeAttr("title");
    $(`#selectInfeccion`).removeAttr("title");

    //Titulo del formulario y etiqueta del boton
    $('.TituloCategoria .titulo_opcion').html('Agregar Meta');
    $('#guardarCambios span').html('Agregar');

    //Evento para agregar finca
    $('#guardarCambios').off().on("click", Metas.funciones.validacionUsuario);
    
    //Funcion para cancelar agregar
    $('#cancelar').off().on("click", Metas.gestionDOM.cancelar);

	$("#principal").css("display","block");
}

/* SELECTORES ANIDADOS */
Metas.gestionDOM.initSelectores = function(){
	//Funciones encargadas de llenar los selectores y mover banderas (edades indicadores)
	$('button[data-id="selectEdad"]').click(Metas.funciones.fillEdad);
	$('#selectEdad').change({select:"selectEdad"},Metas.funciones.changeFlagEI);
	$('button[data-id="selectInfeccion"]').click(Metas.funciones.fillInfeccion);
	$('#selectInfeccion').change({select:"selectInfeccion"},Metas.funciones.changeFlagEI);

	//Anidados de edades e indicadores
	$(`[data-id="selectEdad"]`).removeAttr("title");
	$(`[data-id="selectInfeccion"]`).removeAttr("title");
}

//Se dibujan los valores de los selectores
Metas.gestionDOM.fillSelectores = function(resultado, id){
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

//Se oculta el formulario
Metas.gestionDOM.cancelar = function(){
	$(".dropdown-menu").hide();
	$("#principal").css("display","none");
	$("#principal").html("");
}

//********* Mensajes de error *************/
Metas.gestionDOM.mensajeError = function(tag_id, contenidoHTML){
		$(`${tag_id}`).append(contenidoHTML);	
}