using Bpla.AppData;
using OperatorsBpla.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ModelType = OperatorsBpla.Model.Type;

namespace OperatorsBpla.View.Pages
{
    /// <summary>
    /// Логика взаимодействия для OperatorTestingPage.xaml
    /// </summary>
    public partial class OperatorTestingPage : Page
    {
        private DaryaEntities _context;
        private List<Question> _questions = new List<Question>();
        private List<List<string>> _options = new List<List<string>>();
        private List<int?> _selectedOptionIndex = new List<int?>(); 
        private int _currentIndex = -1;
        private int _correctCount = 0;
        private LevelType _currentLevelType;
        public OperatorTestingPage()
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
            LevelsPanel.Children.Clear();
        }
        private void TypeCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LevelsPanel.Children.Clear();
            var selectedType = TypeCb.SelectedItem as ModelType;
            if (selectedType == null) return;
            var levels = _context.LevelTypes.Local
                .Where(lt => lt.IdType == selectedType.Id)
                .Select(lt => lt.Level)
                .Distinct()
                .OrderBy(l => l.Id)
                .ToList();
            foreach (var level in levels)
            {
                var btn = new Button
                {
                    Content = level.Name,
                    Margin = new Thickness(6),
                    Width = 120,
                    Height = 36,
                    Tag = level
                };
                btn.Click += LevelBtn_Click;
                LevelsPanel.Children.Add(btn);
            }
        }
        private void LevelBtn_Click(object sender, RoutedEventArgs e)
        {
            var level = (Level)((Button)sender).Tag;
            var selectedType = TypeCb.SelectedItem as ModelType;
            if (selectedType == null) return;
            var lt = _context.LevelTypes.FirstOrDefault(x => x.IdLevel == level.Id && x.IdType == selectedType.Id);
            if (lt == null)
            {
                MessageBoxHelper.Warning("Для выбранного типа и уровня нет тестов.");
                return;
            }
            StartTest(lt);
        }
        private void StartTest(LevelType levelType)
        {
            _currentLevelType = levelType;
            _questions = _context.Questions
                .Where(q => q.IdLevelType == levelType.Id)
                .ToList();
            if (_questions == null || _questions.Count == 0)
            {
                MessageBoxHelper.Information("В этом уровне пока нет вопросов.");
                return;
            }
            var rnd = new Random();
            _questions = _questions.OrderBy(x => rnd.Next()).ToList();
            _options.Clear();
            foreach (var q in _questions)
            {
                var opts = new List<string> { q.CorrectAnswer ?? string.Empty, q.WrongAnswerOne ?? string.Empty, q.WrongAnswerTwo ?? string.Empty };
                opts = opts.OrderBy(x => rnd.Next()).ToList();
                _options.Add(opts);
            }
            _selectedOptionIndex = Enumerable.Repeat<int?>(null, _questions.Count).ToList();
            _currentIndex = 0;
            _correctCount = 0;
            WelcomePanel.Visibility = Visibility.Collapsed;
            ResultPanel.Visibility = Visibility.Collapsed;
            QuizPanel.Visibility = Visibility.Visible;
            PrevBtn.IsEnabled = false;
            RenderQuestion();
        }
        private void RenderQuestion()
        {
            if (_currentIndex < 0 || _currentIndex >= _questions.Count) return;
            var q = _questions[_currentIndex];
            QuestionNumberTb.Text = $"Вопрос {_currentIndex + 1} из {_questions.Count}";
            QuestionTextTb.Text = q.Question1 ?? string.Empty;
            AnswersPanel.Children.Clear();
            var opts = _options[_currentIndex];
            for (int i = 0; i < opts.Count; i++)
            {
                int idx = i;
                var rb = new RadioButton
                {
                    Content = opts[i],
                    GroupName = "Answers",
                    Margin = new Thickness(0, 6, 0, 6),
                    FontSize = 14,
                    Foreground = System.Windows.Media.Brushes.White,
                    Background = System.Windows.Media.Brushes.Transparent,
                    Tag = idx
                };
                rb.Checked += Option_Checked;
                if (_selectedOptionIndex[_currentIndex].HasValue && _selectedOptionIndex[_currentIndex].Value == idx)
                    rb.IsChecked = true;
                AnswersPanel.Children.Add(rb);
            }
            PrevBtn.IsEnabled = _currentIndex > 0;
            bool isLast = _currentIndex == _questions.Count - 1;
            if (isLast)
            {
                NextBtn.Visibility = Visibility.Collapsed;
                FinishBtn.Visibility = Visibility.Visible;
            }
            else
            {
                NextBtn.Visibility = Visibility.Visible;
                FinishBtn.Visibility = Visibility.Collapsed;
            }
            NextBtn.IsEnabled = !isLast;
        }
        private void Option_Checked(object sender, RoutedEventArgs e)
        {
            var rb = sender as RadioButton;
            if (rb == null) return;
            if (!(rb.Tag is int idx)) return;
            _selectedOptionIndex[_currentIndex] = idx;
        }
        private void PrevBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_currentIndex <= 0) return;
            _currentIndex--;
            RenderQuestion();
        }
        private void NextBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_currentIndex < _questions.Count - 1)
            {
                _currentIndex++;
                RenderQuestion();
            }
        }
        private void FinishBtn_Click(object sender, RoutedEventArgs e)
        {
            _correctCount = 0;
            for (int i = 0; i < _questions.Count; i++)
            {
                var selIndex = _selectedOptionIndex[i];
                if (!selIndex.HasValue) continue;
                var selectedText = _options[i][selIndex.Value];
                var correct = _questions[i].CorrectAnswer ?? string.Empty;
                bool ok = string.Equals(selectedText, correct, StringComparison.Ordinal);
                if (ok) _correctCount++;
                try
                {
                    if (App.CurrentUser != null)
                    {
                        var qid = _questions[i].Id;
                        var exists = _context.QuestionUsers.FirstOrDefault(qu => qu.IdQuestion == qid && qu.IdUser == App.CurrentUser.Id);
                        if (exists == null)
                        {
                            var qu = new QuestionUser
                            {
                                IdQuestion = qid,
                                IdUser = App.CurrentUser.Id,
                                Done = ok
                            };
                            _context.QuestionUsers.Add(qu);
                        }
                        else
                        {
                            exists.Done = ok;
                        }
                    }
                }
                catch
                {

                }
            }
            try
            {
                _context.SaveChanges();
            }
            catch
            {

            }
            EndTest();
        }
        private void EndTest()
        {
            QuizPanel.Visibility = Visibility.Collapsed;
            ResultPanel.Visibility = Visibility.Visible;
            ResultTb.Text = $"Вы ответили правильно на {_correctCount} из {_questions.Count} вопросов.";
            _questions.Clear();
            _options.Clear();
            _selectedOptionIndex.Clear();
            _currentIndex = -1;
            _currentLevelType = null;
        }
        private void CloseResultBtn_Click(object sender, RoutedEventArgs e)
        {
            ResultPanel.Visibility = Visibility.Collapsed;
            WelcomePanel.Visibility = Visibility.Visible;
        }
    }
}