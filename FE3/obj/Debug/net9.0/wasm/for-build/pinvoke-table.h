// GENERATED FILE, DO NOT MODIFY");


uint32_t CompressionNative_Crc32 (uint32_t, void *, int32_t);

int32_t CompressionNative_Deflate (void *, int32_t);

int32_t CompressionNative_DeflateEnd (void *);

int32_t CompressionNative_DeflateInit2_ (void *, int32_t, int32_t, int32_t, int32_t, int32_t);

int32_t CompressionNative_Inflate (void *, int32_t);

int32_t CompressionNative_InflateEnd (void *);

int32_t CompressionNative_InflateInit2_ (void *, int32_t);

int32_t eglBindAPI (int32_t);

int32_t eglChooseConfig (void *, void *, void *, void *, void *);

void * eglCreateContext (void *, void *, void *, void *);

void * eglCreateWindowSurface (void *, void *, void *, void *);

int32_t eglDestroyContext (void *, void *);

int32_t eglDestroySurface (void *, void *);

int32_t eglGetConfigAttrib (void *, void *, void *, void *);

void * eglGetDisplay (void *);

int32_t eglGetError ();

void * eglGetProcAddress (void *);

int32_t eglInitialize (void *, void *, void *);

int32_t eglMakeCurrent (void *, void *, void *, void *);

int32_t eglTerminate (void *);

void GlobalizationNative_ChangeCase (void *, int32_t, void *, int32_t, int32_t);

void GlobalizationNative_ChangeCaseInvariant (void *, int32_t, void *, int32_t, int32_t);

void GlobalizationNative_ChangeCaseTurkish (void *, int32_t, void *, int32_t, int32_t);

void GlobalizationNative_CloseSortHandle (void *);

int32_t GlobalizationNative_CompareString (void *, void *, int32_t, void *, int32_t, int32_t);

int32_t GlobalizationNative_EndsWith (void *, void *, int32_t, void *, int32_t, int32_t, void *);

int32_t GlobalizationNative_EnumCalendarInfo (void *, void *, uint32_t, int32_t, void *);

int32_t GlobalizationNative_GetCalendarInfo (void *, uint32_t, int32_t, void *, int32_t);

int32_t GlobalizationNative_GetCalendars (void *, void *, int32_t);

int32_t GlobalizationNative_GetDefaultLocaleName (void *, int32_t);

int32_t GlobalizationNative_GetICUVersion ();

int32_t GlobalizationNative_GetJapaneseEraStartDate (int32_t, void *, void *, void *);

int32_t GlobalizationNative_GetLatestJapaneseEra ();

int32_t GlobalizationNative_GetLocaleInfoGroupingSizes (void *, uint32_t, void *, void *);

int32_t GlobalizationNative_GetLocaleInfoInt (void *, uint32_t, void *);

int32_t GlobalizationNative_GetLocaleInfoString (void *, uint32_t, void *, int32_t, void *);

int32_t GlobalizationNative_GetLocaleName (void *, void *, int32_t);

int32_t GlobalizationNative_GetLocales (void *, int32_t);

int32_t GlobalizationNative_GetLocaleTimeFormat (void *, int32_t, void *, int32_t);

int32_t GlobalizationNative_GetSortHandle (void *, void *);

int32_t GlobalizationNative_GetSortKey (void *, void *, int32_t, void *, int32_t, int32_t);

int32_t GlobalizationNative_GetSortVersion (void *);

int32_t GlobalizationNative_IndexOf (void *, void *, int32_t, void *, int32_t, int32_t, void *);

void GlobalizationNative_InitICUFunctions (void *, void *, void *, void *);

void GlobalizationNative_InitOrdinalCasingPage (int32_t, void *);

int32_t GlobalizationNative_IsNormalized (int32_t, void *, int32_t);

int32_t GlobalizationNative_IsPredefinedLocale (void *);

int32_t GlobalizationNative_LastIndexOf (void *, void *, int32_t, void *, int32_t, int32_t, void *);

int32_t GlobalizationNative_LoadICU ();

int32_t GlobalizationNative_NormalizeString (int32_t, void *, int32_t, void *, int32_t);

int32_t GlobalizationNative_StartsWith (void *, void *, int32_t, void *, int32_t, int32_t, void *);

int32_t GlobalizationNative_ToAscii (uint32_t, void *, int32_t, void *, int32_t);

int32_t GlobalizationNative_ToUnicode (uint32_t, void *, int32_t, void *, int32_t);

int32_t SystemNative_Access (void *, int32_t);

void * SystemNative_AlignedAlloc (void *, void *);

void SystemNative_AlignedFree (void *);

