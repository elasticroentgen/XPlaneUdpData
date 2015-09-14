XPlaneUdpData
=============

XPlaneUdpData allows quick and easy access to XPlane UDP packets. With the release of
XPlane 10.40, all datarefs can now be accessed via UDP.

Disclaimer
----------
Be aware that this is my first C# .NET project that is publically available. I will
accept all feedback provided be it positive or negative. Learning curve expected.

Usage Example
-------------

```c#
using System;
using XPlaneUdpData.Core;

namespace ConsoleApplication1
{
    class Program
    {
        // UpdateLat callback
        static void UpdateLat(XPDataRefResult e)
        {
            Console.WriteLine($"Latitude = {e.dref_flt}");
        }

        // UpdateLong callback
        static void UpdateLong(XPDataRefResult e)
        {
            Console.WriteLine($"Longitude = {e.dref_flt}");
        }

        static void Main(string[] args)
        {
            // Initialize the XPlaneData object
            XPlaneData xpData = new XPlaneData("127.0.0.1", 49000);

            // Add the requested datarefs
            xpData.AddDataRef("sim/flightmodel/position/latitude", 1, UpdateLat);
            xpData.AddDataRef("sim/flightmodel/position/longitude", 1, UpdateLong);

            // Start sending UDP packets
            xpData.StartPolling();

            // Wait for ESC key to stop
            Console.WriteLine("Press ESC to stop");
            do
            {
                while (!Console.KeyAvailable)
                {
                }
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);

            // Stop sending UDP packets
            xpData.StopPolling();
        }
    }
}
```
