using Microsoft.AspNetCore.Mvc;
using System.Data;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Identity;

namespace WebApplication1.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly string cadenaConexion = "Server=localhost;Database=gonzalo;Uid=root;Pwd=;";

        public IActionResult Index()
        {
            DataTable dt = new DataTable();

            try
            {
                using (MySqlConnection conexion = new MySqlConnection(cadenaConexion))
                {
                    conexion.Open();
                    string query = "SELECT * FROM usuario";
                    using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                    {
                        using (MySqlDataAdapter da = new MySqlDataAdapter(cmd)) { da.Fill(dt); }
                    }
                }
            }
            catch (Exception)
            {
                ViewBag.ErrorServidor = "¡Alerta! No hay conexión con la base de datos de Usuarios.";
            }

            ViewBag.Usuarios = dt;
            return View();
        }

        public IActionResult Create() => View("~/Views/Usuario/Create.cshtml");

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(string correo, string password)
        {
            PasswordHasher<string> passwordHasher = new PasswordHasher<string>();
            string passwordHash = passwordHasher.HashPassword("", password ?? "");
            using (MySqlConnection conexion = new MySqlConnection(cadenaConexion))
            {
                conexion.Open();
                string query = "INSERT INTO usuario (correo, password) VALUES (@correo, @password)";
                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@correo", correo);
                    cmd.Parameters.AddWithValue("@password", passwordHash); // Guardamos el hash cifrado
                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            DataTable dt = new DataTable();
            using (MySqlConnection conexion = new MySqlConnection(cadenaConexion))
            {
                conexion.Open();
                string query = "SELECT * FROM usuario WHERE id = @id";
                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (MySqlDataAdapter da = new MySqlDataAdapter(cmd)) { da.Fill(dt); }
                }
            }
            if (dt.Rows.Count > 0)
            {
                ViewBag.Usuario = dt.Rows[0];
                return View("~/Views/Usuario/Edit.cshtml");
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, string correo, string password)
        {
            // Si el usuario escribe una contraseña nueva al editar, la volvemos a encriptar
            PasswordHasher<string> passwordHasher = new PasswordHasher<string>();
            string passwordHash = passwordHasher.HashPassword(null, password);

            using (MySqlConnection conexion = new MySqlConnection(cadenaConexion))
            {
                conexion.Open();
                string query = "UPDATE usuario SET correo = @correo, password = @password WHERE id = @id";
                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@correo", correo);
                    cmd.Parameters.AddWithValue("@password", passwordHash);
                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            using (MySqlConnection conexion = new MySqlConnection(cadenaConexion))
            {
                conexion.Open();
                string query = "DELETE FROM usuario WHERE id = @id";
                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index");
        }
    }
}