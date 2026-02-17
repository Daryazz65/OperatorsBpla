using System.Windows;

namespace OperatorsBpla.View.Windows
{
    /// <summary>
    /// Логика взаимодействия для CustomMessageBox.xaml
    /// </summary>
    public partial class CustomMessageBox : Window
    {
        public CustomMessageBox(string message, string title)
        {
            InitializeComponent();
            MessageTextBlock.Text = message;
            Title = title;
        }
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
        public static void Show(string message, string title = "Сообщение")
        {
            CustomMessageBox messageBox = new CustomMessageBox(message, title);
            messageBox.ShowDialog();
        }
    }
}