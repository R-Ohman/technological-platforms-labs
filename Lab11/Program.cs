using System;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Application = System.Windows.Forms.Application;

class Program : Form
{
    private BackgroundWorker worker = new BackgroundWorker();
    private ProgressBar progressBar = new ProgressBar();
    private Label resultLabel = new Label();

    [STAThread]
    static void Main()
    {
        Application.Run(new Program());
    }

    public Program()
    {
        this.Text = "196634 | Laboratorium 11";

        Button buttonNewtonTask = new Button { Text = "Calculate Newton Symbol (Task)", Dock = DockStyle.Top };
        Button buttonNewtonDelegate = new Button { Text = "Calculate Newton Symbol (Delegate)", Dock = DockStyle.Top };
        Button buttonNewtonAsync = new Button { Text = "Calculate Newton Symbol (Async)", Dock = DockStyle.Top };
        Button buttonFibonacci = new Button { Text = "Calculate Fibonacci", Dock = DockStyle.Top };
        Button buttonCompress = new Button { Text = "Compress Files", Dock = DockStyle.Top };
        Button buttonDecompress = new Button { Text = "Decompress Files", Dock = DockStyle.Top };

        progressBar.Dock = DockStyle.Top;
        resultLabel.Dock = DockStyle.Bottom;

        this.Controls.Add(buttonDecompress);
        this.Controls.Add(buttonCompress);
        this.Controls.Add(buttonFibonacci);
        this.Controls.Add(buttonNewtonAsync);
        this.Controls.Add(buttonNewtonDelegate);
        this.Controls.Add(buttonNewtonTask);
        this.Controls.Add(progressBar);
        this.Controls.Add(resultLabel);

        buttonNewtonTask.Click += (s, e) => CalculateNewtonSymbolUsingTask(5, 2);
        buttonNewtonDelegate.Click += (s, e) => CalculateNewtonSymbolUsingDelegate(5, 2);
        buttonNewtonAsync.Click += (s, e) => CalculateNewtonSymbolUsingAsyncAwait(5, 2);
        buttonFibonacci.Click += (s, e) => CalculateFibonacci();
        buttonCompress.Click += (s, e) => CompressFiles();
        buttonDecompress.Click += (s, e) => DecompressFiles();

        worker.WorkerReportsProgress = true;
        worker.DoWork += Worker_DoWork;
        worker.ProgressChanged += Worker_ProgressChanged;
        worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
    }

    // Task and Task<T> Implementation
    private void CalculateNewtonSymbolUsingTask(int N, int K)
    {
        Task<long> numeratorTask = Task.Run(() => CalculateNumerator(N, K));
        Task<long> denominatorTask = Task.Run(() => CalculateDenominator(N, K));

        Task.WhenAll(numeratorTask, denominatorTask).ContinueWith(t =>
        {
            long result = numeratorTask.Result / denominatorTask.Result;
            MessageBox.Show($"({N} {K}) = {result}");
        });
    }

    // Delegate Implementation
    private delegate long FactorialDelegate(int n, int k);

    private void CalculateNewtonSymbolUsingDelegate(int N, int K)
    {
        FactorialDelegate numeratorDelegate = CalculateNumerator;
        FactorialDelegate denominatorDelegate = CalculateDenominator;

        IAsyncResult numeratorResult = numeratorDelegate.BeginInvoke(N, K, null, null);
        IAsyncResult denominatorResult = denominatorDelegate.BeginInvoke(N, K, null, null);

        long numerator = numeratorDelegate.EndInvoke(numeratorResult);
        long denominator = denominatorDelegate.EndInvoke(denominatorResult);

        long result = numerator / denominator;
        MessageBox.Show($"({N} {K}) = {result}");
    }

    private static long CalculateNumerator(int N, int K)
    {
        long result = 1;
        for (int i = 0; i < K; i++)
        {
            result *= (N - i);
        }
        return result;
    }

    private static long CalculateDenominator(int N, int K)
    {
        long result = 1;
        for (int i = 1; i <= K; i++)
        {
            result *= i;
        }
        return result;
    }

    // Async-Await Implementation
    private async void CalculateNewtonSymbolUsingAsyncAwait(int N, int K)
    {
        var numerator = CalculateNumeratorAsync(N, K);
        var denominator = CalculateDenominatorAsync(N, K);

        await Task.WhenAll(numerator, denominator);

        long result = numerator.Result / denominator.Result;
        MessageBox.Show($"({N} {K}) = {result}");
    }

    private async Task<long> CalculateNumeratorAsync(int N, int K)
    {
        long result = 1;
        for (int i = 0; i < K; i++)
        {
            result *= (N - i);
        }
        return result;
    }

    private async Task<long> CalculateDenominatorAsync(int N, int K)
    {
        long result = 1;
        for (int i = 1; i <= K; i++)
        {
            result *= i;
        }
        return result;
    }

    // Fibonacci Calculation using BackgroundWorker
    private void CalculateFibonacci()
    {
        // Calculate the 20th Fibonacci number
        worker.RunWorkerAsync(20);
    }

    private void Worker_DoWork(object sender, DoWorkEventArgs e)
    {
        int n = (int)e.Argument;
        long fib = CalculateFibonacci(n);
        e.Result = fib;
    }

    private long CalculateFibonacci(int n)
    {
        long a = 0, b = 1, c = 0;
        for (int i = 2; i <= n; i++)
        {
            Thread.Sleep(5);
            c = a + b;
            a = b;
            b = c;
            worker.ReportProgress((i * 100) / n);
        }
        return b;
    }

    private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
        progressBar.Value = e.ProgressPercentage;
    }

    private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
        resultLabel.Text = $"Fibonacci Result: {e.Result}";
    }

    // File Compression using GZipStream
    private void CompressFiles()
    {
        using (var dialog = new FolderBrowserDialog())
        {
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string selectedPath = dialog.SelectedPath;
                CompressFilesInDirectory(selectedPath);
            }
        }
    }

    private static void CompressFilesInDirectory(string directoryPath)
    {
        var files = Directory.GetFiles(directoryPath);

        Parallel.ForEach(files, (file) =>
        {
            string compressedFile = file + ".gz";
            using (var originalFileStream = new FileStream(file, FileMode.Open, FileAccess.Read))
            using (var compressedFileStream = new FileStream(compressedFile, FileMode.Create))
            using (var compressionStream = new GZipStream(compressedFileStream, CompressionMode.Compress))
            {
                originalFileStream.CopyTo(compressionStream);
            }
        });
    }

    private void DecompressFiles()
    {
        using (var dialog = new FolderBrowserDialog())
        {
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string selectedPath = dialog.SelectedPath;
                DecompressFilesInDirectory(selectedPath);
            }
        }
    }

    private static void DecompressFilesInDirectory(string directoryPath)
    {
        var files = Directory.GetFiles(directoryPath, "*.gz");

        Parallel.ForEach(files, (file) =>
        {
            string decompressedFile = file.Substring(0, file.Length - 3); // remove ".gz"
            using (var compressedFileStream = new FileStream(file, FileMode.Open, FileAccess.Read))
            using (var decompressedFileStream = new FileStream(decompressedFile, FileMode.Create))
            using (var decompressionStream = new GZipStream(compressedFileStream, CompressionMode.Decompress))
            {
                decompressionStream.CopyTo(decompressedFileStream);
            }
        });
    }
}
