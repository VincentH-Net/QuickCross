#if TEMPLATE // To add a new viewmodel class: in the Visual Studio Package Manager Console (menu View | Other Windows), enter "New-ViewModel <viewmodel name>". Alternatively: copy this file, then in the copy remove the enclosing #if TEMPLATE ... #endif lines and replace _VIEWMODEL_ with the viewmodel name.
using System;
using QuickCross;

namespace CloudAuction.Shared.ViewModels
{
    public class _VIEWNAME_ViewModel : ViewModelBase
    {
        public _VIEWNAME_ViewModel()
        {
            // TODO: Pass any services that the _VIEWNAME_ viewmodel needs as contructor parameters. 
        }

        #region Data-bindable properties and commands
        // TODO: Generate _VIEWNAME_ viewmodel data-bindable properties and commands here with prop* and cmd* code snippets
        
        // Example data-bound property and command:
        public int Count /* One-way data-bindable property generated with propdb1 snippet. Keep on one line - see http://goo.gl/Yg6QMd for why. */ { get { return _Count; } set { if (_Count != value) { _Count = value; RaisePropertyChanged(PROPERTYNAME_Count); } } } private int _Count; public const string PROPERTYNAME_Count = "Count";
        public RelayCommand IncreaseCountCommand /* Data-bindable command that calls IncreaseCount(), generated with cmd snippet. Keep on one line - see http://goo.gl/Yg6QMd for why. */ { get { if (_IncreaseCountCommand == null) _IncreaseCountCommand = new RelayCommand(IncreaseCount); return _IncreaseCountCommand; } } private RelayCommand _IncreaseCountCommand; public const string COMMANDNAME_IncreaseCountCommand = "IncreaseCountCommand";
        #endregion

        private void IncreaseCount() { Count++; } // Example command method
    }
}

// Design-time data support
#if DEBUG
namespace CloudAuction.Shared.ViewModels.Design
{
    public class _VIEWNAME_ViewModelDesign : _VIEWNAME_ViewModel
    {
        public _VIEWNAME_ViewModelDesign()
        {
            // TODO: Initialize the _VIEWNAME_ viewmodel with hardcoded design-time data
        }
    }
}
#endif

#endif // TEMPLATE