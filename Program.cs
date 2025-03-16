using System.Text;
using System.Text.RegularExpressions;

namespace GeneralCodeExtracter
{
    class Program
    {
        // Default file extensions to look for
        private static readonly string[] DefaultFileExtensions = new[]
        {
            // C-family languages
            ".c", ".cpp", ".h", ".hpp", ".cs",
            // Web technologies
            ".html", ".css", ".js", ".ts", ".jsx", ".tsx",
            // Python
            ".py", ".pyw", ".ipynb",
            // Java
            ".java", ".kt", ".groovy",
            // Other languages
            ".go", ".rb", ".php", ".swift", ".rs", ".lua",
            // Markup and config
            ".xml", ".xaml", ".json", ".yaml", ".yml", ".toml",
            // Shell scripts
            ".sh", ".bash", ".ps1", ".bat", ".cmd",
            // Documentation
            ".md", ".rst", ".txt"
        };

        static void Main(string[] args)
        {
            Console.WriteLine("General Code Extractor");
            Console.WriteLine("---------------------");

            // Get source directory
            string sourceDir = GetSourceDirectory(args);

            // Get output file path
            string outputPath = GetOutputFilePath(args, sourceDir);

            // Get word limit (if provided)
            int wordLimit = GetWordLimit(args);

            // Get file extensions to include
            string[] fileExtensions = GetFileExtensions(args);

            // Get directories to exclude
            string[] excludeDirs = GetExcludedDirectories(args);

            // Show configuration
            Console.WriteLine($"Scanning directory: {sourceDir}");
            Console.WriteLine($"Output will be saved to: {outputPath}");
            if (wordLimit > 0)
            {
                Console.WriteLine($"Word limit per file: {wordLimit}");
            }
            Console.WriteLine($"File types: {string.Join(", ", fileExtensions)}");
            Console.WriteLine($"Excluded directories: {string.Join(", ", excludeDirs)}");
            Console.WriteLine();

            try
            {
                // Process the directory and write all code to the output file(s)
                int fileCount = ProcessDirectory(sourceDir, outputPath, wordLimit, fileExtensions, excludeDirs);
                Console.WriteLine($"\nComplete! Successfully processed {fileCount} files.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        private static string GetSourceDirectory(string[] args)
        {
            string dir;

            // Check if directory is provided as command-line argument
            if (args.Length >= 1 && Directory.Exists(args[0]))
            {
                dir = args[0];
            }
            else
            {
                // Otherwise prompt for directory
                Console.Write("Enter the path to your source code directory: ");
                dir = Console.ReadLine().Trim('"');

                if (!Directory.Exists(dir))
                {
                    throw new DirectoryNotFoundException($"Directory not found: {dir}");
                }
            }

            return dir;
        }

        private static string GetOutputFilePath(string[] args, string sourceDir)
        {
            string outputPath;

            // Check if output file is provided as command-line argument
            if (args.Length >= 2)
            {
                outputPath = args[1];
            }
            else
            {
                // Otherwise create a default filename based on directory name and timestamp
                string dirName = new DirectoryInfo(sourceDir).Name;
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                outputPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    $"{dirName}_code_export_{timestamp}.txt"
                );
            }

            // Ensure directory exists
            string outputDir = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(outputDir) && !Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            return outputPath;
        }

        private static int GetWordLimit(string[] args)
        {
            int wordLimit = 0; // Default: no limit

            // Check if word limit is provided as command-line argument
            if (args.Length >= 3 && int.TryParse(args[2], out int limit))
            {
                wordLimit = limit;
            }
            else
            {
                // Otherwise prompt for word limit
                Console.Write("Enter word limit per file (leave blank for no limit): ");
                string input = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(input) && int.TryParse(input, out int userLimit))
                {
                    wordLimit = userLimit;
                }
            }

            return wordLimit;
        }

        private static string[] GetFileExtensions(string[] args)
        {
            string[] extensions;

            // Check if extensions are provided as command-line argument
            if (args.Length >= 4 && !string.IsNullOrWhiteSpace(args[3]))
            {
                extensions = args[3].Split(',', ';').Select(ext => ext.Trim()).ToArray();
            }
            else
            {
                // Otherwise prompt for extensions
                Console.Write("Enter file extensions to include (comma-separated, leave blank for defaults): ");
                string input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                {
                    extensions = DefaultFileExtensions;
                }
                else
                {
                    extensions = input.Split(',', ';').Select(ext => ext.Trim()).ToArray();
                }
            }

            // Ensure extensions start with a dot
            for (int i = 0; i < extensions.Length; i++)
            {
                if (!extensions[i].StartsWith("."))
                {
                    extensions[i] = "." + extensions[i];
                }
            }

            return extensions;
        }

        private static string[] GetExcludedDirectories(string[] args)
        {
            string[] excludedDirs = { "bin", "obj", "node_modules", "packages", ".vs", ".git", "__pycache__", "venv", "env", "dist", "build" };

            // Check if excluded directories are provided as command-line argument
            if (args.Length >= 5 && !string.IsNullOrWhiteSpace(args[4]))
            {
                excludedDirs = args[4].Split(',', ';').Select(dir => dir.Trim()).ToArray();
            }
            else
            {
                // Otherwise prompt for excluded directories
                Console.Write("Enter directories to exclude (comma-separated, leave blank for defaults): ");
                string input = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(input))
                {
                    excludedDirs = input.Split(',', ';').Select(dir => dir.Trim()).ToArray();
                }
            }

            return excludedDirs;
        }

