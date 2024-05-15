using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace LibraryFund
{
    public partial class FormMain : Form
    {
        private SqlConnection connection = new SqlConnection();
        private SqlDataAdapter dataAdapter = new SqlDataAdapter();

        public FormMain()
        {
            InitializeComponent();
            if (Properties.Settings.Default.FormSize != null)
            {
                Size = Properties.Settings.Default.FormSize;

            }

            dataBaseGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Tables.DropDownStyle = ComboBoxStyle.DropDownList;
            Views.DropDownStyle = ComboBoxStyle.DropDownList;

            connection.ConnectionString = "Server=(localdb)\\mssqllocaldb;Database=Library_fund;Trusted_Connection=True;";
            try
            {

                connection.Open();
                DataTable tables = connection.GetSchema("Tables", new string[] { null, null, null, "BASE TABLE" });
                foreach (DataRow row in tables.Rows)
                {
                    string tableName = (string)row["TABLE_NAME"];
                    Tables.Items.Add(tableName);
                }

                DataTable views = connection.GetSchema("Tables", new string[] { null, null, null, "VIEW" });
                foreach (DataRow row in views.Rows)
                {
                    string viewName = (string)row["TABLE_NAME"];
                    Views.Items.Add(viewName);
                }

                MessageBox.Show("Connection is open", "Connection",
MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

                Tables.SelectedIndex = 0;

            }
            catch
            {
                MessageBox.Show("Connection is not open", "Connection",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            GetData("select * from book", bindingSourceServerSQL, dataBaseGridView);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Lab 6\nBy Litvinov Andrii 535v", "About program",
                MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.FormSize = Size;
            Properties.Settings.Default.Save();
        }

        private void sqlButton_Click(object sender, EventArgs e)
        {
            GetData(this.sqlTextBox.Text, bindingSourceServerSQL, dataBaseGridView);
        }

        public void GetData(string selectCommand, BindingSource bindingSource, DataGridView dataGridView)
        {
            try
            {
                dataAdapter = new SqlDataAdapter(selectCommand, connection);

                SqlCommandBuilder builder = new SqlCommandBuilder(dataAdapter);

                DataTable table = new DataTable
                {
                    Locale = System.Globalization.CultureInfo.InvariantCulture
                };
                dataAdapter.Fill(table);
                bindingSource.DataSource = table;

                if (dataGridView != null)
                {
                    dataGridView.DataSource = bindingSource;

                    dataGridView.AutoResizeColumns(
                        DataGridViewAutoSizeColumnsMode.AllCells);
                    dataGridView.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCells);
                };
            }
            catch (SqlException ex)
            {
                MessageBox.Show(selectCommand + "\n\n" + ex.Message, "Помилка SQL-запиту",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void sqlTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                sqlButton_Click(sender, e);
            }
        }

        private void toolStripButtonSave_Click(object sender, EventArgs e)
        {
            dataAdapter.Update((DataTable)bindingSourceServerSQL.DataSource);
        }

        private void SearchBox_TextChanged(object sender, EventArgs e)
        {
            int i = this.bindingSourceServerSQL.Find("id", SearchBox.Text);

            if (i == -1)
            {
                DataView dv = new DataView((DataTable)bindingSourceServerSQL.DataSource);
                dv.RowFilter = String.Format("id LIKE {0}", SearchBox.Text);
                if (dv.Count != 0) i = this.bindingSourceServerSQL.Find("id", dv[0]["id"]);
                dv.Dispose();
            }
            this.bindingSourceServerSQL.Position = i;
        }

        private void Tables_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetData("select * from " + Tables.Text, bindingSourceServerSQL, dataBaseGridView);

        }

        private void Views_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetData("select * from " + Views.Text, bindingSourceServerSQL, dataBaseGridView);

        }

    }
}