using OperatorsBpla.Model;
using System.Windows;

namespace OperatorsBpla
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static User CurrentUser { get; set; }
        private static DaryaEntities _context;
        public static DaryaEntities GetContext()
        {
            if (_context == null)
            {
                _context = new DaryaEntities();
            }
            return _context;
        }
    }
}
