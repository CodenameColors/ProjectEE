using System;
using System.IO;
using System.Runtime.InteropServices;
using BixBite.Audio.FMod.FMODLibrary.PlatformLoader;
using BixBite.Audio.FMod.FMODLibrary.Studio;

namespace BixBite.Audio.FMod.FMODLibrary
{
	public enum FMODMode
	{ 
		Core,
		CoreAndStudio
	}

	public static class FMODManager
	{
		/// <summary>
		/// This is the FMOD version which was tested on this
		/// version of the library. Other versions may work, 
		/// but this is not guaranteed.
		/// Visit https://fmod.com/download
		/// </summary>
		public const string RecommendedNativeLibraryVersion = "2.00.08";

		private static FMODMode _mode;

		internal static bool _initialized { get; private set; } = false;

		public static bool UsesStudio => 
			_mode == FMODMode.CoreAndStudio;

		/// <summary>
		/// Initializes systems and loads the native libraries. Can only be called once. 
		/// </summary>
		/// <param name="preInitAction">Executes before initialization, but after the native instance creation.</param>
		public static void Init(
			FMODMode mode,
			string rootDir,
			int maxChannels = 256,
			uint dspBufferLength = 4,
			int dspBufferCount = 32,
			FMOD.INITFLAGS coreInitFlags = FMOD.INITFLAGS.CHANNEL_LOWPASS | FMOD.INITFLAGS.CHANNEL_DISTANCEFILTER,
			FMOD.Studio.INITFLAGS studioInitFlags = FMOD.Studio.INITFLAGS.NORMAL,
			Action preInitAction = null
		)
		{
			if (_initialized)
			{
				throw new Exception("Manager is already initialized!");	
			}
			_initialized = true;
			_mode = mode;

			FileLoader.RootDirectory = rootDir;
			NativeLibraryLoader.LoadNativeLibrary("fmod");

			if (UsesStudio)
			{ 
				NativeLibraryLoader.LoadNativeLibrary("fmodstudio");

				FMOD.Studio.System.create(out StudioSystem.Native);
				
				StudioSystem.Native.getCoreSystem(out CoreSystem.Native);
				
				preInitAction?.Invoke();

				// This also will init core system. 
				StudioSystem.Native.initialize(maxChannels, studioInitFlags, coreInitFlags, (IntPtr)0);
			}
			else
			{
				FMOD.Factory.System_Create(out CoreSystem.Native);

				preInitAction?.Invoke();

				CoreSystem.Native.init(maxChannels, coreInitFlags, (IntPtr)0);
			}

			// Too high values will cause sound lag.
			CoreSystem.Native.setDSPBufferSize(dspBufferLength, dspBufferCount);

		}

		public static void Update()
		{ 
			CheckInitialized();
			if (UsesStudio)
			{ 
				// Studio update updates core system internally.
				// 2020 design awards winner material right here.
				StudioSystem.Native.update();
			}
			else
			{
				CoreSystem.Native.update();	
			}
		}

		public static void Unload()
		{
			CheckInitialized();
			if (UsesStudio)
			{
				StudioSystem.Native.release();
			}
			else
			{
				CoreSystem.Native.release();
			}
		}

		private static void CheckInitialized()
		{ 
			if (!_initialized)
			{ 
				throw new Exception("You need to call Init() before calling this method!");
			}
		}
	}
#if WINDOWS || LINUX
	/// Windows and Linux-specific part of an audio manager.
	public static class NativeLibraryLoader
	{
		[DllImport("kernel32.dll")]
		private static extern IntPtr LoadLibrary(string dllToLoad);

		// NOTE: To make native libraries work on Linux, we also need <dllmap> entries in App.config.
		[DllImport("libdl.so.2")]
		private static extern IntPtr dlopen(string filename, int flags);

		private const int RTLD_LAZY = 0x0001;

		/// <summary>
		/// Loads Windows or Linux native library.
		/// </summary>
		public static void LoadNativeLibrary(string libName)
		{
			if (Environment.OSVersion.Platform != PlatformID.Unix)
			{
				if (Environment.Is64BitProcess)
				{
					LoadLibrary(Path.GetFullPath("Resources/FMOD/x64/" + libName + ".dll"));
				}
				else
				{
					LoadLibrary(Path.GetFullPath("Resources/FMOD/x86/" + libName + ".dll"));
				}
			}
			else
			{
				if (Environment.Is64BitProcess)
				{
					dlopen(Path.GetFullPath("Resources/FMOD/x64/lib" + libName + ".so"), RTLD_LAZY);
				}
				else
				{
					dlopen(Path.GetFullPath("Resources/FMOD/x86/lib" + libName + ".so"), RTLD_LAZY);
				}
			}
		}
	}
#endif

#if ANDROID
	/// <summary>
	/// Android-specific part of the audio manager.
	/// </summary>
	public static class NativeLibraryLoader
	{
		/// <summary>
		/// Loads Android version of FMOD library.
		/// </summary>
		public static void LoadNativeLibrary(string libName) =>
			Java.Lang.JavaSystem.LoadLibrary(libName);
	}
#endif

}
