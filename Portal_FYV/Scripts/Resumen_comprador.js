const exampleModal = document.getElementById('seleccionar-precio-modal')
const idsREQHDRS_Resumen = document.querySelector("table.table").getAttribute('data-ids').split('-');
const idsUsuariosProductos_Resumen = document.querySelector("table.table").getAttribute('data-idsusps').split('-');
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
        const idrhdr = button.getAttribute('data-bs-idrhdr')
        const cantidad = button.getAttribute('data-bs-cant')
        const provs = button.getAttribute('data-bs-provs')
        console.log(idrhdr)
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
            modalCantidad.querySelector('input.hidden-rqhdr').value = idrhdr
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
        const Id_REQHDR = parseInt(span.getAttribute('data-bs-idrhdr'));
        const Id_UsuarioProducto = parseInt(span.getAttribute('data-bs-idusprod'));
        const Precio = parseFloat(span.getAttribute('data-bs-precio'));
        const Cantidad_comprada = span.textContent.trim();

        // Agregar los datos al arreglo
        datos.push({ Id_UsuarioProducto, Id_REQHDR, Id_Usuario, Cantidad_comprada, Precio});
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
        data: JSON.stringify({ usuariosProductos, idsREQHDRS_Resumen, idsUsuariosProductos_Resumen }),
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
                        document.querySelector(".provedor-distribuido-" + idUsuario + "-" + document.querySelector('.hidden-rqhdr').value).innerText = usuariosProductos[indiceCoincidente].Cantidad_comprada; // Asignar el valor al atributo Cantidad_comprada del elemento coincidente
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
    let totalPorColumna = {};

    // Obtener todas las filas de la tabla
    const filas = document.querySelectorAll('.table tbody tr');

    // Iterar sobre cada fila
    filas.forEach(fila => {
        // Obtener todas las celdas con la clase "seleccionar-precio" de la fila actual
        const celdas = fila.querySelectorAll('.seleccionar-precio');

        // Iterar sobre cada celda
        celdas.forEach((celda, index) => {
            // Verificar si la celda tiene un span con clase "text-danger"
            const esCeldaValida = !celda.querySelector('.text-danger');

            if (esCeldaValida) {
                // Obtener el precio y la cantidad distribuida de la celda actual
                const precio = parseFloat(celda.querySelector('.precio-ultimo').innerText);
                const cantidadDistribuida = parseFloat(celda.querySelector('.cantidad-distribuida span').innerText);

                // Calcular el producto del precio y la cantidad distribuida y agregarlo al total de la columna correspondiente
                totalPorColumna[index] = (totalPorColumna[index] || 0) + (precio * cantidadDistribuida);
            }
        });
    });

    document.querySelector('.total-suma').innerText = "$" + Object.values(totalPorColumna).reduce((acc, val) => acc + val, 0); // Redondear el total a 4 decimales
}


//Hacer que el botón esté deshabilitado si la suma de las cantidades no dan el total solicitado
//Si no dan la cantidad, que se habilite con un checkbox en una pestaña adicional arriba de la tabla para confirmar cantidades a distribuir entre sucursales

