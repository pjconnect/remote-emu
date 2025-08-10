# Remote Emulator Manager

A simple .NET Core console application for managing Android emulators and executing ADB commands.

## Prerequisites

- .NET 6.0 or later
- Android SDK with ADB and Emulator tools installed
- ADB and Emulator commands available in your system PATH

## Setup

1. Make sure Android SDK is installed and ADB is in your PATH:
   ```bash
   adb version
   emulator -version
   ```

2. Build and run the application:
   ```bash
   cd RemoteEmuApp
   dotnet build
   dotnet run
   ```

## Features

The application provides a simple menu-driven interface with the following capabilities:

### ADB Commands
- **List connected devices**: Shows all connected Android devices and emulators
- **Get device info**: Displays key device properties
- **Take screenshot**: Captures a screenshot from the connected device
- **Kill/Start ADB server**: Manages the ADB server process

### Emulator Management
- **List available AVDs**: Shows all Android Virtual Devices configured on your system
- **Start emulator**: Launches an emulator by AVD name

## AdbCommands Class

The `AdbCommands` class provides the following methods:

### Device Management
- `ListDevices()` - Lists all connected devices
- `GetDeviceInfo(deviceId)` - Gets device properties
- `RebootDevice(deviceId)` - Reboots a device

### Emulator Management
- `ListAvds()` - Lists available Android Virtual Devices
- `StartEmulator(avdName)` - Starts an emulator

### ADB Server
- `StartServer()` - Starts the ADB server
- `KillServer()` - Kills the ADB server

### App Management
- `InstallApk(apkPath, deviceId)` - Installs an APK file

### Utilities
- `TakeScreenshot(outputPath, deviceId)` - Takes a screenshot

## Usage Examples

```csharp
var adb = new AdbCommands();

// List devices
string devices = await adb.ListDevices();

// Start an emulator
await adb.StartEmulator("Pixel_4_API_30");

// Take a screenshot
await adb.TakeScreenshot("screenshot.png");

// Install an APK
await adb.InstallApk("/path/to/app.apk");
```

## Error Handling

The application includes comprehensive error handling for:
- Missing ADB/Emulator tools
- Invalid device IDs
- File not found errors
- Command execution failures

## Notes

- Make sure your Android SDK tools are properly installed and configured
- Emulator startup is a background process and may take some time to fully load
- Screenshots are temporarily stored on the device before being pulled to your local machine
