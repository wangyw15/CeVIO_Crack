using System;

namespace CeVIO_crack
{
    class Program
    {
        // N54KC-7U2ZL-PQZBM-SPF8H suzuki trial key
        static void Main(string[] args)
        {
            ActivateUI();
        }

        static void ActivateUI()
        {
            Console.WriteLine("1. CeVIO AI");
            Console.WriteLine("2. CeVIO Creative Studio");
            Console.WriteLine("select (default 1): ");
            var choice = Console.ReadLine();
            if (choice == "1" || choice == "")
            {
                ActivateCeVIOAI("00000-00000-00000-00000");
            }
            else if (choice == "2")
            {
                ActivateCeVIOCS();
            }
            Console.ReadLine();
        }

        static void ActivateCeVIOAI(string key = "00000-00000-00000-00000")
        {
            var activator = new AIActivator();
            activator.ActivationKey = key;
            activator.ActivateProducts();
            Console.WriteLine("Activated all packages");

            activator.GenerateLicenseSummary();
            Console.WriteLine("Authorized");

            Console.WriteLine("All completed");
            Console.WriteLine("You should reactivate CeVIO AI before " + DateTime.Now.AddDays(365).ToLongDateString());
        }

        static void ActivateCeVIOCS(string key = "00000-00000-00000-00000")
        {
            var activator = new CSActivator();
            activator.ActivationKey = key;
            activator.ActivateProducts();
            Console.WriteLine("Activated all packages");

            activator.GenerateLicenseSummary();
            Console.WriteLine("Authorized");

            Console.WriteLine("All completed");
            Console.WriteLine("You should reactivate CeVIO CS before " + DateTime.Now.AddDays(365).ToLongDateString());
        }
    }
}