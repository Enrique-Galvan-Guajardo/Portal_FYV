// Manejar clic en el botón de enviar
$("#login-form-submit").click(function () {
    enviarDatos();
});

function enviarDatos() {
    var user = {
        user_session: $("#user-session").val(),
        user_password: $("#user-password").val()
    }

    // Realizar la solicitud AJAX POST al controlador
    $.ajax({
        url: "https://localhost:44364/Usuarios/login", // Ruta de la acción del controlador
        type: "POST",
        data: JSON.stringify(user), // Datos a enviar en el cuerpo de la solicitud
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            // Manejar la respuesta del servidor
            console.log("Respuesta del servidor:", response);
        },
        error: function (xhr, status, error) {
            // Manejar errores de la solicitud
            console.error("Error en la solicitud:", status, error);
        }
    });
}