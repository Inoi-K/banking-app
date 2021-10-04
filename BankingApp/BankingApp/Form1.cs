using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace BankingApp
{
    public partial class Form1 : Form
    {
        MySqlConnection connection = new MySqlConnection("server=localhost;port=3306;username=root;password=root;database=banking_app");
        MySqlCommand command;
        string[] values = new string[7];

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            RefreshTable();
        }

        void OpenConnection()
        {
            if (connection.State == ConnectionState.Closed)
                connection.Open();
        }

        void CloseConnection()
        {
            if (connection.State == ConnectionState.Open)
                connection.Close();
        }

        void ExecuteQuery(string query)
        {
            try
            {
                OpenConnection();
                command = new MySqlCommand(query, connection);

                if (command.ExecuteNonQuery() == 1 || query == "DELETE FROM users")
                    MessageBox.Show("Запрос выполнен");
                else
                    MessageBox.Show("Запрос НЕ выполнен. Уточните параметры");
            }
            catch
            {
                MessageBox.Show("Запрос НЕ выполнен. Уточните параметры");
            }
            finally
            {
                CloseConnection();
            }
            RefreshTable();
        }

        void RefreshTable()
        {
            string selectQuery = "SELECT * FROM users";
            DataTable table = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter(selectQuery, connection);
            adapter.Fill(table);
            dataGridView1.DataSource = table;
        }

        int Identify()
        {
            OpenConnection();
            string tmpQuery = $"SELECT id FROM users WHERE (contact_no = {values[3]})";
            command = new MySqlCommand(tmpQuery, connection);
            var tmpQueryResult = command.ExecuteScalar();
            if (tmpQueryResult == null)
            {
                tmpQuery = $"SELECT id FROM users WHERE " +
                    $"first_name = '{values[0]}' AND " +
                    $"last_name = '{values[1]}' AND " +
                    $"middle_name = '{values[2]}' AND " +
                    $"adress = '{values[4]}' AND " +
                    $"status = '{values[5]}' AND " +
                    $"sex = '{values[6]}'";
                    command = new MySqlCommand(tmpQuery, connection);
                    tmpQueryResult = command.ExecuteScalar();
                    if (tmpQueryResult == null)
                        return -1;
            }
            CloseConnection();
            return int.Parse(tmpQueryResult.ToString());
        }   

        bool GetValues()
        {
            values[0] = textBox1.Text;
            values[1] = textBox2.Text;
            values[2] = textBox3.Text;
            values[3] = textBox4.Text;
            values[4] = textBox5.Text;
            values[5] = textBox6.Text;
            values[6] = comboBox1.Text;
            int check = 1;
            // проверка на заполненность полей
            for (int i = 0; i < values.Length; ++i)
                check *= values[i].Length;
            if (check == 0)
                return false;
            return true;
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            if (GetValues())
            {
                int id = Identify();
                if (id == -1)
                {
                    string query = $"INSERT INTO users(first_name,last_name,middle_name,contact_no,adress,status,sex) VALUES('{values[0]}','{values[1]}','{values[2]}','{values[3]}','{values[4]}','{values[5]}','{values[6]}')";
                    ExecuteQuery(query);
                }
                else
                    MessageBox.Show("Пользователь уже существует. Уточните параметры");
            }
            else
                MessageBox.Show("Корректно заполните все поля");
        }

        private void UpdateButton_Click(object sender, EventArgs e)
        {
            if (GetValues())
            {
                int id = Identify();
                if (id != -1)
                {
                    string query = $"UPDATE users SET " +
                        $"first_name = '{values[0]}'," +
                        $"last_name = '{values[1]}'," +
                        $"middle_name = '{values[2]}'," +
                        $"contact_no = '{values[3]}'," +
                        $"adress = '{values[4]}'," +
                        $"status = '{values[5]}'," +
                        $"sex = '{values[6]}' " +
                        $"WHERE id = {id}";
                    ExecuteQuery(query);
                }
                else
                    MessageBox.Show("Пользователь не определён. Уточните параметры");
            }
            else
                MessageBox.Show("Корректно заполните все поля");
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (GetValues())
            {
                int id = Identify();
                if (id != -1)
                {
                    string query = $"DELETE FROM users WHERE (id = {id})";
                    ExecuteQuery(query);
                }
                else
                    MessageBox.Show("Пользователь не определён. Уточните параметры");
            }
            else
                MessageBox.Show("Корректно заполните все поля");
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            string query = "DELETE FROM users";
            ExecuteQuery(query);
        }

        private void textBox_KeyPress_Letter(object sender, KeyPressEventArgs e)
        {
            char value = e.KeyChar;
            string rValue = e.KeyChar.ToString();
            if (!Regex.Match(rValue, @"[а-яА-Я]|[a-zA-Z]").Success && value != 8) // 8 - код клавишы backspace
                e.Handled = true;
        }

        private void textBox_KeyPress_Number(object sender, KeyPressEventArgs e)
        {
            char value = e.KeyChar;
            if (!char.IsDigit(value) && value != 8)
                e.Handled = true;
        }
    }
}
