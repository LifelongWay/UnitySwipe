####   Instructions on using VerticalSwipe/HorizontalSwipe panel   ###

# Instructions for Creating Swipe Panel

1. Create a new GameObject.
2. Attach the VerticalScroll script to the GameObject.

# Instructions for Attaching Elements to created Swipe Panel

1. Create your objects (they can be placed anywhere in the scene; the script will handle their position and hierarchy).
2. Add the created objects to the "Buffer List" of GameObjects in the VerticalSwipe/HorizontalSwipe panel Inspector.

# Notes:
    - This swipe panel is designed for full-screen size objects. (Resizing is handled in the script).
    - All panel items should have a default scale of (1, 1, 1).

# Important Note:
    - Scripts are written WITHOUT CANVAS SCALER, otherwise behavior in currently not tested.
