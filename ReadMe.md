﻿# General Code Extractor

## Overview

The General Code Extractor is a command-line tool designed to traverse through directories and repositories, extracting source code from various file types and consolidating it into text files. This tool is particularly useful for preparing codebases to be fed into Large Language Models (LLMs) and coding AI models. By extracting code into plain text, it enables enhanced code analysis, summarization, and discussion by AI, as well as simplifying code sharing and archival.

## Features

*   **Versatile File Extension Support:** Extracts code from a wide range of programming languages, markup languages, and configuration files. Supports default file extensions and allows for custom extension specification.
*   **Directory Exclusion:**  Option to exclude specific directories (like `bin`, `obj`, `node_modules`, `.git`, etc.) to focus on relevant source code and avoid build artifacts or dependency folders.
*   **Word Limit per File (Part Splitting):**  Allows setting a word limit per output file. If the total extracted code exceeds this limit, the output is automatically split into multiple parts, making it suitable for LLMs with token limits.
*   **Organized Output:**  Generates well-formatted text files with clear separators between files, file headers indicating the relative path and language hint, and optional footers with part numbers and total part count.
*   **Command-Line Interface:**  Easy to use from the command line with arguments for source directory, output path, word limit, file extensions, and excluded directories.
*   **UTF-8 Encoding:** Output files are encoded in UTF-8 to ensure proper handling of various character sets in code.
*   **Ordered Output:** Files are processed and written to the output in an order grouped by file extension and then alphabetically by path, improving readability and organization.

## How to Use

### Prerequisites

*   .NET Runtime installed (required to run the compiled C# application).

### Running the Extractor

1.  **Build the Project:** Compile the C# code (`Extracter.txt`, rename it to `Extracter.cs` and build using a C# compiler like `csc` or Visual Studio/MSBuild). This will generate an executable file (e.g., `GeneralCodeExtracter.exe`).

2.  **Open Command Line/Terminal:** Navigate to the directory where you have the compiled executable.

3.  **Run the Extractor with Arguments:** Execute the tool from the command line, providing the necessary arguments.

    **Basic Usage:**

    ```bash
    GeneralCodeExtracter.exe <source_directory>
    ```

    *   `<source_directory>`:  **(Required)** The absolute or relative path to the root directory of your source code project or repository.

    **Advanced Usage with Options:**

    ```bash
    GeneralCodeExtracter.exe <source_directory> <output_file_path> <word_limit> <file_extensions> <excluded_directories>
    ```

    *   `<source_directory>`:  **(Required)** The path to your source code directory.
    *   `<output_file_path>`: **(Optional)** The path and filename for the output text file. If not provided, a default filename based on the source directory name and timestamp will be used, saved on your desktop (e.g., `ProjectName_code_export_yyyyMMdd_HHmmss.txt`).
    *   `<word_limit>`: **(Optional)**  An integer representing the maximum number of words per output file. Set to `0` or leave blank for no word limit. If the extracted code exceeds this limit, the output will be split into multiple files (e.g., `output_file_part1.txt`, `output_file_part2.txt`, etc.).
    *   `<file_extensions>`: **(Optional)** A comma-separated or semicolon-separated list of file extensions to include (e.g., `.cs,.py,.js`). If not provided, default file extensions (C-family, Web, Python, Java, etc.) will be used.
    *   `<excluded_directories>`: **(Optional)** A comma-separated or semicolon-separated list of directory names to exclude from the scan (e.g., `bin,obj,node_modules`). If not provided, default excluded directories (common build and dependency folders) will be used.

    **Examples:**

    *   **Extract code from `MyProject` directory using default settings:**

        ```bash
        GeneralCodeExtracter.exe "C:\path\to\MyProject"
        ```

    *   **Extract code with a custom output file path:**

        ```bash
        GeneralCodeExtracter.exe "C:\path\to\MyProject" "D:\CodeExports\MyProjectCode.txt"
        ```

    *   **Extract code with a word limit of 5000 words per file:**

        ```bash
        GeneralCodeExtracter.exe "C:\path\to\MyProject" "" 5000
        ```

    *   **Extract only Python and JavaScript files:**

        ```bash
        GeneralCodeExtracter.exe "C:\path\to\MyProject" "" "" ".py,.js"
        ```

    *   **Exclude `vendor` and `temp` directories:**

        ```bash
        GeneralCodeExtracter.exe "C:\path\to\MyProject" "" "" "" "vendor,temp"
        ```

    *   **Combine options: Custom output, word limit, specific extensions, and excluded directories:**

        ```bash
        GeneralCodeExtracter.exe "C:\path\to\MyProject" "CodeExport.txt" 10000 ".java;.kt;.groovy" "build,temp,resources"
        ```

4.  **Output Files:** After running the command, the extracted code will be saved in the specified output file(s). If a word limit is used and exceeded, you'll find multiple part files in the same directory as the main output file.

### Interactive Mode (No Command Line Arguments)

If you run `GeneralCodeExtracter.exe` without any command-line arguments, the tool will prompt you interactively for:

1.  Source code directory path.
2.  Output file path (optional).
3.  Word limit per file (optional).
4.  File extensions to include (optional).
5.  Directories to exclude (optional).

Follow the prompts in the console to configure and run the code extraction.

## Customization

*   **File Extensions:**  Customize the file types to be extracted using the `<file_extensions>` command-line argument or by responding to the interactive prompt. The default set is comprehensive, but you can narrow it down to specific languages or file types you're interested in.
*   **Excluded Directories:** Adjust the directories to be excluded using the `<excluded_directories>` argument or the interactive prompt. The default list covers common project directories that are typically not needed for code analysis.
*   **Word Limit:**  Control the size of the output files by setting the `<word_limit>` argument or during interactive setup. This is crucial for working with LLMs that have token limits or for managing very large codebases.

## Use Cases

*   **Preparing Code for LLMs:** Extracting code into text files is ideal for feeding codebases into LLMs for tasks like code summarization, code generation, bug detection, and code understanding. The word limit feature helps in managing input size for models with token constraints.
*   **Code Analysis and Documentation Generation:**  The extracted code can be used with other text analysis tools for generating code documentation, identifying code patterns, or performing static analysis.
*   **Simplified Code Sharing and Archival:**  Consolidating code into text files can simplify sharing code snippets or archiving codebases in a more portable and text-searchable format.
*   **Code Review and Discussion:**  Plain text code exports can be helpful for code review processes or for discussing code offline, especially when dealing with large projects or complex repositories.

## Limitations

*   **Basic Text Extraction:** The tool primarily focuses on extracting the text content of code files. It does not perform any advanced parsing or semantic analysis of the code.
*   **No Language-Specific Formatting:** While language hints are provided in the output, the tool does not attempt to reformat or syntax-highlight the code in the output text files.

## Contributing

Contributions to enhance the tool's features, improve performance, or add support for more file types are welcome! Please feel free to fork the repository, make your changes, and submit a pull request.

## License

This project is open-source and available under the [Specify License Here, e.g., MIT License]. See the `LICENSE` file for more details.

## Contact

For questions, issues, or suggestions, please [Your Contact Method, e.g., open an issue on GitHub, email to your address].