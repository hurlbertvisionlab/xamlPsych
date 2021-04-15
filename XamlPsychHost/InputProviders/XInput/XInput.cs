using System;
using System.Runtime.InteropServices;

public static class XInput
{
    /// <summary>
    /// Specifies keystroke data returned by <see cref="XInputGetKeystroke" />.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Keystroke
    {
        /// <summary>
        /// Virtual-key code of the key, button, or stick movement.
        /// </summary>
        public VirtualKey VirtualKey;

        /// <summary>
        /// This member is unused and the value is zero.
        /// </summary>
        public char Unicode;

        /// <summary>
        /// Flags that indicate the keyboard state at the time of the input event.
        /// </summary>
        public KeystrokeFlags Flags;

        /// <summary>
        /// Index of the signed-in gamer associated with the device.
        /// </summary>
        public byte UserIndex;

        /// <summary>
        /// HID code corresponding to the input.
        /// If there is no corresponding HID code, this value is zero.
        /// </summary>
        public byte HidCode;
    }

    /// <summary>
    /// Flags used in <see cref="Keystroke"/>
    /// </summary>
    [Flags]
    public enum KeystrokeFlags : ushort
    {
        /// <summary>
        /// The key was pressed.
        /// </summary>
        KeyDown = 0x0001,

        /// <summary>
        /// The key was released.
        /// </summary>
        KeyUp = 0x0002,

        /// <summary>
        /// A repeat of a held key.
        /// </summary>
        KeyRepeat = 0x0004
    }

    /// <summary>
    /// Codes returned for the gamepad keystroke
    /// </summary>
    public enum VirtualKey : ushort
    {
        /// <summary>
        /// A button.
        /// </summary>
        GamepadA = 0x5800,
        
        /// <summary>
        /// B button.
        /// </summary>
        GamepadB = 0x5801,
        
        /// <summary>
        /// X button.
        /// </summary>
        GamepadX = 0x5802,
        
        /// <summary>
        /// Y button.
        /// </summary>
        GamepadY = 0x5803,

        /// <summary>
        /// Right shoulder button.
        /// </summary>
        GamepadRightShoulder = 0x5804,

        /// <summary>
        /// Left should button.
        /// </summary>
        GamepadLeftShoulder = 0x5805,

        /// <summary>
        /// Left trigger.
        /// </summary>
        GamepadLeftTrigger = 0x5806,

        /// <summary>
        /// Right trigger.
        /// </summary>
        GamepadRightTrigger = 0x5807,

        /// <summary>
        /// Directional pad up.
        /// </summary>
        GamepadDirectionalPadUp = 0x5810,

        /// <summary>
        /// Directional pad down.
        /// </summary>
        GamepadDirectionalPadDown = 0x5811,

        /// <summary>
        /// Directiona pad left.
        /// </summary>
        GamepadDirectionalPadLeft = 0x5812,

        /// <summary>
        /// Directional pad right.
        /// </summary>
        GamepadDirectionalPadRight = 0x5813,

        /// <summary>
        /// START button.
        /// </summary>
        GamepadStart = 0x5814,

        /// <summary>
        /// BACK button.
        /// </summary>
        GamepadBack = 0x5815,

        /// <summary>
        /// Left thumbstick click.
        /// </summary>
        GamepadLeftThumbPress = 0x5816,

        /// <summary>
        /// Right thumbstick click.
        /// </summary>
        GamepadRightThumbPress = 0x5817,

        /// <summary>
        /// Left thumbstick up.
        /// </summary>
        GamepadLeftThumbUp = 0x5820,

        /// <summary>
        /// Left thumbstick down.
        /// </summary>
        GamepadLeftThumbDown = 0x5821,

        /// <summary>
        /// Left thumbstick right.
        /// </summary>
        GamepadLeftThumbRight = 0x5822,

        /// <summary>
        /// Left thumbstick left.
        /// </summary>
        GamepadLeftThumbLeft = 0x5823,

