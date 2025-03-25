using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        FileManager fileManager = new FileManager();
        AddTask newTask;
        AddCategory newCategory;
        int current = -1;
        bool editMode = false;
        public MainWindow()
        {
            InitializeComponent();
            RenewCategoryList();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ShowAll();
        }
        private DataTable CreateTables(int index)
        {
            string[] files = fileManager.GetFiles();
            DataTable returnTable;

            if (index == -1)
                returnTable = fileManager.RecordAll();

            else
                returnTable = fileManager.Read(files[index]);


            return returnTable;
        }
        private void SetDataGrid()
        {
            DataGridCheckBoxColumn checkBoxColumn = new DataGridCheckBoxColumn
            {
                Header = "Finished",
                Binding = new System.Windows.Data.Binding("IsFinished")
            };

            DataGridComboBoxColumn comboBoxColumn = new DataGridComboBoxColumn
            {
                Header = "Priority"
            };
            List<string> priorities = new List<string> { "Low", "Medium", "High" };
            comboBoxColumn.ItemsSource = priorities;
            Binding binding = new Binding("Urgency");
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            binding.Mode = BindingMode.TwoWay;
            comboBoxColumn.SelectedItemBinding = binding;

            if (!CheckFinished())
            {
                ListTable.Columns.Insert(0, checkBoxColumn);
                ListTable.Columns[6].Visibility = System.Windows.Visibility.Collapsed;
            }
            if (!CheckPriority())
            {
                ListTable.Columns.Insert(4, comboBoxColumn);         
            }
            else
            {
                ListTable.Columns.RemoveAt(1);
                ListTable.Columns.Insert(4, comboBoxColumn);
            }

            foreach (DataGridColumn column in ListTable.Columns)
            {
                if (column.Header.ToString() == "Finished")
                {
                    continue;
                }
                column.Width = new DataGridLength(130);
            }

            ListTable.Columns[6].Visibility = System.Windows.Visibility.Collapsed;
            ListTable.Columns[7].Visibility = System.Windows.Visibility.Collapsed;
            Trigger();
        }
        private void Trigger()
        {
            if (ListTable.Items.Count <= 0)
            {
                EditRow_Button.IsEnabled = false;
                DeleteRow_Button.IsEnabled = false;
            }
            else
            {
                EditRow_Button.IsEnabled = true;
                DeleteRow_Button.IsEnabled = true;
            }
        }
        private void RenewCategoryList()
        {
            string[] files = fileManager.GetFiles();
            Button[] buttons = new Button[files.Length + 2];
            Style buttonStyle = (Style)FindResource("RoundedButtonStyle");

            for (int x = 0; x <= buttons.Length - 1; x++)
            {
                if (x == 0)
                {
                    buttons[x] = new Button();
                    buttons[x].Name = "AllTasks_Button";
                    buttons[x].Content = "All Tasks";
                    buttons[x].Style = buttonStyle;
                    buttons[x].HorizontalAlignment = HorizontalAlignment.Left;
                    buttons[x].VerticalAlignment = VerticalAlignment.Top;
                    buttons[x].Click += AllTasks_Button_Click;
                    buttons[x].MouseEnter += Button_OnHover;
                    buttons[x].MouseLeave += Button_OnLeave;
                }
                else if (x == buttons.Length - 1)
                {
                    buttons[x] = new Button();
                    buttons[x].Name = "AddCategory_Button";
                    buttons[x].Content = "Add Category";
                    buttons[x].Style = buttonStyle;
                    buttons[x].HorizontalAlignment = HorizontalAlignment.Left;
                    buttons[x].VerticalAlignment = VerticalAlignment.Top;
                    buttons[x].Click += AddCategory_Button_Click;
                    buttons[x].MouseEnter += Button_OnHover;
                    buttons[x].MouseLeave += Button_OnLeave;
                }
                else
                {
                    string fileName = System.IO.Path.GetFileName(files[x - 1]).Trim().Replace(".txt", "");
                    string buttonName = fileName.Replace(" ", "");

                    buttons[x] = new Button();
                    buttons[x].Name = buttonName + "_Button";
                    buttons[x].Content = fileName;
                    buttons[x].Style = buttonStyle;
                    buttons[x].HorizontalAlignment = HorizontalAlignment.Left;
                    buttons[x].VerticalAlignment = VerticalAlignment.Top;
                    buttons[x].Click += ButtonCategory_OnClick;
                    buttons[x].MouseEnter += Button_OnHover;
                    buttons[x].MouseLeave += Button_OnLeave;

                }
                ListCategory.Items.Add(buttons[x]);
            }

            if (ListCategory.Items.Count >= 3)
            {
                ManipulateRow_Button.IsEnabled = true;
            }
            else
            {
                ManipulateRow_Button.IsEnabled = false;
            }
        }
        private void ShowAll()
        {
            DataTable viewTable = CreateTables(-1);
            ListTable.ItemsSource = viewTable.DefaultView;
            SetDataGrid();
        }
        private void ButtonCategory_OnClick(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;
            Table_Lbl.Content = clickedButton.Name.ToString().Replace("_Button", "");
            DeleteCategory_Button.IsEnabled = true;
            current = (ListCategory.Items.IndexOf(clickedButton)) - 1;

            DataTable viewTable = CreateTables(current);
            ListTable.ItemsSource = viewTable.DefaultView;
            SetDataGrid();
        }
        private bool CheckFinished()
        {
            foreach (DataGridColumn column in ListTable.Columns)
            {
                if (column.Header.ToString() == "Finished")
                {
                    return true;
                }
            }
            return false;
        }
        private bool CheckPriority()
        {
            foreach (DataGridColumn column in ListTable.Columns)
            {
                if (column.Header.ToString() == "Priority")
                {
                    return true;
                }
            }
            return false;
        }

        private void AllTasks_Button_Click(object sender, RoutedEventArgs e)
        {
            Table_Lbl.Content = "All Tasks";
            current = -1;
            DeleteCategory_Button.IsEnabled = false;
            ShowAll();
        }
        private void ManipulateRow_Button_Click(object sender, RoutedEventArgs e)
        {
            if (newTask == null || !newTask.IsVisible)
            {
                newTask = new AddTask();
                newTask.WindowClosed += RefreshTable;
                newTask.Owner = this;
                newTask.ShowDialog();
            }
        }
        private void RefreshTable(object sender, EventArgs e)
        {
            DataTable viewTable = CreateTables(current);
            ListTable.ItemsSource = viewTable.DefaultView;
            SetDataGrid();
        }
        private void RefreshSelection(object sender, EventArgs e)
        {
            ListCategory.Items.Clear();
            RenewCategoryList();
        }
        private void AddCategory_Button_Click(object sender, RoutedEventArgs e)
        {
            if (newCategory == null || !newCategory.IsVisible)
            {
                newCategory = new AddCategory();
                newCategory.WindowClosed += RefreshSelection;
                newCategory.Owner = this;
                newCategory.ShowDialog();
            }
        }
        private void RefreshWhole()
        {
            ListCategory.Items.Clear();
            RenewCategoryList();
        }
        private void DeleteCategory_Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete?", "Confirmation", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                string[] files = fileManager.GetFiles();
                fileManager.Delete(files[current]);
                RefreshWhole();
            }
            else
            {
                return;
            }

        }
        private void Button_OnHover(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            button.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF417F9A"));
        }
        private void Button_OnLeave(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            button.Background = new SolidColorBrush(Colors.White);
        }
        private void EditRow_Button_Click(object sender, RoutedEventArgs e)
        {
            if (!editMode)
            {
                editMode = true;
                ListTable.IsReadOnly = false;
                ListTable.Columns[2].IsReadOnly = true;
                ListTable.Columns[3].IsReadOnly = true;
                ListTable.CanUserAddRows = false;
                Status.Content = "On";
                Status.Foreground = new SolidColorBrush(Colors.Green);
            }
            else
            {
                editMode = false;
                ListTable.IsReadOnly = true;
                ListTable.CanUserAddRows = false;
                Status.Content = "Off";
                Status.Foreground = new SolidColorBrush(Colors.Red);
                FixDates();


                if (current == -1)
                {
                    DataTable updatedTable = ((DataView)ListTable.ItemsSource).ToTable();
                    fileManager.UpdateTask(updatedTable);

                }
                else
                {
                    string[] files = fileManager.GetFiles();
                    string fileName = files[current];
                    DataTable updatedTable = ((DataView)ListTable.ItemsSource).ToTable();
                    fileManager.UpdateTask(fileName, updatedTable);
                }
            }
        }
        private void FixDates()
        {
            foreach (DataRowView row in ListTable.Items)
            {
                string temp = row["Due Date"].ToString().Replace("-", "/");
                row["Due Date"] = temp;

            }
        }
        private void ListTable_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.Column.Header == "Priority")
            {
                string selectedValue = ((ComboBox)e.EditingElement).SelectedItem as string;
                DataRowView rowView = (DataRowView)e.Row.Item;
                rowView["Urgency"] = selectedValue;
            }

        }
        private void DeleteRow_Button_Click(object sender, RoutedEventArgs e)
        {
            {
                DataTable updatedTable = ((DataView)ListTable.ItemsSource).ToTable();
                fileManager.UpdateTask(updatedTable);

                if (ListTable.SelectedItem is DataRowView selectedRow)
                {
                    string taskName = selectedRow["Task Name"].ToString();
                    string category = selectedRow["Category"].ToString();

                    MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete '{taskName}'?", "Confirmation", MessageBoxButton.YesNo);

                    if (result == MessageBoxResult.Yes)
                    {
                        if(current == -1)
                        {
                            fileManager.DeleteTask(taskName, updatedTable);

                        }


                        fileManager.DeleteTask(fileName, taskName);
                        RefreshTable(null, null); // Refresh the table after deletion
                    }
                }
                else
                {
                    MessageBox.Show("Please select a task to delete.");
                }
            }
        }
    }
}
