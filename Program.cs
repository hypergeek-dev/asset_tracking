// Importing necessary libraries.
using System;
using System.Collections.Generic;
using System.Linq;

namespace AssetTracker
{
    // OfficeLocation and ProductStatus.
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

    // Product class definition with properties and constructor.
    public class Product
    {
        public Product(string type, string brand, string model, OfficeLocation office, DateTime date, decimal price)
        {
            TypeOfProduct = type;
            Brand = brand;
            Model = model;
            Office = office;
            Date = date;
            Price = price;
        }

        // Product properties.
        public string TypeOfProduct { get; }
        public string Brand { get; }
        public string Model { get; }
        public OfficeLocation Office { get; }
        public DateTime Date { get; }
        public decimal Price { get; }
    }

    // The main program.
    class Program
    {
        static void Main()
        {
            // List of products with sample data.
            var products = new List<Product>
            {
                new Product("Phone", "iPhone", "8", OfficeLocation.Spain, new DateTime(2018, 12, 29), 970),
                new Product("Computer", "HP", "Elitebook", OfficeLocation.Spain, new DateTime(2019, 6, 1), 1423),
                new Product("Phone", "iPhone", "11", OfficeLocation.Spain, new DateTime(2022, 6, 25), 990),
                new Product("Phone", "iPhone", "X", OfficeLocation.Sweden, new DateTime(2018, 7, 15), 1245),
                new Product("Phone", "Motorola", "Razr", OfficeLocation.Sweden, new DateTime(2022, 12, 16), 970),
                new Product("Computer", "HP", "Elitebook", OfficeLocation.Sweden, new DateTime(2023, 10, 2), 588),
                new Product("Computer", "Asus", "W234", OfficeLocation.USA, new DateTime(2017, 4, 21), 1200),
                new Product("Computer", "Lenovo", "Yoga 730", OfficeLocation.USA, new DateTime(2018, 5, 28), 835),
                new Product("Computer", "Lenovo", "Yoga 530", OfficeLocation.USA, new DateTime(2019, 5, 21), 1030)

            };

            // Loop to interact with the user.
            while (true)
            {
                // Menu displayed to the user.
                Console.WriteLine("Please choose an option:");
                Console.WriteLine("1. Add a product");
                Console.WriteLine("2. List all products");
                Console.WriteLine("3. Exit");
                string choice = Console.ReadLine();

                // Decision based on user input.
                switch (choice)
                {
                    case "1":
                        // If user chose to add a product.
                        AddProduct(products);
                        break;
                    case "2":
                        // If user chose to list all products.
                        ListProducts(products);
                        break;
                    case "3":
                        // If user chose to exit.
                        Console.WriteLine("Goodbye!");
                        return;
                    default:
                        // If user input doesn't match any options.
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }

                Console.WriteLine();
            }
        }

        // Function to add a product to the list.
        static void AddProduct(List<Product> products)
        {
            // Reading product details from the user.
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

            // Adding the new product to the list.
            products.Add(new Product(type, brand, model, office, date, price));
        }

        // Function to display all products.
        static void ListProducts(List<Product> products)
        {
            // Checking if there are any products.
            if (products.Count == 0)
            {
                Console.WriteLine("No products found.");
                return;
            }

            // Sorting the products by Office and Date.
            var sortedProducts = products.OrderBy(p => p.Office).ThenBy(p => p.Date).ToList();

            // Printing the header.
            Console.WriteLine("{0,-10}{1,-10}{2,-10}{3,-10}{4,-15}{5,-14}{6,-10}{7,-22}",
                "Type", "Brand", "Model", "Office", "Purchase Date", "Price in USD", "Currency", "Local Price Today");
            Console.WriteLine("{0,-10}{1,-10}{2,-10}{3,-10}{4,-15}{5,-14}{6,-10}{7,-22}",
                "-----", "-----", "-----", "------", "-------------", "------------", "--------", "------------------");

            // Displaying the products.
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


        // Function to parse the office location.
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

        // Function to get country information based on the office location.
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

        // Function to calculate the product status based on the purchase date.
        static ProductStatus GetProductStatus(DateTime purchaseDate)
        {
            TimeSpan timeSpan = DateTime.Now - purchaseDate;
            if (timeSpan >= TimeSpan.FromDays(365 * 3 - 90))  // If the timespan is greater or equal to 3 years less 90 days
            {
                return ProductStatus.Red;
            }
            else if (timeSpan >= TimeSpan.FromDays(365 * 3 - 180)) // If the timespan is greater or equal to 3 years less 180 days
            {
                return ProductStatus.Yellow;
            }

            return ProductStatus.Normal;
        }
    }
}