        private static int CountWords(string text)
        {
            // Count words using regex to match word boundaries
            // This handles code better than simple splitting by spaces
            return Regex.Matches(text, @"\b\w+\b").Count;
        }

        private static int ProcessDirectory(string sourceDir, string outputPath, int wordLimit, string[] fileExtensions, string[] excludedDirs)
        {
            int totalFilesProcessed = 0;
            int partNumber = 1;
            int currentPartWordCount = 0;
            string currentOutputPath = outputPath;
            StreamWriter writer = null;

            try
            {
                // Get all relevant files recursively
                var files = GetRelevantFiles(sourceDir, fileExtensions, excludedDirs)
                    .OrderBy(f => Path.GetExtension(f))  // Group by extension
                    .ThenBy(f => f);                     // Then alphabetically by path

                // Create the first output file
                currentOutputPath = GetPartFilePath(outputPath, partNumber);
                writer = new StreamWriter(currentOutputPath, false, Encoding.UTF8);

                // Write header
                string header = GetFileHeader(sourceDir, partNumber, fileExtensions);
                writer.WriteLine(header);
                currentPartWordCount += CountWords(header);

                // Process each file
                foreach (var file in files)
                {
                    totalFilesProcessed++;

                    // Show progress
                    Console.Write($"\rProcessing file {totalFilesProcessed}: {Path.GetFileName(file)}");

                    // Calculate relative path for cleaner output
                    string relativePath = file.Substring(sourceDir.Length).TrimStart('\\', '/');

                    // Read file content
                    string content;
                    try
                    {
                        content = File.ReadAllText(file);
                    }
                    catch (Exception ex)
                    {
                        content = $"// ERROR reading file: {ex.Message}";
                    }

                    // Create file separator with language hint
                    string fileExtension = Path.GetExtension(file).ToLower();
                    string langHint = GetLanguageHint(fileExtension);
                    string fileHeader = $"{new string('-', 80)}\n// FILE: {relativePath} {langHint}\n{new string('-', 80)}\n\n";
                    string fileContent = fileHeader + content + "\n\n\n";

                    // Calculate word count
                    int fileWordCount = CountWords(fileContent);

                    // Check if we need to create a new part due to word limit
                    if (wordLimit > 0 && writer != null && (currentPartWordCount + fileWordCount > wordLimit))
                    {
                        // Close current file
                        string footer = $"\n// End of part {partNumber}";
                        writer.WriteLine(footer);
                        writer.Close();
                        writer.Dispose();

                        // Create new file
                        partNumber++;
                        currentPartWordCount = 0;
                        currentOutputPath = GetPartFilePath(outputPath, partNumber);

                        Console.WriteLine($"\nCreating part {partNumber}: {Path.GetFileName(currentOutputPath)}");

                        writer = new StreamWriter(currentOutputPath, false, Encoding.UTF8);
                        string newHeader = GetFileHeader(sourceDir, partNumber, fileExtensions);
                        writer.WriteLine(newHeader);
                        currentPartWordCount = CountWords(newHeader);
                    }

                    // Write file content
                    writer.Write(fileContent);

                    // Update current part word count
                    currentPartWordCount += fileWordCount;
                }

                // Close the final file
                if (writer != null)
                {
                    string footer = $"\n// End of part {partNumber}";
                    if (partNumber > 1)
                    {
                        footer += $"\n// Total output files: {partNumber}";
                    }
                    writer.WriteLine(footer);
                }
            }
            finally
            {
                // Ensure writer is closed and disposed
                if (writer != null)
                {
                    writer.Close();
                    writer.Dispose();
                }
            }

            if (partNumber > 1)
            {
                Console.WriteLine($"\nCreated {partNumber} parts due to word limit restrictions.");
            }

            return totalFilesProcessed;
        }

