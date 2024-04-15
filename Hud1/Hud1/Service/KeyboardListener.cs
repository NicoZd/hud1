using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Hud1
{
    public class KeyboardListener : IDisposable
    {
        private readonly Thread keyboardThread;

        //Here you can put those keys that you want to capture
        private readonly List<KeyState> numericKeys = new List<KeyState>
        {
            new KeyState(Key.F2),
            new KeyState(Key.Up),
            new KeyState(Key.Down),
            new KeyState(Key.Left),
            new KeyState(Key.Right),
        };

        private bool isRunning = true;

        public KeyboardListener()
        {
            keyboardThread = new Thread(StartKeyboardListener) { IsBackground = true };
            keyboardThread.Start();
        }

        private void StartKeyboardListener()
        {
            while (isRunning)
            {
                Thread.Sleep(15);
                if (Application.Current != null)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (Application.Current.Windows.Count > 0)
                        {
                            foreach (var keyState in numericKeys)
                            {
                                if (Keyboard.IsKeyDown(keyState.Key) && !keyState.IsPressed) //
                                {
                                    keyState.IsPressed = true;
                                    KeyboardDownEvent?.Invoke(null, new KeyEventArgs(Keyboard.PrimaryDevice, PresentationSource.FromDependencyObject(Application.Current.Windows[0]), 0, keyState.Key));
                                }

                                if (Keyboard.IsKeyUp(keyState.Key))
                                {
                                    keyState.IsPressed = false;
                                }
                            }
                        }
                    });
                }
            }
        }

        public event KeyEventHandler KeyboardDownEvent;

        private class KeyState
        {
            public KeyState(Key key)
            {
                this.Key = key;
            }

            public Key Key { get; }
            public bool IsPressed { get; set; }
        }

        public void Dispose()
        {
            isRunning = false;
            Task.Run(() =>
            {
                if (keyboardThread != null && !keyboardThread.Join(1000))
                {
                    keyboardThread.Abort();
                }
            });
        }
    }
}
