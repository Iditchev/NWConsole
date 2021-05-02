using System;
using NLog.Web;
using System.IO;
using System.Linq;
using NorthwindConsole.Model;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace NorthwindConsole
{
    class Program
    {
        private static NLog.Logger logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
        static void Main(string[] args)

        {
           logger.Info("Program started");
            
            try
            {
                string choice;
                do
                {
                    Console.WriteLine("1) Display Categories");
                    Console.WriteLine("2) Add Category");
                    Console.WriteLine("3) Edit Category");
                    Console.WriteLine("4) Display Products ");
                    Console.WriteLine("5) Add Product");
                    Console.WriteLine("6) Edit Product");
                    Console.WriteLine("\"q\" to quit");
                    choice = Console.ReadLine();
                    Console.Clear();
                    logger.Info($"Option {choice} selected");
                    if (choice == "1")
                    {
                        Console.WriteLine("1) Display All Categories");
                        Console.WriteLine("2) Display A Specific Category and related Products");
                        Console.WriteLine("3) Display All Categories and their related Products");
                        string DisplayChoice = Console.ReadLine();
                        if (DisplayChoice == "1")
                        {
                              var db = new NWConsole_96_IDContext();
                              var query = db.Categories.OrderBy(p => p.CategoryName);

                              Console.ForegroundColor = ConsoleColor.Green;
                              Console.WriteLine($"{query.Count()} records returned");
                              Console.ForegroundColor = ConsoleColor.Magenta;
                                foreach (var item in query)
                                {
                                Console.WriteLine($"{item.CategoryName} - {item.Description}");
                                }
                              Console.ForegroundColor = ConsoleColor.White;
                        }
                        else if (DisplayChoice == "2")
                        {
                         var db = new NWConsole_96_IDContext();
                         var query = db.Categories.OrderBy(p => p.CategoryId);

                         Console.WriteLine("Select the category whose products you want to display:");
                         Console.ForegroundColor = ConsoleColor.DarkRed;
                         foreach (var item in query)
                         {
                             Console.WriteLine($"{item.CategoryId}) {item.CategoryName}");
                         }
                         Console.ForegroundColor = ConsoleColor.White;
                         try
                         {int id = int.Parse(Console.ReadLine());
                         Console.Clear();
                         logger.Info($"CategoryId {id} selected");
                         Categories category = db.Categories.Include("Products").FirstOrDefault(c => c.CategoryId == id);
                         Console.WriteLine($"{category.CategoryName} - {category.Description}");
                         foreach (Products p in category.Products.Where(m => m.Discontinued is false))
                         {
                             Console.WriteLine(p.ProductName);
                         }
                        } catch {logger.Error("Invalid category selected");}
                    }
                        else if (DisplayChoice == "3")
                        {
                            var db = new NWConsole_96_IDContext();
                            var query = db.Categories.Include("Products").OrderBy(p => p.CategoryId);
                            foreach (var item in query)
                            {
                             Console.WriteLine($"{item.CategoryName}");
                                foreach (Products p in item.Products.Where(m => m.Discontinued is false))
                                {
                                 Console.WriteLine($"\t{p.ProductName}");
                                }
                            }
                        }
                        else { logger.Error("Invalid entry, please try again");}
                    }
                    else if (choice == "2")
                     {
                         
                         Console.WriteLine("Enter Category Name:");
                         var CategoryName = Console.ReadLine();
                         Console.WriteLine("Enter the Category Description:");
                         var Description = Console.ReadLine();
                         var category = new Categories {CategoryName = CategoryName, Description = Description};
                          ValidationContext context = new ValidationContext(category, null, null);
                         List<ValidationResult> results = new List<ValidationResult>();

                         var isValid = Validator.TryValidateObject(category, context, results, true);
                         if (isValid)
                         {
                               var db = new NWConsole_96_IDContext();
                             // check for unique name
                             if (db.Categories.Any(c => c.CategoryName == category.CategoryName))
                             {
                                 // generate validation error
                                 isValid = false;
                                 results.Add(new ValidationResult("Name exists", new string[] { "CategoryName" }));
                             }
                             else
                             {
                                 logger.Info("Validation passed");
                                 db.AddCategory(category);
                                 logger.Info("Category added - {name}", category.CategoryName);
                                 
                             }
                         }
                         if (!isValid)
                         {
                             foreach (var result in results)
                             {
                                 logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                             }
                         }
                     }
                    else if (choice == "3")
                     {
                          {
                        // edit Category
                        Console.WriteLine("Choose the category to edit:");
                        var db = new NWConsole_96_IDContext();
                        var Category = GetCategories(db);
                        if (Category != null)
                        {
                            // input blog
                            Categories UpdatedCategory = InputCategory(db);
                            if (UpdatedCategory != null)
                            {
                                UpdatedCategory.CategoryId = Category.CategoryId;
                                db.EditCategory(UpdatedCategory);
                                logger.Info($"Blog (id: {Category.CategoryId}) updated");
                            }
                        }
                    }
                     }
                    else if (choice == "4")
                     {
                         Console.WriteLine("1) Display all Products");
                         Console.WriteLine("2) Display discontinued products");
                         Console.WriteLine("3) Display Active Products");
                         Console.WriteLine("4) Display specific product");

                        string DisplayChoice = Console.ReadLine();
                        if (DisplayChoice == "1")
                        {
                            var db = new NWConsole_96_IDContext();
                              var query = db.Products.OrderBy(p => p.ProductName);

                              Console.ForegroundColor = ConsoleColor.Green;
                              Console.WriteLine($"{query.Count()} records returned");
                              Console.ForegroundColor = ConsoleColor.Magenta;
                                foreach (var item in query)
                                {
                                if (item.Discontinued is false)
                                {
                                    Console.ForegroundColor = ConsoleColor.Magenta;
                                    Console.WriteLine($"{item.ProductName}");
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.DarkRed;
                                    Console.WriteLine($"{item.ProductName} -- Discontinued");
                                }
                                
                                }
                              Console.ForegroundColor = ConsoleColor.White;
                        }
                        else if (DisplayChoice == "2")
                        {
                             var db = new NWConsole_96_IDContext();
                              var query = db.Products.OrderBy(p => p.ProductName);

                              
                              Console.ForegroundColor = ConsoleColor.DarkRed;
                                foreach (var item in query)
                                {
                                if (item.Discontinued is true)
                                Console.WriteLine($"{item.ProductName}");
                                }
                              Console.ForegroundColor = ConsoleColor.White;
                        }
                        else if (DisplayChoice == "3")
                        {
                             var db = new NWConsole_96_IDContext();
                              var query = db.Products.OrderBy(p => p.ProductName);

                              
                              Console.ForegroundColor = ConsoleColor.Magenta;
                                foreach (var item in query)
                                {
                                if (item.Discontinued is false)
                                Console.WriteLine($"{item.ProductName}");
                                }
                              Console.ForegroundColor = ConsoleColor.White;
                        }
                        else if (DisplayChoice == "4")
                        {
                            var db = new NWConsole_96_IDContext();
                         var query = db.Categories.OrderBy(p => p.CategoryId);

                         Console.WriteLine("Select the category whose products you want to display:");
                         Console.ForegroundColor = ConsoleColor.DarkRed;
                         foreach (var item in query)
                         {
                             Console.WriteLine($"{item.CategoryId}) {item.CategoryName}");
                         }
                         Console.ForegroundColor = ConsoleColor.White;
                         try
                         {int id = int.Parse(Console.ReadLine());
                         Console.Clear();
                         logger.Info($"CategoryId {id} selected");
                         Categories category = db.Categories.Include("Products").FirstOrDefault(c => c.CategoryId == id);
                         Console.WriteLine($"{category.CategoryName} - {category.Description}");
                         foreach (Products p in category.Products.Where(m => m.Discontinued is false))
                         {
                             Console.WriteLine(p.ProductName);
                         }
                        } catch {logger.Error("Invalid category selected");}
                        }
                        else { logger.Error("Invalid entry, please try again");}
                     }
                    else if (choice == "5")
                    {
                           Console.WriteLine("Select the Category you want to add a Product to:");
                        var db = new NWConsole_96_IDContext();
                        var query = db.Categories.OrderBy(b => b.CategoryId);
                        
                        
                        foreach (var item in query)
                            {
                        Console.WriteLine($"{item.CategoryId}. {item.CategoryName}");
                            }    
                      
                       
                        bool success = Int32.TryParse (Console.ReadLine(),out int CategoryId); 
                        if (success)
                       { var Category = db.Categories.FirstOrDefault(m => m.CategoryId == CategoryId);
                            if (Category != null)
                            {
                            Products Product = new Products();
                            Console.WriteLine("Enter Product Name");
                            Product.ProductName = Console.ReadLine();
                            
                            
                            Product.CategoryId = CategoryId;
                            db.AddProduct(Product);
                            } 
                            else 
                            {
                            logger.Error("There are no blogs saved with that Id");
                            }    
                        }
                        else { logger.Error("Invalid entry, please try again");}
                    }
                    else if (choice == "6")
                    {
                          // edit Product
                        Console.WriteLine("Choose the product to edit:");
                        var db = new NWConsole_96_IDContext();
                        var Product = GetProducts(db);
                        if (Product != null)
                        {
                            // input Product
                            Products UpdatedProduct = InputProduct(db);
                            if (UpdatedProduct != null)
                            {
                                UpdatedProduct.ProductId = Product.ProductId;
                                db.EditProduct(UpdatedProduct);
                                logger.Info($"Blog (id: {Product.ProductId}) updated");
                            }
                        }
                    }
                
                    Console.WriteLine();

                } while (choice.ToLower() != "q");
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
             
             logger.Info("Program ended");
        }
       public static Categories GetCategories(NWConsole_96_IDContext db)
        {
            // display all Categories
            var categories = db.Categories.OrderBy(b => b.CategoryId);
            foreach (Categories b in categories)
            {
                Console.WriteLine($"{b.CategoryId}: {b.CategoryName}");
            }
            if (int.TryParse(Console.ReadLine(), out int CategoryId))
            {
                Categories Category = db.Categories.FirstOrDefault(b => b.CategoryId == CategoryId);
                if (Category != null)
                {
                    return Category;
                }
            }
            logger.Error("Invalid Category Id");
            return null;
        }
      public static Categories InputCategory(NWConsole_96_IDContext db)
        {
            Categories Category = new Categories();
            Console.WriteLine("Enter the Category name");
            Category.CategoryName = Console.ReadLine();

            ValidationContext context = new ValidationContext(Category, null, null);
            List<ValidationResult> results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(Category, context, results, true);
            if (isValid)
            {
                // check for unique name
                if (db.Categories.Any(b => b.CategoryName == Category.CategoryName))
                {
                    // generate validation error
                    isValid = false;
                    results.Add(new ValidationResult("Category name exists", new string[] { "Name" }));
                }
                else
                {
                    logger.Info("Validation passed");
                }
            }
            if (!isValid)
            {
                foreach (var result in results)
                {
                    logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                }
                return null;
            }

            return Category;
        }
         public static Products GetProducts(NWConsole_96_IDContext db)
        {
            // display all Products
            var products = db.Products.OrderBy(b => b.ProductId);
            foreach (Products b in products)
            {
                Console.WriteLine($"{b.ProductId}: {b.ProductName}");
            }
            if (int.TryParse(Console.ReadLine(), out int ProductId))
            {
                Products Product = db.Products.FirstOrDefault(b => b.ProductId == ProductId);
                if (Product != null)
                {
                    return Product;
                }
            }
            logger.Error("Invalid Product Id");
            return null;
        }
      public static Products InputProduct(NWConsole_96_IDContext db)
        {
            Products Product = new Products();
            Console.WriteLine("Enter the Product name");
            Product.ProductName = Console.ReadLine();
    

            ValidationContext context = new ValidationContext(Product, null, null);
            List<ValidationResult> results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(Product, context, results, true);
            if (isValid)
            {
                // check for unique name
                if (db.Products.Any(b => b.ProductName == Product.ProductName))
                {
                    // generate validation error
                    isValid = false;
                    results.Add(new ValidationResult("Product name exists", new string[] { "Name" }));
                }
                else
                {
                    logger.Info("Validation passed");
                }
            }
            if (!isValid)
            {
                foreach (var result in results)
                {
                    logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                }
                return null;
            }

            return Product;
        }
    }
}
