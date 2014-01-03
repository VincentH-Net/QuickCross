using System;
using QuickCross;

namespace QCTest1.Shared.ViewModels
{
    public class ProductViewModel : ViewModelBase
    {
        public ProductViewModel()
        {
            // TODO: Pass any services that the Product viewmodel needs as contructor parameters. 
        }

        #region Data-bindable properties and commands
        // TODO: Generate Product viewmodel data-bindable properties and commands here with prop* and cmd* code snippets
        
        // Example data-bound property and command:
        public string Name /* One-way data-bindable property generated with propdb1 snippet. Keep on one line - see http://goo.gl/Yg6QMd for why. */ { get { return _Name; } set { if (_Name != value) { _Name = value; RaisePropertyChanged(PROPERTYNAME_Name); } } } private string _Name; public const string PROPERTYNAME_Name = "Name";
        public int Stock /* One-way data-bindable property generated with propdb1 snippet. Keep on one line - see http://goo.gl/Yg6QMd for why. */ { get { return _Stock; } set { if (_Stock != value) { _Stock = value; RaisePropertyChanged(PROPERTYNAME_Stock); } } } private int _Stock; public const string PROPERTYNAME_Stock = "Stock";
        #endregion

    }
}

// Design-time data support
#if DEBUG
namespace QCTest1.Shared.ViewModels.Design
{
    public class ProductViewModelDesign : ProductViewModel
    {
        private static Random r = new Random();
        private static int id = 0;

        public ProductViewModelDesign()
        {
            // TODO: Initialize the Product viewmodel with hardcoded design-time data
            Name = "Product " + (++id).ToString();
            Stock = r.Next(100, 1000);
        }
    }
}
#endif
