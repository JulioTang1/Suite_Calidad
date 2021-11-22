CoeficientesSeveridad.gestionDOM = new Object();

/*****************************POPOVER***************************************/
CoeficientesSeveridad.gestionDOM.popoverOff = function(){
	//Cerrar popover
	$(".popover").popover("dispose");
}

//eventos popover
CoeficientesSeveridad.gestionDOM.popoverEditar = function(e, dt, type, indexes){
	//Se borra el popover anterior
	CoeficientesSeveridad.gestionDOM.borrarPopover();
	// Se guardan los datos de la fila de la tabla actualmente seleccionada
	CoeficientesSeveridad.datosFilaTabla = CoeficientesSeveridad.$tabla.rows(indexes).data();
	//Se crea el popoever
	var pop = $(this).children("tbody").children("tr").eq(indexes[0]).popover({
		content: `	<a id="edit" class="btn"> 
						<i id="icon" class="fas fa-edit"></i>
					</a>`, 
		html: true, 
		placement: "left",
		animation: true,	
		trigger:"manual"
	});
	//Se posiciona el popover
	CoeficientesSeveridad.gestionDOM.posicionPopoever(pop);
	//Se muestra el popover
	$(this).children("tbody").children("tr").eq(indexes[0]).popover("show");
	//No se muestra el popver hasta asustar posicion
	$(".popover").css("display","none");
	//Se cierra el popover en los botones de filtrar
	$(".filter-option").off("click").on("click",CoeficientesSeveridad.gestionDOM.cerrarPopover);
	//Elemento anterior
   	CoeficientesSeveridad.ultimoElementoConPopoever = $(this).children("tbody").children("tr").eq(indexes[0]);
   	//Se guarda el elemento tabla
   	CoeficientesSeveridad.tabla = $(this);
   	//Cada que se da click en el contenedor se mira que no tenga nada que ver con el popover y si es asi se oculta
   	$("body").off('click').on('click',CoeficientesSeveridad.gestionDOM.cerrarPopoverEnClick);
   	//Vuelve a esconder el popover al dar click en la fila otra vez
   	$(".selected").click(CoeficientesSeveridad.gestionDOM.borrarPopover);
  	//Se definen los eventos de editar y borrar
    if(CoeficientesSeveridad.baderaPermisosEdicion){
	   //Evento boton editar
	   $("#edit").off("click").on("click", CoeficientesSeveridad.gestionDOM.htmlFormEdit);
    }
    else{ 
        //Se le da el color de button disabled 
        $("#edit").addClass('boton-popover-desabilitado');
        // Tooltip con mensaje para usuario
        $("#edit").tooltip({title: "No posee permisos para Editar", animation: true , placement: "right"}); 
    }
}

CoeficientesSeveridad.gestionDOM.posicionPopoever = function(pop){
	//Se restan 28px que son la mitad del tamaño del contenedor, como offset
	CoeficientesSeveridad.top = CoeficientesSeveridad.eventoSelected.clientY - 28;
	CoeficientesSeveridad.left = CoeficientesSeveridad.eventoSelected.clientX + 10;
}

CoeficientesSeveridad.gestionDOM.cerrarPopoverEnClick = function(e){
	var container = $(`.popover`);
	if(CoeficientesSeveridad.tabla != undefined){
		if ((!container.is(e.target)) && (!CoeficientesSeveridad.tabla.children("tbody").is(e.target.parentElement.parentElement)) 
		&& (container.has(e.target).length === 0)) { 
				//Se ha pulsado en cualquier lado fuera de los elementos contenidos en la variable container
				CoeficientesSeveridad.gestionDOM.borrarPopover();
		}
	}
}

CoeficientesSeveridad.gestionDOM.borrarPopover = function(){
	$(CoeficientesSeveridad.ultimoElementoConPopoever).popover("dispose");
}

//Cuando el popover esta generado (pero oculto):
CoeficientesSeveridad.gestionDOM.popoverFullyShown = function(){
	//Se ajusta la posicion del popover con las cooernadas del click
    $(".popover").css({"transform":`translate(${CoeficientesSeveridad.left}px,${CoeficientesSeveridad.top}px)`});
    //Se muetra el popover despues de acomodado
	$(".popover").css("display","block");
	//Se acomoda la flecha del popoever
	$(".arrow").css("top",`${parseInt(CoeficientesSeveridad.eventoSelected.clientY - ($(".popover").offset().top + 14))}px`);
}

//Borra el popover al reordenar las filas
CoeficientesSeveridad.gestionDOM.borrarPopoverOnOrder = function(e, settings){
	CoeficientesSeveridad.gestionDOM.borrarPopover();
}

/*****************************FIN*POPOVER***************************************/