void * SystemNative_AlignedRealloc (void *, void *, void *);

void * SystemNative_Calloc (void *, void *);

int32_t SystemNative_CanGetHiddenFlag ();

int32_t SystemNative_ChDir (void *);

int32_t SystemNative_ChMod (void *, int32_t);

int32_t SystemNative_Close (void *);

int32_t SystemNative_CloseDir (void *);

int32_t SystemNative_ConvertErrorPalToPlatform (int32_t);

int32_t SystemNative_ConvertErrorPlatformToPal (int32_t);

int32_t SystemNative_CopyFile (void *, void *, int64_t);

void * SystemNative_Dup (void *);

int32_t SystemNative_FAllocate (void *, int64_t, int64_t);

int32_t SystemNative_FChflags (void *, uint32_t);

int32_t SystemNative_FChMod (void *, int32_t);

int32_t SystemNative_FcntlSetFD (void *, int32_t);

int32_t SystemNative_FLock (void *, int32_t);

void SystemNative_Free (void *);

void SystemNative_FreeEnviron (void *);

int32_t SystemNative_FStat (void *, void *);

int32_t SystemNative_FSync (void *);

int32_t SystemNative_FTruncate (void *, int64_t);

int32_t SystemNative_FUTimens (void *, void *);

int32_t SystemNative_GetAddressFamily (void *, int32_t, void *);

double SystemNative_GetCpuUtilization (void *);

int32_t SystemNative_GetCryptographicallySecureRandomBytes (void *, int32_t);

void * SystemNative_GetCwd (void *, int32_t);

void * SystemNative_GetDefaultSearchOrderPseudoHandle ();

void * SystemNative_GetEnv (void *);

void * SystemNative_GetEnviron ();

int32_t SystemNative_GetErrNo ();

uint32_t SystemNative_GetFileSystemType (void *);

int32_t SystemNative_GetIPv4Address (void *, int32_t, void *);

int32_t SystemNative_GetIPv6Address (void *, int32_t, void *, int32_t, void *);

void SystemNative_GetNonCryptographicallySecureRandomBytes (void *, int32_t);

int32_t SystemNative_GetPort (void *, int32_t, void *);

int32_t SystemNative_GetReadDirRBufferSize ();

int32_t SystemNative_GetSocketAddressSizes (void *, void *, void *, void *);

int64_t SystemNative_GetSystemTimeAsTicks ();

uint64_t SystemNative_GetTimestamp ();

void * SystemNative_GetTimeZoneData (void *, void *);

int32_t SystemNative_LChflags (void *, uint32_t);

int32_t SystemNative_LChflagsCanSetHiddenFlag ();

int32_t SystemNative_Link (void *, void *);

int32_t SystemNative_LockFileRegion (void *, int64_t, int64_t, int32_t);

void SystemNative_Log (void *, int32_t);

void SystemNative_LogError (void *, int32_t);

void SystemNative_LowLevelMonitor_Acquire (void *);

void * SystemNative_LowLevelMonitor_Create ();

void SystemNative_LowLevelMonitor_Destroy (void *);

void SystemNative_LowLevelMonitor_Release (void *);

void SystemNative_LowLevelMonitor_Signal_Release (void *);

int32_t SystemNative_LowLevelMonitor_TimedWait (void *, int32_t);

void SystemNative_LowLevelMonitor_Wait (void *);

int64_t SystemNative_LSeek (void *, int64_t, int32_t);

int32_t SystemNative_LStat (void *, void *);

int32_t SystemNative_MAdvise (void *, uint64_t, int32_t);

void * SystemNative_Malloc (void *);

int32_t SystemNative_MkDir (void *, int32_t);

void * SystemNative_MkdTemp (void *);

void * SystemNative_MksTemps (void *, int32_t);

void * SystemNative_MMap (void *, uint64_t, int32_t, int32_t, void *, int64_t);

int32_t SystemNative_MSync (void *, uint64_t, int32_t);

int32_t SystemNative_MUnmap (void *, uint64_t);

void * SystemNative_Open (void *, int32_t, int32_t);

void * SystemNative_OpenDir (void *);

int32_t SystemNative_PosixFAdvise (void *, int64_t, int64_t, int32_t);

int32_t SystemNative_PRead (void *, void *, int32_t, int64_t);

int64_t SystemNative_PReadV (void *, void *, int32_t, int64_t);

int32_t SystemNative_PWrite (void *, void *, int32_t, int64_t);

int64_t SystemNative_PWriteV (void *, void *, int32_t, int64_t);

int32_t SystemNative_Read (void *, void *, int32_t);

