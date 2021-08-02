using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Checking
{
    public partial class Guard : Form
    {
        SqlConnection SqlConnection = null;
        SqlDataAdapter sqlDataAdapter = null;
        SqlDataAdapter SqlDataAdapter2 = null;
        SqlDataAdapter SqlDataAdapter3 = null;
        SqlCommand cmd;

        readonly int idUser;
        public Guard(int idUser)
        {
            this.idUser = idUser;
            InitializeComponent();
        }

        private void Guard_Load(object sender, EventArgs e)
        {
            //Подключение к БД
            DataSet info = new DataSet();
            SqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            SqlConnection.Open();

            //Запрос на получение данных о пользователе на основе ID вошедшего пользователя
            sqlDataAdapter = new SqlDataAdapter($"select user_name, user_surname, user_lvl from users where user_id = {idUser};", SqlConnection);
            sqlDataAdapter.Fill(info, "users");


            object[] obj = info.Tables["users"].Rows[0].ItemArray;


            label5.Text = obj[0].ToString();
            label6.Text = obj[1].ToString();
            label10.Text = obj[2].ToString();

            //Запрос на получение данных о департаментах
            SqlDataAdapter3 = new SqlDataAdapter($"SELECT * FROM departments", SqlConnection);
            DataTable table = new DataTable();
            SqlDataAdapter3.Fill(table);

            //Заполнение выпадающего списка названиями отделов 
            comboBox1.DataSource = table;
            comboBox1.DisplayMember = "department_name";

        }
        //Кнопка регистрации посетителя
        private void Button1_Click(object sender, EventArgs e)
        {
            //Проверка на заполнение всех полей
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
                //Запрос на получение данных о паспорте по введенным номеру и ссерии
                SqlDataAdapter2 = new SqlDataAdapter($"SELECT pasport_cod, pasport_number FROM pasports WHERE pasport_cod = {pasport_cod} AND pasport_number = {pasport_number};", SqlConnection);
                SqlDataAdapter2.Fill(info2, "pasport");
                search = info2.Tables["pasport"].Rows.Count;

                //Если введенные паспортные данные уникальны
                if (search == 0)
                {
                    //Отправка введенных данных в процедуру создания посетителя
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

        }

        //Ограничение на ввод в поле только букв
        void For_letter(KeyPressEventArgs e)
        {
            char number = e.KeyChar;

            if (!char.IsLetter(number) && number != 8)
            {
                e.Handled = true;
            }
        }
        //Ограничение на ввод в поле только цифр
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
        //Ограничение на ввод в поле только цифр и знака +
        private void TextBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;

            if (!char.IsDigit(number) && number != 8 && number != 43)
            {
                e.Handled = true;
            }
        }
        //Ограничение на ввод в поле только цифр
        private void TextBox6_KeyPress(object sender, KeyPressEventArgs e)
        {
            For_number(e);
        }

        private void textBox8_KeyPress(object sender, KeyPressEventArgs e)
        {
            For_number(e);
        }

        private void textBox7_KeyPress(object sender, KeyPressEventArgs e)
        {
            For_number(e);
        }

        //Кнопка проверки паспортных данных посетителя
        private void Button2_Click(object sender, EventArgs e)
        {
            //Проверка на пустоту полей
            if (textBox6.Text != "" && textBox7.Text != "")
            {
                string pasport_cod = textBox7.Text;
                string pasport_number = textBox6.Text;

                int search;
                DataSet info2 = new DataSet();
                //Запрос на получение данных посетителей с введенными паспортными данными
                SqlDataAdapter2 = new SqlDataAdapter($"SELECT * FROM visitors INNER JOIN pasports ON pasports.pasport_id = visitors.pasport_id WHERE pasport_cod = {pasport_cod} AND pasport_number = {pasport_number};", SqlConnection);
                SqlDataAdapter2.Fill(info2, "visitor");
                search = info2.Tables["visitor"].Rows.Count;

                //Если пользователь с такими паспортными данными найден
                if (search != 0)
                {
                    int search2;
                    DataSet info3 = new DataSet();
                    //Запрос на получение данных посетителей с введенными паспортными данными и существующей картой
                    SqlDataAdapter2 = new SqlDataAdapter($"SELECT * FROM visitors INNER JOIN pasports ON pasports.pasport_id = visitors.pasport_id AND pasport_cod = {pasport_cod} AND pasport_number = {pasport_number} INNER JOIN cards ON cards.visitor_id = visitors.visitor_id;", SqlConnection);
                    SqlDataAdapter2.Fill(info3, "card");
                    search2 = info3.Tables["card"].Rows.Count;
                    //Усли у посетителя есть карта
                    if (search2 != 0)
                    {
                        //Окно получение карты
                        DialogResult res = MessageBox.Show("Карта готова, поучить?", "Карта", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                        if (res == DialogResult.Yes)
                        {
                            int search3;
                            DataSet info4 = new DataSet();
                            //Запрос на получение данных о посетителе с введенными паспортными данными и состоянием карты "Готова"
                            SqlDataAdapter2 = new SqlDataAdapter($"SELECT * FROM visitors INNER JOIN pasports ON pasports.pasport_id = visitors.pasport_id AND pasport_cod = {pasport_cod} AND pasport_number = {pasport_number} INNER JOIN cards ON cards.visitor_id = visitors.visitor_id WHERE card_state = 'Готова'; ", SqlConnection);
                            SqlDataAdapter2.Fill(info4, "card2");
                            search3 = info4.Tables["card2"].Rows.Count;

                            //Если у посетителя готова карта
                            if (search3 != 0)
                            {
                                //Запрос на изменение значения карты с "Готова" на "Используется"
                                SqlCommand cmd1 = new SqlCommand($"UPDATE [cards] SET cards.card_state = 'Используется' FROM cards INNER JOIN visitors ON visitors.visitor_id = cards.visitor_id INNER JOIN pasports ON visitors.pasport_id = pasports.pasport_id AND pasports.pasport_cod = {pasport_cod} AND pasport_number = {pasport_number}; ", SqlConnection);
                                cmd1.ExecuteNonQuery();

                                //Вызоф формы карты с передачей в нее паспортных данных посетитля
                                Card card = new Card(pasport_cod, pasport_number);
                                card.Show();
                            }
                            else
                            {
                                MessageBox.Show("Карта используется или использована!!!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Ваша карта в очереди на создание!!!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Такого посетителя нет!!!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Заполните все строки!!!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //Кнопка деактивации карты
        private void Button3_Click(object sender, EventArgs e)
        {
            //Проверка поля на пустоту
            if (textBox8.Text != "")
            {
                string card_number = textBox8.Text;

                int search2;
                DataSet info3 = new DataSet();
                //Запрос на получение данных карт по введенному номеру карты
                SqlDataAdapter2 = new SqlDataAdapter($"SELECT * FROM cards WHERE card_number = {card_number} AND card_state = 'Используется';", SqlConnection);
                SqlDataAdapter2.Fill(info3, "card");
                search2 = info3.Tables["card"].Rows.Count;

                //Усли карта существует
                if (search2 != 0)
                {
                    //Запрос на изменение статуса карты с "Используется" на "Использована"
                    SqlCommand cmd = new SqlCommand($"UPDATE cards SET card_state = 'Использована' FROM cards WHERE card_number = {card_number};", SqlConnection);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Карта деактивирована", "Карта", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Карта не используется, уже использована или такой карты нет!!!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Заполните все строки!!!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void Guard_FormClosed(object sender, FormClosedEventArgs e)
        {
            Autorisation auth = new Autorisation();
            auth.Show();
        }
    }
}