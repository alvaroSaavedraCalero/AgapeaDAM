using AgapeaDAM.Models;
using AgapeaDAM.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AgapeaDAM.Controllers
{
    public class TiendaController : Controller
    {

        #region ... propiedades de la clase ...

        private IBDAccess servicioBD;

        public TiendaController(IBDAccess servicioBD)
        {
            this.servicioBD = servicioBD;
        }



        #endregion


        #region ... metodos de la clase ...

        /// <summary>
        /// Metodo para mostrar los libros en la vista "RecuperaLibros"
        /// </summary>
        /// <param name="id">El idCategoria de cada libro</param>
        /// <returns>La vista RecuperaLibros con los libros a pintar</returns>
        [HttpGet]
        public IActionResult RecuperaLibros(String idCategoria)
        {
            //si el id esta vacio, estamos en la pag.inicial (se acaba de entrar)
            //se podria cargar los libros mas vendidos del mes, las ofertas especiales,...
            //nosotros, para q cargue algo, hacemos q si esta vacio cargue  libros de informatica...
            

            // si el id esta vacio, estamos en la pagina incial 
            // se podrian cargar los libros mas vendido del mes, o las ofertas especiales, etc

            // nosotros, para que cargue algo, hacemos que si esta vacio, cargue los libros de informatica.

            if (String.IsNullOrEmpty(idCategoria)) idCategoria = "2";

            List<Libro> listaLibros = this.servicioBD.recuperaLibros(idCategoria);
            return View(listaLibros);
        }

        /// <summary>
        /// Metodo para mostrar la vista del libro que se seleccione
        /// </summary>
        /// <param name="idISBN13">ISBN13 del libro seleccionado</param>
        /// <returns>La propia vista MostrarDetallesLibro</returns>
        [HttpGet]
        public IActionResult MostrarDetallesLibro(String idISBN13)
        {
            Libro libroSeleccionado = this.servicioBD.recuperarLibroPorISBN13(idISBN13);
            return View(libroSeleccionado);
        }

        #endregion

    }
}
