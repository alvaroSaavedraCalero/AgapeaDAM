﻿using AgapeaDAM.Models;
using AgapeaDAM.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json;

using System.Text.RegularExpressions;

namespace AgapeaDAM.Controllers
{
    public class ClienteController : Controller
    {
        #region ...propiedades clase cliente-controller.....

        private IBDAccess __servicioBD; //<---- variable donde voy a almacenar el servicio de acceso a datos inyectado en el constructor
        private IClienteEmail clienteEmail;

        #endregion

        /*
         para solicitar al modulo de inyeccion de dependencias del servidor un servicio, se invoca en el constructor
         */
        public ClienteController(IBDAccess servicioInyetado, IClienteEmail servicioEmail)
        {
            this.__servicioBD = servicioInyetado;
            this.clienteEmail = servicioEmail;

        }


        #region ....metodos clase cliente-controller....


        #region 1-metodos devuelven vistas

        #region _______________REGISTRO_____________
        [HttpGet]
        public IActionResult Registro()
        {
            return View(new Cliente());
        }

        [HttpPost]
        public IActionResult Registro(Cliente datoscliente, [FromForm] Boolean condUso)
        {
            //1º validar....si esta todo ok
            if (condUso == false)
            {
                ModelState.AddModelError("", "*Debes Aceptar las condiciones de USO");
                return View(datoscliente);
            }


            if (ModelState.IsValid)
            {
                //2º guardar datos en bd, en tabla clientes y cuentas
                if (this.__servicioBD.RegistrarClliente(datoscliente))
                {

                    //3º mandamos email activacion
                    String bodyEmail = "<h3><strong>Te has registrado correctamente en Agapea.com</strong><7h3>" +
                        $"<hr><p>Pulsa <a href='https://localhost:7281/Cliente/activarCuenta/{datoscliente.CuentaCliente.IdCuenta}'>AQUI</a> para activar tu cuenta de correo y poder hacer LOGIN en el portal</p></hr>";

                    this.clienteEmail.enviarEmail(datoscliente.CuentaCliente.Email,
                        "Activa tu cuenta en Apagea.com",
                        bodyEmail,
                        "");

                    //4º redirigmos a vista de REGISTRO OK
                    return RedirectToAction("RegistroOK");

                }
                else
                {
                    ModelState.AddModelError("", "Fallos internos en el servidor, intentalo de nuevo mas tarde");
                    return View(datoscliente);
                }
            }
            else
            {
                //vuelvo a la vista Registro.cshtml con los datos del objeto Cliente q me han mandado para q el model-binder pinte errores de validacion
                return View(datoscliente);

            }
        }

        [HttpGet]
        public IActionResult RegistroOK()
        {
            return View();
        }

        #endregion

        #region ______________LOGIN_______________________

        [HttpGet]
        public IActionResult Login()
        {
            return View(new Cuenta());
        }

        [HttpPost]
        public IActionResult Login(Cuenta cuentauser)
        {
            /*
              si pregunto por ModelStata.IsValid siempre va a ser FALSE aunque los campos Email y Password cumplan validaciones
            ¿por que? pq la propiedad LOGIN del modelo no ESTA ASOCIADA en la vista A NINGUN CAMPO!!! y por tanto no se puede validar su contenido y siempre
            su estado de validacion es FALSE
             */
            if (
                ModelState.GetValidationState("Email") == ModelValidationState.Valid &&
                ModelState.GetValidationState("Password") == ModelValidationState.Valid
                )
            {

                //ANTES Q RECUPERAR LOS DATOS DEL CLIENTE, TENGO Q VERIFICAR SI LA CUENTA ESTA ACTIVADA O NO...
                //pq sino tienes q mandar un nuevo mensaje de email de activacion...
                if (this.__servicioBD.ComprobarCuentaActiva(cuentauser.Email))
                {
                    //usando el servicio de acceso a datos, comprobar el email y la password en la bd
                    //si existen y coinciden con los de alguna cuenta
                    Cliente _clienteLogged = this.__servicioBD.ExisteCliente(cuentauser.Email, cuentauser.Password);
                    if (_clienteLogged != null)
                    {
                        //meto datos del cliente en estado de sesion...
                        // metemos en el diccionario del estado de sesion el String y el objeto de tipo Cliente serializado en formato JSON
                        HttpContext.Session.SetString("datosCliente", JsonSerializer.Serialize<Cliente>(_clienteLogged));


                        //redirecciono al panel del usuario...
                        return RedirectToAction("InicioPanel");
                    }
                    else
                    {
                        ModelState.AddModelError("", "*Email o Password Incorrectos, intentelo de nuevo...");
                        return View(cuentauser);
                    }

                }
                else
                {
                    //cuenta no activada...reenviar el email

                    //y mandar al login
                    ModelState.AddModelError("", "*Tu cuenta no esta ACTIVADA, se te ha mandado un email para que la actives.");
                    return View(cuentauser);
                }



            }
            else
            {
                return View(cuentauser);
            }
        }

        #endregion


        #region _____________PANEL_CLIENTES_________________