        /// <summary>
        /// Left thumbstick up and left.
        /// </summary>
        GamepadLeftThumbUpLeft = 0x5824,

        /// <summary>
        /// Left thumbstick up and right.
        /// </summary>
        GamepadLeftThumbUpRight = 0x5825,

        /// <summary>
        /// Left thumbstick down and right.
        /// </summary>
        GamepadLeftThumbDownRight = 0x5826,

        /// <summary>
        /// Left thumbstick down and left.
        /// </summary>
        GamepadLeftThumbDownLeft = 0x5827,

        /// <summary>
        /// Right thumbstick up.
        /// </summary>
        GamepadRightThumbUp = 0x5830,

        /// <summary>
        /// Right thumbstick down.
        /// </summary>
        GamepadRightThumbDown = 0x5831,

        /// <summary>
        /// Right thumbstick right.
        /// </summary>
        GamepadRightThumbRight = 0x5832,

        /// <summary>
        /// Right thumbstick left.
        /// </summary>
        GamepadRightThumbLeft = 0x5833,

        /// <summary>
        /// Right thumbstick up and left.
        /// </summary>
        GamepadRightThumbUpLeft = 0x5834,

        /// <summary>
        /// Right thumbstick up and right.
        /// </summary>
        GamepadRightThumbUpRight = 0x5835,

        /// <summary>
        /// Right thumbstick down and right.
        /// </summary>
        GamepadRightThumbDownRight = 0x5836,

        /// <summary>
        /// Right thumbstick down and left.
        /// </summary>
        GamepadRightThumbDownLeft = 0x5837,
    }

    /// <summary>
    /// Represents the state of a controller.
    /// </summary>
    /// <remarks>
    /// The <see cref="PacketNumber"/> member is incremented only if the status of the controller has changed since the controller was last polled.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    public struct State
    {
        /// <summary>
        /// State packet number. 
        /// The packet number indicates whether there have been any changes in the state of the controller.
        /// If the <see cref="PacketNumber"/> member is the same in sequentially returned <see cref="State"/> structures, the controller state has not changed.
        /// </summary>
        public uint PacketNumber;

        /// <summary>
        /// <see cref="Gamepad"/> structure containing the current state of an Xbox 360 Controller.
        /// </summary>
        public Gamepad Gamepad;
    }

    /// <summary>
    /// Describes the capabilities of a connected controller.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Capabilities
    {
        /// <summary>
        /// Controller type. 
        /// </summary>
        public DeviceType Type;
        /// <summary>
        /// 
        /// </summary>
        public DeviceSubType SubType;

        /// <summary>
        /// 
        /// </summary>
        public Capability Flags;

        /// <summary>
        /// 
        /// </summary>
        public Gamepad Gamepad;

        /// <summary>
        /// 
        /// </summary>
        public Vibration Vibration;
    }

