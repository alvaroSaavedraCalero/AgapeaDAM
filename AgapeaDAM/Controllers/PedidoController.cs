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

        [HttpGet]
        public IActionResult addLibroPedido(String idISBN13)
        {
            try
            {
                // en el parametro id va el ISBN13 del libro que quiere comprar el cliente
                // tengo que añadir el libro correspondiente a ese ISBN13 al pedido actual del cliente
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

                throw;
            }
            
        }

        [HttpGet]
        public IActionResult MostrarPedido()
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
