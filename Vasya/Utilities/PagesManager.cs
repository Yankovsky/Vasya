using System;
using System.Windows.Controls;
using Vasya.Views;

namespace Vasya.Utilities
{
    public static class PagesManager
    {
        private static Panel _mainContainer;
        private static readonly UserControl SelectFilePage = new SelectFilePage();
        private static readonly UserControl WorkAreaPage = new WorkAreaPage();

        public enum Page
        {
            SelectFilePage,
            WorkAreaPage
        }

        public static void Init(Panel mainContainer)
        {
            _mainContainer = mainContainer;
        }

        public static void SwitchPage(Page page, object dataContext)
        {
            _mainContainer.Children.Clear();
            switch (page)
            {
                case Page.SelectFilePage:
                    _mainContainer.Children.Add(SelectFilePage);
                    break;
                case Page.WorkAreaPage:
                    _mainContainer.Children.Add(WorkAreaPage);
                    break;
                default:
                    throw new Exception();
            }
            _mainContainer.DataContext = dataContext;
        }
    }
}