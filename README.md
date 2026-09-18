# 📝 Notepad--

A text editor and workspace manager built from scratch using **C#**. This application acts as a lightweight clone of Notepad++. 

##  Key Features

* **Workspace Explorer:** Integrated file explorer.
* **Multi-Tab Document Management:** Open, edit, and manage multiple files simultaneously using `ObservableCollection`.
* **Advanced Search & Replace:** A modeless Search window injected with the main context via Dependency Injection. Supports:
    * Find & Replace (first instance).
    * Replace All (Current Tab).
    * Replace All (Across all open tabs).
* **Deep File System I/O:** 
    * Intelligent collision avoidance (auto-renaming like `new_file_1.txt` to prevent overwriting).
    * Direct interoperability with the Windows OS Clipboard (Copy Path).
