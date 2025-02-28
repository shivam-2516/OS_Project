# OS_Project
# 🖥️ Operating Systems Project: Multi-Threading & Interprocess Communication (IPC)

## 📌 Project Overview
This project demonstrates **multi-threading and interprocess communication (IPC) using pipes** in C#. The implementation includes:
- **Multi-threading**: Simulating concurrent execution with synchronization mechanisms (mutexes, locks).
- **Deadlock Handling**: Creating and resolving deadlocks using timeout-based locking.
- **Banking System**: Ensuring safe account transactions through multi-threading.
- **Interprocess Communication (IPC)**: Using **pipes** for parent-child process communication.

## 🛠️ Tech Stack
- **Language**: C#
- **Framework**: .NET 8.0
- **Platform**: macOS (via UTM) 

---

## 📂 Project Structure

---

## 🚀 Setup Instructions

### **1️⃣ Install Prerequisites**
Ensure you have the following installed:
- **.NET SDK 8.0+** 
- **Ubuntu Server**
- **UTM**
- **LinuxL** (if using Windows)

### **2️⃣ Clone the Repository**
```sh
git clone https://github.com/yourusername/OS_Project.git
cd OS_Project


🔄 Multi-Threading Features
1️⃣ Multi-Threaded Counter
Spawns 10 threads, each incrementing a shared counter 1000 times.
Uses locks to prevent race conditions.
2️⃣ Deadlock Scenario
Two threads lock two shared resources in opposite order, causing a deadlock.
3️⃣ Deadlock Resolution
Uses Monitor.TryEnter() to prevent deadlocks by adding timeouts.
4️⃣ Banking System (Safe Transfers)
Ensures safe money transfers between accounts using ordered locking.
Prevents deadlocks and race conditions.


🔗 Interprocess Communication (IPC) Features
1️⃣ Parent Process (ParentProcess.cs)
Creates a pipe using AnonymousPipeServerStream.
Starts the child process and sends data through the pipe.
2️⃣ Child Process (ChildProcess.cs)
Reads the pipe handle.
Receives and prints the message.
