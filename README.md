# Important

Only one function parameter can be passed.
x64 processes aren't currently supported. (maybe just the dlls I tried to inject were the problem)

## C-Sharp dll injector

This is a simple C# dll injector. It can inject dll into remote process and execute functions of it.

## Examples

- /examples/csharp-dllexport
  - Injection of C# library using DllExport
  
- /examples/cpp
  - Injection of cpp dynamic library
  
## Getting started

- To inject dll use `Injector` class, the `Inject`  method will return `InjectedModule` with `ExecuteFunction` method
```
InjectedModule injectedModule = Injector.Inject("RemoteProcess", "MyDynamicLibrary.dll");
FunctionResult result = injectedModule.ExecuteFunction("Main");

int integerResult = result.To<int>(); // Primitive types are casted from IntPtr, Reference types are Read from pointer
```

- See examples for more