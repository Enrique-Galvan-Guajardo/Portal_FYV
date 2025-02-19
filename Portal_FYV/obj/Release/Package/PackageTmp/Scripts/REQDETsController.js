let dataProductos = ''

function getProducts(producto, clave) {
    //$.get(getProductSearch + producto + "&clave_sucursal=" + clave, function (data) {
    $.ajax({
        url: getAllProductsHTTPS,
        type: "GET",
        data: { urlAPI: getAllProducts + clave },
        success: function (response) {
            // La función de callback se ejecutará cuando la solicitud se complete exitosamente
            dataProductos = response;
            //console.log("Productos recibidos:", data);

            // Aquí puedes realizar cualquier operación que necesites hacer con la respuesta.
            // Por ejemplo, podrías llamar a una función que procese los datos.
            imprimirEnTablaAgregar(dataProductos, "tablaProductos")
        },
        error: function (xhr, status, error) {
            // Si la solicitud falla, puedes manejar el error aquí.
            //dataProductos = '[{"cve_art":"46829","existencia":0.0000,"descripcion":"MANGO PICADO EN CHAROLA                                     ","cant":0.0,"fec":"","CantidadVendida":0.0,"Cantidad_Real":0.0,"fecha_consulta":"21/03/2024 18:45:12","linea":"FYVE","sublinea":"FYVE","familia":"FREC","subfamilia":"NOAP"},{"cve_art":"124","existencia":0.0000,"descripcion":"MANGO MANILILLA KG                                          ","cant":0.0,"fec":"","CantidadVendida":0.0,"Cantidad_Real":0.0,"fecha_consulta":"21/03/2024 18:45:12","linea":"FYVE","sublinea":"FYVE","familia":"FRUT","subfamilia":"NOAP"},{"cve_art":"28942","existencia":0.0000,"descripcion":"MANGO AHOGADO GRANEL                                        ","cant":0.0,"fec":"","CantidadVendida":0.0,"Cantidad_Real":0.0,"fecha_consulta":"21/03/2024 18:45:13","linea":"FYVE","sublinea":"FYVE","familia":"FRUT","subfamilia":"NOAP"},{"cve_art":"47","existencia":0.0000,"descripcion":"MANGO ATAULFO  KG                                           ","cant":0.0,"fec":"","CantidadVendida":313.0400,"Cantidad_Real":0.0,"fecha_consulta":"21/03/2024 18:45:18","linea":"FYVE","sublinea":"FYVE","familia":"FRUT","subfamilia":"NOAP"},{"cve_art":"48","existencia":0.0000,"descripcion":"MANGO HADEN                                                 ","cant":0.0,"fec":"","CantidadVendida":0.0,"Cantidad_Real":0.0,"fecha_consulta":"21/03/2024 18:45:19","linea":"FYVE","sublinea":"FYVE","familia":"FRUT","subfamilia":"NOAP"},{"cve_art":"49","existencia":0.0000,"descripcion":"MANGO MANILA                                                ","cant":0.0,"fec":"","CantidadVendida":0.0,"Cantidad_Real":0.0,"fecha_consulta":"21/03/2024 18:45:19","linea":"FYVE","sublinea":"FYVE","familia":"FRUT","subfamilia":"NOAP"},{"cve_art":"50","existencia":0.0000,"descripcion":"MANGO ORO                                                   ","cant":0.0,"fec":"","CantidadVendida":0.0,"Cantidad_Real":0.0,"fecha_consulta":"21/03/2024 18:45:20","linea":"FYVE","sublinea":"FYVE","familia":"FRUT","subfamilia":"NOAP"},{"cve_art":"51","existencia":78.0550,"descripcion":"MANGO TOMY KG                                               ","cant":0.0,"fec":"","CantidadVendida":10.9150,"Cantidad_Real":0.0,"fecha_consulta":"21/03/2024 18:45:20","linea":"FYVE","sublinea":"FYVE","familia":"FRUT","subfamilia":"NOAP"},{"cve_art":"55","existencia":0.0000,"descripcion":"MANGO KENT                                                  ","cant":0.0,"fec":"","CantidadVendida":0.0,"Cantidad_Real":0.0,"fecha_consulta":"21/03/2024 18:45:21","linea":"FYVE","sublinea":"FYVE","familia":"FRUT","subfamilia":"NOAP"},{"cve_art":"669","existencia":0.0000,"descripcion":"FRUTA DESIDRATADA MANGO IMPORTADO                           ","cant":0.0,"fec":"","CantidadVendida":0.0,"Cantidad_Real":0.0,"fecha_consulta":"21/03/2024 18:45:21","linea":"PSEC","sublinea":"PSEC","familia":"PRSE","subfamilia":"NOAP"},{"cve_art":"670","existencia":0.0000,"descripcion":"FRUTA MANGO C/CHILE                                         ","cant":0.0,"fec":"","CantidadVendida":0.0,"Cantidad_Real":0.0,"fecha_consulta":"21/03/2024 18:45:22","linea":"PSEC","sublinea":"PSEC","familia":"PRSE","subfamilia":"NOAP"},{"cve_art":"825","existencia":0.0000,"descripcion":"SNACK MANGO ENCHILADO                                       ","cant":0.0,"fec":"","CantidadVendida":0.0,"Cantidad_Real":0.0,"fecha_consulta":"21/03/2024 18:45:22","linea":"PSEC","sublinea":"PSEC","familia":"PRSE","subfamilia":"NOAP"}]';
            dataProductos = `[]`;
            console.log("Productos recibidos:", JSON.parse(dataProductos));
            imprimirEnTablaAgregar(dataProductos, "tablaProductos")
        }
    });
    /*
    $.get(getAllProducts + clave, function (data) {

        // La función de callback se ejecutará cuando la solicitud se complete exitosamente
        dataProductos = data;
        //console.log("Productos recibidos:", data);

        // Aquí puedes realizar cualquier operación que necesites hacer con la respuesta.
        // Por ejemplo, podrías llamar a una función que procese los datos.
        imprimirEnTablaAgregar(dataProductos, "tablaProductos")
    }).fail(function (jqXHR, textStatus, errorThrown) {
        // Si la solicitud falla, puedes manejar el error aquí.
        //dataProductos = '[{"cve_art":"46829","existencia":0.0000,"descripcion":"MANGO PICADO EN CHAROLA                                     ","cant":0.0,"fec":"","CantidadVendida":0.0,"Cantidad_Real":0.0,"fecha_consulta":"21/03/2024 18:45:12","linea":"FYVE","sublinea":"FYVE","familia":"FREC","subfamilia":"NOAP"},{"cve_art":"124","existencia":0.0000,"descripcion":"MANGO MANILILLA KG                                          ","cant":0.0,"fec":"","CantidadVendida":0.0,"Cantidad_Real":0.0,"fecha_consulta":"21/03/2024 18:45:12","linea":"FYVE","sublinea":"FYVE","familia":"FRUT","subfamilia":"NOAP"},{"cve_art":"28942","existencia":0.0000,"descripcion":"MANGO AHOGADO GRANEL                                        ","cant":0.0,"fec":"","CantidadVendida":0.0,"Cantidad_Real":0.0,"fecha_consulta":"21/03/2024 18:45:13","linea":"FYVE","sublinea":"FYVE","familia":"FRUT","subfamilia":"NOAP"},{"cve_art":"47","existencia":0.0000,"descripcion":"MANGO ATAULFO  KG                                           ","cant":0.0,"fec":"","CantidadVendida":313.0400,"Cantidad_Real":0.0,"fecha_consulta":"21/03/2024 18:45:18","linea":"FYVE","sublinea":"FYVE","familia":"FRUT","subfamilia":"NOAP"},{"cve_art":"48","existencia":0.0000,"descripcion":"MANGO HADEN                                                 ","cant":0.0,"fec":"","CantidadVendida":0.0,"Cantidad_Real":0.0,"fecha_consulta":"21/03/2024 18:45:19","linea":"FYVE","sublinea":"FYVE","familia":"FRUT","subfamilia":"NOAP"},{"cve_art":"49","existencia":0.0000,"descripcion":"MANGO MANILA                                                ","cant":0.0,"fec":"","CantidadVendida":0.0,"Cantidad_Real":0.0,"fecha_consulta":"21/03/2024 18:45:19","linea":"FYVE","sublinea":"FYVE","familia":"FRUT","subfamilia":"NOAP"},{"cve_art":"50","existencia":0.0000,"descripcion":"MANGO ORO                                                   ","cant":0.0,"fec":"","CantidadVendida":0.0,"Cantidad_Real":0.0,"fecha_consulta":"21/03/2024 18:45:20","linea":"FYVE","sublinea":"FYVE","familia":"FRUT","subfamilia":"NOAP"},{"cve_art":"51","existencia":78.0550,"descripcion":"MANGO TOMY KG                                               ","cant":0.0,"fec":"","CantidadVendida":10.9150,"Cantidad_Real":0.0,"fecha_consulta":"21/03/2024 18:45:20","linea":"FYVE","sublinea":"FYVE","familia":"FRUT","subfamilia":"NOAP"},{"cve_art":"55","existencia":0.0000,"descripcion":"MANGO KENT                                                  ","cant":0.0,"fec":"","CantidadVendida":0.0,"Cantidad_Real":0.0,"fecha_consulta":"21/03/2024 18:45:21","linea":"FYVE","sublinea":"FYVE","familia":"FRUT","subfamilia":"NOAP"},{"cve_art":"669","existencia":0.0000,"descripcion":"FRUTA DESIDRATADA MANGO IMPORTADO                           ","cant":0.0,"fec":"","CantidadVendida":0.0,"Cantidad_Real":0.0,"fecha_consulta":"21/03/2024 18:45:21","linea":"PSEC","sublinea":"PSEC","familia":"PRSE","subfamilia":"NOAP"},{"cve_art":"670","existencia":0.0000,"descripcion":"FRUTA MANGO C/CHILE                                         ","cant":0.0,"fec":"","CantidadVendida":0.0,"Cantidad_Real":0.0,"fecha_consulta":"21/03/2024 18:45:22","linea":"PSEC","sublinea":"PSEC","familia":"PRSE","subfamilia":"NOAP"},{"cve_art":"825","existencia":0.0000,"descripcion":"SNACK MANGO ENCHILADO                                       ","cant":0.0,"fec":"","CantidadVendida":0.0,"Cantidad_Real":0.0,"fecha_consulta":"21/03/2024 18:45:22","linea":"PSEC","sublinea":"PSEC","familia":"PRSE","subfamilia":"NOAP"}]';
        dataProductos = `[]`;
        console.log("Productos recibidos:", JSON.parse(dataProductos));
        imprimirEnTablaAgregar(dataProductos, "tablaProductos")
    });
    */
}


