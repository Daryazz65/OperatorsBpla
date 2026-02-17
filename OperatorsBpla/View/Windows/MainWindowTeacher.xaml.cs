using Bpla.AppData;
using OperatorsBpla.View.Pages;
using System.Windows;

namespace OperatorsBpla.View.Windows
{
    /// <summary>
    /// Логика взаимодействия для MainWindowTeacher.xaml
    /// </summary>
    public partial class MainWindowTeacher : Window
    {
        public MainWindowTeacher()
        {
            InitializeComponent();
            FrameHelper.selectedFrame = MainFrameTeacher;
            MainFrameTeacher.Navigate(new TeacherCrudLecturePage());
        }

        private void LectureBtn_Click(object sender, RoutedEventArgs e)
        {
            MainFrameTeacher.Navigate(new TeacherCrudLecturePage());
        }

        private void QuestionBtn_Click(object sender, RoutedEventArgs e)
        {
            MainFrameTeacher.Navigate(new TeacherCrudQuestionPage());
        }

        private void UserLectureBtn_Click(object sender, RoutedEventArgs e)
        {
            MainFrameTeacher.Navigate(new TeacherReadOnlyUserLecturePage());
        }

        private void QuestionUserBtn_Click(object sender, RoutedEventArgs e)
        {
            MainFrameTeacher.Navigate(new TeacherReadOnlyQuestionUserPage());
        }

        private void ProfileBtn_Click(object sender, RoutedEventArgs e)
        {
            MainFrameTeacher.Navigate(new ProfilePage());
        }

        private void GoOutBtn_Click(object sender, RoutedEventArgs e)
        {
            var auth = new AuthorisationWindow();
            auth.Show();
            Close();
        }
    }
}