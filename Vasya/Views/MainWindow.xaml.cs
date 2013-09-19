using System.Windows;
using Vasya.Utilities;

namespace Vasya.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            PagesManager.Init(MainContainer);
            BusyIndicatorManager.Init(AppWideBusyIndicator);
            PagesManager.SwitchPage(PagesManager.Page.SelectFilePage, null);
        }
    }
}