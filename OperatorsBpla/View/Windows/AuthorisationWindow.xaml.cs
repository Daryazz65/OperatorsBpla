using Bpla.AppData;
using OperatorsBpla.Model;
using System.Windows;

namespace OperatorsBpla.View.Windows
{
    /// <summary>
    /// Логика взаимодействия для AuthorisationWindow.xaml
    /// </summary>
    public partial class AuthorisationWindow : Window
    {
        private bool _isCaptchaVerified = false;
        public AuthorisationWindow()
        {
            InitializeComponent();
        }
        private void EnterBtn_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTb.Text;
            string password = PasswordTb.Password;
            if (AuthorisationHelper.Authorise(login, password))
            {
                User user = AuthorisationHelper.selectedUser;
                if (user.Role != null && user.Role.Name.Trim() == "Системный администратор")
                {
                    AdminWindow adminWindow = new AdminWindow();
                    adminWindow.Show();
                    this.Close();
                }
                else if (user.Role != null && user.Role.Name.Trim() == "Учитель")
                {
                    MainWindowTeacher mainWindowTeacher = new MainWindowTeacher();
                    mainWindowTeacher.MainFrameTeacher.Navigate(new View.Pages.TeacherCrudLecturePage());
                    mainWindowTeacher.Show();
                    this.Close();
                }
                else
                {
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    this.Close();
                }
            }
        }
        private void PasswordRecoveryBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxHelper.Information("Обратитесь к системному администратору.");
        }
        private void CaptchaBtn_Click(object sender, RoutedEventArgs e)
        {
            CaptchaWindow captchaWindow = new CaptchaWindow();
            if (captchaWindow.ShowDialog() == true && captchaWindow.IsVerified)
            {
                _isCaptchaVerified = true;
                MessageBoxHelper.Information("Капча пройдена успешно.");
            }
            else
            {
                _isCaptchaVerified = false;
                MessageBoxHelper.Information("Капча не пройдена.");
            }
        }
    }
}