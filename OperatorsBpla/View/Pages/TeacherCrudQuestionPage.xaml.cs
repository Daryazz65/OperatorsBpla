using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using OperatorsBpla.Model;
using System.Data.Entity;
using Bpla.AppData;
using ModelType = OperatorsBpla.Model.Type;

namespace OperatorsBpla.View.Pages
{
    /// <summary>
    /// Логика взаимодействия для TeacherCrudQuestionPage.xaml
    /// </summary>
    public partial class TeacherCrudQuestionPage : Page
    {
        private DaryaEntities _context;

        public TeacherCrudQuestionPage()
        {
            InitializeComponent();
            _context = App.GetContext();
            LoadData();
        }

        private void LoadData()
        {
            _context.Types.Load();
            _context.Levels.Load();
            _context.LevelTypes.Load();
            _context.Questions.Load();

            TypeCb.ItemsSource = _context.Types.Local.ToList();
            LevelCb.ItemsSource = null;

            QuestionGrid.ItemsSource = _context.Questions.Local.ToBindingList();
        }

        private void TypeCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedType = TypeCb.SelectedItem as ModelType;
            if (selectedType == null)
            {
                LevelCb.ItemsSource = null;
                return;
            }

            var levels = _context.LevelTypes.Local
                .Where(lt => lt.IdType == selectedType.Id)
                .Select(lt => lt.Level)
                .Distinct()
                .ToList();

            LevelCb.ItemsSource = levels;
        }

        // Reflection helpers — в сгенерированных классах текст вопроса может называться Question1 или Question
        private string GetQuestionText(Question q)
        {
            if (q == null) return null;
            var t = typeof(Question);
            var p = t.GetProperty("Question1") ?? t.GetProperty("Question");
            return p?.GetValue(q) as string;
        }
        private void SetQuestionText(Question q, string value)
        {
            if (q == null) return;
            var t = typeof(Question);
            var p = t.GetProperty("Question1") ?? t.GetProperty("Question");
            p?.SetValue(q, value);
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(QuestionTb.Text))
                {
                    MessageBoxHelper.Warning("Введите текст вопроса.");
                    return;
                }

                var selectedType = TypeCb.SelectedItem as ModelType;
                if (selectedType == null)
                {
                    MessageBoxHelper.Warning("Выберите тип сложности (Type).");
                    return;
                }

                Level lvl = null;
                if (!string.IsNullOrWhiteSpace(NewLevelTb.Text) && NewLevelTb.Text.Trim() != "Введите имя нового уровня")
                {
                    lvl = new Level { Name = NewLevelTb.Text.Trim() };
                    _context.Levels.Add(lvl);
                    _context.SaveChanges();
                }
                else
                {
                    lvl = LevelCb.SelectedItem as Level;
                    if (lvl == null)
                    {
                        MessageBoxHelper.Warning("Выберите существующий уровень или введите новый.");
                        return;
                    }
                }

                var levelType = _context.LevelTypes.FirstOrDefault(lt => lt.IdLevel == lvl.Id && lt.IdType == selectedType.Id);
                if (levelType == null)
                {
                    levelType = new LevelType { IdLevel = lvl.Id, IdType = selectedType.Id };
                    _context.LevelTypes.Add(levelType);
                    _context.SaveChanges();
                    _context.LevelTypes.Load();
                }

                var question = new Question
                {
                    WrongAnswerOne = WrongOneTb.Text ?? string.Empty,
                    WrongAnswerTwo = WrongTwoTb.Text ?? string.Empty,
                    CorrectAnswer = CorrectTb.Text ?? string.Empty,
                    IdLevelType = levelType.Id
                };
                SetQuestionText(question, QuestionTb.Text.Trim());

                _context.Questions.Add(question);
                _context.SaveChanges();

