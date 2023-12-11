using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.Input
{
    public class KeyboardState
    {
        public byte[] pressedKeys { get; protected set; }

        public byte[] lastKeys { get; protected set; }

        public readonly int KeyCount;

        public KeyboardState(int count)
        {
            KeyCount = count;
            pressedKeys = new byte[count];  
            lastKeys  = new byte[count];
        }
        
        public void SetKeyboardState(Span<byte> state)
        {
            // Save the current state as the old state
            pressedKeys.CopyTo(lastKeys.AsSpan());
            // Set the new state
            state.CopyTo(pressedKeys.AsSpan());
        }

        public bool IsKeyDown(Keys keyCode)
        {
            return pressedKeys[(int)keyCode] == 1;
        }
        public bool IsKeyDown(int keyCode)
        {
            return pressedKeys[keyCode] == 1;
        }

        public ButtonState GetButtonState(int index)
        {
            if (lastKeys[index] == 0)
            {
                if (pressedKeys[index] != 0)
                    return ButtonState.JustPressed;
                if (pressedKeys[index] == 0)
                    return ButtonState.Released;
            }
            else
            {
                if (pressedKeys[index] != 0)
                    return ButtonState.Pressed;
                if (pressedKeys[index] == 0)
                    return ButtonState.JustReleased;
            }
            // Should never hit this
            return ButtonState.Released;
        }
    }
}
