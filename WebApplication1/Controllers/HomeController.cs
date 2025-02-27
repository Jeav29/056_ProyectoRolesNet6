using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplication1.Models;

//1.- AÑADIR LA AUTHORIZACION
using Microsoft.AspNetCore.Authorization;

namespace WebApplication1.Controllers
{
    //2.- AÑADIR LA AUTHORIZACION
    //[Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        private List<MenuItem> GetMenuItems()
        {
            // Define los elementos del menú
            return new List<MenuItem>
            {
                new MenuItem { Name = "Home", Controller = "Home", Action = "Index", Roles = "Administrador,Supervisor,Empleado,Jefe" },
                new MenuItem { Name = "Ventas", Controller = "Home", Action = "Ventas", Roles = "Administrador,Empleado" },
                new MenuItem { Name = "Compras", Controller = "Home", Action = "Compras", Roles = "Administrador,Supervisor" },
                new MenuItem { Name = "Clientes", Controller = "Home", Action = "Clientes", Roles = "Administrador,Supervisor" },
                new MenuItem { Name = "Privacy", Controller = "Home", Action = "Privacy", Roles = "" },
                new MenuItem { Name = "Salir", Controller = "Acceso", Action = "Salir", Roles = "" }
            };
        }

        private List<MenuItem> FilterMenuItemsByRole(List<MenuItem> menuItems)
        {
            // Simular obtención de usuario actual (esto puede variar dependiendo de tu implementación de autenticación)
            string currentUserId = User.Identity?.Name; // Obtiene el email del usuario autenticado

            // Instanciar la clase DA_Usuario
            var userDataAccess = new WebApplication1.Data.DA_Usuario();
            var currentUser = userDataAccess.ListaUsuario().FirstOrDefault(u => u.Nombre == currentUserId);

            // Si no se encuentra el usuario o no tiene roles, devolver una lista vacía
            if (currentUser == null || currentUser.Roles == null)
            {
                return new List<MenuItem>();
            }

            // Filtrar los menús según los roles del usuario
            var filteredItems = new List<MenuItem>();
            foreach (var item in menuItems)
            {
                if (string.IsNullOrEmpty(item.Roles) || item.Roles.Split(',').Any(role => currentUser.Roles.Contains(role)))
                {
                    filteredItems.Add(item);
                }
            }
            return filteredItems;
        }


        [Authorize(Roles = "Administrador,Supervisor,Empleado,Jefe")]
        public IActionResult Index()
        {
            // Filtrar los menús por roles
            var menuItems = GetMenuItems();
            var filteredMenuItems = FilterMenuItemsByRole(menuItems);

            // Pasar los menús a la vista
            ViewData["MenuItems"] = filteredMenuItems;
            return View();
        }

        [Authorize(Roles = "Administrador,Empleado")]
        public IActionResult Ventas()
        {
            return View();
        }

        [Authorize(Roles = "Administrador,Supervisor")]
        public IActionResult Compras()
        {
            return View();
        }

        [Authorize(Roles = "Administrador,Supervisor")]
        public IActionResult Clientes()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}