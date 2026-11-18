using UnityEngine;
using UnityEngine.InputSystem;

// Reads the mouse or a finger through the new Input System (Pointer covers both).
// Only call these from Update, never FixedUpdate.
public static class PointerInput
{
    // True on the frame the finger / mouse button goes down.
    public static bool PressedThisFrame =>
        Pointer.current != null && Pointer.current.press.wasPressedThisFrame;

    // True every frame the finger / mouse button is held.
    public static bool IsHeld =>
        Pointer.current != null && Pointer.current.press.isPressed;

    // Screen X position of the pointer in pixels.
    public static float ScreenX =>
        Pointer.current != null ? Pointer.current.position.ReadValue().x : 0f;
}
