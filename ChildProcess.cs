using System;
using System.IO;          // Required for handling file and stream operations
using System.IO.Pipes;    // Required for Anonymous Pipes

/// <summary>
/// Child process that reads data from the parent process using an Anonymous Pipe.
/// The child receives a message from the parent and prints it to the console.
/// </summary>
class ChildProcess
{
    public static void Main()
    {
        Console.WriteLine("Starting Child Process...");

        // Retrieve the pipe handle from the environment variable
        string pipeHandle = Environment.GetEnvironmentVariable("PIPE_HANDLE");

        if (string.IsNullOrEmpty(pipeHandle)) // Check if the pipe handle was provided
        {
            Console.WriteLine("No pipe handle found. Exiting...");
            return;
        }

        // Connect to the parent process using the provided pipe handle
        using (AnonymousPipeClientStream pipeClient = new AnonymousPipeClientStream(PipeDirection.In, pipeHandle))
        using (StreamReader reader = new StreamReader(pipeClient))  // Create a StreamReader to read from the pipe
        {
            string message = reader.ReadLine();  // Read the message from the parent process
            Console.WriteLine($"Child Process received: {message}");  // Display the received message
        }
    }
}
