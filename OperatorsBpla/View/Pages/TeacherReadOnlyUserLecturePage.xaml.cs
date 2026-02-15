using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.Entity;
using OperatorsBpla.Model;

namespace OperatorsBpla.View.Pages
{
    /// <summary>
    /// Логика взаимодействия для TeacherReadOnlyUserLecturePage.xaml
    /// </summary>
    public partial class TeacherReadOnlyUserLecturePage : Page
    {
        private DaryaEntities _context;

        public TeacherReadOnlyUserLecturePage()
        {
            InitializeComponent();
            _context = App.GetContext();
            LoadData();
        }

        private void LoadData()
        {
            // Загружаем связанные сущности, чтобы навигационные свойства были доступны в привязке
            _context.Users.Load();
            _context.Lectures.Load();
            _context.UserLectures.Load();

            // Привязываем локальную коллекцию EF — всё read-only
            UserLectureGrid.ItemsSource = _context.UserLectures.Local.ToBindingList();
        }
    }
}
