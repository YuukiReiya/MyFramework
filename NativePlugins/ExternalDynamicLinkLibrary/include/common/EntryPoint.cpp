/*
*	
*/
#include <Windows.h>

/*
*   �R�s�y:https://learn.microsoft.com/ja-jp/windows/win32/dlls/dynamic-link-library-entry-point-function#:~:text=BOOL%20WINAPI%20DllMain,TRUE%3B%20%20//%20Successful%20DLL_PROCESS_ATTACH.%0A%7D
*/
BOOL WINAPI DllMain(
    HINSTANCE hinstDLL,  // handle to DLL module
    DWORD fdwReason,     // reason for calling function
    LPVOID lpReserved)  // reserved
{
    // Perform actions based on the reason for calling.
    switch (fdwReason)
    {
    case DLL_PROCESS_ATTACH:
        // Initialize once for each new process.
        // Return FALSE to fail DLL load.
        break;

    case DLL_THREAD_ATTACH:
        // Do thread-specific initialization.
        break;

    case DLL_THREAD_DETACH:
        // Do thread-specific cleanup.
        break;

    case DLL_PROCESS_DETACH:
        // Perform any necessary cleanup.
        break;
    }
    return TRUE;  // Successful DLL_PROCESS_ATTACH.
}