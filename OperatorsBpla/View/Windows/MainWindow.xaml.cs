using Bpla.AppData;
using OperatorsBpla.View.Pages;
using OperatorsBpla.View.Windows;
using System.Windows;

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
            MainFrame.Navigate(new OperatorTestingPage());
        }

        private void TestingBtn_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new OperatorTestingPage());
        }

        private void LectionBtn_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new OperatorLectionPage());
        }

        private void ProfileBtn_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ProfilePage());
        }

        private void GoOutBtn_Click(object sender, RoutedEventArgs e)
        {
            var auth = new AuthorisationWindow();
            auth.Show();
            Close();
        }
    }
}
