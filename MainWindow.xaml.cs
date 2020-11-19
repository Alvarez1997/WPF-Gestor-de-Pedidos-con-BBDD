using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace WPF_ConexionGestionPedidos_BBDD
{
    public partial class MainWindow : Window
    {

        SqlConnection miConexionSql;

        public MainWindow()
        {
            InitializeComponent();
            string miConexion = ConfigurationManager.ConnectionStrings["WPF_ConexionGestionPedidos_BBDD.Properties.Settings.GestionPedidosConnectionString"].ConnectionString;
            
            miConexionSql = new SqlConnection(miConexion); //le paso por parametro una cadena de conexion (miConexion)

            MuestraClientes(); //invocar el metodo para que el programa lo ejecute

            MuestraTodosPedidos();
        }

        private void MuestraClientes() //metodo void por que no va a devolver nada
        {
            try
            {
                string consulta = "SELECT * FROM CLIENTE";
                SqlDataAdapter miAdaptadorSql = new SqlDataAdapter(consulta, miConexionSql); //es decirle: utilizando miConexionSql ejecuta mi consulta

                using (miAdaptadorSql) //almacenar los datos en un data table
                {
                    DataTable clientesTabla = new DataTable();
                    miAdaptadorSql.Fill(clientesTabla); //con esto llamo TODOS los datos dentro de la tabla cliente
                    listaClientes.DisplayMemberPath = "nombre"; //con esto le digo que solo muestre el campo nombre de la tabla cliente
                    listaClientes.SelectedValuePath = "Id"; //especificarle cual es el campo clave
                    listaClientes.ItemsSource = clientesTabla.DefaultView; //especificarle cual es el origen de los datos
                }
            }catch(Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void MuestraPedidos() //metodo
        {
            try
            {
                string consulta1 = "SELECT * FROM PEDIDO P INNER JOIN CLIENTE C ON C.ID=P.cCLIENTE" +
                    " WHERE C.ID=@ClienteId"; //consulta parametrica (consulta con parametros)
                SqlCommand sqlComando = new SqlCommand(consulta1, miConexionSql); //este codigo nos permite ejecutar una instruccion sql con parametro  
                SqlDataAdapter miAdaptadorSql = new SqlDataAdapter(sqlComando); //es decirle: utilizando miConexionSql ejecuta mi consulta

                using (miAdaptadorSql) //almacenar los datos en un data table
                {
                    sqlComando.Parameters.AddWithValue("@ClienteId", listaClientes.SelectedValue); //almacena el cliente que el usuario sleccione y ejecuta el parametro

                    DataTable pedidosTabla = new DataTable();
                    miAdaptadorSql.Fill(pedidosTabla); //con esto llamo TODOS los datos dentro de la tabla cliente
                    pedidosCliente.DisplayMemberPath = "fechaPedido"; //con esto le digo que solo muestre el campo nombre de la tabla cliente
                    pedidosCliente.SelectedValuePath = "Id"; //especificarle cual es el campo clave / campo clave
                    pedidosCliente.ItemsSource = pedidosTabla.DefaultView; //especificarle cual es el origen de los datos
                }
            }catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void MuestraTodosPedidos()
        {
            try
            {
                string consulta = "SELECT *, CONCAT (cCLIENTE, ' ', fechaPedido, ' ', formaPago) AS INFOCOMPLETA FROM PEDIDO"; //consulta de campo nuevo calculado
                SqlDataAdapter miAdaptadorSql = new SqlDataAdapter(consulta, miConexionSql);

                using (miAdaptadorSql)
                {
                    DataTable pedidosTabla = new DataTable(); //creacion de algo asi como una tabla virtual 
                    miAdaptadorSql.Fill(pedidosTabla); //con esto relleno con datos la tabla virtual
                    todosPedidos.DisplayMemberPath = "INFOCOMPLETA"; //muestra todos los datos de la tabla por que estan contatenados en un solo campo creado por mi que no esta en la tabla INFCOMP
                    todosPedidos.SelectedValuePath = "Id";
                    todosPedidos.ItemsSource = pedidosTabla.DefaultView;
                }
            }catch(Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show(todosPedidos.SelectedValue.ToString());
            string consultaSql = "DELETE FROM PEDIDO WHERE ID=@PEDIDOID"; //consulta parametrica
            SqlCommand miComandoSql2 = new SqlCommand(consultaSql, miConexionSql);

            miConexionSql.Open();
            miComandoSql2.Parameters.AddWithValue("@PEDIDOID", todosPedidos.SelectedValue); //elimina el dato seleccionado de la tabla
            miComandoSql2.ExecuteNonQuery(); //Ejecuta comandos como las instrucciones INSERT, DELETE, UPDATE y SET, devuelve el número de filas afectadas (aqui las que quedan despues del delete)
            miConexionSql.Close();

            MuestraTodosPedidos(); //este metodo se llama para que al eliminar el dato refresque la tabla y asi mostrar la interfaz sin el dato eliminado

        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            string consultaSql = "INSERT INTO CLIENTE (nombre) VALUES (@newNombre)"; 
            SqlCommand miComandoSql2 = new SqlCommand(consultaSql, miConexionSql);

            miConexionSql.Open();
            miComandoSql2.Parameters.AddWithValue("@newNombre", insertaCliente.Text); 
            miComandoSql2.ExecuteNonQuery();
            miConexionSql.Close();

            MuestraClientes();
            insertaCliente.Text = "";
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            string consultaSql = "DELETE FROM CLIENTE WHERE ID=@CLIENTEID"; 
            SqlCommand miComandoSql2 = new SqlCommand(consultaSql, miConexionSql);

            miConexionSql.Open();
            miComandoSql2.Parameters.AddWithValue("@CLIENTEID", listaClientes.SelectedValue);
            miComandoSql2.ExecuteNonQuery();
            miConexionSql.Close();

            MuestraClientes();
        }

        private void listaClientes_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MuestraPedidos();
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            Actualiza ventanaActualizar = new Actualiza((int)listaClientes.SelectedValue); //instanciar el nuevo formulario (nueva ventana) y pasarle por parametro el Id del cliente
            

            try
            {
                string consulta = "SELECT nombre FROM CLIENTE WHERE ID=@ClId";
                SqlCommand miSqlCommand = new SqlCommand(consulta, miConexionSql);
                SqlDataAdapter miAdaptadorSql = new SqlDataAdapter(miSqlCommand); 

                using (miAdaptadorSql) //almacenar los datos en un data table
                {
                    miSqlCommand.Parameters.AddWithValue("@ClId", listaClientes.SelectedValue);
                    DataTable clientesTabla1 = new DataTable();
                    miAdaptadorSql.Fill(clientesTabla1);
                    ventanaActualizar.cuadroActualiza.Text = clientesTabla1.Rows[0]["nombre"].ToString();
                }
            }
            catch (Exception e2)
            {
                MessageBox.Show(e2.ToString());
            }

            ventanaActualizar.ShowDialog();
            MuestraClientes();
        }

      
    }
}
