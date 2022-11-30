using AgapeaDAM.Models;
using AgapeaDAM.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace AgapeaDAM.Controllers
{
    public class PedidoController : Controller
    {
        #region ... propiedades de la clase ...

        private IBDAccess servicioBD;

        public PedidoController(IBDAccess servicioBD)
        {
            this.servicioBD = servicioBD;
        }



        #endregion


        #region ... metodos de la clase ...

        /// <summary>
        /// Añade el libro correspondiente al ISBN13 al pedido actual del cliente
        /// </summary>
        /// <param name="idISBN13">ISBN13 del libro a añadir</param>
        /// <returns>Nos retorna a la vista Mostrar Pedido</returns>
        [HttpGet]
        public IActionResult addLibroPedido(String idISBN13)
        {
            try
            {
                // comprondo antes si existe o no dentro del pedido actual (si existe, incremento cantidad en una unidad)
                Cliente cliente = JsonSerializer.Deserialize<Cliente>(HttpContext.Session.GetString("datosCliente"));

                int posicionLibro = cliente.PedidoActual.ItemsPedido.FindIndex((ItemPedido item) => item.LibroItem.ISBN13 == idISBN13);
                if (posicionLibro == -1)
                {
                    Libro libroRecuperado = this.servicioBD.recuperarLibroPorISBN13(idISBN13);
                    // no existe el libro en el pedido actual, necesito recuperar el libro de la base de datos con ese ISBN13
                    cliente.PedidoActual.ItemsPedido.Add(new ItemPedido { LibroItem = libroRecuperado, CantidadItem = 1});
                }
                else
                {
                    // existe el libro, incremento su valor
                    cliente.PedidoActual.ItemsPedido[posicionLibro].CantidadItem += 1;
                }

                // actualizo la variable de sesion
                HttpContext.Session.SetString("datosCliente", JsonSerializer.Serialize<Cliente>(cliente));

                return RedirectToAction("MostrarPedido");
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }

        /// <summary>
        /// Funcion para mostrar la vista MostrarPedio
        /// </summary>
        /// <returns>La propia vista</returns>
        [HttpGet]
        public async Task<IActionResult> MostrarPedido()
        {
            try
            {
                // Recuperamos el cliente del estado de sesion
                Cliente cliente = JsonSerializer.Deserialize<Cliente>(HttpContext.Session.GetString("datosCliente"));

                // es necesario recuperar del servicio REST la lista de provincias y municipios para pasarselas a la vista
                HttpClient clienteHttp = new HttpClient();
                APIRestResponse<Provincia> respuesta = await clienteHttp.GetFromJsonAsync<APIRestResponse<Provincia>>("https://apiv1.geoapi.es/provincias?type=JSON&key=&sandbox=1");

                // Pasamos los datos del controlador a la vista sin usar @model
                ViewData["provincia"] = respuesta.datosConvertidos(respuesta.Data);

                return View(cliente);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Funcion para realizar distintas operaciones con la cantidad de los items del pedido
        /// </summary>
        /// <param name="idISBN13">Identificador del item (libro)</param>
        /// <param name="operacion">Operacion a realizar con el item</param>
        /// <returns>Vista MostrarPedio</returns>
        [HttpGet]
        public IActionResult OperarCantidad(String idISBN13, [FromQuery] String operacion)
        {
            try
            {
                Cliente cliente = JsonSerializer.Deserialize<Cliente>(HttpContext.Session.GetString("datosCliente"));
                int posicionItem = cliente.PedidoActual.ItemsPedido.FindIndex((ItemPedido item) => item.LibroItem.ISBN13 == idISBN13 );

                if (posicionItem == -1) throw new Exception("El libro con ese ISBN no existe en el pedido actual");

                switch (operacion)
                {
                    case "eliminar":
                        if (posicionItem != -1) cliente.PedidoActual.ItemsPedido.RemoveAt(posicionItem);
                        break;

                    case "sumar":
                        if (posicionItem != -1) cliente.PedidoActual.ItemsPedido[posicionItem].CantidadItem += 1;
                        break;

                    case "restar":
                        if (posicionItem != -1 && cliente.PedidoActual.ItemsPedido[posicionItem].CantidadItem > 1) cliente.PedidoActual.ItemsPedido[posicionItem].CantidadItem -= 1;
                        break;
                }

                HttpContext.Session.SetString("datosCliente", JsonSerializer.Serialize<Cliente>(cliente));
                return RedirectToAction("MostrarPedio");

                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        [HttpPost]
        public IActionResult FinalizarPedido(Cliente datoscliente,
                                    [FromForm] String direccionradios,
                                    [FromForm] String calle,
                                    [FromForm] String cp,
                                    [FromForm] String pais,
                                    [FromForm] String provincia,
                                    [FromForm] String municipio,
                                    [FromForm] String nombre,
                                    [FromForm] String apellidos,
                                    [FromForm] String email,
                                    [FromForm] String telefono,
                                    [FromForm] String otrosdatos,
                                    [FromForm] String datosfacturaradios,
                                    [FromForm] String nombreEmpresa,
                                    [FromForm] String cifEmpresa,
                                    [FromForm] String pagoradios,
                                    [FromForm] String numerocard,
                                    [FromForm] String aniocard,
                                    [FromForm] String mescard,
                                    [FromForm] String cvv,
                                    [FromForm] String nombrebancocard )
        {
            try
            {
                Cliente cliente = JsonSerializer.Deserialize<Cliente>(HttpContext.Session.GetString("datosCliente"));
                return View(cliente);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Opera con la cantidad de libros
        /// </summary>
        /// <param name="id">tercer segmento de la url</param>
        /// <param name="operacion">parametro en la url</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult operarCantidad(String id, [FromQuery] String operacion)
        {
            try
            {
                Cliente cliente = JsonSerializer.Deserialize<Cliente>(HttpContext.Session.GetString("datosCliente"));

                int posItem = cliente.PedidoActual.ItemsPedido.FindIndex((ItemPedido item) => item.LibroItem.ISBN13 == id);

                if (posItem == -1) throw new Exception("El libro con ese ISBN no existe en el pedido actual");

                switch (operacion)
                {
                    case "eliminar":
                        if (posItem != -1) cliente.PedidoActual.ItemsPedido.RemoveAt(posItem);
                        break;

                    case "sumar":
                        if (posItem != -1) cliente.PedidoActual.ItemsPedido[posItem].CantidadItem += 1;
                        break;

                    case "restar":
                        if (posItem != -1 && cliente.PedidoActual.ItemsPedido[posItem].CantidadItem > 1)
                        {
                            cliente.PedidoActual.ItemsPedido[posItem].CantidadItem -= 1;
                        }
                        break;
                }

                HttpContext.Session.SetString("datosCliente", JsonSerializer.Serialize<Cliente>(cliente));
                return RedirectToAction("MostrarPedido");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        /*
        [HttpGet]
        public IActionResult eliminarLibroPedido(String id)
        {
            // en el parametro id va el libro a eliminar...
            // recupero de la variable de sesion el objeto cliente, obtengo el pedido actual <-- Elementos del pedido
            // busco el elemento cuyo libro tenga ese isbn y lo borro
            // Despues actualizo la variable de sesion y redirijo a MostrarPedido

            try
            {
                Cliente cliente = JsonSerializer.Deserialize<Cliente>(HttpContext.Session.GetString("datosCliente"));

                int posEliminar = cliente.PedidoActual.ItemsPedido.FindIndex( (ItemPedido item)=> item.LibroItem.ISBN13 == id );
                if (posEliminar != -1) cliente.PedidoActual.ItemsPedido.RemoveAt(posEliminar);

                HttpContext.Session.SetString("datosCliente", JsonSerializer.Serialize<Cliente>(cliente));
                return RedirectToAction("MostrarPedido");

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpGet]
        public IActionResult sumarCantidadLibro(String id)
        {
            // en el id va el isbn13 del libro del que quiero incrementar la cantidad
            try
            {
                Cliente cliente = JsonSerializer.Deserialize<Cliente>(HttpContext.Session.GetString("datosCliente"));

                int posicionItemLibro = cliente.PedidoActual.ItemsPedido.FindIndex((ItemPedido item)=> item.LibroItem.ISBN13 == id);

                if (posicionItemLibro == -1) throw new Exception("Libro no existe en el pedido... ");

                cliente.PedidoActual.ItemsPedido[posicionItemLibro].CantidadItem += 1;

                HttpContext.Session.SetString("datosCliente", JsonSerializer.Serialize<Cliente>(cliente));
                return RedirectToAction("MostrarPedido");

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        [HttpGet]
        public IActionResult restarCantidadLibro(String id)
        {
            // en el id va el isbn13 del libro del que quiero incrementar la cantidad

            try
            {
                Cliente cliente = JsonSerializer.Deserialize<Cliente>(HttpContext.Session.GetString("datosCliente"));

                int posicionItemLibro = cliente.PedidoActual.ItemsPedido.FindIndex((ItemPedido item) => item.LibroItem.ISBN13 == id);

                if (posicionItemLibro == -1) throw new Exception("Libro no existe en el pedido... ");

                if (cliente.PedidoActual.ItemsPedido[posicionItemLibro].CantidadItem > 1) cliente.PedidoActual.ItemsPedido[posicionItemLibro].CantidadItem -= 1;

                HttpContext.Session.SetString("datosCliente", JsonSerializer.Serialize<Cliente>(cliente));
                return RedirectToAction("MostrarPedido");

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        */

        #endregion


    }
}
