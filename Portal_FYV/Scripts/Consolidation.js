
document.querySelector('#buscar').addEventListener('click', function () {
    // Selecciona el elemento por su ID (reemplaza 'elementoId' con el ID real del elemento)
    const elemento = document.querySelector('.validar-consolidacion');

    // Obtiene el valor del atributo data-Sucursal utilizando la propiedad dataset
    const sucursal = elemento.dataset.sucursal;
    getProducts(document.querySelector('input#texto').value, sucursal)
})

function saveREQDET() {
    let rEQDET = {}

    rEQDET = {
        "Id_REQDET": parseInt(Date.now()),
        "Id_REQHDR": parseInt(document.getElementById('select_Id_REQHDR').value),
        "Clave_articulo": parseInt(document.getElementById('Clave_articulo').value),
        "Descripcion": document.getElementById('Descripcion').value,
        "Cantidad_solicitada": parseFloat(document.getElementById('Cantidad_solicitada').value),
        "Cantidad_validada": parseFloat(document.getElementById('Cantidad_solicitada').value),
        "Id_Embalaje": parseInt(document.getElementById('Id_Embalaje').value),
        "Id_Embalaje_validado": parseInt(document.getElementById('Id_Embalaje').value)
    };
    console.log(rEQDET)
    // Enviar el arreglo de objetos al controlador utilizando AJAX
    /*
    */
    $.ajax({
        url: '/REQDETs/guardarREQDET',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({ rEQDET }),
        success: function (response) {
            // Manejar la respuesta del servidor si es necesario
            console.log('Datos enviados correctamente');
            console.log(response.value);
            let tabla = document.getElementById('productosConsolidados').querySelector('tbody');
            let reqdet = JSON.parse(response.value);
            let reqdet_relations = response.Message_data;

            // Buscar todos los tr en la tabla
            let trs = tabla.querySelectorAll('tr');
            let trExistente = null;

            // Iterar sobre cada tr para buscar la clave del artículo
            trs.forEach(function (tr) {
                let divClave = tr.querySelector('td:first-child div');
                if (divClave && divClave.textContent.trim() === reqdet.Clave_articulo) {
                    trExistente = tr;
                    return;
                }
            });

            if (trExistente) {
                // Agregar el nuevo contenido al tr existente
                //trExistente.innerHTML += `Nuevo contenido`; // Aquí coloca el nuevo contenido que deseas agregar
                let div = `<div class="d-flex my-1">
                                    <div class="d-flex gap-4">
                                        <b>${reqdet_relations.sucursal}:</b>
                                        <div class="d-flex flex-column gap-3">
                                            <span class="embalaje_${reqdet.Id_REQDET}">&nbsp;${reqdet_relations.embalaje}</span>
                                            <select class="form-control" id="id_EmbalajeValidado_${reqdet.Id_REQDET}" name="id_EmbalajeValidado_${reqdet.Id_REQDET}"><option value="">Seleccione un tipo de embalaje</option>
                                            ${document.getElementById('Id_Embalaje').innerHTML}
                                            </select>
                                        </div>
                                        <div class="d-flex flex-column gap-3">
                                            <span class="cantidad_${reqdet.Id_REQDET}">&nbsp;${reqdet.Cantidad_solicitada}</span>
                                            <input type="number" class="form-control text-box single-line" name="cantidad_Validada_${reqdet.Id_REQDET}" id="cantidad_Validada_${reqdet.Id_REQDET}" value="${reqdet.Cantidad_validada}">
                                        </div>
                                    </div>
                                    <div class="ms-auto d-flex gap-2">
                                        <div>
                                            <button class="btn btn-sm btn-primary rounded-1 m-0 w-auto" type="button" onclick="confirmEdit(${reqdet.Id_REQDET}, this)"><i class="bi bi-pencil"></i></button>
                                        </div>
                                        <div>
                                            <div class="m-0 w-auto">
                                                <button class="btn btn-sm btn-danger rounded-1 w-auto" type="button" onclick="deleteREQDET(${reqdet.Id_REQDET}, this, '${reqdet_relations.sucursal}')"><i class="bi bi-trash"></i></button>
                                            </div>
                                        </div>
                                    </div>
                                </div>`
                trExistente.children[1].children[0].children[1].children[0].children[0].children[0].innerHTML += div

                let span_class
                if (reqdet_relations.sucursal == "JUA") {
                    span_class = "text-bg-primary"
                }
                else if (reqdet_relations.sucursal == "GUA") {
                    span_class = "text-bg-secondary"
                }
                else if (reqdet_relations.sucursal == "OFE") {
                    span_class = "text-bg-success"
                }
                else if (reqdet_relations.sucursal == "BAL") {
                    span_class = "text-bg-danger"
                }
                else if (reqdet_relations.sucursal == "GTO") {
                    span_class = "text-bg-warning"
                }
                else if (reqdet_relations.sucursal == "CDI") {
                    span_class = "text-bg-info"
                }
                else if (reqdet_relations.sucursal == "JAR") {
                    span_class = "text-bg-light"
                }
                else if (reqdet_relations.sucursal == "AMG") {
                    span_class = "text-bg-dark"
                }

                let span = `<span class="badge rounded-pill ${span_class} p-2 m-1 sucursal_${reqdet.Id_REQDET}">${reqdet_relations.sucursal} - ${reqdet.Cantidad_validada}</span>`

                trExistente.children[2].children[0].innerHTML += span
                trExistente.children[3].children[0].innerHTML = (parseFloat(trExistente.children[3].children[0].innerHTML) + parseFloat(reqdet.Cantidad_validada)).toFixed(4);
            } else {
                let tr = `<tr class="py-3" id="tr_${reqdet.Id_REQHDR}">
                                    <td>
                                        <div class="my-0">
                                            ${reqdet.Clave_articulo}
                                        </div>
                                    </td>

                                    <td>
                                        <div class="my-0">
                                            <div class="accordion accordion-flush" id="accordionFlush-${reqdet.Id_REQDET}">
                                                <div class="accordion-item">
                                                    <h2 class="accordion-header">
                                                        <button class="accordion-button" type="button" data-bs-toggle="collapse" data-bs-target="#flush-${reqdet.Id_REQDET}" aria-expanded="true" aria-controls="flush-${reqdet.Id_REQDET}">
                                                            ${reqdet.Descripcion}
                                                        </button>
                                                    </h2>
                                                </div>
                                            </div>
                                            <div id="flush-${reqdet.Id_REQDET}" class="accordion-collapse collapse show" data-bs-parent="#accordionFlush-${reqdet.Id_REQDET}" style="">
                                                <div class="accordion-body">
                                                    <div class="p-2 bg-light">
                                                        <div class="d-flex flex-column gap-4 m-1">
                                                                <div class="d-flex my-1">
                                                                    <div class="d-flex gap-4">
                                                                        <b>${reqdet_relations.sucursal}:</b>
                                                                        <div class="d-flex flex-column gap-3">
                                                                            <span class="embalaje_${reqdet.Id_REQDET}">&nbsp;${reqdet_relations.embalaje}</span>
                                                                            <select class="form-control" id="id_EmbalajeValidado_${reqdet.Id_REQDET}" name="id_EmbalajeValidado_${reqdet.Id_REQDET}"><option value="">Seleccione un tipo de embalaje</option>
                                                                            ${document.getElementById('Id_Embalaje').innerHTML}
                                                                            </select>
                                                                        </div>
                                                                        <div class="d-flex flex-column gap-3">
                                                                            <span class="cantidad_${reqdet.Id_REQDET}">&nbsp;${reqdet.Cantidad_solicitada}</span>
                                                                            <input type="number" class="form-control text-box single-line" name="cantidad_Validada_${reqdet.Id_REQDET}" id="cantidad_Validada_${reqdet.Id_REQDET}" value="${reqdet.Cantidad_validada}">
                                                                        </div>
                                                                    </div>
                                                                    <div class="ms-auto d-flex gap-2">
                                                                        <div>
                                                                            <button class="btn btn-sm btn-primary rounded-1 m-0 w-auto" type="button" onclick="confirmEdit(${reqdet.Id_REQDET}, this)"><i class="bi bi-pencil"></i></button>
                                                                        </div>
                                                                        <div>
                                                                            <div class="m-0 w-auto">
                                                                                <button class="btn btn-sm btn-danger rounded-1 w-auto" type="button" onclick="deleteREQDET(${reqdet.Id_REQDET}, this, '${reqdet_relations.sucursal}')"><i class="bi bi-trash"></i></button>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>

                                        </div>
                                    </td>

                                    <td>
                                        <div class="my-0">
                                            <span class="badge rounded-pill text-bg-primary p-2 m-1 sucursal_${reqdet.Id_REQDET}">${reqdet_relations.sucursal} - ${reqdet.Cantidad_validada}</span>
                                        </div>
                                    </td>

                                    <td>
                                        <div class="my-0">
                                           ${reqdet.Cantidad_validada}
                                        </div>
                                    </td>
                                </tr>`
                tabla.innerHTML += tr
            }
            sumarUltimosTd()
        },
        error: function (xhr, status, error) {
            // Manejar errores si ocurrieron durante la solicitud AJAX
            console.error('Error al enviar datos:', error);
        }
    });
}