    /// <summary>
    /// Describes the current state of the Xbox 360 Controller.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Gamepad
    {
        /// <summary>
        /// Bitmask of the device digital buttons.
        /// A set bit indicates that the corresponding button is pressed.
        /// Bits that are set but not defined are reserved, and their state is undefined.
        /// </summary>
        public GamepadButtons Buttons;

        /// <summary>
        /// The current value of the left trigger analog control.
        /// The value is between 0 and 255.
        /// </summary>
        public byte LeftTrigger;

        /// <summary>
        /// The current value of the right trigger analog control.
        /// The value is between 0 and 255.
        /// </summary>
        public byte RightTrigger;

        /// <summary>
        /// Left thumbstick x-axis value. The value is between -32768 and 32767.
        /// </summary>
        public short ThumbLeftX;

        /// <summary>
        /// Left thumbstick y-axis value. The value is between -32768 and 32767.
        /// </summary>
        public short ThumbLeftY;

        /// <summary>
        /// Right thumbstick x-axis value. The value is between -32768 and 32767.
        /// </summary>
        public short ThumbRightX;

        /// <summary>
        /// Right thumbstick y-axis value. The value is between -32768 and 32767.
        /// </summary>
        public short ThumbRightY;

        private const uint GamePadLeftThumbDeadzoneSquared = (uint)GamePadLeftThumbDeadzone * GamePadLeftThumbDeadzone;
        private const uint GamePadRightThumbDeadzoneSquared = (uint)GamePadRightThumbDeadzone * GamePadRightThumbDeadzone;
        public bool IsThumbLeftAlive => ThumbLeftX * ThumbLeftX + ThumbLeftY * ThumbLeftY > GamePadLeftThumbDeadzoneSquared;
        public bool IsThumbRightAlive => ThumbRightX * ThumbRightX + ThumbRightY * ThumbRightY > GamePadLeftThumbDeadzoneSquared;

        public bool GetThumbRight(out double x, out double y, int power = 1)
        {
            return GetThumb(ThumbRightX, ThumbRightY, GamePadRightThumbDeadzone, power, out x, out y);
        }
        public bool GetThumbLeft(out double x, out double y, int power = 1)
        {
            return GetThumb(ThumbLeftX, ThumbLeftY, GamePadLeftThumbDeadzone, power, out x, out y);
        }
        private bool GetThumb(short rawX, short rawY, ushort deadZone, int power, out double x, out double y)
        {
            x = 0.0;
            y = 0.0;

            double magnitude = Math.Sqrt(rawX * rawX + rawY * rawY);

            if (magnitude < deadZone)
                return false;

            double normalizedX = rawX / magnitude;
            double normalizedY = rawY / magnitude;

            if (magnitude > short.MaxValue)
                magnitude = short.MaxValue;

            magnitude -= deadZone;

            double normalizedMagnitude = magnitude / (short.MaxValue - deadZone);

            if (power != 1)
                normalizedMagnitude = Math.Pow(normalizedMagnitude, power);

            x = normalizedX * normalizedMagnitude;
            y = normalizedY * normalizedMagnitude;
            return true;
        }
    }

    public enum DeviceType : byte
    {
        /// <summary>
        /// The device is a game controller. 
        /// </summary>
        Gamepad = 0x01
    }

    /// <summary>
    /// Subtype of the game controller.
    /// </summary>
    public enum DeviceSubType : byte
    {
        Unknown = 0x00,
        GamePad = 0x01,
        Wheel = 0x02,
        ArcadeStick = 0x03,
        FlightStick = 0x04,
        DancePad = 0x05,
        Guitar = 0x06,
        GuitarAlternate = 0x07,
        DrumKit = 0x08,
        GuitarBass = 0x0B,
        ArcadePad = 0x13
    }

    [Flags]
    public enum Capability : ushort
    {
        None = 0x0000,

        /// <summary>
        /// Device supports force feedback functionality.
        /// </summary>
        /// <remarks>
        /// Note that these force-feedback features beyond rumble are not currently supported through XINPUT on Windows.
        /// </remarks>
        ForceFeedbackSupported = 0x0001,

        /// <summary>
        /// Device is wireless.
        /// </summary>
        Wireless = 0x0002,

        /// <summary>
        /// Device has an integrated voice device.
        /// </summary>
        VoiceSupported = 0x0004,

        /// <summary>
        /// Device supports plug-in modules. 
        /// </summary>
        /// <remarks>
        /// Note that plug-in modules like the text input device (TID)are not supported currently through XINPUT on Windows.
        /// </remarks>
        PluginModulesSupported = 0x0008,

        /// <summary>
        /// Device lacks menu navigation buttons (START, BACK, DPAD).
        /// </summary>
        NoNavigation = 0x0010,
    }

