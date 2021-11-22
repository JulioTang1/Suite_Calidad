Datos.gestionDOM = new Object();

/*****************************POPOVER***************************************/
Datos.gestionDOM.popoverOff = function(){
	//Cerrar popover
	$(".popover").popover("dispose");
}

//eventos popover
Datos.gestionDOM.popoverEditar = function(e, dt, type, indexes){
	//Se borra el popover anterior
	Datos.gestionDOM.borrarPopover();
	// Se guardan los datos de la fila de la tabla actualmente seleccionada
	Datos.datosFilaTabla = Datos.$tabla[e.data.index].rows(indexes).data();
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
	Datos.gestionDOM.posicionPopoever(pop);
	//Se muestra el popover
	$(this).children("tbody").children("tr").eq(indexes[0]).popover("show");
	//No se muestra el popver hasta asustar posicion
	$(".popover").css("display","none");
	//Se cierra el popover en los botones de filtrar
	$(".filter-option").off("click").on("click",Datos.gestionDOM.cerrarPopover);
	//Elemento anterior
   	Datos.ultimoElementoConPopoever = $(this).children("tbody").children("tr").eq(indexes[0]);
   	//Se guarda el elemento tabla
   	Datos.tabla = $(this);
   	//Cada que se da click en el contenedor se mira que no tenga nada que ver con el popover y si es asi se oculta
   	$("body").off('click').on('click',Datos.gestionDOM.cerrarPopoverEnClick);
   	//Vuelve a esconder el popover al dar click en la fila otra vez
   	$(".selected").click(Datos.gestionDOM.borrarPopover);
  	//Se definen los eventos de editar y borrar
    if(Datos.baderaPermisosEdicion){
	   //Evento boton editar
	   $("#edit").off("click").on("click", {index: e.data.index}, Datos.gestionDOM.edit);
    }
    else{ 
        //Se le da el color de button disabled 
        $("#edit").addClass('boton-popover-desabilitado');
        // Tooltip con mensaje para usuario
        $("#edit").tooltip({title: "No posee permisos para Editar", animation: true , placement: "right"}); 
    }
}

Datos.gestionDOM.posicionPopoever = function(pop){
	//Se restan 28px que son la mitad del tamaño del contenedor, como offset
	Datos.top = Datos.eventoSelected.clientY - 28;
	Datos.left = Datos.eventoSelected.clientX + 10;
}

Datos.gestionDOM.cerrarPopoverEnClick = function(e){
	var container = $(`.popover`);
	if(Datos.tabla != undefined){
		if ((!container.is(e.target)) && (!Datos.tabla.children("tbody").is(e.target.parentElement.parentElement)) 
		&& (container.has(e.target).length === 0)) { 
				//Se ha pulsado en cualquier lado fuera de los elementos contenidos en la variable container
				Datos.gestionDOM.borrarPopover();
		}
	}
}

Datos.gestionDOM.borrarPopover = function(){
	$(Datos.ultimoElementoConPopoever).popover("dispose");
}

//Cuando el popover esta generado (pero oculto):
Datos.gestionDOM.popoverFullyShown = function(){
	//Se ajusta la posicion del popover con las cooernadas del click
    $(".popover").css({"transform":`translate(${Datos.left}px,${Datos.top}px)`});
    //Se muetra el popover despues de acomodado
	$(".popover").css("display","block");
	//Se acomoda la flecha del popoever
	$(".arrow").css("top",`${parseInt(Datos.eventoSelected.clientY - ($(".popover").offset().top + 14))}px`);
}

//Borra el popover al reordenar las filas
Datos.gestionDOM.borrarPopoverOnOrder = function(e, settings){
	Datos.gestionDOM.borrarPopover();
}

/*****************************FIN*POPOVER***************************************/