int32_t SystemNative_ReadDirR (void *, void *, int32_t, void *);

int32_t SystemNative_ReadLink (void *, void *, int32_t);

void * SystemNative_Realloc (void *, void *);

int32_t SystemNative_Rename (void *, void *);

int32_t SystemNative_RmDir (void *);

int32_t SystemNative_SchedGetCpu ();

int32_t SystemNative_SetAddressFamily (void *, int32_t, int32_t);

void SystemNative_SetErrNo (int32_t);

int32_t SystemNative_SetIPv4Address (void *, int32_t, uint32_t);

int32_t SystemNative_SetIPv6Address (void *, int32_t, void *, int32_t, uint32_t);

int32_t SystemNative_SetPort (void *, int32_t, uint32_t);

void * SystemNative_ShmOpen (void *, int32_t, int32_t);

int32_t SystemNative_ShmUnlink (void *);

int32_t SystemNative_Stat (void *, void *);

void * SystemNative_StrErrorR (int32_t, void *, int32_t);

int32_t SystemNative_SymLink (void *, void *);

int64_t SystemNative_SysConf (int32_t);

void SystemNative_SysLog (int32_t, void *, void *);

uint32_t SystemNative_TryGetUInt32OSThreadId ();

int32_t SystemNative_Unlink (void *);

int32_t SystemNative_UTimensat (void *, void *);

