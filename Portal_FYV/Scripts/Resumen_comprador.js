const exampleModal = document.getElementById('seleccionar-precio-modal')

const spanMaximo = document.querySelector('#seleccionar-cantidad-modal-Label span');

let usuariosProductos = [];

declararInputs(exampleModal)
calcularTotal();

if (exampleModal) {
    exampleModal.addEventListener('show.bs.modal', event => {
        // Button that triggered the modal
        const button = event.relatedTarget


        // Extract info from data-bs-* attributes
        const producto = button.getAttribute('data-bs-prod')
        const cantidad = button.getAttribute('data-bs-cant')
        const provs = button.getAttribute('data-bs-provs')

        let dist = obtenerDistribuciones(button)

        declararDistribucion(dist)
        usuariosProductos = dist
        // If necessary, you could initiate an Ajax request here
        // and then do the updating in a callback.

        // Update the modal's content.
        const modalTitulo = exampleModal.querySelector('#seleccionar-producto-modal-Label')
        const modalCantidad = exampleModal.querySelector('#seleccionar-cantidad-modal-Label')

        //const modalBodyInput = exampleModal.querySelector('.modal-body input')
        if (producto != "NA") {
            modalTitulo.innerHTML = `Producto seleccionado: <span class="prov-sel text-primary">${producto}</span>`
            modalCantidad.querySelector('span').innerHTML = `${cantidad}`
        } else {
            modalTitulo.innerHTML = `Precio no asignado por <span class="prov-sel">proveedor</span>`
            modalCantidad.querySelector('span').innerHTML = `0`
        }
        //modalBodyInput.value = producto
    })
}

function obtenerDistribuciones(button) {
    // Obtener la fila actual
    const fila = button.closest('tr');

    // Seleccionar todos los elementos span dentro de la fila
    const spans = fila.querySelectorAll('.cantidad-distribuida span');

    // Inicializar un arreglo para almacenar los datos
    const datos = [];

    // Iterar sobre cada span y obtener sus atributos y contenido de texto
    spans.forEach(span => {
        const Id_Usuario = parseInt(span.getAttribute('data-bs-idprov'));
        const Id_UsuarioProducto = parseInt(span.getAttribute('data-bs-idusprod'));
        const Precio = parseFloat(span.getAttribute('data-bs-precio'));
        const Cantidad_comprada = span.textContent.trim();

        // Agregar los datos al arreglo
        datos.push({ Id_UsuarioProducto, Id_Usuario, Cantidad_comprada, Precio});
    });

    // Ahora puedes usar el arreglo `datos` para lo que necesites
    console.log(datos);
    return datos;
}

function declararDistribucion(datos) {

    // Recorrer el arreglo de datos
    datos.forEach(dato => {
        // Construir el ID del input
        const inputId = `input-${dato.Id_Usuario}`;
        const tagId = `tag-precio-${dato.Id_Usuario}`;
        const tagSel = `tag-seleccionado-${dato.Id_Usuario}`;

        // Buscar el input correspondiente en la tabla
        const input = document.getElementById(inputId);
        const tag = document.getElementById(tagId);
        const tagCalculo = document.getElementById(tagSel);

        if (tag) {
            // Si se encuentra el input, habilitarlo y establecer su valor
            tag.innerText = "$ " + dato.Precio;
            tagCalculo.innerText = "$ " + 0;
        }

        if (input) {
            // Si se encuentra el input, habilitarlo y establecer su valor
            input.value = dato.Cantidad_comprada;
            input.disabled = false;
        }

        calcularPrecio(input)
    });

    // Deshabilitar los inputs que no coinciden con ningún proveedor en el arreglo
    document.querySelectorAll('#seleccionar-precio-modal .table tbody input').forEach(input => {
        const Id_Usuario = parseInt(input.id.split('-')[1]);
        const coincide = datos.some(dato => dato.Id_Usuario === Id_Usuario);

        if (!coincide) {
            document.getElementById('tag-precio-' + Id_Usuario).innerText = "$ " + 0;
            document.getElementById('tag-seleccionado-' + Id_Usuario).innerText = "$ " + 0;
            input.value = 0;
            input.disabled = true;
        }
    });
}