//Se verifica a que fila y que tabla se dio clic
Datos.gestionDOM.edit = function(e){
	var index = e.data.index;
	$("#modal").remove();
	if(index == 0){//tablaSigatoka
		if( Datos.datosFilaTabla[0]["Edad"] == "P10" ){//HF, YLS y CF

			var html = `
			<div class="modal" id="modal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
				<div class="modal-dialog" role="document">
					<div class="modal-content">
						<div class="modal-header">
							<h4 class="modal-title">
								<span>Edición P10 </span> 
							</h4>
							<button type="button" class="close" data-dismiss="modal" aria-label="Close">
								<i class="fas fa-times"></i>
							</button>					
						</div>
						<div class="modal-body row">
							<div class="col-lg-6 col-md-6 mb-lg-3 mb-3">
								<input autocomplete="off" type="text" required placeholder=" " id="YLS" value="${Intl.NumberFormat("de-DE").format(Datos.datosFilaTabla[0]["YLS"])}">
								<label>YLS</label>
								<div id="invalidFeedback_YLS"></div>
							</div>
							<div class="col-lg-6 col-md-6 mb-lg-3 mb-3">
								<input autocomplete="off" type="text" required placeholder=" " id="CF" value="${Intl.NumberFormat("de-DE").format(Datos.datosFilaTabla[0]["CF"])}">
								<label>CF</label>
								<div id="invalidFeedback_CF"></div>
							</div>
							<div class="col-lg-6 col-md-6 mb-lg-3 mb-3">
								<input autocomplete="off" type="text" required placeholder=" " id="HF" value="${Intl.NumberFormat("de-DE").format(Datos.datosFilaTabla[0]["HF"])}">
								<label>HF</label>
								<div id="invalidFeedback_HF"></div>
							</div>
							<div class="col-lg-12 col-md-12 mb-lg-6 mb-6">
								<textarea autocomplete="off" required placeholder=" " id="Lote">${Datos.datosFilaTabla[0]["Lote"] == null ? "" : Datos.datosFilaTabla[0]["Lote"]}</textarea>
								<label>Lote</label>
								<div id="invalidFeedback_Lote"></div>
							</div>
						</div>
						<div class="modal-footer">
							<button class="btn btn-md btn-primary" data-dismiss="modal">
								<i class="fas fa-arrow-circle-left"></i>
								<span>Cancelar</span>		      	
							</button>
							<button id="editar" class="btn btn-md btn-success">
								<i class="far fa-check-circle"></i>
								<span>Editar</span>
							</button>
						</div>
					</div>
				</div>
			</div>
			`;
			
			$("#contenedorDatos").append(html);
			$('#modal').modal('show');
			gestionImask.cualquierNumeroDecimal("#YLS", 0, 0, 999.99, 2);
			gestionImask.cualquierNumeroEntero("#CF", 0, 0, 4);
			gestionImask.cualquierNumeroDecimal("#HF", 0, 0, 999.99, 2);

			$('#editar').off().on("click", {index: index}, Datos.funciones.saveSigatokaP10P7);

		}else if( Datos.datosFilaTabla[0]["Edad"] == "P7" ){//HF, YLS y CF)

			var html = `
			<div class="modal" id="modal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
				<div class="modal-dialog" role="document">
					<div class="modal-content">
						<div class="modal-header">
							<h4 class="modal-title">
								<span>Edición P7 </span> 
							</h4>
							<button type="button" class="close" data-dismiss="modal" aria-label="Close">
								<i class="fas fa-times"></i>
							</button>					
						</div>
						<div class="modal-body row">
							<div class="col-lg-6 col-md-6 mb-lg-3 mb-3">
								<input autocomplete="off" type="text" required placeholder=" " id="YLS" value="${Intl.NumberFormat("de-DE").format(Datos.datosFilaTabla[0]["YLS"])}">
								<label>YLS</label>
								<div id="invalidFeedback_YLS"></div>
							</div>
							<div class="col-lg-6 col-md-6 mb-lg-3 mb-3">
								<input autocomplete="off" type="text" required placeholder=" " id="CF" value="${Intl.NumberFormat("de-DE").format(Datos.datosFilaTabla[0]["CF"])}">
								<label>CF</label>
								<div id="invalidFeedback_CF"></div>
							</div>
							<div class="col-lg-6 col-md-6 mb-lg-3 mb-3">
								<input autocomplete="off" type="text" required placeholder=" " id="HF" value="${Intl.NumberFormat("de-DE").format(Datos.datosFilaTabla[0]["HF"])}">
								<label>HF</label>
								<div id="invalidFeedback_HF"></div>
							</div>
							<div class="col-lg-12 col-md-12 mb-lg-6 mb-6">
								<textarea autocomplete="off" required placeholder=" " id="Lote">${Datos.datosFilaTabla[0]["Lote"] == null ? "" : Datos.datosFilaTabla[0]["Lote"]}</textarea>
								<label>Lote</label>
								<div id="invalidFeedback_Lote"></div>
							</div>
						</div>
						<div class="modal-footer">
							<button class="btn btn-md btn-primary" data-dismiss="modal">
								<i class="fas fa-arrow-circle-left"></i>
								<span>Cancelar</span>		      	
							</button>
							<button id="editar" class="btn btn-md btn-success">
								<i class="far fa-check-circle"></i>
								<span>Editar</span>
							</button>
						</div>
					</div>
				</div>
			</div>
			`;
			
			$("#contenedorDatos").append(html);
			$('#modal').modal('show');
			gestionImask.cualquierNumeroDecimal("#YLS", 0, 0, 999.99, 2);
			gestionImask.cualquierNumeroEntero("#CF", 0, 0, 4);
			gestionImask.cualquierNumeroDecimal("#HF", 0, 0, 999.99, 2);

			$('#editar').off().on("click", {index: index}, Datos.funciones.saveSigatokaP10P7);

		}else if( Datos.datosFilaTabla[0]["Edad"] == "PxP" ){//YLS, CF, TH, YLI

			var html = `
			<div class="modal" id="modal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
				<div class="modal-dialog" role="document">
					<div class="modal-content">
						<div class="modal-header">
							<h4 class="modal-title">
								<span>Edición PxP </span> 
							</h4>
							<button type="button" class="close" data-dismiss="modal" aria-label="Close">
								<i class="fas fa-times"></i>
							</button>					
						</div>
						<div class="modal-body row">
							<div class="col-lg-6 col-md-6 mb-lg-3 mb-3">
								<input autocomplete="off" type="text" required placeholder=" " id="TH" value="${Intl.NumberFormat("de-DE").format(Datos.datosFilaTabla[0]["TH"])}">
								<label>TH</label>
								<div id="invalidFeedback_TH"></div>
							</div>
							<div class="col-lg-6 col-md-6 mb-lg-3 mb-3">
								<input autocomplete="off" type="text" required placeholder=" " id="YLI" value="${Intl.NumberFormat("de-DE").format(Datos.datosFilaTabla[0]["YLI"])}">
								<label>YLI</label>
								<div id="invalidFeedback_YLI"></div>
							</div>
							<div class="col-lg-6 col-md-6 mb-lg-3 mb-3">
								<input autocomplete="off" type="text" required placeholder=" " id="YLS" value="${Intl.NumberFormat("de-DE").format(Datos.datosFilaTabla[0]["YLS"])}">
								<label>YLS</label>
								<div id="invalidFeedback_YLS"></div>
							</div>
							<div class="col-lg-6 col-md-6 mb-lg-3 mb-3">
								<input autocomplete="off" type="text" required placeholder=" " id="CF" value="${Intl.NumberFormat("de-DE").format(Datos.datosFilaTabla[0]["CF"])}">
								<label>CF</label>
								<div id="invalidFeedback_CF"></div>
							</div>
							<div class="col-lg-12 col-md-12 mb-lg-6 mb-6">
								<textarea autocomplete="off" required placeholder=" " id="Lote">${Datos.datosFilaTabla[0]["Lote"] == null ? "" : Datos.datosFilaTabla[0]["Lote"]}</textarea>
								<label>Lote</label>
								<div id="invalidFeedback_Lote"></div>
							</div>
						</div>
						<div class="modal-footer">
							<button class="btn btn-md btn-primary" data-dismiss="modal">
								<i class="fas fa-arrow-circle-left"></i>
								<span>Cancelar</span>		      	
							</button>
							<button id="editar" class="btn btn-md btn-success">
								<i class="far fa-check-circle"></i>
								<span>Editar</span>
							</button>
						</div>
					</div>
				</div>
			</div>
			`;
			
			$("#contenedorDatos").append(html);
			$('#modal').modal('show');
			gestionImask.cualquierNumeroDecimal("#TH", 0, 0, 999.99, 2);
			gestionImask.cualquierNumeroDecimal("#YLI", 0, 0, 999.99, 2);
			gestionImask.cualquierNumeroDecimal("#YLS", 0, 0, 999.99, 2);
			gestionImask.cualquierNumeroEntero("#CF", 0, 0, 4);

			$('#editar').off().on("click", {index: index}, Datos.funciones.saveSigatokaPxP);

		}else if (Datos.datosFilaTabla[0]["Edad"] == "Fija"){//TH, EFA, CF, H2, H3, H4

			var html = `
			<div class="modal" id="modal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
				<div class="modal-dialog" role="document">
					<div class="modal-content">
						<div class="modal-header">
							<h4 class="modal-title">
								<span>Edición PxP </span> 
							</h4>
							<button type="button" class="close" data-dismiss="modal" aria-label="Close">
								<i class="fas fa-times"></i>
							</button>					
						</div>
						<div class="modal-body row">
							<div class="col-lg-6 col-md-6 mb-lg-3 mb-3">
								<input autocomplete="off" type="text" required placeholder=" " id="TH" value="${Intl.NumberFormat("de-DE").format(Datos.datosFilaTabla[0]["TH"])}">
								<label>TH</label>
								<div id="invalidFeedback_TH"></div>
							</div>
							<div class="col-lg-6 col-md-6 mb-lg-3 mb-3">
								<input autocomplete="off" type="text" required placeholder=" " id="EFA" value="${Intl.NumberFormat("de-DE").format(Datos.datosFilaTabla[0]["EFA"])}">
								<label>EFA</label>
								<div id="invalidFeedback_EFA"></div>
							</div>
							<div class="col-lg-6 col-md-6 mb-lg-3 mb-3">
								<input autocomplete="off" type="text" required placeholder=" " id="CF" value="${Intl.NumberFormat("de-DE").format(Datos.datosFilaTabla[0]["CF"])}">
								<label>CF</label>
								<div id="invalidFeedback_CF"></div>
							</div>
							<div class="container_Select col-lg-6 col-md-6 mb-lg-3 mb-3">
								<select id="H2" class='selectpicker'>
									<option ${Datos.datosFilaTabla[0]["H2"]== null ? "selected" : ""} value="0">0</option>
									<option ${Datos.datosFilaTabla[0]["H2"]== "1-" ? "selected" : ""} value="2">1-</option>
									<option ${Datos.datosFilaTabla[0]["H2"]== "1+" ? "selected" : ""} value="1">1+</option>
									<option ${Datos.datosFilaTabla[0]["H2"]== "2-" ? "selected" : ""} value="4">2-</option>
									<option ${Datos.datosFilaTabla[0]["H2"]== "2+" ? "selected" : ""} value="3">2+</option>
									<option ${Datos.datosFilaTabla[0]["H2"]== "3-" ? "selected" : ""} value="6">3-</option>
									<option ${Datos.datosFilaTabla[0]["H2"]== "3+" ? "selected" : ""} value="5">3+</option>
								</select>
								<label>H2</label>
								<!-- Contenedor para mensajes de error -->
								<div id="invalidFeedback_H2"></div>	
							</div>
							<div class="container_Select col-lg-6 col-md-6 mb-lg-3 mb-3">
								<select id="H3" class='selectpicker'>
									<option ${Datos.datosFilaTabla[0]["H3"]== null ? "selected" : ""} value="0">0</option>
									<option ${Datos.datosFilaTabla[0]["H3"]== "1-" ? "selected" : ""} value="2">1-</option>
									<option ${Datos.datosFilaTabla[0]["H3"]== "1+" ? "selected" : ""} value="1">1+</option>
									<option ${Datos.datosFilaTabla[0]["H3"]== "2-" ? "selected" : ""} value="4">2-</option>
									<option ${Datos.datosFilaTabla[0]["H3"]== "2+" ? "selected" : ""} value="3">2+</option>
									<option ${Datos.datosFilaTabla[0]["H3"]== "3-" ? "selected" : ""} value="6">3-</option>
									<option ${Datos.datosFilaTabla[0]["H3"]== "3+" ? "selected" : ""} value="5">3+</option>
								</select>
								<label>H3</label>
								<!-- Contenedor para mensajes de error -->
								<div id="invalidFeedback_H3"></div>	
							</div>
							<div class="container_Select col-lg-6 col-md-6 mb-lg-3 mb-3">
								<select id="H4" class='selectpicker'>
									<option ${Datos.datosFilaTabla[0]["H4"]== null ? "selected" : ""} value="0">0</option>
									<option ${Datos.datosFilaTabla[0]["H4"]== "1-" ? "selected" : ""} value="2">1-</option>
									<option ${Datos.datosFilaTabla[0]["H4"]== "1+" ? "selected" : ""} value="1">1+</option>
									<option ${Datos.datosFilaTabla[0]["H4"]== "2-" ? "selected" : ""} value="4">2-</option>
									<option ${Datos.datosFilaTabla[0]["H4"]== "2+" ? "selected" : ""} value="3">2+</option>
									<option ${Datos.datosFilaTabla[0]["H4"]== "3-" ? "selected" : ""} value="6">3-</option>
									<option ${Datos.datosFilaTabla[0]["H4"]== "3+" ? "selected" : ""} value="5">3+</option>
								</select>
								<label>H4</label>
								<!-- Contenedor para mensajes de error -->
								<div id="invalidFeedback_H4"></div>	
							</div>
							<div class="col-lg-12 col-md-12 mb-lg-6 mb-6">
								<textarea autocomplete="off" required placeholder=" " id="Lote">${Datos.datosFilaTabla[0]["Lote"] == null ? "" : Datos.datosFilaTabla[0]["Lote"]}</textarea>
								<label>Lote</label>
								<div id="invalidFeedback_Lote"></div>
							</div>
						</div>
						<div class="modal-footer">
							<button class="btn btn-md btn-primary" data-dismiss="modal">
								<i class="fas fa-arrow-circle-left"></i>
								<span>Cancelar</span>		      	
							</button>
							<button id="editar" class="btn btn-md btn-success">
								<i class="far fa-check-circle"></i>
								<span>Editar</span>
							</button>
						</div>
					</div>
				</div>
			</div>
			`;
			
			$("#contenedorDatos").append(html);
			$('#modal').modal('show');
			gestionImask.cualquierNumeroDecimal("#TH", 0, 0, 999.99, 2);
			gestionImask.cualquierNumeroDecimal("#EFA", 0, 0, 999.99, 2);
			gestionImask.cualquierNumeroEntero("#CF", 0, 0, 4);

			//Aplica el estilo de selectores 
			gestionBSelect.generic3(`#H2`,'#modal'," ");
			gestionBSelect.generic3(`#H3`,'#modal'," ");
			gestionBSelect.generic3(`#H4`,'#modal'," ");
		
			//Se remueben los tooltip de BSelect
			$(`#H2`).removeAttr("title");
			$(`#H3`).removeAttr("title");
			$(`#H4`).removeAttr("title");

			$('#editar').off().on("click", {index: index}, Datos.funciones.saveSigatokaFija);

		}
	}else if (index == 1){//tablaVasculares: Fusarium, Moko, Erwinia
		
		var html = `
		<div class="modal" id="modal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
			<div class="modal-dialog" role="document">
				<div class="modal-content">
					<div class="modal-header">
						<h4 class="modal-title">
							<span>Edición E.Vasculares </span> 
						</h4>
						<button type="button" class="close" data-dismiss="modal" aria-label="Close">
							<i class="fas fa-times"></i>
						</button>					
					</div>
					<div class="modal-body row">
						<div class="container_Select col-lg-6 col-md-6 mb-lg-3 mb-3">
							<select id="Fusarium" class='selectpicker'>
								<option ${Datos.datosFilaTabla[0]["Fusarium"]== "Ausencia" ? "selected" : ""} value="1">Ausencia</option>
								<option ${Datos.datosFilaTabla[0]["Fusarium"]== "Presencia tratada" ? "selected" : ""} value="2">Presencia tratada</option>
								<option ${Datos.datosFilaTabla[0]["Fusarium"]== "Presencia sin tratar" ? "selected" : ""} value="3">Presencia sin tratar</option>
								<option ${Datos.datosFilaTabla[0]["Fusarium"]== "Sospechosa" ? "selected" : ""} value="4">Sospechosa</option>
							</select>
							<label>Fusarium</label>
							<!-- Contenedor para mensajes de error -->
							<div id="invalidFeedback_Fusarium"></div>	
						</div>
						<div class="container_Select col-lg-6 col-md-6 mb-lg-3 mb-3">
							<select id="Moko" class='selectpicker'>
								<option ${Datos.datosFilaTabla[0]["Moko"]== "Ausencia" ? "selected" : ""} value="1">Ausencia</option>
								<option ${Datos.datosFilaTabla[0]["Moko"]== "Presencia tratada" ? "selected" : ""} value="2">Presencia tratada</option>
								<option ${Datos.datosFilaTabla[0]["Moko"]== "Presencia sin tratar" ? "selected" : ""} value="3">Presencia sin tratar</option>
								<option ${Datos.datosFilaTabla[0]["Moko"]== "Sospechosa" ? "selected" : ""} value="4">Sospechosa</option>
							</select>
							<label>Moko</label>
							<!-- Contenedor para mensajes de error -->
							<div id="invalidFeedback_Moko"></div>	
						</div>
						<div class="container_Select col-lg-6 col-md-6 mb-lg-3 mb-3">
							<select id="Erwinia" class='selectpicker'>
								<option ${Datos.datosFilaTabla[0]["Erwinia"]== "Ausencia" ? "selected" : ""} value="1">Ausencia</option>
								<option ${Datos.datosFilaTabla[0]["Erwinia"]== "Presencia tratada" ? "selected" : ""} value="2">Presencia tratada</option>
								<option ${Datos.datosFilaTabla[0]["Erwinia"]== "Presencia sin tratar" ? "selected" : ""} value="3">Presencia sin tratar</option>
								<option ${Datos.datosFilaTabla[0]["Erwinia"]== "Sospechosa" ? "selected" : ""} value="4">Sospechosa</option>
							</select>
							<label>Erwinia</label>
							<!-- Contenedor para mensajes de error -->
							<div id="invalidFeedback_Erwinia"></div>	
						</div>
					</div>
					<div class="modal-footer">
						<button class="btn btn-md btn-primary" data-dismiss="modal">
							<i class="fas fa-arrow-circle-left"></i>
							<span>Cancelar</span>		      	
						</button>
						<button id="editar" class="btn btn-md btn-success">
							<i class="far fa-check-circle"></i>
							<span>Editar</span>
						</button>
					</div>
				</div>
			</div>
		</div>
		`;
		
		$("#contenedorDatos").append(html);
		$('#modal').modal('show');

		//Aplica el estilo de selectores 
		gestionBSelect.generic3(`#Fusarium`,'#modal'," ");
		gestionBSelect.generic3(`#Moko`,'#modal'," ");
		gestionBSelect.generic3(`#Erwinia`,'#modal'," ");
	
		//Se remueben los tooltip de BSelect
		$(`#Fusarium`).removeAttr("title");
		$(`#Moko`).removeAttr("title");
		$(`#Erwinia`).removeAttr("title");

		$('#editar').off().on("click", {index: index}, Datos.funciones.saveVasculares);

	}else if(index ==2){//tablaCulturales: NF, FIT, Comentario FIT, RTI, Comentario RTI

		var html = `
		<div class="modal" id="modal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
			<div class="modal-dialog" role="document">
				<div class="modal-content">
					<div class="modal-header">
						<h4 class="modal-title">
							<span>Edición E.Vasculares </span> 
						</h4>
						<button type="button" class="close" data-dismiss="modal" aria-label="Close">
							<i class="fas fa-times"></i>
						</button>					
					</div>
					<div class="modal-body row">
							<div class="col-lg-6 col-md-6 mb-lg-3 mb-3">
								<input autocomplete="off" type="text" required placeholder=" " id="NF" value="${Intl.NumberFormat("de-DE").format(Datos.datosFilaTabla[0]["NF"])}">
								<label>NF</label>
								<div id="invalidFeedback_NF"></div>
							</div>
							<div class="container_Select col-lg-6 col-md-6 mb-lg-3 mb-3">
								<select id="FIT" class='selectpicker'>
									<option ${Datos.datosFilaTabla[0]["FIT"]== "Si" ? "selected" : ""} value="1">Si</option>
									<option ${Datos.datosFilaTabla[0]["FIT"]== "No" ? "selected" : ""} value="2">No</option>
								</select>
								<label>FIT</label>
								<!-- Contenedor para mensajes de error -->
								<div id="invalidFeedback_FIT"></div>	
							</div>
							<div class="container_Select col-lg-6 col-md-6 mb-lg-3 mb-3">
								<select id="RTI" class='selectpicker'>
									<option ${Datos.datosFilaTabla[0]["RTI"]== "Si" ? "selected" : ""} value="1">Si</option>
									<option ${Datos.datosFilaTabla[0]["RTI"]== "No" ? "selected" : ""} value="2">No</option>
								</select>
								<label>RTI</label>
								<!-- Contenedor para mensajes de error -->
								<div id="invalidFeedback_RTI"></div>	
							</div>
							<div class="col-lg-12 col-md-12 mb-lg-6 mb-6">
								<textarea autocomplete="off" required placeholder=" " id="CFIT">${Datos.datosFilaTabla[0]["Comentario FIT"] == null ? "" : Datos.datosFilaTabla[0]["Comentario FIT"]}</textarea>
								<label>Comentario FIT</label>
								<div id="invalidFeedback_CFIT"></div>
							</div>
							<div class="col-lg-12 col-md-12 mb-lg-6 mb-6">
								<textarea autocomplete="off" required placeholder=" " id="CRTI">${Datos.datosFilaTabla[0]["Comentario RTI"] == null ? "" : Datos.datosFilaTabla[0]["Comentario RTI"]}</textarea>
								<label>Comentario RTI</label>
								<div id="invalidFeedback_CRTI"></div>
							</div>
						</div>
					<div class="modal-footer">
						<button class="btn btn-md btn-primary" data-dismiss="modal">
							<i class="fas fa-arrow-circle-left"></i>
							<span>Cancelar</span>		      	
						</button>
						<button id="editar" class="btn btn-md btn-success">
							<i class="far fa-check-circle"></i>
							<span>Editar</span>
						</button>
					</div>
				</div>
			</div>
		</div>
		`;
		
		$("#contenedorDatos").append(html);
		$('#modal').modal('show');

		gestionImask.cualquierNumeroDecimal("#NF", 0, 0, 999.99, 2);

		//Aplica el estilo de selectores 
		gestionBSelect.generic3(`#FIT`,'#modal'," ");
		gestionBSelect.generic3(`#RTI`,'#modal'," ");
	
		//Se remueben los tooltip de BSelect
		$(`#FIT`).removeAttr("title");
		$(`#RTI`).removeAttr("title");

		$('#editar').off().on("click", {index: index}, Datos.funciones.saveCulturales);

	}else if(index == 3){//tablaPrecipitaciones

		var html = `
		<div class="modal" id="modal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
			<div class="modal-dialog" role="document">
				<div class="modal-content">
					<div class="modal-header">
						<h4 class="modal-title">
							<span>Edición Precipitaciones </span> 
						</h4>
						<button type="button" class="close" data-dismiss="modal" aria-label="Close">
							<i class="fas fa-times"></i>
						</button>					
					</div>
					<div class="modal-body row">
						<div class="col-lg-12 mb-3">
							<input autocomplete="off" type="text" required placeholder=" " id="precipitacion" value="${Intl.NumberFormat("de-DE").format(Datos.datosFilaTabla[0]["Precipitación"])}">
							<label>Precipitación</label>
							<div id="invalidFeedback_precipitacion"></div>
						</div>
					<div class="modal-footer">
						<button class="btn btn-md btn-primary" data-dismiss="modal">
							<i class="fas fa-arrow-circle-left"></i>
							<span>Cancelar</span>		      	
						</button>
						<button id="editar" class="btn btn-md btn-success">
							<i class="far fa-check-circle"></i>
							<span>Editar</span>
						</button>
					</div>
				</div>
			</div>
		</div>
		`;
		
		$("#contenedorDatos").append(html);
		$('#modal').modal('show');

		gestionImask.cualquierNumeroDecimal("#precipitacion", 0, 0, 999.99, 2);

		$('#editar').off().on("click", {index: index}, Datos.funciones.savePrecipitaciones);

		$(".modal-dialog").addClass("modal-dialog-2");

	}
	else { }
}