int32_t SystemNative_Write (void *, void *, int32_t);
static PinvokeImport libEGL_imports [] = {
    {"eglBindAPI", eglBindAPI}, // Ab4d.SharpEngine.Web
    {"eglChooseConfig", eglChooseConfig}, // Ab4d.SharpEngine.Web
    {"eglCreateContext", eglCreateContext}, // Ab4d.SharpEngine.Web
    {"eglCreateWindowSurface", eglCreateWindowSurface}, // Ab4d.SharpEngine.Web
    {"eglDestroyContext", eglDestroyContext}, // Ab4d.SharpEngine.Web
    {"eglDestroySurface", eglDestroySurface}, // Ab4d.SharpEngine.Web
    {"eglGetConfigAttrib", eglGetConfigAttrib}, // Ab4d.SharpEngine.Web
    {"eglGetDisplay", eglGetDisplay}, // Ab4d.SharpEngine.Web
    {"eglGetError", eglGetError}, // Ab4d.SharpEngine.Web
    {"eglGetProcAddress", eglGetProcAddress}, // Ab4d.SharpEngine.Web
    {"eglInitialize", eglInitialize}, // Ab4d.SharpEngine.Web
    {"eglMakeCurrent", eglMakeCurrent}, // Ab4d.SharpEngine.Web
    {"eglTerminate", eglTerminate}, // Ab4d.SharpEngine.Web
    {NULL, NULL}
};
static PinvokeImport libSystem_Native_imports [] = {
    {"SystemNative_Access", SystemNative_Access}, // System.Private.CoreLib
    {"SystemNative_AlignedAlloc", SystemNative_AlignedAlloc}, // System.Private.CoreLib
    {"SystemNative_AlignedFree", SystemNative_AlignedFree}, // System.Private.CoreLib
    {"SystemNative_AlignedRealloc", SystemNative_AlignedRealloc}, // System.Private.CoreLib
    {"SystemNative_Calloc", SystemNative_Calloc}, // System.Private.CoreLib
    {"SystemNative_CanGetHiddenFlag", SystemNative_CanGetHiddenFlag}, // System.Private.CoreLib
    {"SystemNative_ChDir", SystemNative_ChDir}, // System.Private.CoreLib
    {"SystemNative_ChMod", SystemNative_ChMod}, // System.Private.CoreLib
    {"SystemNative_Close", SystemNative_Close}, // System.Private.CoreLib
    {"SystemNative_CloseDir", SystemNative_CloseDir}, // System.Private.CoreLib
    {"SystemNative_ConvertErrorPalToPlatform", SystemNative_ConvertErrorPalToPlatform}, // System.Console, System.IO.Compression.ZipFile, System.IO.MemoryMappedFiles, System.Net.Primitives, System.Private.CoreLib
    {"SystemNative_ConvertErrorPlatformToPal", SystemNative_ConvertErrorPlatformToPal}, // System.Console, System.IO.Compression.ZipFile, System.IO.MemoryMappedFiles, System.Net.Primitives, System.Private.CoreLib
    {"SystemNative_CopyFile", SystemNative_CopyFile}, // System.Private.CoreLib
    {"SystemNative_Dup", SystemNative_Dup}, // System.Console
    {"SystemNative_FAllocate", SystemNative_FAllocate}, // System.Private.CoreLib
    {"SystemNative_FChflags", SystemNative_FChflags}, // System.Private.CoreLib
    {"SystemNative_FChMod", SystemNative_FChMod}, // System.Private.CoreLib
    {"SystemNative_FcntlSetFD", SystemNative_FcntlSetFD}, // System.IO.MemoryMappedFiles
    {"SystemNative_FLock", SystemNative_FLock}, // System.Private.CoreLib
    {"SystemNative_Free", SystemNative_Free}, // System.Private.CoreLib
    {"SystemNative_FreeEnviron", SystemNative_FreeEnviron}, // System.Private.CoreLib
    {"SystemNative_FStat", SystemNative_FStat}, // System.IO.Compression.ZipFile, System.IO.MemoryMappedFiles, System.Private.CoreLib
    {"SystemNative_FSync", SystemNative_FSync}, // System.Private.CoreLib
    {"SystemNative_FTruncate", SystemNative_FTruncate}, // System.IO.MemoryMappedFiles, System.Private.CoreLib
    {"SystemNative_FUTimens", SystemNative_FUTimens}, // System.Private.CoreLib
    {"SystemNative_GetAddressFamily", SystemNative_GetAddressFamily}, // System.Net.Primitives
    {"SystemNative_GetCpuUtilization", SystemNative_GetCpuUtilization}, // System.Private.CoreLib
    {"SystemNative_GetCryptographicallySecureRandomBytes", SystemNative_GetCryptographicallySecureRandomBytes}, // System.Private.CoreLib, System.Security.Cryptography
    {"SystemNative_GetCwd", SystemNative_GetCwd}, // System.Private.CoreLib
    {"SystemNative_GetDefaultSearchOrderPseudoHandle", SystemNative_GetDefaultSearchOrderPseudoHandle}, // System.Private.CoreLib
    {"SystemNative_GetEnv", SystemNative_GetEnv}, // System.Private.CoreLib
    {"SystemNative_GetEnviron", SystemNative_GetEnviron}, // System.Private.CoreLib
    {"SystemNative_GetErrNo", SystemNative_GetErrNo}, // System.Private.CoreLib
    {"SystemNative_GetFileSystemType", SystemNative_GetFileSystemType}, // System.Private.CoreLib
    {"SystemNative_GetIPv4Address", SystemNative_GetIPv4Address}, // System.Net.Primitives
    {"SystemNative_GetIPv6Address", SystemNative_GetIPv6Address}, // System.Net.Primitives
    {"SystemNative_GetNonCryptographicallySecureRandomBytes", SystemNative_GetNonCryptographicallySecureRandomBytes}, // System.Private.CoreLib
    {"SystemNative_GetPort", SystemNative_GetPort}, // System.Net.Primitives
    {"SystemNative_GetReadDirRBufferSize", SystemNative_GetReadDirRBufferSize}, // System.Private.CoreLib
    {"SystemNative_GetSocketAddressSizes", SystemNative_GetSocketAddressSizes}, // System.Net.Primitives
    {"SystemNative_GetSystemTimeAsTicks", SystemNative_GetSystemTimeAsTicks}, // System.Private.CoreLib
    {"SystemNative_GetTimestamp", SystemNative_GetTimestamp}, // System.Private.CoreLib
    {"SystemNative_GetTimeZoneData", SystemNative_GetTimeZoneData}, // System.Private.CoreLib
    {"SystemNative_LChflags", SystemNative_LChflags}, // System.Private.CoreLib
    {"SystemNative_LChflagsCanSetHiddenFlag", SystemNative_LChflagsCanSetHiddenFlag}, // System.Private.CoreLib
    {"SystemNative_Link", SystemNative_Link}, // System.Private.CoreLib
    {"SystemNative_LockFileRegion", SystemNative_LockFileRegion}, // System.Private.CoreLib
    {"SystemNative_Log", SystemNative_Log}, // System.Private.CoreLib
    {"SystemNative_LogError", SystemNative_LogError}, // System.Private.CoreLib
    {"SystemNative_LowLevelMonitor_Acquire", SystemNative_LowLevelMonitor_Acquire}, // System.Private.CoreLib
    {"SystemNative_LowLevelMonitor_Create", SystemNative_LowLevelMonitor_Create}, // System.Private.CoreLib
    {"SystemNative_LowLevelMonitor_Destroy", SystemNative_LowLevelMonitor_Destroy}, // System.Private.CoreLib
    {"SystemNative_LowLevelMonitor_Release", SystemNative_LowLevelMonitor_Release}, // System.Private.CoreLib
    {"SystemNative_LowLevelMonitor_Signal_Release", SystemNative_LowLevelMonitor_Signal_Release}, // System.Private.CoreLib
    {"SystemNative_LowLevelMonitor_TimedWait", SystemNative_LowLevelMonitor_TimedWait}, // System.Private.CoreLib
    {"SystemNative_LowLevelMonitor_Wait", SystemNative_LowLevelMonitor_Wait}, // System.Private.CoreLib
    {"SystemNative_LSeek", SystemNative_LSeek}, // System.Private.CoreLib
    {"SystemNative_LStat", SystemNative_LStat}, // System.Private.CoreLib
    {"SystemNative_MAdvise", SystemNative_MAdvise}, // System.IO.MemoryMappedFiles
    {"SystemNative_Malloc", SystemNative_Malloc}, // System.Private.CoreLib
    {"SystemNative_MkDir", SystemNative_MkDir}, // System.Private.CoreLib
    {"SystemNative_MkdTemp", SystemNative_MkdTemp}, // System.Private.CoreLib
    {"SystemNative_MksTemps", SystemNative_MksTemps}, // System.Private.CoreLib
    {"SystemNative_MMap", SystemNative_MMap}, // System.IO.MemoryMappedFiles
    {"SystemNative_MSync", SystemNative_MSync}, // System.IO.MemoryMappedFiles
    {"SystemNative_MUnmap", SystemNative_MUnmap}, // System.IO.MemoryMappedFiles
    {"SystemNative_Open", SystemNative_Open}, // System.Private.CoreLib
    {"SystemNative_OpenDir", SystemNative_OpenDir}, // System.Private.CoreLib
    {"SystemNative_PosixFAdvise", SystemNative_PosixFAdvise}, // System.Private.CoreLib
    {"SystemNative_PRead", SystemNative_PRead}, // System.Private.CoreLib
    {"SystemNative_PReadV", SystemNative_PReadV}, // System.Private.CoreLib
    {"SystemNative_PWrite", SystemNative_PWrite}, // System.Private.CoreLib
    {"SystemNative_PWriteV", SystemNative_PWriteV}, // System.Private.CoreLib
    {"SystemNative_Read", SystemNative_Read}, // System.Private.CoreLib
    {"SystemNative_ReadDirR", SystemNative_ReadDirR}, // System.Private.CoreLib
    {"SystemNative_ReadLink", SystemNative_ReadLink}, // System.Private.CoreLib
    {"SystemNative_Realloc", SystemNative_Realloc}, // System.Private.CoreLib
    {"SystemNative_Rename", SystemNative_Rename}, // System.Private.CoreLib
    {"SystemNative_RmDir", SystemNative_RmDir}, // System.Private.CoreLib
    {"SystemNative_SchedGetCpu", SystemNative_SchedGetCpu}, // System.Private.CoreLib
    {"SystemNative_SetAddressFamily", SystemNative_SetAddressFamily}, // System.Net.Primitives
    {"SystemNative_SetErrNo", SystemNative_SetErrNo}, // System.Private.CoreLib
    {"SystemNative_SetIPv4Address", SystemNative_SetIPv4Address}, // System.Net.Primitives
    {"SystemNative_SetIPv6Address", SystemNative_SetIPv6Address}, // System.Net.Primitives
    {"SystemNative_SetPort", SystemNative_SetPort}, // System.Net.Primitives
    {"SystemNative_ShmOpen", SystemNative_ShmOpen}, // System.IO.MemoryMappedFiles
    {"SystemNative_ShmUnlink", SystemNative_ShmUnlink}, // System.IO.MemoryMappedFiles
    {"SystemNative_Stat", SystemNative_Stat}, // System.IO.Compression.ZipFile, System.Private.CoreLib
    {"SystemNative_StrErrorR", SystemNative_StrErrorR}, // System.Console, System.IO.Compression.ZipFile, System.IO.MemoryMappedFiles, System.Net.Primitives, System.Private.CoreLib
    {"SystemNative_SymLink", SystemNative_SymLink}, // System.Private.CoreLib
    {"SystemNative_SysConf", SystemNative_SysConf}, // System.IO.MemoryMappedFiles, System.Private.CoreLib
    {"SystemNative_SysLog", SystemNative_SysLog}, // System.Private.CoreLib
    {"SystemNative_TryGetUInt32OSThreadId", SystemNative_TryGetUInt32OSThreadId}, // System.Private.CoreLib
    {"SystemNative_Unlink", SystemNative_Unlink}, // System.IO.MemoryMappedFiles, System.Private.CoreLib
    {"SystemNative_UTimensat", SystemNative_UTimensat}, // System.Private.CoreLib
    {"SystemNative_Write", SystemNative_Write}, // System.Console, System.Private.CoreLib
    {NULL, NULL}
};
static PinvokeImport libSystem_IO_Compression_Native_imports [] = {
    {"CompressionNative_Crc32", CompressionNative_Crc32}, // System.IO.Compression
    {"CompressionNative_Deflate", CompressionNative_Deflate}, // System.IO.Compression, System.Net.WebSockets
    {"CompressionNative_DeflateEnd", CompressionNative_DeflateEnd}, // System.IO.Compression, System.Net.WebSockets
    {"CompressionNative_DeflateInit2_", CompressionNative_DeflateInit2_}, // System.IO.Compression, System.Net.WebSockets
    {"CompressionNative_Inflate", CompressionNative_Inflate}, // System.IO.Compression, System.Net.WebSockets
    {"CompressionNative_InflateEnd", CompressionNative_InflateEnd}, // System.IO.Compression, System.Net.WebSockets
    {"CompressionNative_InflateInit2_", CompressionNative_InflateInit2_}, // System.IO.Compression, System.Net.WebSockets
    {NULL, NULL}
};
static PinvokeImport libSystem_Globalization_Native_imports [] = {
    {"GlobalizationNative_ChangeCase", GlobalizationNative_ChangeCase}, // System.Private.CoreLib
    {"GlobalizationNative_ChangeCaseInvariant", GlobalizationNative_ChangeCaseInvariant}, // System.Private.CoreLib
    {"GlobalizationNative_ChangeCaseTurkish", GlobalizationNative_ChangeCaseTurkish}, // System.Private.CoreLib
    {"GlobalizationNative_CloseSortHandle", GlobalizationNative_CloseSortHandle}, // System.Private.CoreLib
    {"GlobalizationNative_CompareString", GlobalizationNative_CompareString}, // System.Private.CoreLib
    {"GlobalizationNative_EndsWith", GlobalizationNative_EndsWith}, // System.Private.CoreLib
    {"GlobalizationNative_EnumCalendarInfo", GlobalizationNative_EnumCalendarInfo}, // System.Private.CoreLib
    {"GlobalizationNative_GetCalendarInfo", GlobalizationNative_GetCalendarInfo}, // System.Private.CoreLib
    {"GlobalizationNative_GetCalendars", GlobalizationNative_GetCalendars}, // System.Private.CoreLib
    {"GlobalizationNative_GetDefaultLocaleName", GlobalizationNative_GetDefaultLocaleName}, // System.Private.CoreLib
    {"GlobalizationNative_GetICUVersion", GlobalizationNative_GetICUVersion}, // System.Private.CoreLib
    {"GlobalizationNative_GetJapaneseEraStartDate", GlobalizationNative_GetJapaneseEraStartDate}, // System.Private.CoreLib
    {"GlobalizationNative_GetLatestJapaneseEra", GlobalizationNative_GetLatestJapaneseEra}, // System.Private.CoreLib
    {"GlobalizationNative_GetLocaleInfoGroupingSizes", GlobalizationNative_GetLocaleInfoGroupingSizes}, // System.Private.CoreLib
    {"GlobalizationNative_GetLocaleInfoInt", GlobalizationNative_GetLocaleInfoInt}, // System.Private.CoreLib
    {"GlobalizationNative_GetLocaleInfoString", GlobalizationNative_GetLocaleInfoString}, // System.Private.CoreLib
    {"GlobalizationNative_GetLocaleName", GlobalizationNative_GetLocaleName}, // System.Private.CoreLib
    {"GlobalizationNative_GetLocales", GlobalizationNative_GetLocales}, // System.Private.CoreLib
    {"GlobalizationNative_GetLocaleTimeFormat", GlobalizationNative_GetLocaleTimeFormat}, // System.Private.CoreLib
    {"GlobalizationNative_GetSortHandle", GlobalizationNative_GetSortHandle}, // System.Private.CoreLib
    {"GlobalizationNative_GetSortKey", GlobalizationNative_GetSortKey}, // System.Private.CoreLib
    {"GlobalizationNative_GetSortVersion", GlobalizationNative_GetSortVersion}, // System.Private.CoreLib
    {"GlobalizationNative_IndexOf", GlobalizationNative_IndexOf}, // System.Private.CoreLib
    {"GlobalizationNative_InitICUFunctions", GlobalizationNative_InitICUFunctions}, // System.Private.CoreLib
    {"GlobalizationNative_InitOrdinalCasingPage", GlobalizationNative_InitOrdinalCasingPage}, // System.Private.CoreLib
    {"GlobalizationNative_IsNormalized", GlobalizationNative_IsNormalized}, // System.Private.CoreLib
    {"GlobalizationNative_IsPredefinedLocale", GlobalizationNative_IsPredefinedLocale}, // System.Private.CoreLib
    {"GlobalizationNative_LastIndexOf", GlobalizationNative_LastIndexOf}, // System.Private.CoreLib
    {"GlobalizationNative_LoadICU", GlobalizationNative_LoadICU}, // System.Private.CoreLib
    {"GlobalizationNative_NormalizeString", GlobalizationNative_NormalizeString}, // System.Private.CoreLib
    {"GlobalizationNative_StartsWith", GlobalizationNative_StartsWith}, // System.Private.CoreLib
    {"GlobalizationNative_ToAscii", GlobalizationNative_ToAscii}, // System.Private.CoreLib
    {"GlobalizationNative_ToUnicode", GlobalizationNative_ToUnicode}, // System.Private.CoreLib
    {NULL, NULL}
};