function imprimirEnTablaAgregar(data, tablaId) {
    $(document.getElementById('tablaProductos')).slideDown();
    $(document.getElementById('spinner')).slideUp();
    data = JSON.parse(data)
    let tabla = document.getElementById(tablaId).querySelector('tbody');

    // Limpiar la tabla antes de agregar nuevos datos
    tabla.innerHTML = '';


    // Crear una fila para cada producto y agregarla a la tabla
    data.forEach(producto => {

        let is_frut = document.querySelector('.datasets').dataset.fru == "True" ? true : false
        let is_sec = document.querySelector('.datasets').dataset.sec == "True" ? true : false
        let is_verd = document.querySelector('.datasets').dataset.veg == "True" ? true : false
        
        if ((producto.familia == "FRUT" && is_frut) || ((producto.familia == "PRSE" || producto.familia == "PSEC") && is_sec) || (producto.familia == "VERD" && is_verd)) {
            let fila = '<tr id="obtenido-' + producto.cve_art.trim() + '" data-tipo="' + producto.familia + '">'
                            + '<td>' + producto.cve_art + '</td>'
                            + '<td>' + producto.descripcion + '</td>'
                            + '<td>' + producto.Cantidad_Real + '</td>'
                            + '<td>'
                            + '<button class="btn btn-primary d-flex w-100 justify-content-evenly" onclick="getTablaProductosItem(this)" data-cveart="' + producto.cve_art + '" data-description="' + producto.descripcion + '" data-existencia="' + producto.Cantidad_Real + '" data-fechaUltimaCompra="' + producto.fec + '" data-fechaExistencia="' + producto.fecha_consulta + '" data-bs-target="#productoElegido" data-bs-toggle="modal"><i class="bi bi-check2-circle me-2"></i>Elegir</button>'
                            + '</td>'
                        + '</tr>'
            // Agregar la fila a la tabla
            tabla.innerHTML += fila;
        }
    });
    tabla.closest('div.modal-content').querySelector('.modal-footer').innerHTML = tabla.childElementCount > 0 ? '<div class="alert alert-info my-2 w-100" role="alert">Se ha encontrado más de 1 artículo. Elija uno para continuar.</div>' : '<div class="alert alert-warning my-2 w-100" role="alert">Producto no encontrado</div>';
}

