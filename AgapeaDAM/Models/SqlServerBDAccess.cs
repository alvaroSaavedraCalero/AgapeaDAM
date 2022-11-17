using AgapeaDAM.Models.Interfaces;
using System.Data.SqlClient;

using BCrypt.Net; //paquete con clases para cifrar/comprobar passwords
using Microsoft.AspNetCore.Razor.Language.Extensions;

// Lo mejor es que todas las funciones sean async

namespace AgapeaDAM.Models
{
    public class SqlServerBDAccess : IBDAccess
    {
        #region ...propiedades de la clase de acceso a datos contra sqlserver....
        public string CadenaConexionSever { get; set; } = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ApageaBD;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";



        #endregion




        #region ...metodos de la clase de acceso a datos contra sqlserver....

        #region ... metodos panel cliente ...

        /// <summary>
        /// Funcion para registrar un cliente en la base de datos
        /// </summary>
        /// <param name="clienteAguardar">Los datos del cliente a registrar</param>
        /// <returns>Retorna true en caso de que todo vaya bien</returns>
        public Boolean RegistrarClliente(Cliente clienteAguardar)
        {

            //si todo ok, return true...si hay excepciones return false
            try
            {
                //1º abrir conexion a la BD usando la cadena conexion
                SqlConnection __conexionBD = new SqlConnection(this.CadenaConexionSever);
                __conexionBD.Open();

                //2º insert en tabla clientes ----si ok--> insert en tabla cuentas
                SqlCommand __insertCli = new SqlCommand();
                __insertCli.Connection = __conexionBD;
                __insertCli.CommandText = "INSERT INTO dbo.Clientes VALUES (@id,@nom,@ap,@tlfno,@idc,@fech,@nf,@ge,@desc)";
                __insertCli.Parameters.AddWithValue("@id", clienteAguardar.IdCliente);
                __insertCli.Parameters.AddWithValue("@nom", clienteAguardar.Nombre);
                __insertCli.Parameters.AddWithValue("@ap", clienteAguardar.Apellidos);
                __insertCli.Parameters.AddWithValue("@tlfno", clienteAguardar.Telefono);
                __insertCli.Parameters.AddWithValue("@idc", clienteAguardar.CuentaCliente.IdCuenta);
                __insertCli.Parameters.AddWithValue("@fech", clienteAguardar.FechaNacimiento);
                __insertCli.Parameters.AddWithValue("@nf", clienteAguardar.NIF);
                __insertCli.Parameters.AddWithValue("@ge", clienteAguardar.Genero);
                __insertCli.Parameters.AddWithValue("@desc", clienteAguardar.Descripcion);

                int __numfilasinsertcli = __insertCli.ExecuteNonQuery(); //metodo .executenonquery para iNSERTS, UPDATES, DELETES
                if (__numfilasinsertcli == 1)
                {
                    SqlCommand __insertCu = new SqlCommand("INSERT INTO dbo.Cuentas VALUES(@id,@log,@em,@pass,@img,@idc,@cueAct, @img64)", __conexionBD);
                    __insertCu.Parameters.AddWithValue("@id", clienteAguardar.CuentaCliente.IdCuenta);
                    __insertCu.Parameters.AddWithValue("@log", clienteAguardar.CuentaCliente.Login);
                    __insertCu.Parameters.AddWithValue("@em", clienteAguardar.CuentaCliente.Email);

                    //hasheamos password...instalando paquete NuGet Bcrypt.Net
                    //__insertCu.Parameters.AddWithValue("@pass", clienteAguardar.CuentaCliente.Password);
                    String __hasspass = BCrypt.Net.BCrypt.HashPassword(clienteAguardar.CuentaCliente.Password);
                    __insertCu.Parameters.AddWithValue("@pass", __hasspass);

                    __insertCu.Parameters.AddWithValue("@img", clienteAguardar.CuentaCliente.ImagenAvatar);
                    __insertCu.Parameters.AddWithValue("@idc", clienteAguardar.IdCliente);
                    __insertCu.Parameters.AddWithValue("@cueAct", clienteAguardar.CuentaCliente.CuentaActivada);
                    __insertCu.Parameters.AddWithValue("@img64", clienteAguardar.CuentaCliente.ImagenAvatarBASE64);


                    int __numfilasinsertcu = __insertCu.ExecuteNonQuery();
                    if (__numfilasinsertcu == 1)
                    {
                        return true;

                    }
                    else
                    {
                        return false; //<---- fallo en insertar valores en tabla cuentas
                    }

                }
                else
                {
                    return false; // <---- fallo en insertar valores en tabla clientes
                }
            }
            catch (Exception ex)
            {
                return false; //<---- excepcion por timeout, problemas en parametros, etc....
            }

        }

