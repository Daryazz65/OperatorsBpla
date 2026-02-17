using Bpla.AppData;
using OperatorsBpla.Model;
using System;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace OperatorsBpla.View.Pages
{
    /// <summary>
    /// Логика взаимодействия для OperatorLectionPage.xaml
    /// </summary>
    public partial class OperatorLectionPage : Page
    {
        private DaryaEntities _context;
        public OperatorLectionPage()
        {
            InitializeComponent();
            _context = App.GetContext();
            LoadData();
        }
        private void LoadData()
        {
            _context.LevelTypes.Load();
            _context.Levels.Load();
            _context.Types.Load();
            _context.Lectures.Load();
            LectureList.ItemsSource = _context.Lectures.Local.ToBindingList();
            if (LectureList.Items.Count > 0)
                LectureList.SelectedIndex = 0;
        }
        private void LectureList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var lecture = LectureList.SelectedItem as Lecture;
            DisplayLecture(lecture);
        }
        private void DisplayLecture(Lecture lecture)
        {
            if (lecture == null)
            {
                TitleTb.Text = "—";
                TypeTb.Text = "—";
                LevelTb.Text = "—";
                TextCorrectTb.Text = string.Empty;
                FilePathTb.Text = "—";
                VideoUrlTb.Text = "—";
                OpenFileBtn.IsEnabled = false;
                OpenVideoBtn.IsEnabled = false;
                return;
            }
            TitleTb.Text = lecture.Title ?? "—";
            TextCorrectTb.Text = lecture.TextCorrect ?? string.Empty;
            var lt = lecture.LevelType;
            if (lt != null)
            {
                TypeTb.Text = lt.Type != null ? lt.Type.Name ?? "—" : "—";
                LevelTb.Text = lt.Level != null ? lt.Level.Name ?? "—" : "—";
            }
            else
            {
                TypeTb.Text = "—";
                LevelTb.Text = "—";
            }
            FilePathTb.Text = string.IsNullOrWhiteSpace(lecture.FilePath) ? "—" : lecture.FilePath;
            VideoUrlTb.Text = string.IsNullOrWhiteSpace(lecture.VideoUrl) ? "—" : lecture.VideoUrl;
            OpenFileBtn.IsEnabled = !string.IsNullOrWhiteSpace(lecture.FilePath);
            OpenVideoBtn.IsEnabled = !string.IsNullOrWhiteSpace(lecture.VideoUrl);
        }
        private void OpenFileBtn_Click(object sender, RoutedEventArgs e)
        {
            var lecture = LectureList.SelectedItem as Lecture;
            if (lecture == null || string.IsNullOrWhiteSpace(lecture.FilePath))
            {
                MessageBoxHelper.Warning("Файл не задан.");
                return;
            }
            var path = lecture.FilePath.Trim();
            try
            {
                if (!File.Exists(path))
                {
                    MessageBoxHelper.Warning("Файл не найден по пути: " + path);
                    return;
                }
                var psi = new ProcessStartInfo(path)
                {
                    UseShellExecute = true
                };
                Process.Start(psi);
            }
            catch (Exception ex)
            {
                MessageBoxHelper.Error(ex);
            }
        }
        private void OpenVideoBtn_Click(object sender, RoutedEventArgs e)
        {
            var lecture = LectureList.SelectedItem as Lecture;
            if (lecture == null || string.IsNullOrWhiteSpace(lecture.VideoUrl))
            {
                MessageBoxHelper.Warning("URL видео не задан.");
                return;
            }
            var url = lecture.VideoUrl.Trim();
            try
            {
                if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
                {
                    MessageBoxHelper.Warning("Неверный URL: " + url);
                    return;
                }
                var psi = new ProcessStartInfo(url)
                {
                    UseShellExecute = true
                };
                Process.Start(psi);
            }
            catch (Exception ex)
            {
                MessageBoxHelper.Error(ex);
            }
        }
    }
}