using RemoteEmuApp;
using System;
using System.Threading.Tasks;

namespace RemoteEmuApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Remote Emulator Manager");
            Console.WriteLine("======================");

            var adb = new AdbCommands();

            try
            {
                // Display menu
                await DisplayMenu(adb);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine("Make sure ADB is installed and available in your PATH.");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        static async Task DisplayMenu(AdbCommands adb)
        {
            while (true)
            {
                Console.WriteLine("\nSelect an option:");
                Console.WriteLine("1. List connected devices");
                Console.WriteLine("2. List available AVDs");
                Console.WriteLine("3. Start emulator");
                Console.WriteLine("4. Get device info");
                Console.WriteLine("5. Take screenshot");
                Console.WriteLine("6. Kill ADB server");
                Console.WriteLine("7. Start ADB server");
                Console.WriteLine("0. Exit");
                Console.Write("\nEnter your choice: ");

                string? choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            await ListDevices(adb);
                            break;
                        case "2":
                            await ListAvds(adb);
                            break;
                        case "3":
                            await StartEmulator(adb);
                            break;
                        case "4":
                            await GetDeviceInfo(adb);
                            break;
                        case "5":
                            await TakeScreenshot(adb);
                            break;
                        case "6":
                            await KillAdbServer(adb);
                            break;
                        case "7":
                            await StartAdbServer(adb);
                            break;
                        case "0":
                            return;
                        default:
                            Console.WriteLine("Invalid choice. Please try again.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }

        static async Task ListDevices(AdbCommands adb)
        {
            Console.WriteLine("\nListing connected devices...");
            string result = await adb.ListDevices();
            Console.WriteLine(result);
        }

        static async Task ListAvds(AdbCommands adb)
        {
            Console.WriteLine("\nListing available AVDs...");
            string result = await adb.ListAvds();
            Console.WriteLine(string.IsNullOrEmpty(result.Trim()) ? "No AVDs found." : result);
        }

        static async Task StartEmulator(AdbCommands adb)
        {
            Console.Write("Enter AVD name: ");
            string? avdName = Console.ReadLine();
            
            if (!string.IsNullOrEmpty(avdName))
            {
                Console.WriteLine($"\nStarting emulator '{avdName}'...");
                string result = await adb.StartEmulator(avdName);
                Console.WriteLine(result);
            }
            else
            {
                Console.WriteLine("Invalid AVD name.");
            }
        }

        static async Task GetDeviceInfo(AdbCommands adb)
        {
            Console.WriteLine("\nGetting device information...");
            string result = await adb.GetDeviceInfo();
            
            // Display only key properties for brevity
            var lines = result.Split('\n');
            Console.WriteLine("Key Device Properties:");
            foreach (var line in lines)
            {
                if (line.Contains("ro.product.model") || 
                    line.Contains("ro.build.version.release") || 
                    line.Contains("ro.product.manufacturer") ||
                    line.Contains("ro.build.version.sdk"))
                {
                    Console.WriteLine($"  {line.Trim()}");
                }
            }
        }

        static async Task TakeScreenshot(AdbCommands adb)
        {
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string outputPath = $"screenshot_{timestamp}.png";
            
            Console.WriteLine($"\nTaking screenshot and saving as '{outputPath}'...");
            string result = await adb.TakeScreenshot(outputPath);
            Console.WriteLine("Screenshot saved successfully!");
        }

        static async Task KillAdbServer(AdbCommands adb)
        {
            Console.WriteLine("\nKilling ADB server...");
            string result = await adb.KillServer();
            Console.WriteLine("ADB server killed.");
        }

        static async Task StartAdbServer(AdbCommands adb)
        {
            Console.WriteLine("\nStarting ADB server...");
            string result = await adb.StartServer();
            Console.WriteLine("ADB server started.");
        }
    }
}
