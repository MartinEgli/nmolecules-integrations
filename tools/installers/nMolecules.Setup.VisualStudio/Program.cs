using System.Diagnostics;

namespace nMolecules.Setup.VisualStudio;

internal static class Program
{
    private const int Success = 0;
    private const int InvalidArguments = 1;
    private const int MissingFile = 2;
    private const int MissingInstaller = 3;

    private static int Main(string[] args)
    {
        var options = Options.Parse(args);

        if (!options.IsValid(out var validationError))
        {
            Console.Error.WriteLine(validationError);
            PrintUsage();
            return InvalidArguments;
        }

        var vsixPath = ResolveVsixPath(options.VsixPath);

        if (vsixPath is null)
        {
            Console.Error.WriteLine("Could not find the Visual Studio extension package (.vsix).");
            Console.Error.WriteLine("Use --vsix <path> or place nMolecules.Analyzers.VisualStudio.vsix next to this setup executable.");
            return MissingFile;
        }

        var installerPath = ResolveInstallerPath(options.InstallerPath);

        if (installerPath is null)
        {
            Console.Error.WriteLine("Could not locate VSIXInstaller.exe.");
            Console.Error.WriteLine("Install Visual Studio 2022/2026 or pass --installer <path-to-VSIXInstaller.exe>.");
            return MissingInstaller;
        }

        Console.WriteLine($"Using installer: {installerPath}");
        Console.WriteLine($"Installing package: {vsixPath}");

        var arguments = options.Quiet
            ? $"/quiet \"{vsixPath}\""
            : $"\"{vsixPath}\"";

        var process = Process.Start(new ProcessStartInfo
        {
            FileName = installerPath,
            Arguments = arguments,
            UseShellExecute = false
        });

        if (process is null)
        {
            Console.Error.WriteLine("Failed to start VSIXInstaller.exe.");
            return MissingInstaller;
        }

        process.WaitForExit();
        return process.ExitCode;
    }

    private static string? ResolveVsixPath(string? explicitPath)
    {
        if (!string.IsNullOrWhiteSpace(explicitPath))
        {
            var fullPath = Path.GetFullPath(explicitPath);
            return File.Exists(fullPath) ? fullPath : null;
        }

        var localCandidates = new[]
        {
            Path.Combine(AppContext.BaseDirectory, "nMolecules.Analyzers.VisualStudio.vsix"),
            Path.Combine(AppContext.BaseDirectory, "nMolecules.Analyzers.Vsix.vsix")
        };

        foreach (var candidate in localCandidates)
        {
            if (File.Exists(candidate))
            {
                return candidate;
            }
        }

        return null;
    }

    private static string? ResolveInstallerPath(string? explicitPath)
    {
        if (!string.IsNullOrWhiteSpace(explicitPath))
        {
            var fullPath = Path.GetFullPath(explicitPath);
            return File.Exists(fullPath) ? fullPath : null;
        }

        var fromPath = ResolveFromWhere("VSIXInstaller.exe");

        if (fromPath is not null)
        {
            return fromPath;
        }

        foreach (var installationPath in ResolveVisualStudioInstallations())
        {
            var candidate = Path.Combine(installationPath, "Common7", "IDE", "VSIXInstaller.exe");

            if (File.Exists(candidate))
            {
                return candidate;
            }
        }

        return null;
    }

    private static IEnumerable<string> ResolveVisualStudioInstallations()
    {
        var programFilesX86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
        var vswherePath = Path.Combine(programFilesX86, "Microsoft Visual Studio", "Installer", "vswhere.exe");

        if (!File.Exists(vswherePath))
        {
            return [];
        }

        var process = Process.Start(new ProcessStartInfo
        {
            FileName = vswherePath,
            Arguments = "-products * -property installationPath -prerelease",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        });

        if (process is null)
        {
            return [];
        }

        var output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            return [];
        }

        return output
            .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }

    private static string? ResolveFromWhere(string executableName)
    {
        var process = Process.Start(new ProcessStartInfo
        {
            FileName = "where.exe",
            Arguments = executableName,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        });

        if (process is null)
        {
            return null;
        }

        var output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            return null;
        }

        return output
            .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .FirstOrDefault(File.Exists);
    }

    private static void PrintUsage()
    {
        Console.WriteLine("Usage:");
        Console.WriteLine("  nMolecules.Setup.VisualStudio.exe [--vsix <path>] [--installer <path>] [--quiet]");
    }

    private sealed record Options(string? VsixPath, string? InstallerPath, bool Quiet)
    {
        public static Options Parse(string[] args)
        {
            string? vsixPath = null;
            string? installerPath = null;
            var quiet = false;

            for (var index = 0; index < args.Length; index++)
            {
                switch (args[index])
                {
                    case "--vsix":
                        if (index + 1 >= args.Length)
                        {
                            return new Options(null, null, false) { ParseError = "--vsix requires a path argument." };
                        }

                        vsixPath = args[++index];
                        break;

                    case "--installer":
                        if (index + 1 >= args.Length)
                        {
                            return new Options(null, null, false) { ParseError = "--installer requires a path argument." };
                        }

                        installerPath = args[++index];
                        break;

                    case "--quiet":
                        quiet = true;
                        break;

                    default:
                        return new Options(null, null, false) { ParseError = $"Unknown argument '{args[index]}'." };
                }
            }

            return new Options(vsixPath, installerPath, quiet);
        }

        private string? ParseError { get; init; }

        public bool IsValid(out string error)
        {
            if (ParseError is not null)
            {
                error = ParseError;
                return false;
            }

            error = string.Empty;
            return true;
        }
    }
}