//Se redibuja la tabla cuando se cambia la seleccion
Datos.gestionDOM.genTable = function(){
	//Se limpia el contenedor
	$('#tableDatos').empty();

	var txt = `
	<div id="scroller-left" class="scroller scroller-left"><i class="fas fa-angle-left"></i></div>
	<div id="scroller-right" class="scroller scroller-right"><i class="fas fa-angle-right"></i></div>
	<div id="navWrapper" class="wrapper">
		<nav id='nav-wrapper-balance' class="nav nav-tabs list">
			<a class="nav-tabs nav-link active" data-toggle="tab" href="#sigatoka">Sigatoka</a>
			<a class="nav-tabs nav-link" data-toggle="tab" href="#vasculares">E. Vasculares</a>
			<a class="nav-tabs nav-link" data-toggle="tab" href="#culturales">C. Culturales</a>
			<a class="nav-tabs nav-link" data-toggle="tab" href="#tableDatos">Precipitaciones</a>
		</nav>
	</div>
	<div class="tab-content">
		<div id="sigatoka" class="tab-pane active">
			<div id="divtabla_0" class="dataTableStyle">		
				<!--para configurar opciones con los data-->
				<!-- Por defecto siempre se organiza por la primera columna asc -->
				<table id="tablaSigatoka" class="table-striped table-bordered" data-order='[[ 1, "asc" ]]' data-page-length='100'>
				</table> <!-- el id de la tabla si se puede cambiar -->
			</div>
		</div>
		<div id="vasculares" class="tab-pane fade">
			<div id="divtabla_1" class="dataTableStyle">		
				<!--para configurar opciones con los data-->
				<!-- Por defecto siempre se organiza por la primera columna asc -->
				<table id="tablaVasculares" class="table-striped table-bordered" data-order='[[ 1, "asc" ]]' data-page-length='100'>
				</table> <!-- el id de la tabla si se puede cambiar -->
			</div>
		</div>
		<div id="culturales" class="tab-pane fade">
			<div id="divtabla_2" class="dataTableStyle">		
				<!--para configurar opciones con los data-->
				<!-- Por defecto siempre se organiza por la primera columna asc -->
				<table id="tablaCulturales" class="table-striped table-bordered" data-order='[[ 1, "asc" ]]' data-page-length='100'>
				</table> <!-- el id de la tabla si se puede cambiar -->
			</div>
		</div>
		<div id="tableDatos" class="tab-pane fade">
			<div id="divtabla_3" class="dataTableStyle">		
				<!--para configurar opciones con los data-->
				<!-- Por defecto siempre se organiza por la primera columna asc -->
				<table id="tablaPrecipitaciones" class="table-striped table-bordered" data-order='[[ 1, "asc" ]]' data-page-length='100'>
				</table> <!-- el id de la tabla si se puede cambiar -->
			</div>
		</div>
	</div>
	`;

	//Se dibuja el div de data table
	$('#tableDatos').append(txt);

	//Al clickar cualquier elemento tr dentro de tbody 
    $("#tablaSigatoka, #tablaVasculares, #tablaCulturales, #tablaPrecipitaciones").on("click","tbody tr", Datos.funciones.clickSelected);

	$("#principal").css("display","none");

	//evento risize cada que se cambia de tabla
	$(".nav-tabs").off('click').on('click', Datos.gestionDOM.resize)

	Datos.funciones.genTable1();
	Datos.funciones.genTable2();
	Datos.funciones.genTable3();
	Datos.funciones.genTable4();

	$(window).off('resize').on('resize', Datos.gestionDOM.reAdjust);

	//desplaza el navbar a la derecha
	$(`#scroller-right`).off('click').on('click', Datos.gestionDOM.scrollerRight);
	//desplaza el navbar a la izquierda
	$(`#scroller-left`).off('click').on('click', Datos.gestionDOM.scrollerLeft);

	Datos.gestionDOM.reAdjust();
}