function setData(button) {
    // Obtener la fila que contiene el botón
    var row = button.closest('tr');
    let producto = row.querySelector('div > div:first-child').innerText.trim()
    document.getElementById('cantidad-total-solicitar').innerText = parseFloat(button.dataset.cantidad).toFixed(2);
    document.getElementById('cantidad-total-distribuir').innerText = 0;
    document.getElementById('producto-solicitar').innerText = producto;

    //Mostrar imágenes de productos acorde al proveedor
    document.querySelectorAll('#table-tab-pane tbody td img').forEach(function (img) {
        if (img.dataset.imgname.includes(producto)) {
            img.classList.remove('d-none')
        } else {
            img.classList.add('d-none')
        }
    })
    //Mostrar headers de sucursales acorde al producto
    document.querySelectorAll('#distribucion-tab-pane th').forEach(function (th) {
        if (th.dataset.productos.includes(producto)) {
            th.classList.remove('d-none')
        } else {
            th.classList.add('d-none')
        }
    })
    //Mostrar campos de sucursales acorde al producto
    document.querySelectorAll('#distribucion-tab-pane td').forEach(function (td) {
        td.querySelector('input').value = 0;
        if (td.dataset.productos.includes(producto)) {
            td.classList.remove('d-none')
        } else {
            td.classList.add('d-none')
        }
    })

    // Seleccionar todos los elementos span con clase 'stocks' dentro de esa fila
    var stocksElements = row.querySelectorAll('span.stocks');

    // Crear un array para almacenar los objetos con los datos
    var dataObjects = [];

    document.querySelectorAll('.tag-disp').forEach(function (tag) {
        tag.innerText = 0;
    })
    // Recorrer cada elemento y obtener sus datasets
    stocksElements.forEach(function (stocksElement) {
        var dataObject = {
            producto,
            precio: parseFloat(stocksElement.dataset.precio),
            provedor: stocksElement.dataset.provedor,
            solicitado: parseFloat(stocksElement.parentElement.parentElement.querySelector(".solicitado").innerText),
            idUp: stocksElement.dataset.idUp.split(',').map(Number),
            idProveedor: parseInt(stocksElement.dataset.idProveedor), // Nuevo dataset agregado
            stock: parseFloat(stocksElement.dataset.stock) // Nuevo dataset agregado
        };

        document.getElementById('tag-disp-' + dataObject.idProveedor).innerText = dataObject.stock;
        document.getElementById('input-' + dataObject.idProveedor).innerText = dataObject.stock;
        // Añadir el objeto al array
        dataObjects.push(dataObject);
    });

    //Asignar solicitado automáticamente
    if (dataObjects.length > 0) {
        dataObjects.forEach(function (data) {
            document.querySelectorAll('#table-tab-pane .input-cantidad').forEach(function (input) {
                //Limpiar todos los campos a 0
                input.value = 0
            })
            document.querySelectorAll('#table-tab-pane .input-cantidad').forEach(function (input) {
                //Asignar campos que ya están reseteados
                if (input.parentElement.querySelector('.prov_name').value.trim() == data.provedor) {
                    input.value = data.solicitado
                }
                validarInput(input)
            })
        })
    }
    else {
        document.querySelectorAll('#table-tab-pane .input-cantidad').forEach(function (input) {
            input.value = 0
            validarInput(input)
        })
    }

    console.log(dataObjects)
}

