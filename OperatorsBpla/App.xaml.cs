using OperatorsBpla.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
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
