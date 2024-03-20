const local = ""
const public = "http://192.168.1.120:8025/"

const env = public + "api/"

//Regions API
const getEstados = env + "region/estado"
const getMunicipios = env + "region/estado/municipio?estado="
const getColonias = env + "region/estado/municipio/colonias?estado="
const getLocalidades = env + "region/estado/localidades?estado="

//Products API
const getProductSearch = env + "fyv/busqueda?art="