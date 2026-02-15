using Bpla.AppData;
using OperatorsBpla.View.Pages;
using OperatorsBpla.View.Windows;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OperatorsBpla
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            FrameHelper.selectedFrame = MainFrame;
            OperatorTestingPage operatorTestingPage = new OperatorTestingPage();
            MainFrame.Navigate(operatorTestingPage);
        }

        private void TestingBtn_Click(object sender, RoutedEventArgs e)
        {
            OperatorTestingPage operatorTestingPage = new OperatorTestingPage();
            MainFrame.Navigate(operatorTestingPage);
        }

        private void LectionBtn_Click(object sender, RoutedEventArgs e)
        {
            OperatorLectionPage OperatorlectionPage = new OperatorLectionPage();
            MainFrame.Navigate(OperatorlectionPage);
        }

        private void GoOutBtn_Click(object sender, RoutedEventArgs e)
        {
            AuthorisationWindow authorisationWindow = new AuthorisationWindow();
            authorisationWindow.Show();
            Close();
        }

        private void ProfileBtn_Click(object sender, RoutedEventArgs e)
        {
            ProfilePage profilePage = new ProfilePage();
            MainFrame.Navigate(profilePage);
        }
    }
}
