using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Windows.Forms;

namespace Checking
{
    public partial class Administrator : Form
    {
        SqlConnection SqlConnection = null;
        SqlDataAdapter sqlDataAdapter = null;
        SqlDataAdapter sqlDataAdapter2 = null;
        SqlDataAdapter SqlDataAdapter3 = null;
        SqlDataAdapter SqlDataAdapter4 = null;
        SqlCommand cmd = null;
        readonly Random rnd = new Random();

        readonly int idUser;
        //Получение ID пользователя из формы авторизации
        public Administrator(int idUser)
        {
            this.idUser = idUser;
            InitializeComponent();
        }
        //Вывод данных о пользователе
        private void Administrator_Load(object sender, EventArgs e)
        {
            DataSet info = new DataSet();
            SqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            SqlConnection.Open();

            //Запрос на получение данных о пользователя по ID
            sqlDataAdapter = new SqlDataAdapter($"select user_name, user_surname, user_lvl from users where user_id = {idUser}", SqlConnection);
            sqlDataAdapter.Fill(info, "users");


            object[] obj = info.Tables["users"].Rows[0].ItemArray;


            label5.Text = obj[0].ToString();
            label6.Text = obj[1].ToString();
            label10.Text = obj[2].ToString();

            //Запрос на получение данных о посетителе и передача его в таблицу
            sqlDataAdapter2 = new SqlDataAdapter($"SELECT visitor_id, visitor_name, visitor_surname, pasport_cod, pasport_number, visitor_phone, department_name, department_lvl, visitor_email FROM pasports INNER JOIN visitors ON pasports.pasport_id = visitors.pasport_id INNER JOIN departments ON visitors.department_id = departments.department_id; ", SqlConnection);
            DataSet dataSet = new DataSet();
            sqlDataAdapter2.Fill(dataSet);
            dataGridView1.DataSource = dataSet.Tables[0];

            textBox1.Text = rnd.Next(10000, 100000).ToString();

            //Запрос на получение данных о всех пользователях
            sqlDataAdapter2 = new SqlDataAdapter($"SELECT user_id, user_name, user_surname, user_login, user_password, user_lvl FROM users;", SqlConnection);
            sqlDataAdapter2.Fill(dataSet, "users");
            dataGridView2.DataSource = dataSet.Tables["users"];

            //Запрос на получение данных о всех посетителях
            sqlDataAdapter2 = new SqlDataAdapter($"SELECT visitor_id, visitor_name, visitor_surname, pasport_cod, pasport_number, visitor_phone, department_name, department_lvl, visitor_email FROM visitors INNER JOIN pasports ON pasports.pasport_id = visitors.pasport_id INNER JOIN departments ON departments.department_id = visitors.department_id; ", SqlConnection);
            sqlDataAdapter2.Fill(dataSet, "visitors");
            dataGridView3.DataSource = dataSet.Tables["visitors"];
        }

