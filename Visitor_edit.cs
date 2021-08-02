using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Checking
{
    public partial class Visitor_edit : Form
    {
        readonly int visitor_id;
        SqlConnection SqlConnection = null;
        SqlDataAdapter sqlDataAdapter = null;

        //Получение ID посетителя
        public Visitor_edit(int visitor_id)
        {
            this.visitor_id = visitor_id;
            InitializeComponent();
        }

        private void Visitor_edit_Load(object sender, EventArgs e)
        {
            //Подключение к БД
            SqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            SqlConnection.Open();

            //Запрос на получнеие данных о посетителе
            sqlDataAdapter = new SqlDataAdapter($"SELECT visitor_name, visitor_surname, pasport_cod, pasport_number, visitor_phone, department_name, visitor_email FROM visitors INNER JOIN pasports ON pasports.pasport_id = visitors.pasport_id INNER JOIN departments ON departments.department_id = visitors.department_id WHERE visitors.visitor_id = {visitor_id}; ", SqlConnection);
            DataSet set = new DataSet();
            sqlDataAdapter.Fill(set, "visitor");

            object[] obj = set.Tables["visitor"].Rows[0].ItemArray;

            textBox1.Text = obj[0].ToString();
            textBox2.Text = obj[1].ToString();
            textBox3.Text = obj[2].ToString();
            textBox4.Text = obj[3].ToString();
            textBox5.Text = obj[4].ToString();
            textBox6.Text = obj[6].ToString();

            //Запрос на получение данных о департаментах
            sqlDataAdapter = new SqlDataAdapter("SELECT * FROM departments", SqlConnection);
            DataTable table = new DataTable();
            sqlDataAdapter.Fill(table);

            //Заполнение выпадающего списка названиями отделов из запроса
            comboBox1.DataSource = table;
            comboBox1.DisplayMember = "department_name";
            comboBox1.Text = obj[5].ToString();
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

        private void TextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            For_letter(e);
        }

        private void TextBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            For_letter(e);
        }

        private void TextBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            For_number(e);
        }

        private void TextBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            For_number(e);
        }

        private void TextBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            For_number(e);
        }

        //Кнопка редактирования посетителя
        private void Button8_Click(object sender, EventArgs e)
        {
            //Проверка на пустые строки
            if (textBox1.Text != "" && textBox2.Text != "" && textBox3.Text != "" && textBox4.Text != "" && textBox5.Text != "" && comboBox1.Text != "" && textBox6.Text != "")
            {

                int search;
                string pasport_cod = textBox3.Text;
                string pasport_number = textBox4.Text;
                string visitor_name = textBox1.Text;
                string visitor_surname = textBox2.Text;
                string visitor_phone = textBox5.Text;
                string department_name = comboBox1.Text;
                string visitor_email = textBox6.Text;

                //Запрос на получение данных о посетителе с введенными паспортными данными и определенным ID
                sqlDataAdapter = new SqlDataAdapter($"SELECT visitor_name, pasports.pasport_id FROM visitors INNER JOIN pasports ON visitors.pasport_id = pasports.pasport_id WHERE pasport_cod = '{pasport_cod}' AND pasport_number = '{pasport_number}' AND visitor_id != {visitor_id}; ", SqlConnection);
                DataSet set = new DataSet();
                sqlDataAdapter.Fill(set, "visitor_search");

                search = set.Tables["visitor_search"].Rows.Count;

                //Если по запросу нет посетителей
                if (search == 0)
                {
                    //Передача введенных переменных в процедуру редактирование посетителя
                    SqlCommand cmd = new SqlCommand("visitor_edit", SqlConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.Add(new SqlParameter("@visitor_id", visitor_id));
                    cmd.Parameters.Add(new SqlParameter("@visitor_name", visitor_name));
                    cmd.Parameters.Add(new SqlParameter("@visitor_surname", visitor_surname));
                    cmd.Parameters.Add(new SqlParameter("@pasport_cod", pasport_cod));
                    cmd.Parameters.Add(new SqlParameter("@pasport_number", pasport_number));
                    cmd.Parameters.Add(new SqlParameter("@visitor_phone", visitor_phone));
                    cmd.Parameters.Add(new SqlParameter("@department_name", department_name));
                    cmd.Parameters.Add(new SqlParameter("@visitor_email", visitor_email));

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Данные изменены", "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Введите уникальные паспортные данные!!!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Заполните все поля!!!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