function validarInput(input) {
    // Obtener el total permitido
    var totalSolicitar = parseFloat(document.getElementById('cantidad-total-solicitar').textContent);
    
    // Obtener todos los inputs de cantidad
    var inputs = document.querySelectorAll('.input-cantidad');

    // Obtener la cantidad disponible para el input actual
    var cantidadDisponible = parseFloat(input.closest('td').querySelector('.tag-disp').textContent);

    var sumaTotal = 0;
    var valorActual = parseFloat(input.value) || 0;

    // Verificar si el valor del input actual excede la cantidad disponible
    if (valorActual > cantidadDisponible) {
        toastFill({ Message_Classes: "warning", Message: "La cantidad solicitada excede la cantidad disponible de " + cantidadDisponible})
        input.value = cantidadDisponible;
        valorActual = cantidadDisponible;
    }

    // Calcular la suma de todos los inputs
    inputs.forEach(function (input) {
        var valor = parseFloat(input.value) || 0;
        sumaTotal += valor;
        document.getElementById('cantidad-total-distribuir').innerText = parseFloat(sumaTotal).toFixed(2)
    });

    // Verificar si la suma total excede el límite permitido
    if (sumaTotal > totalSolicitar) {
        toastFill({ Message_Classes: "warning", Message: "La suma total de las cantidades solicitadas excede el límite permitido de " + totalSolicitar })
        document.getElementById('cantidad-total-distribuir').innerText = parseFloat(document.getElementById('cantidad-total-distribuir').innerText) - parseFloat(input.value).toFixed(2)
        input.value = 0;
    }


    // Habilitar o deshabilitar el botón guardar-solicitudes basado en la suma total
    var botonGuardar = document.getElementById('guardar-solicitudes');
    if (sumaTotal === totalSolicitar) {
        botonGuardar.disabled = false;
        document.getElementById('cantidad-total-distribuir').className = "text-success"
        //document.getElementById('distribucion-tab').className = "nav-link d-none"
        let distribucionCols = document.getElementById('distribucion-tab-pane').querySelectorAll('td:not(.d-none)')
        distribucionCols.forEach(function (td) {
            td.querySelector('input').disabled = true
        });
    } else {
        botonGuardar.disabled = true;
        document.getElementById('cantidad-total-distribuir').className = "text-body-tertiary"
        let distribucionCols = document.getElementById('distribucion-tab-pane').querySelectorAll('td:not(.d-none)')
        //document.getElementById('distribucion-tab').className = "nav-link"
        distribucionCols.forEach(function (td) {
            td.querySelector('input').disabled = false
        });
    }

    var dists = document.querySelectorAll('.input-dist');
    dists.forEach(function (input) {
        input.value = 0;
    });

    let cantidades_validadas = []
    // Obtener el total permitido a distribuir desde el elemento con id 'cantidad-total-distribuir'
    var totalDistribuir = parseFloat(document.getElementById('cantidad-total-distribuir').textContent).toFixed(2);

    if (totalSolicitar == totalDistribuir) {
        document.querySelectorAll('.cantidades-validadas-sucursal').forEach(function (input) {
            if (input.dataset.descripcion == document.getElementById('producto-solicitar').innerText) {
                Cantidad_validada = parseFloat(input.value)
                cantidades_validadas.push(Cantidad_validada)
            }
        })

        let distribucionCols = document.getElementById('distribucion-tab-pane').querySelectorAll('td:not(.d-none)')
        distribucionCols.forEach(function (td, i) {
            td.querySelector('input').value = parseFloat(cantidades_validadas[i])
        });
    }

}
function validarDist(input) {
    
    // Obtener todos los inputs de cantidad
    var dists = document.querySelectorAll('.input-dist');
    var cantidades = document.querySelectorAll('.input-cantidad');

    // Calcular la suma de todos los inputs
    var sumaTotal = 0;
    var sumaDisponible = 0;
    cantidades.forEach(function (input) {
        var valor = parseFloat(input.value) || 0;
        sumaTotal += valor;
    });
    dists.forEach(function (input) {
        var valor = parseFloat(input.value) || 0;
        sumaDisponible += valor;
    });

    //console.log(totalDistribuir)
    //console.log(sumaTotal - sumaDisponible)
    let residuo = parseFloat((sumaTotal - sumaDisponible).toFixed(2))

    if (residuo >= 0) {
        document.getElementById('cantidad-total-distribuir').textContent = residuo;
    } else {
        //El restante debe sumarse, no retornar a 0 y volver a empezar
        if (residuo < 0) {
            input.value = 0;
            document.getElementById('cantidad-total-distribuir').textContent = sumaTotal
        } else {
            input.value += residuo
            document.getElementById('cantidad-total-distribuir').textContent = residuo
        }
    }


    var botonGuardar = document.getElementById('guardar-solicitudes');
    if (residuo == 0) {
        botonGuardar.disabled = false;
        document.getElementById('cantidad-total-distribuir').className = "text-success"
    } else {
        botonGuardar.disabled = true;
        document.getElementById('cantidad-total-distribuir').className = "text-body-tertiary"
    }
    /*
    */
}

