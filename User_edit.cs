using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Checking
{
    public partial class User_edit : Form
    {
        int user_id;
        SqlConnection SqlConnection = null;
        SqlDataAdapter sqlDataAdapter = null;
        public User_edit(int user_id)
        {
            this.user_id = user_id;
            InitializeComponent();
        }
        //выовод данных об изменяемом пользователе
        private void User_edit_Load(object sender, EventArgs e)
        {
            SqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            SqlConnection.Open();

            sqlDataAdapter = new SqlDataAdapter($"SELECT user_name, user_surname, user_login, user_password, user_lvl FROM users WHERE user_id = {user_id}; ", SqlConnection);
            DataSet set = new DataSet();
            sqlDataAdapter.Fill(set, "user");

            object[] obj = set.Tables["user"].Rows[0].ItemArray;

            textBox1.Text = obj[0].ToString();
            textBox2.Text = obj[1].ToString();
            textBox3.Text = obj[2].ToString();
            textBox4.Text = obj[3].ToString();
            comboBox1.Text = obj[4].ToString();
        }
        //Заполнение только буквенных символов
        void For_letter(KeyPressEventArgs e)
        {
            char number = e.KeyChar;

            if (!char.IsLetter(number) && number != 8)
            {
                e.Handled = true;
            }
        }
        //Заполнение только буквенных и цифровых символов
        void For_letter_number(KeyPressEventArgs e)
        {
            char number = e.KeyChar;

            if (!char.IsLetterOrDigit(number) && number != 8)
            {
                e.Handled = true;
            }
        }
        //Кнопка редактирования пользоватедя
        private void button8_Click(object sender, EventArgs e)
        {
            //Проверка на пустые строки
            if (textBox1.Text != "" && textBox2.Text != "" && textBox1.Text != "" && textBox3.Text != "" && textBox4.Text != "" && comboBox1.Text != "")
            {

                string user_name = textBox1.Text;
                string user_surname = textBox2.Text;
                string user_login = textBox3.Text;
                string user_password = textBox4.Text;
                string user_lvl = comboBox1.Text;
                int search;

                //Запрос на поиск пользователя с введенным ID и обратным логином
                sqlDataAdapter = new SqlDataAdapter($"SELECT user_name FROM users WHERE user_login = '{user_login}' AND user_id = {user_id}; ", SqlConnection);
                DataSet set = new DataSet();
                sqlDataAdapter.Fill(set, "user_search");

                search = set.Tables["user_search"].Rows.Count;
                //Если пользователей с совподающим логином нет, тогда отправляем данные в процедуру на изменение пользователя
                if (search == 0)
                {
                    SqlCommand cmd = new SqlCommand("user_edit", SqlConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.Add(new SqlParameter("@user_id", user_id));
                    cmd.Parameters.Add(new SqlParameter("@user_name", user_name));
                    cmd.Parameters.Add(new SqlParameter("@user_surname", user_surname));
                    cmd.Parameters.Add(new SqlParameter("@user_login", user_login));
                    cmd.Parameters.Add(new SqlParameter("@user_password", user_password));
                    cmd.Parameters.Add(new SqlParameter("@user_lvl", user_lvl));
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Данные изменены", "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Придумайте уникальный логин!!!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Заполните все поля!!!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            For_letter_number(e);
        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            For_letter_number(e);
        }
    }
}
