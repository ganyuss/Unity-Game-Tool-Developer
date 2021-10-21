
# Unity Game Tool Developer - Case Study

The goal of this case study was to improve the workflow of adding a character to the store. This workflow was 7 steps long, and is described in the PDF that came along with the case study. 

## The photo booth

The steps 3 and 5 have been replaced by an automated process, happening in the scene called `CharacterPhotoShootBooth.unity`. Using the `CharacterPreviewCreator` class, one can create a preview for a character by simply calling one static method. The style of the preview is also highly customizable. For example, changing the background color is as easy as changing the color in the render camera of the scene.  

## The character and character list data structure

Instead of having a simple list of `StoreItem` in the `Store` class, it now also has a reference to a `CharacterList` scriptable object. This object holds a list of all the `CharacterData` objects to put in the store. 

At runtime, the `CharacterData` objects are converted to `StoreItem`s. I wanted to keep the way the store work as it is, because the `StoreItem` class seems to have a lot of uses in the code, and that way the store can still be used for things other than characters (like SFXs, stages...). That is why the store items list still exist, it is just now empty.

Using this structure, one can simply delete a character by opening its associated character data asset, and clicking the delete button.  

## The character creation window

To create a character, one can simply open the character creation window, by going in the top menu and clicking "Character Creator" > "Open Window". In there, they would simply specify the name of the new character, its price, and the FBX file to create the character from. They would then need to add the different materials they want to apply to the model, and click "Create Character".

The window will then create a prefab for the character from the FBX file, create a preview using the photo booth described above, create a character data structure populated with the specified information, and add it to the character list. By doing so, the character will be directly available in the store.

Once the FBX file is given to the window, the material model will be requested. One can click the "Open Material List" button to have access to all the `Toon/Lit` materials in the project. I set this filter because it looked like it was the style of the game, but this can easily be modified.

# The spreadsheet export feature

The first step in the previous character creation workflow was for the game designer to add the name and the price of the character in a spreadsheet. I added a "Character Creator" > "Export Spreadsheet" menu, to create a CSV file containing the names and prices of all the characters in the game. 

I do not know what the spreadsheet is used for, and if this spreadsheet is used frequently, we may want to change the code to have it constantly updated in the project.

## The Character creation parameters

The character creator references a lot of elements, so I created an object called `CharacterCreationParameters`, to centralize these references. 

### The folders

THe character creation window creates 3 different type of assets: prefabs, CharacterData assets and previews. I decided to put them in 3 different folders. To do so, I used `FolderLocator`s, which are just scriptable objects with an associated ID, that can be specified to a static method to get the folder path.

### The asset references

The character creator has to interact with a couple of other assets: 
* The "character logic" prefab, which is a prefab common to all characters. For now it contains only the capsule collider, but it can later be used to hold other things, like an audio listener for example.
* The character animator controller, which is also common to all characters, and is added to the model when creating the prefab.
* The photo booth scene, because the path to the scene needs to be stored somewhere.

### The spreadsheet export preferences

I added here a couple of options for the spreadsheet export feature: 
* Whether or not all the character should be included to the spreadsheet. If this value is false, then only the characters in the character list will be added. This is here to be able to hide "in progress" characters, for public communication for example.
* What separator to use in the CSV file. This can seem like a minor feature, but it can greatly increase work speed, especially since changing separator settings in excel can be really tedious.


## The debug key system

It is quite simple, but I added a debug system where keys are liked to `UnityAction`s when playing in the editor. This allowed me to link a key to a "skip mode" in the player controller, to not collide with enemies and go way faster, to test the game's win state.

## Request for change

When testing the game on my phone, the names and prices of the characters were not displayed. This was because the font size was too big for my phone screen. In my opinion, the cleanest way to fix this would be to use TextMeshPro instead, with their adaptative font size function.  