        [HttpGet]
        public async Task<IActionResult> InicioPanel()
        {
            // Siempre que se recuperan los datos de sesion en un try-catch por si la cookie ya no esta (caducada, eliminada, etc)
            try
            {
                // Para pasarle los datos del cliente a la vista los recupero del estado de sesion
                Cliente clienteLogeado = JsonSerializer.Deserialize<Cliente>(HttpContext.Session.GetString("datosCliente"));
                // tengo que recuperar del servicio externos la lista de provincias y pasarselas a la vista
                HttpClient clienteHttp = new HttpClient();
                // guardamos el JSON recibido como una lista de objetos provincia
                APIRestResponse<Provincia> respuesta = await clienteHttp.GetFromJsonAsync<APIRestResponse<Provincia>>
                    ("https://apiv1.geoapi.es/provincias?type=JSON&key=&sandbox=1");


                // -----------paso de datos del controlador a la vista sin usar modelo-------------
                // 1º froma utilizando prop. de controladores/vistas : ViewData <----Dicctionary<String,Object>
                ViewData["provincias"] = respuesta.datosConvertidos(respuesta.Data);

                // 2º forma ultilizando una propiedad de controladores/vistas: ViewBag <--- dynamic Object
                //ViewBag.provincias = listaProvincias;

                // 3º forma ultilizando una prop. de controladores/vistas: TempData <---Dicctionaty<Sring, Object>
                // no se destruye en la misma peticion como el ViewData o el ViewBag si no que permanece entre
                // dos peticiones del cliente seguidas
                // p.e. -> entre Login y InicioPanel

                return View(clienteLogeado);


            }
            catch (Exception ex)
            {
                // le obligo a que se loguee de nuevo para crear la cookie con el token de sesion y el servidor reserve un estado de sesion nuevo
                return RedirectToAction("Login");
            }

        }

        [HttpPost]
        public IActionResult AltaDireccion([FromForm] String calle, [FromForm] String cp,
            [FromForm] String pais, [FromForm] String provincia, [FromForm] String municipio, [FromForm] String operacion)
        {
            try
            {
                // ojo,  cuando este borrando TODOS LOS PARAMENTROS ESTAN A NULL!!! menos operacion
                // hay que inicializarlos a algo por defecto, pq sino al construir el objeto Direccion peta

                if (new Regex("^borrar_").IsMatch(operacion))
                {
                    calle = ""; cp = "0"; pais = "España"; provincia = "0-xdxd"; municipio = "0-xdxd";
                }

                Cliente cliente = JsonSerializer.Deserialize<Cliente>(HttpContext.Session.GetString("datosCliente"));

                Direccion nuevaDireccion = new Direccion
                {
                    Calle = calle,
                    CP = System.Convert.ToInt32(cp),
                    Pais = pais,
                    ProvDirec = new Provincia
                    {
                        CCOM = "",
                        CPRO = provincia.Split('-')[0],
                        PRO = provincia.Split('-')[1]
                    },
                    MuniDirecc = new Municipio
                    {
                        CPRO = provincia.Split('-')[0],
                        CMUM = municipio.Split('-')[0],
                        DMUN50 = municipio.Split('-')[1],
                        CUN = ""
                    },
                    EsPrincipal = false,
                    EsFacturacion = false
                };

                // si estamos modificando o borrando una direccion, el IdDireccion generado automaticamente lo tengo 
                // que machacar con el IdDireccion que quiero borrar o modificar, y lo saco del segundo campo del parametro operacion
                if (new Regex("^(modificar_ | borrar_)").IsMatch(operacion)) { nuevaDireccion.IdDireccion = operacion.Split('_')[1]; }

                if (this.__servicioBD.operarDireccion(nuevaDireccion, cliente.IdCliente, operacion.Split('_')[0]))
                {
                    // el insert, update o delete ha ido bien
                    // hay que actualizar la lista de direcciones del cliente y meterlo en la variable de sesion
                    // si la operacion es "crear" directamente añado direccion
                    // si la operacion es "modificar_..." borro direccion en esa posicion en la lista de direcciones y añado nueva direccion
                    // si la operacion es "borrar_..." borro direccion en esa posicion en la lista de direcciones

                    int posDireccion = cliente.MisDireciones.FindIndex((Direccion direc) => direc.IdDireccion == nuevaDireccion.IdDireccion);

                    switch (operacion.Split('_')[0])
                    {
                        case "borrar":
                            cliente.MisDireciones.RemoveAt(posDireccion);
                            break;

                        case "modificar":
                            cliente.MisDireciones.RemoveAt(posDireccion);
                            cliente.MisDireciones.Add(nuevaDireccion);
                            break;

                        case "crear":
                            cliente.MisDireciones.Add(nuevaDireccion);
                            break;
                    }

                    // actualizo variable de sesion
                    HttpContext.Session.SetString("datosCliente", JsonSerializer.Serialize<Cliente>(cliente));
                } else
                {
                    // algo ha fallado en el insert, updatre o delete en bd, mandar mensaje de error a la vista
                }

                return RedirectToAction("InicioPanel");

                
            }
            catch (Exception)
            {

                return RedirectToAction("Login");
            }


        }

        /*
        
        [HttpPost]
        public IActionResult InicioPanel(Cliente clienteLogeado)
        {

        }
        
        */
        #endregion


        #region funciones especiales
        [HttpGet]
        public IActionResult activarCuenta(String id)
        {
            // en el tercer segmento del enlace que se manda por correo al usuario para activar su cuenta va el IDCUENTA
            // este tercer segmento se mapea contra parametro "id" de la ruta, y asi se define en el metodo de accion
            // activarCuenta(String id) <--- en ese parametro "id" se recive el IdCuenta del cliente a Activar
            if (this.__servicioBD.activarCuentaCliente(id))
            {
                // se ha activado cuenta ok,,, redirigimos login
                return RedirectToAction("Login");
            }
            else
            {
                // o bien el idCuenta no existe... o bien ha habido una Excepcion al intentar hacer el UPDATE
                // puedo : 1º comprobar si ese idCuenta existe, si existe se lo vuelve a mandar el email de nuevo
                //          2º si no existe ese idCuenta se reenvia al registro
                return RedirectToAction("Registro");
            }
        }
        #endregion

        #endregion


        #region 2-metodos funcionales

        #endregion

        #endregion
    }
}