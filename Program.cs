using System;

namespace CeVIO_crack
{
    class Program
    {
        // N54KC-7U2ZL-PQZBM-SPF8H suzuki trial key
        static void Main(string[] args)
        {
            Console.Write("CeVIO*.exe: ");
            var path = Console.ReadLine().Replace("\"","");
            var activator = new Activator(path);

            Console.WriteLine("Loading...");

            activator.ActivateProducts();
            Console.WriteLine("Activated all packages");

            activator.GenerateLicenseSummary();
            Console.WriteLine("Authorized");

            Console.WriteLine("Completed");
            Console.WriteLine("You should reactivate CeVIO AI before " + DateTime.Now.AddDays(365).ToLongDateString());
            Console.ReadLine();
        }
    }
}