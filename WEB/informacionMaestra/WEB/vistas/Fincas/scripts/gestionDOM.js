Fincas.gestionDOM = new Object();

/*****************************POPOVER***************************************/
Fincas.gestionDOM.popoverOff = function(){
	//Cerrar popover
	$(".popover").popover("dispose");
}

//eventos popover
Fincas.gestionDOM.popoverEditar = function(e, dt, type, indexes){
	//Se borra el popover anterior
	Fincas.gestionDOM.borrarPopover();
	// Se guardan los datos de la fila de la tabla actualmente seleccionada
	Fincas.datosFilaTabla = Fincas.$tabla.rows(indexes).data();
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
	Fincas.gestionDOM.posicionPopoever(pop);
	//Se muestra el popover
	$(this).children("tbody").children("tr").eq(indexes[0]).popover("show");
	//No se muestra el popver hasta asustar posicion
	$(".popover").css("display","none");
	//Se cierra el popover en los botones de filtrar
	$(".filter-option").off("click").on("click",Fincas.gestionDOM.cerrarPopover);
	//Elemento anterior
   	Fincas.ultimoElementoConPopoever = $(this).children("tbody").children("tr").eq(indexes[0]);
   	//Se guarda el elemento tabla
   	Fincas.tabla = $(this);
   	//Cada que se da click en el contenedor se mira que no tenga nada que ver con el popover y si es asi se oculta
   	$("body").off('click').on('click',Fincas.gestionDOM.cerrarPopoverEnClick);
   	//Vuelve a esconder el popover al dar click en la fila otra vez
   	$(".selected").click(Fincas.gestionDOM.borrarPopover);
  	//Se definen los eventos de editar y borrar
    if(Fincas.baderaPermisosEdicion){
	   //Evento boton editar
	   $("#edit").off("click").on("click", Fincas.funciones.bringSelectsEdit);
    }
    else{ 
        //Se le da el color de button disabled 
        $("#edit").addClass('boton-popover-desabilitado');
        // Tooltip con mensaje para usuario
        $("#edit").tooltip({title: "No posee permisos para Editar", animation: true , placement: "right"}); 
    }
}

Fincas.gestionDOM.posicionPopoever = function(pop){
	//Se restan 28px que son la mitad del tamaño del contenedor, como offset
	Fincas.top = Fincas.eventoSelected.clientY - 28;
	Fincas.left = Fincas.eventoSelected.clientX + 10;
}

Fincas.gestionDOM.cerrarPopoverEnClick = function(e){
	var container = $(`.popover`);
	if(Fincas.tabla != undefined){
		if ((!container.is(e.target)) && (!Fincas.tabla.children("tbody").is(e.target.parentElement.parentElement)) 
		&& (container.has(e.target).length === 0)) { 
				//Se ha pulsado en cualquier lado fuera de los elementos contenidos en la variable container
				Fincas.gestionDOM.borrarPopover();
		}
	}
}

Fincas.gestionDOM.borrarPopover = function(){
	$(Fincas.ultimoElementoConPopoever).popover("dispose");
}

//Cuando el popover esta generado (pero oculto):
Fincas.gestionDOM.popoverFullyShown = function(){
	//Se ajusta la posicion del popover con las cooernadas del click
    $(".popover").css({"transform":`translate(${Fincas.left}px,${Fincas.top}px)`});
    //Se muetra el popover despues de acomodado
	$(".popover").css("display","block");
	//Se acomoda la flecha del popoever
	$(".arrow").css("top",`${parseInt(Fincas.eventoSelected.clientY - ($(".popover").offset().top + 14))}px`);
}

//Borra el popover al reordenar las filas
Fincas.gestionDOM.borrarPopoverOnOrder = function(e, settings){
	Fincas.gestionDOM.borrarPopover();
}

/*****************************FIN*POPOVER***************************************/

