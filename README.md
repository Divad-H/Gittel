# Gittel

This is a work in progress to create a proof of concept of a git gui that uses the [libgit2](https://libgit2.github.com/) library 
with the help of generated C# bindings using [CppSharp](https://github.com/mono/CppSharp/). The GUI is written in TypeScript using Angular
and is hosted in WebView2. An interface between the C# and TypeScript code is generated from controller classes using a simple code generator
and [TypeGen](https://github.com/jburzynski/TypeGen) for TypeScript dto types.

## Building

Currently it is necessary to build three steps manually:

* Build libgit2 using CMake
* Build the C# bindings for libgit2 by running Libgit2BindingsGenerator
* Build the Solution
