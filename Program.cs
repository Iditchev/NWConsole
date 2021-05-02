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
                    Console.WriteLine("6) ");
                    Console.WriteLine("7) Edit Product");
                    Console.WriteLine("8) Display All Products");
                    Console.WriteLine("9) Display single product");
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
                           //TODO Edit Category
                    }
                    else if (choice == "6")
                    {
                        //TODO Add Product
                    }
                    else if (choice == "7")
                    {
                        //TODO Edit Product
                    }
                    else if (choice == "8")
                    {
                        //TODO Display Products, user decides if they want to see all products, discontinued products, or active products.
                    }
                    else if (choice == "9")
                    {
                        //TODO Display single product of choosing.
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
            logger.Error("Invalid Blog Id");
            return null;
        }
    }
}