//Se redibuja la tabla cuando se cambia la seleccion
Fincas.gestionDOM.genTable = function(){
	//Cerrar popover
	Fincas.gestionDOM.popoverOff();

	//Se limpia el contenedor
	$('#tableFincas').empty();

	var txt = `
	<div id="divtabla_0" class="c_shadow rounded dataTableStyle">		
		<!--para configurar opciones con los data-->
		<!-- Por defecto siempre se organiza por la primera columna asc -->
		<table id="tablaFincas" class="table-striped table-bordered" data-order='[[ 6, "asc" ]]' data-page-length='100'>
		   </table> <!-- el id de la tabla si se puede cambiar -->
	</div>
	`;

	//Se dibuja el div de data table
	$('#tableFincas').append(txt);

	//Al clickar cualquier elemento tr dentro de tbody 
    $("#tablaFincas").on("click","tbody tr",Fincas.funciones.clickSelected);

	$("#principal").css("display","none");

	Fincas.funciones.genTable();
}

//Se refrescan las opciones de municipios
Fincas.gestionDOM.bringMunicipios = function(resultado){
	var txt;
	for(x in resultado)
	{
		if(Fincas.edicion == 1){
			txt = `${txt}
			<option value="${resultado[x].id}">
				${resultado[x].nombre}
			</option>
			`;
		}
		else{
			txt = `${txt}
			<option value="${resultado[x].id}">
				${resultado[x].nombre}
			</option>
			`;
		}
	}
	
	$("#selectMunicipio").html(txt);
	$("#selectMunicipio").selectpicker("refresh");

	// Si el selector solo tiene 1 opcion se autoselecciona	
	if(resultado.length == 1){
		$("#selectMunicipio").val(resultado[0].id).prop('disabled',true);
		$(`#selectMunicipio`).selectpicker('refresh');
	}

		//Si viene del flujo de edicion se selecciona lo que tenga este dato en tabla
	if(Fincas.edicion == 1){
		$(`#selectMunicipio option:contains(${Fincas.datosFilaTabla[0]["Municipio"]})`).attr("selected",true);
		$(`#selectMunicipio`).selectpicker('refresh');
		Fincas.edicion = 0;
	}
}

