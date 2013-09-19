using System;
using System.ComponentModel;
using Xceed.Wpf.Toolkit;

namespace Vasya.Utilities
{
    internal class BusyIndicatorManager
    {
        private static BusyIndicator _busyIndicator;

        public static void Init(BusyIndicator busyIndicator)
        {
            _busyIndicator = busyIndicator;
        }

        public static void ShowDuringAction(Action action, string message)
        {
            var worker = new BackgroundWorker();
            worker.DoWork += (o, ea) => action();
            worker.RunWorkerCompleted += (o, ea) => _busyIndicator.IsBusy = false;
            _busyIndicator.BusyContent = message;
            _busyIndicator.IsBusy = true;
            worker.RunWorkerAsync();
        }
    }
}