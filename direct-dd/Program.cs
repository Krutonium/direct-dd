using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;

class Program
{
    private static readonly int BufferSize = 4 * 1024 * 1024; // 4 MB

    static async Task Main(string[] args)
    {
        if (args.Length != 2)
        {
            Console.Error.WriteLine("Usage: dotnet run <image-url> </dev/sdX>");
            return;
        }

        string url = args[0];
        string devicePath = args[1];

        Console.WriteLine($"Downloading image from: {url}");
        Console.WriteLine($"Writing to device: {devicePath}");
        Console.WriteLine($"Buffer size: {BufferSize / 1024} KB");

        try
        {
            using var httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromHours(1)
            };

            string expectedHash = null;
            try
            {
                var sha256Response = await httpClient.GetStringAsync($"{url}.sha256");
                expectedHash = sha256Response.Split(' ')[0].Trim();
                Console.WriteLine($"Found SHA256 hash: {expectedHash}");
            }
            catch
            {
                Console.WriteLine("No SHA256 hash file found, will write without verification");
            }

            using var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            var contentLength = response.Content.Headers.ContentLength;

            if (contentLength.HasValue)
                Console.WriteLine($"Remote file size: {contentLength.Value / (1024 * 1024)} MB");

            using var inputStream = await response.Content.ReadAsStreamAsync();

            string writtenHash = null;
            
            using (var sha256 = expectedHash != null ? SHA256.Create() : null)
            using (var deviceStream = new FileStream(
                devicePath,
                FileMode.Open,
                FileAccess.Write,
                FileShare.None,
                BufferSize,
                FileOptions.WriteThrough))
            {
                byte[] buffer = new byte[BufferSize];
                long totalWritten = 0;
                int bytesRead;
                var sw = Stopwatch.StartNew();

                Console.WriteLine("Writing...");

                while ((bytesRead = await inputStream.ReadAsync(buffer)) > 0)
                {
                    await deviceStream.WriteAsync(buffer.AsMemory(0, bytesRead));
                    sha256?.TransformBlock(buffer, 0, bytesRead, null, 0);
                    totalWritten += bytesRead;

                    var elapsed = sw.Elapsed;
                    double speedMBps = totalWritten / (1024.0 * 1024.0) / elapsed.TotalSeconds;

                    string progressLine = $"\rWritten: {totalWritten / (1024 * 1024)} MB @ {speedMBps:F2} MB/s";

                    if (contentLength.HasValue)
                    {
                        double percent = (double)totalWritten / contentLength.Value * 100.0;
                        double estimatedSeconds = elapsed.TotalSeconds * (contentLength.Value - totalWritten) / totalWritten;
                        string eta = TimeSpan.FromSeconds(estimatedSeconds).ToString(@"hh\:mm\:ss");
                        progressLine += $" ({percent:F1}%) ETA: {eta}";
                    }

                    Console.Write(progressLine);
                }

                if (sha256 != null)
                {
                    sha256.TransformFinalBlock(Array.Empty<byte>(), 0, 0);
                    writtenHash = BitConverter.ToString(sha256.Hash!).Replace("-", "").ToLowerInvariant();
                }
            }

            Console.WriteLine("\nWrite complete.");
            
            if (expectedHash != null && writtenHash != null)
            {
                if (writtenHash.Equals(expectedHash, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("✅ Verification successful: SHA256 hash matches.");
                }
                else
                {
                    Console.WriteLine("❌ Verification failed: SHA256 hash does NOT match.");
                    Console.WriteLine($"Expected: {expectedHash}");
                    Console.WriteLine($"Actual:   {writtenHash}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"\nError: {ex.Message}");
        }
    }
}