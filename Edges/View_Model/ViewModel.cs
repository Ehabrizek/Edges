using Edges.Logic;
using System;
using System.Data.OleDb;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace Edges.View_Model
{
    class ViewModel : ViewModelBase
    {
        #region Private Members
        string _excelFilePath = @"C:\Folder\Subfolder\ExcelFile.xls";
        string _sheetName = "By Pathway";
        string _geneIdHeader = "Gene Id";
        string _pathwayDescriptionHeader = "Pathway Description";
        bool _isDisabled = false;
        private long _progressBarMaximum = 100;
        private long _progressBarValue = 0;
        private bool _progressBarVisible = false;
        #endregion

        #region Properties
        public string ExcelFilePath { get { return _excelFilePath; } set { _excelFilePath = value; OnPropertyChanged(); } }

        public string SheetName { get { return _sheetName; } set { _sheetName = value; OnPropertyChanged(); } }

        public string GeneIdHeader { get { return _geneIdHeader; } set { _geneIdHeader = value; OnPropertyChanged(); } }

        public string PathwayDescriptionHeader { get { return _pathwayDescriptionHeader; } set { _pathwayDescriptionHeader = value; OnPropertyChanged(); } }

        public bool IsDisabled { get { return _isDisabled; } set { _isDisabled = value; OnPropertyChanged(); } }

        public long ProgressBarMaximum { get { return _progressBarMaximum; } set { _progressBarMaximum = value; OnPropertyChanged(); } }

        public long ProgressBarValue { get { return _progressBarValue; } set { _progressBarValue = value; OnPropertyChanged(); } }

        public bool ProgressBarVisible { get { return _progressBarVisible; } set { _progressBarVisible = value; OnPropertyChanged(); } }
        #endregion

        #region Construction
        public ViewModel()
        {
            ProcessButton_Click = new DelegateCommand(Process);
        }
        #endregion

        #region Commands
        public ICommand ProcessButton_Click { get; }
        #endregion

        #region Private Methods
        async void Process()
        {
            if (string.IsNullOrWhiteSpace(ExcelFilePath) || string.IsNullOrWhiteSpace(SheetName) || string.IsNullOrWhiteSpace(GeneIdHeader)
                || string.IsNullOrWhiteSpace(PathwayDescriptionHeader))
            {
                MessageBox.Show("Please make sure all fields are filled out!");
                return;
            }

            if (!File.Exists(ExcelFilePath))
            {
                MessageBox.Show("The Excel file does not exist at that location!");
                return;
            }

            IsDisabled = true;
            ProgressBarVisible = true;

            try
            {
                CombinationProcessor processor = new CombinationProcessor();
                await Task.Run(() => processor.WriteCombinations(ExcelFilePath, SheetName, GeneIdHeader, PathwayDescriptionHeader, IncrementProgressBar, SetProgressBarMax));
                MessageBox.Show("Processing complete! The generated text file can be found where this program is located.");
            }
            catch (OleDbException e)
            {
                if (e.Message.StartsWith("No value given"))
                {
                    MessageBox.Show("One of the values entered for the headers is incorrect!");
                }
                else if (e.Message.Contains("is not a valid name"))
                {
                    MessageBox.Show("The value entered for the sheet name is incorrect!");
                }
                else
                {
                    MessageBox.Show(e.Message);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            IsDisabled = false;
            ProgressBarValue = 0;
            ProgressBarVisible = false;
        }

        void SetProgressBarMax(long maximum)
        {
            ProgressBarMaximum = maximum;
        }

        void IncrementProgressBar()
        {
            MethodInvoker mI = () => ProgressBarValue++;
            //lets update the progressbar on the UI thread and not the processor thread. 
            Application.Current.Dispatcher.Invoke(mI);
        }
        #endregion
    }
}
