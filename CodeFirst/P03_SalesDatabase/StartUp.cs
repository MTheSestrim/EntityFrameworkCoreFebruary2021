using P03_SalesDatabase.Data;
using P03_SalesDatabase.Data.Models;
using System;
using System.Collections.Generic;

namespace P03_SalesDatabase
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            var context = new SalesContext();

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

           CustomSaleInitializer.SeedDatabase(context);
        }
    }
}
