using Bpla.AppData;
using OperatorsBpla.Model;
using System;
using System.Collections;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace OperatorsBpla.View.Pages
{
    /// <summary>
    /// Логика взаимодействия для AdminUserPage.xaml
    /// </summary>
    public partial class AdminUserPage : Page
    {
        private DaryaEntities _context;
        private IList _rolesLocalList; // коллекция ролей (если есть)
        public AdminUserPage()
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
                _context.UserLectures.Load();
                _context.QuestionUsers.Load();
                var rolesProp = _context.GetType().GetProperty("Roles");
                if (rolesProp != null)
                {
                    var rolesDbSet = rolesProp.GetValue(_context);
                    var loadMethod = rolesDbSet.GetType().GetMethod("Load");
                    loadMethod?.Invoke(rolesDbSet, null);

                    var localProp = rolesDbSet.GetType().GetProperty("Local");
                    _rolesLocalList = localProp?.GetValue(rolesDbSet) as IList;
                    if (_rolesLocalList != null)
                        RoleCb.ItemsSource = _rolesLocalList;
                }
                UserGrid.ItemsSource = _context.Users.Local.ToBindingList();
            }
            catch (Exception ex)
            {
                MessageBoxHelper.Error(ex);
            }
        }
        private void UserGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = UserGrid.SelectedItem as User;
            if (selected == null)
            {
                ClearForm();
                return;
            }
            LoginTb.Text = GetStringProp(selected, "Login");
            PasswordPb.Password = GetStringProp(selected, "Password");
            FullNameTb.Text = GetStringProp(selected, "Fullname");
            var reg = GetPropValue(selected, "RegistrationDate");
            if (reg is DateTime dt)
                RegistrationDateDp.SelectedDate = dt;
            else
                RegistrationDateDp.SelectedDate = null;
            var idRoleObj = GetPropValue(selected, "IdRole");
            if (idRoleObj != null && _rolesLocalList != null)
            {
                var idRole = Convert.ToInt32(idRoleObj);
                object found = null;
                foreach (var r in _rolesLocalList)
                {
                    var idProp = r.GetType().GetProperty("Id");
                    if (idProp == null) continue;
                    var val = idProp.GetValue(r);
                    if (val != null && Convert.ToInt32(val) == idRole)
                    {
                        found = r;
                        break;
                    }
                }
                RoleCb.SelectedItem = found;
            }
            else
            {
                RoleCb.SelectedItem = null;
            }
        }
        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(LoginTb.Text))
                {
                    MessageBoxHelper.Warning("Введите логин.");
                    return;
                }
                var u = new User();
                SetPropIfExists(u, "Login", LoginTb.Text.Trim());
                SetPropIfExists(u, "Fullname", FullNameTb.Text?.Trim());
                if (!string.IsNullOrEmpty(PasswordPb.Password))
                    SetPropIfExists(u, "Password", PasswordPb.Password);
                if (RegistrationDateDp.SelectedDate.HasValue)
                    SetPropIfExists(u, "RegistrationDate", RegistrationDateDp.SelectedDate.Value);
                else
                    SetPropIfExists(u, "RegistrationDate", DateTime.Now);
                if (RoleCb.SelectedItem != null)
                {
                    var roleId = GetPropValue(RoleCb.SelectedItem, "Id");
                    if (roleId != null)
                        SetPropIfExists(u, "IdRole", roleId);
                }
                _context.Users.Add(u);
                _context.SaveChanges();
                UserGrid.SelectedItem = u;
                UserGrid.ScrollIntoView(u);
                MessageBoxHelper.Information("Пользователь добавлен.");
            }
            catch (Exception ex)
            {
                MessageBoxHelper.Error(ex);
            }
        }
        private void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            var selected = UserGrid.SelectedItem as User;
            if (selected == null)
            {
                MessageBoxHelper.Warning("Выберите пользователя для изменения.");
                return;
            }
            try
            {
                if (string.IsNullOrWhiteSpace(LoginTb.Text))
                {
                    MessageBoxHelper.Warning("Логин не может быть пустым.");
                    return;
                }
                SetPropIfExists(selected, "Login", LoginTb.Text.Trim());
                if (!string.IsNullOrEmpty(PasswordPb.Password))
                    SetPropIfExists(selected, "Password", PasswordPb.Password);
                SetPropIfExists(selected, "Fullname", FullNameTb.Text?.Trim());
                if (RegistrationDateDp.SelectedDate.HasValue)
                    SetPropIfExists(selected, "RegistrationDate", RegistrationDateDp.SelectedDate.Value);
                if (RoleCb.SelectedItem != null)
                {
                    var roleId = GetPropValue(RoleCb.SelectedItem, "Id");
                    if (roleId != null)
                        SetPropIfExists(selected, "IdRole", roleId);
                }
                _context.SaveChanges();
                UserGrid.Items.Refresh();
                MessageBoxHelper.Information("Изменения сохранены.");
            }
            catch (Exception ex)
            {
                MessageBoxHelper.Error(ex);
            }
        }
        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            var selected = UserGrid.SelectedItem as User;
            if (selected == null)
            {
                MessageBoxHelper.Warning("Выберите пользователя для удаления.");
                return;
            }
            if (!MessageBoxHelper.Question($"Вы точно хотите удалить пользователя (Id={selected.Id}) и все связанные записи?"))
                return;
            try
            {
                var relatedUL = _context.UserLectures.Where(ul => ul.IdUser == selected.Id).ToList();
                foreach (var r in relatedUL) _context.UserLectures.Remove(r);
                var relatedQU = _context.QuestionUsers.Where(qu => qu.IdUser == selected.Id).ToList();
                foreach (var r in relatedQU) _context.QuestionUsers.Remove(r);
                _context.Users.Remove(selected);
                _context.SaveChanges();
                MessageBoxHelper.Information("Пользователь и связанные записи удалены.");
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
        private void UserGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
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
            LoginTb.Text = string.Empty;
            PasswordPb.Password = string.Empty;
            FullNameTb.Text = string.Empty;
            RegistrationDateDp.SelectedDate = null;
            RoleCb.SelectedItem = null;
            UserGrid.SelectedItem = null;
        }
        #region Reflection helpers (безопасно устанавливают/читают свойства, если они есть)
        private void SetPropIfExists(object obj, string propName, object value)
        {
            var prop = obj.GetType().GetProperty(propName);
            if (prop == null) return;
            try
            {
                if (value == null)
                {
                    if (prop.PropertyType.IsValueType)
                        return;
                    prop.SetValue(obj, null);
                    return;
                }
                var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                var safeValue = Convert.ChangeType(value, targetType);
                prop.SetValue(obj, safeValue);
            }
            catch
            {
                // Игнорируем, чтобы не ломать работу при несовпадении типов.
            }
        }
        private object GetPropValue(object obj, string propName)
        {
            var prop = obj.GetType().GetProperty(propName);
            if (prop == null) return null;
            try
            {
                return prop.GetValue(obj);
            }
            catch { return null; }
        }
        private string GetStringProp(object obj, string propName)
        {
            var v = GetPropValue(obj, propName);
            return v?.ToString() ?? string.Empty;
        }
        #endregion
    }
}