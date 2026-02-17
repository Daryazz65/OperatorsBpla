using Bpla.AppData;
using Microsoft.Win32;
using OperatorsBpla.Model;
using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace OperatorsBpla.View.Pages
{
    public partial class ProfilePage : Page
    {
        private readonly DaryaEntities _context;
        private User _user;
        private readonly PropertyInfo _photoProp;
        public ProfilePage()
        {
            InitializeComponent();
            _context = App.GetContext();
            _photoProp = typeof(User).GetProperty("Photo");
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
                _user = _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefault(u => u.Id == App.CurrentUser.Id);
                if (_user == null)
                {
                    MessageBoxHelper.Warning("Пользователь не найден в базе.");
                    return;
                }
                DataContext = _user;
                var isTeacher = _user.Role != null &&
                                (string.Equals(_user.Role.Name, "Teacher", StringComparison.OrdinalIgnoreCase) ||
                                 string.Equals(_user.Role.Name, "Учитель", StringComparison.OrdinalIgnoreCase));
                CompletedBorder.Visibility = isTeacher ? Visibility.Collapsed : Visibility.Visible;
                AnsweredBorder.Visibility = isTeacher ? Visibility.Collapsed : Visibility.Visible;
                if (!isTeacher)
                {
                    var completed = _context.UserLectures.Count(ul => ul.IdUser == _user.Id && ul.IsCompleted);
                    var answered = _context.QuestionUsers.Count(qu => qu.IdUser == _user.Id && qu.Done);
                    CompletedCountTb.Text = completed.ToString();
                    AnsweredCountTb.Text = answered.ToString();
                }
                AvatarTb.Text = MakeInitials(_user.Fullname);
                UpdatePhotoFromModel();
            }
            catch (Exception ex)
            {
                MessageBoxHelper.Error(ex);
            }
        }
        private void UpdatePhotoFromModel()
        {
            if (_photoProp == null)
            {
                PhotoImg.Visibility = Visibility.Collapsed;
                AvatarTb.Visibility = Visibility.Visible;
                DeletePhotoBtn.Visibility = Visibility.Collapsed;
                AvatarBgEllipse.Visibility = Visibility.Visible;
                return;
            }
            var bytes = _photoProp.GetValue(_user) as byte[];
            if (bytes != null && bytes.Length > 0)
            {
                var img = ByteArrayToImageSource(bytes);
                if (img != null)
                {
                    PhotoImg.Source = img;
                    PhotoImg.Visibility = Visibility.Visible;
                    AvatarTb.Visibility = Visibility.Collapsed;
                    AvatarBgEllipse.Visibility = Visibility.Collapsed;
                    DeletePhotoBtn.Visibility = Visibility.Visible;
                    return;
                }
            }
            PhotoImg.Source = null;
            PhotoImg.Visibility = Visibility.Collapsed;
            AvatarTb.Visibility = Visibility.Visible;
            AvatarBgEllipse.Visibility = Visibility.Visible;
            DeletePhotoBtn.Visibility = Visibility.Collapsed;
        }
        private BitmapImage ByteArrayToImageSource(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) return null;
            try
            {
                var bi = new BitmapImage();
                using (var ms = new MemoryStream(bytes))
                {
                    bi.BeginInit();
                    bi.CacheOption = BitmapCacheOption.OnLoad;
                    bi.StreamSource = ms;
                    bi.EndInit();
                    bi.Freeze();
                }
                return bi;
            }
            catch
            {
                return null;
            }
        }
        private void ChangePhotoBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dlg = new OpenFileDialog
                {
                    Filter = "Image Files (*.jpg;*.jpeg;*.png;*.bmp;*.gif)|*.jpg;*.jpeg;*.png;*.bmp;*.gif",
                    Title = "Выберите фотографию профиля"
                };
                if (dlg.ShowDialog() != true) return;
                var bytes = File.ReadAllBytes(dlg.FileName);
                if (_photoProp == null)
                {
                    MessageBoxHelper.Warning("Поле Photo не найдено в модели User. Обновите EDMX/модель.");
                    return;
                }
                _photoProp.SetValue(_user, bytes);
                _context.SaveChanges();
                UpdatePhotoFromModel();
                MessageBoxHelper.Information("Фотография профиля обновлена.");
            }
            catch (Exception ex)
            {
                MessageBoxHelper.Error(ex);
            }
        }
        private void DeletePhotoBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_photoProp == null)
                {
                    MessageBoxHelper.Warning("Поле Photo не найдено в модели User. Обновите EDMX/модель.");
                    return;
                }
                var current = _photoProp.GetValue(_user) as byte[];
                if (current == null || current.Length == 0)
                {
                    DeletePhotoBtn.Visibility = Visibility.Collapsed;
                    MessageBoxHelper.Warning("Фотография отсутствует.");
                    return;
                }
                if (!MessageBoxHelper.Question("Вы действительно хотите удалить фотографию профиля?")) return;
                _photoProp.SetValue(_user, null);
                _context.SaveChanges();
                UpdatePhotoFromModel();
                MessageBoxHelper.Information("Фотография удалена.");
            }
            catch (Exception ex)
            {
                MessageBoxHelper.Error(ex);
            }
        }
        private string MakeInitials(string fullname)
        {
            if (string.IsNullOrWhiteSpace(fullname)) return "?";
            var parts = fullname.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1) return parts[0].Substring(0, Math.Min(1, parts[0].Length)).ToUpperInvariant();
            var first = parts[0].Substring(0, 1).ToUpperInvariant();
            var second = parts.Length > 1 ? parts[1].Substring(0, 1).ToUpperInvariant() : string.Empty;
            return (first + second).Trim();
        }
    }
}