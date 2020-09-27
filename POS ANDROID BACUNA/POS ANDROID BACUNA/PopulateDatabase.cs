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
            //GlobalVariables.globalParentProductList.Clear();
            var mParentProducts = new List<ParentProductsModel>();
            mParentProducts.Add(new ParentProductsModel() { Id = 1, ParentProductName = "Pants Black", CategoryId = 4, CategoryName = "Pants", ProductColorBg = "00203e", ProductAlias = "PNT BLK" }); //use category id
            mParentProducts.Add(new ParentProductsModel() { Id = 2, ParentProductName = "Polo Jacket Kat", CategoryId = 2, CategoryName = "Polo", ProductColorBg = "f7cac9", ProductAlias = "PJ KAT" });
            mParentProducts.Add(new ParentProductsModel() { Id = 3, ParentProductName = "Polo Barong", CategoryId = 2, CategoryName = "Polo", ProductColorBg = "ffdc73", ProductAlias = "POLO B" });
            mParentProducts.Add(new ParentProductsModel() { Id = 4, ParentProductName = "Baby Collar Kat", CategoryId = 3, CategoryName = "Blouse", ProductColorBg = "a0db8e", ProductAlias = "BC KAT" });
            mParentProducts.Add(new ParentProductsModel() { Id = 5, ParentProductName = "Marine Collar Kat", CategoryId = 3, CategoryName = "Blouse", ProductColorBg = "ffc0cb", ProductAlias = "MC KAT" });
            mParentProducts.Add(new ParentProductsModel() { Id = 6, ParentProductName = "Pants White", CategoryId = 4, CategoryName = "Pants", ProductColorBg = "dcdcdc", ProductAlias = "PNT WHT" });
            mParentProducts.Add(new ParentProductsModel() { Id = 7, ParentProductName = "Polo Straight", CategoryId = 2, CategoryName = "Polo", ProductColorBg = "f26052", ProductAlias = "POLO ST" });
            //GlobalVariables.globalParentProductList = mParentProducts;
        }

        public static void PopulateProducts()
        {
            //GlobalVariables.globalProductList.Clear();
            var mProducts = new List<ProductsModel>();
            //pants
            mProducts.Add(new ProductsModel() { Id = 1, ParentProductId = 1, ProductName = "(XL) Pants Black", ProductCategoryId = 4, ProductCategory = "Pants", ProductSizeId = 3, ProductSize = "XL", ProductRetailPrice = 180.00M, ProductWholesalePrice = 120.00M, ProductRunnerPrice = 150.00M, ProductColorBg = "00203e", ProductAlias = "PNT BLK" });
            mProducts.Add(new ProductsModel() { Id = 2, ParentProductId = 1, ProductName = "(L) Pants Black", ProductCategoryId = 4, ProductCategory = "Pants", ProductSizeId = 4, ProductSize = "L", ProductRetailPrice = 180.00M, ProductWholesalePrice = 120.00M, ProductRunnerPrice = 150.00M, ProductColorBg = "00203e", ProductAlias = "PNT BLK" });
            mProducts.Add(new ProductsModel() { Id = 3, ParentProductId = 1, ProductName = "(M) Pants Black", ProductCategoryId = 4, ProductCategory = "Pants", ProductSizeId = 5, ProductSize = "M", ProductRetailPrice = 180.00M, ProductWholesalePrice = 120.00M, ProductRunnerPrice = 150.00M, ProductColorBg = "00203e", ProductAlias = "PNT BLK" });
            mProducts.Add(new ProductsModel() { Id = 4, ParentProductId = 1, ProductName = "(S) Pants Black", ProductCategoryId = 4, ProductCategory = "Pants", ProductSizeId = 6, ProductSize = "S", ProductRetailPrice = 180.00M, ProductWholesalePrice = 120.00M, ProductRunnerPrice = 150.00M, ProductColorBg = "00203e", ProductAlias = "PNT BLK" });
            mProducts.Add(new ProductsModel() { Id = 5, ParentProductId = 1, ProductName = "(24) Pants Black", ProductCategoryId = 4, ProductCategory = "Pants", ProductSizeId = 7, ProductSize = "24", ProductRetailPrice = 150.00M, ProductWholesalePrice = 110.00M, ProductRunnerPrice = 120.00M, ProductColorBg = "00203e", ProductAlias = "PNT BLK" });
            mProducts.Add(new ProductsModel() { Id = 6, ParentProductId = 1, ProductName = "(22) Pants Black", ProductCategoryId = 4, ProductCategory = "Pants", ProductSizeId = 8, ProductSize = "22", ProductRetailPrice = 150.00M, ProductWholesalePrice = 110.00M, ProductRunnerPrice = 120.00M, ProductColorBg = "00203e", ProductAlias = "PNT BLK" });
            mProducts.Add(new ProductsModel() { Id = 7, ParentProductId = 1, ProductName = "(20) Pants Black", ProductCategoryId = 4, ProductCategory = "Pants", ProductSizeId = 9, ProductSize = "20", ProductRetailPrice = 150.00M, ProductWholesalePrice = 110.00M, ProductRunnerPrice = 120.00M, ProductColorBg = "00203e", ProductAlias = "PNT BLK" });
            mProducts.Add(new ProductsModel() { Id = 8, ParentProductId = 1, ProductName = "(18) Pants Black", ProductCategoryId = 4, ProductCategory = "Pants", ProductSizeId = 10, ProductSize = "18", ProductRetailPrice = 150.00M, ProductWholesalePrice = 110.00M, ProductRunnerPrice = 120.00M, ProductColorBg = "00203e", ProductAlias = "PNT BLK" });
            //polo
            mProducts.Add(new ProductsModel() { Id = 9, ParentProductId = 2, ProductName = "(24) Polo Jacket Kat", ProductCategoryId = 2, ProductCategory = "Polo", ProductSizeId = 7, ProductSize = "24", ProductRetailPrice = 110.00M, ProductColorBg = "f7cac9", ProductAlias = "PJ KAT" });
            mProducts.Add(new ProductsModel() { Id = 10, ParentProductId = 2, ProductName = "(S) Polo Jacket Kat", ProductCategoryId = 2, ProductCategory = "Polo", ProductSizeId = 6, ProductSize = "S", ProductRetailPrice = 120.00M, ProductColorBg = "f7cac9", ProductAlias = "PJ KAT" });
            mProducts.Add(new ProductsModel() { Id = 11, ParentProductId = 2, ProductName = "(M) Polo Jacket Kat", ProductCategoryId = 2, ProductCategory = "Polo", ProductSizeId = 5, ProductSize = "M", ProductRetailPrice = 130.00M, ProductColorBg = "f7cac9", ProductAlias = "PJ KAT" });
            mProducts.Add(new ProductsModel() { Id = 12, ParentProductId = 2, ProductName = "(L) Polo Jacket Kat", ProductCategoryId = 2, ProductCategory = "Polo", ProductSizeId = 4, ProductSize = "L", ProductRetailPrice = 140.00M, ProductColorBg = "f7cac9", ProductAlias = "PJ KAT" });

            mProducts.Add(new ProductsModel() { Id = 13, ParentProductId = 3, ProductName = "(24) Polo Barong", ProductCategoryId = 2, ProductCategory = "Polo", ProductSizeId = 7, ProductSize = "24", ProductRetailPrice = 160.00M, ProductColorBg = "ffdc73", ProductAlias = "POLO B" });
            mProducts.Add(new ProductsModel() { Id = 14, ParentProductId = 3, ProductName = "(S) Polo Barong", ProductCategoryId = 2, ProductCategory = "Polo", ProductSizeId = 6, ProductSize = "S", ProductRetailPrice = 170.00M, ProductColorBg = "ffdc73", ProductAlias = "POLO B" });
            mProducts.Add(new ProductsModel() { Id = 15, ParentProductId = 3, ProductName = "(M) Polo Barong", ProductCategoryId = 2, ProductCategory = "Polo", ProductSizeId = 5, ProductSize = "M", ProductRetailPrice = 180.00M, ProductColorBg = "ffdc73", ProductAlias = "POLO B" });
            mProducts.Add(new ProductsModel() { Id = 16, ParentProductId = 3, ProductName = "(L) Polo Barong", ProductCategoryId = 2, ProductCategory = "Polo", ProductSizeId = 4, ProductSize = "L", ProductRetailPrice = 190.00M, ProductColorBg = "ffdc73", ProductAlias = "POLO B" });
            //blouse
            mProducts.Add(new ProductsModel() { Id = 17, ParentProductId = 4, ProductName = "(S) Baby Collar Kat", ProductCategoryId = 3, ProductCategory = "Blouse", ProductSizeId = 6, ProductSize = "S", ProductRetailPrice = 120.00M, ProductColorBg = "a0db8e", ProductAlias = "BC KAT" });
            mProducts.Add(new ProductsModel() { Id = 18, ParentProductId = 4, ProductName = "(M) Baby Collar Kat", ProductCategoryId = 3, ProductCategory = "Blouse", ProductSizeId = 5, ProductSize = "M", ProductRetailPrice = 130.00M, ProductColorBg = "a0db8e", ProductAlias = "BC KAT" });
            mProducts.Add(new ProductsModel() { Id = 19, ParentProductId = 4, ProductName = "(L) Baby Collar Kat", ProductCategoryId = 3, ProductCategory = "Blouse", ProductSizeId = 4, ProductSize = "L", ProductRetailPrice = 140.00M, ProductColorBg = "a0db8e", ProductAlias = "BC KAT" });
            mProducts.Add(new ProductsModel() { Id = 20, ParentProductId = 4, ProductName = "(XL) Baby Collar Kat", ProductCategoryId = 3, ProductCategory = "Blouse", ProductSizeId = 3, ProductSize = "XL", ProductRetailPrice = 150.00M, ProductColorBg = "a0db8e", ProductAlias = "BC KAT" });

            mProducts.Add(new ProductsModel() { Id = 21, ParentProductId = 5, ProductName = "(S) Marine Collar Kat", ProductCategoryId = 3, ProductCategory = "Blouse", ProductSizeId = 6, ProductSize = "S", ProductRetailPrice = 100.00M, ProductColorBg = "ffc0cb", ProductAlias = "MC KAT" });
            mProducts.Add(new ProductsModel() { Id = 22, ParentProductId = 5, ProductName = "(M) Marine Collar Kat", ProductCategoryId = 3, ProductCategory = "Blouse", ProductSizeId = 5, ProductSize = "M", ProductRetailPrice = 110.00M, ProductColorBg = "ffc0cb", ProductAlias = "MC KAT" });
            mProducts.Add(new ProductsModel() { Id = 23, ParentProductId = 5, ProductName = "(L) Marine Collar Kat", ProductCategoryId = 3, ProductCategory = "Blouse", ProductSizeId = 4, ProductSize = "L", ProductRetailPrice = 120.00M, ProductColorBg = "ffc0cb", ProductAlias = "MC KAT" });
            mProducts.Add(new ProductsModel() { Id = 24, ParentProductId = 5, ProductName = "(XL) Marine Collar Kat", ProductCategoryId = 3, ProductCategory = "Blouse", ProductSizeId = 3, ProductSize = "XL", ProductRetailPrice = 130.00M, ProductColorBg = "ffc0cb", ProductAlias = "MC KAT" });

            mProducts.Add(new ProductsModel() { Id = 25, ParentProductId = 6, ProductName = "(XL) Pants White", ProductCategoryId = 4, ProductCategory = "Pants", ProductSizeId = 3, ProductSize = "XL", ProductRetailPrice = 180.00M, ProductColorBg = "dcdcdc", ProductAlias = "PNT WHT" });
            mProducts.Add(new ProductsModel() { Id = 26, ParentProductId = 6, ProductName = "(L) Pants White", ProductCategoryId = 4, ProductCategory = "Pants", ProductSizeId = 4, ProductSize = "L", ProductRetailPrice = 180.00M, ProductColorBg = "dcdcdc", ProductAlias = "PNT WHT" });
            mProducts.Add(new ProductsModel() { Id = 27, ParentProductId = 6, ProductName = "(M) Pants White", ProductCategoryId = 4, ProductCategory = "Pants", ProductSizeId = 5, ProductSize = "M", ProductRetailPrice = 180.00M, ProductColorBg = "dcdcdc", ProductAlias = "PNT WHT" });
            mProducts.Add(new ProductsModel() { Id = 28, ParentProductId = 6, ProductName = "(S) Pants White", ProductCategoryId = 4, ProductCategory = "Pants", ProductSizeId = 6, ProductSize = "S", ProductRetailPrice = 180.00M, ProductColorBg = "dcdcdc", ProductAlias = "PNT WHT" });
            mProducts.Add(new ProductsModel() { Id = 29, ParentProductId = 6, ProductName = "(24) Pants White", ProductCategoryId = 4, ProductCategory = "Pants", ProductSizeId = 7, ProductSize = "24", ProductRetailPrice = 150.00M, ProductColorBg = "dcdcdc", ProductAlias = "PNT WHT" });
            mProducts.Add(new ProductsModel() { Id = 30, ParentProductId = 6, ProductName = "(22) Pants White", ProductCategoryId = 4, ProductCategory = "Pants", ProductSizeId = 8, ProductSize = "22", ProductRetailPrice = 150.00M, ProductColorBg = "dcdcdc", ProductAlias = "PNT WHT" });
            mProducts.Add(new ProductsModel() { Id = 31, ParentProductId = 6, ProductName = "(20) Pants White", ProductCategoryId = 4, ProductCategory = "Pants", ProductSizeId = 9, ProductSize = "20", ProductRetailPrice = 150.00M, ProductColorBg = "dcdcdc", ProductAlias = "PNT WHT" });
            mProducts.Add(new ProductsModel() { Id = 32, ParentProductId = 6, ProductName = "(18) Pants White", ProductCategoryId = 4, ProductCategory = "Pants", ProductSizeId = 10, ProductSize = "18", ProductRetailPrice = 150.00M, ProductColorBg = "dcdcdc", ProductAlias = "PNT WHT" });
            //polo
            mProducts.Add(new ProductsModel() { Id = 33, ParentProductId = 7, ProductName = "(20) Polo Straight Kat", ProductCategoryId = 2, ProductCategory = "Polo", ProductSizeId = 9, ProductSize = "20", ProductRetailPrice = 110.00M, ProductColorBg = "f26052", ProductAlias = "POLO ST" });
            mProducts.Add(new ProductsModel() { Id = 34, ParentProductId = 7, ProductName = "(S) Polo Straight Kat", ProductCategoryId = 2, ProductCategory = "Polo", ProductSizeId = 6, ProductSize = "S", ProductRetailPrice = 120.00M, ProductColorBg = "f26052", ProductAlias = "POLO ST" });
            mProducts.Add(new ProductsModel() { Id = 35, ParentProductId = 7, ProductName = "(M) Polo Straight Kat", ProductCategoryId = 2, ProductCategory = "Polo", ProductSizeId = 5, ProductSize = "M", ProductRetailPrice = 130.00M, ProductColorBg = "f26052", ProductAlias = "POLO ST" });
            mProducts.Add(new ProductsModel() { Id = 36, ParentProductId = 7, ProductName = "(L) Polo Straight Kat", ProductCategoryId = 2, ProductCategory = "Polo", ProductSizeId = 4, ProductSize = "L", ProductRetailPrice = 140.00M, ProductColorBg = "f26052", ProductAlias = "POLO ST" });
            mProducts.Add(new ProductsModel() { Id = 37, ParentProductId = 7, ProductName = "(18) Polo Straight Kat", ProductCategoryId = 2, ProductCategory = "Polo", ProductSizeId = 10, ProductSize = "18", ProductRetailPrice = 110.00M, ProductColorBg = "f26052", ProductAlias = "POLO ST" });
            mProducts.Add(new ProductsModel() { Id = 38, ParentProductId = 7, ProductName = "(16) Polo Straight Kat", ProductCategoryId = 2, ProductCategory = "Polo", ProductSizeId = 11, ProductSize = "16", ProductRetailPrice = 110.00M, ProductColorBg = "f26052", ProductAlias = "POLO ST" });
            mProducts.Add(new ProductsModel() { Id = 39, ParentProductId = 7, ProductName = "(14) Polo Straight Kat", ProductCategoryId = 2, ProductCategory = "Polo", ProductSizeId = 12, ProductSize = "14", ProductRetailPrice = 110.00M, ProductColorBg = "f26052", ProductAlias = "POLO ST" });
            mProducts.Add(new ProductsModel() { Id = 40, ParentProductId = 7, ProductName = "(12) Polo Straight Kat", ProductCategoryId = 2, ProductCategory = "Polo", ProductSizeId = 13, ProductSize = "12", ProductRetailPrice = 110.00M, ProductColorBg = "f26052", ProductAlias = "POLO ST" });
            mProducts.Add(new ProductsModel() { Id = 41, ParentProductId = 7, ProductName = "(10) Polo Straight Kat", ProductCategoryId = 2, ProductCategory = "Polo", ProductSizeId = 14, ProductSize = "10", ProductRetailPrice = 100.00M, ProductColorBg = "f26052", ProductAlias = "POLO ST" });
            mProducts.Add(new ProductsModel() { Id = 42, ParentProductId = 7, ProductName = "(8) Polo Straight Kat", ProductCategoryId = 2, ProductCategory = "Polo", ProductSizeId = 15, ProductSize = "8", ProductRetailPrice = 100.00M, ProductColorBg = "f26052", ProductAlias = "POLO ST" });
            mProducts.Add(new ProductsModel() { Id = 43, ParentProductId = 7, ProductName = "(6) Polo Straight Kat", ProductCategoryId = 2, ProductCategory = "Polo", ProductSizeId = 16, ProductSize = "6", ProductRetailPrice = 100.00M, ProductColorBg = "f26052", ProductAlias = "POLO ST" });
            mProducts.Add(new ProductsModel() { Id = 44, ParentProductId = 7, ProductName = "(4) Polo Straight Kat", ProductCategoryId = 2, ProductCategory = "Polo", ProductSizeId = 17, ProductSize = "4", ProductRetailPrice = 100.00M, ProductColorBg = "f26052", ProductAlias = "POLO ST" });
            //GlobalVariables.globalProductList = mProducts;
        }

        public static void PopulateCategories()
        {
            //GlobalVariables.globalProductCategoryList.Clear();
            var mCategories = new List<ProductCategoriesModel>();
            mCategories.Add(new ProductCategoriesModel() { Id = 1, ProductCategoryName = "All" });
            mCategories.Add(new ProductCategoriesModel() { Id = 2, ProductCategoryName = "Polo" });
            mCategories.Add(new ProductCategoriesModel() { Id = 3, ProductCategoryName = "Blouse" });
            mCategories.Add(new ProductCategoriesModel() { Id = 4, ProductCategoryName = "Pants" });
            mCategories.Add(new ProductCategoriesModel() { Id = 5, ProductCategoryName = "Palda" });
            mCategories.Add(new ProductCategoriesModel() { Id = 6, ProductCategoryName = "Panyo" });
            //GlobalVariables.globalProductCategoryList = mCategories;
        }
        public static void PopulateSizes()
        {
            //GlobalVariables.globalSizesList.Clear();
            var sizes = new List<ProductSizesModel>();
            sizes.Add(new ProductSizesModel() { Id = 1, SizeRank = 19, ProductSizeName = "3X" });
            sizes.Add(new ProductSizesModel() { Id = 2, SizeRank = 18, ProductSizeName = "2X" });
            sizes.Add(new ProductSizesModel() { Id = 3, SizeRank = 17, ProductSizeName = "XL" });
            sizes.Add(new ProductSizesModel() { Id = 4, SizeRank = 16, ProductSizeName = "L" });
            sizes.Add(new ProductSizesModel() { Id = 5, SizeRank = 15, ProductSizeName = "M" });
            sizes.Add(new ProductSizesModel() { Id = 6, SizeRank = 6, ProductSizeName = "S" });
            sizes.Add(new ProductSizesModel() { Id = 7, SizeRank = 7, ProductSizeName = "24" });
            sizes.Add(new ProductSizesModel() { Id = 8, SizeRank = 8, ProductSizeName = "22" });
            sizes.Add(new ProductSizesModel() { Id = 9, SizeRank = 9, ProductSizeName = "20" });
            sizes.Add(new ProductSizesModel() { Id = 10, SizeRank = 10, ProductSizeName = "18" });
            sizes.Add(new ProductSizesModel() { Id = 11, SizeRank = 11, ProductSizeName = "16" });
            sizes.Add(new ProductSizesModel() { Id = 12, SizeRank = 12, ProductSizeName = "14" });
            sizes.Add(new ProductSizesModel() { Id = 13, SizeRank = 13, ProductSizeName = "12" });
            sizes.Add(new ProductSizesModel() { Id = 14, SizeRank = 14, ProductSizeName = "10" });
            sizes.Add(new ProductSizesModel() { Id = 15, SizeRank = 15, ProductSizeName = "8" });
            sizes.Add(new ProductSizesModel() { Id = 16, SizeRank = 16, ProductSizeName = "6" });
            sizes.Add(new ProductSizesModel() { Id = 17, SizeRank = 17, ProductSizeName = "4" });
            sizes.Add(new ProductSizesModel() { Id = 18, SizeRank = 18, ProductSizeName = "2" });
            sizes.Add(new ProductSizesModel() { Id = 19, SizeRank = 19, ProductSizeName = "0" });

            int counter = sizes.Count();
            foreach (var item in sizes.OrderBy(x=>x.Id))
            {
                item.SizeRank = counter;
                counter -= 1;
            }

            //GlobalVariables.globalSizesList = sizes;
        }
    }
}