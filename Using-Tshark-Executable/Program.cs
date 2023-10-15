using System.Diagnostics;
using System.Text;

class Program
{
    static async Task Main(string[] args)
    {
        var watch = new Stopwatch();

        watch.Start();
        
        List<Task> tasks = new();
        int maxConcurrentTasks = Environment.ProcessorCount*2;
        var limit = new SemaphoreSlim(initialCount: maxConcurrentTasks);
        for (int i = 1; i <= 10; i++)
        {
            await limit.WaitAsync();
            try
            {
                tasks.Add(ProcessPcapFileAsync(args[0]));
            }
            finally
            {
                limit.Release();
            }    
        }

        await Task.WhenAll(tasks);
        
        watch.Stop();

        Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");
        Console.WriteLine("task finished");
    }

    static async Task ProcessPcapFileAsync(string filename)
    {
        StringBuilder output = new();
        string command = "bash";
        string arguments = "-c \"" + $"tshark -T json -r {filename}" + "\"";

        ProcessStartInfo startInfo = new()
        {
            FileName = command,
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using Process process = new();
        process.StartInfo = startInfo;
        process.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                output.Append(e.Data + "\n");
            }
        });
        process.Start();
        process.BeginOutputReadLine();
        await process.WaitForExitAsync();
        Console.WriteLine(output);
    }
}
