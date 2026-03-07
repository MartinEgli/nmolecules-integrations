using System.Diagnostics;

namespace nMolecules.Setup.VSCode;

internal static class Program
{
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

        Console.WriteLine($"Using VS Code CLI: {codeCommand.DisplayName}");
        Console.WriteLine($"Installing package: {vsixPath}");

        var extensionArguments = $"--install-extension \"{vsixPath}\"";

        if (options.Force)
        {
            extensionArguments += " --force";
        }

        var process = Process.Start(codeCommand.CreateStartInfo(extensionArguments));

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

        foreach (var candidate in EnumerateVsixCandidates())
        {
            if (File.Exists(candidate))
            {
                return Path.GetFullPath(candidate);
            }
        }

        return null;
    }

    private static IEnumerable<string> EnumerateVsixCandidates()
    {
        var fileNames = new[]
        {
            "nMolecules.VSCode.vsix",
            "nmolecules-vscode.vsix"
        };

        foreach (var directory in EnumerateSearchDirectories())
        {
            foreach (var fileName in fileNames)
            {
                yield return Path.Combine(directory, fileName);
            }
        }
    }

    private static IEnumerable<string> EnumerateSearchDirectories()
    {
        var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        static IEnumerable<string?> EnumerateDirectoryChain(string startDirectory)
        {
            var current = new DirectoryInfo(startDirectory);

            while (current is not null)
            {
                yield return current.FullName;
                current = current.Parent;
            }
        }

        static IEnumerable<string> EnumerateArtifactDirectories(string rootDirectory)
        {
            yield return Path.Combine(rootDirectory, "artifacts", "installers", "vscode");
            yield return Path.Combine(rootDirectory, "artifacts", "installers", "vscode", "setup");
        }

        var seedDirectories = new[]
        {
            AppContext.BaseDirectory,
            Environment.CurrentDirectory
        };

        foreach (var seedDirectory in seedDirectories)
        {
            foreach (var directory in EnumerateDirectoryChain(seedDirectory))
            {
                if (directory is null)
                {
                    continue;
                }

                var normalizedDirectory = Path.GetFullPath(directory);

                if (visited.Add(normalizedDirectory))
                {
                    yield return normalizedDirectory;
                }

                foreach (var artifactDirectory in EnumerateArtifactDirectories(normalizedDirectory))
                {
                    var normalizedArtifactDirectory = Path.GetFullPath(artifactDirectory);

                    if (visited.Add(normalizedArtifactDirectory))
                    {
                        yield return normalizedArtifactDirectory;
                    }
                }
            }
        }
    }

    private static CodeCommand? ResolveCodeCommand(string? explicitPath)
    {
        if (!string.IsNullOrWhiteSpace(explicitPath))
        {
            var fullPath = Path.GetFullPath(explicitPath);

            return File.Exists(fullPath)
                ? ResolveUsableCodeCommand([fullPath])
                : null;
        }

        var fromEnvironment = Environment.GetEnvironmentVariable("VSCODE_CLI");

        if (!string.IsNullOrWhiteSpace(fromEnvironment) && File.Exists(fromEnvironment))
        {
            var commandFromEnvironment = ResolveUsableCodeCommand([fromEnvironment]);

            if (commandFromEnvironment is not null)
            {
                return commandFromEnvironment;
            }
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

        var directExecutableCandidates = new[]
        {
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Programs",
                "Microsoft VS Code",
                "Code.exe"),
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Programs",
                "Microsoft VS Code",
                "new_Code.exe"),
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Programs",
                "Microsoft VS Code",
                "old_Code.exe"),
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Programs",
                "Microsoft VS Code Insiders",
                "Code - Insiders.exe")
        };

        var allCandidates = localCandidates
            .Concat(directExecutableCandidates)
            .Concat(ResolveFromWhere("code"))
            .Concat(ResolveFromWhere("code-insiders"));

        return ResolveUsableCodeCommand(allCandidates);
    }

    private static CodeCommand? ResolveUsableCodeCommand(IEnumerable<string> candidatePaths)
    {
        var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var candidatePath in candidatePaths.Where(static path => !string.IsNullOrWhiteSpace(path)))
        {
            var normalizedCandidatePath = Path.GetFullPath(candidatePath);

            if (!File.Exists(normalizedCandidatePath) || !visited.Add(normalizedCandidatePath))
            {
                continue;
            }

            foreach (var command in ExpandCodeCommands(normalizedCandidatePath))
            {
                if (IsUsableCodeCommand(command))
                {
                    return command;
                }
            }
        }

        return null;
    }

    private static IEnumerable<CodeCommand> ExpandCodeCommands(string candidatePath)
    {
        if (IsBatchScript(candidatePath))
        {
            yield return new CodeCommand(candidatePath, null, false);
        }

        if (!candidatePath.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
        {
            yield break;
        }

        var fileName = Path.GetFileName(candidatePath);
        var isElectronShell =
            fileName.Equals("Code.exe", StringComparison.OrdinalIgnoreCase)
            || fileName.Equals("new_Code.exe", StringComparison.OrdinalIgnoreCase)
            || fileName.Equals("old_Code.exe", StringComparison.OrdinalIgnoreCase)
            || fileName.Equals("Code - Insiders.exe", StringComparison.OrdinalIgnoreCase);

        if (!isElectronShell)
        {
            yield return new CodeCommand(candidatePath, null, false);
            yield break;
        }

        var cliScript = ResolveCliScript(candidatePath);

        if (cliScript is not null)
        {
            yield return new CodeCommand(candidatePath, $"\"{cliScript}\"", true);
        }
    }

    private static string? ResolveCliScript(string executablePath)
    {
        var installationRoot = Path.GetDirectoryName(executablePath);

        if (string.IsNullOrWhiteSpace(installationRoot) || !Directory.Exists(installationRoot))
        {
            return null;
        }

        return Directory
            .EnumerateFiles(installationRoot, "cli.js", SearchOption.AllDirectories)
            .Where(path => path.EndsWith(Path.Combine("resources", "app", "out", "cli.js"), StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(path => File.GetLastWriteTimeUtc(path))
            .FirstOrDefault();
    }

    private static bool IsUsableCodeCommand(CodeCommand command)
    {
        try
        {
            var startInfo = command.CreateStartInfo("--version");
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;

            using var process = Process.Start(startInfo);

            if (process is null)
            {
                return false;
            }

            if (!process.WaitForExit(10000))
            {
                try
                {
                    process.Kill(entireProcessTree: true);
                }
                catch
                {
                    // Ignore kill failures during probe.
                }

                return false;
            }

            return process.ExitCode == 0;
        }
        catch
        {
            return false;
        }
    }

    private static IEnumerable<string> ResolveFromWhere(string executableName)
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
            return [];
        }

        var output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            return [];
        }

        return output
            .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(File.Exists);
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

    private sealed record CodeCommand(string ExecutablePath, string? PrefixArguments, bool RequiresElectronRunAsNode)
    {
        public string DisplayName => ExecutablePath;

        public ProcessStartInfo CreateStartInfo(string arguments)
        {
            var fullArguments = string.IsNullOrWhiteSpace(PrefixArguments)
                ? arguments
                : $"{PrefixArguments} {arguments}";

            ProcessStartInfo startInfo;

            if (IsBatchScript(ExecutablePath))
            {
                startInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c \"\"{ExecutablePath}\" {fullArguments}\"",
                    UseShellExecute = false
                };
            }
            else
            {
                startInfo = new ProcessStartInfo
                {
                    FileName = ExecutablePath,
                    Arguments = fullArguments,
                    UseShellExecute = false
                };
            }

            if (RequiresElectronRunAsNode)
            {
                startInfo.Environment["ELECTRON_RUN_AS_NODE"] = "1";
            }

            return startInfo;
        }
    }
}
