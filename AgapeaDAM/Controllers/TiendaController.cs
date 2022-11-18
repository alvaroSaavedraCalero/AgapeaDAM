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
        /// <returns></returns>
        [HttpGet]
        public IActionResult RecuperaLibros(String idCategoria)
        {
            // en el parametro id va el IdCategoria de los libros que se van a mostrar 
            // usando el servicio de acceso a datos recuperar los libros de esa categoria 
            // y pasarselos a la vista para que los pinte

            List<Libro> listaLibros = this.servicioBD.recuperaLibros(idCategoria);
            return View(listaLibros);
        }

        #endregion

    }
}
