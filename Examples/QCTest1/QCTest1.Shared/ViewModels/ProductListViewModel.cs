using System;
using QuickCross;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace QCTest1.Shared.ViewModels
{
    public class ProductListViewModel : ViewModelBase
    {
        public ProductListViewModel()
        {
            // TODO: Pass any services that the ProductList viewmodel needs as contructor parameters. 
        }

        #region Data-bindable properties and commands
        // TODO: Generate ProductList viewmodel data-bindable properties and commands here with prop* and cmd* code snippets
        public ObservableCollection<ProductViewModel> ProductList /* One-way data-bindable property generated with propdbcol snippet. Keep on one line - see http://goo.gl/Yg6QMd for why. */ { get { return _ProductList; } set { if (_ProductList != value) { _ProductList = value; RaisePropertyChanged(PROPERTYNAME_ProductList); UpdateProductListHasItems(); } } } private ObservableCollection<ProductViewModel> _ProductList; public const string PROPERTYNAME_ProductList = "ProductList";
        public bool ProductListHasItems /* One-way data-bindable property generated with propdbcol snippet. Keep on one line - see http://goo.gl/Yg6QMd for why. */ { get { return _ProductListHasItems; } protected set { if (_ProductListHasItems != value) { _ProductListHasItems = value; RaisePropertyChanged(PROPERTYNAME_ProductListHasItems); } } } private bool _ProductListHasItems; public const string PROPERTYNAME_ProductListHasItems = "ProductListHasItems";
        protected void UpdateProductListHasItems() /* Helper method generated with propdbcol snippet. Keep on one line - see http://goo.gl/Yg6QMd for why. */ { ProductListHasItems = _ProductList != null && _ProductList.Count > 0; }
        
        #endregion

    }
}

// Design-time data support
#if DEBUG
namespace QCTest1.Shared.ViewModels.Design
{
    public class ProductListViewModelDesign : ProductListViewModel
    {
        public ProductListViewModelDesign()
        {
            // TODO: Initialize the ProductList viewmodel with hardcoded design-time data
            ProductList = new ObservableCollection<ProductViewModel>();
            for (int i = 1; i <= 100; i++) ProductList.Add(new ProductViewModelDesign());
            var _ = DecreaseStock();
        }

        private async Task DecreaseStock()
        {
            for (; ; )
            {
                await Task.Delay(1000);
                foreach (var product in ProductList) product.Stock--;
            }
        }
    }
}
#endif
