using System.Diagnostics;

namespace nMolecules.Setup.VSCode;

internal static class Program
{
    private const int Success = 0;
    private const int InvalidArguments = 1;
    private const int MissingFile = 2;
    private const int MissingCommand = 3;

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
            Console.Error.WriteLine("Could not find the VS Code extension package (.vsix).");
            Console.Error.WriteLine("Use --vsix <path> or place nMolecules.VSCode.vsix next to this setup executable.");
            return MissingFile;
        }

        var codeCommand = ResolveCodeCommand(options.CodePath);

        if (codeCommand is null)
        {
            Console.Error.WriteLine("Could not locate the VS Code CLI.");
            Console.Error.WriteLine("Install Visual Studio Code or pass --code <path-to-code.cmd/code-insiders.cmd>.");
            return MissingCommand;
        }

        Console.WriteLine($"Using VS Code CLI: {codeCommand}");
        Console.WriteLine($"Installing package: {vsixPath}");

        var extensionArguments = $"--install-extension \"{vsixPath}\"";

        if (options.Force)
        {
            extensionArguments += " --force";
        }

        ProcessStartInfo startInfo;

        if (IsBatchScript(codeCommand))
        {
            startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c \"\"{codeCommand}\" {extensionArguments}\"",
                UseShellExecute = false
            };
        }
        else
        {
            startInfo = new ProcessStartInfo
            {
                FileName = codeCommand,
                Arguments = extensionArguments,
                UseShellExecute = false
            };
        }

        var process = Process.Start(startInfo);

        if (process is null)
        {
            Console.Error.WriteLine("Failed to start the VS Code CLI.");
            return MissingCommand;
        }

        process.WaitForExit();
        return process.ExitCode;
    }

    private static bool IsBatchScript(string path)
        => path.EndsWith(".cmd", StringComparison.OrdinalIgnoreCase)
            || path.EndsWith(".bat", StringComparison.OrdinalIgnoreCase);

    private static string? ResolveVsixPath(string? explicitPath)
    {
        if (!string.IsNullOrWhiteSpace(explicitPath))
        {
            var fullPath = Path.GetFullPath(explicitPath);
            return File.Exists(fullPath) ? fullPath : null;
        }

        var localCandidates = new[]
        {
            Path.Combine(AppContext.BaseDirectory, "nMolecules.VSCode.vsix"),
            Path.Combine(AppContext.BaseDirectory, "nmolecules-vscode.vsix")
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

    private static string? ResolveCodeCommand(string? explicitPath)
    {
        if (!string.IsNullOrWhiteSpace(explicitPath))
        {
            var fullPath = Path.GetFullPath(explicitPath);
            return File.Exists(fullPath) ? fullPath : null;
        }

        var fromEnvironment = Environment.GetEnvironmentVariable("VSCODE_CLI");

        if (!string.IsNullOrWhiteSpace(fromEnvironment) && File.Exists(fromEnvironment))
        {
            return fromEnvironment;
        }

        var localCandidates = new[]
        {
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Programs",
                "Microsoft VS Code",
                "bin",
                "code.cmd"),
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Programs",
                "Microsoft VS Code Insiders",
                "bin",
                "code-insiders.cmd")
        };

        foreach (var candidate in localCandidates)
        {
            if (File.Exists(candidate))
            {
                return candidate;
            }
        }

        return ResolveFromWhere("code")
            ?? ResolveFromWhere("code-insiders");
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
        Console.WriteLine("  nMolecules.Setup.VSCode.exe [--vsix <path>] [--code <path>] [--no-force]");
    }

    private sealed record Options(string? VsixPath, string? CodePath, bool Force)
    {
        public static Options Parse(string[] args)
        {
            string? vsixPath = null;
            string? codePath = null;
            var force = true;

            for (var index = 0; index < args.Length; index++)
            {
                switch (args[index])
                {
                    case "--vsix":
                        if (index + 1 >= args.Length)
                        {
                            return new Options(null, null, true) { ParseError = "--vsix requires a path argument." };
                        }

                        vsixPath = args[++index];
                        break;

                    case "--code":
                        if (index + 1 >= args.Length)
                        {
                            return new Options(null, null, true) { ParseError = "--code requires a path argument." };
                        }

                        codePath = args[++index];
                        break;

                    case "--no-force":
                        force = false;
                        break;

                    default:
                        return new Options(null, null, true) { ParseError = $"Unknown argument '{args[index]}'." };
                }
            }

            return new Options(vsixPath, codePath, force);
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