    [Flags]
    public enum GamepadButtons : ushort
    {
        DirectionalPadUp = 0x0001,
        DirectionalPadDown = 0x0002,
        DirectionalPadLeft = 0x0004,
        DirectionalPadRight = 0x0008,
        Start = 0x0010,
        Back = 0x0020,
        LeftThumb = 0x0040,
        RightThumb = 0x0080,
        LeftShoulder = 0x0100,
        RightShoulder = 0x0200,
        A = 0x1000,
        B = 0x2000,
        X = 0x4000,
        Y = 0x8000,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct BatteryInformation
    {
        /// <summary>
        /// The type of battery.
        /// </summary>
        public BatteryType BatteryType;

        /// <summary>
        /// The charge state of the battery.
        /// This value is only valid for wireless devices with a known battery type.
        /// </summary>
        public BatteryLevel BatteryLevel;
    }

    /// <summary>
    /// The type of battery.
    /// </summary>
    public enum BatteryType : byte
    {
        /// <summary>
        /// This device is not connected.
        /// </summary>
        Disconnected = 0x00,

        /// <summary>
        /// The device is a wired device and does not have a battery.
        /// </summary>
        Wired = 0x01,

        /// <summary>
        /// The device has an alkaline battery.
        /// </summary>
        Alkaline = 0x02,

        /// <summary>
        /// The device has a nickel metal hydride battery.
        /// </summary>
        NiMH = 0x03,

        /// <summary>
        /// The device has an unknown battery type.
        /// </summary>
        Unknown = 0xFF
    }

    /// <summary>
    /// The charge state of the battery.
    /// </summary>
    public enum BatteryLevel : byte
    {
        /// <summary>
        /// Empty.
        /// </summary>
        Empty = 0x00,

        /// <summary>
        /// Low.
        /// </summary>
        Low = 0x01,

        /// <summary>
        /// Medium.
        /// </summary>
        Medium = 0x02,

        /// <summary>
        /// Full.
        /// </summary>
        Full = 0x03
    }

    /// <summary>
    /// Specifies motor speed levels for the vibration function of a controller.
    /// </summary>
    /// <remarks>
    /// The left motor is the low-frequency rumble motor.
    /// The right motor is the high-frequency rumble motor.
    /// The two motors are not the same, and they create different vibration effects.
    /// </remarks>
    public struct Vibration
    {
        /// <summary>
        /// Speed of the left motor.
        /// Valid values are in the range 0 to 65,535.
        /// Zero signifies no motor use; 65,535 signifies 100 percent motor use.
        /// </summary>
        public ushort LeftMotorSpeed;

        /// <summary>
        /// Speed of the right motor.
        /// Valid values are in the range 0 to 65,535.
        /// Zero signifies no motor use; 65,535 signifies 100 percent motor use.
        /// </summary>
        public ushort RightMotorSpeed;
    }

    /// <summary>
    /// Input flags that identify the controller type.
    /// </summary>
    [Flags]
    public enum CapabilitiesFlags : uint
    {
        /// <summary>
        /// The capabilities of all controllers connected to the system are returned.
        /// </summary>
        AllControllers = 0x00000000,

        /// <summary>
        /// Limit query to devices of Xbox 360 Controller type.
        /// </summary>
        Gamepad = 0x00000001
    }

    public const ushort GamePadLeftThumbDeadzone = 7849;
    public const ushort GamePadRightThumbDeadzone = 8689;
    public const ushort GamePadTriggerThreshold = 30;

    public const uint ErrorDeviceNotConnected = 1167;

    public const int MaximumUserCount = 4;
    public const int AnyUser = 0x000000FF;

    /// <summary>
    /// Retrieves the current state of the specified controller.
    /// </summary>
    /// <param name="userIndex">Index of the user's controller. Can be a value from 0 to 3.</param>
    /// <param name="state">A <see cref="State"/> structure that receives the current state of the controller.</param>
    /// <returns>
    /// If the function succeeds, the return value is ERROR_SUCCESS. 
    /// If the controller is not connected, the return value is ERROR_DEVICE_NOT_CONNECTED. 
    /// If the function fails, the return value is an error code defined in WinError.h.
    /// </returns>
    [DllImport("xinput1_4.dll", EntryPoint = "XInputGetState")]
    public static extern uint GetState(int userIndex, ref State state);

