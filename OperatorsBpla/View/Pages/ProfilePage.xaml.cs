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
    /// Логика взаимодействия для ProfilePage.xaml
    /// </summary>
    public partial class ProfilePage : Page
    {
        private DaryaEntities _context;
        private User _user;

        public ProfilePage()
        {
            InitializeComponent();
            _context = App.GetContext();
            LoadProfile();
        }

        private void LoadProfile()
        {
            try
            {
                if (App.CurrentUser == null)
                {
                    MessageBoxHelper.Warning("Пользователь не авторизован.");
                    return;
                }

                // Подгружаем свежую информацию с навигацией Role
                _user = _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefault(u => u.Id == App.CurrentUser.Id);

                if (_user == null)
                {
                    MessageBoxHelper.Warning("Пользователь не найден в базе.");
                    return;
                }

                // Устанавливаем DataContext для биндингов в XAML
                DataContext = _user;

                // Если пользователь — преподаватель, не показываем/не считаем счетчики
                bool isTeacher = _user.Role != null &&
                                 (string.Equals(_user.Role.Name, "Teacher", StringComparison.OrdinalIgnoreCase) ||
                                  string.Equals(_user.Role.Name, "Учитель", StringComparison.OrdinalIgnoreCase));

                if (isTeacher)
                {
                    // Скрываем панели со счетчиками для роли преподавателя
                    CompletedBorder.Visibility = Visibility.Collapsed;
                    AnsweredBorder.Visibility = Visibility.Collapsed;
                }
                else
                {
                    // Показываем панели и заполняем счетчики для остальных ролей
                    CompletedBorder.Visibility = Visibility.Visible;
                    AnsweredBorder.Visibility = Visibility.Visible;

                    var completed = _context.UserLectures.Count(ul => ul.IdUser == _user.Id && ul.IsCompleted);
                    var answered = _context.QuestionUsers.Count(qu => qu.IdUser == _user.Id && qu.Done);

                    CompletedCountTb.Text = completed.ToString();
                    AnsweredCountTb.Text = answered.ToString();
                }

                // Инициализируем аватар (инициалы)
                AvatarTb.Text = MakeInitials(_user.Fullname);
            }
            catch (Exception ex)
            {
                MessageBoxHelper.Error(ex);
            }
        }

        private string MakeInitials(string fullname)
        {
            if (string.IsNullOrWhiteSpace(fullname))
                return "?";

            var parts = fullname.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1)
                return parts[0].Substring(0, Math.Min(1, parts[0].Length)).ToUpperInvariant();

            var first = parts[0].Substring(0, 1).ToUpperInvariant();
            var second = parts.Length > 1 ? parts[1].Substring(0, 1).ToUpperInvariant() : string.Empty;
            return (first + second).Trim();
        }
    }
}
