using Microsoft.AspNetCore.Mvc;
using System.Data;
using MySql.Data.MySqlClient;

namespace WebApplication1.Controllers
{
    public class CategoriaController : Controller
    {
        private readonly string cadenaConexion = "Server=localhost;Database=gonzalo;Uid=root;Pwd=;";

        public IActionResult Index()
        {
            DataTable tablaCategorias = new DataTable();
            try
            {
                using (MySqlConnection conexion = new MySqlConnection(cadenaConexion))
                {
                    conexion.Open();
                    string query = "SELECT * FROM categoria";
                    using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                    {
                        using (MySqlDataAdapter adaptador = new MySqlDataAdapter(cmd))
                        {
                            adaptador.Fill(tablaCategorias);
                        }
                    }
                }
                ViewBag.Categorias = tablaCategorias;
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error BD: " + ex.Message;
            }
            return View();
        }

        public IActionResult Create() => View();

        [HttpPost]
        public IActionResult Create(string nombre, string descripcion)
        {
            using (MySqlConnection conexion = new MySqlConnection(cadenaConexion))
            {
                conexion.Open();
                string query = "INSERT INTO categoria (nombre, descripcion) VALUES (@nombre, @descripcion)";
                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@nombre", nombre);
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
                string query = "SELECT * FROM categoria WHERE id = @id";
                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (MySqlDataAdapter da = new MySqlDataAdapter(cmd)) { da.Fill(dt); }
                }
            }
            if (dt.Rows.Count > 0)
            {
                ViewBag.Categoria = dt.Rows[0];
                return View();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Edit(int id, string nombre, string descripcion)
        {
            using (MySqlConnection conexion = new MySqlConnection(cadenaConexion))
            {
                conexion.Open();
                string query = "UPDATE categoria SET nombre = @nombre, descripcion = @descripcion WHERE id = @id";
                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@nombre", nombre);
                    cmd.Parameters.AddWithValue("@descripcion", descripcion);
                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            using (MySqlConnection conexion = new MySqlConnection(cadenaConexion))
            {
                conexion.Open();
                string query = "DELETE FROM categoria WHERE id = @id";
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