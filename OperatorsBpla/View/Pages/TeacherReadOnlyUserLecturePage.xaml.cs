using OperatorsBpla.Model;
using System.Data.Entity;
using System.Windows.Controls;

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
            _context.Users.Load();
            _context.Lectures.Load();
            _context.UserLectures.Load();

            UserLectureGrid.ItemsSource = _context.UserLectures.Local.ToBindingList();
        }
    }
}
