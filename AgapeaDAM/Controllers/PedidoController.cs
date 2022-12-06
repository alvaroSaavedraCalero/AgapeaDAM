using AgapeaDAM.Models;
using AgapeaDAM.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System.Text.Json;

namespace AgapeaDAM.Controllers
{
    public class PedidoController : Controller
    {
        #region ... propiedades de la clase ...

        private IBDAccess servicioBD;
        private IConfiguration config; //<--- variable interna donde encapsulamos servicio para acceder a fichero configuracion appsettings.json

        public PedidoController(IBDAccess servicioBD, IConfiguration config)
        {
            this.servicioBD = servicioBD;
            this.config = config;
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

                // la direccion depende si el radio direccionradios esta chequeado la direccion principal del cliente y otra direccion...
                Direccion direccionStripe;

                if (direccionradios == "direccionprincipal")
                {
                    direccionStripe = cliente.MisDireciones.Where((Direccion direc) => direc.EsPrincipal == true).Single<Direccion>();
                }
                else
                {
                    direccionStripe = new Direccion
                    {
                        Calle = calle,
                        CP = System.Convert.ToInt32(cp),
                        Pais = pais,
                        ProvDirec = new Provincia { CPRO = provincia.Split('-')[0], PRO = provincia.Split('-')[1], CCOM = "" },
                        MuniDirecc = new Municipio { CPRO = provincia.Split('-')[0], CMUM = municipio.Split('-')[0], DMUN50 = municipio.Split('-')[1], CUN = "" }
                    };
                }

                // si la variable pasada en el formulario "pagoradios" vale "pagotarjeta" pago con stripe si vale "pagopaypal"
                if (pagoradios == "pagotarjeta")
                {
                    #region ... pago mediante tarjeta de credito con stripe ...

                    //OJO!!!! NO HAY Q CREARSE UN OBJETO CUSTOMER Y CARD SIEMPRE, si el cliente ya ha comprado antes
                    //lo tendra creado, habra q comprobar si lo tiene creado o no, usando metodo de la api stripe 
                    //"SEARCH CUSTOMER": https://stripe.com/docs/api/customers/search?lang=dotnet

                    // 1º paso es crearte un objeto Stripe CUSTOMER (Cliente) sobre el cual vas a cargar el cargo del pedido
                    // https://stripe.com/docs/api/customers/create?lang=dotnet

                    StripeConfiguration.ApiKey = this.config.GetSection("StripeAPIKey").Value;

                    CustomerCreateOptions options = new CustomerCreateOptions
                    {
                        Email = cliente.CuentaCliente.Email,
                        Name = cliente.Nombre + " " + cliente.Apellidos,
                        Phone = cliente.Telefono,
                        Address = new AddressOptions
                        {
                            City = direccionStripe.MuniDirecc.DMUN50,
                            State = direccionStripe.ProvDirec.PRO,
                            Country = direccionStripe.Pais,
                            Line1 = direccionStripe.Calle,
                            PostalCode = direccionStripe.CP.ToString()
                        },
                        Description = "Cliente de Agapea.com",
                        Metadata = new Dictionary<string, string>
                    {
                        {"FechaNacimiento", cliente.FechaNacimiento.ToString() },
                        {"IdCliente", cliente.IdCliente }
                    }
                    };

                    Customer customer = new CustomerService().Create(options);

                    // 2º paso es asociar a este objeto CUSTOMER un medio de pago, en nuestro caso una tarjeta de credito
                    // objeto CARD de stripe

                    // 2.1 primero crear lo que se denomina un TOKEN para la tarjeta a asociar al cliente (donde se especifican las caract. de 
                    // la tarjeta (numero, fecha exp. numero cvv, nombre del propietario tarjeta, etc...)
                    //https://stripe.com/docs/api/tokens/create_card?lang=dotnet

                    TokenCreateOptions optionsCardToken = new TokenCreateOptions
                    {
                        Card = new TokenCardOptions
                        {
                            Number = numerocard, // <-- para hacer pruebas meter este numero: "4242424242424242",
                            ExpMonth = mescard,
                            ExpYear = aniocard,
                            Cvc = cvv,
                            Name = cliente.Nombre + " " + cliente.Apellidos
                        }
                    };

                    Token tokenTarjeta = new TokenService().Create(optionsCardToken);

                    // 2.2 usando este TOKEN, se crear la tarjeta objeto CARD y se asocia al cliente creado en el paso 1º
                    //https://stripe.com/docs/api/cards/create?lang=dotnet

                    CardCreateOptions cardOptions = new CardCreateOptions
                    {
                        Source = tokenTarjeta.Id
                    };

                    Card tarjetaCredito = new CardService().Create(customer.Id, cardOptions);


                    // 3º paso es crear el cargo (el pago a realizar por el cliente usando esa tarjeta), es un objeto CHARGE de stripe
                    // defines la cantidad a pagar total, los gastos, el tipo de moneda, ... una vez pasado el cargo vemos su estado
                    // para ver si se ha cargado
                    // //https://stripe.com/docs/api/charges/create?lang=dotnet

                    ChargeCreateOptions chargeOptions = new ChargeCreateOptions
                    {
                        Amount = System.Convert.ToInt64(cliente.PedidoActual.Total) * 100,
                        Currency = "eur",
                        Source = tarjetaCredito.Id,
                        Description = "Pedido de Agapea.com con el IdPedido: " + cliente.PedidoActual.IdPedido,
                        Customer = customer.Id
                    };

                    Charge cargoPedido = new ChargeService().Create(chargeOptions);

                    if (cargoPedido.Status.ToLower() == "succeeded")
                    {
                        return RedirectToAction("FinalizarPedidoOK");
                    }
                    else
                    {
                        throw new Exception("Pago rechazado por la pasarela de pago, revisa lso datos de tu tarjeta eintentalo de nuevo mas tarde");
                    }

                    #endregion
                }
                else
                {
                    #region ... pago mediante paypal ...

                    return View();

                    #endregion

                }
            } catch (Exception ex)
            {
                ViewData["errores"] = ex.Message;
                return View();
            }
        }

        /// <summary>
        /// Opera con la cantidad de libros
        /// </summary>
        /// <param name="id">tercer segmento de la url</param>
        /// <param name="operacion">parametro en la url</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult OperarCantidad(String id, [FromQuery] String operacion)
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


        #endregion


    }
}