        //Создание карты посетителя
        private void Button1_Click(object sender, EventArgs e)
        {
            //Если поле с номером карты заполнено
            if (textBox1.Text != "")
            {
                int visitor_id = Convert.ToInt32(dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[0].Value.ToString());
                string card_number = textBox1.Text;
                string state = "Готова";

                int search;
                DataSet info2 = new DataSet();
                //Ищем карту по ID посетителя
                SqlDataAdapter3 = new SqlDataAdapter($"SELECT * FROM cards WHERE visitor_id = {visitor_id};", SqlConnection);
                SqlDataAdapter3.Fill(info2, "card");
                search = info2.Tables["card"].Rows.Count;

                //Если карты нет
                if(search == 0)
                {
                    int search2;
                    DataSet info3 = new DataSet();
                    //Ищем карту по номеру
                    SqlDataAdapter4 = new SqlDataAdapter($"SELECT * FROM cards WHERE card_number = {textBox1.Text};", SqlConnection);
                    SqlDataAdapter4.Fill(info3, "card2");
                    search2 = info3.Tables["card2"].Rows.Count;

                    //Если карты с искомым номером нет, создается новая карта
                    if (search2 == 0)
                    {
                        cmd = new SqlCommand("card_add", SqlConnection)
                        {
                            CommandType = CommandType.StoredProcedure
                        };

                        cmd.Parameters.Add(new SqlParameter("@card_number", card_number));
                        cmd.Parameters.Add(new SqlParameter("@visitor_id", visitor_id));
                        cmd.Parameters.Add(new SqlParameter("@card_state", state));
                        cmd.ExecuteNonQuery();

                        //Отправка электронного письма посетителю о готовности карты
                        SqlCommand cmd2 = new SqlCommand($"SELECT visitor_email FROM visitors WHERE visitor_id = {visitor_id};", SqlConnection);
                        string email = Convert.ToString(cmd2.ExecuteScalar());

                        MailAddress from = new MailAddress("gelios90@bk.ru", "Andrey");
                        MailAddress to = new MailAddress(email);
                        MailMessage m = new MailMessage(from, to);
                        m.Subject = "Уведрмление о готовности карты";
                        m.Body = "<h2>Здравствуйте, ваша карта готова, ждем вас в офисе!</h2>";
                        m.IsBodyHtml = true;

                        SmtpClient smtp = new SmtpClient("smtp.bk.ru", 2525);
                        smtp.Credentials = new NetworkCredential("gelios90@bk.ru", "Arhangel_58");
                        smtp.EnableSsl = true;

                        smtp.Send(m);


                        MessageBox.Show("Данные приняты", "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Карта с таким номером уже есть!!!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("У этого пользователя уже есть карта!!!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Заполните номер карты!!!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //Обновление генератора номеров карты
        private void Button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = rnd.Next(10000, 100000).ToString();
        }
        //Вывод всех посетителей в таблицу 
        private void ToolStripButton3_Click(object sender, EventArgs e)
        {
            sqlDataAdapter2 = new SqlDataAdapter($"SELECT visitor_id, visitor_name, visitor_surname, pasport_cod, pasport_number, visitor_phone, department_name, department_lvl, visitor_email FROM pasports INNER JOIN visitors ON pasports.pasport_id = visitors.pasport_id INNER JOIN departments ON visitors.department_id = departments.department_id; ", SqlConnection);
            DataSet dataSet = new DataSet();
            sqlDataAdapter2.Fill(dataSet);
            dataGridView1.DataSource = dataSet.Tables[0];
        }
        //Вывод только тех посетителей у которых есть готовая карта
        private void ToolStripButton1_Click(object sender, EventArgs e)
        {
            SqlDataAdapter adapter = new SqlDataAdapter("visitors_with_card", SqlConnection);
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;

            DataSet set = new DataSet();
            adapter.Fill(set);
            dataGridView1.DataSource = set.Tables[0];
        }
        //Вывод только тех посетителей у которых нет карты
        private void ToolStripButton2_Click(object sender, EventArgs e)
        {
            SqlDataAdapter adapter2 = new SqlDataAdapter("visitors_without_card", SqlConnection);
            adapter2.SelectCommand.CommandType = CommandType.StoredProcedure;

            DataSet set2 = new DataSet();
            adapter2.Fill(set2);
            dataGridView1.DataSource = set2.Tables[0];
        }
        //Кнопка удаления выбранного пользователя
        private void Button5_Click(object sender, EventArgs e)
        {
            int search;
            string user_lvl = dataGridView2.Rows[dataGridView2.CurrentRow.Index].Cells[5].Value.ToString();
            int user_id = Convert.ToInt32(dataGridView2.Rows[dataGridView2.CurrentRow.Index].Cells[0].Value.ToString());

            //Запрос на поиск всех администраторов системы
            SqlDataAdapter sqlDataAdapter3 = new SqlDataAdapter("SELECT * FROM users WHERE user_lvl = 'Администратор';", SqlConnection);
            DataSet set = new DataSet();
            sqlDataAdapter3.Fill(set, "users");
            search = set.Tables["users"].Rows.Count;

            //Проверка на единственного администратора
            if (user_lvl == "Администратор" && search < 2)
            {
                MessageBox.Show("Запрещено удалять единственного администратора!!!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            //Если администраторов больше одного, то можно удалить пользователя
            else
            {
                SqlCommand cmd = new SqlCommand($"DELETE FROM users WHERE user_id = {user_id};", SqlConnection);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Пользователь удален", "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            
        }
        //Открытив вкладки добавления пользователя
        private void Button3_Click(object sender, EventArgs e)
        {
            Add_user add = new Add_user();
            add.Show();
        }
        //Обновление таблицы пользователей
        private void Button6_Click(object sender, EventArgs e)
        {
            sqlDataAdapter2 = new SqlDataAdapter($"SELECT user_id, user_name, user_surname, user_login, user_password, user_lvl FROM users;", SqlConnection);
            DataSet set = new DataSet();
            sqlDataAdapter2.Fill(set, "users");
            dataGridView2.DataSource = set.Tables["users"];
        }
        //Обновление таблицы посетителей
        private void Button10_Click(object sender, EventArgs e)
        {
            sqlDataAdapter2 = new SqlDataAdapter($"SELECT visitor_id, visitor_name, visitor_surname, pasport_cod, pasport_number, visitor_phone, department_name, department_lvl, visitor_email FROM visitors INNER JOIN pasports ON pasports.pasport_id = visitors.pasport_id INNER JOIN departments ON departments.department_id = visitors.department_id; ", SqlConnection);
            DataSet set = new DataSet();
            sqlDataAdapter2.Fill(set, "visitors");
            dataGridView3.DataSource = set.Tables["visitors"];
        }
        //Вызов формы редактирования посетителя и передача в нее его ID
        private void Button8_Click(object sender, EventArgs e)
        {
            int visitor_id = Convert.ToInt32(dataGridView3.Rows[dataGridView3.CurrentRow.Index].Cells[0].Value.ToString());
            Visitor_edit edit = new Visitor_edit(visitor_id);
            edit.Show();
        }
        //Вызов формы редактирования пользователя и передача в нее его ID
        private void Button4_Click(object sender, EventArgs e)
        {
            int user_id = Convert.ToInt32(dataGridView2.Rows[dataGridView2.CurrentRow.Index].Cells[0].Value.ToString());
            User_edit edit = new User_edit(user_id);
            edit.Show();
        }
        //Удаление посетителя
        private void Button7_Click(object sender, EventArgs e)
        {
            int visitor_id = Convert.ToInt32(dataGridView3.Rows[dataGridView3.CurrentRow.Index].Cells[0].Value.ToString());
            string pasport_cod = dataGridView3.Rows[dataGridView3.CurrentRow.Index].Cells[3].Value.ToString();
            string pasport_number = dataGridView3.Rows[dataGridView3.CurrentRow.Index].Cells[4].Value.ToString();
            SqlCommand cmd = new SqlCommand($"SELECT visitor_id, visitor_name, visitor_surname, pasport_cod, pasport_number, visitor_phone, department_name, visitor_email FROM visitors INNER JOIN pasports ON visitors.pasport_id = pasports.pasport_id INNER JOIN departments ON visitors.department_id = departments.department_id WHERE visitors.visitor_id = {visitor_id}; ", SqlConnection);

            string path = null;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                path = openFileDialog1.FileName;
            }

            using (StreamWriter tw = File.AppendText(path))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    //Создание макета удаляемого посетителя для записи в файл
                    while (reader.Read())
                    {
                        tw.Write(reader["visitor_id"].ToString());
                        tw.Write(" |   " + reader["visitor_name"].ToString());
                        tw.Write("   |   " + reader["visitor_surname"].ToString());
                        tw.Write("   |   " + reader["pasport_cod"].ToString());
                        tw.Write("   |   " + reader["pasport_number"].ToString());
                        tw.Write("   |   " + reader["visitor_phone"].ToString());
                        tw.Write("   |   " + reader["department_name"].ToString());
                        tw.WriteLine("   |   " + reader["visitor_email"].ToString());
                    }
                    tw.WriteLine("Report Generate at : " + DateTime.Now);
                    tw.WriteLine("---------------------------------");
                    tw.Close();
                    reader.Close();
                    //Запрос на удаление посетителя с его паспортными данными и картой
                    SqlCommand cmd2 = new SqlCommand($"DELETE pasports FROM pasports WHERE pasport_cod = '{pasport_cod}' AND pasport_number = '{pasport_number}';", SqlConnection);
                    cmd2.ExecuteNonQuery();
                    MessageBox.Show("Успешно", "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

        }
        //Обновление данных посетителя в таблице создания карт
        private void button9_Click(object sender, EventArgs e)
        {
            sqlDataAdapter2 = new SqlDataAdapter($"SELECT visitor_id, visitor_name, visitor_surname, pasport_cod, pasport_number, visitor_phone, department_name, department_lvl, visitor_email FROM pasports INNER JOIN visitors ON pasports.pasport_id = visitors.pasport_id INNER JOIN departments ON visitors.department_id = departments.department_id; ", SqlConnection);
            DataSet dataSet = new DataSet();
            sqlDataAdapter2.Fill(dataSet);
            dataGridView1.DataSource = dataSet.Tables[0];
        }
        //Открытие окна добавления посетителя
        private void button11_Click(object sender, EventArgs e)
        {
            Visitor_add add = new Visitor_add();
            add.Show();
        }

        private void Administrator_FormClosed(object sender, FormClosedEventArgs e)
        {
            Autorisation autor = new Autorisation();
            autor.Show();
        }
    }
}
