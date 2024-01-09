![CPU IMAGE, GRAPHIC, ICON](Images/CPU_ICON.png)

# DESCRIPTION

Welcome to the Memory Scanner/Debugger for Emulators Project! This tool is designed to help other developers or enthusiasts to gain a better understanding of how memory works within a games process by providing a real-time insight into the memory usage of the emulator

# FEATURES

- Memory Scanning: With the help of the imported functions, the program scans the memory of the emulator. It reads the memory addresses and offsets, converts them into a readable format, and displays the result in a user-friendly interface.
- Read Memory: We can read different data types including 2 Byte Big Endian, 4 Byte Big Endian, Float Big Endian and Double Big Endian from a processes memory. This is especially useful for emulators such as RPCS3 as they emulate the PS3 console which uses Big Endian data types
- Write Memory: We can also write to memory, modifying values
- Pointer Scan: Processes can use dynamic addresses so a pointer scanner identifies and tracks pointers in a processes memory. Pointers are variables that store the memory addresses of other values. By tracking these pointers, a pointer scanner can help you understand how and where a process is storing and accessing data in memory. This can be helpful for video-game hacking

# HOW IT WORKS

This program works by using the WinAPI (Windows API) DLL Imports. The Windows API is a collection of functions that allow developers to interact with the operating system and perform a variety of tasks including manipulating files, interfacing with devices, and interacting with system services

# HOW TO USE

1. Install the program

2. Name your RPCS3 exe to "rpcs3"

3. Launch the program and press connect

![MEMORY SCANNER PROGRAM IMAGE](Images/Memory_Scanner.png)
