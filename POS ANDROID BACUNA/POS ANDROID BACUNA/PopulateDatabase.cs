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
    public class PopulateDatabase
    {
        public static void PopulateAll()
        {
            PopulateParentProducts();
            PopulateProducts();
            PopulateSizes();
            PopulateCategories();
        }

        public static void PopulateParentProducts()
        {
            GlobalVariables.globalParentProductList.Clear();
            var mParentProducts = new List<ParentProducts>();
            mParentProducts.Add(new ParentProducts() { parentProductId = 1, parentProductName = "Pants Black", categoryId = 4, categoryName = "Pants", productColorBg = "00203e", productAlias = "PNT BLK" }); //use category id
            mParentProducts.Add(new ParentProducts() { parentProductId = 2, parentProductName = "Polo Jacket Kat", categoryId = 2, categoryName = "Polo", productColorBg = "f7cac9", productAlias = "PJ KAT" });
            mParentProducts.Add(new ParentProducts() { parentProductId = 3, parentProductName = "Polo Barong", categoryId = 2, categoryName = "Polo", productColorBg = "ffdc73", productAlias = "POLO B" });
            mParentProducts.Add(new ParentProducts() { parentProductId = 4, parentProductName = "Baby Collar Kat", categoryId = 3, categoryName = "Blouse", productColorBg = "a0db8e", productAlias = "BC KAT" });
            mParentProducts.Add(new ParentProducts() { parentProductId = 5, parentProductName = "Marine Collar Kat", categoryId = 3, categoryName = "Blouse", productColorBg = "ffc0cb", productAlias = "MC KAT" });
            mParentProducts.Add(new ParentProducts() { parentProductId = 6, parentProductName = "Pants White", categoryId = 4, categoryName = "Pants", productColorBg = "dcdcdc", productAlias = "PNT WHT" });
            mParentProducts.Add(new ParentProducts() { parentProductId = 7, parentProductName = "Polo Straight", categoryId = 2, categoryName = "Polo", productColorBg = "f26052", productAlias = "POLO ST" });
            GlobalVariables.globalParentProductList = mParentProducts;
        }

        public static void PopulateProducts()
        {
            GlobalVariables.globalProductList.Clear();
            var mProducts = new List<Product>();
            //pants
            mProducts.Add(new Product() { productId = 1, parentProductId = 1, productName = "(XL) Pants Black", productCategoryId = 4, productCategory = "Pants", productSizeId = 3, productSize = "XL", productRetailPrice = 180.00M, productWholesalePrice = 120.00M, productRunnerPrice = 150.00M, productColorBg = "00203e", productAlias = "PNT BLK" });
            mProducts.Add(new Product() { productId = 2, parentProductId = 1, productName = "(L) Pants Black", productCategoryId = 4, productCategory = "Pants", productSizeId = 4, productSize = "L", productRetailPrice = 180.00M, productWholesalePrice = 120.00M, productRunnerPrice = 150.00M, productColorBg = "00203e", productAlias = "PNT BLK" });
            mProducts.Add(new Product() { productId = 3, parentProductId = 1, productName = "(M) Pants Black", productCategoryId = 4, productCategory = "Pants", productSizeId = 5, productSize = "M", productRetailPrice = 180.00M, productWholesalePrice = 120.00M, productRunnerPrice = 150.00M, productColorBg = "00203e", productAlias = "PNT BLK" });
            mProducts.Add(new Product() { productId = 4, parentProductId = 1, productName = "(S) Pants Black", productCategoryId = 4, productCategory = "Pants", productSizeId = 6, productSize = "S", productRetailPrice = 180.00M, productWholesalePrice = 120.00M, productRunnerPrice = 150.00M, productColorBg = "00203e", productAlias = "PNT BLK" });
            mProducts.Add(new Product() { productId = 5, parentProductId = 1, productName = "(24) Pants Black", productCategoryId = 4, productCategory = "Pants", productSizeId = 7, productSize = "24", productRetailPrice = 150.00M, productWholesalePrice = 110.00M, productRunnerPrice = 120.00M, productColorBg = "00203e", productAlias = "PNT BLK" });
            mProducts.Add(new Product() { productId = 6, parentProductId = 1, productName = "(22) Pants Black", productCategoryId = 4, productCategory = "Pants", productSizeId = 8, productSize = "22", productRetailPrice = 150.00M, productWholesalePrice = 110.00M, productRunnerPrice = 120.00M, productColorBg = "00203e", productAlias = "PNT BLK" });
            mProducts.Add(new Product() { productId = 7, parentProductId = 1, productName = "(20) Pants Black", productCategoryId = 4, productCategory = "Pants", productSizeId = 9, productSize = "20", productRetailPrice = 150.00M, productWholesalePrice = 110.00M, productRunnerPrice = 120.00M, productColorBg = "00203e", productAlias = "PNT BLK" });
            mProducts.Add(new Product() { productId = 8, parentProductId = 1, productName = "(18) Pants Black", productCategoryId = 4, productCategory = "Pants", productSizeId = 10, productSize = "18", productRetailPrice = 150.00M, productWholesalePrice = 110.00M, productRunnerPrice = 120.00M, productColorBg = "00203e", productAlias = "PNT BLK" });
            //polo
            mProducts.Add(new Product() { productId = 9, parentProductId = 2, productName = "(24) Polo Jacket Kat", productCategoryId = 2, productCategory = "Polo", productSizeId = 7, productSize = "24", productRetailPrice = 110.00M, productColorBg = "f7cac9", productAlias = "PJ KAT" });
            mProducts.Add(new Product() { productId = 10, parentProductId = 2, productName = "(S) Polo Jacket Kat", productCategoryId = 2, productCategory = "Polo", productSizeId = 6, productSize = "S", productRetailPrice = 120.00M, productColorBg = "f7cac9", productAlias = "PJ KAT" });
            mProducts.Add(new Product() { productId = 11, parentProductId = 2, productName = "(M) Polo Jacket Kat", productCategoryId = 2, productCategory = "Polo", productSizeId = 5, productSize = "M", productRetailPrice = 130.00M, productColorBg = "f7cac9", productAlias = "PJ KAT" });
            mProducts.Add(new Product() { productId = 12, parentProductId = 2, productName = "(L) Polo Jacket Kat", productCategoryId = 2, productCategory = "Polo", productSizeId = 4, productSize = "L", productRetailPrice = 140.00M, productColorBg = "f7cac9", productAlias = "PJ KAT" });

            mProducts.Add(new Product() { productId = 13, parentProductId = 3, productName = "(24) Polo Barong", productCategoryId = 2, productCategory = "Polo", productSizeId = 7, productSize = "24", productRetailPrice = 160.00M, productColorBg = "ffdc73", productAlias = "POLO B" });
            mProducts.Add(new Product() { productId = 14, parentProductId = 3, productName = "(S) Polo Barong", productCategoryId = 2, productCategory = "Polo", productSizeId = 6, productSize = "S", productRetailPrice = 170.00M, productColorBg = "ffdc73", productAlias = "POLO B" });
            mProducts.Add(new Product() { productId = 15, parentProductId = 3, productName = "(M) Polo Barong", productCategoryId = 2, productCategory = "Polo", productSizeId = 5, productSize = "M", productRetailPrice = 180.00M, productColorBg = "ffdc73", productAlias = "POLO B" });
            mProducts.Add(new Product() { productId = 16, parentProductId = 3, productName = "(L) Polo Barong", productCategoryId = 2, productCategory = "Polo", productSizeId = 4, productSize = "L", productRetailPrice = 190.00M, productColorBg = "ffdc73", productAlias = "POLO B" });
            //blouse
            mProducts.Add(new Product() { productId = 17, parentProductId = 4, productName = "(S) Baby Collar Kat", productCategoryId = 3, productCategory = "Blouse", productSizeId = 6, productSize = "S", productRetailPrice = 120.00M, productColorBg = "a0db8e", productAlias = "BC KAT" });
            mProducts.Add(new Product() { productId = 18, parentProductId = 4, productName = "(M) Baby Collar Kat", productCategoryId = 3, productCategory = "Blouse", productSizeId = 5, productSize = "M", productRetailPrice = 130.00M, productColorBg = "a0db8e", productAlias = "BC KAT" });
            mProducts.Add(new Product() { productId = 19, parentProductId = 4, productName = "(L) Baby Collar Kat", productCategoryId = 3, productCategory = "Blouse", productSizeId = 4, productSize = "L", productRetailPrice = 140.00M, productColorBg = "a0db8e", productAlias = "BC KAT" });
            mProducts.Add(new Product() { productId = 20, parentProductId = 4, productName = "(XL) Baby Collar Kat", productCategoryId = 3, productCategory = "Blouse", productSizeId = 3, productSize = "XL", productRetailPrice = 150.00M, productColorBg = "a0db8e", productAlias = "BC KAT" });

            mProducts.Add(new Product() { productId = 21, parentProductId = 5, productName = "(S) Marine Collar Kat", productCategoryId = 3, productCategory = "Blouse", productSizeId = 6, productSize = "S", productRetailPrice = 100.00M, productColorBg = "ffc0cb", productAlias = "MC KAT" });
            mProducts.Add(new Product() { productId = 22, parentProductId = 5, productName = "(M) Marine Collar Kat", productCategoryId = 3, productCategory = "Blouse", productSizeId = 5, productSize = "M", productRetailPrice = 110.00M, productColorBg = "ffc0cb", productAlias = "MC KAT" });
            mProducts.Add(new Product() { productId = 23, parentProductId = 5, productName = "(L) Marine Collar Kat", productCategoryId = 3, productCategory = "Blouse", productSizeId = 4, productSize = "L", productRetailPrice = 120.00M, productColorBg = "ffc0cb", productAlias = "MC KAT" });
            mProducts.Add(new Product() { productId = 24, parentProductId = 5, productName = "(XL) Marine Collar Kat", productCategoryId = 3, productCategory = "Blouse", productSizeId = 3, productSize = "XL", productRetailPrice = 130.00M, productColorBg = "ffc0cb", productAlias = "MC KAT" });

            mProducts.Add(new Product() { productId = 25, parentProductId = 6, productName = "(XL) Pants White", productCategoryId = 4, productCategory = "Pants", productSizeId = 3, productSize = "XL", productRetailPrice = 180.00M, productColorBg = "dcdcdc", productAlias = "PNT WHT" });
            mProducts.Add(new Product() { productId = 26, parentProductId = 6, productName = "(L) Pants White", productCategoryId = 4, productCategory = "Pants", productSizeId = 4, productSize = "L", productRetailPrice = 180.00M, productColorBg = "dcdcdc", productAlias = "PNT WHT" });
            mProducts.Add(new Product() { productId = 27, parentProductId = 6, productName = "(M) Pants White", productCategoryId = 4, productCategory = "Pants", productSizeId = 5, productSize = "M", productRetailPrice = 180.00M, productColorBg = "dcdcdc", productAlias = "PNT WHT" });
            mProducts.Add(new Product() { productId = 28, parentProductId = 6, productName = "(S) Pants White", productCategoryId = 4, productCategory = "Pants", productSizeId = 6, productSize = "S", productRetailPrice = 180.00M, productColorBg = "dcdcdc", productAlias = "PNT WHT" });
            mProducts.Add(new Product() { productId = 29, parentProductId = 6, productName = "(24) Pants White", productCategoryId = 4, productCategory = "Pants", productSizeId = 7, productSize = "24", productRetailPrice = 150.00M, productColorBg = "dcdcdc", productAlias = "PNT WHT" });
            mProducts.Add(new Product() { productId = 30, parentProductId = 6, productName = "(22) Pants White", productCategoryId = 4, productCategory = "Pants", productSizeId = 8, productSize = "22", productRetailPrice = 150.00M, productColorBg = "dcdcdc", productAlias = "PNT WHT" });
            mProducts.Add(new Product() { productId = 31, parentProductId = 6, productName = "(20) Pants White", productCategoryId = 4, productCategory = "Pants", productSizeId = 9, productSize = "20", productRetailPrice = 150.00M, productColorBg = "dcdcdc", productAlias = "PNT WHT" });
            mProducts.Add(new Product() { productId = 32, parentProductId = 6, productName = "(18) Pants White", productCategoryId = 4, productCategory = "Pants", productSizeId = 10, productSize = "18", productRetailPrice = 150.00M, productColorBg = "dcdcdc", productAlias = "PNT WHT" });
            //polo
            mProducts.Add(new Product() { productId = 33, parentProductId = 7, productName = "(20) Polo Straight Kat", productCategoryId = 2, productCategory = "Polo", productSizeId = 9, productSize = "20", productRetailPrice = 110.00M, productColorBg = "f26052", productAlias = "POLO ST" });
            mProducts.Add(new Product() { productId = 34, parentProductId = 7, productName = "(S) Polo Straight Kat", productCategoryId = 2, productCategory = "Polo", productSizeId = 6, productSize = "S", productRetailPrice = 120.00M, productColorBg = "f26052", productAlias = "POLO ST" });
            mProducts.Add(new Product() { productId = 35, parentProductId = 7, productName = "(M) Polo Straight Kat", productCategoryId = 2, productCategory = "Polo", productSizeId = 5, productSize = "M", productRetailPrice = 130.00M, productColorBg = "f26052", productAlias = "POLO ST" });
            mProducts.Add(new Product() { productId = 36, parentProductId = 7, productName = "(L) Polo Straight Kat", productCategoryId = 2, productCategory = "Polo", productSizeId = 4, productSize = "L", productRetailPrice = 140.00M, productColorBg = "f26052", productAlias = "POLO ST" });
            GlobalVariables.globalProductList = mProducts;
        }

        public static void PopulateCategories()
        {
            GlobalVariables.globalProductCategoryList.Clear();
            var mCategories = new List<ProductCategories>();
            mCategories.Add(new ProductCategories() { productCategoryId = 1, productCategoryName = "All" });
            mCategories.Add(new ProductCategories() { productCategoryId = 2, productCategoryName = "Polo" });
            mCategories.Add(new ProductCategories() { productCategoryId = 3, productCategoryName = "Blouse" });
            mCategories.Add(new ProductCategories() { productCategoryId = 4, productCategoryName = "Pants" });
            mCategories.Add(new ProductCategories() { productCategoryId = 5, productCategoryName = "Palda" });
            mCategories.Add(new ProductCategories() { productCategoryId = 6, productCategoryName = "Panyo" });
            GlobalVariables.globalProductCategoryList = mCategories;
        }
        public static void PopulateSizes()
        {
            GlobalVariables.globalSizesList.Clear();
            var sizes = new List<ProductSizes>();
            sizes.Add(new ProductSizes() { productSizeId = 1, sizeRank = 19, productSizeName = "3X" });
            sizes.Add(new ProductSizes() { productSizeId = 2, sizeRank = 18, productSizeName = "2X" });
            sizes.Add(new ProductSizes() { productSizeId = 3, sizeRank = 17, productSizeName = "XL" });
            sizes.Add(new ProductSizes() { productSizeId = 4, sizeRank = 16, productSizeName = "L" });
            sizes.Add(new ProductSizes() { productSizeId = 5, sizeRank = 15, productSizeName = "M" });
            sizes.Add(new ProductSizes() { productSizeId = 6, sizeRank = 6, productSizeName = "S" });
            sizes.Add(new ProductSizes() { productSizeId = 7, sizeRank = 7, productSizeName = "24" });
            sizes.Add(new ProductSizes() { productSizeId = 8, sizeRank = 8, productSizeName = "22" });
            sizes.Add(new ProductSizes() { productSizeId = 9, sizeRank = 9, productSizeName = "20" });
            sizes.Add(new ProductSizes() { productSizeId = 10, sizeRank = 10, productSizeName = "18" });
            sizes.Add(new ProductSizes() { productSizeId = 11, sizeRank = 11, productSizeName = "16" });
            sizes.Add(new ProductSizes() { productSizeId = 12, sizeRank = 12, productSizeName = "14" });
            sizes.Add(new ProductSizes() { productSizeId = 13, sizeRank = 13, productSizeName = "12" });
            sizes.Add(new ProductSizes() { productSizeId = 14, sizeRank = 14, productSizeName = "10" });
            sizes.Add(new ProductSizes() { productSizeId = 15, sizeRank = 15, productSizeName = "8" });
            sizes.Add(new ProductSizes() { productSizeId = 16, sizeRank = 16, productSizeName = "6" });
            sizes.Add(new ProductSizes() { productSizeId = 17, sizeRank = 17, productSizeName = "4" });
            sizes.Add(new ProductSizes() { productSizeId = 18, sizeRank = 18, productSizeName = "2" });
            sizes.Add(new ProductSizes() { productSizeId = 19, sizeRank = 19, productSizeName = "0" });

            int counter = sizes.Count();
            foreach (var item in sizes.OrderBy(x=>x.productSizeId))
            {
                item.sizeRank = counter;
                counter -= 1;
            }

            GlobalVariables.globalSizesList = sizes;
        }
    }
}