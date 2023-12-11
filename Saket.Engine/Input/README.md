A universal input handling library similar to Unity's new input system and Unreal's enchanced input.


HID Custom Source (Ex SDL) -> [ DeviceState -> Actions -> ] -> Action Reference

A way to directly read memory and convert it to devicestate?
Device Memory -> DeviceState


Features first priority
- automatically handle delta state. Store previous state to determine justpressed and justreleased.
- Independent of input source. State is to be injected into the system.
- As generic as possible to support a wide array of input devices, while having common implementations like Keyboard, Mouse and Gamepad.
- Easy and straightforward User end API
- Rumble support. Two way communcation.

Fetures secondary priority
- Advanced pointer support / Touch Support
	- Tracking of common gestures
- Handle Double/triple click, Timing (Time Held, Time Released)
- Stylus support 

Inspiration:
Monogame, openTK,
Unity Input:
https://docs.unity3d.com/Packages/com.unity.inputsystem@1.8/manual/index.html