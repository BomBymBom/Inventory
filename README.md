# Inventory
 
![Gif_1](https://github.com/user-attachments/assets/317a4d17-61e1-4567-8c3b-39db859d6f16)





![Gif_2](https://github.com/user-attachments/assets/10db0599-fe5f-495b-b092-a46f8f2015e7)





![Gif_3](https://github.com/user-attachments/assets/6dcda8ab-4fd9-477b-b54d-b7abd86c5d9c)





![Gif_4](https://github.com/user-attachments/assets/f4eaedce-e2f2-4e13-b73e-3de370bba663)





*** Instructions for Running the Project ***
Clone this repository to your local machine:

git clone https://github.com/user/repo-name.git

Open the project in Unity (recommended version: Unity 2022.3.46f).

Navigate to the main scene located at:
Assets/_Assets/Scenes/MainScene.unity

Click the Play button in the Unity Editor to start testing the project.


*** Basic Controls ***
- W/A/S/D - Move the character.
- I - Open inventory.
- Left Mouse Button (Click) - Select objects.
- Middle Mouse Button (Click) - Open the configuration window for an item.
- Right Mouse Button (Click) - Use the selected item.
- Drag & Drop - Move items between inventory and equipment.
- E - Pick up items from the ground.



*** Used Assets ***

1. **UI**  
   Link: https://assetstore.unity.com/packages/2d/gui/icons/ui-button-and-hallowing-element-pack-5-143157

2. **Keyboard Icons**  
   Link: https://assetstore.unity.com/packages/2d/gui/keyboard-keys-mouse-sprites-225232

3. **3D Character**  
   Link: https://assetstore.unity.com/packages/3d/characters/humanoids/fantasy/mini-simple-characters-skeleton-free-demo-262897

4. **3D Potion**  
   Link: https://assetstore.unity.com/packages/3d/props/potions-coin-and-box-of-pandora-pack-71778

5. **Controller**  
   Link: https://assetstore.unity.com/packages/essentials/starter-assets-thirdperson-updates-in-new-charactercontroller-pa-196526




*** Used Patterns ***

1. **Factory Pattern**  
   - Script: `ItemFactory`.  
   - Purpose: Used to create instances of `Item` based on data from `ItemData`.

2. **Strategy Pattern**  
   - Script: `ItemUseData`.  
   - Purpose: Manages the specific logic for using different types of items through separate classes implementing `ItemUseData`.

3. **Observer Pattern**  
   - Scripts: `Inventory`, `EquipmentSystem`.  
   - Purpose: Notifies the UI when the inventory or equipment changes.

4. **Singleton Pattern**  
   - Script: `GameManager`.  
   - Purpose: Provides access to shared resources such as `PlayerInventory` and `PlayerEquipment`.
