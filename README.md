<h1 align="center">
    Outward Enchantments Viewer
</h1>
<br/>
<div align="center">
  <img src="./preview/images/1.png" alt="Outward added item description."/>
  <img src="./preview/images/3.png" alt="Outward added item description."/>
</div>

Outward mod that provides additional descriptions for items and enchantments.

## Why use this mod?

Do you find yourself constantly switching between **Outward** and the [Outward Wiki](https://outward.fandom.com/wiki/Outward_Wiki)? 
Tired of **alt-tabbing** and letting Chrome consume unnecessary system resources just to find information that should already be available in-game?  

The **Outward Enchantments Viewer Mod** has you covered!  

### Features:

1. **Detailed Enchantment Descriptions** – Enchantments now display both their bonuses and drawbacks.  
2. **Enchantment Availability Count** – Item descriptions indicate the total number of enchantments available for that item.  
3. **Inventory Compatibility Tracking** – Item descriptions show how many compatible enchantments exist in your inventory.  
4. **Enchantment Listing** – All enchantments that can be applied to the item are listed in the description.  
5. **Dynamic Enchantment Descriptions** – The system retrieves enchantment details from other mods, ensuring comprehensive and up-to-date descriptions.  
6. **Adaptive Item Descriptions** – Item descriptions dynamically update by gathering information from other mods about available enchantments.  
7. **Fixed Scroll View for Item Display Details** – Improves handling of longer modded descriptions by adjusting the scroll view, allowing for better readability and navigation. This enhances precision, provides more screen space, and ensures smooth scrolling, even when using a controller. 

## How to use

1. Either clone/download the repository with Git or GitHub Desktop, or simply download the code manually.
2. Open `src/OutwardEnchantments.sln` with any C# IDE (Visual Studio, Rider, etc)
3. When you're ready, build the solution. It will be built to the `Release` folder (next to the `src` folder).
4. Take the DLL from the `Release` folder and put it in the `BepInEx/plugins/` folder. If you use r2modman, this can be found by going into r2modman settings and clicking on `Browse Profile Folder`.

## Known Bugs

1. The Outward developers did not attach sub-recipes (`EnchantmentRecipe`) to the **Filter** item of type `EnchantmentRecipeItem`.  
   - This item should include three sub-recipes: **boots, helmet, and chest armor**.  
   - However, only the chest armor variant is recognized, while the others are not counted as unlocked enchantments.

### If you liked the mod leave a star it's free