    /// <summary>
    /// Sends data to a connected controller.
    /// This function is used to activate the vibration function of a controller.
    /// </summary>
    /// <param name="userIndex">Index of the user's controller. Can be a value from 0 to 3.</param>
    /// <param name="vibration">A <see cref="Vibration"/> structure containing the vibration information to send to the controller.</param>
    /// <returns>
    /// If the function succeeds, the return value is ERROR_SUCCESS. 
    /// If the controller is not connected, the return value is ERROR_DEVICE_NOT_CONNECTED. 
    /// If the function fails, the return value is an error code defined in WinError.h.
    /// </returns>
    [DllImport("xinput1_4.dll", EntryPoint = "XInputSetState")]
    public static extern uint SetState(int userIndex, in Vibration vibration);


    /// <summary>
    /// Retrieves the capabilities and features of a connected controller.
    /// </summary>
    /// <param name="userIndex">Index of the user's controller. Can be a value in the range 0–3.</param>
    /// <param name="flags">Input flags that identify the controller type. If this value is 0, then the capabilities of all controllers connected to the system are returned.</param>
    /// <param name="capabilities">A <see cref="Capabilities"/> structure that receives the controller capabilities.</param>
    /// <returns>
    /// If the function succeeds, the return value is ERROR_SUCCESS. 
    /// If the controller is not connected, the return value is ERROR_DEVICE_NOT_CONNECTED. 
    /// If the function fails, the return value is an error code defined in WinError.h.
    /// </returns>
    [DllImport("xinput1_4.dll", EntryPoint = "XInputGetCapabilities")]
    public static extern uint GetCapabilities(int userIndex, CapabilitiesFlags flags, out Capabilities capabilities);

    /// <summary>
    /// Sets the reporting state of XInput.
    /// </summary>
    /// <remarks>
    /// This function is meant to be called when an application gains or loses focus (such as via WM_ACTIVATEAPP).
    /// Using this function, you will not have to change the XInput query loop in your application as neutral data will always be reported if XInput is disabled.
    /// In a controller that supports vibration effects:
    ///  • Passing <see langword="false"/> will stop any vibration effects currently playing.
    ///    In this state, calls to XInputSetState will be registered, but not passed to the device.
    ///  • Passing <see langword="true"/> will pass the last vibration request (even if it is 0) sent to XInputSetState to the device.
    /// </remarks>
    /// <param name="enable">If enable is <see langword="false"/>, XInput will only send neutral data in response to <see cref="XInputGetState"/> (all buttons up, axes centered, and triggers at 0). XInputSetState calls will be registered but not sent to the device. Sending any value other than <see langword="false"/> will restore reading and writing functionality to normal.</param>
    [DllImport("xinput1_4.dll", EntryPoint = "XInputEnable")]
    public static extern void Enable(bool enable);

    /// <summary>
    /// Retrieves the sound rendering and sound capture audio device IDs that are associated with the headset connected to the specified controller.
    /// </summary>
    /// <param name="userIndex">Index of the gamer associated with the device.</param>
    /// <param name="renderDeviceId">Windows Core Audio device ID string for render (speakers).</param>
    /// <param name="renderCount">Size, in wide-chars, of the render device ID string buffer.</param>
    /// <param name="captureDeviceId">Windows Core Audio device ID string for capture (microphone).</param>
    /// <param name="captureCount">Size, in wide-chars, of capture device ID string buffer.</param>
    /// <returns>
    /// If the function successfully retrieves the device IDs for render and capture, the return code is ERROR_SUCCESS.
    /// If there is no headset connected to the controller, the function will also retrieve ERROR_SUCCESS with <see langword="null"/> as the values for <paramref name="renderDeviceId"/> and <paramref name="captureDeviceId"/>.
    /// If the controller port device is not physically connected, the function will return ERROR_DEVICE_NOT_CONNECTED.
    /// If the function fails, it will return a valid Win32 error code.
    /// </returns>
    [DllImport("xinput1_4.dll", EntryPoint = "XInputGetAudioDeviceIds")]
    public static extern uint GetAudioDeviceIds(int userIndex, [MarshalAs(UnmanagedType.LPWStr)] out string renderDeviceId, out uint renderCount, [MarshalAs(UnmanagedType.LPWStr)] out string captureDeviceId, out uint captureCount);


