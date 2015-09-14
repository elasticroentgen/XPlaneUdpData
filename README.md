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

# Check that the job is listed
$ helios jobs

# List helios hosts
$ helios hosts

# Deploy the nginx job on one of the hosts
$ helios deploy nginx:v1 <host>

# Check the job status
$ helios status

# Curl the nginx container when it's started running
$ curl <host>:8080

# Undeploy the nginx job
$ helios undeploy -a nginx:v1

# Remove the nginx job
$ helios remove nginx:v1
```