/*Esta funcion ajusta el navbar despues de moverse como tabn ajusta el legend y el navbar si se mueve 
el ancho de la pantalla. tambien se encarga del ancho de cada culumna de filtros o .section
Solo esta enfocada al modo de uso de una grafica por link*/
Datos.gestionDOM.reAdjust = function(){
	//navbar
	if ( !(Datos.gestionDOM.widthOfList()-$(`#tableDatos`).width()-(-1*Datos.gestionDOM.getLeftPosi()) < 1) ) {
		$(`#scroller-right`).fadeIn(0);
	}
	else {
		$(`#scroller-right`).fadeOut(0);
	}

	if (Datos.gestionDOM.getLeftPosi() < 0) {
		$(`#scroller-left`).fadeIn(0);
	}
	else {
	$(`.wrapper nav .item`).animate({left:"-="+Datos.gestionDOM.getLeftPosi()+"px"},0);
		$(`#scroller-left`).fadeOut(0);
	}
}

//Funcion que retorna el ancho de la lista navbar
Datos.gestionDOM.widthOfList = function(){
	var itemsWidth = 0;
	$(`.wrapper .list a`).each(function(){
		var itemWidth = $(this).outerWidth();
		itemsWidth+=itemWidth;
	});
	return itemsWidth;
}