function imprimirEnTabla(tablaId) {
    let tabla = document.getElementById(tablaId).querySelector('tbody');
    let mdl_item_selected = document.querySelector('#productoElegido');
    let claveArticulo = mdl_item_selected.querySelector('#Clave_articulo').value;

    // Verificar si ya existe una fila con el mismo producto
    let filaExistente = tabla.querySelector(`#agregar-${claveArticulo}`);
    if (filaExistente) {
        toastFill({ Message_Classes: 'warning', Message: 'El producto ya ha sido agregado, si deseas cambiar la cantidad solicitada, elimina el registro existente de la tabla y vuelve a agregarlo.'});
        return; // Salir de la función si el producto ya está en la tabla
    }

    if (mdl_item_selected.querySelector('#Cantidad_solicitada').value <= 0) {
        toastFill({ Message_Classes: 'warning', Message: 'El producto debe tener una cantidad mayor a 0.' });
        return; // Salir de la función si el producto ya está en la tabla
    }

    let fila;
    if (tablaId == 'productosSeleccionados') {
        // Crear una fila para cada producto y agregarla a la tabla
        fila = `
            <tr class="alert alert-dismissible fade show" id="agregar-${claveArticulo}">
                <td>${claveArticulo}</td>
                <td>${mdl_item_selected.querySelector('#Descripcion').value}</td>
                <td>${mdl_item_selected.querySelector('#Cantidad_solicitada').value}</td>
                <td>${mdl_item_selected.querySelector('#Cantidad_ultima_compra').value}</td>
                <td>${mdl_item_selected.querySelector('#Existencia').value}</td>
                <td data-embalaje="${mdl_item_selected.querySelector('#Id_Embalaje').value}">
                    ${mdl_item_selected.querySelector('#Id_Embalaje').children[mdl_item_selected.querySelector('#Id_Embalaje').selectedIndex].innerText}
                </td>
                <td>
                    <button type="button" class="btn btn-danger" data-bs-dismiss="alert" aria-label="Close">
                        <i class="bi bi-trash"></i>
                    </button>
                </td>
            </tr>`;
        tabla.innerHTML += fila;
    } else if (tablaId == 'productosValidados') {
        fila = `
            <tr class="newRow alert alert-dismissible fade show" id="agregar-${claveArticulo}">
                <td>${claveArticulo}</td>
                <td>
                    <input class="form-control text-box single-line" 
                           data-val="true" 
                           data-val-number="El campo Cantidad debe ser un número." 
                           id="item_Cantidad" name="item.Cantidad" type="text" 
                           value="${mdl_item_selected.querySelector('#Cantidad_solicitada').value}" 
                           placeholder="Cantidad solicitada">
                </td>
                <td>
                    <input class="form-control text-box single-line" 
                           data-val="true" 
                           data-val-length="El campo Descripcion debe ser una cadena con una longitud máxima de 60." 
                           data-val-length-max="60" 
                           data-val-required="El campo Descripcion es obligatorio." 
                           value="${mdl_item_selected.querySelector('#Descripcion').value}" 
                           id="item_Descripcion" name="item.Descripcion" type="text" 
                           placeholder="Descripción">
                </td>
                <td>${document.querySelector('.table tbody tr td select').outerHTML}</td>
                <td>
                    <button type="button" class="btn btn-danger" data-bs-dismiss="alert" aria-label="Close">
                        <i class="bi bi-trash"></i>
                    </button>
                </td>
            </tr>`;
        tabla.innerHTML += fila;
        let sel = tabla.lastElementChild.querySelector('#item_Id_Embalaje_validado');
        sel.setAttribute('data-id', '');
        sel.setAttribute('value', mdl_item_selected.querySelector('#Id_Embalaje').value);
        sel.selectedIndex = mdl_item_selected.querySelector('#Id_Embalaje').selectedIndex + 1;
    }

    // Limpiar los campos de entrada
    document.getElementById('Cantidad_solicitada').value = 0;
    document.getElementById('Id_Embalaje').selectedIndex = 0;
}


