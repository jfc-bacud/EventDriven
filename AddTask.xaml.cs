using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Eventing.Reader;
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
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace EventDriven
{
    /// <summary>
    /// Interaction logic for AddTask.xaml
    /// </summary>
    public partial class AddTask : Window
    {
        bool save = false;
        FileManager fileManager = new FileManager();
        public EventHandler WindowClosed;
        public AddTask()
        {
            InitializeComponent();
        }
        private void PopulateBoxes()
        {
            CategoryList();
            PriorityList();
        }
        private void CategoryList()
        {
            string[] files = Directory.GetFiles(fileManager.solutionDirectory);
            Category_Box.Items.Add("- Set Category -");
            Category_Box.SelectedIndex = 0; 

            for (int x = 0; x < files.Length; x++)
            {
                string item = System.IO.Path.GetFileName(files[x]).Trim().Replace(".txt", "");
                Category_Box.Items.Add(item);
            }
        }
        private void PriorityList()
        {
            Priority_Box.Items.Add("- Set Priority -");
            Priority_Box.Items.Add("High");
            Priority_Box.Items.Add("Medium");
            Priority_Box.Items.Add("Low");
            Priority_Box.SelectedIndex = 0;
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            WindowClosed?.Invoke(this, EventArgs.Empty);
            var mainWindow = Application.Current.MainWindow;
            mainWindow.WindowState = WindowState.Normal;
        }

        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Validate())
            {
                save = true;
                fileManager.Write(TaskName_TB.Text.ToString(), Category_Box.Text.ToString(),
                    Priority_Box.Text.ToString(), DatePicker.Text.ToString(), Description_TB.Text.ToString());

                this.Close();
            }
            else
            {
                MessageBox.Show("All fields must be filled out!");
            }
        }
        private bool Validate()
        {
            if (String.IsNullOrWhiteSpace(TaskName_TB.Text.ToString()))
                return false;

            if (Category_Box.SelectedIndex == 0)
                return false;

            if (Priority_Box.SelectedIndex == 0)
                return false;

            if (String.IsNullOrWhiteSpace(DatePicker.Text.ToString()))
                return false;

            if (String.IsNullOrWhiteSpace(Description_TB.ToString()))
                return false;

            return true;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PopulateBoxes();
        }
        private void AttemptClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!save)
            {
                MessageBoxResult result = MessageBox.Show("There are unconfirmed changes! Continue?", "Confirmation", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.No)
                {
                    e.Cancel = true;
                }
            }
        }
    }
}
