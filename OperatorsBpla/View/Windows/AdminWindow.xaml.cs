using Bpla.AppData;
using OperatorsBpla.View.Pages;
using System;
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
using System.Windows.Shapes;

namespace OperatorsBpla.View.Windows
{
    /// <summary>
    /// Логика взаимодействия для AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        public AdminWindow()
        {
            InitializeComponent();
            FrameHelper.selectedFrame = MainFrameAdmin;
            AdminUserPage adminUserPage = new AdminUserPage();
            MainFrameAdmin.Navigate(adminUserPage);
        }
        private void UserBtn_Click(object sender, RoutedEventArgs e)
        {
            AdminUserPage adminUserPage = new AdminUserPage();
            MainFrameAdmin.Navigate(adminUserPage);
        }
        private void UserLectureBtn_Click(object sender, RoutedEventArgs e)
        {
            AdminUserLecturePage adminUserLecturePage = new AdminUserLecturePage();
            MainFrameAdmin.Navigate(adminUserLecturePage);
        }
        private void QuestionUserBtn_Click(object sender, RoutedEventArgs e)
        {
            AdminQuestionUserPage adminQuestionUserPage = new AdminQuestionUserPage();
            MainFrameAdmin.Navigate(adminQuestionUserPage);
        }
        private void GoOutBtn_Click(object sender, RoutedEventArgs e)
        {
            AuthorisationWindow authorisationWindow = new AuthorisationWindow();
            authorisationWindow.Show();
            Close();
        }
    }
}