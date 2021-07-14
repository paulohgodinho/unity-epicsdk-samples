using System;
using System.IO;
using System.Runtime.InteropServices;
using Epic.OnlineServices;
using UnityEngine;

public class LoadEpicDLL : MonoBehaviour
{
#if UNITY_EDITOR
    [DllImport("Kernel32.dll")]
    private static extern IntPtr LoadLibrary(string lpLibFileName);

    [DllImport("Kernel32.dll")]
    private static extern int FreeLibrary(IntPtr hLibModule);

    [DllImport("Kernel32.dll")]
    private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

    private IntPtr _libraryPointer;

    void Awake()
    {
        if (Application.isEditor == false)
        {
            Destroy(this.gameObject);
            return;
        }
        
        var libraryPath = Path.Combine(Application.dataPath, "EpicSDK", "Plugins", "x86_64", $"{Config.LibraryName}.dll");
        
        _libraryPointer = LoadLibrary(libraryPath);
        if (_libraryPointer == IntPtr.Zero)
        {
            throw new Exception("Failed to load library" + libraryPath);
        }

        Bindings.Hook(_libraryPointer, GetProcAddress);
        Debug.Log("<b>[LoadEpicLibsOnEditor]</b> Hooked to Epic Library on Editor");

        EpicPlatform.Instance.OnAfterPlatformReleasedAndShutdown += Unhook;
    }

    private void Unhook()
    {
        if (_libraryPointer != IntPtr.Zero)
        {
            // Free until the module ref count is 0
            while (FreeLibrary(_libraryPointer) != 0) { }
            _libraryPointer = IntPtr.Zero;

            Bindings.Unhook();
            Debug.Log("<b>[LoadEpicLibsOnEditor]</b> Unhooked from Epic Library on Editor");
        }
    }
    
#endif
}