static void *pinvoke_tables[] = {
    (void*)libEGL_imports, (void*)libSystem_Native_imports, (void*)libSystem_IO_Compression_Native_imports, (void*)libSystem_Globalization_Native_imports
};

static char *pinvoke_names[] =  {
    "libEGL", "libSystem.Native", "libSystem.IO.Compression.Native", "libSystem.Globalization.Native"
};
#include <mono/utils/details/mono-error-types.h>
                #include <mono/metadata/assembly.h>
                #include <mono/utils/mono-error.h>
                #include <mono/metadata/object.h>
                #include <mono/utils/details/mono-logger-types.h>
                #include "runtime.h"
                InterpFtnDesc wasm_native_to_interp_ftndescs[7] = {};
typedef void (*WasmInterpEntrySig_0) (int*, int*, int*, int*, int*, int*, int*, int*);
int32_t wasm_native_to_interp_Internal_Runtime_InteropServices_System_Private_CoreLib_ComponentActivator_LoadAssemblyAndGetFunctionPointer (void * arg0, void * arg1, void * arg2, void * arg3, void * arg4, void * arg5) { 
  int32_t res;
  if (!(WasmInterpEntrySig_0)wasm_native_to_interp_ftndescs [0].func) {
   mono_wasm_marshal_get_managed_wrapper ("System.Private.CoreLib","Internal.Runtime.InteropServices", "ComponentActivator", "LoadAssemblyAndGetFunctionPointer", 6);
  }
  ((WasmInterpEntrySig_0)wasm_native_to_interp_ftndescs [0].func) ((int*)&res, (int*)&arg0, (int*)&arg1, (int*)&arg2, (int*)&arg3, (int*)&arg4, (int*)&arg5, wasm_native_to_interp_ftndescs [0].arg);
  return res;
}

