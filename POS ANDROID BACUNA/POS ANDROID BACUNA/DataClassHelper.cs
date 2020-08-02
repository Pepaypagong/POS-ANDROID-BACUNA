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
        public static List<Product> ToProduct(List<NewProduct> _newProduct)
        {
            var retVal = new List<Product>();

            foreach (var item in _newProduct)
            {
                retVal.Add(new Product
                {
                    productId = item.productId,
                    productName = item.productName,
                    parentProductId = item.parentProductId,
                    productCategoryId = item.productCategoryId,
                    productCategory = item.productCategory,
                    productSizeId = item.productSizeId,
                    productSize = item.productSize,
                    productCode = item.productCode,
                    productCost = item.productCost,
                    productRetailPrice = item.productRetailPrice,
                    productWholesalePrice = item.productWholesalePrice,
                    productRunnerPrice = item.productRunnerPrice,
                    productColorBg = item.productColorBg,
                    productImage = item.productImage,
                    productAlias = item.productAlias
                });
            }

            return retVal;
        }

        public static List<NewProduct> ToNewProduct(List<Product> _product)
        {
            var retVal = new List<NewProduct>();

            foreach (var item in _product)
            {
                retVal.Add(new NewProduct
                {
                    productId = item.productId,
                    productName = item.productName,
                    parentProductId = item.parentProductId,
                    productCategoryId = item.productCategoryId,
                    productCategory = item.productCategory,
                    productSizeId = item.productSizeId,
                    productSize = item.productSize,
                    productCode = item.productCode,
                    productCost = item.productCost,
                    productRetailPrice = item.productRetailPrice,
                    productWholesalePrice = item.productWholesalePrice,
                    productRunnerPrice = item.productRunnerPrice,
                    productColorBg = item.productColorBg,
                    productImage = item.productImage,
                    productAlias = item.productAlias
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
                    productId = item.productId,
                    productName = item.productName,
                    parentProductId = item.parentProductId,
                    productCategoryId = item.productCategoryId,
                    productCategory = item.productCategory,
                    productSizeId = item.productSizeId,
                    productSize = item.productSize,
                    productCode = item.productCode,
                    productCost = item.productCost,
                    productRetailPrice = item.productRetailPrice,
                    productWholesalePrice = item.productWholesalePrice,
                    productRunnerPrice = item.productRunnerPrice,
                    productColorBg = item.productColorBg,
                    productImage = item.productImage,
                    productAlias = item.productAlias
                });
            }

            return retVal;
        }
    }
}