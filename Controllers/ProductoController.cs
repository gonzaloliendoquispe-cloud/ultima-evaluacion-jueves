using Microsoft.AspNetCore.Mvc;
using System.Data;
using MySql.Data.MySqlClient;
using System; // Agregamos esto para reconocer la clase Exception

namespace WebApplication1.Controllers
{
    public class ProductoController : Controller
    {
        private readonly string cadenaConexion = "Server=localhost;Database=gonzalo;Uid=root;Pwd=;";

        public IActionResult Index(string categoriaBuscar)
        {
            DataTable dt = new DataTable();

            // Iniciamos el Try-Catch para evitar que la página se caiga si XAMPP está apagado
            try
            {
                using (MySqlConnection conexion = new MySqlConnection(cadenaConexion))
                {
                    conexion.Open();
                    string query = "SELECT * FROM producto";

                    // Filtra si el usuario selecciona una categoría en el buscador
                    if (!string.IsNullOrEmpty(categoriaBuscar) && categoriaBuscar != "Todas")
                    {
                        query += " WHERE descripcion = @cat";
                    }

                    using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                    {
                        if (!string.IsNullOrEmpty(categoriaBuscar) && categoriaBuscar != "Todas")
                        {
                            cmd.Parameters.AddWithValue("@cat", categoriaBuscar);
                        }
                        using (MySqlDataAdapter da = new MySqlDataAdapter(cmd)) { da.Fill(dt); }
                    }
                }
            }
            catch (Exception)
            {
                // Si hay un error (ej. XAMPP apagado), el código salta aquí en vez de caerse
                ViewBag.ErrorServidor = "¡Alerta! No se pudo conectar a la base de datos. Verifica que el servidor de MySQL en XAMPP esté encendido.";
            }

            ViewBag.Productos = dt;
            return View();
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken] // <--- ¡CANDADO ANTI-HACKERS PARA CREAR!
        public IActionResult Create(string nombre, decimal precio, int stock, string descripcion)
        {
            using (MySqlConnection conexion = new MySqlConnection(cadenaConexion))
            {
                conexion.Open();
                string query = "INSERT INTO producto (nombre, precio, stock, descripcion) VALUES (@nombre, @precio, @stock, @descripcion)";
                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@nombre", nombre);
                    cmd.Parameters.AddWithValue("@precio", precio);
                    cmd.Parameters.AddWithValue("@stock", stock);
                    cmd.Parameters.AddWithValue("@descripcion", descripcion);
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
                string query = "SELECT * FROM producto WHERE id = @id";
                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (MySqlDataAdapter da = new MySqlDataAdapter(cmd)) { da.Fill(dt); }
                }
            }
            if (dt.Rows.Count > 0)
            {
                ViewBag.Producto = dt.Rows[0];
                return View();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // <--- ¡CANDADO ANTI-HACKERS PARA EDITAR!
        public IActionResult Edit(int id, string nombre, decimal precio, int stock, string descripcion)
        {
            using (MySqlConnection conexion = new MySqlConnection(cadenaConexion))
            {
                conexion.Open();
                string query = "UPDATE producto SET nombre = @nombre, precio = @precio, stock = @stock, descripcion = @descripcion WHERE id = @id";
                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@nombre", nombre);
                    cmd.Parameters.AddWithValue("@precio", precio);
                    cmd.Parameters.AddWithValue("@stock", stock);
                    cmd.Parameters.AddWithValue("@descripcion", descripcion);
                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // <--- ¡CANDADO ANTI-HACKERS PARA ELIMINAR!
        public IActionResult Delete(int id)
        {
            using (MySqlConnection conexion = new MySqlConnection(cadenaConexion))
            {
                conexion.Open();
                string query = "DELETE FROM producto WHERE id = @id";
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