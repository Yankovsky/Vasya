using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using Vasya.Utilities;

namespace Vasya.Views
{
    public partial class SelectFilePage : UserControl
    {
        public SelectFilePage()
        {
            InitializeComponent();
        }

        private void SelectFileClicked(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
                {
                    FileName = "Document",
                    DefaultExt = ".txt",
                    Filter = "Text documents (.txt)|*.txt"
                };
            if (dlg.ShowDialog() == true)
            {
                var logic = new Logic(dlg.FileName);
                BusyIndicatorManager.ShowDuringAction(() =>
                    {
                        logic.DoWork();
                        Application.Current.Dispatcher.Invoke((Action) (() =>
                            {
                                var vasyaVm = new VasyaVM(logic);
                                PagesManager.SwitchPage(PagesManager.Page.WorkAreaPage, vasyaVm);
                            }));
                    }, "Reading file and creating image representation");
            }
        }
    }
}