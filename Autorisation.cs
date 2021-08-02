using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Checking
{
    public partial class Autorisation : Form
    {
        private SqlConnection SqlConnection = null;
        private SqlDataAdapter sqlDataAdapter = null;
        public Autorisation()
        {
            InitializeComponent();
        }
        //Подключение к БД
        private void Autorisation_Load(object sender, EventArgs e)
        {
            SqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            SqlConnection.Open();
        }
        //Кнопка входа в приложение
        private void Button1_Click(object sender, EventArgs e)
        {
            string loginIn = textBox1.Text;
            string passwordIn = textBox2.Text;

            int search;

            DataSet logpas = new DataSet();

            //Запрос на поиск пользователей по введенным паролю и логину
            sqlDataAdapter = new SqlDataAdapter($"select user_lvl, user_id from Users where user_login = '{loginIn}' and user_password = '{passwordIn}'", SqlConnection);
            sqlDataAdapter.Fill(logpas, "user_lvl");

            search = logpas.Tables["user_lvl"].Rows.Count;
            //Если пользователь найден
            if (search != 0)
            {
                //Получение данных об уровне пользователя
                object[] obj = logpas.Tables["user_lvl"].Rows[0].ItemArray;
                string access = obj[0].ToString();
                int idUser = Convert.ToInt32(obj[1].ToString());

                switch (access)
                {
                    //Если уровень доступа пользователя "Администратор", то открывается форма для администратора и в нее передается ID пользователя
                    case "Администратор":
                        Administrator admin = new Administrator(idUser);
                        admin.Show();
                        this.Hide();
                        break;
                    //Если уровень доступа пользователя "Охранник", то открывается форма для охранника и в нее передается ID пользователя
                    case "Охранник":
                        Guard guard = new Guard(idUser);
                        guard.Show();
                        this.Hide();
                        break;
                }
            }
            else
            {
                //Проявление сообщения об ошибке входа
                label3.Visible = true;
            }
        }
        //Установка точек вместо символов в поле пароля 
        private void TextBox2_Enter(object sender, EventArgs e)
        {
            textBox2.UseSystemPasswordChar = true;
        }
        //Окошко выбора, показать пароль или спрятать
        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                textBox2.UseSystemPasswordChar = false;
            }
            else
            {
                textBox2.UseSystemPasswordChar = true;
            }
        }
    }
}
