// On a separate thread, run yarn dev and restart it if it crashes.

using System.Diagnostics;

// Work back up to the solution root.
var solutionDirectory = Directory.GetCurrentDirectory();
while (!File.Exists(Path.Combine(solutionDirectory, "ogame-v3.sln")))
{
    solutionDirectory = Path.GetDirectoryName(solutionDirectory);
}

// Check for any running "yarn dev" instances and shut them down
foreach (var proc in Process.GetProcessesByName("node"))
{
    if (string.Join(" ", proc.StartInfo.Arguments).Contains("yarn dev") &&
        proc.StartInfo.WorkingDirectory == Path.Combine(solutionDirectory, "OGame.MVC", "wwwroot"))
    {
        proc.Kill();
    }
}

Process? yarn = null;
new Thread(() =>
{
    yarn = new Process
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = "/c yarn dev",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
            WorkingDirectory = Path.Combine(solutionDirectory, "OGame.MVC", "wwwroot")
        }
    };

    yarn.Start();

    yarn.BeginOutputReadLine();
    yarn.BeginErrorReadLine();

    yarn.OutputDataReceived += (sender, args) =>
    {
        if (args.Data != null)
        {
            Console.WriteLine(args.Data);
        }
    };

    yarn.ErrorDataReceived += (sender, args) =>
    {
        if (args.Data != null)
        {
            Console.WriteLine(args.Data);
        }
    };

    yarn.WaitForExit();
});

Console.CancelKeyPress += new ConsoleCancelEventHandler((sender, _) =>
{
    yarn?.Kill();
});