//Funcion que retorna el ancho de la lista navbar
Datos.gestionDOM.widthOfList = function(){
	var itemsWidth = 0;
	$(`.wrapper .list a`).each(function(){
		var itemWidth = $(this).outerWidth();
		itemsWidth+=itemWidth;
	});
	return itemsWidth;
}

//funcion que retorna la posicion a la izquierda del navbar (inicialmente es cero por que no se ha movido a la derecha)
Datos.gestionDOM.getLeftPosi = function(){
	return $(`.wrapper .list`).length > 0 ? $(`.wrapper .list`).position().left : 0;
}

/*Scroll horizontal*/
//Evento de desplazamiento del navbar a la izquierda
Datos.gestionDOM.scrollerRight = function(e) {
	//variable de cantidad a desplazar en el scroll horizontal
	var cant;
	cant = Datos.gestionDOM.widthOfList()-$(`#tableDatos`).width()-(-1*Datos.gestionDOM.getLeftPosi());
	if( cant >= 300){
		$(`#scroller-right`).off('click');
		$(`.wrapper .list`).animate({left:'+=-300px'},300);
		window.setTimeout("Datos.gestionDOM.onScrollerRight()", 350);
	}
	else{
		$(`#scroller-right`).off('click');
		$(`.wrapper .list`).animate({left:`+=-${cant+20}px`},300);
		window.setTimeout("Datos.gestionDOM.onScrollerRight()", 350);
	}
}

