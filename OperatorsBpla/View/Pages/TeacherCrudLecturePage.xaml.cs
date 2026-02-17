using Bpla.AppData;
using Microsoft.Win32;
using OperatorsBpla.Model;
using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ModelType = OperatorsBpla.Model.Type;

namespace OperatorsBpla.View.Pages
{
    /// <summary>
    /// Логика взаимодействия для TeacherCrudLecturePage.xaml
    /// </summary>
    public partial class TeacherCrudLecturePage : Page
    {
        private DaryaEntities _context;
        public TeacherCrudLecturePage()
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
            _context.Lectures.Load();
            TypeCb.ItemsSource = _context.Types.Local.ToList();
            LevelCb.ItemsSource = null;
            LectureGrid.ItemsSource = _context.Lectures.Local.ToBindingList();
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
        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(TitleTb.Text))
                {
                    MessageBoxHelper.Warning("Введите название уровня.");
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
                var lecture = new Lecture
                {
                    Title = TitleTb.Text.Trim(),
                    TextCorrect = TextCorrectTb.Text ?? string.Empty,
                    FilePath = string.IsNullOrWhiteSpace(FilePathTb.Text) ? null : FilePathTb.Text.Trim(),
                    VideoUrl = string.IsNullOrWhiteSpace(VideoUrlTb.Text) ? null : VideoUrlTb.Text.Trim(),
                    IdLevelType = levelType.Id
                };
                _context.Lectures.Add(lecture);
                _context.SaveChanges();
                LectureGrid.SelectedItem = lecture;
                LectureGrid.ScrollIntoView(lecture);
                MessageBoxHelper.Information("Новый уровень обучения добавлен.");
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBoxHelper.Error(ex);
            }
        }
        private void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedLecture = LectureGrid.SelectedItem as Lecture;
            if (selectedLecture == null)
            {
                MessageBoxHelper.Warning("Выберите запись для изменения.");
                return;
            }
            try
            {
                if (string.IsNullOrWhiteSpace(TitleTb.Text))
                {
                    MessageBoxHelper.Warning("Название не может быть пустым.");
                    return;
                }
                selectedLecture.Title = TitleTb.Text.Trim();
                selectedLecture.TextCorrect = TextCorrectTb.Text ?? string.Empty;
                selectedLecture.FilePath = string.IsNullOrWhiteSpace(FilePathTb.Text) ? null : FilePathTb.Text.Trim();
                selectedLecture.VideoUrl = string.IsNullOrWhiteSpace(VideoUrlTb.Text) ? null : VideoUrlTb.Text.Trim();
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
                selectedLecture.IdLevelType = levelType.Id;
                _context.SaveChanges();
                LectureGrid.Items.Refresh();
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
            var selected = LectureGrid.SelectedItem as Lecture;
            if (selected == null)
            {
                MessageBoxHelper.Warning("Выберите запись для удаления.");
                return;
            }
            if (!MessageBoxHelper.Question($"Вы точно хотите удалить уровень \"{selected.Title}\"?"))
                return;
            try
            {
                _context.Lectures.Remove(selected);
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
        private void LectureGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = LectureGrid.SelectedItem as Lecture;
            if (selected == null)
            {
                ClearForm();
                return;
            }
            TitleTb.Text = selected.Title;
            TextCorrectTb.Text = selected.TextCorrect;
            FilePathTb.Text = selected.FilePath;
            VideoUrlTb.Text = selected.VideoUrl;
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
        private void LectureGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
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
        private void ChooseFileBtn_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.Filter = "Все файлы|*.*";
            if (dlg.ShowDialog() == true)
            {
                FilePathTb.Text = dlg.FileName;
            }
        }
        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }
        private void ClearForm()
        {
            TitleTb.Text = string.Empty;
            TextCorrectTb.Text = string.Empty;
            FilePathTb.Text = string.Empty;
            VideoUrlTb.Text = string.Empty;
            NewLevelTb.Text = string.Empty;
            TypeCb.SelectedItem = null;
            LevelCb.ItemsSource = null;
            LectureGrid.SelectedItem = null;
        }
    }
}