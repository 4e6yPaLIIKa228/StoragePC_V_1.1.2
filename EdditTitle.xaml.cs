using System;
using System.Windows;
using System.Data.SQLite;
using YchetPer.Connection;


namespace YchetPer
{
    /// <summary>
    /// Логика взаимодействия для EdditTitle.xaml
    /// </summary>
    public partial class EdditTitle : Window
    {
        public EdditTitle()
        {
            InitializeComponent();
        }
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            using (SQLiteConnection connection = new SQLiteConnection(DBConnection.myConn))
            {

                if (String.IsNullOrEmpty(TbTitl.Text))
                {
                    MessageBox.Show("Заполните поле", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    connection.Open();
                    string query = $@"INSERT INTO Titles('Title') values (@Title)";
                    SQLiteCommand cmd = new SQLiteCommand(query, connection);
                    try
                    {
                        cmd.Parameters.AddWithValue("@Title", TbTitl.Text);
                        cmd.ExecuteNonQuery();
                        this.Close();
                    }
                    catch (SQLiteException ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }

            }
        }
    }
}