        private static string GetLanguageHint(string fileExtension)
        {
            // Return a language hint based on file extension
            switch (fileExtension)
            {
                case ".cs": return "[C#]";
                case ".c": return "[C]";
                case ".cpp": case ".hpp": case ".h": return "[C++]";
                case ".js": return "[JavaScript]";
                case ".ts": return "[TypeScript]";
                case ".jsx": return "[JSX]";
                case ".tsx": return "[TSX]";
                case ".py": case ".pyw": return "[Python]";
                case ".java": return "[Java]";
                case ".kt": return "[Kotlin]";
                case ".html": return "[HTML]";
                case ".css": return "[CSS]";
                case ".xml": case ".xaml": return "[XML]";
                case ".json": return "[JSON]";
                case ".md": return "[Markdown]";
                case ".yml": case ".yaml": return "[YAML]";
                case ".rb": return "[Ruby]";
                case ".php": return "[PHP]";
                case ".go": return "[Go]";
                case ".rs": return "[Rust]";
                case ".swift": return "[Swift]";
                case ".sh": case ".bash": return "[Shell]";
                case ".ps1": return "[PowerShell]";
                case ".bat": case ".cmd": return "[Batch]";
                case ".sql": return "[SQL]";
                case ".lua": return "[Lua]";
                case ".r": return "[R]";
                case ".pl": return "[Perl]";
                default: return "";
            }
        }

        private static string GetFileHeader(string sourceDir, int partNumber, string[] fileExtensions)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"// Code Export from {sourceDir}");
            sb.AppendLine($"// Generated on {DateTime.Now}");
            sb.AppendLine($"// File types included: {string.Join(", ", fileExtensions)}");

            if (partNumber > 1)
            {
                sb.AppendLine($"// Part {partNumber}");
            }

            sb.AppendLine();
            return sb.ToString();
        }

        private static string GetPartFilePath(string originalPath, int partNumber)
        {
            if (partNumber == 1)
            {
                return originalPath;
            }

            string directory = Path.GetDirectoryName(originalPath);
            string fileName = Path.GetFileNameWithoutExtension(originalPath);
            string extension = Path.GetExtension(originalPath);

            return Path.Combine(directory, $"{fileName}_part{partNumber}{extension}");
        }

        private static IEnumerable<string> GetRelevantFiles(string directory, string[] fileExtensions, string[] excludedDirs)
        {
            List<string> results = new List<string>();

            try
            {
                // Get all directories except excluded ones
                var directories = Directory.GetDirectories(directory)
                    .Where(d => !excludedDirs.Contains(Path.GetFileName(d), StringComparer.OrdinalIgnoreCase))
                    .ToList();

                // Get all target files in current directory
                results.AddRange(Directory.GetFiles(directory)
                    .Where(f => fileExtensions.Contains(Path.GetExtension(f).ToLower(), StringComparer.OrdinalIgnoreCase)));

                // Recursively get files from subdirectories
                foreach (var dir in directories)
                {
                    results.AddRange(GetRelevantFiles(dir, fileExtensions, excludedDirs));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nWarning: Could not access {directory}: {ex.Message}");
            }

            return results;
        }
    }
}