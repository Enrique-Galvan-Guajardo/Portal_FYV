const local = ""
const public = "http://192.168.1.120:8025/"
const api = "http://200.188.143.250:1313"

const env = public + "api/"

//Regions API
const getEstados = env + "region/estado"
const getMunicipios = env + "region/estado/municipio?estado="
const getColonias = env + "region/estado/municipio/colonias?estado="
const getLocalidades = env + "region/estado/localidades?estado="

//Products API
const getProductSearch = env + "fyv/busqueda?art="
const getAllProducts = api + "/api/fyv/busquedaAll?clave_sucursal="
const getAllProductsHTTPS = "/REQDETs/getProductListAPI"
const getAllOrderSearchListHTTPS = "/REQHDRs/getOrderSearchListAPI"
const SendProductDataAPIHTTPS = "/REQDETs/SendProductDataAPI"

//Home
const RestoreSession = "/Home/RenovarSesion"
const getOrdersDocuments = api + "/fyve/proveedor/getordenes?id_proveedor={idsVendor}&_reqhdrs={idsReqhdr}"