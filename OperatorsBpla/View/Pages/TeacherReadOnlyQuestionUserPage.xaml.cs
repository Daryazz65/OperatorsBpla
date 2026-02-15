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
            // Загружаем связанные сущности для корректного отображения навигационных свойств
            _context.Users.Load();
            _context.Questions.Load();
            _context.QuestionUsers.Load();

            QuestionUserGrid.ItemsSource = _context.QuestionUsers.Local.ToBindingList();
        }
    }
}