    /// <summary>
    /// Specifies which device associated with this user index should be queried.
    /// </summary>
    /// <param name="userIndex">Index of the signed-in gamer associated with the device. Can be a value in the range 0–<see cref="MaximumUserCount"/> − 1.</param>
    /// <param name="deviceType">Specifies which device associated with this user index should be queried.</param>
    /// <param name="batteryInformation">An <see cref="BatteryInformation"/> structure that receives the battery information.</param>
    /// <returns>If the function succeeds, the return value is ERROR_SUCCESS.</returns>
    [DllImport("xinput1_4.dll", EntryPoint = "XInputGetBatteryInformation")]
    public static extern uint GetBatteryInformation(int userIndex, BatteryDeviceType deviceType, out BatteryInformation batteryInformation);

    /// <summary>
    /// Devices that support batteries.
    /// </summary>
    public enum BatteryDeviceType : byte
    {
        /// <summary>
        /// GAmepad.
        /// </summary>
        Gamepad = 0x00,

        /// <summary>
        /// Headset.
        /// </summary>
        Headset = 0x01
    }

    /// <summary>
    /// Retrieves a gamepad input event.
    /// </summary>
    /// <param name="userIndex">Index of the signed-in gamer associated with the device. Can be a value in the range 0–<see cref="MaximumUserCount"/> − 1, or <see cref="AnyUser"/> to fetch the next available input event from any user.</param>
    /// <param name="reserved">Reserved.</param>
    /// <param name="keystroke">An <see cref="Keystroke"/> structure that receives an input event.</param>
    /// <returns>
    /// If the function succeeds, the return value is ERROR_SUCCESS.
    /// If no new keys have been pressed, the return value is ERROR_EMPTY.
    /// If the controller is not connected or the user has not activated it, the return value is ERROR_DEVICE_NOT_CONNECTED.
    /// If the function fails, the return value is an error code defined in Winerror.h.
    /// </returns>
    [DllImport("xinput1_4.dll", EntryPoint = "XInputGetKeystroke")]
    public static extern uint GetKeystroke(int userIndex, int reserved, ref Keystroke keystroke);

    /// <summary>
    /// Gets the sound rendering and sound capture device GUIDs that are associated with the headset connected to the specified controller.
    /// </summary>
    /// <param name="userIndex">Index of the user's controller. It can be a value in the range 0–3.</param>
    /// <param name="renderGuid">The GUID of the headset sound rendering device.</param>
    /// <param name="captureGuid">The GUID of the headset sound capture device.</param>
    /// <returns>
    /// If the function successfully retrieves the device IDs for render and capture, the return code is ERROR_SUCCESS.
    /// If there is no headset connected to the controller, the function also retrieves ERROR_SUCCESS with GUID_NULL as the values for <paramref name="dsoundRenderGuid"/> and <paramref name="dsoundCaptureGuid"/>.
    /// If the controller port device is not physically connected, the function returns ERROR_DEVICE_NOT_CONNECTED.
    /// If the function fails, it returns a valid Win32 error code.
    /// </returns>
    [Obsolete("XInputGetDSoundAudioDeviceGuids is deprecated because it isn't supported by Windows 8 (XInput 1.4).")]
    [DllImport("xinput9_1_0.dll", EntryPoint = "XInputGetDSoundAudioDeviceGuids")]
    public static extern uint GetDSoundAudioDeviceGuids(int userIndex, out Guid dsoundRenderGuid, out Guid dsoundCaptureGuid);
}