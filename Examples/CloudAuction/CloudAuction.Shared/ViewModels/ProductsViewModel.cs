using System;
using System.Collections.ObjectModel;
using QuickCross;
using System.Threading.Tasks;

namespace CloudAuction.Shared.ViewModels
{
    public class ProductsViewModel : ViewModelBase
    {
        public ProductsViewModel()
        {
            // TODO: pass any services that this model needs as contructor parameters. 
        }

        #region Data-bindable properties and commands
        public ObservableCollection<ProductViewModel> ProductList /* One-way data-bindable property generated with propdbcol snippet. Keep on one line - see http://goo.gl/Yg6QMd for why. */ { get { return _ProductList; } protected set { if (_ProductList != value) { _ProductList = value; RaisePropertyChanged(PROPERTYNAME_ProductList); UpdateProductListHasItems(); } } } private ObservableCollection<ProductViewModel> _ProductList; public const string PROPERTYNAME_ProductList = "ProductList";
        public bool ProductListHasItems /* One-way data-bindable property generated with propdbcol snippet. Keep on one line - see http://goo.gl/Yg6QMd for why. */ { get { return _ProductListHasItems; } protected set { if (_ProductListHasItems != value) { _ProductListHasItems = value; RaisePropertyChanged(PROPERTYNAME_ProductListHasItems); } } } private bool _ProductListHasItems; public const string PROPERTYNAME_ProductListHasItems = "ProductListHasItems";
        protected void UpdateProductListHasItems() /* Helper method generated with propdbcol snippet. Keep on one line - see http://goo.gl/Yg6QMd for why. */ { ProductListHasItems = _ProductList != null && _ProductList.Count > 0; }
        public RelayCommand SelectProductCommand /* Data-bindable command with parameter that calls SelectProduct(), generated with cmdp snippet. Keep on one line - see http://goo.gl/Yg6QMd for why. */ { get { if (_SelectProductCommand == null) _SelectProductCommand = new RelayCommand(SelectProduct); return _SelectProductCommand; } } private RelayCommand _SelectProductCommand;
        #endregion

        private void SelectProduct(object parameter)
        {
            var product = (ProductViewModel)parameter;
            // Note that viewmodels should not be passed as parameters, because they are bound to 
            CloudAuctionApplication.Instance.ContinueToProduct(product);
        }
    }
}

// Design-time data support
namespace CloudAuction.Shared.ViewModels.Design
{
    public class ProductsViewModelDesign : ProductsViewModel
    {
        private static Uri[] productInfos = new Uri[] { new Uri("http://cloudauction.macaw.nl/Auction/Lot/1"), new Uri("http://cloudauction.macaw.nl/Auction/Lot/2"), new Uri("http://cloudauction.macaw.nl/Auction/Lot/3"), new Uri("http://cloudauction.macaw.nl/Auction/Lot/4"), new Uri("http://cloudauction.macaw.nl/Auction/Lot/5") };

        public ProductsViewModelDesign()
        {
            ProductList = new ObservableCollection<ProductViewModel>();
            for (int i = 0; i < 1000; i++) ProductList.Add(new ProductViewModelDesign { ProductInfo = productInfos[i % productInfos.Length] });
            var _ = LowerPrices();
        }

        private async Task LowerPrices()
        {
            for (;;)
            {
                await Task.Delay(1000);
                foreach (var product in ProductList) product.ListPriceNumeric--;
            }
        }
    }
}