typedef void (*WasmInterpEntrySig_1) (int*, int*, int*, int*, int*);
int32_t wasm_native_to_interp_Internal_Runtime_InteropServices_System_Private_CoreLib_ComponentActivator_LoadAssembly (void * arg0, void * arg1, void * arg2) { 
  int32_t res;
  if (!(WasmInterpEntrySig_1)wasm_native_to_interp_ftndescs [1].func) {
   mono_wasm_marshal_get_managed_wrapper ("System.Private.CoreLib","Internal.Runtime.InteropServices", "ComponentActivator", "LoadAssembly", 3);
  }
  ((WasmInterpEntrySig_1)wasm_native_to_interp_ftndescs [1].func) ((int*)&res, (int*)&arg0, (int*)&arg1, (int*)&arg2, wasm_native_to_interp_ftndescs [1].arg);
  return res;
}

typedef void (*WasmInterpEntrySig_2) (int*, int*, int*, int*, int*, int*, int*, int*);
int32_t wasm_native_to_interp_Internal_Runtime_InteropServices_System_Private_CoreLib_ComponentActivator_LoadAssemblyBytes (void * arg0, void * arg1, void * arg2, void * arg3, void * arg4, void * arg5) { 
  int32_t res;
  if (!(WasmInterpEntrySig_2)wasm_native_to_interp_ftndescs [2].func) {
   mono_wasm_marshal_get_managed_wrapper ("System.Private.CoreLib","Internal.Runtime.InteropServices", "ComponentActivator", "LoadAssemblyBytes", 6);
  }
  ((WasmInterpEntrySig_2)wasm_native_to_interp_ftndescs [2].func) ((int*)&res, (int*)&arg0, (int*)&arg1, (int*)&arg2, (int*)&arg3, (int*)&arg4, (int*)&arg5, wasm_native_to_interp_ftndescs [2].arg);
  return res;
}