function declararInputs(exampleModal) {
    const inputs = exampleModal.querySelectorAll('.table tbody input');

    // Agregar evento input a cada input
    inputs.forEach(input => {
        input.addEventListener('input', () => {
            // Calcular la suma de todos los valores de los inputs
            let suma = 0.0;
            inputs.forEach(input => {
                suma += parseFloat(input.value) || 0.0; // Convertir el valor a flotante o 0 si es NaN
            });

            // Obtener el valor máximo
            const maximo = parseFloat(spanMaximo.textContent);

            // Verificar si la suma supera el valor máximo
            if (suma > maximo) {
                // Restablecer el valor del input actual para que no sobrepase el máximo
                let res = parseFloat(input.value) - (suma - maximo);
                if (res == "4.440892098500626e-16") {
                    input.value = 0
                } else {
                    if (String(res).includes('.') && String(res).split('.')[1].length > 4) {
                        // Redondear el número a 4 decimales después del punto
                        input.value = parseFloat(res).toFixed(4);
                    } else {
                        input.value = res;
                    }

                }

            } else if (input.value < 0) {
                input.value = 0
            }

            calcularPrecio(input)
        });
    });
}

function calcularPrecio(input) {
    // Calcular el porcentaje del precio asociado al input
    const parent = input.closest('td');
    const precioSpan = parent.querySelector('.tag-precio');
    const precio = parseFloat(precioSpan.innerText.split('$')[1]); // Obtener el precio eliminando el signo $
    //const porcentaje = (parseFloat(input.value) / maximo) * 100; // Calcular el porcentaje
    //parent.querySelector('.tag-precio-seleccionado').innerText = `$ ${(precio * porcentaje / 100).toFixed(4)}`;
    parent.querySelector('.tag-precio-seleccionado').innerText = `$ ${(precio * input.value).toFixed(4)}`;
    //console.log(`El precio de la cantidad seleccionada es: $${(precio * porcentaje / 100).toFixed(4)}`); // Mostrar el precio de la cantidad seleccionada
}

function guardarPrecios(e) {
    const inputs = document.querySelectorAll('#seleccionar-precio-modal .table tbody input');

    inputs.forEach(input => {
        const idUsuario = parseInt(input.id.split('-')[1]);
        const indiceCoincidente = usuariosProductos.findIndex(dato => dato.Id_Usuario === idUsuario);

        if (indiceCoincidente !== -1) {
            // Si se encontró un elemento coincidente
            const cantidadComprada = parseFloat(input.value); // Obtener el valor del input como un número flotante
            usuariosProductos[indiceCoincidente].Cantidad_comprada = cantidadComprada; // Asignar el valor al atributo Cantidad_comprada del elemento coincidente
            console.log(`Cantidad comprada para el usuario ${idUsuario}: ${cantidadComprada}`);

        }
    });


    console.log(usuariosProductos)
    
    //Ajax
    e.disabled = true;
    // Enviar el arreglo de objetos al controlador utilizando AJAX
    $.ajax({
        url: '/REQHDRs/distribuirCompras',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({ usuariosProductos }),
        beforeSend: function () {
            // Código a ejecutar antes de enviar la petición AJAX
            e.innerText = "Guardando..."
            // Puedes mostrar un indicador de carga, deshabilitar botones, etc.
        },
        success: function (response) {
            // Manejar la respuesta del servidor si es necesario
            console.log('Datos enviados correctamente');
            console.log(response);

            if (response.Success) {
                inputs.forEach(input => {
                    const idUsuario = parseInt(input.id.split('-')[1]);
                    const indiceCoincidente = usuariosProductos.findIndex(dato => dato.Id_Usuario === idUsuario);

                    if (indiceCoincidente !== -1) {
                        // Si se encontró un elemento coincidente
                        document.getElementById("provedor-distribuido-" + idUsuario).innerText = usuariosProductos[indiceCoincidente].Cantidad_comprada; // Asignar el valor al atributo Cantidad_comprada del elemento coincidente
                    }
                });
                calcularTotal();
            }

            // Esperar 1 segundo (1000 milisegundos) y luego mostrar el texto
            setTimeout(function () {
                e.innerText = "Guardar cambios"
                e.disabled = false;
            }, 2000); // 1000 milisegundos = 1 segundo
        },
        error: function (xhr, status, error) {
            // Manejar errores si ocurrieron durante la solicitud AJAX
            console.error('Error al enviar datos:', error);
        }
    });
}

function calcularTotal() {
    let total = 0.0;

    // Obtener todas las filas de la tabla
    const filas = document.querySelectorAll('.table tbody tr');

    // Iterar sobre cada fila
    filas.forEach(fila => {
        // Obtener el precio y la cantidad distribuida de la fila actual
        if (fila.querySelector('.precio-ultimo') && fila.querySelector('.cantidad-distribuida span')) {
            const precio = parseFloat(fila.querySelector('.precio-ultimo').innerText);
            const cantidadDistribuida = parseFloat(fila.querySelector('.cantidad-distribuida span').innerText);

            // Calcular el producto del precio y la cantidad distribuida y agregarlo al total
            total += precio * cantidadDistribuida;
        }
    });

    document.querySelector('.total-suma').innerText = "$"+ total.toFixed(3); // Redondear el total a 4 decimales
}

