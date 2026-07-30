using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;

namespace WebApplication1.Controllers
{
    public class LoginController : Controller
    {
        private readonly string cadenaConexion = "Server=localhost;Database=gonzalo;Uid=root;Pwd=;";

        public IActionResult Index() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(string correo, string password)
        {
            using (MySqlConnection conexion = new MySqlConnection(cadenaConexion))
            {
                conexion.Open();
                string query = "SELECT * FROM usuario WHERE correo = @correo AND password = @password";

                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@correo", correo);
                    cmd.Parameters.AddWithValue("@password", password);

                    DataTable dt = new DataTable();
                    using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                    if (dt.Rows.Count > 0)
                    {
                        // ¡Inicio de sesión exitoso!
                        HttpContext.Session.SetString("UsuarioLogueado", correo);
                        return RedirectToAction("Index", "Producto");
                    }
                }
            }

            ViewBag.Error = "Correo o contraseña incorrectos.";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}