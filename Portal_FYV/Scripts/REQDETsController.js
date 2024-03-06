let dataProductos = ''

function getProducts(producto, clave) {
    $.get(getProductSearch + producto + "&clave_sucursal=" + clave, function (data) {
        // La función de callback se ejecutará cuando la solicitud se complete exitosamente
        dataProductos = data;
        console.log("Productos recibidos:", data);

        // Aquí puedes realizar cualquier operación que necesites hacer con la respuesta.
        // Por ejemplo, podrías llamar a una función que procese los datos.
        imprimirEnTablaAgregar(dataProductos, "tablaProductos")
    }).fail(function (jqXHR, textStatus, errorThrown) {
        // Si la solicitud falla, puedes manejar el error aquí.
        dataProductos = '[{"cve_art":"46829","existencia":0.0000,"descripcion":"MANGO PICADO EN CHAROLA                                     ","cant":0.0,"fec":"","CantidadVendida":0.0,"Cantidad_Real":0.0},{"cve_art":"124","existencia":0.0000,"descripcion":"MANGO MANILILLA KG                                          ","cant":0.0,"fec":"","CantidadVendida":0.0,"Cantidad_Real":0.0},{"cve_art":"28942","existencia":0.0000,"descripcion":"MANGO AHOGADO GRANEL                                        ","cant":0.0,"fec":"","CantidadVendida":0.0,"Cantidad_Real":0.0},{"cve_art":"47","existencia":0.0000,"descripcion":"MANGO ATAULFO  KG                                           ","cant":0.0,"fec":"","CantidadVendida":325.9100,"Cantidad_Real":0.0},{"cve_art":"48","existencia":0.0000,"descripcion":"MANGO HADEN                                                 ","cant":0.0,"fec":"","CantidadVendida":0.0,"Cantidad_Real":0.0},{"cve_art":"49","existencia":0.0000,"descripcion":"MANGO MANILA                                                ","cant":0.0,"fec":"","CantidadVendida":0.0,"Cantidad_Real":0.0},{"cve_art":"50","existencia":0.0000,"descripcion":"MANGO ORO                                                   ","cant":0.0,"fec":"","CantidadVendida":0.0,"Cantidad_Real":0.0},{"cve_art":"51","existencia":78.0550,"descripcion":"MANGO TOMY KG                                               ","cant":0.0,"fec":"","CantidadVendida":7.4150,"Cantidad_Real":0.0},{"cve_art":"55","existencia":0.0000,"descripcion":"MANGO KENT                                                  ","cant":0.0,"fec":"","CantidadVendida":0.0,"Cantidad_Real":0.0},{"cve_art":"669","existencia":0.0000,"descripcion":"FRUTA DESIDRATADA MANGO IMPORTADO                           ","cant":0.0,"fec":"","CantidadVendida":0.0,"Cantidad_Real":0.0},{"cve_art":"670","existencia":0.0000,"descripcion":"FRUTA MANGO C/CHILE                                         ","cant":0.0,"fec":"","CantidadVendida":0.0,"Cantidad_Real":0.0},{"cve_art":"825","existencia":0.0000,"descripcion":"SNACK MANGO ENCHILADO                                       ","cant":0.0,"fec":"","CantidadVendida":0.0,"Cantidad_Real":0.0}]';
        console.log("Productos recibidos:", JSON.parse(dataProductos));
        imprimirEnTablaAgregar(dataProductos, "tablaProductos")
    });
}


function imprimirEnTablaAgregar(data, tablaId) {
    data = JSON.parse(data)
    let tabla = document.getElementById(tablaId).querySelector('tbody');

    // Limpiar la tabla antes de agregar nuevos datos
    tabla.innerHTML = '';

    // Crear una fila para cada producto y agregarla a la tabla
    data.forEach(producto => {
        let fila = '<tr id="agregar-' + producto.cve_art + '">'
                        + '<td>' + producto.cve_art + '</td>'
                        + '<td>' + producto.descripcion + '</td>'
                        + '<td>' + producto.existencia + '</td>'
                        + '<td>'
                            + '<button class="btn btn-primary" onclick="getTablaProductosItem(this)" data-cveart="' + producto.cve_art + '" data-description="' + producto.descripcion + '" data-existencia="' + producto.existencia + '" data-bs-target="#productoElegido" data-bs-toggle="modal">Elegir</button>'
                        + '</td>'
                    + '</tr>'
        // Agregar la fila a la tabla
        tabla.innerHTML += fila;
    });

    tabla.closest('div.modal-content').querySelector('.modal-footer').innerHTML = data.length > 0 ? '<div class="alert alert-warning my-2 w-100" role="alert">Se ha encontrado más de 1 artículo. Elija uno para continuar.</div>' : '<div class="alert alert-warning my-2 w-100" role="alert">Producto no encontrado</div>';
}

function imprimirEnTabla(data, tablaId) {
    let tabla = document.getElementById(tablaId).querySelector('tbody');

    // Limpiar la tabla antes de agregar nuevos datos
    tabla.innerHTML = '';

    // Crear una fila para cada producto y agregarla a la tabla
    data.forEach(producto => {
        let fila = `
        <tr id="agregar-` + producto.cve_art + `">
            <td>` + producto.cve_art + `<td/>
            <td>` + producto.descripcion + `<td/>
            <td>` + producto.cant + `<td/>
            <td>` + producto.fec + `<td/>
            <td>` + producto.existencia + `<td/>
            <td class="embalaje"><td/>
        <tr/>`

        // Agregar la fila a la tabla
        tabla.innerHTML += fila;
    });
}

function getTablaProductosItem(element) {

    let mdl_item_selected = document.querySelector('#productoElegido');
    
    mdl_item_selected.querySelector('#Clave_articulo').value = element.dataset.cveart.trim();
    mdl_item_selected.querySelector('#Descripcion').value = element.dataset.description.trim();
    mdl_item_selected.querySelector('#Existencia').value = parseInt(element.dataset.existencia);

    console.log(element)
}
