
function searchOrders(idVendor, summary) {
    let spans = summary.closest('details').querySelectorAll("mark span.text-primary")
    let btnsList = summary.closest('details').querySelectorAll("div.downloadBtns")
    let idsReqhdrs = []
    for (var i = 0; i < spans.length; i++) {
        idsReqhdrs.push(spans[i].innerHTML)
    }
    /*
    console.log(idsReqhdrs.join("-"))
    */
    $.ajax({
        url: getAllOrderSearchListHTTPS,
        type: "GET",
        data: { urlAPI: getOrdersDocuments.replace("{idsVendor}", idVendor).replace("{idsReqhdr}", idsReqhdrs.join("-")) },
        success: function (response) {
            let data = JSON.parse(response);

            if (data.length > 0) {
                // Limpiar todos los div.downloadBtns antes de insertar nuevos botones
                summary.closest('details').querySelectorAll("div.downloadBtns").forEach(div => div.innerHTML = "");

                data.forEach(obj => {
                    let solicitudSpan = Array.from(spans).find(span => span.innerHTML == obj.id_reqhdr);

                    if (solicitudSpan) {
                        let downloadBtnsDiv = solicitudSpan.closest(".parentDiv").querySelector(".downloadBtns");

                        if (downloadBtnsDiv) {
                            let existingBtn = downloadBtnsDiv.querySelector(`button[data-archivo="${obj.archivo}"]`);

                            if (!existingBtn) {
                                let btn = document.createElement("button");
                                btn.className = "btn col-5 btn-sm btn-outline-primary order";
                                btn.id = `order-${obj.id_reqhdr}`;
                                btn.dataset.archivo = obj.archivo;
                                btn.dataset.idmerksys = obj.id_merksys;
                                btn.dataset.idreqhdr = obj.id_reqhdr;
                                btn.innerHTML = `Archivo ${obj.id_reqhdr}-${obj.id_merksys} <i class="mr-3 bi bi-cloud-arrow-down"></i>`;
                                btn.onclick = function () {
                                    downloadSearchedOrder(this);
                                };

                                // Agregar el botón dentro del div correspondiente
                                downloadBtnsDiv.appendChild(btn);
                            }
                        }
                    }
                });
            }
        },
        error: function (xhr, status, error) {
            console.log(error);  // Rechazar la promesa en caso de error
        }
    });
}
// Función para convertir base64 en un Blob
function convertBase64ToBlob(base64, contentType = '') {
    const byteCharacters = atob(base64);
    const byteArrays = [];

    for (let offset = 0; offset < byteCharacters.length; offset += 512) {
        const slice = byteCharacters.slice(offset, offset + 512);
        const byteNumbers = new Array(slice.length);
        for (let i = 0; i < slice.length; i++) {
            byteNumbers[i] = slice.charCodeAt(i);
        }
        const byteArray = new Uint8Array(byteNumbers);
        byteArrays.push(byteArray);
    }

    return new Blob(byteArrays, { type: contentType });
}
function downloadSearchedOrder(btn) {
    let archivo = btn.dataset.archivo
    let id_merksys = btn.dataset.idmerksys
    let id_reqhdr = btn.dataset.idreqhdr
    let pdfBlob = convertBase64ToBlob(archivo, 'application/pdf');
    let pdfUrl = URL.createObjectURL(pdfBlob);

    // Crear un enlace para descargar el PDF
    let a = document.createElement('a');
    a.href = pdfUrl;
    a.download = `Archivo ${id_reqhdr}-${id_merksys}.pdf`;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);

    // Revocar la URL temporal después de descargar
    URL.revokeObjectURL(pdfUrl);
}