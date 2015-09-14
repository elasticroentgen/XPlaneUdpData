XPlaneUdpData
=============

XPlaneUdpData allows quick and easy access to XPlane UDP packets. With the release of
XPlane 10.40, all datarefs can now be accessed via UDP.

Usage Example
-------------

```c#
// Include the appropriate reference
using XPlaneUdpData.Core;

// Initialize the XPlaneData object
XPlaneData xpData = new XPlaneData("127.0.0.1", 49000);

// Create the required callbacks
void UpdateLat(XPDataRefResult e)
{
    Dispatcher.Invoke(() =>
    {
        Console.WriteLine($"Latitude = {e.dref_flt}");
    });
}

void UpdateLong(XPDataRefResult e)
{
    Dispatcher.Invoke(() =>
    {
        Console.WriteLine($"Longitude = {e.dref_flt}");
    });
}

// Add the requested datarefs
xpData.AddDataRef("sim/flightmodel/position/latitude", 1, UpdateLat);
xpData.AddDataRef("sim/flightmodel/position/longitude", 1, UpdateLong);

// Start sending UDP packets
xpData.StartPolling();

...

// Stop sending UDP packets
xpData.StopPolling();
```