function getTablaProductosItem(element) {

    let mdl_item_selected = document.querySelector('#productoElegido');
    
    mdl_item_selected.querySelector('#Clave_articulo').value = element.dataset.cveart.trim();
    mdl_item_selected.querySelector('#Descripcion').value = element.dataset.description.trim();
    mdl_item_selected.querySelector('#Existencia').value = parseInt(element.dataset.existencia);
    mdl_item_selected.querySelector('#Fecha_existencia').value = element.dataset.fechaexistencia.split(' ')[0].split('/').reverse().join('-');

    console.log(element)
}


// Declarar un arreglo vacío para almacenar objetos
var rs = [];


async function saveREQDETS() {
    let table = document.querySelector('#productosSeleccionados tbody');

    let rs = [];
    let sucursal = document.getElementById('sucursal').value;
    table.querySelectorAll('tr').forEach(tr => {
        let reqdet = {
            "Clave_articulo": tr.id.split('-')[1],
            "Descripcion": tr.children[1].innerText,
            "Cantidad_solicitada": tr.children[2].innerText,
            "Cantidad_ultima_compra": tr.children[3].innerText,
            "Existencia": tr.children[4].innerText,
            "Id_Embalaje": tr.children[5].dataset.embalaje
        };
        rs.push(reqdet);
    });

    let batchSize = 50;
    let chunks = [];
    for (let i = 0; i < rs.length; i += batchSize) {
        chunks.push(rs.slice(i, i + batchSize));
    }

    let Id_rh = 0;

    for (let i = 0; i < chunks.length; i++) {
        let chunk = chunks[i];
        try {
            let response = await sendBatch(chunk, sucursal, Id_rh);
            console.log(response);
            toastFill(response);
            Id_rh = response.Value;  // Actualizar el Id_rh para el siguiente lote

            if (i === chunks.length - 1 && response.Success) {
                sendEmail();
                document.getElementById('productosSeleccionados').querySelector('tbody').innerHTML = "";
            }
        } catch (error) {
            console.error('Error en el lote:', error);
        }
    }
}

