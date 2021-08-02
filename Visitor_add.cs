using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Checking
{
    public partial class Visitor_add : Form
    {
        SqlConnection SqlConnection = null;
        SqlDataAdapter sqlDataAdapter = null;
        SqlCommand cmd;
        public Visitor_add()
        {
            InitializeComponent();
        }

        private void Visitor_add_Load(object sender, EventArgs e)
        {
            //Подключение к БД
            SqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            SqlConnection.Open();

            //Запрос на поиск всех департаментов
            sqlDataAdapter = new SqlDataAdapter($"SELECT * FROM departments", SqlConnection);
            DataTable table = new DataTable();
            sqlDataAdapter.Fill(table);

            //Заполнение выпадающего списка департаментами из запроса выше
            comboBox1.DataSource = table;
            comboBox1.DisplayMember = "department_name";
        }
        //Кнопка добавления посетителя
        private void button1_Click(object sender, EventArgs e)
        {
            //Проверка на заполнение всех строк
            if (textBox1.Text != "" && textBox2.Text != "" && textBox3.Text != "" && textBox4.Text != "" && textBox5.Text != "" && textBox9.Text != "" && comboBox1.Text != "")
            {

                string pasport_cod = Convert.ToString(textBox3.Text);
                string pasport_number = Convert.ToString(textBox4.Text);
                string department_name = Convert.ToString(comboBox1.Text);
                string visitor_name = Convert.ToString(textBox1.Text);
                string visitor_phone = Convert.ToString(textBox5.Text);
                string visitor_surname = Convert.ToString(textBox2.Text);
                string visitor_email = Convert.ToString(textBox9.Text);

                int search;
                DataSet info2 = new DataSet();
                //Запрос на поиск посетителей по заданным паспортным данным
                sqlDataAdapter = new SqlDataAdapter($"SELECT pasport_cod, pasport_number FROM pasports WHERE pasport_cod = {pasport_cod} AND pasport_number = {pasport_number};", SqlConnection);
                sqlDataAdapter.Fill(info2, "pasport");
                search = info2.Tables["pasport"].Rows.Count;

                //Если пользователей с такими паспортными данными нет
                if (search == 0)
                {
                    //Передача введенных данных в процедуру добавления посетителя
                    cmd = new SqlCommand("visitor_add", SqlConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.Add(new SqlParameter("@pasport_cod", pasport_cod));
                    cmd.Parameters.Add(new SqlParameter("@pasport_number", pasport_number));
                    cmd.Parameters.Add(new SqlParameter("@department_name", department_name));
                    cmd.Parameters.Add(new SqlParameter("@visitor_name", visitor_name));
                    cmd.Parameters.Add(new SqlParameter("@visitor_surname", visitor_surname));
                    cmd.Parameters.Add(new SqlParameter("@visitor_phone", visitor_phone));
                    cmd.Parameters.Add(new SqlParameter("@visitor_email", visitor_email));
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Данные приняты", "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Введите уникальные паспортные данные!!!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }


            }
            else
            {
                MessageBox.Show("Заполните все строки!!!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.Close();
        }
        //Разрешение на ввод в текстовое поле только букв
        void For_letter(KeyPressEventArgs e)
        {
            char number = e.KeyChar;

            if (!char.IsLetter(number) && number != 8)
            {
                e.Handled = true;
            }
        }
        //Разрешение на ввод в текстовое поле только цифр
        void For_number(KeyPressEventArgs e)
        {
            char number = e.KeyChar;

            if (!char.IsDigit(number) && number != 8)
            {
                e.Handled = true;
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            For_letter(e);
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            For_letter(e);
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            For_number(e);
        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            For_number(e);
        }
        //Разрешение на ввод в текстовое поле только цифр и знака +
        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;

            if (!char.IsDigit(number) && number != 8 && number != 43)
            {
                e.Handled = true;
            }
        }
    }
}