//Ajusta la pantalla segun corresponda despues del evento de mover navbar a la derecha
Datos.gestionDOM.onScrollerRight = function(){
	$(`#scroller-right`).off('click').on('click', Datos.gestionDOM.scrollerRight);
	Datos.gestionDOM.reAdjust();
}

//Evento de desplazamiento del navbar a la derecha
Datos.gestionDOM.scrollerLeft = function(e) {
	//variable de cantidad a desplazar en el scroll horizontal
	var cant;
	if (Datos.gestionDOM.getLeftPosi() < 0) {
		cant = -1*Datos.gestionDOM.getLeftPosi();
		if( cant > 300 ){
			$(`#scroller-left`).off('click');
			$(`.wrapper .list`).animate({left:'+=300px'},300);
			window.setTimeout("Datos.gestionDOM.onScrollerLeft()", 350);
		}
		else{
			$(`#scroller-left`).off('click');
			$(`.wrapper .list`).animate({left:`+=${cant}px`},300);
			window.setTimeout("Datos.gestionDOM.onScrollerLeft()", 350);
		}
	}
}

//Ajusta la pantalla segun corresponda despues del evento de mover navbar a la izquierda
Datos.gestionDOM.onScrollerLeft = function(){
	$(`#scroller-left`).off('click').on('click', Datos.gestionDOM.scrollerLeft);
	Datos.gestionDOM.reAdjust();
}
/*Scroll horizontal fin*/

