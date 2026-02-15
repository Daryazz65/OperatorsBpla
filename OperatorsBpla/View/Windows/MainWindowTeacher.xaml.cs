using System;
using System.Windows;
using System.Windows.Controls;
using OperatorsBpla.View.Pages;
using Bpla.AppData;

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
            TeacherCrudLecturePage lecturePage = new TeacherCrudLecturePage();
            MainFrameTeacher.Navigate(lecturePage);
        }

        private void LectureBtn_Click(object sender, RoutedEventArgs e)
        {
            TeacherCrudLecturePage lecturePage = new TeacherCrudLecturePage();
            MainFrameTeacher.Navigate(lecturePage);
        }

        private void QuestionBtn_Click(object sender, RoutedEventArgs e)
        {
            TeacherCrudQuestionPage questionPage = new TeacherCrudQuestionPage();
            MainFrameTeacher.Navigate(questionPage);
        }

        private void UserLectureBtn_Click(object sender, RoutedEventArgs e)
        {
            TeacherReadOnlyUserLecturePage userLecturePage = new TeacherReadOnlyUserLecturePage();
            MainFrameTeacher.Navigate(userLecturePage);
        }

        private void QuestionUserBtn_Click(object sender, RoutedEventArgs e)
        {
            TeacherReadOnlyQuestionUserPage questionUserPage = new TeacherReadOnlyQuestionUserPage();
            MainFrameTeacher.Navigate(questionUserPage);
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
            MainFrameTeacher.Navigate(profilePage);
        }
    }
}
