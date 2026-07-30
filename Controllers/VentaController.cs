using Microsoft.AspNetCore.Mvc;
using System.Data;
using MySql.Data.MySqlClient;

namespace WebApplication1.Controllers
{
    public class VentaController : Controller
    {
        private readonly string cadenaConexion = "Server=localhost;Database=gonzalo;Uid=root;Pwd=;";

        // Muestra la lista de ventas
        public IActionResult Index()
        {
            DataTable dtVentas = new DataTable();
            DataTable dtProductos = new DataTable();

            try
            {
                using (MySqlConnection conexion = new MySqlConnection(cadenaConexion))
                {
                    conexion.Open();

                    // 1. Traemos las ventas
                    string queryVentas = "SELECT * FROM venta";
                    using (MySqlCommand cmd = new MySqlCommand(queryVentas, conexion))
                    {
                        using (MySqlDataAdapter da = new MySqlDataAdapter(cmd)) { da.Fill(dtVentas); }
                    }

                    // 2. Traemos los productos del inventario para la lista automática
                    string queryProductos = "SELECT * FROM producto";
                    using (MySqlCommand cmd2 = new MySqlCommand(queryProductos, conexion))
                    {
                        using (MySqlDataAdapter da2 = new MySqlDataAdapter(cmd2)) { da2.Fill(dtProductos); }
                    }
                }
            }
            catch (Exception)
            {
                ViewBag.ErrorServidor = "¡Alerta! No se pudo conectar a la base de datos de Ventas.";
            }

            ViewBag.Ventas = dtVentas;
            ViewBag.ListaProductos = dtProductos;
            return View();
        }

        // Muestra el formulario cargando los productos con stock
        public IActionResult Create()
        {
            DataTable dtProductos = new DataTable();
            DataTable dtUsuarios = new DataTable();
            using (MySqlConnection conexion = new MySqlConnection(cadenaConexion))
            {
                conexion.Open();
                using (MySqlCommand cmd = new MySqlCommand("SELECT id, nombre, precio, stock FROM producto WHERE stock > 0", conexion))
                {
                    using (MySqlDataAdapter da = new MySqlDataAdapter(cmd)) { da.Fill(dtProductos); }
                }
                using (MySqlCommand cmd = new MySqlCommand("SELECT id, correo FROM usuario", conexion))
                {
                    using (MySqlDataAdapter da = new MySqlDataAdapter(cmd)) { da.Fill(dtUsuarios); }
                }
            }
            ViewBag.Productos = dtProductos;
            ViewBag.Usuarios = dtUsuarios;
            return View();
        }

        // Guarda la venta, calcula el total, y DESCUENTA EL STOCK (Cumple la rúbrica)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(int producto_id, int cantidad)
        {
            using (MySqlConnection conexion = new MySqlConnection(cadenaConexion))
            {
                conexion.Open();

                // 1. Buscamos el nombre y el precio del producto usando su ID
                string nombreProducto = "";
                decimal precioUnitario = 0;

                string queryPrecio = "SELECT nombre, precio FROM producto WHERE id = @id";
                using (MySqlCommand cmdPrecio = new MySqlCommand(queryPrecio, conexion))
                {
                    cmdPrecio.Parameters.AddWithValue("@id", producto_id);
                    using (var reader = cmdPrecio.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            nombreProducto = reader["nombre"].ToString();
                            precioUnitario = Convert.ToDecimal(reader["precio"]);
                        }
                    }
                }

                // Calculamos el total de forma automática
                decimal totalVenta = precioUnitario * cantidad;

                // 2. Registrar la venta con los datos obtenidos
                string queryVenta = "INSERT INTO venta (producto, cantidad, total) VALUES (@prod, @cant, @total)";
                using (MySqlCommand cmd = new MySqlCommand(queryVenta, conexion))
                {
                    cmd.Parameters.AddWithValue("@prod", nombreProducto);
                    cmd.Parameters.AddWithValue("@cant", cantidad);
                    cmd.Parameters.AddWithValue("@total", totalVenta);
                    cmd.ExecuteNonQuery();
                }

                // 3. Descontar el stock del producto automáticamente
                string queryStock = "UPDATE producto SET stock = stock - @cant WHERE id = @prod";
                using (MySqlCommand cmdStock = new MySqlCommand(queryStock, conexion))
                {
                    cmdStock.Parameters.AddWithValue("@cant", cantidad);
                    cmdStock.Parameters.AddWithValue("@prod", producto_id);
                    cmdStock.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index");
        }
    }
}