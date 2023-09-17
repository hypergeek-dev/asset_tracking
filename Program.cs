using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace AssetTracker
{
    public enum OfficeLocation
    {
        Unknown,
        Spain,
        Sweden,
        USA
    }

    public enum ProductStatus
    {
        Normal,
        Yellow,
        Red
    }

    public class Product
    {
        public int Id { get; set; }
        public string TypeOfProduct { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public OfficeLocation Office { get; set; }
        public DateTime Date { get; set; }
        public decimal Price { get; set; }
    }

    public class AssetContext : DbContext
    {
        public DbSet<Product> Products { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=MAINCORE\SQLEXPRESS;Database=AssetTrackingDB;Trusted_Connection=True;TrustServerCertificate=true;");


        }

    }

    class Program
    {
        static void Main()
        {
            using (var db = new AssetContext())
            {
                db.Database.EnsureCreated();
            }

            while (true)
            {
                Console.WriteLine("Please choose an option:");
                Console.WriteLine("1. Add a product");
                Console.WriteLine("2. List all products");
                Console.WriteLine("3. Exit");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddProduct();
                        break;
                    case "2":
                        ListProducts();
                        break;
                    case "3":
                        Console.WriteLine("Goodbye!");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }

                Console.WriteLine();
            }
        }

        static void AddProduct()
        {
            Console.WriteLine("Please write the type of the product:");
            string type = Console.ReadLine();

            Console.WriteLine("Please write the brand of the product:");
            string brand = Console.ReadLine();

            Console.WriteLine("Please write the model of the product:");
            string model = Console.ReadLine();

            Console.WriteLine("Please choose the office, where it is located.");
            Console.WriteLine("(S)weden, (E)Spain or (U)USA?:");
            string officeInput = Console.ReadLine();
            OfficeLocation office = ParseOfficeLocation(officeInput);

            Console.WriteLine("Please write the purchase date of the product (dd/MM/yyyy):");
            DateTime date;
            while (!DateTime.TryParseExact(Console.ReadLine(), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out date))
            {
                Console.WriteLine("Invalid date format. Please try again (dd/MM/yyyy):");
            }

            Console.WriteLine("Please write the price of the product:");
            decimal price;
            while (!decimal.TryParse(Console.ReadLine(), out price))
            {
                Console.WriteLine("Invalid price format. Please try again:");
            }

            using (var db = new AssetContext())
            {
                var product = new Product { TypeOfProduct = type, Brand = brand, Model = model, Office = office, Date = date, Price = price };
                db.Products.Add(product);
                db.SaveChanges();
            }
        }

        static OfficeLocation ParseOfficeLocation(string input)
        {
            switch (input.ToUpper())
            {
                case "S":
                    return OfficeLocation.Sweden;
                case "E":
                    return OfficeLocation.Spain;
                case "U":
                    return OfficeLocation.USA;
                default:
                    return OfficeLocation.Unknown;
            }
        }

        static void ListProducts()
        {
            using (var db = new AssetContext())
            {
                var products = db.Products.ToList();

                if (products.Count == 0)
                {
                    Console.WriteLine("No products found.");
                    return;
                }

                var sortedProducts = products.OrderBy(p => p.Office).ThenBy(p => p.Date).ToList();

                Console.WriteLine("{0,-10}{1,-10}{2,-10}{3,-10}{4,-15}{5,-14}{6,-10}{7,-22}",
                    "Type", "Brand", "Model", "Office", "Purchase Date", "Price in USD", "Currency", "Local Price Today");
                Console.WriteLine("{0,-10}{1,-10}{2,-10}{3,-10}{4,-15}{5,-14}{6,-10}{7,-22}",
                    "-----", "-----", "-----", "------", "-------------", "------------", "--------", "------------------");

                foreach (var product in sortedProducts)
                {
                    var countryInfo = GetCountryInfo(product.Office);
                    decimal localPriceToday = product.Price * countryInfo.ExchangeRate;

                    ProductStatus status = GetProductStatus(product.Date);
                    Console.ForegroundColor = status == ProductStatus.Red ? ConsoleColor.Red : (status == ProductStatus.Yellow ? ConsoleColor.Yellow : Console.ForegroundColor);

                    Console.WriteLine("{0,-10}{1,-10}{2,-10}{3,-10}{4,-15:dd/MM/yyyy}{5,-14:#,##0.00}{6,-10}{7,-22:#,##0.00}",
                        product.TypeOfProduct, product.Brand, product.Model, countryInfo.Country, product.Date, product.Price, countryInfo.Currency, localPriceToday);
                    Console.ResetColor();
                }
            }
        }

        static (string Country, string Currency, decimal ExchangeRate) GetCountryInfo(OfficeLocation office)
        {
            switch (office)
            {
                case OfficeLocation.Sweden:
                    return ("Sweden", "SEK", 10.54m);
                case OfficeLocation.Spain:
                    return ("Spain", "EUR", 0.90m);
                case OfficeLocation.USA:
                    return ("USA", "USD", 1.00m);
                default:
                    throw new InvalidOperationException($"Unknown office location: {office}");
            }
        }

        static ProductStatus GetProductStatus(DateTime purchaseDate)
        {
            TimeSpan timeSpan = DateTime.Now - purchaseDate;
            if (timeSpan >= TimeSpan.FromDays(365 * 3 - 90))
            {
                return ProductStatus.Red;
            }
            else if (timeSpan >= TimeSpan.FromDays(365 * 3 - 180))
            {
                return ProductStatus.Yellow;
            }

            return ProductStatus.Normal;
        }
    }
}
