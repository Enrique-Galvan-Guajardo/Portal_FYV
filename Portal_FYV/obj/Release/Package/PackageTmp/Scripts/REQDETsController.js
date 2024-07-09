let dataProductos = ''

function getProducts(producto, clave) {
    //$.get(getProductSearch + producto + "&clave_sucursal=" + clave, function (data) {
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

    let fila
    // Limpiar la tabla antes de agregar nuevos datos
    //tabla.innerHTML = '';
    if (tablaId == 'productosSeleccionados') {

        // Crear una fila para cada producto y agregarla a la tabla
        fila = '<tr class="alert alert-dismissible fade show" id="agregar-' + mdl_item_selected.querySelector('#Clave_articulo').value + '">'
            + '<td>' + mdl_item_selected.querySelector('#Clave_articulo').value + '</td>'
            + '<td>' + mdl_item_selected.querySelector('#Descripcion').value + '</td>'
            + '<td>' + mdl_item_selected.querySelector('#Cantidad_solicitada').value + '</td>'
            + '<td>' + mdl_item_selected.querySelector('#Cantidad_ultima_compra').value + '</td>'
            + '<td>' + mdl_item_selected.querySelector('#Existencia').value + '</td>'
            + '<td data-embalaje="' + mdl_item_selected.querySelector('#Id_Embalaje').value + '">' + mdl_item_selected.querySelector('#Id_Embalaje').children[mdl_item_selected.querySelector('#Id_Embalaje').selectedIndex].innerText + '</td>'
            + '<td><button type="button" class="btn btn-danger" data-bs-dismiss="alert" aria-label="Close"><i class="bi bi-trash"></i></button></td>'
            + '</tr>'

        // Agregar la fila a la tabla
        tabla.innerHTML += fila;
    } else if (tablaId == 'productosValidados') {
        // Crear una fila para cada producto y agregarla a la tabla
        fila = `<tr class="newRow alert alert-dismissible fade show">
                    <td>
                        ${mdl_item_selected.querySelector('#Clave_articulo').value}
                    </td>
                    <td>
                        <input class="form-control text-box single-line" data-val="true" data-val-number="El campo Cantidad debe ser un número." id="item_Cantidad" name="item.Cantidad" type="text" value="${mdl_item_selected.querySelector('#Cantidad_solicitada').value}" placeholder="Cantidad solicitada">
                    </td>
                    <td>
                        <input class="form-control text-box single-line" data-val="true" data-val-length="El campo Descripcion debe ser una cadena con una longitud máxima de 60." data-val-length-max="60" data-val-required="El campo Descripcion es obligatorio." value="${mdl_item_selected.querySelector('#Descripcion').value}" id="item_Descripcion" name="item.Descripcion" type="text" placeholder="Descripción">
                    </td>
                    <td>
                        ${document.querySelector('.table tbody tr td select').outerHTML}
                    </td>
                    <td><button type="button" class="btn btn-danger" data-bs-dismiss="alert" aria-label="Close"><i class="bi bi-trash"></i></button></td>
                </tr>`
        // Agregar la fila a la tabla
        tabla.innerHTML += fila;
        let sel = tabla.lastElementChild.querySelector('#item_Id_Embalaje_validado')
        sel.setAttribute('data-id', '')
        sel.setAttribute('value', mdl_item_selected.querySelector('#Id_Embalaje').value)
        sel.selectedIndex = mdl_item_selected.querySelector('#Id_Embalaje').selectedIndex + 1
    }

    document.getElementById('Cantidad_solicitada').value = 0
    document.getElementById('Id_Embalaje').selectedIndex = 0

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


function saveREQDETS() {
    let table = document.querySelector('#productosSeleccionados tbody');

    rs = [];
    let sucursal = document.getElementById('sucursal').value
    table.childNodes.forEach(tr => {
        // Crear un objeto y añadirlo al arreglo
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

    // Enviar el arreglo de objetos al controlador utilizando AJAX
    $.ajax({
        url: '/REQDETs/CreateREQDETS',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({ rs, sucursal }),
        success: function (response) {
            // Manejar la respuesta del servidor si es necesario
            //console.log('Datos enviados correctamente');
            console.log(response);
            toastFill(response);

            if ($('#productosSeleccionados')) {
                document.getElementById('productosSeleccionados').querySelector('tbody').innerHTML = "";
            }
        },
        error: function (xhr, status, error) {
            // Manejar errores si ocurrieron durante la solicitud AJAX
            console.error('Error al enviar datos:', error);
        }
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
