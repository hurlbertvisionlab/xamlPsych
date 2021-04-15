//
// Copyright © 2016 miloush.net. All rights reserved.
//

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

public static class PowerManagement
{
    private struct RequestContext
    {
        public uint Version;
        public uint Flags;

        [MarshalAs(UnmanagedType.LPWStr)]
        public string SimpleReason;
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr PowerCreateRequest(ref RequestContext pReasonContext);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool PowerSetRequest(IntPtr hPowerRequest, PowerRequestType requestType);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool PowerClearRequest(IntPtr hPowerRequest, PowerRequestType requestType);

    [DllImport("kernel32.dll", SetLastError = true)]
    internal static extern bool CloseHandle(IntPtr handle);

    public static PowerRequest Request(PowerRequestType requestType, string reason)
    {        
        RequestContext context = new RequestContext
        {
            Version = 0,
            Flags = 1,
            SimpleReason = reason
        };

        IntPtr hRequest = PowerCreateRequest(ref context);
        if (hRequest == new IntPtr(-1))
            throw new Win32Exception();

        if (!PowerSetRequest(hRequest, requestType))
            throw new Win32Exception();

        return new PowerRequest(hRequest, requestType, reason);
    }

    public static void CancelRequest(PowerRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        PowerClearRequest(request.Handle, request.RequestType);
        CloseHandle(request.Handle);
    }
}

// this is deliberately not a safe handle so that consumers do not have to keep a reference around
public class PowerRequest
{
    internal readonly IntPtr Handle;

    private readonly PowerRequestType _requestType;
    private readonly string _reason;

    public PowerRequestType RequestType => _requestType;
    public string Reason => _reason;

    internal PowerRequest(IntPtr handle, PowerRequestType requestType, string reason)
    {
        _requestType = requestType;
        _reason = reason;

        Handle = handle;
    }
}

public enum PowerRequestType
{
    /// <summary>
    /// The display remains on even if there is no user input for an extended period of time.
    /// </summary>
    DisplayRequired = 0,
    /// <summary>
    /// The system continues to run instead of entering sleep after a period of user inactivity. 
    /// This request type is not honored on systems capable of connected standby. Applications should use ExecutionRequired requests instead.
    /// </summary>
    SystemRequired = 1,
    /// <summary>
    /// The system enters away mode instead of sleep in response to explicit action by the user.
    /// In away mode, the system continues to run but turns off audio and video to give the appearance of sleep.
    /// </summary>
    AwayModeRequired = 2,
    /// <summary>
    /// The calling process continues to run instead of being suspended or terminated by process lifetime management mechanisms. 
    /// When and how long the process is allowed to run depends on the operating system and power policy settings. 
    /// On systems not capable of connected standby, an active ExecutionRequired request implies SystemRequired.
    /// </summary>
    ExecutionRequired = 3
}
