using System;
using QuickCross;

namespace CloudAuction.Shared.ViewModels
{
    public class ProductViewModel : ViewModelBase
    {
        public ProductViewModel()
        {
            // TODO: pass any services that this model needs as contructor parameters. 
        }

        /* Fields:
        public string Name, Description;
        public string ListPrice;
        public int ListPriceNumeric;
        public Uri ProductInfo;
        */

        #region Data-bindable properties and commands
        // TODO: Generate data-bindable properties and commands here with prop* and cmd* code snippets
        public string Name /* One-way data-bindable property generated with propdb1 snippet. Keep on one line - see http://goo.gl/Yg6QMd for why. */ { get { return _Name; } set { if (_Name != value) { _Name = value; RaisePropertyChanged(PROPERTYNAME_Name); } } } private string _Name; public const string PROPERTYNAME_Name = "Name";
        public string Description /* One-way data-bindable property generated with propdb1 snippet. Keep on one line - see http://goo.gl/Yg6QMd for why. */ { get { return _Description; } set { if (_Description != value) { _Description = value; RaisePropertyChanged(PROPERTYNAME_Description); } } } private string _Description; public const string PROPERTYNAME_Description = "Description";
        public int ListPriceNumeric /* Two-way data-bindable property that calls custom code in OnListPriceNumericChanged() from setter, generated with propdb2c snippet. Keep on one line - see http://goo.gl/Yg6QMd for why. */ { get { return _ListPriceNumeric; } set { if (_ListPriceNumeric != value) { _ListPriceNumeric = value; RaisePropertyChanged(PROPERTYNAME_ListPriceNumeric); OnListPriceNumericChanged(); } } } private int _ListPriceNumeric; public const string PROPERTYNAME_ListPriceNumeric = "ListPriceNumeric";
        public string ListPrice /* One-way data-bindable property generated with propdb1 snippet. Keep on one line - see http://goo.gl/Yg6QMd for why. */ { get { return _ListPrice; } set { if (_ListPrice != value) { _ListPrice = value; RaisePropertyChanged(PROPERTYNAME_ListPrice); } } } private string _ListPrice; public const string PROPERTYNAME_ListPrice = "ListPrice";
        public Uri ProductInfo /* One-way data-bindable property generated with propdb1 snippet. Keep on one line - see http://goo.gl/Yg6QMd for why. */ { get { return _ProductInfo; } set { if (_ProductInfo != value) { _ProductInfo = value; RaisePropertyChanged(PROPERTYNAME_ProductInfo); } } } private Uri _ProductInfo; public const string PROPERTYNAME_ProductInfo = "ProductInfo";
        #endregion

        private void OnListPriceNumericChanged()
        {
            ListPrice = string.Format("$ {0},00", ListPriceNumeric);
        }

        public override string ToString()
        {
            return string.Format("{0}\r\n$ {1},00\r\n{2}", Name, ListPriceNumeric, Description);
        }

        public void Initialize(ProductViewModel product)
        {
            if (product == null) return;
            Name = product.Name;
            Description = product.Description;
            ListPriceNumeric = product.ListPriceNumeric;
            ProductInfo = product.ProductInfo;
        }
    }
}

// Design-time data support
#if DEBUG
namespace CloudAuction.Shared.ViewModels.Design
{
    public class ProductViewModelDesign : ProductViewModel
    {
        private static int nr = 1;

        public ProductViewModelDesign()
        {
            Name = "Product Name " + nr.ToString();
            Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec id placerat nisi. Phasellus scelerisque vestibulum lorem eget aliquam. Nunc quis.";
            ListPriceNumeric = 240 + nr++;
        }
    }
}
#endif

