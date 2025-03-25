using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;

namespace EventDriven
{
    /// <summary>
    /// Interaction logic for AddCategory.xaml
    /// </summary>
    public partial class AddCategory : Window
    {
        FileManager fileManager = new FileManager();
        public EventHandler WindowClosed;
        public AddCategory()
        {
            InitializeComponent();
        }

        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {

            if (File.Exists(System.IO.Path.Combine(fileManager.solutionDirectory, CategoryName_TB.Text.ToString())))
            {
                MessageBox.Show("Category already exists!");
            }
            else
            {
                fileManager.Write(CategoryName_TB.Text.ToString());
                this.Close();
            }
  
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            WindowClosed?.Invoke(this, EventArgs.Empty);
            var mainWindow = Application.Current.MainWindow;
            mainWindow.WindowState = WindowState.Normal;
        }
    }
}
