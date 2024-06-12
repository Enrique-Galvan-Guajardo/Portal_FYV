function sumarPrecios() {
    let precios = document.querySelectorAll('.precio-ultimo')
    let suma = 0;
    console.log(precios)
    for (var i = 0; i < precios.length; i++) {
        console.log(precios[i].innerHTML)
        suma += parseFloat(precios[i].innerHTML)
    }
    console.log(suma)
    document.querySelector('.total-suma') ? (document.querySelector('.total-suma').innerText = "$" + suma) : ""
}

if (document.querySelectorAll('.precio-ultimo')) {
    sumarPrecios()
}

function guardarPrecio(Id_REQDET, producto, Id_Usuario, e) {
    let tag = document.getElementById('label-' + Id_REQDET);
    let input = document.getElementById('precio-' + Id_REQDET);
    let precio = {
        "Id_Producto": 0,
        "Id_Usuario": Id_Usuario,
        "Precio": parseFloat(input.value)
    };

    let ids_REQHDRS = e.closest('tr').id.includes('-') ? e.closest('tr').id.split('-').map(Number) : parseInt(e.closest('tr').id);

    console.log(precio)
    console.log(producto)

    e.disabled = true;
    // Enviar el arreglo de objetos al controlador utilizando AJAX
    $.ajax({
        url: '/REQHDRs/guardarPrecio',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({ precio, producto, ids_REQHDRS }),
        success: function (response) {
            // Manejar la respuesta del servidor si es necesario
            console.log('Datos enviados correctamente');
            console.log(response);
            toastFill(response)
            tag.querySelector('span').innerText = precio.Precio;

            sumarPrecios()
            // Esperar 1 segundo (1000 milisegundos) y luego mostrar el texto
            setTimeout(function () {
                e.disabled = false;

            }, 2000); // 1000 milisegundos = 1 segundo
        },
        error: function (xhr, status, error) {
            // Manejar errores si ocurrieron durante la solicitud AJAX
            console.error('Error al enviar datos:', error);
        }
    });
    /*
    */
}



function actualizarStock(Id_Producto, e) {
    let stockValue = document.getElementById('stock-' + Id_Producto).value;

    let producto = {
        "Id_Producto": Id_Producto,
        "Stock": parseFloat(stockValue)
    };

    if (stockValue > 0 && Id_Producto > 0) {
        e.disabled = true;
        // Enviar el arreglo de objetos al controlador utilizando AJAX
        console.log(producto)
        $.ajax({
            url: '/Productos/actualizarStock',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ producto }),
            success: function (response) {
                // Manejar la respuesta del servidor si es necesario
                if (response.Success) {
                    toastFill(response)
                    setTimeout(function () {
                        e.disabled = false;

                    }, 2000);
                }
            },
            error: function (xhr, status, error) {
                // Manejar errores si ocurrieron durante la solicitud AJAX
                console.error('Error al enviar datos:', error);
            }
        });
        /* 
        */
    }
}