//Creacion del formulario
Fincas.gestionDOM.htmlForm = function(resultado){
	var txt;
	txt =`
	<div class="TituloCategoria">
		<span class="titulo_opcion"></span>
	</div>

	<div class="infoFinca row">
		<div class="contenedorInput col-lg-3 col-md-6 I_Finca mb-4">
			<input type="text" id="codigo" required>
			<label>Código Finca</label>
			<!-- Contenedor para mensajes de error -->
			<div id="invalidFeedback_codigo"></div>				
		</div>
		<div class="contenedorInput col-lg-3 col-md-6 I_Finca mb-4">
			<input type="text" id="nombre" required>
			<label>Nombre Finca</label>
			<!-- Contenedor para mensajes de error -->
			<div id="invalidFeedback_nombre"></div>				
		</div>
		<div class="container_Select col-lg-3 col-md-6 mb-4">
			<select id="selectDepartamento" class='selectpicker'>`;
		for(x in resultado.DEPARTAMENTOS)
		{
			txt = `${txt}
				<option ${resultado.DEPARTAMENTOS.length == 1 ? "selected disabled" : ""} value="${resultado.DEPARTAMENTOS[x].id}">
					${resultado.DEPARTAMENTOS[x].nombre}
				</option>
			`;
		}
			txt = `${txt}
			</select>
			<label>Departamento</label>
			<!-- Contenedor para mensajes de error -->
			<div id="invalidFeedback_selectDepartamento"></div>			
		</div>
		<div class="container_Select col-lg-3 col-md-6 mb-4">
			<select id="selectMunicipio" class='selectpicker'>
				<option disabled value="0"> </option>
			</select>
			<label>Municipio</label>
			<!-- Contenedor para mensajes de error -->
			<div id="invalidFeedback_selectMunicipio"></div>				
		</div>
		<div class="container_Select col-lg-3 col-md-6 mb-4">
			<select id="selectSector" class='selectpicker'>`;
			for(x in resultado.SECTORES)
			{
				txt = `${txt}
					<option ${resultado.SECTORES.length == 1 ? "selected disabled" : ""} value="${resultado.SECTORES[x].id}">
						${resultado.SECTORES[x].nombre}
					</option>
				`;
			}
				txt = `${txt}
			</select>
			<label>Sector</label>
			<!-- Contenedor para mensajes de error -->
			<div id="invalidFeedback_selectSector"></div>			
		</div>
		<div class="container_Select col-lg-3 col-md-6 mb-4">
			<select id="selectGrupo" class='selectpicker'>`;
			for(x in resultado.GRUPOS)
			{
				txt = `${txt}
					<option ${resultado.GRUPOS.length == 1 ? "selected disabled" : ""} value="${resultado.GRUPOS[x].id}">
						${resultado.GRUPOS[x].nombre}
					</option>
				`;
			}
				txt = `${txt}
			</select>
			<label>Grupo</label>
			<!-- Contenedor para mensajes de error -->
			<div id="invalidFeedback_selectGrupo"></div>			
		</div>
		<div class="container_Select col-lg-3 col-md-6 mb-4">
			<select id="selectEstado" class='selectpicker'>`;
			for(x in resultado.ESTADOS)
			{
				txt = `${txt}
					<option ${resultado.ESTADOS.length == 1 ? "selected disabled" : ""} value="${resultado.ESTADOS[x].id}">
						${resultado.ESTADOS[x].nombre}
					</option>
				`;
			}
				txt = `${txt}
			</select>
			<label>Estado</label>
			<!-- Contenedor para mensajes de error -->
			<div id="invalidFeedback_selectEstado"></div>			
		</div>
		<div class="contenedorInput col-lg-3 col-md-6 mb-4">
			<input type="text" id="latitud" required>
			<label>Latitud</label>
			<!-- Contenedor para mensajes de error -->
			<div id="invalidFeedback_latitud"></div>			
		</div>
		<div class="contenedorInput col-lg-3 col-md-6 mb-4">
			<input type="text" id="longitud" required>
			<label>Longitud</label>
			<!-- Contenedor para mensajes de error -->
			<div id="invalidFeedback_longitud"></div>
		</div>
		<div class="contenedorInput col-lg-3 col-md-6 mb-4">
			<input type="text" id="hectareas" required>
			<label>Hectáreas</label>
			<!-- Contenedor para mensajes de error -->
			<div id="invalidFeedback_hectareas"></div>
		</div>
		<div class="container_Select col-lg-3 col-md-6 mb-4">
			<select id="selectOrganica" class='selectpicker'>`;
			for(x in resultado.ORGANICA)
			{
				txt = `${txt}
					<option ${resultado.ORGANICA.length == 1 ? "selected disabled" : ""} value="${resultado.ORGANICA[x].id}">
						${resultado.ORGANICA[x].nombre}
					</option>
				`;
			}
				txt = `${txt}
			</select>
			<label>Orgánica</label>
			<!-- Contenedor para mensajes de error -->
			<div id="invalidFeedback_selectOrganica"></div>			
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

	gestionImask.ModTextoNumeros("#nombre");
	gestionImask.Numerico4Digitos("#codigo");
	gestionImask.cualquierNumeroDecimal("#latitud");
	gestionImask.cualquierNumeroDecimal("#longitud");
	gestionImask.cualquierNumeroDecimal("#hectareas");

	//Se asignan los eventos de los selectores del form
	$("#selectDepartamento").off("change").on("change", Fincas.funciones.bringMunicipios);

	//Aplica el estilo de selectores 
    gestionBSelect.generic3(`#selectDepartamento`,'#contenedorFincas'," ");
    gestionBSelect.generic3(`#selectMunicipio`,'#contenedorFincas'," ");
    gestionBSelect.generic3(`#selectSector`,'#contenedorFincas'," ");
    gestionBSelect.generic3(`#selectGrupo`,'#contenedorFincas'," ");
    gestionBSelect.generic3(`#selectEstado`,'#contenedorFincas'," ");
	gestionBSelect.generic3(`#selectOrganica`,'#contenedorFincas'," ");

	//Se remueben los tooltip de BSelect
    $(`#selectDepartamento`).removeAttr("title");
    $(`#selectMunicipio`).removeAttr("title");
    $(`#selectSector`).removeAttr("title");
    $(`#selectGrupo`).removeAttr("title");
    $(`#selectEstado`).removeAttr("title");
	$(`#selectOrganica`).removeAttr("title");

    //Titulo del formulario y etiqueta del boton
    $('.TituloCategoria .titulo_opcion').html('Agregar Finca');
    $('#guardarCambios span').html('Agregar');

    //Evento para agregar finca
    $('#guardarCambios').off().on("click", Fincas.funciones.validacionUsuario);
    
    //Funcion para cancelar agregar
    $('#cancelar').off().on("click", Fincas.gestionDOM.cancelar);

	$("#principal").css("display","block");
}

