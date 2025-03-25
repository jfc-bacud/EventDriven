using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
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

namespace EventDriven
{
    internal class FileManager
    {
        public string solutionDirectory = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\", @"EventDriven\Data\"));
        // Assuming your folder is inside the solution directory (e.g., SolutionFolder/TargetFolder)

        public string[] GetFiles()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(solutionDirectory);
            FileInfo[] files = directoryInfo.GetFiles();

            for (int i = 0; i < files.Length - 1; i++)
            {
                for (int j = 0; j < files.Length - 1 - i; j++)
                {
                    // Compare the creation time of the two files
                    if (files[j].CreationTime > files[j + 1].CreationTime)
                    {
                        // Swap the files if they are in the wrong order
                        FileInfo temp = files[j];
                        files[j] = files[j + 1];
                        files[j + 1] = temp;
                    }
                }
            }

            string[] fileNames = new string[files.Length];
            
            for (int x = 0; x < fileNames.Length; x++)
                fileNames[x] = files[x].Name;

            return fileNames;

        }


        public DataTable RecordAll()
        {
            DataTable masterTable = new DataTable();

            string[] files = Directory.GetFiles(solutionDirectory);

            masterTable.Columns.Add("Task Name", typeof(string));
            masterTable.Columns.Add("Category", typeof(string));
            masterTable.Columns.Add("Due Date", typeof(string));
            masterTable.Columns.Add("Description", typeof(string));
            masterTable.Columns.Add("IsFinished", typeof(bool));
            masterTable.Columns.Add("Urgency", typeof(string));

            foreach (string file in files)
            {
                DataTable fileDataTable = Read(file);

                foreach (DataRow row in fileDataTable.Rows)
                {
                    masterTable.ImportRow(row);
                }
            }

            return masterTable;
        }

        public DataTable Read(string file)
        {
            DataTable dataTable = new DataTable();
            using (StreamReader sr = new StreamReader(Path.Combine(solutionDirectory, file)))
            {
                string line;
                bool isFirstLine = true;

                while ((line = sr.ReadLine()) != null)
                {
                    var rowValues = line.Split(',');

                    if (isFirstLine)
                    {
                        foreach (var column in rowValues)
                        {
                            if (column == "IsFinished")
                            {
                                dataTable.Columns.Add(column.Trim(), typeof(bool));
                            }
                            else
                                dataTable.Columns.Add(column.Trim());
                        }
                        isFirstLine = false;
                    }
                    else
                    {
                        DataRow row = dataTable.NewRow();
                        for (int i = 0; i < rowValues.Length; i++)
                        {
                            row[i] = rowValues[i].Trim();
                        }
                        dataTable.Rows.Add(row);
                    }
                }
            }
            return dataTable;
        }

        public void Write(string name, string category, string priority, string date, string description)
        {
            using (StreamWriter sw = new StreamWriter(Path.Combine(solutionDirectory, (category + ".txt")), true))
            {
                sw.BaseStream.Seek(0, SeekOrigin.End);
                sw.Write($"\n{name},{category},{priority},{date},{description},false");
            }
        }

        public void Write(string nameCategory)
        {
            using (StreamWriter sw = new StreamWriter(Path.Combine(solutionDirectory, (nameCategory + ".txt"))))
            {
                sw.Write($"Task Name,Category,Due Date,Description,IsFinished,Urgency");
            }
        }

        public void Delete(string file)
        {
            File.Delete(Path.Combine(solutionDirectory, file));
        }
    }
}
