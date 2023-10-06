# Naohiro2g / minecraft_remote_unity

Minecraft Remote for Unity

## A brother project of [Minecraft Remote](https://github.com/naohiro2g/minecraft_remote) for Unity

### Server/receiver: Python to Unity world

- It allows you to build something/anything in Unity world with blocks/voxels in Python.
   C# server listens to Python client and builds blocks in Unity world.
- The Python codes are fully compatible with the Minecraft Remote.

### Client/sender: C# to Unity world, or Minecraft world

- You can write code in C# in place of Python.
   C# client sends commands to the C# server, then receiver builds blocks in Unity world.
   Or you can send commands to Minecraft world with the same code.

## Script files and how to use

Assetes/Scripts/

- TCPServer.cs: C# server script
- MinecraftRemoteReceiver.cs: C# receiver script
- MinecraftRemoteSender.cs: C# sender script
- send_demo_01.cs: C# sender demo script

PythonScripts/

- digital_clock.py: Python client script
- set_columns1.py: Python client script
- set_columns2.py: Python client script
- set_pyramids.py: Python client script
- set_maze.py: Python client script
