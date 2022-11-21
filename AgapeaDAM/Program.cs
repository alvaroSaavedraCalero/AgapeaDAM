using AgapeaDAM.Models;
using AgapeaDAM.Models.Interfaces;

var builder = WebApplication.CreateBuilder(args);


//============================================================================================
// 1º seccion configuracion contenedor de servicios de modulo de inyeccion de dependencias(DI)
//Add services to the container.
//=============================================================================================
builder.Services.AddScoped<IBDAccess, SqlServerBDAccess>(); //<--- con los metodos .AddScoped<interface, clase>   .AddTransient<interface, clase>   .AddSingleton<interface,clase>
                                                            // añades un objeto de esa clase q implemente esa interface al modulo de inyeccion de dependencias del servidor
                                                            //para q pueda ser inyectado como un servicio cuando se solicite por un controlador, vista
                                                            // .AddTransient <---- cada vez q se solicita un servicio, se crea de nuevo (no se recupera)
                                                            // .AddScoped <-------se crea un servicio por cliente y se reutiliza
                                                            // .AddSingleton <----se crea un servicio para toda la aplicacion y todos los clientes

builder.Services.AddSession((SessionOptions configure) =>
{
    configure.Cookie.MaxAge = new TimeSpan(1, 0, 0); // La cookie durara 1 hora
    configure.Cookie.IsEssential = true; // para que la cookie sea obligatorio (por defecto esta en true)
    configure.Cookie.HttpOnly = true; // solo admite la cookie si la manda el navegador, por si acaso la cambian por javascript
    

}); // <----- 1ºconfiguracion servicio estado de sesion, siguiente paso: habilitar el middleware para recuperar el tokenb de sesion metido en la cookie

builder.Services.AddScoped<IClienteEmail, EmailClienteMailJet>();
builder.Services.AddControllersWithViews();

var app = builder.Build();


//=============================================================================================================
// 2º seccion configuracion de los modulos middleware del servidor web q van a procesar la pet.http del cliente
//  middle-1 ===> middle-2 ===> middle-3 ...
//   |----------------|------------|----- PIPELINE
// Configure the HTTP request pipeline.
//=============================================================================================================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection(); //1º modulo middleware
app.UseStaticFiles(); // 2º modulo middleware

app.UseSession(); // 3º modulo middleware, para recuperar la cookie con el token de sesion y habilitar diccionario estado de sesion asociado

app.UseRouting(); //4º modulo middleware <------------------IMPRESCINDIBLE POR ENCARGARSE DEL ENRUTAMIENTO
// el modulo de enrutamiento extrae de la pet. HTTP-REQUEST del cliente la url, la divide por segmentos y busca
// dentro de una lista de objetos ROUTE (EndPoint) q tiene definidos, el q mas se aproxime por patron. Una vez seleccionado
//ese objeto Route va a indicar q Controlador tiene q cargar en memoria y q metodo del mismo tiene q ejecutar


app.UseAuthorization(); //5º modulo middleware

//------------------------------- lista de defincion de endpoints para el modulo de enrutamiento ------------
app.MapControllerRoute(
    name: "default", //<----clave q id al endPoint en la lista de rutas del modulo de enrutamiento
    pattern: "{controller=Tienda}/{action=RecuperaLibros}/{id?}"); //<-----patron de busqueda contra la url del cliente
                                                        //----------------  -------------  -----
                                                        //   segmento-1        segmento-2   segmento-3
                                                        // cliente en navegador:   https://localhost:Xxxx/Cliente/Registro <===== si lo cumple segmento-1=Cliente, segmento-2=Registro
                                                        // cliente en navegador:   https://localhost:Xxxx/                 <===== si lo cumple segmento-1=Home, segmento-2=Index
                                                        // cliente en navegador:   https://localhost:Xxxx/Tienda/MostrarLibro/12331412  <===== si lo cumple segmento-1=Tienda, segmento-2=MostrarLibro, segemento-3, id=1233412

//-----------Ruta de 4 segmentos para el panel del cliente, nos facilita la tarea a la hora de mostrar las opciones en el LAYOUT ---
app.MapControllerRoute(
    name: "panelCliente",
    pattern: "{controller=Cliente}/Panel/{action=InicioPanel}/{id?}" // <-- patron de busqueda contra la url del cliente
    );

//-----------------------------------------------------------------------------------------------------------
app.Run();
