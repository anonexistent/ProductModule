using EfiritPro.Retail.ProductModule.Models;
using System.Text.Json.Serialization;

namespace EfiritPro.Retail.ProductModule.OutputContracts
{
    public  class OutputProductSet : OutputProductItem
    {
        //[JsonPropertyName("includedProductsInSet")]
        //public ICollection<OutputProductItem> IncludedProducts { get; set; } = Array.Empty<OutputProductItem>();
        //public OutputProductSet(Product product) : base(product)
        //{   
        //    var tempList = product.includingProducts;
        //    IncludedProducts = tempList.Select(x => new OutputProductItem(x.IncludedProduct)).ToArray();
        //    //foreach (var item in tempList)
        //    //{
        //    //    IncludedProducts.Add(new(item.IncludedProduct));
        //    //}

        //}

        public OutputProductSet(Product product) : base(product) { }
    }
}
