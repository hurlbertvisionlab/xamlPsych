using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Input;
using System.Windows.Markup;

namespace HurlbertVisionLab.XamlPsychHost
{
    // TODO: support repeat and timeout in GamepadMap

    [ContentProperty(nameof(Mappings))]
    public class GamepadInputProvider : EnumInputProvider<XInput.VirtualKey, MapGamepad>
    {
        private XInput.Keystroke _keyStroke;

        private Thread _thread;
        private volatile bool _continue;

        protected override void Bind()
        {
            _continue = true;
            _thread = new Thread(XInputThread);
            _thread.Name = "XInput Thread";
            _thread.IsBackground = true;
            _thread.Start();
        }

        private void XInputThread()
        {
            while (_continue)
            {
                while (XInput.GetKeystroke(0, 0, ref _keyStroke) == 0)
                {
                    // Log("XKeystroke", _keyStroke.VirtualKey, _keyStroke.Flags);

                    if ((_keyStroke.Flags & XInput.KeystrokeFlags.KeyDown) != 0)
                        OnGameKeyDown(_keyStroke.VirtualKey, (_keyStroke.Flags & XInput.KeystrokeFlags.KeyRepeat) != 0);

                    if ((_keyStroke.Flags & XInput.KeystrokeFlags.KeyUp) != 0)
                        OnGameKeyUp(_keyStroke.VirtualKey);
                }

                Thread.Sleep(100);
            }
        }

        private void OnGameKeyDown(XInput.VirtualKey key, bool repeat)
        {
            ReportInput(key);
        }

        private void OnGameKeyUp(XInput.VirtualKey key)
        {

        }

        public override void Unbind()
        {       
            _continue = false;
        }

        protected override MapGamepad CreateMap(MapInput map)
        {
            return new MapGamepad { Key = MapGamepad.GetKey(map), ToInput = map.ToInput };
        }
    }
}