        /// <summary>
        /// Funcion para comprobar si existe un cliente en la base de datos
        /// </summary>
        /// <param name="email">Email del cliente a comprobar</param>
        /// <param name="password">Contraseña del cliente a comprobar</param>
        /// <returns>Retorna el cliente con todos los datos existentes en la BD, en caso de que exista</returns>
        public Cliente ExisteCliente(string email, string password)
        {
            try
            {

                Cliente _clienteADevolver = new Cliente();

                SqlConnection _conexionBD = new SqlConnection(this.CadenaConexionSever);
                _conexionBD.Open();


                SqlCommand _selectCuenta = new SqlCommand("SELECT * FROM dbo.Cuentas WHERE Email=@em", _conexionBD);
                _selectCuenta.Parameters.AddWithValue("@em", email);


                SqlDataReader _cursorCuenta = _selectCuenta.ExecuteReader();
                if (_cursorCuenta.HasRows)
                {
                    String _hash = "";
                    while (_cursorCuenta.Read())
                    {
                        _clienteADevolver.CuentaCliente.Login = _cursorCuenta["Login"].ToString();
                        _clienteADevolver.CuentaCliente.Email = _cursorCuenta["Email"].ToString();
                        _clienteADevolver.CuentaCliente.Password = "";
                        _clienteADevolver.CuentaCliente.ImagenAvatar = _cursorCuenta["ImagenAvatar"].ToString();
                        _clienteADevolver.CuentaCliente.ImagenAvatarBASE64 = _cursorCuenta["ImagenAvatarBASE64"].ToString();
                        _clienteADevolver.CuentaCliente.IdCuenta = _cursorCuenta["IdCuenta"].ToString();
                        _clienteADevolver.CuentaCliente.CuentaActivada = System.Convert.ToBoolean(_cursorCuenta["CuentaActivada"]);
                        _clienteADevolver.IdCliente = _cursorCuenta["IdCliente"].ToString();
                        _hash = _cursorCuenta["Password"].ToString();

                    }
                    _cursorCuenta.Close();
                    _cursorCuenta.Dispose();
                    if (BCrypt.Net.BCrypt.Verify(password, _hash))
                    {
                        SqlCommand _selectClientes = new SqlCommand("SELECT * FROM dbo.Clientes WHERE IdCliente=@id", _conexionBD);
                        _selectClientes.Parameters.AddWithValue("@id", _clienteADevolver.IdCliente);

                        SqlDataReader _cursorCliente = _selectClientes.ExecuteReader();
                        //leo el cursor del resultado de la SELECT sobre Clientes con un bucle while....

                        while (_cursorCliente.Read())
                        {
                            _clienteADevolver.Nombre = _cursorCliente["Nombre"].ToString();
                            _clienteADevolver.Apellidos = _cursorCliente["Apellidos"].ToString();
                            _clienteADevolver.FechaNacimiento = System.Convert.ToDateTime(_cursorCliente["FechaNacimiento"]);
                            _clienteADevolver.NIF = _cursorCliente["NIF"].ToString();
                            _clienteADevolver.Telefono = _cursorCliente["Telefono"].ToString();
                        }
                        _cursorCliente.Close();
                        _cursorCliente.Dispose();

                        // Antes de cerrar la conexion hay que cargar la lista de direcciones, lista de pedidos, lista de opiniones, ...
                        SqlCommand selectDirecciones = new SqlCommand("select * from dbo.Direcciones where IdCliente = @id", _conexionBD);
                        selectDirecciones.Parameters.AddWithValue("@id", _clienteADevolver.IdCliente);
                        SqlDataReader cursorDirecciones = selectDirecciones.ExecuteReader();
                        while (cursorDirecciones.Read())
                        {
                            Direccion nuevaDirecc = new Direccion
                            {
                                IdDireccion = cursorDirecciones["IdDireccion"].ToString(),
                                Calle = cursorDirecciones["Calle"].ToString(),
                                CP = System.Convert.ToInt32(cursorDirecciones["CP"]),
                                ProvDirec = new Provincia
                                {
                                    CCOM = "",
                                    CPRO = cursorDirecciones["Provincia"].ToString().Split('-')[0],
                                    PRO = cursorDirecciones["Provincia"].ToString().Split('-')[1]
                                },
                                MuniDirecc = new Municipio
                                {
                                    CPRO = cursorDirecciones["Provincia"].ToString().Split('-')[0],
                                    CMUM = cursorDirecciones["Municipio"].ToString().Split('-')[0],
                                    DMUN50 = cursorDirecciones["Municipio"].ToString().Split('-')[1],
                                    CUN = ""
                                }
                            };
                            _clienteADevolver.MisDireciones.Add(nuevaDirecc);
                        }
                        cursorDirecciones.Close();

                        _conexionBD.Close();

                        return _clienteADevolver;
                    }
                    else
                    {
                        return null; //password invalida....
                    }


                }
                else
                {
                    return null; //email no existe...
                }
            }
            catch (Exception ex)
            {

                return null;
            }
        }