function habilitarGuardado(checkbox) {
    var botonGuardar = document.getElementById('guardar-solicitudes');
    if (checkbox.checked) {
        botonGuardar.disabled = false;
        document.getElementById('cantidad-total-distribuir').className = "text-success"
    } else {
        botonGuardar.disabled = true;
        document.getElementById('cantidad-total-distribuir').className = "text-body-tertiary"
    }
}

function guardarOrdenCompra(button) {
    let tabPaneDistribucion = [];
    let tabPaneSolicitud = [];
    let odc = [];
    

    let solicitudCols = document.getElementById('table-tab-pane').querySelectorAll('td');
    let distribucionCols = document.getElementById('distribucion-tab-pane').querySelectorAll('td:not(.d-none)');

    solicitudCols.forEach(function (td) {
        let Cantidad_solicitada = parseFloat(td.querySelector('input.form-control').value) || 0;
        let Id_Proveedor = parseInt(td.querySelector('input.form-control').id.split('-')[1]);
        let Id_REQHDR = parseInt(td.querySelector('input.ids_up_sucursal').value);
        let Proveedor = td.querySelector('input.prov_name').value;
        let Cantidad_disponible = parseFloat(td.querySelector('span.tag-disp').innerText);

        let SolicitudObjeto = {
            Id_REQHDR,
            Id_Proveedor,
            Proveedor,
            Cantidad_disponible,
            Cantidad_solicitada,
            Producto: document.getElementById('producto-solicitar').innerText
        };
        tabPaneSolicitud.push(SolicitudObjeto);
    });

    const sucursalMap = {
        "JUA": 'Juarez',
        "GUA": 'Guanza',
        "OFE": 'Ofertas',
        "BAL": 'Balcones',
        "GTO": 'Guanajuato',
        "CDI": 'Cedis',
        "JAR": 'Jarachina',
        "AMG": 'Almaguer'
    };

    distribucionCols.forEach(function (td) {
        let Sucursal = sucursalMap[td.className] || 'NA';
        let Id_REQHDR = parseInt(td.querySelector('div').dataset.idrhdr);
        let Cantidad_distribuida = parseFloat(td.querySelector('input').value) || 0;
        let Cantidad_validada = "";
        document.querySelectorAll('.cantidades-validadas-sucursal').forEach(function (input) {
            if (input.dataset.descripcion == document.getElementById('producto-solicitar').innerText &&
                input.dataset.idrhdr == Id_REQHDR) {
                Cantidad_validada = parseFloat(input.value);
            }
        });

        let DistribucionObjeto = {
            Sucursal,
            Id_REQHDR,
            Cantidad_distribuida,
            Cantidad_validada,
            Producto: document.getElementById('producto-solicitar').innerText
        };
        tabPaneDistribucion.push(DistribucionObjeto);
    });

    tabPaneDistribucion.sort((a, b) => b.Cantidad_validada - a.Cantidad_validada);
    tabPaneSolicitud.sort((a, b) => b.Cantidad_solicitada - a.Cantidad_solicitada);

    console.table(tabPaneSolicitud);
    const tabPaneSolicitud_temp = JSON.parse(JSON.stringify(tabPaneSolicitud));
    console.table(tabPaneDistribucion);
    console.table(JSON.stringify(tabPaneSolicitud));
    console.table(JSON.stringify(tabPaneDistribucion));

    //array.forEach.forEach(function (input) { });
    //odc = generarOrdenesDeCompra(tabPaneSolicitud, tabPaneDistribucion);
    odc = generarODC(tabPaneSolicitud, tabPaneDistribucion);
    console.log(JSON.stringify(odc));
    console.log(JSON.stringify(odc));
    console.log(odc);

    if (odc.length > 0) {

        //Ajax
        button.disabled = true;
        // Enviar el arreglo de objetos al controlador utilizando AJAX
        $.ajax({
            url: '/REQHDRs/guardarDistribucion',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ odc, usuariosProductos: tabPaneSolicitud_temp }),
            beforeSend: function () {
                // Código a ejecutar antes de enviar la petición AJAX
                button.innerText = "Guardando..."
                // Puedes mostrar un indicador de carga, deshabilitar botones, etc.
            },
            success: function (response) {
                // Manejar la respuesta del servidor si es necesario
                console.log('Datos enviados correctamente');
                console.log(response);
                if (response.Success) {
                    let product_name = document.querySelector('#producto-solicitar').innerText
                    document.querySelectorAll('#table-work tbody tr').forEach(function (tr) {
                        let celd_name = tr.children[0].children[0].children[0]
                        celd_name.innerText == product_name ? (tr.children[0].className = "bg-opacity-50 bg-success fw-bold") : console.log("Rechazado")
                    });
                }
                toastFill(response)
                setSolicitado(tabPaneSolicitud_temp)
                calcularTotal()
                // Esperar 1 segundo (1000 milisegundos) y luego mostrar el texto
                setTimeout(function () {
                    button.innerText = "Guardar"
                    button.disabled = false;
                }, 2000); // 1000 milisegundos = 1 segundo
            },
            error: function (xhr, status, error) {
                // Manejar errores si ocurrieron durante la solicitud AJAX
                console.error('Error al enviar datos:', error);
            }
        });
    }
    /**
     * 
    let modal = button.closest('.modal-content')
    let product = modal.querySelector('.modal-body #producto-solicitar').innerText

    let modal_rows = document.getElementById('table-tab-pane').querySelectorAll('table tbody > tr')
    let distribution_rows = document.getElementById('distribucion-tab-pane').querySelectorAll('table tbody > tr')
    let sucs = distribution_rows.closest('.table').querySelectorAll('thead th')
    let table_rows = document.querySelectorAll('.productos-consolidacion table tbody > tr')
    let cantidades_validadas_sucursal = table_rows.querySelectorAll('.cantidades-validadas-sucursal')


    let ordenCompra = {
        REQHDRS: "", //REQHDRS que hicieron la solicitud (ejemplo "2005-2006" concatenados como texto)
        Proveedor: "", //Proveedor (o proveedores) seleccionado para surtir la solicitud <nombre>
        Producto: "", //Producto de la solicitud <nombre>
        //Cantidad_solicitada: 0, //Cantidad total solicitada (originalmente) <sumatoria>
        Precio: 0, //Precio de compra final por todos los productos comprados <multiplicación>
        Cantidad_validada: 0, //Cantidad total validada de productos <sumatoria>
        Fecha_creacion: Date.now(), //Fecha actual <datetime>
        Juarez: 0, //Cuánto se distribuirá para Juarez
        Villas: 0,//Cuánto se distribuirá para Villas
        Almaguer: 0,//Cuánto se distribuirá para Almaguer
        Jarachina: 0,//...
        Balcones: 0,
        Cedis: 0,
        Guanza: 0,
        Ofertas: 0,
        Guanajuato: 0
    }
     */
    /* 
    modal_rows.querySelectorAll('.input-cantidad').forEach(function (input) {
        var valor = parseFloat(input.value) || 0;
        if (valor > 0) {
            let drows = {}
            table_rows.forEach(function (row) {
                if (row.querySelector('td div > div:first-child').innerText == product) {
                    distribution_rows.forEach(function (drow) {
                        let Sucursal_name = ""
                        switch (drow.className) {
                            case "JUA":
                                Sucursal_name = 'Juarez'
                                break;
                            case "GUA":
                                Sucursal_name = 'Guanza'
                                break;
                            case "OFE":
                                Sucursal_name = 'Ofertas'
                                break;
                            case "BAL":
                                Sucursal_name = 'Balcones'
                                break;
                            case "GTO":
                                Sucursal_name = 'Guanajuato'
                                break;
                            case "CDI":
                                Sucursal_name = 'Cedis'
                                break;
                            case "JAR":
                                Sucursal_name = 'Jarachina'
                                break;
                            case "AMG":
                                Sucursal_name = 'Almaguer'
                                break;
                            default:
                                Sucursal_name = 'NA'
                                break;
                        }
                        drows.push({
                            Sucursal: Sucursal_name,
                            Id_REQHDR: drow.children[0].dataset.idrhdr,
                            Cantidad_distribuir: drow.querySelector('input').value > 0 ? drow.querySelector('input').value : 0,
                            Cantidad_validada: row.getElementById('validado-' + product + '-' + drow.children[0].dataset.idrhdr).value
                        })
                    })


                }
            })
            drows.forEach(function (d) {
                proveedores.push({
                    Producto: product,
                    REQHDRS: drows.Id_REQHDR.join('-'),
                    Proveedor: input.closest('div').querySelector('.prov_name').value,
                    [d.Sucursal]: d.Cantidad_validada
                })
            })
        }
    });      

    let ordenesCompra = {} //Guardar uno por uno los objetos generados con el formatio anterior para enviarlos como lista al controlador
    */
}
function cancelarODC(button, Id_OrdenCompra) {
    if (Id_OrdenCompra > 0) {

        //Ajax
        button.disabled = true;
        // Enviar el arreglo de objetos al controlador utilizando AJAX
        $.ajax({
            url: '/REQHDRs/cancelarDistribucion',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ Id_OrdenCompra }),
            beforeSend: function () {
                // Código a ejecutar antes de enviar la petición AJAX
                button.innerText = "Cancelando..."
                // Puedes mostrar un indicador de carga, deshabilitar botones, etc.
            },
            success: function (response) {
                // Manejar la respuesta del servidor si es necesario
                console.log('Datos enviados correctamente');
                console.log(response);
                if (response.Success) {
                    button.closest('tr').children[0].className = "bg-opacity-50 ";
                    button.closest('tr').querySelectorAll('.solicitado').forEach(function (celd) {
                        celd.innerText = "0.00";
                    });
                }
                toastFill(response)
                calcularTotal()
                // Esperar 1 segundo (1000 milisegundos) y luego mostrar el texto
                setTimeout(function () {
                    button.innerText = "Capturar"
                    button.className = "btn btn-sm btn-primary my-2"
                    button.disabled = false;
                    button.setAttribute('data-bs-toggle', 'modal');
                    button.setAttribute('data-bs-target', '#cantidadesModal');
                    button.setAttribute('onclick', 'setData(this)');
                }, 2000); // 1000 milisegundos = 1 segundo
            },
            error: function (xhr, status, error) {
                // Manejar errores si ocurrieron durante la solicitud AJAX
                console.error('Error al enviar datos:', error);
                setTimeout(function () {
                    button.innerText = "Cancelar"
                    button.disabled = false;
                }, 2000); // 1000 milisegundos = 1 segundo
            }
        });
    }
}
function generarOrdenesDeCompra(tabPaneSolicitud, tabPaneDistribucion) {
    let proveedores = [];
    const fechaActual = new Date().toISOString().replace('T', ' ').substring(0, 23);

    // Filtrar proveedores con Cantidad_solicitada mayor a 0
    let proveedoresValidos = tabPaneSolicitud.filter(solicitud => solicitud.Cantidad_solicitada > 0);

    // Ordenar proveedores por Cantidad_disponible en orden descendente
    proveedoresValidos.sort((a, b) => b.Cantidad_disponible - a.Cantidad_disponible);

    let totalSolicitado = tabPaneDistribucion.reduce((suma, distribucion) => suma + distribucion.Cantidad_validada, 0);
    let totalDistribuido = 0;

    for (let i = 0; i < proveedoresValidos.length; i++) {
        let proveedor = proveedoresValidos[i];
        let distribucionesMismoProducto = tabPaneDistribucion.filter(distribucion => distribucion.Producto === proveedor.Producto);

        let cantidadRestante = proveedor.Cantidad_solicitada;

        let ordenCompra = {
            REQHDRS: distribucionesMismoProducto.map(d => d.Id_REQHDR).join('-'),
            Proveedor: proveedor.Proveedor,
            Producto: proveedor.Producto,
            Precio: 0,
            Cantidad_validada: 0,
            Fecha_creacion: fechaActual,
            Juarez: 0,
            Villas: 0,
            Almaguer: 0,
            Jarachina: 0,
            Balcones: 0,
            Cedis: 0,
            Guanza: 0,
            Ofertas: 0,
            Guanajuato: 0
        };

        for (let j = 0; j < distribucionesMismoProducto.length; j++) {
            let distribucion = distribucionesMismoProducto[j];
            let cantidadADistribuir = Math.min(cantidadRestante, distribucion.Cantidad_validada);

            if (cantidadADistribuir > 0) {
                cantidadRestante -= cantidadADistribuir;
                distribucion.Cantidad_validada -= cantidadADistribuir;
                totalDistribuido += cantidadADistribuir;

                ordenCompra.Cantidad_validada += cantidadADistribuir;

                switch (distribucion.Sucursal) {
                    case 'Juarez':
                        ordenCompra.Juarez += cantidadADistribuir;
                        break;
                    case 'Villas':
                        ordenCompra.Villas += cantidadADistribuir;
                        break;
                    case 'Almaguer':
                        ordenCompra.Almaguer += cantidadADistribuir;
                        break;
                    case 'Jarachina':
                        ordenCompra.Jarachina += cantidadADistribuir;
                        break;
                    case 'Balcones':
                        ordenCompra.Balcones += cantidadADistribuir;
                        break;
                    case 'Cedis':
                        ordenCompra.Cedis += cantidadADistribuir;
                        break;
                    case 'Guanza':
                        ordenCompra.Guanza += cantidadADistribuir;
                        break;
                    case 'Ofertas':
                        ordenCompra.Ofertas += cantidadADistribuir;
                        break;
                    case 'Guanajuato':
                        ordenCompra.Guanajuato += cantidadADistribuir;
                        break;
                    default:
                        console.error(`Sucursal desconocida: ${distribucion.Sucursal}`);
                }
            }
        }

        if (ordenCompra.Cantidad_validada > 0) {
            proveedores.push(ordenCompra);
        }
    }

    return proveedores;
}
function generarODC(tabPaneSolicitud, tabPaneDistribucion) {
    let odc = [];

    const actualizarODC = (odc_recuperado, row2, cantidad) => {
        odc_recuperado.REQHDRS += odc_recuperado.REQHDRS ? `-${row2.Id_REQHDR}` : String(row2.Id_REQHDR);
        odc_recuperado.Fecha_creacion = new Date().toISOString();
        odc_recuperado.Cantidad_validada += cantidad;

        switch (row2.Sucursal) {
            case "Juarez": odc_recuperado.Juarez += cantidad; break;
            case "Guanza": odc_recuperado.Guanza += cantidad; break;
            case "Ofertas": odc_recuperado.Ofertas += cantidad; break;
            case "Balcones": odc_recuperado.Balcones += cantidad; break;
            case "Guanajuato": odc_recuperado.Guanajuato += cantidad; break;
            case "Cedis": odc_recuperado.Cedis += cantidad; break;
            case "Jarachina": odc_recuperado.Jarachina += cantidad; break;
            case "Almaguer": odc_recuperado.Almaguer += cantidad; break;
            case "Villas": odc_recuperado.Villas += cantidad; break;
            default: console.log("Sucursal no coincidente."); break;
        }
    };

    tabPaneSolicitud.forEach(function (row) {
        if (row.Cantidad_solicitada > 0) {
            let REQHDRS = "";
            let Proveedor = row.Proveedor;
            let Producto = row.Producto;
            let Cantidad_validada = 0;
            let Juarez = 0, Villas = 0, Almaguer = 0, Jarachina = 0, Balcones = 0, Cedis = 0, Guanza = 0, Ofertas = 0, Guanajuato = 0;

            tabPaneDistribucion.forEach(function (row2) {
                if (row2.Cantidad_distribuida != 0) {
                    let cantidadParaAsignar = Math.min(row2.Cantidad_distribuida, row.Cantidad_solicitada);
                    let odc_recuperado = odc.find(item => item.Proveedor === row.Proveedor && item.Producto === row.Producto);

                    if (odc_recuperado) {
                        actualizarODC(odc_recuperado, row2, cantidadParaAsignar);
                    } else {
                        REQHDRS = String(row2.Id_REQHDR);
                        let Fecha_creacion = new Date().toISOString();
                        Cantidad_validada = cantidadParaAsignar;
                        switch (row2.Sucursal) {
                            case "Juarez": Juarez = cantidadParaAsignar; break;
                            case "Guanza": Guanza = cantidadParaAsignar; break;
                            case "Ofertas": Ofertas = cantidadParaAsignar; break;
                            case "Balcones": Balcones = cantidadParaAsignar; break;
                            case "Guanajuato": Guanajuato = cantidadParaAsignar; break;
                            case "Cedis": Cedis = cantidadParaAsignar; break;
                            case "Jarachina": Jarachina = cantidadParaAsignar; break;
                            case "Almaguer": Almaguer = cantidadParaAsignar; break;
                            default: console.log("Sucursal no coincidente."); break;
                        }

                        let ordenCompra = {
                            REQHDRS,
                            Proveedor,
                            Producto,
                            Precio: 0,
                            Cantidad_validada,
                            Fecha_creacion,
                            Juarez,
                            Villas,
                            Almaguer,
                            Jarachina,
                            Balcones,
                            Cedis,
                            Guanza,
                            Ofertas,
                            Guanajuato
                        };
                        odc.push(ordenCompra);
                    }

                    row.Cantidad_solicitada -= cantidadParaAsignar;
                    row2.Cantidad_distribuida -= cantidadParaAsignar;
                }
            });
        }
    });

    console.table(odc);
    return odc
}

