using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        // Original connection string
        private string connectionString = "Server=DT11GM0LS7E6;Database=UserDB;Trusted_Connection=True;";

        public Form1()
        {
            InitializeComponent();
            EnsureDatabaseAndTable(); // Create DB and table if they don't exist
            LoadUsers();              // Load users into DataGridView
        }

        // Ensure Database and Users table exist
        private void EnsureDatabaseAndTable()
        {
            // Step 1: Create Database if not exists
            using (SqlConnection con = new SqlConnection("Server=DT11GM0LS7E6;Database=master;Trusted_Connection=True;"))
            {
                string createDB = @"
                IF DB_ID('UserDB') IS NULL
                CREATE DATABASE UserDB";
                using (SqlCommand cmd = new SqlCommand(createDB, con))
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            // Step 2: Create Users table if not exists
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string createTable = @"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Users' AND xtype='U')
                CREATE TABLE Users (
                    Id INT PRIMARY KEY IDENTITY(1,1),
                    FullName NVARCHAR(100),
                    Username NVARCHAR(50),
                    Password NVARCHAR(50)
                );";
                using (SqlCommand cmd = new SqlCommand(createTable, con))
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Load users into DataGridView
        private void LoadUsers()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter("SELECT Id, FullName, Username FROM Users", con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;

                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dataGridView1.MultiSelect = false;
            }
        }

        // Add User
        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox3.Text != textBox4.Text)
            {
                MessageBox.Show("Passwords do not match!");
                return;
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Users (FullName, Username, Password) VALUES (@FullName, @Username, @Password)";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@FullName", textBox1.Text);
                    cmd.Parameters.AddWithValue("@Username", textBox2.Text);
                    cmd.Parameters.AddWithValue("@Password", textBox3.Text);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            MessageBox.Show("User added successfully!");
            LoadUsers();
            ClearFields();
        }

        // Update User
        private void button4_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Select a user to update.");
                return;
            }

            if (textBox3.Text != textBox4.Text)
            {
                MessageBox.Show("Passwords do not match!");
                return;
            }

            int userId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["Id"].Value);

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "UPDATE Users SET FullName=@FullName, Username=@Username, Password=@Password WHERE Id=@Id";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@FullName", textBox1.Text);
                    cmd.Parameters.AddWithValue("@Username", textBox2.Text);
                    cmd.Parameters.AddWithValue("@Password", textBox3.Text);
                    cmd.Parameters.AddWithValue("@Id", userId);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            MessageBox.Show("User updated successfully!");
            LoadUsers();
            ClearFields();
        }

        // Delete User
        private void button5_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Select a user to delete.");
                return;
            }

            int userId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["Id"].Value);

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Users WHERE Id=@Id";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Id", userId);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            MessageBox.Show("User deleted successfully!");
            LoadUsers();
            ClearFields();
        }

        // Click row to fill textboxes
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                textBox1.Text = row.Cells["FullName"].Value.ToString();
                textBox2.Text = row.Cells["Username"].Value.ToString();
                textBox3.Text = "";
                textBox4.Text = "";
            }
        }

        // Clear textboxes
        private void ClearFields()
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
        }

        private void textBox1_TextChanged(object sender, EventArgs e) { }
        private void textBox2_TextChanged(object sender, EventArgs e) { }
        private void textBox3_TextChanged(object sender, EventArgs e) { }
        private void textBox4_TextChanged(object sender, EventArgs e) { }
    }
}