//Se redibuja la tabla cuando se cambia la seleccion
CoeficientesSeveridad.gestionDOM.genTable = function(){
	//Cerrar popover
	CoeficientesSeveridad.gestionDOM.popoverOff();

	//Se limpia el contenedor
	$('#tableCoeficientesSeveridad').empty();

	var txt = `
	<div id="divtabla_0" class="c_shadow rounded dataTableStyle">		
		<!--para configurar opciones con los data-->
		<!-- Por defecto siempre se organiza por la primera columna asc -->
		<table id="tablaCoeficientesSeveridad" class="table-striped table-bordered" data-order='[[ 0, "asc" ]]' data-page-length='100'>
		   </table> <!-- el id de la tabla si se puede cambiar -->
	</div>
	`;

	//Se dibuja el div de data table
	$('#tableCoeficientesSeveridad').append(txt);

	//Al clickar cualquier elemento tr dentro de tbody 
    $("#tablaCoeficientesSeveridad").on("click","tbody tr",CoeficientesSeveridad.funciones.clickSelected);

	$("#principal").css("display","none");

	CoeficientesSeveridad.funciones.genTable();
}

//Creacion del formulario
CoeficientesSeveridad.gestionDOM.htmlFormEdit = function(resultado){
	CoeficientesSeveridad.datosFilaTabla[0]["Síntoma"] = CoeficientesSeveridad.datosFilaTabla[0]["Síntoma"] == null ? "": CoeficientesSeveridad.datosFilaTabla[0]["Síntoma"];
	CoeficientesSeveridad.datosFilaTabla[0]["Posición de la hoja (II)"] = CoeficientesSeveridad.datosFilaTabla[0]["Posición de la hoja (II)"] == null ? "": CoeficientesSeveridad.datosFilaTabla[0]["Posición de la hoja (II)"];
	CoeficientesSeveridad.datosFilaTabla[0]["Posición de la hoja (III)"] = CoeficientesSeveridad.datosFilaTabla[0]["Posición de la hoja (III)"] == null ? "": CoeficientesSeveridad.datosFilaTabla[0]["Posición de la hoja (III)"];
	CoeficientesSeveridad.datosFilaTabla[0]["Posición de la hoja (IV)"] = CoeficientesSeveridad.datosFilaTabla[0]["Posición de la hoja (IV)"] == null ? "": CoeficientesSeveridad.datosFilaTabla[0]["Posición de la hoja (IV)"];

	var txt;
	txt =`
	<div class="TituloCategoria">
		<span class="titulo_opcion"></span>
	</div>

	<div class="info row">
		<div class="contenedorInput col-lg-4 col-md-6 mb-4">
			<input type="text" id="dos" required value="${Intl.NumberFormat("de-DE").format(CoeficientesSeveridad.datosFilaTabla[0]["Posición de la hoja (II)"])}">
			<label>Posición de la hoja (II)</label>
			<!-- Contenedor para mensajes de error -->
			<div id="invalidFeedback_dos"></div>			
		</div>
		<div class="contenedorInput col-lg-4 col-md-6 mb-4">
			<input type="text" id="tres" required value="${Intl.NumberFormat("de-DE").format(CoeficientesSeveridad.datosFilaTabla[0]["Posición de la hoja (III)"])}">
			<label>Posición de la hoja (III)</label>
			<!-- Contenedor para mensajes de error -->
			<div id="invalidFeedback_tres"></div>
		</div>
		<div class="contenedorInput col-lg-4 col-md-6 mb-4">
			<input type="text" id="cuatro" required value="${Intl.NumberFormat("de-DE").format(CoeficientesSeveridad.datosFilaTabla[0]["Posición de la hoja (IV)"])}">
			<label>Posición de la hoja (IV)</label>
			<!-- Contenedor para mensajes de error -->
			<div id="invalidFeedback_cuatro"></div>
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

	gestionImask.cualquierNumeroEntero("#dos");
	gestionImask.cualquierNumeroEntero("#tres");
	gestionImask.cualquierNumeroEntero("#cuatro");

    //Titulo del formulario y etiqueta del boton
    $('.TituloCategoria .titulo_opcion').html(`Síntoma (${CoeficientesSeveridad.datosFilaTabla[0]["Síntoma"]})`);
	$('#guardarCambios span').html('Editar');

    //Evento para agregar finca
    $('#guardarCambios').off().on("click", CoeficientesSeveridad.funciones.validacionUsuarioEdit);
    
    //Funcion para cancelar agregar
    $('#cancelar').off().on("click", CoeficientesSeveridad.gestionDOM.cancelar);

	$("#principal").css("display","block");

	$("#contenedorCoeficientesSeveridad").animate({ scrollTop: 0}, 300);

	CoeficientesSeveridad.gestionDOM.borrarPopover();
}

//Se oculta el formulario
CoeficientesSeveridad.gestionDOM.cancelar = function(){
	$("#principal").css("display","none");
	$("#principal").html("");
}

//********* Mensajes de error *************/
CoeficientesSeveridad.gestionDOM.mensajeError = function(tag_id, contenidoHTML){
		$(`${tag_id}`).append(contenidoHTML);	
}