                QuestionGrid.SelectedItem = question;
                QuestionGrid.ScrollIntoView(question);
                MessageBoxHelper.Information("Вопрос добавлен.");
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBoxHelper.Error(ex);
            }
        }

        private void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            var selected = QuestionGrid.SelectedItem as Question;
            if (selected == null)
            {
                MessageBoxHelper.Warning("Выберите запись для изменения.");
                return;
            }

            try
            {
                if (string.IsNullOrWhiteSpace(QuestionTb.Text))
                {
                    MessageBoxHelper.Warning("Текст вопроса не может быть пустым.");
                    return;
                }

                selected.WrongAnswerOne = WrongOneTb.Text ?? string.Empty;
                selected.WrongAnswerTwo = WrongTwoTb.Text ?? string.Empty;
                selected.CorrectAnswer = CorrectTb.Text ?? string.Empty;
                SetQuestionText(selected, QuestionTb.Text.Trim());

                var selectedType = TypeCb.SelectedItem as ModelType;
                if (selectedType == null)
                {
                    MessageBoxHelper.Warning("Выберите тип сложности (Type).");
                    return;
                }

                Level lvl = null;
                if (!string.IsNullOrWhiteSpace(NewLevelTb.Text) && NewLevelTb.Text.Trim() != "Введите имя нового уровня")
                {
                    lvl = new Level { Name = NewLevelTb.Text.Trim() };
                    _context.Levels.Add(lvl);
                    _context.SaveChanges();
                }
                else
                {
                    lvl = LevelCb.SelectedItem as Level;
                    if (lvl == null)
                    {
                        MessageBoxHelper.Warning("Выберите существующий уровень или введите новый.");
                        return;
                    }
                }

                var levelType = _context.LevelTypes.FirstOrDefault(lt => lt.IdLevel == lvl.Id && lt.IdType == selectedType.Id);
                if (levelType == null)
                {
                    levelType = new LevelType { IdLevel = lvl.Id, IdType = selectedType.Id };
                    _context.LevelTypes.Add(levelType);
                    _context.SaveChanges();
                    _context.LevelTypes.Load();
                }

                selected.IdLevelType = levelType.Id;

                _context.SaveChanges();
                QuestionGrid.Items.Refresh();
                MessageBoxHelper.Information("Изменения сохранены.");
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBoxHelper.Error(ex);
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            var selected = QuestionGrid.SelectedItem as Question;
            if (selected == null)
            {
                MessageBoxHelper.Warning("Выберите запись для удаления.");
                return;
            }

            if (!MessageBoxHelper.Question($"Вы точно хотите удалить вопрос?"))
                return;

            try
            {
                _context.Questions.Remove(selected);
                _context.SaveChanges();
                MessageBoxHelper.Information("Запись удалена.");
                ClearForm();
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

        private void QuestionGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = QuestionGrid.SelectedItem as Question;
            if (selected == null)
            {
                ClearForm();
                return;
            }

            QuestionTb.Text = GetQuestionText(selected) ?? string.Empty;
            WrongOneTb.Text = selected.WrongAnswerOne ?? string.Empty;
            WrongTwoTb.Text = selected.WrongAnswerTwo ?? string.Empty;
            CorrectTb.Text = selected.CorrectAnswer ?? string.Empty;
            NewLevelTb.Text = string.Empty;

            if (selected.LevelType != null)
            {
                var lt = selected.LevelType;
                TypeCb.SelectedItem = _context.Types.Local.FirstOrDefault(t => t.Id == lt.IdType);
                TypeCb_SelectionChanged(null, null);
                LevelCb.SelectedItem = _context.Levels.Local.FirstOrDefault(l => l.Id == lt.IdLevel);
            }
            else
            {
                TypeCb.SelectedItem = null;
                LevelCb.ItemsSource = null;
            }
        }

        private void QuestionGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
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
            QuestionTb.Text = string.Empty;
            WrongOneTb.Text = string.Empty;
            WrongTwoTb.Text = string.Empty;
            CorrectTb.Text = string.Empty;
            NewLevelTb.Text = string.Empty;
            TypeCb.SelectedItem = null;
            LevelCb.ItemsSource = null;
            QuestionGrid.SelectedItem = null;
        }
    }
}
