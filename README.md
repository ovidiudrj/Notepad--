# 📝 Notepad-Clone

A feature-rich, multi-tabbed text editor and workspace manager built from scratch using **C#** and **WPF**. This application acts as a lightweight clone of Notepad++, strictly adhering to the **MVVM (Model-View-ViewModel)** architectural pattern. 

It was designed to showcase advanced Windows desktop development concepts, including complex Data Binding, custom command routing, and deep File System I/O operations.

## ✨ Key Features

* **🗂️ Workspace Explorer (Lazy Loading):** Integrated TreeView file explorer. Uses a custom Lazy Loading implementation to only read sub-directories when expanded, ensuring high performance even in large projects.
* **📑 Multi-Tab Document Management:** Open, edit, and manage multiple files simultaneously using `ObservableCollection`. The app tracks unsaved changes (`IsDirty` state) and safely prompts the user before closing tabs.
* **🔍 Advanced Search & Replace:** A modeless Search window injected with the main context via Dependency Injection. Supports:
    * Find & Replace (first instance).
    * Replace All (Current Tab).
    * Replace All (Across all open tabs).
* **💾 Deep File System I/O:** * Recursive directory cloning (Depth-First traversal).
    * Intelligent collision avoidance (auto-renaming like `new_file_1.txt` to prevent overwriting).
    * Direct interoperability with the Windows OS Clipboard (Copy Path).
* **⚙️ Custom MVVM Framework:** Implements a boilerplate-free MVVM architecture using a custom `ViewModelBase` (leveraging `INotifyPropertyChanged` and `[CallerMemberName]`) and `RelayCommand` (`ICommand`) for strict separation of concerns.

## 🛠️ Tech Stack

* **Language:** C#
* **Framework:** .NET / WPF (Windows Presentation Foundation)
* **Architecture:** MVVM (Model-View-ViewModel)
* **UI/Markup:** XAML