/* SELECTORES ANIDADOS */
Datos.gestionDOM.initSelectores = function(){
	/* Estilo para selectores */
	gestionBSelect.generic3(`#selectDepartamento`, "#divSelectVista", 'Departamentos');
	gestionBSelect.generic3(`#selectMunicipio`, "#divSelectVista", 'Municipios');
	gestionBSelect.generic3(`#selectFinca`, "#divSelectVista", 'Fincas', undefined, false);

	//Funciones encargadas de llenar los selectores y mover banderas
	$('button[data-id="selectDepartamento"]').click(Datos.funciones.fillDepartamento);
	$('#selectDepartamento').change({select:"selectDepartamento"},Datos.funciones.changeFlag);
	$('button[data-id="selectMunicipio"]').click(Datos.funciones.fillMunicipio);
	$('#selectMunicipio').change({select:"selectMunicipio"},Datos.funciones.changeFlag);
	$('button[data-id="selectFinca"]').click(Datos.funciones.fillFinca);
	$('#selectFinca').change({select:"selectFinca"},Datos.funciones.changeFlag);

	//Se quita el tooltip por defecto de los selectores
	$(`[data-id="selectDepartamento"]`).removeAttr("title");
	$(`[data-id="selectMunicipio"]`).removeAttr("title");
	$(`[data-id="selectFinca"]`).removeAttr("title");
}

