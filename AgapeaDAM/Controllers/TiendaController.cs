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

        [HttpPost]
        public IActionResult RecuperaLibros(String id)
        {
            // en el parametro id va el IdCategoria de los libros que se van a mostrar 
            // usando el servicio de acceso a datos recuperar los libros de esa categoria 
            // y pasarselos a la vista para que los pinte
            return View();
        }

        #endregion

    }
}
