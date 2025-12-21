# AIObservabilityAndEvaluationWorkshop

## Requirements

- .NET 10.0 SDK or later

## Setup Instructions

### 1. Install .NET 10.0 SDK

Download and install the .NET 10.0 SDK from the official .NET download page:
- [Download .NET 10.0](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)

Verify the installation:
```bash
dotnet --version
```

### 2. Install Aspire CLI

Install the Aspire CLI using the official installation script:

**Linux/macOS:**
```bash
curl -sSL https://aspire.dev/install.sh | bash
```

**Windows (PowerShell):**
```powershell
irm https://aspire.dev/install.ps1 | iex
```

For more information, see the [Aspire Prerequisites](https://aspire.dev/get-started/prerequisites/) page.

Verify the installation:
```bash
aspire --version
```

## Troubleshooting

### Inotify Limit Error on Linux

If you encounter the error:
```
System.IO.IOException: The configured user limit (128) on the number of inotify instances has been reached
```

This is a Linux system limitation. The Aspire AppHost creates file watchers for configuration files, which can exceed the default inotify limit.

**Solution:** Increase the system inotify limit:

```bash
# Temporary (until reboot)
sudo sysctl -w fs.inotify.max_user_instances=512

# Permanent
echo "fs.inotify.max_user_instances=512" | sudo tee -a /etc/sysctl.conf
sudo sysctl -p
```

Alternatively, you can run the provided helper script:
```bash
./fix-inotify-limit.sh
```