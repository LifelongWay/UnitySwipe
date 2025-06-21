# Swipe Panel Usage Guide

## 🚀 Creating a Swipe Panel (VerticalSwipe / HorizontalSwipe)

1. Create a new `GameObject` in your scene.
2. Attach the `VerticalScroll` (or `HorizontalScroll`) script to this GameObject.

---

## 🔗 Attaching Elements to the Swipe Panel

1. Create the content objects you want to swipe through.  
   > These can be placed anywhere in the scene — the script will handle their position and hierarchy.
2. In the Inspector for the swipe panel GameObject, locate the **"Buffer List"**.
3. Add your content objects to the **Buffer List**.

---

## 📝 Notes

- The swipe panel is designed for **full-screen-sized objects**.  
  > Resizing is handled programmatically in the script.
- All panel items must have a default scale of **`(1, 1, 1)`**.

---

## ⚠️ Important

- The scripts are written **without Canvas Scaler**.  
  > Behavior with a Canvas Scaler is **currently untested**.
