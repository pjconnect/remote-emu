using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace RemoteEmuApp
{
    public class AdbCommands
    {
        private readonly string _adbPath;
        private const string ANDROID_SDK_PATH = "/Users/pasindujayawardana/Library/Android/sdk";
        private const string ADB_RELATIVE_PATH = "platform-tools/adb";

        public AdbCommands(string? customAdbPath = null)
        {
            _adbPath = customAdbPath ?? Path.Combine(ANDROID_SDK_PATH, ADB_RELATIVE_PATH);
        }


        /// <summary>
        /// Executes an ADB command and returns the output
        /// </summary>
        /// <param name="arguments">ADB command arguments</param>
        /// <returns>Command output</returns>
        private async Task<string> ExecuteAdbCommand(string arguments)
        {
            try
            {
                using (var process = new Process())
                {
                    process.StartInfo.FileName = _adbPath;
                    process.StartInfo.Arguments = arguments;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;
                    process.StartInfo.CreateNoWindow = true;

                    process.Start();

                    string output = await process.StandardOutput.ReadToEndAsync();
                    string error = await process.StandardError.ReadToEndAsync();

                    await process.WaitForExitAsync();

                    if (process.ExitCode != 0)
                    {
                        throw new Exception($"ADB command failed: {error}");
                    }

                    return output;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error executing ADB command '{arguments}': {ex.Message}");
            }
        }

        /// <summary>
        /// Lists all connected devices
        /// </summary>
        /// <returns>List of connected devices</returns>
        public async Task<string> ListDevices()
        {
            return await ExecuteAdbCommand("devices");
        }

        /// <summary>
        /// Starts an Android emulator by AVD name
        /// </summary>
        /// <param name="avdName">Name of the Android Virtual Device</param>
        /// <returns>Command output</returns>
        public Task<string> StartEmulator(string avdName)
        {
            try
            {
                // Use emulator command instead of adb for starting emulator
                var process = new Process();
                process.StartInfo.FileName = "emulator";
                process.StartInfo.Arguments = $"-avd {avdName}";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = false;

                process.Start();

                // Don't wait for emulator to fully start as it's a long-running process
                return Task.FromResult($"Emulator '{avdName}' started successfully");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error starting emulator '{avdName}': {ex.Message}");
            }
        }

        /// <summary>
        /// Lists all available AVDs
        /// </summary>
        /// <returns>List of available AVDs</returns>
        public async Task<string> ListAvds()
        {
            try
            {
                using (var process = new Process())
                {
                    process.StartInfo.FileName = "emulator";
                    process.StartInfo.Arguments = "-list-avds";
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;
                    process.StartInfo.CreateNoWindow = true;

                    process.Start();

                    string output = await process.StandardOutput.ReadToEndAsync();
                    await process.WaitForExitAsync();

                    return output;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error listing AVDs: {ex.Message}");
            }
        }

        /// <summary>
        /// Kills the ADB server
        /// </summary>
        /// <returns>Command output</returns>
        public async Task<string> KillServer()
        {
            return await ExecuteAdbCommand("kill-server");
        }

        /// <summary>
        /// Starts the ADB server
        /// </summary>
        /// <returns>Command output</returns>
        public async Task<string> StartServer()
        {
            return await ExecuteAdbCommand("start-server");
        }

        /// <summary>
        /// Installs an APK file to the connected device
        /// </summary>
        /// <param name="apkPath">Path to the APK file</param>
        /// <param name="deviceId">Optional device ID</param>
        /// <returns>Command output</returns>
        public async Task<string> InstallApk(string apkPath, string? deviceId = null)
        {
            if (!File.Exists(apkPath))
            {
                throw new FileNotFoundException($"APK file not found: {apkPath}");
            }

            string arguments = string.IsNullOrEmpty(deviceId)
                ? $"install \"{apkPath}\""
                : $"-s {deviceId} install \"{apkPath}\"";

            return await ExecuteAdbCommand(arguments);
        }

        /// <summary>
        /// Gets device information
        /// </summary>
        /// <param name="deviceId">Optional device ID</param>
        /// <returns>Device information</returns>
        public async Task<string> GetDeviceInfo(string? deviceId = null)
        {
            string arguments = string.IsNullOrEmpty(deviceId)
                ? "shell getprop"
                : $"-s {deviceId} shell getprop";

            return await ExecuteAdbCommand(arguments);
        }

        /// <summary>
        /// Reboots the connected device
        /// </summary>
        /// <param name="deviceId">Optional device ID</param>
        /// <returns>Command output</returns>
        public async Task<string> RebootDevice(string? deviceId = null)
        {
            string arguments = string.IsNullOrEmpty(deviceId)
                ? "reboot"
                : $"-s {deviceId} reboot";

            return await ExecuteAdbCommand(arguments);
        }

        /// <summary>
        /// Takes a screenshot and saves it to the specified path
        /// </summary>
        /// <param name="outputPath">Path to save the screenshot</param>
        /// <param name="deviceId">Optional device ID</param>
        /// <returns>Command output</returns>
        public async Task<string> TakeScreenshot(string outputPath, string? deviceId = null)
        {
            string tempPath = "/sdcard/screenshot.png";

            // Take screenshot on device
            string screenshotArgs = string.IsNullOrEmpty(deviceId)
                ? $"shell screencap -p {tempPath}"
                : $"-s {deviceId} shell screencap -p {tempPath}";

            await ExecuteAdbCommand(screenshotArgs);

            // Pull screenshot to local machine
            string pullArgs = string.IsNullOrEmpty(deviceId)
                ? $"pull {tempPath} \"{outputPath}\""
                : $"-s {deviceId} pull {tempPath} \"{outputPath}\"";

            return await ExecuteAdbCommand(pullArgs);
        }
    }
}