async function sendBatch(chunk, sucursal, Id_rh) {
    return new Promise((resolve, reject) => {
        $.ajax({
            url: '/REQDETs/CreateREQDETS',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ rs: chunk, sucursal, Id_rh }),
            success: function (response) {
                resolve(response);  // Resolver la promesa con la respuesta
            },
            error: function (xhr, status, error) {
                reject(error);  // Rechazar la promesa en caso de error
            }
        });
    });
}


$(document).ready(function () {
    // Tu código aquí
    console.log('El DOM ha sido completamente cargado y parseado (jQuery).');
    document.getElementById('searchProduct').addEventListener('input', filterTable);
    document.querySelectorAll('input[name="options-base"]').forEach(function (radio) {
        radio.addEventListener('change', filterTable);
    });
});

function filterTable() {
    var filter = document.getElementById('searchProduct').value.toLowerCase();
    var selectedTipo = document.querySelector('input[name="options-base"]:checked').dataset.tipo;
    var rows = document.querySelectorAll('#tablaProductos tbody tr');

    rows.forEach(function (row) {
        var text = row.textContent.toLowerCase();
        var tipo = row.dataset.tipo;
        var textMatch = text.includes(filter);
        var tipoMatch = selectedTipo === "NA" || tipo === selectedTipo;

        if (textMatch && tipoMatch) {
            row.style.display = '';
        } else {
            row.style.display = 'none';
        }
    });

    if (filter === '' && selectedTipo === "NA") {
        rows.forEach(function (row) {
            row.style.display = '';
        });
    }
}