function confirmEdit(id, btn) {
    let rEQDET = {
        "Id_REQDET": id,
        "Cantidad_validada": parseFloat(btn.closest('.accordion-body').querySelector('#cantidad_Validada_' + id).value),
        "Id_Embalaje_validado": parseInt(btn.closest('.accordion-body').querySelector('#id_EmbalajeValidado_' + id).value)
    };
    // Mostrar el mensaje de confirmación
    var confirmacion = confirm("¿Estás seguro de que deseas editar esto?");

    // Verificar si el usuario confirmó la eliminación
    if (confirmacion) {
        // Aquí colocas el código para eliminar
        //alert("El elemento ha sido eliminado."); // Solo un mensaje de ejemplo
        // Enviar el arreglo de objetos al controlador utilizando AJAX
        $.ajax({
            url: '/REQDETs/EditarCantidadValidada',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ rEQDET }),
            success: function (response) {
                // Manejar la respuesta del servidor si es necesario
                console.log('Datos enviados correctamente');
                console.log(response);
                let embalaje_val = document.querySelector('.embalaje_' + rEQDET.Id_REQDET)
                let cantidad_val = document.querySelector('.cantidad_' + rEQDET.Id_REQDET)
                let cantidad_suc = document.querySelector('.sucursal_' + rEQDET.Id_REQDET)

                if (rEQDET.Id_Embalaje_validado) {
                    embalaje_val.innerHTML = "&nbsp;" + document.getElementById('id_EmbalajeValidado_' + rEQDET.Id_REQDET).querySelector("option[value='" + rEQDET.Id_Embalaje_validado + "']").textContent
                }

                if (rEQDET.Cantidad_validada) {
                    cantidad_val.innerHTML = "&nbsp;" + rEQDET.Cantidad_validada
                    let prev_cant_val = parseFloat(cantidad_suc.innerText.split('-')[1])
                    cantidad_suc.innerHTML = cantidad_suc.innerText.split('-')[0] + ' - ' + rEQDET.Cantidad_validada
                    let sum = btn.closest('tr').children[btn.closest('tr').childElementCount - 1]
                    sum.innerText = (parseFloat(sum.innerText) - prev_cant_val) + rEQDET.Cantidad_validada
                }
                sumarUltimosTd()
            },
            error: function (xhr, status, error) {
                // Manejar errores si ocurrieron durante la solicitud AJAX
                console.error('Error al enviar datos:', error);
            }
        });
    } else {
        // El usuario canceló la eliminación
        alert("La edición ha sido cancelada."); // Solo un mensaje de ejemplo
    }
}

