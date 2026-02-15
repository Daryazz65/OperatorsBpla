using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Data.Entity;
using OperatorsBpla.Model;
using Bpla.AppData;

namespace OperatorsBpla.View.Pages
{
    /// <summary>
    /// Логика взаимодействия для AdminUserLecturePage.xaml
    /// </summary>
    public partial class AdminUserLecturePage : Page
    {
        private DaryaEntities _context;

        public AdminUserLecturePage()
        {
            InitializeComponent();
            _context = App.GetContext();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                _context.Users.Load();
                _context.Lectures.Load();
                _context.UserLectures.Load();

                UserCb.ItemsSource = _context.Users.Local.ToList();
                LectureCb.ItemsSource = _context.Lectures.Local.ToList();
                UserLectureGrid.ItemsSource = _context.UserLectures.Local.ToBindingList();
            }
            catch (Exception ex)
            {
                MessageBoxHelper.Error(ex);
            }
        }

        private void UserLectureGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var sel = UserLectureGrid.SelectedItem as UserLecture;
            if (sel == null)
            {
                ClearForm();
                return;
            }

            UserCb.SelectedItem = _context.Users.Local.FirstOrDefault(u => u.Id == sel.IdUser);
            LectureCb.SelectedItem = _context.Lectures.Local.FirstOrDefault(l => l.Id == sel.IdLecture);
            IsCompletedChk.IsChecked = sel.IsCompleted;
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var user = UserCb.SelectedItem as User;
                var lecture = LectureCb.SelectedItem as Lecture;

                if (user == null || lecture == null)
                {
                    MessageBoxHelper.Warning("Выберите пользователя и лекцию.");
                    return;
                }

                var ul = new UserLecture
                {
                    IdUser = user.Id,
                    IdLecture = lecture.Id,
                    IsCompleted = IsCompletedChk.IsChecked == true
                };

                _context.UserLectures.Add(ul);
                _context.SaveChanges();

                UserLectureGrid.SelectedItem = ul;
                UserLectureGrid.ScrollIntoView(ul);
                MessageBoxHelper.Information("Запись добавлена.");
            }
            catch (Exception ex)
            {
                MessageBoxHelper.Error(ex);
            }
        }

        private void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            var sel = UserLectureGrid.SelectedItem as UserLecture;
            if (sel == null)
            {
                MessageBoxHelper.Warning("Выберите запись для изменения.");
                return;
            }

            try
            {
                var user = UserCb.SelectedItem as User;
                var lecture = LectureCb.SelectedItem as Lecture;

                if (user != null) sel.IdUser = user.Id;
                if (lecture != null) sel.IdLecture = lecture.Id;
                sel.IsCompleted = IsCompletedChk.IsChecked == true;

                _context.SaveChanges();
                UserLectureGrid.Items.Refresh();
                MessageBoxHelper.Information("Изменения сохранены.");
            }
            catch (Exception ex)
            {
                MessageBoxHelper.Error(ex);
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            var sel = UserLectureGrid.SelectedItem as UserLecture;
            if (sel == null)
            {
                MessageBoxHelper.Warning("Выберите запись для удаления.");
                return;
            }

            if (!MessageBoxHelper.Question($"Удалить запись Id={sel.Id}?"))
                return;

            try
            {
                _context.UserLectures.Remove(sel);
                _context.SaveChanges();
                MessageBoxHelper.Information("Запись удалена.");
            }
            catch (Exception ex)
            {
                MessageBoxHelper.Error(ex);
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _context.SaveChanges();
                MessageBoxHelper.Information("Все изменения сохранены.");
            }
            catch (Exception ex)
            {
                MessageBoxHelper.Error(ex);
            }
        }

        private void UserLectureGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                try
                {
                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    MessageBoxHelper.Error(ex);
                }
            }
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            UserCb.SelectedItem = null;
            LectureCb.SelectedItem = null;
            IsCompletedChk.IsChecked = false;
            UserLectureGrid.SelectedItem = null;
        }
    }
}
