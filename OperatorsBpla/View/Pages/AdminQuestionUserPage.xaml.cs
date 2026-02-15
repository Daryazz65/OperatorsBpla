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
    /// Логика взаимодействия для AdminQuestionUserPage.xaml
    /// </summary>
    public partial class AdminQuestionUserPage : Page
    {
        private DaryaEntities _context;

        public AdminQuestionUserPage()
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
                _context.Questions.Load();
                _context.QuestionUsers.Load();

                UserCb.ItemsSource = _context.Users.Local.ToList();
                QuestionCb.ItemsSource = _context.Questions.Local.ToList();
                QuestionUserGrid.ItemsSource = _context.QuestionUsers.Local.ToBindingList();
            }
            catch (Exception ex)
            {
                MessageBoxHelper.Error(ex);
            }
        }

        private void QuestionUserGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var sel = QuestionUserGrid.SelectedItem as QuestionUser;
            if (sel == null)
            {
                ClearForm();
                return;
            }

            UserCb.SelectedItem = _context.Users.Local.FirstOrDefault(u => u.Id == sel.IdUser);
            QuestionCb.SelectedItem = _context.Questions.Local.FirstOrDefault(q => q.Id == sel.IdQuestion);
            DoneChk.IsChecked = sel.Done;
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var user = UserCb.SelectedItem as User;
                var question = QuestionCb.SelectedItem as Question;

                if (user == null || question == null)
                {
                    MessageBoxHelper.Warning("Выберите пользователя и вопрос.");
                    return;
                }

                var qu = new QuestionUser
                {
                    IdUser = user.Id,
                    IdQuestion = question.Id,
                    Done = DoneChk.IsChecked == true
                };

                _context.QuestionUsers.Add(qu);
                _context.SaveChanges();

                QuestionUserGrid.SelectedItem = qu;
                QuestionUserGrid.ScrollIntoView(qu);
                MessageBoxHelper.Information("Запись добавлена.");
            }
            catch (Exception ex)
            {
                MessageBoxHelper.Error(ex);
            }
        }

        private void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            var sel = QuestionUserGrid.SelectedItem as QuestionUser;
            if (sel == null)
            {
                MessageBoxHelper.Warning("Выберите запись для изменения.");
                return;
            }

            try
            {
                var user = UserCb.SelectedItem as User;
                var question = QuestionCb.SelectedItem as Question;

                if (user != null) sel.IdUser = user.Id;
                if (question != null) sel.IdQuestion = question.Id;
                sel.Done = DoneChk.IsChecked == true;

                _context.SaveChanges();
                QuestionUserGrid.Items.Refresh();
                MessageBoxHelper.Information("Изменения сохранены.");
            }
            catch (Exception ex)
            {
                MessageBoxHelper.Error(ex);
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            var sel = QuestionUserGrid.SelectedItem as QuestionUser;
            if (sel == null)
            {
                MessageBoxHelper.Warning("Выберите запись для удаления.");
                return;
            }

            if (!MessageBoxHelper.Question($"Удалить запись Id={sel.Id}?"))
                return;

            try
            {
                _context.QuestionUsers.Remove(sel);
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

        private void QuestionUserGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
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
            QuestionCb.SelectedItem = null;
            DoneChk.IsChecked = false;
            QuestionUserGrid.SelectedItem = null;
        }
    }
}