typedef void (*WasmInterpEntrySig_3) (int*, int*, int*, int*, int*, int*, int*, int*);
int32_t wasm_native_to_interp_Internal_Runtime_InteropServices_System_Private_CoreLib_ComponentActivator_GetFunctionPointer (void * arg0, void * arg1, void * arg2, void * arg3, void * arg4, void * arg5) { 
  int32_t res;
  if (!(WasmInterpEntrySig_3)wasm_native_to_interp_ftndescs [3].func) {
   mono_wasm_marshal_get_managed_wrapper ("System.Private.CoreLib","Internal.Runtime.InteropServices", "ComponentActivator", "GetFunctionPointer", 6);
  }
  ((WasmInterpEntrySig_3)wasm_native_to_interp_ftndescs [3].func) ((int*)&res, (int*)&arg0, (int*)&arg1, (int*)&arg2, (int*)&arg3, (int*)&arg4, (int*)&arg5, wasm_native_to_interp_ftndescs [3].arg);
  return res;
}

typedef void (*WasmInterpEntrySig_4) (int*, int*, int*);
void wasm_native_to_interp_System_Globalization_System_Private_CoreLib_CalendarData_EnumCalendarInfoCallback (void * arg0, void * arg1) { 
  if (!(WasmInterpEntrySig_4)wasm_native_to_interp_ftndescs [4].func) {
   mono_wasm_marshal_get_managed_wrapper ("System.Private.CoreLib","System.Globalization", "CalendarData", "EnumCalendarInfoCallback", 2);
  }
  ((WasmInterpEntrySig_4)wasm_native_to_interp_ftndescs [4].func) ((int*)&arg0, (int*)&arg1, wasm_native_to_interp_ftndescs [4].arg);
}

