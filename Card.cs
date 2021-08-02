using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Checking
{
    public partial class Card : Form
    {
        SqlConnection SqlConnection = null;
        SqlDataAdapter sqlDataAdapter = null;
        string pasport_cod;
        string pasport_number;
        //Получение переданных паспортных даннных
        public Card(string pasport_cod, string pasport_number)
        {
            this.pasport_cod = pasport_cod;
            this.pasport_number = pasport_number;
        
            InitializeComponent();
        }

        //Вывод на карту данных о посетителе, номере карты, департаменту и уровню допуска в него 
        private void Card_Load(object sender, EventArgs e)
        {
            DataSet info = new DataSet();
            SqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            SqlConnection.Open();

            sqlDataAdapter = new SqlDataAdapter($"SELECT card_number, visitor_name, visitor_surname, department_name, department_lvl FROM departments INNER JOIN visitors ON departments.department_id = visitors.department_id INNER JOIN cards ON visitors.visitor_id = cards.visitor_id INNER JOIN pasports ON visitors.pasport_id = pasports.pasport_id WHERE pasports.pasport_cod = {pasport_cod} AND pasports.pasport_number = {pasport_number}; ", SqlConnection);
            sqlDataAdapter.Fill(info, "card");


            object[] obj = info.Tables["card"].Rows[0].ItemArray;


            label10.Text = obj[0].ToString();
            label6.Text = obj[1].ToString();
            label7.Text = obj[2].ToString();
            label8.Text = obj[3].ToString();
            label9.Text = obj[4].ToString();
        }
    }
}
