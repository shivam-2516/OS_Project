using System;
using System.Diagnostics;  // Required for starting child process
using System.IO;           // Required for handling file and stream operations
using System.IO.Pipes;     // Required for Anonymous Pipes

/// <summary>
/// Parent process that creates an Anonymous Pipe and starts the child process.
/// The parent process writes a message to the pipe, and the child process reads it.
/// </summary>
class ParentProcess
{
    static void Main()
    {
        Console.WriteLine("Starting Parent Process...");

        string childProcessPath = @"C:\User\sbavariy\source\repos\ParentProcess\ChildProcess\bin\Debug\net8.0\ChildProcess.exe";

        if (!File.Exists(childProcessPath))
        {
            Console.WriteLine("Error: Child process file not found at " + childProcessPath);
            return;
        }


        // Create an anonymous pipe for interprocess communication
        using (AnonymousPipeServerStream pipeServer = new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.Inheritable))
        {
            Process childProcess = new Process(); // Create a new process instance for the child process
            childProcess.StartInfo.FileName = childProcessPath;  // Specify the command to execute the child process
            childProcess.StartInfo.Arguments = "";  // Run the child project
            childProcess.StartInfo.UseShellExecute = false;  // Required for redirection of standard input/output
            childProcess.StartInfo.RedirectStandardInput = true;  // Enable input redirection
            childProcess.StartInfo.EnvironmentVariables["PIPE_HANDLE"] = pipeServer.GetClientHandleAsString(); // Pass pipe handle
            childProcess.Start(); // Start the child process

            pipeServer.DisposeLocalCopyOfClientHandle(); // Dispose of the local copy of the client handle to avoid conflicts

            // Write message to the pipe using a StreamWriter
            using (StreamWriter writer = new StreamWriter(pipeServer))
            {
                writer.AutoFlush = true;  // Ensure the data is flushed immediately
                writer.WriteLine("Hello from Parent Process!");  // Send message to child
            }

            childProcess.WaitForExit();  // Wait for the child process to complete before exiting
        }
    }
}