function deleteREQDET(id, btn, sucursal) {
    // Mostrar el mensaje de confirmación
    var confirmacion = confirm("¿Estás seguro de que deseas eliminar esto?");
    // Verificar si el usuario confirmó la eliminación
    if (confirmacion) {
        // Aquí colocas el código para eliminar
        //alert("El elemento ha sido eliminado."); // Solo un mensaje de ejemplo
        // Enviar el arreglo de objetos al controlador utilizando AJAX
        $.ajax({
            url: '/REQDETs/DeleteREQDET',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ id }),
            success: function (response) {
                // Manejar la respuesta del servidor si es necesario
                console.log(response.Message);
                console.log(response);
                let tr = btn.closest('tr')
                let suc = tr.querySelector('.sucursal_' + id)

                tr.children[3].innerText = (parseFloat(tr.children[3].innerText) - parseFloat(suc.innerText.split('-')[1])).toFixed(4)

                $(suc).fadeOut();
                $(btn.parentNode.parentNode.parentNode.parentNode).slideUp();
                btn.parentNode.parentNode.parentNode.parentNode.classList.remove('d-flex')
                sumarUltimosTd()
            },
            error: function (xhr, status, error) {
                // Manejar errores si ocurrieron durante la solicitud AJAX
                console.error('Error al enviar datos:', error);
            }
        });
    } else {
        // El usuario canceló la eliminación
        alert("La eliminación ha sido cancelada."); // Solo un mensaje de ejemplo
    }
}

function sumarUltimosTd() {
    // Obtener todas las filas de la tabla
    var filas = document.querySelectorAll('#productosConsolidados tbody tr');

    // Inicializar la suma
    var suma = 0;

    // Iterar sobre cada fila
    filas.forEach(function (fila) {
        // Obtener el último td de la fila
        var ultimoTd = fila.lastElementChild;

        // Obtener el contenido del td como número y sumarlo
        var contenido = parseFloat(ultimoTd.textContent.trim());
        if (!isNaN(contenido)) {
            suma += contenido;
        }
    });

    // Retornar la suma
    document.querySelector('.total-sum').innerText = suma.toFixed(4);

}

function saveREQHDRS(opt) {
    let ids = document.getElementById('Id_REQHDRs').value.split(',');
    let rhs = ids.map(id => ({ "Id_REQHDR": parseInt(id) }));
    //console.log(rhs);

    // Enviar el arreglo de objetos al controlador utilizando AJAX
    $.ajax({
        url: '/REQHDRs/ValidarREQHDR',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({ rhs, opt }),
        success: function (response) {
            // Manejar la respuesta del servidor si es necesario
            console.log('Datos enviados correctamente');
            console.log(response);
            document.querySelector('.validar-consolidacion').children[0].children[0].children[2].innerHTML = response.Message
            document.querySelector('.validar-consolidacion').children[0].children[0].children[2].className += response.Message_Classes
        },
        error: function (xhr, status, error) {
            // Manejar errores si ocurrieron durante la solicitud AJAX
            console.error('Error al enviar datos:', error);
        }
    });
}