//Se dibujan los valores de los selectores
Datos.gestionDOM.fillSelectores = function(resultado, id){
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

//Habilita o deshabilita el boton
Datos.gestionDOM.enabledBtn = function(){
	if($(`#selectFinca`).val().length > 0){
		$("#cargar").prop("disabled", false);
	}else{
		$("#cargar").prop("disabled", true);
	}
}

/*Evento de click en calendario para alterar la forma en la que se abre el 
calendario si se encuentra en dispositivo mobil*/
Datos.gestionDOM.calendarMobil = function(){
    //REVISAR, AÚN FALTA
	if((/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent))){
		//Se limita el ancho de la ventana de fechas para dispositivos mobiles
		$("div.daterangepicker.opensright").css("top","0");
	}
}

Datos.gestionDOM.FechasLimitedRankGraph2 = function(tagDiv){
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
        Datos.fechaIniRank = start.format('YYYY-MM-DD');
        Datos.fechaFinRank = end.format('YYYY-MM-DD');
        $(`#${tagDiv} span`).html("Rango Visualización &nbsp; &nbsp; &nbsp; ");
    }

    $(`#${tagDiv}`).daterangepicker(option, cb);
    cb(start, end);
}

//ejcuta evento risize a la ventana para ajustar el datatable
Datos.gestionDOM.resize = function(){
	$(window).resize();
}

//********* Mensajes de error *************/
Datos.gestionDOM.mensajeError = function(tag_id, contenidoHTML){
	$(`${tag_id}`).append(contenidoHTML);	
}