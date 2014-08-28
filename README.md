Asimov
======

Asimov is an interactive robot built on an iRobot Create and Microsoft Kinect. It supports basic voice and gesture commands as well as various operating modes.

## Depenencies

Asimov was created on top of the [libBiscuit API](https://github.com/shanet/libbiscuit). It must be compiled before compiling Asimov. See the README in the libBiscuit repo for instructions and more info on the API.

## Building

Asimov uses a client-server model. The server is written in C (to interface with libBiscuit) and the client is written in C#.

### Server

1. The server is statically linked with the libBiscuit library (added a submodule in the `libs` directory). Ensure that it is compiled before compiling the server.
1. Simply run `make all` at the top level of this repo to build the server. The build binary will be placed in the `bin` directory.
1. Optional: Install the server by running `make install`.
1. The server utilizies the GNU `getopt` library. The easiest way to run it on Windows is through the use of Cgywin.

### Client

1. The client relies on the Microsoft Kinect API. However, this is included in the `libs` directory. No other dependencies should be required.
1. Open the `Asimov.sln` solution with Visual Studio and build or run the project like any VS project.

## Team

libBiscuit API & server:
* Shane Tully
* Emily Last

Client and Kinect interaction:
* Gage Ames
* Aaron Goodermuth
* Trevor Sprinkle
