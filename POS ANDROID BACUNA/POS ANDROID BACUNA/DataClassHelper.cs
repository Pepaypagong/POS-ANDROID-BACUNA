using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using POS_ANDROID_BACUNA.Data_Classes;

namespace POS_ANDROID_BACUNA
{
    public class DataClassHelper
    {
        //coverts the data from product to new product and viceversa
        public static List<ProductsModel> ToProduct(List<NewProduct> _newProduct)
        {
            var retVal = new List<ProductsModel>();

            foreach (var item in _newProduct)
            {
                retVal.Add(new ProductsModel
                {
                    Id = item.ProductId,
                    ProductName = item.ProductName,
                    ParentProductId = item.ParentProductId,
                    ProductCategoryId = item.ProductCategoryId,
                    ProductCategory = item.ProductCategory,
                    ProductSizeId = item.ProductSizeId,
                    ProductSize = item.ProductSize,
                    ProductCode = item.ProductCode,
                    ProductCost = item.ProductCost,
                    ProductRetailPrice = item.ProductRetailPrice,
                    ProductWholesalePrice = item.ProductWholesalePrice,
                    ProductRunnerPrice = item.ProductRunnerPrice,
                    ProductColorBg = item.ProductColorBg,
                    ProductImage = item.ProductImage,
                    ProductAlias = item.ProductAlias
                });
            }

            return retVal;
        }

        public static List<NewProduct> ToNewProduct(List<ProductsModel> _product)
        {
            var retVal = new List<NewProduct>();

            foreach (var item in _product)
            {
                retVal.Add(new NewProduct
                {
                    ProductId = item.Id,
                    ProductName = item.ProductName,
                    ParentProductId = item.ParentProductId,
                    ProductCategoryId = item.ProductCategoryId,
                    ProductCategory = item.ProductCategory,
                    ProductSizeId = item.ProductSizeId,
                    ProductSize = item.ProductSize,
                    ProductCode = item.ProductCode,
                    ProductCost = item.ProductCost,
                    ProductRetailPrice = item.ProductRetailPrice,
                    ProductWholesalePrice = item.ProductWholesalePrice,
                    ProductRunnerPrice = item.ProductRunnerPrice,
                    ProductColorBg = item.ProductColorBg,
                    ProductImage = item.ProductImage,
                    ProductAlias = item.ProductAlias
                });
            }
           
            return retVal;
        }

        public static List<NewProduct> CopyToNewProduct(List<NewProductCopyHolder> _product)
        {
            var retVal = new List<NewProduct>();

            foreach (var item in _product)
            {
                retVal.Add(new NewProduct
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    ParentProductId = item.ParentProductId,
                    ProductCategoryId = item.ProductCategoryId,
                    ProductCategory = item.ProductCategory,
                    ProductSizeId = item.ProductSizeId,
                    ProductSize = item.ProductSize,
                    ProductCode = item.ProductCode,
                    ProductCost = item.ProductCost,
                    ProductRetailPrice = item.ProductRetailPrice,
                    ProductWholesalePrice = item.ProductWholesalePrice,
                    ProductRunnerPrice = item.ProductRunnerPrice,
                    ProductColorBg = item.ProductColorBg,
                    ProductImage = item.ProductImage,
                    ProductAlias = item.ProductAlias
                });
            }

            return retVal;
        }
    }
}