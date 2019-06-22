using System.Diagnostics;
using System.IO;

public class CommandRunner {
	public string ExecutablePath { get; }
	public string WorkingDirectory { get; }

	public CommandRunner (string executablePath, string workingDirectory = null) {
		ExecutablePath = executablePath;
		WorkingDirectory = workingDirectory ?? Path.GetDirectoryName (executablePath);
	}

	public string Run (string arguments) {
		var info = new ProcessStartInfo (ExecutablePath, arguments) {
			CreateNoWindow = true,
				RedirectStandardOutput = true,
				UseShellExecute = false,
				WorkingDirectory = WorkingDirectory,
		};

		var process = new Process {
			StartInfo = info,
		};
		process.Start ();
		return process.StandardOutput.ReadToEnd ();
	}
}