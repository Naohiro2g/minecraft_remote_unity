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

Assetes/Scripts/  C# scripts

- TCPServer.cs: TCP server, multi-threaded, multi-client
- MinecraftRemoteReceiver.cs: Receiver for Minecraft Remote messages
- MinecraftRemoteSender.cs: Sender for Minecraft Remote messages
- send_demo_01.cs: C# sender demo

PythonScripts/  Python scripts

- digital_clock.py:
- set_columns1.py: builds columns
- set_columns2.py: builds columns
- set_pyramids.py: builds pyramids
- set_maze.py: builds mazes
