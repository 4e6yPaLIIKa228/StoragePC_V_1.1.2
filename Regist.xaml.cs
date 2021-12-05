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

namespace YchetPer
{
    /// <summary>
    /// Логика взаимодействия для Regist.xaml
    /// </summary>
    public partial class Regist : Window
    

    {
        private string TimeKod;
        public Regist()
        {
            InitializeComponent();
            HiddenElements();
            TbLogin.MaxLength = 30;
            TbPass.MaxLength = 30;
            TbMail.MaxLength = 34;
            TbProverka.MaxLength = 8;
        }

        private void HiddenElements()
        {
            TbProverka.Visibility = Visibility.Hidden;
            LabProv.Visibility = Visibility.Hidden;
            BtnProverka.Visibility = Visibility.Hidden;
        }
        private void VisibleElements()
        {
            TbProverka.Visibility = Visibility.Visible;
            LabProv.Visibility = Visibility.Visible;
            BtnProverka.Visibility = Visibility.Visible;
            BtnRegist.Visibility = Visibility.Hidden;

        }
        private void OffButton()
        {
            TbLogin.IsReadOnly = true;
            TbPass.IsEnabled = false;
            TbMail.IsReadOnly = true;
        }
        private static string RndStr(int len) //геннератор пароля(временного)
        {
            string s = "", symb = "1234567890qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM";
            Random rnd = new Random();

            for (int i = 0; i < len; i++)
                s += symb[rnd.Next(0, symb.Length)];
            return s;

        }

        private void BtnRegist_Click(object sender, RoutedEventArgs e)
        {
            using (SQLiteConnection connection = new SQLiteConnection(DBConnection.myConn))
            {

                if (String.IsNullOrEmpty(TbLogin.Text) || String.IsNullOrEmpty(TbPass.Password) || String.IsNullOrEmpty(TbMail.Text))
                {
                    MessageBox.Show("Заполните поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (TbLogin.Text.Length <= 4)
                {
                    MessageBox.Show(" Логин должен быть больше 4", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (TbPass.Password.Length <= 4)
                {
                    MessageBox.Show(" Пароль должен быть больше 4", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                        connection.Open();
                        string query = $@"SELECT  COUNT(1) FROM USERS WHERE Login=@Login";
                        SQLiteCommand cmd = new SQLiteCommand(query, connection);
                        cmd.Parameters.AddWithValue("@Login", TbLogin.Text.ToLower());
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        int pr = 0;  
                        if (count == 1)
                        {
                            MessageBox.Show("Login занят", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            pr = 1;
                        }
                        string query2 = $@"SELECT  COUNT(1) FROM USERS WHERE Mail=@Mail";
                        SQLiteCommand cmd2 = new SQLiteCommand(query2, connection);
                        cmd2.Parameters.AddWithValue("@Mail", TbMail.Text.ToLower());
                        int count2 = Convert.ToInt32(cmd2.ExecuteScalar()); 
                        int pr2 = 0;
                        if (count2 == 1)
                        {
                            MessageBox.Show("Mail занят", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            pr2 = 1;
                        }
                        if (pr == 0 && pr2 == 0)
                        {
                            try
                            {
                            connection.Close();
                            TimeKod = "1";
                            TimeKod = RndStr(5);
                            SmtpClient Smtp = new SmtpClient("smtp.mail.ru");
                            Smtp.UseDefaultCredentials = true;
                            Smtp.EnableSsl = true;
                            Smtp.Credentials = new NetworkCredential("yarik.test@mail.ru", "UkRjn459Xwf2MNXDS6Zm");
                            MailMessage Message = new MailMessage();
                            Message.From = new MailAddress("yarik.test@mail.ru");
                            Message.To.Add(new MailAddress(TbMail.Text));
                            Message.To.Add(new MailAddress(TbMail.Text));
                            Message.Subject = "Учёт компьютерной техники.";
                            Message.Body = "Временный код: " + TimeKod + "  . Для регистрации  аккаунта: " + TbLogin.Text + ". На это сообще не нужно отвечать.";
                            Smtp.Send(Message);
                            MessageBox.Show("На вашу почту выслан код для проверки, введите его, чтобы продолжить", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                            OffButton();
                            VisibleElements();
                            }
                            catch (FormatException)
                            {
                                MessageBox.Show("Введенная почта некорректна.");
                            }
                            catch (SmtpFailedRecipientsException)
                            {
                                MessageBox.Show("Введенная почта некорректна.");
                            }
                        }
                }
            }
        }
       
        private void BtnProverka_Click(object sender, RoutedEventArgs e)
        {

            if (String.IsNullOrEmpty(TbProverka.Text))
            {
                MessageBox.Show("Заполните поле.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (TbProverka.Text == TimeKod)
            {
            using (SQLiteConnection connection = new SQLiteConnection(DBConnection.myConn))

                {
                    connection.Open();
                    string query = $@"INSERT INTO Users ('Login','Mail','Pass') VALUES (@Login,@Mail,@Pass)";
                    SQLiteCommand cmd = new SQLiteCommand(query, connection);
                    try
                    {
                        byte[] Pass;
                        Func f = new Func();
                        Pass = f.GetHashPassword(TbPass.Password);
                        cmd.Parameters.AddWithValue("@Login", TbLogin.Text.ToLower());
                        cmd.Parameters.AddWithValue("@Pass", Pass);
                        cmd.Parameters.AddWithValue("@Mail", TbMail.Text.ToLower());
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Проверка пройдена. Аккаунт зарегистрирован.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        Authoriz Aftoriz = new Authoriz();
                        this.Close();
                        Aftoriz.ShowDialog();
                    }

                    catch (SQLiteException ex)
                    {
                        MessageBox.Show("Ошибка" + ex);
                       
                    }
                }
            }
            else
            {
                MessageBox.Show("Неправильные данные.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OffButton();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            Authoriz Aftoriz = new Authoriz();
            this.Close();
            Aftoriz.ShowDialog();
        }
       

        private void TbLogin_TextInput(object sender, TextCompositionEventArgs e)
        {
             //if (!Char.IsDigit(e.Text,47)) e.Handled = true;
        }

 
    }
}
