# chromium-unity-server

**Proxy server for an embedded Chromium browser in your Unity games.**

🚧 This is a work in progress, do not use this 🚧

![Embedded Chrome browser ingame in Unity](screenshot.PNG)

🚧 This is a work in progress, do not use this 🚧

## Features

- Fast and low overhead communication using named pipes, support high FPS
- Pass mouse and key input events down to underlying browser
- Bi-directional messaging between C# and JavaScript

The project consists of two parts: First, the server, which manages a CEF (Chrome Embedded Framework) browser instance.
Second, a .NET library for integration in Unity or other applications, which communicates with the server via named pipe.

## Requirements

***Currently* only targets Windows.** Mono support is possible in the underlying libraries and APIs, so Linux and Mac support is possible with some additional work.

.NET 4.x scripting compatibility is required, you will **need to enable the "Expirimental" scripting engine** for your unity project. The legacy / stable scripting engine will not work, as it does *not* support named pipes.

**Only 64-bit architectures** are supported. If you are looking for x86 support, this project is not for you.
