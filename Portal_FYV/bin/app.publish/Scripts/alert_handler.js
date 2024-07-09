// Selecciona todos los elementos con la clase "auto-close-alert"
let autoCloseAlerts = document.querySelectorAll('.alert');
// Cierra automáticamente todas las alertas después de 5 segundos
autoCloseAlerts.forEach(alertElement => {
    setTimeout(() => closeAlert(alertElement), 5000); // 5000 milisegundos (5 segundos)
});

// Función para cerrar una alerta
function closeAlert(alertElement) {
    $(alertElement).fadeOut();  // Remueve la clase 'show' para ocultar la alerta
}