//Creacion del formulario
Fincas.gestionDOM.htmlFormEdit = function(resultado){
	Fincas.datosFilaTabla[0]["Código"] = Fincas.datosFilaTabla[0]["Código"] == null ? "": Fincas.datosFilaTabla[0]["Código"];
	Fincas.datosFilaTabla[0]["Nombre"] = Fincas.datosFilaTabla[0]["Nombre"] == null ? "": Fincas.datosFilaTabla[0]["Nombre"];
	Fincas.datosFilaTabla[0]["Departamento"] = Fincas.datosFilaTabla[0]["Departamento"] == null ? "": Fincas.datosFilaTabla[0]["Departamento"];
	Fincas.datosFilaTabla[0]["Municipio"] = Fincas.datosFilaTabla[0]["Municipio"] == null ? "": Fincas.datosFilaTabla[0]["Municipio"];
	Fincas.datosFilaTabla[0]["Sector"] = Fincas.datosFilaTabla[0]["Sector"] == null ? "": Fincas.datosFilaTabla[0]["Sector"];
	Fincas.datosFilaTabla[0]["Grupo"] = Fincas.datosFilaTabla[0]["Grupo"] == null ? "": Fincas.datosFilaTabla[0]["Grupo"];
	Fincas.datosFilaTabla[0]["Estado"] = Fincas.datosFilaTabla[0]["Estado"] == null ? "": Fincas.datosFilaTabla[0]["Estado"];
	Fincas.datosFilaTabla[0]["Latitud"] = Fincas.datosFilaTabla[0]["Latitud"] == null ? "": Fincas.datosFilaTabla[0]["Latitud"];
	Fincas.datosFilaTabla[0]["Longitud"] = Fincas.datosFilaTabla[0]["Longitud"] == null ? "": Fincas.datosFilaTabla[0]["Longitud"];
	Fincas.datosFilaTabla[0]["Hectáreas"] = Fincas.datosFilaTabla[0]["Hectáreas"] == null ? "": Fincas.datosFilaTabla[0]["Hectáreas"];
	Fincas.datosFilaTabla[0]["Orgánica"] = Fincas.datosFilaTabla[0]["Orgánica"] == null ? "": Fincas.datosFilaTabla[0]["Orgánica"];

	var txt;
	txt =`
	<div class="TituloCategoria">
		<span class="titulo_opcion"></span>
	</div>

	<div class="infoFinca row">
		<div class="contenedorInput col-lg-3 col-md-6 I_Finca mb-4">
			<input type="text" id="codigo" required value="${Fincas.datosFilaTabla[0]["Código"]}">
			<label>Código Finca</label>
			<!-- Contenedor para mensajes de error -->
			<div id="invalidFeedback_codigo"></div>				
		</div>
		<div class="contenedorInput col-lg-3 col-md-6 I_Finca mb-4">
			<input type="text" id="nombre" required value="${Fincas.datosFilaTabla[0]["Nombre"]}">
			<label>Nombre Finca</label>
			<!-- Contenedor para mensajes de error -->
			<div id="invalidFeedback_nombre"></div>				
		</div>
		<div class="container_Select col-lg-3 col-md-6 mb-4">
			<select id="selectDepartamento" class='selectpicker'>`;
		for(x in resultado.DEPARTAMENTOS)
		{
			txt = `${txt}
				<option ${resultado.DEPARTAMENTOS.length == 1 ? "selected disabled" : ""} value="${resultado.DEPARTAMENTOS[x].id}"
					${Fincas.datosFilaTabla[0]["Departamento"] == resultado.DEPARTAMENTOS[x].nombre ? "selected" : ""}>
					${resultado.DEPARTAMENTOS[x].nombre}
				</option>
			`;
		}
			txt = `${txt}
			</select>
			<label>Departamento</label>
			<!-- Contenedor para mensajes de error -->
			<div id="invalidFeedback_selectDepartamento"></div>			
		</div>
		<div class="container_Select col-lg-3 col-md-6 mb-4">
			<select id="selectMunicipio" class='selectpicker'>
				<option disabled value="0"> </option>
			</select>
			<label>Municipio</label>
			<!-- Contenedor para mensajes de error -->
			<div id="invalidFeedback_selectMunicipio"></div>				
		</div>
		<div class="container_Select col-lg-3 col-md-6 mb-4">
			<select id="selectSector" class='selectpicker'>`;
			for(x in resultado.SECTORES)
			{
				txt = `${txt}
					<option ${resultado.SECTORES.length == 1 ? "selected disabled" : ""} value="${resultado.SECTORES[x].id}"
					${Fincas.datosFilaTabla[0]["Sector"] == resultado.SECTORES[x].nombre ? "selected" : ""}>
						${resultado.SECTORES[x].nombre}
					</option>
				`;
			}
				txt = `${txt}
			</select>
			<label>Sector</label>
			<!-- Contenedor para mensajes de error -->
			<div id="invalidFeedback_selectSector"></div>			
		</div>
		<div class="container_Select col-lg-3 col-md-6 mb-4">
			<select id="selectGrupo" class='selectpicker'>`;
			for(x in resultado.GRUPOS)
			{
				txt = `${txt}
					<option ${resultado.GRUPOS.length == 1 ? "selected disabled" : ""} value="${resultado.GRUPOS[x].id}"
					${Fincas.datosFilaTabla[0]["Grupo"] == resultado.GRUPOS[x].nombre ? "selected" : ""}>
						${resultado.GRUPOS[x].nombre}
					</option>
				`;
			}
				txt = `${txt}
			</select>
			<label>Grupo</label>
			<!-- Contenedor para mensajes de error -->
			<div id="invalidFeedback_selectGrupo"></div>			
		</div>
		<div class="container_Select col-lg-3 col-md-6 mb-4">
			<select id="selectEstado" class='selectpicker'>`;
			for(x in resultado.ESTADOS)
			{
				txt = `${txt}
					<option ${resultado.ESTADOS.length == 1 ? "selected disabled" : ""} value="${resultado.ESTADOS[x].id}"
					${Fincas.datosFilaTabla[0]["Estado"] == resultado.ESTADOS[x].nombre ? "selected" : ""}>
						${resultado.ESTADOS[x].nombre}
					</option>
				`;
			}
				txt = `${txt}
			</select>
			<label>Estado</label>
			<!-- Contenedor para mensajes de error -->
			<div id="invalidFeedback_selectEstado"></div>			
		</div>
		<div class="contenedorInput col-lg-3 col-md-6 mb-4">
			<input type="text" id="latitud" required value="${Intl.NumberFormat("de-DE").format(Fincas.datosFilaTabla[0]["Latitud"])}">
			<label>Latitud</label>
			<!-- Contenedor para mensajes de error -->
			<div id="invalidFeedback_latitud"></div>			
		</div>
		<div class="contenedorInput col-lg-3 col-md-6 mb-4">
			<input type="text" id="longitud" required value="${Intl.NumberFormat("de-DE").format(Fincas.datosFilaTabla[0]["Longitud"])}">
			<label>Longitud</label>
			<!-- Contenedor para mensajes de error -->
			<div id="invalidFeedback_longitud"></div>
		</div>
		<div class="contenedorInput col-lg-3 col-md-6 mb-4">
			<input type="text" id="hectareas" required value="${Intl.NumberFormat("de-DE").format(Fincas.datosFilaTabla[0]["Hectáreas"])}">
			<label>Hectáreas</label>
			<!-- Contenedor para mensajes de error -->
			<div id="invalidFeedback_hectareas"></div>
		</div>
		<div class="container_Select col-lg-3 col-md-6 mb-4">
			<select id="selectOrganica" class='selectpicker'>`;
			for(x in resultado.ORGANICA)
			{
				txt = `${txt}
					<option ${resultado.ORGANICA.length == 1 ? "selected disabled" : ""} value="${resultado.ORGANICA[x].id}"
					${Fincas.datosFilaTabla[0]["Orgánica"] == resultado.ORGANICA[x].nombre ? "selected" : ""}>
						${resultado.ORGANICA[x].nombre}
					</option>
				`;
			}
				txt = `${txt}
			</select>
			<label>Orgánica</label>
			<!-- Contenedor para mensajes de error -->
			<div id="invalidFeedback_selectOrganica"></div>			
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

	gestionImask.ModTextoNumeros("#nombre");
	gestionImask.Numerico4Digitos("#codigo");
	gestionImask.cualquierNumeroDecimal("#latitud");
	gestionImask.cualquierNumeroDecimal("#longitud");
	gestionImask.cualquierNumeroDecimal("#hectareas");

	//Se asignan los eventos de los selectores del form
	$("#selectDepartamento").off("change").on("change", Fincas.funciones.bringMunicipios);	

	//Aplica el estilo de selectores 
    gestionBSelect.generic3(`#selectDepartamento`,'#contenedorFincas'," ");
    gestionBSelect.generic3(`#selectMunicipio`,'#contenedorFincas'," ");
    gestionBSelect.generic3(`#selectSector`,'#contenedorFincas'," ");
    gestionBSelect.generic3(`#selectGrupo`,'#contenedorFincas'," ");
	gestionBSelect.generic3(`#selectEstado`,'#contenedorFincas'," ");
	gestionBSelect.generic3(`#selectOrganica`,'#contenedorFincas'," ");

	//Se remueben los tooltip de BSelect
    $(`#selectDepartamento`).removeAttr("title");
    $(`#selectMunicipio`).removeAttr("title");
    $(`#selectSector`).removeAttr("title");
    $(`#selectGrupo`).removeAttr("title");
    $(`#selectEstado`).removeAttr("title");
	$(`#selectOrganica`).removeAttr("title");

    //Titulo del formulario y etiqueta del boton
    $('.TituloCategoria .titulo_opcion').html('Editar Finca');
	$('#guardarCambios span').html('Editar');

	Fincas.funciones.bringMunicipios();
	Fincas.edicion = 1;

    //Evento para agregar finca
    $('#guardarCambios').off().on("click", Fincas.funciones.validacionUsuarioEdit);
    
    //Funcion para cancelar agregar
    $('#cancelar').off().on("click", Fincas.gestionDOM.cancelar);

	$("#principal").css("display","block");

	$("#contenedorFincas").animate({ scrollTop: 0}, 300);
}

//Se oculta el formulario
Fincas.gestionDOM.cancelar = function(){
	$(".dropdown-menu").hide();
	$("#principal").css("display","none");
	$("#principal").html("");
}

//********* Mensajes de error *************/
Fincas.gestionDOM.mensajeError = function(tag_id, contenidoHTML){
		$(`${tag_id}`).append(contenidoHTML);	
}