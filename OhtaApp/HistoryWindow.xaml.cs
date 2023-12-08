using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Логика взаимодействия для HistoryWindow.xaml
    /// </summary>
    public partial class HistoryWindow : Window
    {

        Ohta_ParkEntities ohtaent = new Ohta_ParkEntities();
        DispatcherTimer timer = new DispatcherTimer();
        bool createOnlyOneWindow = true;
        bool createOnlyOneMessage = true;
        int timeLeft;
        DispatcherTimer Timer = new DispatcherTimer();
        IEnumerable<Employees_Users> EmployeeList = new ObservableCollection<Employees_Users>();
        Employees user = new Employees();
        public HistoryWindow(Employees userinfo, int continueTime)
        {
            InitializeComponent();
            timeLeft = continueTime;
            user = userinfo;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1); // интервал времени в секундах
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
            Timer.Interval = TimeSpan.FromMilliseconds(1000); // интервал времени в миллисекундах
            Timer.Tick += new EventHandler(timerLeft_Tick);
            Timer.Start();
            EntersLV.ItemsSource = ohtaent.Employees_Users.ToList();
            EmployeeList = ohtaent.Employees_Users.ToList();
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            if (timeLeft <= 0)
            {
                timer.Stop();
                if (createOnlyOneWindow)
                {
                    var mw = new LoginWindow(false);
                    mw.Show();
                    createOnlyOneWindow = false;
                }
                this.Close();
            }
            if (timeLeft == 300)
            {
                if (createOnlyOneMessage)
                {
                    MessageBox.Show("Сеанс закончится через 5 минут!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                    createOnlyOneMessage = false;
                }
            }

        }
        public void timerLeft_Tick(object sender, EventArgs e)
        {
            countDownTB.Text = String.Format("Время сеанса:"+"{0}:{1}", timeLeft / 60, timeLeft % 60);
            if (timeLeft <= 0)
            {
                timeLeft = 60;
                Timer.Stop();
            }
            timeLeft--;
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            Timer.Stop();
            int continueTime = timeLeft;
            var adminWndw = new AdministratorWindow(user ,continueTime,false);
            adminWndw.Show();
            createOnlyOneWindow = true;
            this.Close();
        }
        public void FilterList()//метод для фильтрации по логину и сортировке даты и времени посещения
        {
            var FilterText = FilterTB.Text.Trim();
            if (FilterText.Length > 0)
            {
                if (DateCB.SelectedIndex == 0)
                {
                    EmployeeList = ohtaent.Employees_Users.OrderBy(x => x.Last_Enter).Where(x => x.Login.ToLower().Contains(FilterText.ToLower())).ToList();
                }
                else
                {
                    EmployeeList = ohtaent.Employees_Users.OrderByDescending(x => x.Last_Enter).Where(x => x.Login.ToLower().Contains(FilterText.ToLower())).ToList();
                }
            }
            else
            {
                if (DateCB.SelectedIndex == 0)
                {
                    EmployeeList = ohtaent.Employees_Users.OrderBy(x => x.Last_Enter).ToList();
                }
                else
                {
                    EmployeeList = ohtaent.Employees_Users.OrderByDescending(x => x.Last_Enter).ToList();
                }
            }
            EntersLV.ItemsSource = EmployeeList;
        }
        private void FilterTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterList();
        }
        private void DateCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FilterTB.Text.Length <= 0)
            {
                if (DateCB.SelectedIndex == 0)
                {
                    EntersLV.ItemsSource = EmployeeList.OrderBy(x => x.Last_Enter);
                }
                else
                {
                    EntersLV.ItemsSource = EmployeeList.OrderByDescending(x => x.Last_Enter);
                }
            }
            else
            {
                FilterList();
            }  
        }       
    }
}