        /// <summary>
        /// Funcion para comprobar si la cuenta de un cliente esta activa
        /// </summary>
        /// <param name="email">El email del cliente</param>
        /// <returns>Retorna true en caso de que la cuenta este activa</returns>
        public bool ComprobarCuentaActiva(string email)
        {
            try
            {
                SqlConnection _conexionBD = new SqlConnection(this.CadenaConexionSever);
                _conexionBD.Open();

                SqlCommand _selectCuenta = new SqlCommand("SELECT * FROM dbo.Cuentas WHERE Email=@em", _conexionBD);
                _selectCuenta.Parameters.AddWithValue("@em", email);

                SqlDataReader _cursor = _selectCuenta.ExecuteReader();

                Boolean _cuentaActiva = false;
                while (_cursor.Read())
                {
                    _cuentaActiva = System.Convert.ToBoolean(_cursor["CuentaActivada"]);
                }

                _cursor.Close();
                _conexionBD.Close();

                return _cuentaActiva;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Funcion para activar la cuenta de un cliente
        /// </summary>
        /// <param name="idCuenta">El id de la cuenta del cliente</param>
        /// <returns>Retorna true en caso de que la cuenta se haya activado correctamente</returns>
        public bool activarCuentaCliente(string idCuenta)
        {
            try
            {
                // hay que hacer un update sobre la tabla cuentas y poner columna cuentaActiva a true
                SqlConnection conexionBD = new SqlConnection(this.CadenaConexionSever);
                conexionBD.Open();
                SqlCommand updateCuenta = new SqlCommand("update dbo.Cuentas set CuentaActivada=true where IdCuenta=@id", conexionBD);
                updateCuenta.Parameters.AddWithValue("@id", idCuenta);

                int filasModificadas = updateCuenta.ExecuteNonQuery();

                // si filas modificadas es 1 me retornas true, y si no, me retornas false
                return filasModificadas == 1 ? true : false;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Funcion para operar con las direcciones
        /// </summary>
        /// <param name="nuevaDir">Direccion con la que operar en caso de insertar o modificar</param>
        /// <param name="idCliente">El id del cliente</param>
        /// <param name="operacion">La operacion que queremos realizar</param>
        /// <returns>Retorna true en caso de que la operacion sea satisfactoria</returns>
        public bool operarDireccion(Direccion nuevaDir, String idCliente, String operacion)
        {
            try
            {
                SqlConnection conexion = new SqlConnection(this.CadenaConexionSever);
                conexion.Open();

                SqlCommand comandoEjecutar = new SqlCommand();
                comandoEjecutar.Connection = conexion;

                switch (operacion)
                {
                    case "crear":
                        // En el caso de crear, insertaremos una nueva Direccion con los datos de "nuevaDir"
                        comandoEjecutar.CommandText = "insert into dbo.Direcciones values (@id, @calle, @cp, @cpro, @cmun, @esppal, @esfact, @idd, @pais)";
                        comandoEjecutar.Parameters.AddWithValue("@id", idCliente);
                        comandoEjecutar.Parameters.AddWithValue("@calle", nuevaDir.Calle);
                        comandoEjecutar.Parameters.AddWithValue("@cp", nuevaDir.CP);
                        comandoEjecutar.Parameters.AddWithValue("@cpro", nuevaDir.ProvDirec.CPRO + "-" + nuevaDir.ProvDirec.PRO);
                        comandoEjecutar.Parameters.AddWithValue("@cmun", nuevaDir.MuniDirecc.CMUM + "-" + nuevaDir.MuniDirecc.DMUN50);
                        comandoEjecutar.Parameters.AddWithValue("@esppal", nuevaDir.EsPrincipal);
                        comandoEjecutar.Parameters.AddWithValue("@esfact", nuevaDir.EsFacturacion);
                        comandoEjecutar.Parameters.AddWithValue("@idd", nuevaDir.IdDireccion);
                        comandoEjecutar.Parameters.AddWithValue("@pais", nuevaDir.Pais);

                        break;

                    case "modificar":
                        // Modificamos la direccion que corresponde al idDireccion
                        comandoEjecutar.CommandText = "update dbo.Direcciones set Calle=@calle,CP=@cp,Provincia=@cpro,Municipio=@cmun,EsPrincipal=0,EsFacturacion=0,Pais=@pais where IdDireccion=@idd";
                        comandoEjecutar.Parameters.AddWithValue("@calle", nuevaDir.Calle);
                        comandoEjecutar.Parameters.AddWithValue("@cp", nuevaDir.CP);
                        comandoEjecutar.Parameters.AddWithValue("@cpro", nuevaDir.ProvDirec.CPRO + "-" + nuevaDir.ProvDirec.PRO);
                        comandoEjecutar.Parameters.AddWithValue("@cmun", nuevaDir.MuniDirecc.CMUM + "-" + nuevaDir.MuniDirecc.DMUN50);
                        comandoEjecutar.Parameters.AddWithValue("@idd", nuevaDir.IdDireccion);
                        comandoEjecutar.Parameters.AddWithValue("@pais", nuevaDir.Pais);
                        break;

                    case "borrar":
                        // Eliminamos la direccion correspondiente al idDireccion
                        comandoEjecutar.CommandText = "delete from dbo.Direcciones where IdDireccion=@idd";
                        comandoEjecutar.Parameters.AddWithValue("@idd", nuevaDir.IdDireccion);
                        break;

                }


                int numfilasInsert = comandoEjecutar.ExecuteNonQuery();

                return numfilasInsert == 1;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Funcion para añadir la imagen Avatar del Cliente a la base de datos
        /// </summary>
        /// <param name="nombreFichero"></param>
        /// <param name="contenidoBASE64"></param>
        /// <param name="idCuenta"></param>
        /// <returns>Retorna true en caso de que la imagen se haya almacenado de forma correcta</returns>
        public bool updateCuentaSubirImagen(string nombreFichero, string contenidoBASE64, String idCuenta)
        {
            try
            {
                SqlConnection conexion = new SqlConnection(this.CadenaConexionSever);
                conexion.Open();

                SqlCommand updateCuenta = new SqlCommand("update dbo.Cuentas set ImagenAvatar=@img, ImagenAvatarBASE64=@img64 where IdCuenta=@id", conexion);
                updateCuenta.Parameters.AddWithValue("@id", idCuenta);
                updateCuenta.Parameters.AddWithValue("@img", nombreFichero);
                updateCuenta.Parameters.AddWithValue("@img64", contenidoBASE64);

                int numFilasInsertadas = updateCuenta.ExecuteNonQuery();
                return numFilasInsertadas == 1;
            }
            catch (Exception ex)
            {

                return false;
            }
        }

        /// <summary>
        /// Funcion para actualizar los datos del cliente
        /// </summary>
        /// <param name="datosCliente"></param>
        /// <param name="newPassword"></param>
        /// <returns>Retorna true en caso de que todo funcione correctamente</returns>
        public bool updateDatosCliente(Cliente datosCliente, String newPassword, String oldLogin)
        {
            if (datosCliente.Descripcion == null) datosCliente.Descripcion = "";
            // Preguntar al profe por como funcionan los retornos de esta funcion
            try
            {
                SqlConnection conexion = new SqlConnection(this.CadenaConexionSever);
                conexion.Open();

                SqlCommand updateCliente = new SqlCommand("update dbo.Clientes set Nombre=@nom, Apellidos=@apell, Telefono=@tel, FechaNacimiento=@fecna, Genero=@gen, Descripcion=@desc where IdCliente=@idc", conexion);
                updateCliente.Parameters.AddWithValue("@nom", datosCliente.Nombre);
                updateCliente.Parameters.AddWithValue("@apell", datosCliente.Apellidos);
                updateCliente.Parameters.AddWithValue("@tel", datosCliente.Telefono);
                updateCliente.Parameters.AddWithValue("@fecna", datosCliente.FechaNacimiento);
                // si la descripcion es null, dara una excepcion
                updateCliente.Parameters.AddWithValue("@desc", datosCliente.Descripcion);
                updateCliente.Parameters.AddWithValue("@gen", datosCliente.Genero);
                updateCliente.Parameters.AddWithValue("@idc", datosCliente.IdCliente);

                int numFilasUpdateCliente = updateCliente.ExecuteNonQuery();

                Boolean resultadoUpdateCuentasPassword = false;
                Boolean resultadoUpdateCuentasLogin = false;

                if (numFilasUpdateCliente == 1)
                {
                    // Mofificamos las Password si esta rellena en el Form
                    if (!String.IsNullOrEmpty(newPassword))
                    {
                        SqlCommand updateCuenta = new SqlCommand("update dbo.Cuentas set Password=@pass where IdCuenta=@idc", conexion);

                        String hashpass = BCrypt.Net.BCrypt.HashPassword(newPassword);
                        updateCuenta.Parameters.AddWithValue("@pass", hashpass);
                        updateCuenta.Parameters.AddWithValue("@idc", datosCliente.CuentaCliente.IdCuenta);

                        int numFilasUpdateCuentaPass = updateCuenta.ExecuteNonQuery();

                        resultadoUpdateCuentasPassword = numFilasUpdateCuentaPass == 1;
                    } else { resultadoUpdateCuentasPassword = true; }

                    // Modifico el login si ha variado con respecto al viejo
                    if (datosCliente.CuentaCliente.Login != oldLogin)
                    {
                        SqlCommand updateCuentaLogin = new SqlCommand("update dbo.Cuentas set Login=@log where IdCuenta=@idc", conexion);
                        updateCuentaLogin.Parameters.AddWithValue("@log", datosCliente.CuentaCliente.Login);
                        updateCuentaLogin.Parameters.AddWithValue("@idc", datosCliente.CuentaCliente.IdCuenta);

                        int numFilasUpdateCuentaLogin = updateCuentaLogin.ExecuteNonQuery();

                        resultadoUpdateCuentasLogin = numFilasUpdateCuentaLogin == 1;
                    } else { resultadoUpdateCuentasLogin= true; }

                    return resultadoUpdateCuentasPassword && resultadoUpdateCuentasLogin;
                    
                }
                else { return false; }
            }
            catch (Exception)
            {

                return false;
            }
        }

        #endregion

        #region ... metodos Tienda ...

        public List<Categoria> devolverCategoriasRaiz()
        {
            try
            {
                List<Categoria> listaCategorias = new List<Categoria>();

                SqlConnection conexion = new SqlConnection(this.CadenaConexionSever);
                conexion.Open();

                SqlCommand selectCatRaiz = new SqlCommand("select * from dbo.Categorias where IdCategoria like '_' or IdCategoria like '__'", conexion);
                SqlDataReader cursor = selectCatRaiz.ExecuteReader();

                if (cursor.HasRows)
                {
                    while (cursor.Read())
                    {
                        listaCategorias.Add(new Categoria(cursor["IdCategoria"].ToString(), cursor["NombreCategoria"].ToString()));
                    }

                    cursor.Close();
                    cursor.Dispose();

                    return listaCategorias;
                } else { return null; }

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<Libro> recuperaLibros(String idCategoria)
        {
            try
            {
                List<Libro> listaLibros= new List<Libro>();
                SqlConnection conexion = new SqlConnection(this.CadenaConexionSever);
                conexion.Open();

                SqlCommand selectLibro = new SqlCommand("select * from dbo.Libros where IdCategoria=@idc", conexion);
                selectLibro.Parameters.AddWithValue("@idc", idCategoria);

                SqlDataReader cursor = selectLibro.ExecuteReader();
                if (cursor.HasRows)
                {
                    while (cursor.Read())
                    {
                        Libro libro = new Libro
                        {
                            IdCategoria = cursor["IdCategoria"].ToString(),
                            ImagenLibro = cursor["ImagenLibro"].ToString(),
                            ImagenLibroBase64 = cursor["ImagenLibroBASE64"].ToString(),
                            Titulo = cursor["Titulo"].ToString(),
                            Editorial = cursor["Editorial"].ToString(),
                            Autores = cursor["Autores"].ToString(),
                            Edicion = cursor["Edicion"].ToString(),
                            NumeroPaginas = System.Convert.ToInt16(cursor["NumeroPaginas"]),
                            Dimensiones = cursor["Dimensiones"].ToString(),
                            Idioma = cursor["Idioma"].ToString(),
                            ISBN10 = cursor["ISBN10"].ToString(),
                            ISBN13 = cursor["ISBN13"].ToString(),
                            Resumen = cursor["Resumen"].ToString(),
                            Precio = System.Convert.ToInt16(cursor["Precio"])

                        };
                        listaLibros.Add(libro);
                    }

                    cursor.Close();
                    cursor.Dispose();
                    return listaLibros;
                } else { return null; }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        #endregion


        #endregion


    }
}