typedef void (*WasmInterpEntrySig_5) (int*);
void wasm_native_to_interp_System_Threading_System_Private_CoreLib_ThreadPool_BackgroundJobHandler () { 
  if (!(WasmInterpEntrySig_5)wasm_native_to_interp_ftndescs [5].func) {
   mono_wasm_marshal_get_managed_wrapper ("System.Private.CoreLib","System.Threading", "ThreadPool", "BackgroundJobHandler", 0);
  }
  ((WasmInterpEntrySig_5)wasm_native_to_interp_ftndescs [5].func) (wasm_native_to_interp_ftndescs [5].arg);
}

typedef void (*WasmInterpEntrySig_6) (int*);
void wasm_native_to_interp_System_Threading_System_Private_CoreLib_TimerQueue_TimerHandler () { 
  if (!(WasmInterpEntrySig_6)wasm_native_to_interp_ftndescs [6].func) {
   mono_wasm_marshal_get_managed_wrapper ("System.Private.CoreLib","System.Threading", "TimerQueue", "TimerHandler", 0);
  }
  ((WasmInterpEntrySig_6)wasm_native_to_interp_ftndescs [6].func) (wasm_native_to_interp_ftndescs [6].arg);
}


static void *wasm_native_to_interp_funcs[] = {
    wasm_native_to_interp_Internal_Runtime_InteropServices_System_Private_CoreLib_ComponentActivator_LoadAssemblyAndGetFunctionPointer, wasm_native_to_interp_Internal_Runtime_InteropServices_System_Private_CoreLib_ComponentActivator_LoadAssembly, wasm_native_to_interp_Internal_Runtime_InteropServices_System_Private_CoreLib_ComponentActivator_LoadAssemblyBytes, wasm_native_to_interp_Internal_Runtime_InteropServices_System_Private_CoreLib_ComponentActivator_GetFunctionPointer, wasm_native_to_interp_System_Globalization_System_Private_CoreLib_CalendarData_EnumCalendarInfoCallback, wasm_native_to_interp_System_Threading_System_Private_CoreLib_ThreadPool_BackgroundJobHandler, wasm_native_to_interp_System_Threading_System_Private_CoreLib_TimerQueue_TimerHandler
};

// these strings need to match the keys generated in get_native_to_interp
static const char *wasm_native_to_interp_map[] = {
    "System_Private_CoreLib_ComponentActivator_LoadAssemblyAndGetFunctionPointer", "System_Private_CoreLib_ComponentActivator_LoadAssembly", "System_Private_CoreLib_ComponentActivator_LoadAssemblyBytes", "System_Private_CoreLib_ComponentActivator_GetFunctionPointer", "System_Private_CoreLib_CalendarData_EnumCalendarInfoCallback", "System_Private_CoreLib_ThreadPool_BackgroundJobHandler", "System_Private_CoreLib_TimerQueue_TimerHandler"
};