function setSolicitado(tabPaneSolicitud) {
    tabPaneSolicitud.forEach(function (row) {
        document.querySelectorAll('.stocks').forEach(function (celda) {
            if (celda.dataset.idProveedor == row.Id_Proveedor && celda.dataset.producto == row.Producto) {
                celda.closest('td').querySelector('.solicitado').innerText = row.Cantidad_solicitada
            }
        })
    });
}

/*
    array.forEach(function (row) {
    });
    */
/* 
// Ejemplo de uso
const tabPaneSolicitud = [
    { "Id_REQHDR": 2025, "Id_Proveedor": 42, "Proveedor": "Proveedor de salchichas", "Cantidad_disponible": 13, "Cantidad_solicitada": 13, "Producto": "MANGO ORO" },
    { "Id_REQHDR": 2026, "Id_Proveedor": 1037, "Proveedor": "papas", "Cantidad_disponible": 12, "Cantidad_solicitada": 0, "Producto": "MANGO ORO" }
];

const tabPaneDistribucion = [
    { "Sucursal": "Juarez", "Id_REQHDR": 2025, "Cantidad_distribuida": 8, "Cantidad_validada": 8, "Producto": "MANGO ORO" },
    { "Sucursal": "Jarachina", "Id_REQHDR": 2026, "Cantidad_distribuida": 5, "Cantidad_validada": 5, "Producto": "MANGO ORO" }
];

const proveedores = generarOrdenCompra(tabPaneSolicitud, tabPaneDistribucion);
*/
