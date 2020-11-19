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
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

namespace WPF_ConexionGestionPedidos_BBDD
{
    public partial class Actualiza : Window
    {

        SqlConnection miConexionSql;
        private int z1;

        public Actualiza(int elId)
        {
            InitializeComponent();

            z1 = elId;

            string miConexion = ConfigurationManager.ConnectionStrings["WPF_ConexionGestionPedidos_BBDD.Properties.Settings.GestionPedidosConnectionString"].ConnectionString;
            miConexionSql = new SqlConnection(miConexion);
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnActualiza_Click(object sender, RoutedEventArgs e)
        {
            string consultaSql = "UPDATE CLIENTE SET nombre=@nombre WHERE Id=" + z1;
            SqlCommand miSqlCommand = new SqlCommand(consultaSql, miConexionSql);

            miConexionSql.Open();
            miSqlCommand.Parameters.AddWithValue("@nombre", cuadroActualiza.Text);
            miSqlCommand.ExecuteNonQuery();
            miConexionSql.Close();
            MessageBox.Show("Se ha actualizado correctamente el dato!");
            this.Close();
        }
    }



}
