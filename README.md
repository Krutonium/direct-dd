# direct-dd

## Overview
This project is a simple yet effective command-line utility for downloading and writing disk images to a specified device. It is designed to handle large files efficiently and supports optional SHA256 hash verification to ensure the integrity of the downloaded image.

The application applies modern development techniques and is written in **C# 13.0**, targeting the **.NET 9.0** runtime.

A Flake has been provided for your convenience.

---

## Features
- **Efficient Data Handling**: Optimized for downloading and writing large images quickly using a 4 MB buffer.
- **Memory Efficiency**: In my testing, Never went over 70MB of memory usage.
- **SHA256 Verification**: Verifies the integrity of the downloaded file if a `.sha256` file is available at the source.
- **Progress Display**: Real-time progress updates while downloading and writing the image, including:
    - Total data written
    - Writing speed in MB/s
    - Time remaining (ETA)

---

## Usage
The program takes two command-line arguments:
1. `<image-url>` - The URL of the image file to download.
2. `</dev/sdX>` - The device path where the image will be written.

**Example Command**:
```shell script
dotnet run <image-url> </dev/sdX>
```
Or if it's installed on your system (EG via flakes:)
```shell script
direct-dd <image-url> </dev/sdX>
```


### Output Details
- The program provides detailed logs, showing:
    - Image download progress and remote file size
    - Writing progress and speed metrics
    - SHA256 verification results (if applicable)

---

## Prerequisites
- .NET 9.0 runtime installed on your system
- Root permissions for accessing the target device (e.g., `/dev/sdX` on Linux)

---

## How It Works
1. The program fetches the image file from the specified URL
2. It attempts to fetch a SHA256 checksum file (`<image-url>.sha256`) to verify file integrity.
3. Writes the image to the target device (`</dev/sdX>`)
4. Provides real-time progress updates and a final verification summary. 
 - Note that it isn't a *read back* verification, it's just checking that the data that was downloaded was correct.

---

## Error Handling
The following error cases are handled gracefully:
- **Insufficient Arguments**: Displays usage instructions if incorrect arguments are passed.
- **Download Failures**: Errors during file download are displayed to the user.
- **Write Permissions**: If the target device is inaccessible, an error is reported.
- **Verification Mismatch**: Alerts the user if the SHA256 hash does not match.

---

## License
This project is licensed under the **MIT License**. See the [LICENSE.md](LICENSE.md) file for details.

---

## Contributing
Contributions are welcome! If you have suggestions for improvement or find a bug, feel free to submit an issue or create a pull request.

---

## Contact
For any questions, reach out to **Krutonium**, the author of this project.

--- 

### Example Output
```
Downloading image from: https://example.com/image.img
Writing to device: /dev/sdX
Buffer size: 4096 KB
Found SHA256 hash: abcdef123456789...
Remote file size: 100 MB
Writing...

Written: 50 MB @ 5 MB/s (50.0%) ETA: 00:00:10
Writing complete.

✅ Verification successful: SHA256 hash matches.
```


Enjoy the simplicity and reliability of securely downloading and writing disk images, without having to download them first! 🖥️