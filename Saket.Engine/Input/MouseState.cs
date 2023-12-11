using Saket.Engine.GUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.Input;
/// <summary>
///     Specifies the buttons of a mouse.
/// </summary>
public enum MouseButton : int
{
    
    /// <summary>
    ///     The left mouse button. This corresponds to <see cref="Button1"/>.
    /// </summary>
    Left = 0,

    /// <summary>
    ///     The right mouse button. This corresponds to <see cref="Button2"/>.
    /// </summary>
    Right = 2,

    /// <summary>
    ///     The middle mouse button. This corresponds to <see cref="Button3"/>.
    /// </summary>
    Middle = 1,

}

public class MouseState
{
    private int btnCount = 8;

    private int _buttons;
    private int _buttonsPrevious;
    /// <summary>
    /// Gets a <see cref="Vector2"/> representing the absolute position of the pointer
    /// in the current frame, relative to the top-left corner of the contents of the window.
    /// </summary>
    public Vector2 Position { get; internal set; }

    /// <summary>
    /// Gets a <see cref="Vector2"/> representing the absolute position of the pointer
    /// in the previous frame, relative to the top-left corner of the contents of the window.
    /// </summary>
    public Vector2 PreviousPosition { get; internal set; }

    /// <summary>
    /// Gets a <see cref="Vector2"/> representing the amount that the mouse moved since the last frame.
    /// This does not necessarily correspond to pixels, for example in the case of raw input.
    /// </summary>
    public Vector2 Delta => Position - PreviousPosition;

    /// <summary>
    /// Get a Vector2 representing the position of the mouse wheel.
    /// </summary>
    public Vector2 Scroll { get; internal set; }

    /// <summary>
    /// Get a Vector2 representing the position of the mouse wheel.
    /// </summary>
    public Vector2 PreviousScroll { get; internal set; }

    /// <summary>
    /// Get a Vector2 representing the amount that the mouse wheel moved since the last frame.
    /// </summary>
    public Vector2 ScrollDelta => Scroll - PreviousScroll;

    /*
    /// <summary>
    /// Gets a <see cref="bool" /> indicating whether the specified
    ///  <see cref="MouseButton" /> is pressed.
    /// </summary>
    /// <param name="button">The <see cref="MouseButton" /> to check.</param>
    /// <returns><c>true</c> if key is pressed; <c>false</c> otherwise.</returns>
    public bool this[MouseButton button]
    {
        get { return ((_buttons >> (int)button) & 1) != 0; }
        internal set { _buttons |= 1 << (int)button; }
    }
    */
    /// <summary>
    /// Gets an integer representing the absolute x position of the pointer, in window pixel coordinates.
    /// </summary>
    public float X => Position.X;

    /// <summary>
    /// Gets an integer representing the absolute y position of the pointer, in window pixel coordinates.
    /// </summary>
    public float Y => Position.Y;

    /// <summary>
    /// Gets an integer representing the absolute x position of the pointer, in window pixel coordinates.
    /// </summary>
    public float PreviousX => PreviousPosition.X;

    /// <summary>
    /// Gets an integer representing the absolute y position of the pointer, in window pixel coordinates.
    /// </summary>
    public float PreviousY => PreviousPosition.Y;

    public void SetState(int data, float x, float y)
    {
        _buttonsPrevious = _buttons;
        _buttons = data;
  

        PreviousPosition = Position;
        Position = new Vector2(x, y);
    }



    #region 
    
    /// <summary>
    /// Gets a value indicating whether any button is down.
    /// </summary>
    /// <value><c>true</c> if any button is down; otherwise, <c>false</c>.</value>
    public bool IsAnyButtonDown()
    {
        return _buttons != 0;
    }
    
    /// <summary>
    /// Gets a <see cref="bool" /> indicating whether this button is down.
    /// </summary>
    /// <param name="button">The <see cref="MouseButton" /> to check.</param>
    /// <returns><c>true</c> if the <paramref name="button"/> is down, otherwise <c>false</c>.</returns>
    public bool IsButtonDown(MouseButton button)
    {
        return ((_buttons >> (int)button) & 1) != 0;
    }


    
    #endregion


}
