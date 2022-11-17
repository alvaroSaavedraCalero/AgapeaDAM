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
        public IActionResult AddLibroPedido(String idISBN13)
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
            return View();
        }

        #endregion


    }
}
