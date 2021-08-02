using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Checking
{
    public partial class Add_user : Form
    {
        SqlConnection SqlConnection = null;
        SqlDataAdapter sqlDataAdapter = null;
        public Add_user()
        {
            InitializeComponent();
        }
        //Подключение к базе данных
        private void Add_user_Load(object sender, EventArgs e)
        {
            SqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            SqlConnection.Open();

        }
        //Функция для написания в поле только букв
        void For_letter(KeyPressEventArgs e)
        {
            char number = e.KeyChar;

            if (!char.IsLetter(number) && number != 8)
            {
                e.Handled = true;
            }
        }
        //Только буквы и цифры
        void For_letter_number(KeyPressEventArgs e)
        {
            char number = e.KeyChar;

            if (!char.IsLetterOrDigit(number) && number != 8)
            {
                e.Handled = true;
            }
        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            For_letter(e);
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            For_letter(e);
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            For_letter_number(e);
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            For_letter_number(e);
        }
        //Добавление пользователя
        private void button1_Click(object sender, EventArgs e)
        {
            //Проверка на пустоту строк
            if (textBox1.Text != "" && textBox2.Text != "" && textBox3.Text != "" && textBox4.Text != "" && comboBox1.Text != "")
            {
                string user_name = textBox4.Text;
                string user_surname = textBox3.Text;
                string user_lvl = comboBox1.Text;
                string user_login = textBox2.Text;
                string user_password = textBox1.Text;

                int search;
                DataSet info = new DataSet();
                //Составление запроса к БД на получение данных о пользователе по заданным логину и паролю
                sqlDataAdapter = new SqlDataAdapter($"SELECT * FROM users WHERE user_login = '{user_login}' AND user_password = '{user_password}';", SqlConnection);
                sqlDataAdapter.Fill(info, "users");
                search = info.Tables["users"].Rows.Count;
                //Если логин и пароль не заняты добавляем пользователя
                if (search == 0)
                {
                    //Передача переменных в процедуру добавления посетителя
                    SqlCommand cmd = new SqlCommand("add_user", SqlConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.Add(new SqlParameter("@user_name", user_name));
                    cmd.Parameters.Add(new SqlParameter("@user_surname", user_surname));
                    cmd.Parameters.Add(new SqlParameter("@user_login", user_login));
                    cmd.Parameters.Add(new SqlParameter("@user_password", user_password));
                    cmd.Parameters.Add(new SqlParameter("@user_lvl", user_lvl));
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Пользователь добавлен", "Карта", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Введите уникальные логин и пароль!!!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Заполните все строки!!!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}