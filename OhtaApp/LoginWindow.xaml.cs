using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace OhtaApp
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {


        Ohta_ParkEntities ohtaent = new Ohta_ParkEntities();
        int errorCount = 0;
        int timeLeft = 180;
        bool manualEnter;
        DispatcherTimer timer = new DispatcherTimer();
        public LoginWindow(bool enter)
        {
            InitializeComponent();
            manualEnter = enter;
            passwordCloseButton.Visibility = Visibility.Hidden;
        }
        private void passwordOpenButton_Click(object sender, RoutedEventArgs e)//функция для открытия пароля
        {
            passwordOpenButton.Visibility = Visibility.Hidden;
            passwordCloseButton.Visibility = Visibility.Visible;
            passwordTB.Text = passwordPB.Password;
            passwordTB.Visibility = Visibility.Visible;
            passwordPB.Visibility = Visibility.Hidden;
        }

        private void passwordCloseButton_Click(object sender, RoutedEventArgs e)//функция для закрытия пароля
        {
            passwordOpenButton.Visibility = Visibility.Visible;
            passwordCloseButton.Visibility = Visibility.Hidden;
            passwordPB.Password = passwordTB.Text;
            passwordTB.Visibility = Visibility.Hidden;
            passwordPB.Visibility = Visibility.Visible;
        }
        private void loginbutton_Click(object sender, RoutedEventArgs e)
        {
            if (loginTB.Text != "" && passwordTB.Text != "" || passwordPB.Password != "")
            {
                if (errorCount < 1)
                {
                    if (ohtaent.Employees_Users.Any(u => u.Login == loginTB.Text) && (ohtaent.Employees_Users.Any(u => u.Password == passwordTB.Text) 
                        || ohtaent.Employees_Users.Any(u => u.Password == passwordPB.Password)))//проверка логина и пароля на существование в бд
                    {
                        Employees_Users user = ohtaent.Employees_Users.Where(x => x.Login == loginTB.Text).First();
                        Employees employee = ohtaent.Employees.Find(user.ID_Employee);
                        MessageBox.Show("Вы успешно зашли, " + employee.Name + "!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                        switch (employee.ID_Role)
                        {
                            case 1:
                                SellerWindow sw = new SellerWindow(employee, 600, true);
                                sw.Show();
                                break;
                            case 2:
                                AdministratorWindow aw = new AdministratorWindow(employee, 600, true);
                                aw.Show();
                                break;
                            case 3:
                                ShiftSupervisorWindow ssw = new ShiftSupervisorWindow(employee, 600, true);
                                ssw.Show();
                                break;
                        }
                        this.Close();
                    }
                    else
                    {
                        errorCount++;
                        MessageBox.Show("Не удалось войти в систему", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    CaptchaWindow cptchWndw = new CaptchaWindow();
                    cptchWndw.Show();
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("Введие логин и пароль для входа", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Hand);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!manualEnter)//если окно отрылось после окончания времени сеанса
            {
                timeLeftToEnterTB.Visibility = Visibility.Visible;
                loginButton.IsEnabled = false;
                timer.Interval = TimeSpan.FromSeconds(1); // интервал времени в секундах
                timer.Tick += new EventHandler(timer_Tick);
                timer.Start();
            }
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            timeLeftToEnterTB.Text = string.Format("Повторный вход через: " + "{0}:{1}", timeLeft / 60, timeLeft % 60);
            if (timeLeft <= 0)
            {
                timeLeftToEnterTB.Visibility = Visibility.Hidden;
                loginButton.IsEnabled = true;
                timer.Stop();
            }
            timeLeft--;
        }
    }
}
