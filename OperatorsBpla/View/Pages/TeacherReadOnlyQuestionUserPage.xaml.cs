using OperatorsBpla.Model;
using System.Data.Entity;
using System.Windows.Controls;

namespace OperatorsBpla.View.Pages
{
    /// <summary>
    /// Логика взаимодействия для TeacherReadOnlyQuestionUserPage.xaml
    /// </summary>
    public partial class TeacherReadOnlyQuestionUserPage : Page
    {
        private DaryaEntities _context;

        public TeacherReadOnlyQuestionUserPage()
        {
            InitializeComponent();
            _context = App.GetContext();
            LoadData();
        }

        private void LoadData()
        {
            _context.Users.Load();
            _context.Questions.Load();
            _context.QuestionUsers.Load();

            QuestionUserGrid.ItemsSource = _context.QuestionUsers.Local.ToBindingList();
        }
    }
}
