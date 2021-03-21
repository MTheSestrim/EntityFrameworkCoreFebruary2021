namespace BookShop
{
    using BookShop.Initializer;
    using BookShop.Models.Enums;
    using Data;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
            DbInitializer.ResetDatabase(db);

            Console.WriteLine(RemoveBooks(db));
        }

        public static string GetBookresultyAgeRestriction(BookShopContext context, string command)
        {
            Dictionary<string, AgeRestriction> comparator = new Dictionary<string, AgeRestriction>()
            {
                { "minor", AgeRestriction.Minor },
                { "teen", AgeRestriction.Teen },
                { "adult", AgeRestriction.Adult },
            };
            AgeRestriction enumValue;
            bool ageRestrictionExists = comparator.TryGetValue(command.ToLower(), out enumValue);

            if (ageRestrictionExists)
            {
                var books = context
                    .Books
                    .Where(b => b.AgeRestriction == enumValue)
                    .Select(b => new { b.Title })
                    .OrderBy(b => b.Title)
                    .ToList();

                StringBuilder result = new StringBuilder();

                foreach (var book in books)
                {
                    result.AppendLine(book.Title);
                }

                return result.ToString().TrimEnd();
            }

            return "";
        }

        public static string GetGoldenBooks(BookShopContext context)
        {
            var goldenBooks = context
                .Books
                .Where(b => b.EditionType == EditionType.Gold && b.Copies < 5000)
                .OrderBy(b => b.BookId)
                .Select(b => new { b.Title })
                .ToList();

            StringBuilder result = new StringBuilder();

            foreach (var book in goldenBooks)
            {
                result.AppendLine(book.Title);
            }

            return result.ToString().TrimEnd();
        }

        public static string GetBookresultyPrice(BookShopContext context)
        {
            var books = context
                .Books
                .Select(b => new { b.Title, b.Price })
                .Where(b => b.Price > 40)
                .OrderByDescending(b => b.Price)
                .ToList();

            StringBuilder result = new StringBuilder();

            foreach (var book in books)
            {
                result.AppendLine($"{book.Title} - ${book.Price:F2}");
            }

            return result.ToString().TrimEnd();
        }

        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            var books = context
                        .Books
                        .Where(b => b.ReleaseDate.Value.Year != year)
                        .OrderBy(b => b.BookId)
                        .Select(b => new { b.Title })
                        .ToList();

            StringBuilder result = new StringBuilder();

            foreach (var book in books)
            {
                result.AppendLine($"{book.Title}");
            }

            return result.ToString().TrimEnd();
        }

        public static string GetBookresultyCategory(BookShopContext context, string input)
        {
            string[] categories = input
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(cat => cat.ToLower())
                .ToArray();

            var books = context
                .Books
                .Select(b => new { b.Title, b.BookCategories })
                .Where(b => b.BookCategories.Any(bc => categories.Contains(bc.Category.Name.ToLower())))
                .OrderBy(b => b.Title)
                .ToList();

            StringBuilder result = new StringBuilder();

            foreach (var book in books)
            {
                result.AppendLine(book.Title);
            }

            return result.ToString().TrimEnd();
        }

        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            DateTime parsedDate = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.CurrentCulture);

            var books = context
                .Books
                .Where(b => b.ReleaseDate < parsedDate)
                .OrderByDescending(b => b.ReleaseDate)
                .Select(b => new { b.Title, b.EditionType, b.Price })
                .ToList();

            StringBuilder result = new StringBuilder();

            foreach (var book in books)
            {
                result.AppendLine($"{book.Title} - {book.EditionType} - ${book.Price:F2}");
            }

            return result.ToString().TrimEnd();
        }

        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var authors = context
                .Authors
                .Select(a => new { a.FirstName, a.LastName })
                .Where(a => a.FirstName.EndsWith(input))
                .OrderBy(a => a.FirstName)
                .ThenBy(a => a.LastName)
                .ToList();

            StringBuilder result = new StringBuilder();

            foreach (var author in authors)
            {
                result.AppendLine($"{author.FirstName} {author.LastName}");
            }

            return result.ToString().TrimEnd();
        }

        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            input = input.ToLower();

            var books = context
                .Books
                .Select(b => new { b.Title })
                .Where(b => b.Title.ToLower().Contains(input))
                .OrderBy(b => b.Title)
                .ToList();

            StringBuilder result = new StringBuilder();

            foreach (var book in books)
            {
                result.AppendLine(book.Title);
            }

            return result.ToString().TrimEnd();
        }

        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            var books = context
                .Books
                .OrderBy(b => b.BookId)
                .Select(b => new
                {
                    b.Title,
                    AuthorFirstName = b.Author.FirstName,
                    AuthorLastName = b.Author.LastName
                })
                .Where(b => EF.Functions.Like(b.AuthorLastName, $"{input}%"))
                .ToList();

            StringBuilder result = new StringBuilder();

            foreach (var book in books)
            {
                result.AppendLine($"{book.Title} ({book.AuthorFirstName} {book.AuthorLastName})");
            }

            return result.ToString();
        }

        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            int count = context.Books.Where(b => b.Title.Length > lengthCheck).Count();

            return count;
        }

        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var authors = context
                .Authors
                .Select(a => new
                {
                    FullName = a.FirstName + " " + a.LastName,
                    TotalCopies = a.Books.Sum(b => b.Copies)
                })
                .OrderByDescending(a => a.TotalCopies)
                .ToList();

            StringBuilder result = new StringBuilder();

            foreach (var author in authors)
            {
                result.AppendLine($"{author.FullName} - {author.TotalCopies}");
            }

            return result.ToString();
        }

        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var categories = context
                .Categories
                .Select(c => new
                {
                    c.Name,
                    TotalProfit = c.CategoryBooks.Sum(cb => cb.Book.Price * cb.Book.Copies)
                })
                .OrderByDescending(c => c.TotalProfit)
                .ThenBy(c => c.Name)
                .ToList();

            StringBuilder result = new StringBuilder();

            foreach (var category in categories)
            {
                result.AppendLine($"{category.Name} ${category.TotalProfit:F2}");
            }

            return result.ToString();
        }

        public static string GetMostRecentBooks(BookShopContext context)
        {
            var categories = context
                .Categories
                .Select(c => new
                {
                    CategoryName = c.Name,
                    BooksInfo = c
                        .CategoryBooks
                        .Select(cb => new { cb.Book.Title, cb.Book.ReleaseDate })
                        .OrderByDescending(cb => cb.ReleaseDate)
                        .Take(3)
                })
                .OrderBy(c => c.CategoryName)
                .ToList();

            StringBuilder result = new StringBuilder();

            foreach (var category in categories)
            {
                result.AppendLine($"--{category.CategoryName}");

                foreach (var book in category.BooksInfo)
                {
                    result.AppendLine($"{book.Title} ({book.ReleaseDate.Value.Year})");
                }
            }

            return result.ToString();
        }

        public static void IncreasePrices(BookShopContext context)
        {
            var books = context
                .Books
                .Where(b => b.ReleaseDate.HasValue && b.ReleaseDate.Value.Year < 2010)
                .ToList();

            foreach (var book in books)
            {
                book.Price += 5;
            }

            context.SaveChanges();
        }

        public static int RemoveBooks(BookShopContext context)
        {
            var books = context
                .Books
                .Where(b => b.Copies < 4200)
                .ToList();

            context.Books.RemoveRange(books);

            context.SaveChanges();

            return books.Count;
        }
    }
}
