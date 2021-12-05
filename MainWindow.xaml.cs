﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SQLite;
using YchetPer.Connection;
using System.Data;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;
using System.Drawing;
using System.Net.Mail;
using System.Net;
using System.Drawing.Printing;
using Brushes = System.Windows.Media.Brushes;
using System.Globalization;

namespace YchetPer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window

    {

        public MainWindow()
        {
            InitializeComponent();
            DisplayData();
            CbFill();
            //DGAllEmp.Columns[0].ColumnTextWidth = 3;
            //DGAllEmp.Columns[0].IsReadOnly = true;
            //DGAllEmp.Columns[1].IsReadOnly = true;
            //DGAllEmp.Columns[2].IsReadOnly = true;
            //DGAllEmp.Columns[3].IsReadOnly = true;
            //DGAllEmp.Columns[4].IsReadOnly = true;
            //DGAllEmp.Columns[5].IsReadOnly = true;
            //DGAllEmp.Columns[6].IsReadOnly = true;
            //DGAllEmp.Columns[].IsReadOnly = true;
        }
        public void DisplayData()
        {
            using (SQLiteConnection connection = new SQLiteConnection(DBConnection.myConn))
            {
                try
                {
                    connection.Open();
                    string query = $@"SELECT Devices.ID, Types.Class, Titles.Title, Devices.Number, Conditions.Condition ,NumberKabs.NumKab ,Devices.StartWork,Users.Login, Brands.Brand, Models.Model
                                        FROM Devices JOIN  Types
                                        ON Devices.IDType = Types.ID
                                        JOIN  Conditions
                                        ON Devices.IDCondition = Conditions.ID
                                        JOIN  NumberKabs
                                        ON Devices.IDKabuneta = NumberKabs.ID
                                        JOIN Titles
                                        ON Devices.IDTitle = Titles.ID
										JOIN Users
										ON Devices.IDAddUser = Users.ID
										JOIN Brands
										ON Devices.IDBrand = Brands.ID					
										JOIN Models
										ON Devices.IDModel = Models.ID;";
                    SQLiteCommand cmd = new SQLiteCommand(query, connection);
                    DataTable DT = new DataTable("Devices");
                    SQLiteDataAdapter SDA = new SQLiteDataAdapter(cmd);
                    SDA.Fill(DT);
                    DGAllEmp.ItemsSource = DT.DefaultView;
                    cmd.ExecuteNonQuery();
                    //Login.Text = $"Ваш логин: " + Saver.Login;


                }
                catch (Exception exp)
                {
                    MessageBox.Show(exp.Message);
                }
            }
        }
        public void Delete()
        {
            using (SQLiteConnection connection = new SQLiteConnection(DBConnection.myConn))
            {
                try
                {

                    foreach (var item in DGAllEmp.SelectedItems.Cast<DataRowView>())
                    {
                        string query1 = $@"DELETE FROM Devices WHERE ID = " + item["ID"];
                        connection.Open();
                        SQLiteCommand cmd1 = new SQLiteCommand(query1, connection);
                        DataTable DT = new DataTable("Devices");
                        cmd1.ExecuteNonQuery();
                    }
                }
                catch (Exception exp)
                {
                    MessageBox.Show(exp.Message);
                }
            }
        }
        private void BtnUpd_Click(object sender, RoutedEventArgs e)
        {

            DisplayData();
        }

        private void BtnDel_Click(object sender, RoutedEventArgs e)
        {
            Delete();
            DisplayData();

        }
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddTechnic AddTec = new AddTechnic();
            AddTec.Owner = this;
            AddTec.ShowDialog();
            DisplayData();

        }

        private void Eddit()
        {
            if (DGAllEmp.SelectedIndex != -1)
            {
                EditTechnic editTech = new EditTechnic((DataRowView)DGAllEmp.SelectedItem);
                editTech.Owner = this;
                bool? result = editTech.ShowDialog();
                switch (result)
                {
                    default:
                        DisplayData();
                        break;
                }
            }
            else
            {
                MessageBox.Show("Выберите строку с данными,чтобы ее изменить");
            }
        }
        private void DGAllEmp_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            CbFill();
            Eddit();
        }
        public void CbFill()  //Данные для комбобоксов 
        {
            using (SQLiteConnection connection = new SQLiteConnection(DBConnection.myConn))
            {
                try
                {
                    connection.Open();
                    string query1 = $@"SELECT * FROM Types"; // Типы
                    string query2 = $@"SELECT * FROM Conditions"; // Состояние
                    string query3 = $@"SELECT * FROM NumberKabs"; // Кабинеты
                    string query4 = $@"SELECT * FROM Brands"; // Бренд
                    string query5 = $@"SELECT * FROM Titles"; // Устройства
                    string query6 = $@"SELECT * FROM Models"; // Устройства

                    //----------------------------------------------
                    SQLiteCommand cmd1 = new SQLiteCommand(query1, connection);
                    SQLiteCommand cmd2 = new SQLiteCommand(query2, connection);
                    SQLiteCommand cmd3 = new SQLiteCommand(query3, connection);
                    SQLiteCommand cmd4 = new SQLiteCommand(query4, connection);
                    SQLiteCommand cmd5 = new SQLiteCommand(query5, connection);
                    SQLiteCommand cmd6 = new SQLiteCommand(query6, connection);
                    //----------------------------------------------
                    SQLiteDataAdapter SDA1 = new SQLiteDataAdapter(cmd1);
                    SQLiteDataAdapter SDA2 = new SQLiteDataAdapter(cmd2);
                    SQLiteDataAdapter SDA3 = new SQLiteDataAdapter(cmd3);
                    SQLiteDataAdapter SDA4 = new SQLiteDataAdapter(cmd4);
                    SQLiteDataAdapter SDA5 = new SQLiteDataAdapter(cmd5);
                    SQLiteDataAdapter SDA6 = new SQLiteDataAdapter(cmd6);
                    //----------------------------------------------
                    DataTable dt1 = new DataTable("Types");
                    DataTable dt2 = new DataTable("Conditions");
                    DataTable dt3 = new DataTable("NumberKabs");
                    DataTable dt4 = new DataTable("Brands");
                    DataTable dt5 = new DataTable("Titles");
                    DataTable dt6 = new DataTable("Models");
                    //----------------------------------------------
                    SDA1.Fill(dt1);
                    SDA2.Fill(dt2);
                    SDA3.Fill(dt3);
                    SDA4.Fill(dt4);
                    SDA5.Fill(dt5);
                    SDA6.Fill(dt6);
                    //----------------------------------------------
                    CbClass.ItemsSource = dt1.DefaultView;
                    CbClass.DisplayMemberPath = "Class";
                    CbClass.SelectedValuePath = "ID";
                    //----------------------------------------------
                    CbCondition.ItemsSource = dt2.DefaultView;
                    CbCondition.DisplayMemberPath = "Condition";
                    CbCondition.SelectedValuePath = "ID";
                    //----------------------------------------------
                    CbNumKab.ItemsSource = dt3.DefaultView;
                    CbNumKab.DisplayMemberPath = "NumKab";
                    CbNumKab.SelectedValuePath = "ID";
                    //----------------------------------------------
                    CbBrand.ItemsSource = dt4.DefaultView;
                    CbBrand.DisplayMemberPath = "Brand";
                    CbBrand.SelectedValuePath = "ID";
                    //----------------------------------------------
                    CbTitle.ItemsSource = dt5.DefaultView;
                    CbTitle.DisplayMemberPath = "Title";
                    CbTitle.SelectedValuePath = "ID";
                    //----------------------------------------------
                    CbModel.ItemsSource = dt6.DefaultView;
                    CbModel.DisplayMemberPath = "Model";
                    CbModel.SelectedValuePath = "ID";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        //private void BtnEdd_Click(object sender, RoutedEventArgs e) //Изменение
        //{
        //    if (TbID.Text == null)
        //    {
        //        MessageBox.Show("Выберите в таблице строку изменения", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        //        BtnEdd.IsEnabled = false;
        //    }
        //    else
        //    {
        //        using (SQLiteConnection connection = new SQLiteConnection(DBConnection.myConn))
        //        {
        //            int id, id2, id3, id4;
        //            bool resultClass = int.TryParse(CbClass.SelectedValue.ToString(), out id);
        //            bool resultKab = int.TryParse(CbNumKab.SelectedValue.ToString(), out id2);
        //            bool resultCon = int.TryParse(CbCondition.SelectedValue.ToString(), out id3);
        //            bool resultTitl = int.TryParse(CbTitle.SelectedValue.ToString(), out id4);
        //            var numkab = TbNumber.Text;
        //            var number = TbNumber.Text;
        //            var idtype = CbClass.Text;
        //            var idcon = CbCondition.Text;
        //            var startWork = StartWork.Text;
        //            var ID = TbID.Text;
        //            connection.Open();
        //    string query = $@"UPDATE Devices SET IDType=@IDType, IDKabuneta=@IDKabuneta, IDTitle=@IDTitle, Number=@Number, IDCondition=@IDCondition, StartWork=@StartWork WHERE ID=@ID;";
        //    SQLiteCommand cmd = new SQLiteCommand(query, connection);
        //                try
        //                {
        //                    cmd.Parameters.AddWithValue("@IDType", id);
        //                    cmd.Parameters.AddWithValue("@IDKabuneta", id2);
        //                    cmd.Parameters.AddWithValue("@IDTitle", id4);
        //                    cmd.Parameters.AddWithValue("@Number", number);
        //                    cmd.Parameters.AddWithValue("@IDCondition", id3);
        //                    cmd.Parameters.AddWithValue("@StartWork", startWork);
        //                    cmd.Parameters.AddWithValue("@ID", ID);
        //                    cmd.ExecuteNonQuery();
        //                    MessageBox.Show("Данные изменены");
        //                    DisplayData();
        //}

        //            catch (SQLiteException ex)
        //            {
        //                MessageBox.Show("Error: " + ex.Message);
        //            }
        //        }
        //    }
        //}

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnExcel_Click(object sender, RoutedEventArgs e)
        {
            Excel.Application excel = new Excel.Application();
            excel.Visible = true;
            Excel.Workbook workbook = excel.Workbooks.Add(System.Reflection.Missing.Value);
            Excel.Worksheet sheet1 = (Excel.Worksheet)workbook.Sheets[1];
            sheet1.Name = "Учет техники " + DateTime.Now.Date.ToString("dd/MM/yyyy");
            Excel.Range myRange1 = sheet1.get_Range("A1", "G1");
            myRange1.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
            myRange1.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            myRange1.Merge(Type.Missing);
            myRange1.Font.Name = "Times New Roman";
            myRange1.Font.Bold = true;
            myRange1.Cells.Font.Size = 16;
            sheet1.Range["A1"].Value = "Отчёт был создан в программе YchetPer " + DateTime.Now.Date.ToString("dd/MM/yyyy");
            for (int j = 0; j < DGAllEmp.Columns.Count; j++)
            {
                Excel.Range myRange = (Excel.Range)sheet1.Cells[2, j + 1];
                myRange.Font.Bold = true;
                sheet1.Columns[j + 1].ColumnWidth = 15;
                myRange.Value2 = DGAllEmp.Columns[j].Header;
            }
            for (int i = 0; i < DGAllEmp.Columns.Count; i++)
            {
                for (int j = 0; j < DGAllEmp.Items.Count; j++)
                {
                    TextBlock b = DGAllEmp.Columns[i].GetCellContent(DGAllEmp.Items[j]) as TextBlock;
                    Microsoft.Office.Interop.Excel.Range myRange = (Microsoft.Office.Interop.Excel.Range)sheet1.Cells[j + 3, i + 1];
                    myRange.Value2 = b.Text;
                }
            }
        }


        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            Authoriz aut = new Authoriz();
            aut.Show();
            this.Close();
        }


        //private void TbNumber_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    if (TbNumber.Text == "")
        //    {
        //        DisplayData();
        //    }
        //    else
        //    {
        //        using (SQLiteConnection connection = new SQLiteConnection(DBConnection.myConn))
        //        {
        //            try
        //            {
        //                connection.Open();
        //                string query = $@"SELECT Devices.ID, Types.Class, Titles.Title, Devices.Number, Conditions.Condition ,NumberKabs.NumKab ,Devices.StartWork,Users.Login
        //                                FROM Devices JOIN  Types
        //                                ON Devices.IDType = Types.ID
        //                                JOIN  Conditions
        //                                ON Devices.IDCondition = Conditions.ID
        //                                JOIN  NumberKabs
        //                                ON Devices.IDKabuneta = NumberKabs.ID
        //                                JOIN Titles
        //                                ON Devices.IDTitle = Titles.ID
								//		JOIN Users
								//		ON Devices.IDAddUser = Users.ID
	       //                             WHERE Number=@Number
	       //                             ORDER BY Number ASC;";
        //                SQLiteCommand cmd = new SQLiteCommand(query, connection);
        //                cmd.Parameters.AddWithValue("@Number", TbNumber.Text);
        //                DataTable DT = new DataTable("Devices");
        //                SQLiteDataAdapter SDA = new SQLiteDataAdapter(cmd);
        //                SDA.Fill(DT);
        //                DGAllEmp.ItemsSource = DT.DefaultView;
        //                //Login.Text = $"Ваш логин: " + Saver.Login;


        //            }
        //            catch (Exception exp)
        //            {
        //                MessageBox.Show(exp.Message);
        //            }
        //        }
        //    }
        //}

        private void BtSearch_Click(object sender, RoutedEventArgs e) //Поиск
        {
            if (CbClass.SelectedIndex != -1 || CbTitle.SelectedIndex != -1 || CbBrand.SelectedIndex != -1 || CbModel.SelectedIndex != -1 || CbNumKab.SelectedIndex != -1 || CbCondition.SelectedIndex != -1 || TbNumber.Text != "" || StartWork.Text != "")
            {
               SelectComboBoxOnSeaarch();
            }
            else
            { 
                MessageBox.Show("Выберите хотя бы один пункт для поиска", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                DisplayData(); 
            }           
        }



        private void BtnEdd_Click(object sender, RoutedEventArgs e)
        {
            CbFill();
            Eddit();
        }

        private void BtnEdd3_Click(object sender, RoutedEventArgs e) //Печать
        {
            var pd = new PrintDialog();
            var result = pd.ShowDialog();
            if (result.HasValue && result.Value)
                pd.PrintVisual(DGAllEmp, "Учет техники и переферии");
            //DGAllEmp.Print();
            //DGAllEmp.ShowPrintPreview();
        }

        private void CbClass_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (CbClass.SelectedIndex != -1)
            //{
            //    SelectComboBoxOnSeaarch();
            //}
            //else
            //{
            //    DisplayData();
            //}


            //this.DGAllEmp.SearchHelper.Search(CbClass);
            //var query = from st in MyEntity.MyTabelleDB where st.Schifr >= 3 && st.Schifr <= 5 select st;
            //MyCol = new List<MyClass>(query);
            //for (int i = 0; i < DGAllEmp.Items.Count; i++)
            //{
            //    try
            //    {
            //        if (CbClass.Text != null)
            //        {
            //            object item = DGAllEmp.Items[i];
            //            DGAllEmp.SelectedItem = item;
            //            DGAllEmp.ScrollIntoView(item);
            //            (DGAllEmp.ItemContainerGenerator.ContainerFromIndex(i) as DataGridRow).Background = Brushes.Green;
            //            //(this.DGAllEmp.DetailsViewDefinition[0] as GridViewDefinition).DataGrid.SearchHelper.Search(CbClass.Text);

            //        }
            //    }
            //    catch { }
            //}
            //for (int i = 0; i < DGAllEmp.Items.Count; i++)
            //{
            //    DGAllEmp..Items[i].Selected = false;
            //    for (int j = 0; j < dataGridView1.ColumnCount; j++)
            //        if (dataGridView1.Rows[i].Cells[j].Value != null)
            //            if (dataGridView1.Rows[i].Cells[j].Value.ToString().Contains(textBox1.Text))
            //            {
            //                dataGridView1.Rows[i].Selected = true;
            //                break;
            //            }
            //}

        }
        public void SelectComboBoxOnSeaarch()
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(DBConnection.myConn))
                {
                    //int idClass; int idTitle; int idBrand; int idModel; int idKab; int idCondition;
                    if (CbClass.SelectedIndex != -1) {bool resultClass =  int.TryParse(CbClass.SelectedValue.ToString(), out  Saver.idClass);}
                    if (CbTitle.SelectedIndex != -1) {bool resultTitle = int.TryParse(CbTitle.SelectedValue.ToString(), out Saver.idTitle);   }
                    if (CbBrand.SelectedIndex != -1) {bool resultBrand = int.TryParse(CbBrand.SelectedValue.ToString(), out Saver.idBrand);  }
                    if (CbModel.SelectedIndex != -1) {bool resultModel = int.TryParse(CbModel.SelectedValue.ToString(), out Saver.idModel);   }
                    if (CbNumKab.SelectedIndex != -1) {bool resultKab = int.TryParse(CbNumKab.SelectedValue.ToString(), out Saver.idKab);  }
                    if (CbCondition.SelectedIndex != -1) {bool resultCondition = int.TryParse(CbCondition.SelectedValue.ToString(), out Saver.idCondition);  }                            
                    connection.Open();
                        string query = $@"SELECT Devices.ID, Types.Class, Titles.Title, Devices.Number, Conditions.Condition ,NumberKabs.NumKab ,Devices.StartWork,Users.Login, Brands.Brand, Models.Model
                                        FROM Devices JOIN  Types
                                        ON Devices.IDType = Types.ID
                                        JOIN  Conditions
                                        ON Devices.IDCondition = Conditions.ID
                                        JOIN  NumberKabs
                                        ON Devices.IDKabuneta = NumberKabs.ID
                                        JOIN Titles
                                        ON Devices.IDTitle = Titles.ID
										JOIN Users
										ON Devices.IDAddUser = Users.ID
										JOIN Brands
										ON Devices.IDBrand = Brands.ID					
										JOIN Models
										ON Devices.IDModel = Models.ID
	                                    WHERE Devices.IDType = '{Saver.idClass}' or Devices.IDCondition = '{Saver.idCondition}' or Devices.Number = '{TbNumber.Text}' or Devices.IDTitle = '{Saver.idTitle}' or Devices.IDBrand = '{Saver.idBrand}' or Devices.IDModel = '{Saver.idModel}' or Devices.StartWork = '{StartWork.Text}' or Devices.IDKabuneta= '{Saver.idKab}'
 										ORDER BY Devices.ID ASC;";
                        SQLiteCommand cmd = new SQLiteCommand(query, connection);
                        //cmd.Parameters.AddWithValue("@Number", TbNumber.Text);
                        DataTable DT = new DataTable("Devices");
                        SQLiteDataAdapter SDA = new SQLiteDataAdapter(cmd);
                        SDA.Fill(DT);
                        DGAllEmp.ItemsSource = DT.DefaultView;
                        cmd.ExecuteNonQuery();
                
                }
             }
            catch (Exception exp) { MessageBox.Show(exp.Message); }
        }

        private void BtClear_Click(object sender, RoutedEventArgs e)
        {
            CbClass.SelectedIndex = -1;  
            CbTitle.SelectedIndex = -1;  
            CbBrand.SelectedIndex = -1;  
            CbModel.SelectedIndex = -1; 
            CbNumKab.SelectedIndex = -1; 
            CbCondition.SelectedIndex = -1; 
            TbNumber.Text = ""; 
            StartWork.Text = "";
            DisplayData();
            Saver.idClass = 0;
            Saver.idCondition = 0;
            Saver.idTitle = 0;
            Saver.idModel = 0;
            Saver.idKab = 0;
            Saver.idBrand = 0;
        }
    }
}