function exportButton() {

    let tbody = document.querySelector('#productosSeleccionados tbody');

    // Convertir el contenido del tbody a texto HTML
    let tbodyHtml = "<tbody>" + tbody.innerHTML + "</tbody>";

    // Crear un elemento de texto temporal para copiar el HTML al portapapeles
    let tempTextArea = document.createElement('textarea');
    tempTextArea.value = tbodyHtml;
    document.body.appendChild(tempTextArea);

    // Seleccionar y copiar el contenido
    tempTextArea.select();
    document.execCommand('copy');

    // Eliminar el elemento de texto temporal
    document.body.removeChild(tempTextArea);
    // Confirmación de copia exitosa
    toastFill({ Message_Classes: "info", Message: "El contenido de la tabla se ha copiado al portapapeles, guardalo en un block de notas para no perderlo." });
}

async function importButton() {
    try {
        // Obtener el contenido del portapapeles
        let clipboardText = document.querySelector('#paste-area');

        // Seleccionar el tbody donde se pegará el contenido
        let tbody = document.querySelector('#productosSeleccionados tbody');

        // Verificar si el contenido copiado es un HTML válido de tbody
        if (clipboardText.value.includes("<tr") && clipboardText.value.includes("<td")) {
            // Insertar el HTML copiado directamente en el tbody
            tbody.innerHTML = clipboardText.value;
            clipboardText.value = ""
            clipboardText.closest("details").children[0].click()
            toastFill({ Message_Classes: "success", Message: "Contenido pegado en la tabla." });
        } else {
            toastFill({ Message_Classes: "warning", Message: "El contenido del portapapeles no es apropiado para la tabla." });
        }
    } catch (error) {
        toastFill({ Message_Classes: "warning", Message: "Error al acceder al portapapeles." });
    }
}
// Función para descargar el archivo
function descargarPDF(base64, nombreArchivo = "documento.pdf") {
    // Crear un enlace temporal
    const enlace = document.createElement("a");

    // Convertir el Base64 a un archivo descargable
    enlace.href = `data:application/pdf;base64,${base64}`;
    enlace.download = nombreArchivo;

    // Simular el clic para descargar
    enlace.click();
}
function sendEmail() {
    let req = {
        arts_html: '<tbody>' + document.querySelector('#productosSeleccionados tbody').innerHTML + '</tbody>',
        usuario: document.querySelector('.capturarDetalles').dataset.nombre,
        sucursal: document.querySelector('.capturarDetalles').dataset.sucursalName
    }
    $.ajax({
        url: SendProductDataAPIHTTPS,
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({
            urlAPI: api + '/api/fyv/reporte_requisicion',
            arts_html: '<tbody>' + document.querySelector('#productosSeleccionados tbody').innerHTML + '</tbody>',
            usuario: document.querySelector('.capturarDetalles').dataset.nombre,
            sucursal: document.querySelector('#sucursal').children[document.querySelector('#sucursal').selectedIndex].innerText
        }),
        success: function (response) {
            // Manejar la respuesta del servidor si es necesario
            console.log('Datos enviados correctamente al correo');
            console.log(response);
            toastFill(response)

            if (response.Success) {
                descargarPDF(response.value, `Reporte ${document.querySelector('#sucursal').children[document.querySelector('#sucursal').selectedIndex].innerText}.pdf`)
            }
        },
        error: function (xhr, status, error) {
            // Manejar errores si ocurrieron durante la solicitud AJAX
            console.error('Error al enviar datos:', error);
        }
    });
}
function obtenerFechaActual() {
    const meses = [
        "enero", "febrero", "marzo", "abril", "mayo", "junio",
        "julio", "agosto", "septiembre", "octubre", "noviembre", "diciembre"
    ];

    const fecha = new Date();
    const dia = fecha.getDate();
    const mes = meses[fecha.getMonth()];
    const año = fecha.getFullYear();

    return `${dia} de ${mes} de ${año}`;
}


