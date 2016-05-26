using GalaSoft.MvvmLight;
using RockSmithSongExplorer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RockSmithSongExplorer.ViewModel
{
    public class AddEditResult
    {
        public AddEditMode Mode { get; set; }
        public object DataItem { get; set; }
        public bool IsCanceled { get; set; }
    }

    public class EditLibraryPathViewModel : ViewModelBase,  ICompleteWithResult<AddEditResult>
    {
        readonly TaskCompletionSource<AddEditResult> _completedTaskSource = new TaskCompletionSource<AddEditResult>();
        AddEditMode _addEditMode;
        LibraryPath _originalItem;

        public Task<AddEditResult> Completed {  get { return _completedTaskSource.Task; } }

        public ICommand OKCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        public LibraryPath DataItem { get; private set; }


        public EditLibraryPathViewModel()
        {
            DataItem = new LibraryPath();
            _originalItem = null;
            _addEditMode = AddEditMode.AddNew;
        }

        public EditLibraryPathViewModel(LibraryPath existingItem)
        {
            DataItem = new LibraryPath();
            _originalItem = DataItem;
            _addEditMode = AddEditMode.Edit;
        }

        void CopyProperties(LibraryPath src, LibraryPath dest)
        {
            dest.Folder = src.Folder;
            dest.IncludeFilter = src.IncludeFilter;
            dest.RecurseSubFolders = src.RecurseSubFolders;
        }

        void DoOKCommand()
        {
            //TODO: Validate path, and filterpattern

            if (_addEditMode == AddEditMode.Edit)
            {
                CopyProperties(DataItem, _originalItem);
            }
            SetResult(false);
        }

        void DoCancelCommand()
        {
            SetResult(true);
        }

        private void SetResult(bool isCanceled)
        {
            var result = new AddEditResult() { DataItem = DataItem, IsCanceled = isCanceled, Mode = _addEditMode };
            _completedTaskSource.SetResult(result);
        }



    }

    public enum AddEditMode
    {
        AddNew=10,
        Edit=20
    }


}
