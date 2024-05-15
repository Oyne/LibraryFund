using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace LibraryFund
{
    public partial class FormMain : Form
    {
        private static readonly string _connectionString = "Server=(localdb)\\mssqllocaldb;Database=Library_fund;Trusted_Connection=True;";
        private static readonly SqlConnection _connection = new SqlConnection(_connectionString);
        private SqlDataAdapter _dataAdapter = new SqlDataAdapter();

        public FormMain()
        {
            InitializeComponent();

            Size = Properties.Settings.Default.FormSize;

            dataBaseGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Tables.DropDownStyle = ComboBoxStyle.DropDownList;
            Views.DropDownStyle = ComboBoxStyle.DropDownList;
            SearchAttribute.DropDownStyle = ComboBoxStyle.DropDownList;

            FillComboBoxes();
        }

        private void FillComboBoxes()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
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
            }
            catch
            {
                MessageBox.Show("Connection is not open", "Connection",
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

        public void GetData(string selectCommand)
        {
            try
            {
                _dataAdapter = new SqlDataAdapter(selectCommand, _connection);

                SqlCommandBuilder builder = new SqlCommandBuilder(_dataAdapter);

                DataTable table = new DataTable
                {
                    Locale = System.Globalization.CultureInfo.InvariantCulture
                };

                _dataAdapter.Fill(table);

                List<string> columns = table.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToList();
                SearchAttribute.ComboBox.DataSource = columns;

                bindingSourceServerSQL.DataSource = table;

                if (dataBaseGridView != null)
                {
                    dataBaseGridView.DataSource = bindingSourceServerSQL;

                    dataBaseGridView.AutoResizeColumns(
                        DataGridViewAutoSizeColumnsMode.AllCells);
                    dataBaseGridView.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCells);
                };
            }
            catch (SqlException ex)
            {
                MessageBox.Show(selectCommand + "\n\n" + ex.Message, "Помилка SQL-запиту",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void sqlButton_Click(object sender, EventArgs e)
        {
            GetData(this.sqlTextBox.Text);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Сourse work\nBy Litvinov Andrii 535v", "About program",
                MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.FormSize = Size;
            Properties.Settings.Default.Save();
        }

        private void toolStripButtonSave_Click(object sender, EventArgs e)
        {
            _dataAdapter.Update((DataTable)bindingSourceServerSQL.DataSource);
        }

        private void SearchBox_TextChanged(object sender, EventArgs e)
        {
            int index;
            string columnName = SearchAttribute.Text;
            string searchText = SearchBox.Text;
            try
            {
                index = bindingSourceServerSQL.Find(columnName, searchText);
            }
            catch
            {
                index = -1;
            }

            if (index == -1)
            {
                try
                {
                    using (DataView view = new DataView((DataTable)bindingSourceServerSQL.DataSource))
                    {
                        view.RowFilter = $"{columnName} LIKE '%{searchText}%'";
                        if (view.Count != 0)
                        {
                            index = bindingSourceServerSQL.Find(columnName, view[0][columnName]);
                        }
                    }
                }
                catch
                {
                    index = -1;
                }

            }

            bindingSourceServerSQL.Position = index;
        }

        private void Tables_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetData("select * from " + Tables.Text);

        }

        private void Views_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetData("select * from " + Views.Text);

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }


    }
}