// dllmain.cpp : Defines the entry point for the DLL application.
#include "pch.h"
#include <iostream>

BOOL APIENTRY DllMain(HMODULE hModule,
    DWORD  ul_reason_for_call,
    LPVOID lpReserved
)
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}

#pragma pack(push, 1)
struct AdditionParams
{
    int first;
    int second;
};
#pragma pack(pop)

#pragma pack(push, 1)
struct MyFunctionParams
{
    char* data;
    int number;
};
#pragma pack(pop)

int __declspec(dllexport) Main(void)
{
    AllocConsole();
    freopen("CONOUT$", "w", stdout);

    std::cout << "Hello from cpp library dllmain::Main!" << std::endl;

    return 1;
}

int __declspec(dllexport) Add(AdditionParams* params)
{
    std::cout << "Performing addition of numbers " << params->first << " and " << params->second << std::endl;

    return params->first + params->second;
}

bool __declspec(dllexport) MyFunction(MyFunctionParams* params)
{
    std::cout << "Hello from dllmain::MyFunction" << std::endl;
    std::cout << "  Data: " << params->data << std::endl;
    std::cout << "  Number: " << params->number << std::endl;

    return true;
}