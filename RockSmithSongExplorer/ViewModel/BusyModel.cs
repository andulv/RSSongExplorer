using GalaSoft.MvvmLight;
using RockSmithSongExplorer.Services;
using GalaSoft.MvvmLight.Messaging;
using RockSmithSongExplorer.Models;
using RocksmithToolkitLib.Xml;
using System.Windows.Threading;
using System;
using System.Threading;
using System.Collections.Generic;
using RockSmithSongExplorer.Controls;
using System.Linq;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Command;

namespace RockSmithSongExplorer.ViewModel
{
    public class BusyModel : ViewModelBase
    {
        private readonly Stack<string> _stackedBusyTexts = new Stack<string>();
        private string _busyText;

        public string BusyText
        {
            get { return _busyText; }
            private set { Set(() => BusyText, ref _busyText, value); }
        }
        public bool IsBusy { get { return !string.IsNullOrEmpty(BusyText); } }

        public IDisposable BeginBusyOperation(string busyText)
        {
            if (!string.IsNullOrEmpty(_busyText))
                _stackedBusyTexts.Push(_busyText);

            BusyText = busyText;

            var dispAction = new DisposableAction(() => {
                if (BusyText != _busyText)
                    throw new InvalidOperationException("Invalid sequence in completion of busy operations");

                if (_stackedBusyTexts.Any())
                    BusyText = _stackedBusyTexts.Pop();

                else
                    BusyText = null;
            });

            return dispAction;
        }

        private class DisposableAction : IDisposable
        {
            readonly Action _action;
            public DisposableAction(Action action)
            {
                _action = action;
            }

            public void Dispose()
            {
                _action();
            }
        }
    }
}
