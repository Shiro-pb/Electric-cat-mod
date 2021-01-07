using Partiality.Modloader;
using UnityEngine;
using RWCustom;
using MonoMod;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Menu
{
    [MonoModPatch("global::Menu.SlugcatSelectMenu/SlugcatPageNewGame")]
    public class patch_SlugcatPageNewGame : SlugcatSelectMenu.SlugcatPageNewGame
    {
        [MonoModIgnore]
        public patch_SlugcatPageNewGame(Menu menu, MenuObject owner, int pageIndex, int slugcatNumber) : base(menu, owner, pageIndex, slugcatNumber)
        {
        }
        [MonoModLinkTo("SlugcatSelectMenu.SlugcatPage", "System.Void AddImage(System.Boolean)")]
        public static void base_AddImage(bool ascended)
        {
        }
        [MonoModIgnore]
        public extern void orig_ctor(Menu menu, MenuObject owner, int pageIndex, int slugcatNumber);
        [MonoModConstructor]
        public void ctor(Menu menu, MenuObject owner, int pageIndex, int slugcatNumber)
        {
            //base_AddImage(false);
            string text = string.Empty;
            string text2 = string.Empty;
            this.orig_ctor(menu, owner, pageIndex, slugcatNumber);
            switch (slugcatNumber)
            {
                case 0:
                    text = menu.Translate("THE SURVIVOR");
                    text2 = menu.Translate("A nimble omnivore, both predator and prey. Lost in a harsh and indifferent<LINE>land you must make your own way, with wit and caution as your greatest assets.");
                    break;
                case 1:
                    text = menu.Translate("THE MONK");// TRANSFER CAT CHARACTER NUMBER
                    text2 = menu.Translate("Weak of body but strong of spirit. In tune with the mysteries of the world and<LINE>empathetic to its creatures, your journey will be a significantly more peaceful one.");
                    break;
                case 2:
                    text = menu.Translate("THE HUNTER");
                    text2 = menu.Translate("Strong and quick, with a fierce metabolism requiring a steady diet of meat. But the<LINE>stomach wont be your only concern, as the path of the hunter is one of extreme peril.");
                    break;
                case 3:
                    text = menu.Translate("THE ELECTRIC CAT");// TRANSFER CAT CHARACTER NUMBER
                    text2 = menu.Translate("Danger, high voltage");
                    break;
            }
            this.difficultyLabel = new MenuLabel(menu, this, text, new Vector2(-1000f, this.imagePos.y - 249f), new Vector2(200f, 30f), true);
            this.difficultyLabel.label.alignment = FLabelAlignment.Center;
            this.subObjects.Add(this.difficultyLabel);
            text2 = text2.Replace("<LINE>", Environment.NewLine);
            this.infoLabel = new MenuLabel(menu, this, text2, new Vector2(-1000f, this.imagePos.y - 249f - 40f), new Vector2(200f, 30f), false);
            this.infoLabel.label.alignment = FLabelAlignment.Center;
            this.subObjects.Add(this.infoLabel);
            if (slugcatNumber == 2 && !menu.manager.rainWorld.progression.miscProgressionData.redUnlocked)
            {
                this.difficultyLabel.label.color = Menu.MenuRGB(Menu.MenuColors.VeryDarkGrey);
                this.infoLabel.label.color = Menu.MenuRGB(Menu.MenuColors.VeryDarkGrey);
                return;
            }
            this.difficultyLabel.label.color = Menu.MenuRGB(Menu.MenuColors.MediumGrey);
            this.infoLabel.label.color = Menu.MenuRGB(Menu.MenuColors.DarkGrey);
        }
    }
    [MonoModPatch("global::Menu.SlugcatSelectMenu")]
    public class patch_SlugcatSelectMenu : SlugcatSelectMenu
    {
        [MonoModIgnore]
        public patch_SlugcatSelectMenu(ProcessManager manager) : base(manager) { }
        [MonoModIgnore]
        public extern void orig_ctor(ProcessManager manager);
        [MonoModConstructor]
        public void ctor(ProcessManager manager)
        {
            orig_ctor(manager);
            this.pages.Add(new Page(this, null, "main", 0));
            this.slugcatColorOrder = new int[]
            {
                1,
                0,
                2,
                3
            };
            for (int i = 0; i < this.slugcatColorOrder.Length; i++)
            {
                if (this.slugcatColorOrder[i] == manager.rainWorld.progression.miscProgressionData.currentlySelectedSinglePlayerSlugcat)
                {
                    this.slugcatPageIndex = i;
                }
            }
            this.saveGameData = new SlugcatSelectMenu.SaveGameData[4];
            for (int j = 0; j < 4; j++)
            {
                this.saveGameData[j] = SlugcatSelectMenu.MineForSaveData(manager, this.slugcatColorOrder[j]);
            }
            if (this.saveGameData[2] != null && ((this.saveGameData[2].redsDeath && this.saveGameData[2].cycle >= RedsIllness.RedsCycles(this.saveGameData[2].redsExtraCycles)) || this.saveGameData[2].ascended))
            {
                this.redIsDead = true;
            }
            int num = 0;
            for (int k = 0; k < 4; k++)
            {
                if (this.saveGameData[k] != null)
                {
                    num++;
                }
            }
            if (num == 1)
            {
                for (int l = 0; l < 4; l++)
                {
                    if (this.saveGameData[l] != null)
                    {
                        this.slugcatPageIndex = l;
                        break;
                    }
                }
            }
            this.slugcatPages = new SlugcatSelectMenu.SlugcatPage[4];
            for (int m = 0; m < this.slugcatPages.Length; m++)
            {
                if (this.saveGameData[m] != null)
                {
                    this.slugcatPages[m] = new SlugcatSelectMenu.SlugcatPageContinue(this, null, 1 + m, this.slugcatColorOrder[m]);
                }
                else
                {
                    this.slugcatPages[m] = new SlugcatSelectMenu.SlugcatPageNewGame(this, null, 1 + m, this.slugcatColorOrder[m]);
                }
                this.pages.Add(this.slugcatPages[m]);
            }           
        }
    }

    [MonoModPatch("global::Menu.SlugcatSelectMenu/SlugcatPage")]
    public class patch_SlugcatPage : SlugcatSelectMenu.SlugcatPage
    {
        [MonoModIgnore]
        public patch_SlugcatPage(Menu menu, MenuObject owner, int pageIndex, int slugcatNumber) : base(menu, owner, pageIndex, slugcatNumber) { }
        [MonoModIgnore]
        public extern void orig_ctor(Menu menu, MenuObject owner, int pageIndex, int slugcatNumber);
        [MonoModConstructor]
        public void ctor(Menu menu, MenuObject owner, int pageIndex, int slugcatNumber)
        {
            orig_ctor(menu, owner, pageIndex, slugcatNumber);
            this.colorName = string.Empty;
            this.slugcatNumber = slugcatNumber;
            this.effectColor = PlayerGraphics.SlugcatColor(slugcatNumber);
            switch (slugcatNumber)
            {
                case 0:
                    this.colorName = "White";
                    break;
                case 1:
                    this.colorName = "Yellow";
                    break;
                case 2:
                    this.colorName = "Red";
                    this.effectColor = Color.Lerp(this.effectColor, Color.red, 0.2f);
                    break;
                case 3:
                    this.colorName = "Electric";
                    break;
            }
        }
        public extern void orig_AddImage(bool ascended);
        public void AddImage(bool ascended)
        {
            this.imagePos = new Vector2(683f, 484f);
            this.sceneOffset = default(Vector2);
            this.slugcatDepth = 1f;
            MenuScene.SceneID sceneID = MenuScene.SceneID.Slugcat_White;
            switch (this.slugcatNumber)
            {
                case 0:
                    if (ascended)
                    {
                        sceneID = MenuScene.SceneID.Ghost_White;
                    }
                    else
                    {
                        sceneID = MenuScene.SceneID.Slugcat_White;
                    }
                    this.sceneOffset = new Vector2(-10f, 100f);
                    this.slugcatDepth = 3.10000014f;
                    this.markOffset = new Vector2(-15f, -2f);
                    this.glowOffset = new Vector2(-30f, -50f);
                    break;
                case 1:
                    if (ascended)
                    {
                        sceneID = MenuScene.SceneID.Ghost_Yellow;
                    }
                    else
                    {
                        sceneID = MenuScene.SceneID.Slugcat_Yellow;
                    }
                    this.sceneOffset = new Vector2(10f, 75f);
                    this.slugcatDepth = 3f;
                    this.markOffset = new Vector2(24f, -19f);
                    this.glowOffset = new Vector2(0f, -50f);
                    break;
                case 2:
                    if (ascended)
                    {
                        sceneID = MenuScene.SceneID.Ghost_Red;
                    }
                    else if ((this.menu as SlugcatSelectMenu).redIsDead)
                    {
                        sceneID = MenuScene.SceneID.Slugcat_Dead_Red;
                    }
                    else
                    {
                        sceneID = MenuScene.SceneID.Slugcat_Red;
                    }
                    this.sceneOffset = new Vector2(10f, 45f);
                    this.slugcatDepth = 2.7f;
                    this.markOffset = new Vector2(-3f, -73f);
                    this.glowOffset = new Vector2(-20f, -90f);
                    break;
                case 3:
                    if (ascended)
                    {
                        sceneID = MenuScene.SceneID.Ghost_Yellow;
                    }
                    else
                    {
                        sceneID = (MenuScene.SceneID)70;//Slugcat_Electric (MenuScene.SceneID)70
                    }
                    this.sceneOffset = new Vector2(10f, 75f);
                    this.slugcatDepth = 3f;
                    this.markOffset = new Vector2(24f, -19f);
                    this.glowOffset = new Vector2(0f, -50f);
                    break;
            }
            this.slugcatImage = new InteractiveMenuScene(this.menu, this, sceneID);
            this.subObjects.Add(this.slugcatImage);
            if (this.HasMark)
            {
                this.markSquare = new FSprite("pixel", true);
                this.markSquare.scale = 14f;
                this.markSquare.color = Color.Lerp(this.effectColor, Color.white, 0.7f);
                this.Container.AddChild(this.markSquare);
                this.markGlow = new FSprite("Futile_White", true);
                this.markGlow.shader = this.menu.manager.rainWorld.Shaders["FlatLight"];
                this.markGlow.color = this.effectColor;
                this.Container.AddChild(this.markGlow);
            }
        }
    }
    
    [MonoModPatch("global::Menu.MenuScene")]
    public class patch_MenuScene : MenuScene
    {
        [MonoModIgnore]
        public patch_MenuScene(Menu menu, MenuObject owner, MenuScene.SceneID sceneID) : base(menu, owner, sceneID) { }
        [MonoModIgnore]
        public extern void orig_ctor(Menu menu, MenuObject owner, MenuScene.SceneID sceneID);
        
        [MonoModLinkTo("MenuObject", "System.Void .ctor(Menu, MenuObject)")]
        public static void base_ctor(Menu menu, MenuObject owner) { }
        [MonoModConstructor]
        public void ctor(Menu menu, MenuObject owner, MenuScene.SceneID sceneID)
        { 
            orig_ctor(menu, owner, sceneID);
        }
        
        public extern void orig_BuildScene();
        public void BuildScene()
        {
            Vector2 vector = new Vector2(0f, 0f);
            orig_BuildScene();
            switch (this.sceneID)
            {
                case (MenuScene.SceneID)70://Select screen normal
                    this.sceneFolder = "Scenes" + Path.DirectorySeparatorChar + "Slugcat - Electric";
                    if (this.flatMode)
                    {
                        this.AddIllustration(new MenuIllustration(this.menu, this, this.sceneFolder, "Slugcat - Electric - Flat", new Vector2(683f, 384f), false, true));
                    }
                    else
                    {
                        this.AddIllustration(new MenuDepthIllustration(this.menu, this, this.sceneFolder, "Electric Background - 5", new Vector2(0f, 0f), 3.6f, MenuDepthIllustration.MenuShader.Normal));
                        this.AddIllustration(new MenuDepthIllustration(this.menu, this, this.sceneFolder, "Electric Specks - 4", new Vector2(0f, 0f), 2.7f, MenuDepthIllustration.MenuShader.Overlay));
                        this.AddIllustration(new MenuDepthIllustration(this.menu, this, this.sceneFolder, "Electric Vines - 3", new Vector2(0f, 0f), 2.9f, MenuDepthIllustration.MenuShader.Normal));
                        this.AddIllustration(new MenuDepthIllustration(this.menu, this, this.sceneFolder, "Electric BkgPlants - 2", new Vector2(0f, 0f), 3.1f, MenuDepthIllustration.MenuShader.Normal));
                        if (this.owner is SlugcatSelectMenu.SlugcatPage)
                        {
                            (this.owner as SlugcatSelectMenu.SlugcatPage).AddGlow();
                        }
                        this.AddIllustration(new MenuDepthIllustration(this.menu, this, this.sceneFolder, "Electric Slugcat - 1", new Vector2(0f, 0f), 2.6f, MenuDepthIllustration.MenuShader.LightEdges));
                        this.AddIllustration(new MenuDepthIllustration(this.menu, this, this.sceneFolder, "Electric FgPlants - 0", new Vector2(0f, 0f), 2.1f, MenuDepthIllustration.MenuShader.Normal));
                        ((this as MenuScene) as InteractiveMenuScene).idleDepths.Add(3.6f);
                        ((this as MenuScene) as InteractiveMenuScene).idleDepths.Add(2.8f);
                        ((this as MenuScene) as InteractiveMenuScene).idleDepths.Add(2.6f);
                        ((this as MenuScene) as InteractiveMenuScene).idleDepths.Add(2.5f);
                        ((this as MenuScene) as InteractiveMenuScene).idleDepths.Add(1.5f);
                    }
                    break;
                    
                case (MenuScene.SceneID)71://select screen ghost
                    this.sceneFolder = "Scenes" + Path.DirectorySeparatorChar + "Electric Ghost Slugcat";
                    if (this.flatMode)
                    {
                        this.AddIllustration(new MenuIllustration(this.menu, this, this.sceneFolder, "Electric Ghost Slugcat - Flat", new Vector2(683f, 384f), false, true));
                    }
                    else
                    {
                        this.AddIllustration(new MenuDepthIllustration(this.menu, this, this.sceneFolder, "Electric Ghost Bkg", new Vector2(0f, 0f), 4.5f, MenuDepthIllustration.MenuShader.Normal));
                        this.AddIllustration(new MenuDepthIllustration(this.menu, this, this.sceneFolder, "Electric Ghost A", new Vector2(0f, 0f), 2.85f, MenuDepthIllustration.MenuShader.Normal));
                        this.AddIllustration(new MenuDepthIllustration(this.menu, this, this.sceneFolder, "Electric Ghost B", new Vector2(0f, 0f), 2.7f, MenuDepthIllustration.MenuShader.Overlay));
                        ((this as MenuScene) as InteractiveMenuScene).idleDepths.Add(3.1f);
                        ((this as MenuScene) as InteractiveMenuScene).idleDepths.Add(2.8f);
                    }
                    break;
                case MenuScene.SceneID.SleepScreen:
                    //Debug.Log("||---||--sleep screen cleared?--||---||");
                    this.subObjects.Clear();
                    this.depthIllustrations.Clear();
                    int num;
                    if (this.menu.manager.currentMainLoop is RainWorldGame)
                    {
                        num = (this.menu.manager.currentMainLoop as RainWorldGame).StoryCharacter;
                    }
                    else
                    {
                        num = this.menu.manager.rainWorld.progression.PlayingAsSlugcat;
                    }
                    string text = "White";
                    if (num != 0)
                    {
                        text = ((num != 1) ? "Red" : "Yellow");
                    }
                    if (num == 3)
                    {
                        text = "Electric";
                    }
                    this.sceneFolder = string.Concat(new object[]
                    {
                        "Scenes",
                        Path.DirectorySeparatorChar,
                        "Sleep Screen - ",
                        text
                    });
                    if (this.flatMode)
                    {
                        this.AddIllustration(new MenuIllustration(this.menu, this, this.sceneFolder, "Sleep Screen - " + text + " - Flat", new Vector2(683f, 384f), false, true));
                    }
                    else
                    {
                        this.AddIllustration(new MenuDepthIllustration(this.menu, this, this.sceneFolder, "Sleep - 5", new Vector2(23f, 17f), 3.5f, MenuDepthIllustration.MenuShader.Normal));
                        this.AddIllustration(new MenuDepthIllustration(this.menu, this, this.sceneFolder, "Sleep - 4", new Vector2(23f, 17f), 2.8f, MenuDepthIllustration.MenuShader.Normal));
                        this.depthIllustrations[this.depthIllustrations.Count - 1].setAlpha = new float?(0.24f);
                        this.AddIllustration(new MenuDepthIllustration(this.menu, this, this.sceneFolder, "Sleep - 3", new Vector2(23f, 17f), 2.2f, MenuDepthIllustration.MenuShader.Normal));
                        this.AddIllustration(new MenuDepthIllustration(this.menu, this, this.sceneFolder, "Sleep - 2 - " + text, new Vector2(23f, 17f), 1.7f, MenuDepthIllustration.MenuShader.Normal));
                        this.AddIllustration(new MenuDepthIllustration(this.menu, this, this.sceneFolder, "Sleep - 1", new Vector2(23f, 17f), 1.2f, MenuDepthIllustration.MenuShader.Normal));
                        ((this as MenuScene) as InteractiveMenuScene).idleDepths.Add(3.3f);
                        ((this as MenuScene) as InteractiveMenuScene).idleDepths.Add(2.7f);
                        ((this as MenuScene) as InteractiveMenuScene).idleDepths.Add(1.8f);
                        ((this as MenuScene) as InteractiveMenuScene).idleDepths.Add(1.7f);
                        ((this as MenuScene) as InteractiveMenuScene).idleDepths.Add(1.6f);
                        ((this as MenuScene) as InteractiveMenuScene).idleDepths.Add(1.2f);
                    }
                    break;
                    
            }
            if (this.sceneFolder != string.Empty && File.Exists(string.Concat(new object[]
            {
                Custom.RootFolderDirectory(),
                Path.DirectorySeparatorChar,
                "Assets",
                Path.DirectorySeparatorChar,
                "Futile",
                Path.DirectorySeparatorChar,
                "Resources",
                Path.DirectorySeparatorChar,
                this.sceneFolder,
                Path.DirectorySeparatorChar,
                "positions.txt"
            })))
            {
                string[] array = File.ReadAllLines(string.Concat(new object[]
                {
                    Custom.RootFolderDirectory(),
                    Path.DirectorySeparatorChar,
                    "Assets",
                    Path.DirectorySeparatorChar,
                    "Futile",
                    Path.DirectorySeparatorChar,
                    "Resources",
                    Path.DirectorySeparatorChar,
                    this.sceneFolder,
                    Path.DirectorySeparatorChar,
                    "positions.txt"
                }));
                int num2 = 0;
                while (num2 < array.Length && num2 < this.depthIllustrations.Count)
                {
                    this.depthIllustrations[num2].pos.x = float.Parse(Regex.Split(array[num2], ", ")[0]) + vector.x;//+ vector.x
                    this.depthIllustrations[num2].pos.y = float.Parse(Regex.Split(array[num2], ", ")[1]) + vector.y;//+ vector.y
                    this.depthIllustrations[num2].lastPos = this.depthIllustrations[num2].pos;
                    num2++;
                }
            }
        }
        public enum SceneID
        {
            Empty,
            MainMenu,
            SleepScreen,
            RedsDeathStatisticsBkg,
            NewDeath,
            StarveScreen,
            Intro_1_Tree,
            Intro_2_Branch,
            Intro_3_In_Tree,
            Intro_4_Walking,
            Intro_5_Hunting,
            Intro_6_7_Rain_Drop,
            Intro_8_Climbing,
            Intro_9_Rainy_Climb,
            Intro_10_Fall,
            Intro_10_5_Separation,
            Intro_11_Drowning,
            Intro_12_Waking,
            Intro_13_Alone,
            Intro_14_Title,
            Endgame_Survivor,
            Endgame_Hunter,
            Endgame_Saint,
            Endgame_Traveller,
            Endgame_Chieftain,
            Endgame_Monk,
            Endgame_Outlaw,
            Endgame_DragonSlayer,
            Endgame_Martyr,
            Endgame_Scholar,
            Endgame_Mother,
            Endgame_Friend,
            Landscape_CC,
            Landscape_DS,
            Landscape_GW,
            Landscape_HI,
            Landscape_LF,
            Landscape_SB,
            Landscape_SH,
            Landscape_SI,
            Landscape_SL,
            Landscape_SS,
            Landscape_SU,
            Landscape_UW,
            Outro_1_Left_Swim,
            Outro_2_Up_Swim,
            Outro_3_Face,
            Outro_4_Tree,
            Options_Bkg,
            Dream_Sleep,
            Dream_Sleep_Fade,
            Dream_Acceptance,
            Dream_Iggy,
            Dream_Iggy_Doubt,
            Dream_Iggy_Image,
            Dream_Moon_Betrayal,
            Dream_Moon_Friend,
            Dream_Pebbles,
            Void_Slugcat_Upright,
            Void_Slugcat_Down,
            Slugcat_White,
            Slugcat_Yellow,
            Slugcat_Red,
            Ghost_White,
            Ghost_Yellow,
            Ghost_Red,
            Yellow_Intro_A,
            Yellow_Intro_B,
            Slugcat_Dead_Red,
            Red_Ascend,
            Slugcat_Electric,
            Ghost_Electric
        }
    }
    

}
namespace ElectricCat_Mod
{
    [MonoModPatch("global::WorldLoader")]
    public class patch_WorldLoader : WorldLoader
    {
        [MonoModIgnore]
        public patch_WorldLoader(RainWorldGame game, int playerCharacter, bool singleRoomWorld, string worldName, Region region, RainWorldGame.SetupValues setupValues) : base(game, playerCharacter, singleRoomWorld, worldName, region, setupValues) { }
        [MonoModIgnore]
        public extern void orig_ctor(RainWorldGame game, int playerCharacter, bool singleRoomWorld, string worldName, Region region, RainWorldGame.SetupValues setupValues);
        [MonoModConstructor]
        public void ctor(RainWorldGame game, int playerCharacter, bool singleRoomWorld, string worldName, Region region, RainWorldGame.SetupValues setupValues)
        {
            if (playerCharacter == 3)
            {
                //Debug.Log("SLUGLOAF THREE SELECTED");
                this.newSlugcat = true;
                orig_ctor(game, 2, singleRoomWorld, worldName, region, setupValues);
            }
            else
            {
                //Debug.Log("SLUGLOAF THREE NOT SELECTED");
                this.newSlugcat = false;
                orig_ctor(game, playerCharacter, singleRoomWorld, worldName, region, setupValues);
            }
               
            
            
        }        
        public bool newSlugcat;
    }
    [MonoModPatch("global::PlacedObject/FilterData")]
    public class patch_FilterData : PlacedObject.FilterData
    {
        [MonoModIgnore]
        public patch_FilterData(PlacedObject owner) : base(owner) { }
        [MonoModIgnore]
        public extern void orig_ctor(PlacedObject owner);
        [MonoModConstructor]
        public void ctor(PlacedObject owner)
        {
            orig_ctor(owner);
            this.availableToPlayers = new bool[4];
            for (int i = 0; i < this.availableToPlayers.Length; i++)
            {
                this.availableToPlayers[i] = true;
            }
        }
    }
    [MonoModPatch("global::EventTrigger")]
    public class patch_EventTrigger : EventTrigger
    {
        [MonoModIgnore]
        public patch_EventTrigger(EventTrigger.TriggerType type) : base(type) { }
        [MonoModIgnore]
        public extern void orig_ctor(EventTrigger.TriggerType type);
        [MonoModConstructor]
        public void ctor(EventTrigger.TriggerType type)
        {
            orig_ctor(type);
            this.slugcats = new bool[]
        {
            true,
            true,
            true,
            true
        };
        }
    }
    [MonoModPatch("global::SlugcatStats")]
    public class patch_SlugcatStats : SlugcatStats
    {
        [MonoModIgnore]
        public patch_SlugcatStats(int slugcatNumber, bool malnourished) : base(slugcatNumber, malnourished)
        {
        }
        [MonoModIgnore]
        public extern void orig_ctor(int slugcatNumber, bool malnourished);
        [MonoModConstructor]
        public void ctor(int slugcatNumber, bool malnourished)
        {
            this.orig_ctor(slugcatNumber, malnourished);
            switch(this.name)
            {
                case (SlugcatStats.Name)0x00000003:
                    this.throwingSkill = 2;
                    break;
            }
        }
        public new static IntVector2 SlugcatFoodMeter(int slugcatNum)
        {
            switch (slugcatNum)
            {
                case 0:
                    return new IntVector2(7, 4);
                case 1:
                    return new IntVector2(5, 3);
                case 2:
                    return new IntVector2(9, 6);
                default:
                    return new IntVector2(8, 4);
            }
        }
        public enum Name
        {
            White,
            Yellow,
            Red,
            Electric
        }
    }
    [MonoModPatch("global::PlayerGraphics")]
    public class patch_PlayerGraphics : PlayerGraphics
    {
        [MonoModIgnore]
        public patch_PlayerGraphics(PhysicalObject ow) : base(ow)
        {
        }
        [MonoModIgnore]
        public extern void orig_ctor(PhysicalObject ow);
        public override void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
        {
            if (newContatiner == null)
            {
                newContatiner = rCam.ReturnFContainer("Midground");
            }
            for (int i = 0; i < sLeaser.sprites.Length; i++)
            {
                if ((i > 6 && i < 9) || i > 9)
                {
                    
                    if(this.player.playerState.slugcatCharacter == 3)//TRANSFER SLUGCAT CHARACTER   
                    {
                        if (i == 12)
                        {
                            newContatiner.AddChild(sLeaser.sprites[i]);
                        }
                        else if (i == 13)
                        {
                            newContatiner.AddChild(sLeaser.sprites[i]);
                        }
                        else if (i == 14)
                        {
                            newContatiner.AddChild(sLeaser.sprites[i]);
                        }
                        else
                        {
                            rCam.ReturnFContainer("Foreground").AddChild(sLeaser.sprites[i]);
                        }
                    }
                    else
                    {
                        if (i == 12)
                        {
                            rCam.ReturnFContainer("Bloom").AddChild(sLeaser.sprites[i]);
                        }
                        else if (i == 13)
                        {
                            newContatiner.AddChild(sLeaser.sprites[i]);
                        }
                        else if (i == 14)
                        {
                            newContatiner.AddChild(sLeaser.sprites[i]);
                        }
                        else
                        {
                            rCam.ReturnFContainer("Foreground").AddChild(sLeaser.sprites[i]);
                        }
                    }                 
                }
                else
                {
                    newContatiner.AddChild(sLeaser.sprites[i]);
                }
            }
        }
        public extern void orig_ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette);
        public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            Color color = PlayerGraphics.SlugcatColor(this.player.playerState.slugcatCharacter);
            Color color2 = palette.blackColor;
            if (this.malnourished > 0f)
            {
                float num = (!this.player.Malnourished) ? Mathf.Max(0f, this.malnourished - 0.005f) : this.malnourished;
                color = Color.Lerp(color, Color.gray, 0.4f * num);
                color2 = Color.Lerp(color2, Color.Lerp(Color.white, palette.fogColor, 0.5f), 0.2f * num * num);
            }
            if (this.player.playerState.slugcatCharacter == 3) // TRANSFER CAT CHARACTER NUMBER
            {
                color = patch_PlayerGraphics.ElectricBodyColor(this.player.chargedActive, this.player.chargedTimer, this.player.stunDelay);
                color2 = palette.blackColor;
            }
            /*
            if (this.player.playerState.slugcatCharacter == 3)
            {
                color2 = Color.Lerp(new Color(1f, 1f, 1f), color, 0.3f);
                color = Color.Lerp(palette.blackColor, Custom.HSL2RGB(0.63055557f, 0.54f, 0.5f), Mathf.Lerp(0.08f, 0.04f, palette.darkness));
            }
            */
            for (int i = 0; i < sLeaser.sprites.Length; i++)
            {
                sLeaser.sprites[i].color = color;
            }
            sLeaser.sprites[11].color = Color.Lerp(PlayerGraphics.SlugcatColor(this.player.playerState.slugcatCharacter), Color.white, 0.3f);
            sLeaser.sprites[9].color = color2;
            if (this.player.playerState.slugcatCharacter == 3) // TRANSFER CAT CHARACTER NUMBER
            {
                sLeaser.sprites[12].color = AntennaBaseColor(player.chargedActive);
                sLeaser.sprites[13].color = AntennaTipColor(player.chargedActive, player.receivingMessage);
                if (player.receivingMessage && player.chargedActive)
                {
                    sLeaser.sprites[9].color = AntennaTipColor(true, true);
                }
            }                               
        }     
        public extern void orig_DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos);
        public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            this.ApplyPalette(sLeaser, rCam, rCam.currentPalette);
            this.orig_DrawSprites(sLeaser, rCam, timeStacker, camPos);
            if(this.player.playerState.slugcatCharacter == 3) // TRANSFER CAT CHARACTER NUMBER
            {
                if (!player.antennaActive)
                {
                    sLeaser.sprites[13].scale = 0.01f;
                    sLeaser.sprites[12].scale = 0.01f;
                }
                else
                {
                    sLeaser.sprites[13].scale = 2.3f;
                    sLeaser.sprites[12].scaleX = 0.95f;
                    sLeaser.sprites[12].scaleY = 4f;
                }               
                sLeaser.sprites[12].x = sLeaser.sprites[3].x;
                sLeaser.sprites[12].y = sLeaser.sprites[3].y + 2f;
                sLeaser.sprites[12].rotation = sLeaser.sprites[3].rotation;
                sLeaser.sprites[13].x = sLeaser.sprites[3].x;
                sLeaser.sprites[13].y = sLeaser.sprites[3].y + 2.6f;
                sLeaser.sprites[13].rotation = sLeaser.sprites[3].rotation;
            }
        }
        public new static Color SlugcatColor(int i)
        {
            switch (i)
            {
                case 0:
                    return new Color(1f, 1f, 1f);
                case 1:
                    return new Color(1f, 1f, 0.4509804f); // TRANSFER CAT CHARACTER NUMBER
                case 2:
                    return new Color(1f, 0.4509804f, 0.4509804f);
                case 3:
                    return new Color(0.6509f, 0.3764f, 0f); // TRANSFER CAT CHARACTER NUMBER
                default:
                    return new Color(1f, 1f, 1f);
            }
        }       
        [MonoModLinkTo("GraphicsModule", "System.Void InitiateSprites(RoomCamera.SpriteLeaser, RoomCamera )")]
        public static void base_InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
        }
        public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            if (!base.owner.room.game.DEBUGMODE)
            {
                if (this.player.playerState.slugcatCharacter == 3) // TRANSFER CAT CHARACTER NUMBER
                {
                    sLeaser.sprites = new FSprite[14];
                }
                else
                {
                    sLeaser.sprites = new FSprite[12];

                }
                sLeaser.sprites[0] = new FSprite("BodyA", true);
                sLeaser.sprites[0].anchorY = 0.7894737f;
                sLeaser.sprites[1] = new FSprite("HipsA", true);
                TriangleMesh.Triangle[] tris = new TriangleMesh.Triangle[]
                {
                    new TriangleMesh.Triangle(0, 1, 2),
                    new TriangleMesh.Triangle(1, 2, 3),
                    new TriangleMesh.Triangle(4, 5, 6),
                    new TriangleMesh.Triangle(5, 6, 7),
                    new TriangleMesh.Triangle(8, 9, 10),
                    new TriangleMesh.Triangle(9, 10, 11),
                    new TriangleMesh.Triangle(12, 13, 14),
                    new TriangleMesh.Triangle(2, 3, 4),
                    new TriangleMesh.Triangle(3, 4, 5),
                    new TriangleMesh.Triangle(6, 7, 8),
                    new TriangleMesh.Triangle(7, 8, 9),
                    new TriangleMesh.Triangle(10, 11, 12),
                    new TriangleMesh.Triangle(11, 12, 13)
                };
                TriangleMesh triangleMesh = new TriangleMesh("Futile_White", tris, false, false);
                sLeaser.sprites[2] = triangleMesh;
                sLeaser.sprites[3] = new FSprite("HeadA0", true);
                sLeaser.sprites[4] = new FSprite("LegsA0", true);
                sLeaser.sprites[4].anchorY = 0.25f;
                sLeaser.sprites[5] = new FSprite("PlayerArm0", true);
                sLeaser.sprites[5].anchorX = 0.9f;
                sLeaser.sprites[5].scaleY = -1f;
                sLeaser.sprites[6] = new FSprite("PlayerArm0", true);
                sLeaser.sprites[6].anchorX = 0.9f;
                sLeaser.sprites[7] = new FSprite("OnTopOfTerrainHand", true);
                sLeaser.sprites[8] = new FSprite("OnTopOfTerrainHand", true);
                sLeaser.sprites[8].scaleX = -1f;
                sLeaser.sprites[9] = new FSprite("FaceA0", true);
                sLeaser.sprites[11] = new FSprite("pixel", true);
                sLeaser.sprites[11].scale = 5f;
                sLeaser.sprites[10] = new FSprite("Futile_White", true);
                sLeaser.sprites[10].shader = rCam.game.rainWorld.Shaders["FlatLight"];               
                if (this.player.playerState.slugcatCharacter == 3) // TRANSFER CAT CHARACTER NUMBER
                {
                    sLeaser.sprites[12] = new FSprite("pixel", false);//base
                    sLeaser.sprites[12].scaleY = 4f;
                    sLeaser.sprites[12].scaleX = 1f;
                    sLeaser.sprites[12].anchorY = -1.02f;
                    sLeaser.sprites[13] = new FSprite("pixel", false);//tip
                    sLeaser.sprites[13].scale = 2.3f;
                    sLeaser.sprites[13].anchorY = -3.1f;
                    //rCam.ReturnFContainer("Midground").AddChild(sLeaser.sprites[12]);
                    //rCam.ReturnFContainer("Midground").AddChild(sLeaser.sprites[13]);

                }
                this.AddToContainer(sLeaser, rCam, null);
            }
            else
            {
                sLeaser.sprites = new FSprite[2];
                for (int i = 0; i < 2; i++)
                {
                    FSprite fsprite = new FSprite("pixel", true);
                    sLeaser.sprites[i] = fsprite;
                    rCam.ReturnFContainer("Midground").AddChild(fsprite);
                    fsprite.x = -10000f;
                    fsprite.color = new Color(1f, 0.7f, 1f);
                    fsprite.scale = base.owner.bodyChunks[i].rad * 2f;
                    fsprite.shader = FShader.Basic;
                }
            }
            patch_PlayerGraphics.base_InitiateSprites(sLeaser, rCam);
        }        
        public extern void orig_Update();
        public override void Update()
        {
            if(player.playerState.slugcatCharacter == 3)  // TRANSFER CAT CHARACTER NUMBER
            {
                if (this.lightSource != null)
                {
                    this.lightSource.stayAlive = true;
                    this.lightSource.setPos = new Vector2?(player.firstChunk.pos);
                    this.lightSource.setRad = new float?((300f * Mathf.Pow(player.lightFlash * UnityEngine.Random.value, 0.01f) * Mathf.Lerp(0.5f, 2f, 0.8f)) - 1.3f);//THE LAST FLOAT IN MATHF.LERP IS A MIGUE
                    this.lightSource.setAlpha = new float?((Mathf.Pow(player.lightFlash * UnityEngine.Random.value, 0.01f)) - 0.8f);
                    float num5 = player.lightFlash * UnityEngine.Random.value;
                    num5 = Mathf.Lerp(num5, 1f, 0.5f * (1f - player.room.Darkness(player.firstChunk.pos)));
                    this.lightSource.color = new Color(num5, num5, 1.5f);
                    if (player.lightFlash <= 0f)
                    {
                        this.lightSource.Destroy();
                    }
                    if (this.lightSource.slatedForDeletetion)
                    {
                        this.lightSource = null;
                    }
                }
                else if (player.lightFlash > 0f)
                {
                    this.lightSource = new LightSource(player.firstChunk.pos, false, new Color(1f, 1f, 1f), player);
                    this.lightSource.affectedByPaletteDarkness = 0f;
                    this.lightSource.requireUpKeep = true;
                    player.room.AddObject(this.lightSource);
                }
                if (player.lightFlash > 0f)
                {
                    player.lightFlash = Mathf.Max(0f, player.lightFlash - 0.0333933351f);
                }
                if (this.player.room != null)
                {
                    int slugcatCharacter = this.player.playerState.slugcatCharacter;
                }
                if (player.chargedActive)
                {
                    for (int i = 0; i < (int)Mathf.Lerp(4f, 5f, 0.15f); i++)
                    {
                        player.room.AddObject(new Spark(player.firstChunk.pos, Custom.RNV() * Mathf.Lerp(4f, 14f, UnityEngine.Random.value), new Color(0.7f, 0.7f, 1f), null, 2, 14));
                    }
                }
            }          
            this.orig_Update();
            

        }
        public static Color ElectricBodyColor(bool chargedActive, float chargedTimer, float stunDelay)
        {
            Color b = new Color(0.6509f, 0.3764f, 0f);//discharged color
            Color a = new Color(1f, 0.6274f, 0.1254f);//charged color
            Color c = new Color(1f, 0.7176f, 0.3333f);//overcharged
            Color color;
            if (chargedActive)
            {
                if(stunDelay > 0)
                {
                    color = Color.Lerp(c, b, (stunDelay/300f));
                }
                else
                {
                    if (chargedTimer <= 100)
                    {
                        color = new Color(0.6509f + (chargedTimer * 0.00349f), 0.3764f + (chargedTimer * 0.0025f), 0f + (chargedTimer * 0.00125f));
                    }
                    else
                    {
                        color = a;
                    }
                }                
            }
            else
            {
                color = b;
            }            
            return color;
        }
        public static Color AntennaBaseColor(bool chargedActive)
        {
            Color a = Custom.HSL2RGB(0.8588f, 0.7333f, 0.5216f);//discharged Custom.HSL2RGB(0.09019f, 0.94117f, 0.3568f)
            Color b = Custom.HSL2RGB(0.8588f, 0.7294f, 0.6942f);//charged Custom.HSL2RGB(0.09019f, 0.94117f, 0.52941f)
            return (chargedActive ? b : a);
        }
        public Color AntennaTipColor(bool chargedActive, bool receivingMessage)
        {
            
            Color a = Custom.HSL2RGB(0.8588f, 0.7333f, 0.4352f);//discharged Custom.HSL2RGB(0.09019f, 0.94117f, 0.5647f)
            Color b = Custom.HSL2RGB(receivingMessage ? (this.count / 255f) : 0.8588f, 0.7294f, 0.74509f);//charged Custom.HSL2RGB(receivingMessage ? (this.count / 255f) : 0.09019f, 0.94117f, 0.7019f)
            if (count >= 250)
            {
                count = 0;
            }
            else
            {
                count = count + 10;
            }
            return (chargedActive ? b : a);
        }
        public LightSource lightSource;
        public GraphicsModule graphicsModule;
        public int count = 0;

        [MonoModIgnore]
        private patch_Player player;
    }
    [MonoModPatch("global::Player")]
    public class patch_Player : Player
    {
        [MonoModIgnore]
        public patch_Player(AbstractCreature abstractCreature, World world) : base(abstractCreature, world)
        {
        }
        [MonoModIgnore]
        public extern void orig_ctor(AbstractCreature abstractCreature, World world);
        [MonoModConstructor]
        public void ctor(AbstractCreature abstractCreature, World world)
        {
            this.orig_ctor(abstractCreature, world);

            this.receivingMessage = false;
            this.antennaActive = true; // ANTENNA HANDLER
            this.stunDelay = 0;
            this.animationCount = 0;
            this.animationDelay = 0;
            this.chargedActive = false;
            this.chargedTimer = 0f;
            this.buttonPressed = 0f;
        }
        public void EnterChargedModeNoToll()
        {
            if (!chargedActive)
            {
                this.room.AddObject(new Explosion.ExplosionLight(base.firstChunk.pos, 100f, 0.85f, 6, new Color(0.7f, 0.7f, 1f)));
                this.buttonPressed = 0f;
                this.room.PlaySound(SoundID.Bomb_Explode, this.firstChunk, false, 0.7f, 1);
                this.chargedActive = true;
                base.gravity = 0.75f;
                base.slugcatStats.runspeedFac = 1.5f;
                base.slugcatStats.poleClimbSpeedFac = 2f;
                base.slugcatStats.corridorClimbSpeedFac = 2f;
                base.buoyancy = 0.95f;
                this.slugcatStats.throwingSkill = 2;
                this.chargedTimer = 600 * 2;// 600= ten seconds
            }
        }
        public void ExitChargeMode()
        {
            this.chargedActive = false;
            this.chargedTimer = 0;
            base.slugcatStats.runspeedFac = 0.9f;
            base.slugcatStats.poleClimbSpeedFac = 1f;
            base.slugcatStats.corridorClimbSpeedFac = 1f;
            base.gravity = 0.9f;
            base.buoyancy = 0.95f;
            this.slugcatStats.throwingSkill = 1;
            this.room.PlaySound(SoundID.Centipede_Shock, this.firstChunk);
        }
        public void EnterChargedMode()
        {
            if (!chargedActive)
            {
                this.room.AddObject(new Explosion.ExplosionLight(base.firstChunk.pos, 150f, 0.85f, 6, new Color(0.7f, 0.7f, 1f)));
                this.buttonPressed = 0f;
                this.room.PlaySound(SoundID.Bomb_Explode, this.firstChunk, false, 0.7f, 1);
                this.chargedActive = true;
                base.gravity = 0.75f;
                base.slugcatStats.runspeedFac = 1.5f;
                base.slugcatStats.poleClimbSpeedFac = 2f;
                base.slugcatStats.corridorClimbSpeedFac = 2f;                
                this.playerState.foodInStomach -= 4;
                base.buoyancy = 0.95f;
                this.slugcatStats.throwingSkill = 2;
                if (this.Karma > 5)
                {
                    this.chargedTimer = 600 * this.Karma;// 600= ten seconds 
                }
                else
                {
                    this.chargedTimer = 600 * 4;// 600= ten seconds 
                }
                if (base.abstractCreature.world.game.IsStorySession)
                {
                    base.abstractCreature.world.game.GetStorySession.saveState.totFood -= 4;
                }
            }

        }
        public void CheckChargingUp()
        {          
            if (this.playerState.foodInStomach >= 4)
            {
                if (base.stun == 0 && !base.dead && !this.room.GetTile(this.coord).verticalBeam && !this.chargedActive && this.bodyMode == Player.BodyModeIndex.Stand)
                {
                    if (base.input[0].y > 0)// base.input[0].mp
                    {
                        this.buttonPressed = this.buttonPressed + 1f;
                        this.Blink(5);
                        if(this.buttonPressed >= 40f)
                        {
                            this.room.PlaySound(SoundID.Centipede_Shock, this.firstChunk, false, 0.6f, 0.8f);
                        }                       
                        if (this.buttonPressed >= 90f)
                        {
                            
                            this.EnterChargedMode();
                            return;
                        }
                    }                   
                }
                else
                {
                    this.buttonPressed = 0f;
                }
            }
        }
        public extern void orig_Update(bool eu);
        public void Update(bool eu)
        {
            if(this.playerState.slugcatCharacter == 3) // TRANSFER CAT CHARACTER NUMBER
            {
                if (this.stunDelay > 0)
                {
                    if(stunDelay == 120)
                    {
                        this.room.PlaySound(SoundID.Centipede_Electric_Charge_LOOP, this.firstChunk, false, 1f, 1f);
                    }
                    this.stunDelay--;
                }
                //antenna random pop up handler
                /*
                if (this.coord == this.preCoord)
                {
                    animationCount++;
                    if (this.animationCount >= 60)
                    {
                        this.animationCount = 0;
                        this.animationDelay = 120;
                    }

                }
                else
                {
                    this.animationCount = 0;
                }
                if (this.animationDelay >= 0)
                {
                    this.antennaActive = true;
                    this.animationDelay--;
                }
                else
                {
                    this.antennaActive = false;
                }
                */
                this.preCoord = this.coord;
                //simulate message received
                if (Input.GetKeyDown(KeyCode.L))
                {
                    if (!this.receivingMessage)
                    {
                        this.receivingMessage = true;
                    }
                    else
                    {
                        this.receivingMessage = false;
                    }
                }
                //end charged mode for certain situations
                if (this.submerged || this.bodyMode == Player.BodyModeIndex.Swimming)
                {
                    if (this.chargedActive)
                    {
                        this.room.AddObject(new UnderwaterShock(this.room, this, this.firstChunk.pos, 14, Mathf.Lerp(200f, 1200f, 0.5f), 0.2f + 1.9f * 0.5f, this, new Color(0.7f, 0.7f, 1f)));
                        this.ExitChargeMode();
                    }
                }
                if (base.dead)
                {
                    if (this.chargedActive)
                    {
                        this.ExitChargeMode();
                    }
                }
                //charged mode activation handler
                this.CheckChargingUp();
                if (chargedActive)
                {
                    this.lightFlash = 4f;
                    this.room.PlaySound(SoundID.Centipede_Shock, this.firstChunk, false, 0.25f, 4f);
                    this.chargedTimer--;
                    if (chargedTimer == 0)
                    {
                        chargedActive = false;
                        ExitChargeMode();
                    }
                }
            }           
            this.orig_Update(eu);
        }
        public extern void orig_Collide(PhysicalObject otherObject, int myChunk, int otherChunk);
        public void Collide(PhysicalObject otherObject, int myChunk, int otherChunk)
        {
            orig_Collide(otherObject, myChunk, otherChunk);
            if(this.playerState.slugcatCharacter == 3) // TRANSFER CAT CHARACTER NUMBER
            {
                if (otherObject is Creature && otherObject != this.thrownBy && this.chargedActive && this.stunDelay == 0)
                {
                    Debug.Log("Shocking dead creature: " + (otherObject as Creature).dead);
                    if (!(otherObject is BigEel) && !(otherObject is Centipede) && !(otherObject is Fly) && !(otherObject as Creature).dead && !(otherObject is Player))
                    {
                        (otherObject as Creature).Violence(base.firstChunk, new Vector2?(Custom.DirVec(base.firstChunk.pos, otherObject.bodyChunks[otherChunk].pos) * 5f), otherObject.bodyChunks[otherChunk], null, Creature.DamageType.Electric, 0.8f, (!(otherObject is Player)) ? (270f * Mathf.Lerp((otherObject as Creature).Template.baseStunResistance, 1f, 0.5f)) : 140f);
                        this.room.AddObject(new CreatureSpasmer(otherObject as Creature, false, (otherObject as Creature).stun));
                        this.stunDelay = 300;
                        this.room.PlaySound(SoundID.Jelly_Fish_Tentacle_Stun, base.firstChunk.pos);
                        this.room.AddObject(new Explosion.ExplosionLight(base.firstChunk.pos, 150f, 0.85f, 4, new Color(0.7f, 0.7f, 1f)));
                        this.chargedTimer -= 20;
                    }                  
                }
            }           
        }
        public float lightFlash;
        public float buttonPressed;
        public float chargedTimer;
        public bool chargedActive;
        public bool antennaActive;
        public bool receivingMessage;
        public int animationCount;
        public int animationDelay;
        public float stunDelay;
        public WorldCoordinate preCoord;
        public Creature thrownBy;
    }
    [MonoModPatch("global::ZapCoil")]
    public class patch_ZapCoil : ZapCoil
    {
        [MonoModIgnore]
        public patch_ZapCoil(IntRect rect, Room room) :base(rect, room) { }
        [MonoModIgnore]
        public extern void orig_ctor(IntRect rect, Room room);
        [MonoModLinkTo("UpdatableAndDeletable", "System.Void Update(System.Boolean)")]
        public static void base_Update(bool eu) { }
        public extern void orig_Update(bool eu);
        public void Update(bool eu)
        {
            this.preTurnedOn = this.turnedOn;
            this.turnedOn = 0f;
            orig_Update(eu);
            this.turnedOn = preTurnedOn;
            if (this.turnedOn > 0.5f)
            {
                for (int i = 0; i < this.room.physicalObjects.Length; i++)
                {
                    for (int j = 0; j < this.room.physicalObjects[i].Count; j++)
                    {
                        for (int k = 0; k < this.room.physicalObjects[i][j].bodyChunks.Length; k++)
                        {
                            if ((this.horizontalAlignment && this.room.physicalObjects[i][j].bodyChunks[k].ContactPoint.y != 0) || (!this.horizontalAlignment && this.room.physicalObjects[i][j].bodyChunks[k].ContactPoint.x != 0))
                            {
                                Vector2 a = this.room.physicalObjects[i][j].bodyChunks[k].ContactPoint.ToVector2();
                                Vector2 v = this.room.physicalObjects[i][j].bodyChunks[k].pos + a * (this.room.physicalObjects[i][j].bodyChunks[k].rad + 30f);
                                if (this.GetFloatRect.Vector2Inside(v))
                                {
                                    this.room.AddObject(new ZapCoil.ZapFlash(this.room.physicalObjects[i][j].bodyChunks[k].pos + a * this.room.physicalObjects[i][j].bodyChunks[k].rad, Mathf.InverseLerp(-0.05f, 15f, this.room.physicalObjects[i][j].bodyChunks[k].rad)));
                                    this.room.physicalObjects[i][j].bodyChunks[k].vel -= (a * 6f + Custom.RNV() * UnityEngine.Random.value) / this.room.physicalObjects[i][j].bodyChunks[k].mass;
                                    this.disruption = Mathf.Max(this.disruption, Mathf.InverseLerp(-0.05f, 9f, this.room.physicalObjects[i][j].bodyChunks[k].rad) + UnityEngine.Random.value * 0.5f);
                                    this.smoothDisruption = this.disruption;
                                    if (this.room.physicalObjects[i][j] is Creature)
                                    {
                                        //Debug.Log("IS CREATURE");
                                        if (this.room.physicalObjects[i][j] is patch_Player)
                                        {
                                            if ((this.room.physicalObjects[i][j] as patch_Player).playerState.slugcatCharacter != 3)
                                            {
                                                //Debug.Log("CREATURE SLUG CHARA not THREE");
                                                (this.room.physicalObjects[i][j] as patch_Player).Die();
                                            }
                                            else
                                            {
                                                //Debug.Log("CREATURE SLUG CHARA THREE");
                                                (this.room.physicalObjects[i][j] as patch_Player).EnterChargedModeNoToll();
                                            }
                                        }
                                        else
                                        {
                                            //Debug.Log("CREATURE NOT SLUG");
                                            (this.room.physicalObjects[i][j] as Creature).Die();
                                        }                                        
                                    }
                                    if (UnityEngine.Random.value < this.disruption && UnityEngine.Random.value < 0.5f)
                                    {
                                        this.turnedOffCounter = UnityEngine.Random.Range(2, 15);
                                    }
                                    this.room.PlaySound(SoundID.Zapper_Zap, this.room.physicalObjects[i][j].bodyChunks[k].pos, 1f, 1f);
                                    this.zapLit = 1f;
                                }
                            }
                        }
                    }
                }
            }
            this.lastTurnedOn = this.turnedOn;
            if (UnityEngine.Random.value < 0.005f)
            {
                this.disruption = Mathf.Max(this.disruption, UnityEngine.Random.value);
            }
            this.disruption = Mathf.Max(0f, this.disruption - 1f / Mathf.Lerp(70f, 300f, UnityEngine.Random.value));
            this.smoothDisruption = Mathf.Lerp(this.smoothDisruption, this.disruption, 0.2f);
            float num = Mathf.InverseLerp(0.1f, 1f, this.smoothDisruption);
            this.soundLoop.Volume = (1f - num) * this.turnedOn;
            this.disruptedLoop.Volume = num * Mathf.Pow(this.turnedOn, 0.2f);
            for (int l = 0; l < this.flicker.GetLength(0); l++)
            {
                this.flicker[l, 1] = this.flicker[l, 0];
                this.flicker[l, 3] = Mathf.Clamp(this.flicker[l, 3] + Mathf.Lerp(-1f, 1f, UnityEngine.Random.value) / 10f, 0f, 1f);
                this.flicker[l, 2] += 1f / Mathf.Lerp(70f, 20f, this.flicker[l, 3]);
                this.flicker[l, 0] = Mathf.Clamp(0.5f + this.smoothDisruption * (Mathf.Lerp(0.2f, 0.1f, this.flicker[l, 3]) * Mathf.Sin(6.28318548f * this.flicker[l, 2]) + Mathf.Lerp(-1f, 1f, UnityEngine.Random.value) / 20f), 0f, 1f);
            }
            if (UnityEngine.Random.value < this.disruption && UnityEngine.Random.value < 0.0025f)
            {
                this.turnedOffCounter = UnityEngine.Random.Range(10, 100);
            }
            if (!this.powered)
            {
                this.turnedOn = Mathf.Max(0f, this.turnedOn - 0.1f);
            }
            if (this.turnedOffCounter > 0)
            {
                this.turnedOffCounter--;
                if (UnityEngine.Random.value < 0.5f || UnityEngine.Random.value > this.disruption || !this.powered)
                {
                    this.turnedOn = 0f;
                }
                else
                {
                    this.turnedOn = UnityEngine.Random.value;
                }
                if (this.powered)
                {
                    this.turnedOn = Mathf.Lerp(this.turnedOn, 1f, this.zapLit * UnityEngine.Random.value);
                }
                this.smoothDisruption = 1f;
            }
            else if (this.powered)
            {
                this.turnedOn = Mathf.Min(this.turnedOn + UnityEngine.Random.value / 30f, 1f);
            }
            this.zapLit = Mathf.Max(0f, this.zapLit - 0.1f);
            if (this.room.fullyLoaded)
            {
                this.disruption = Mathf.Max(this.disruption, this.room.gravity);
            }
            if (this.room.roomSettings.GetEffectAmount(RoomSettings.RoomEffect.Type.BrokenZeroG) > 0f)
            {
                bool flag = this.room.world.rainCycle.brokenAntiGrav.to == 1f && this.room.world.rainCycle.brokenAntiGrav.progress == 1f;
                if (!flag)
                {
                    this.disruption = 1f;
                    if (this.powered && UnityEngine.Random.value < 0.2f)
                    {
                        this.powered = false;
                    }
                }
                if (flag && !this.powered && UnityEngine.Random.value < 0.025f)
                {
                    this.powered = true;
                }
            }
        }
        public float preTurnedOn;
        [MonoModIgnore]
        private patch_Player player;
    }
    [MonoModPatch("global::Centipede")]
    public class patch_Centipede : Centipede
    {
        [MonoModIgnore]
        patch_Centipede(AbstractCreature abstractCreature, World world) : base(abstractCreature, world) { }
        [MonoModIgnore]
        public extern void orig_ctor(AbstractCreature abstractCreature, World world);
        public extern void orig_Shock(PhysicalObject shockObj);
        public void Shock(PhysicalObject shockObj)
        {
            this.prePhyObj = shockObj;
            if(shockObj is Player)
            {
                if (base.graphicsModule != null)
                {
                    (base.graphicsModule as CentipedeGraphics).lightFlash = 1f;
                    for (int i = 0; i < (int)Mathf.Lerp(4f, 8f, this.size); i++)
                    {
                        this.room.AddObject(new Spark(this.HeadChunk.pos, Custom.RNV() * Mathf.Lerp(4f, 14f, UnityEngine.Random.value), new Color(0.7f, 0.7f, 1f), null, 8, 14));
                    }
                }
                for (int j = 0; j < base.bodyChunks.Length; j++)
                {
                    base.bodyChunks[j].vel += Custom.RNV() * 6f * UnityEngine.Random.value;
                    base.bodyChunks[j].pos += Custom.RNV() * 6f * UnityEngine.Random.value;
                }
                for (int k = 0; k < prePhyObj.bodyChunks.Length; k++)
                {
                    prePhyObj.bodyChunks[k].vel += Custom.RNV() * 6f * UnityEngine.Random.value;
                    prePhyObj.bodyChunks[k].pos += Custom.RNV() * 6f * UnityEngine.Random.value;
                }
                if ((prePhyObj as Player).playerState.slugcatCharacter == 3)//TRANSFER SLUGCAT
                {
                    if (this.Red)
                    {
                        (this.prePhyObj as patch_Player).EnterChargedModeNoToll();

                    }
                    else
                    {
                        (this.prePhyObj as patch_Player).EnterChargedModeNoToll();
                        this.Die();
                    }
                    
                }
                else
                {
                    (prePhyObj as Creature).Die();
                    this.room.AddObject(new CreatureSpasmer(prePhyObj as Player, true, (int)Mathf.Lerp(70f, 120f, this.size)));
                }
            }
            else
            {
                orig_Shock(shockObj);
            }
            
        }
        PhysicalObject prePhyObj;
    }
    [MonoModPatch("global::JellyFish")]
    public class patch_JellyFish : JellyFish
    {
        [MonoModIgnore]
        public patch_JellyFish(AbstractPhysicalObject abstractPhysicalObject) : base(abstractPhysicalObject) { }
        [MonoModIgnore]
        public extern void orig_ctor(AbstractPhysicalObject abstractPhysicalObject);
        public extern void orig_BitByPlayer(Creature.Grasp grasp, bool eu);
        public void BitByPlayer(Creature.Grasp grasp, bool eu)
        {
            if((grasp.grabber as patch_Player).playerState.slugcatCharacter == 3)//TRANSFER SLUGCAT
            {
                this.bites--;
                this.room.PlaySound((this.bites != 0) ? SoundID.Slugcat_Bite_Jelly_Fish : SoundID.Slugcat_Eat_Jelly_Fish, base.firstChunk.pos);
                base.firstChunk.MoveFromOutsideMyUpdate(eu, grasp.grabber.mainBodyChunk.pos);
                if (!this.AbstrConsumable.isConsumed)
                {
                    this.AbstrConsumable.Consume();
                }
                for (int i = 0; i < this.tentacles.Length; i++)
                {
                    for (int j = 0; j < this.tentacles[i].GetLength(0); j++)
                    {
                        this.tentacles[i][j, 0] = Vector2.Lerp(this.tentacles[i][j, 0], base.firstChunk.pos, 0.2f);
                    }
                }
                if (this.bites < 1)
                {
                    (grasp.grabber as patch_Player).EnterChargedModeNoToll();
                    grasp.Release();
                    this.Destroy();
                }
            }
            else
            {
                orig_BitByPlayer(grasp, eu);
            }
        }

    }
    [MonoModPatch("global::LanternMouse")]
    public class patch_LanternMouse : LanternMouse
    {
        [MonoModIgnore]
        patch_LanternMouse(AbstractCreature abstractCreature, World world) : base(abstractCreature, world) { }
        [MonoModIgnore]
        public extern void orig_ctor(AbstractCreature abstractCreature, World world);
        public extern void orig_Carried();
        public void Carried()
        {
            if (this.grabbedBy[0].grabber is patch_Player)
            {
                if ((this.grabbedBy[0].grabber as patch_Player).playerState.slugcatCharacter == 3)//TRANSFER CAT
                {
                    if (base.dead)
                    {
                        return;
                    }
                    bool flag = this.room.aimap.TileAccessibleToCreature(base.mainBodyChunk.pos, base.Template) || this.room.aimap.TileAccessibleToCreature(base.bodyChunks[1].pos, base.Template);
                    if (this.grabbedBy[0].grabber is Player && ((this.grabbedBy[0].grabber as Player).input[0].x != 0 || (this.grabbedBy[0].grabber as Player).input[0].y != 0))
                    {
                        flag = false;
                    }
                    if (flag)
                    {
                        this.struggleCountdownA--;
                        if (this.struggleCountdownA < 0)
                        {
                            if (UnityEngine.Random.value < 0.008333334f)
                            {
                                this.struggleCountdownA = UnityEngine.Random.Range(20, 400);
                            }
                            for (int i = 0; i < 2; i++)
                            {
                                base.bodyChunks[i].vel += Custom.DegToVec(UnityEngine.Random.value * 360f) * 6f * UnityEngine.Random.value;
                            }
                        }
                    }
                    this.struggleCountdownB--;
                    if (this.struggleCountdownB < 0 && UnityEngine.Random.value < 0.008333334f)
                    {
                        this.struggleCountdownB = UnityEngine.Random.Range(10, 100);
                    }
                    if (!base.dead && base.graphicsModule != null && (this.struggleCountdownA < 0 || this.struggleCountdownB < 0))
                    {
                        if (UnityEngine.Random.value < 0.025f)
                        {
                            (base.graphicsModule as MouseGraphics).ResetUnconsiousProfile();
                            this.Die();
                            (this.grabbedBy[0].grabber as patch_Player).EnterChargedModeNoToll();
                        }
                        for (int j = 0; j < base.graphicsModule.bodyParts.Length; j++)
                        {
                            base.graphicsModule.bodyParts[j].pos += Custom.DegToVec(UnityEngine.Random.value * 360f) * 3f * UnityEngine.Random.value;
                            base.graphicsModule.bodyParts[j].vel += Custom.DegToVec(UnityEngine.Random.value * 360f) * 6f * UnityEngine.Random.value;
                        }
                    }
                }
                else
                {
                    orig_Carried();
                }
            }
            else
            {
                orig_Carried();
            }
            
        }
    }
    [MonoModPatch("global::ElectricDeath")]
    public class patch_ElectricDeath : ElectricDeath
    {
        [MonoModIgnore]
        patch_ElectricDeath(RoomSettings.RoomEffect effect, Room room) :base(effect, room) { }
        [MonoModIgnore]
        public extern void orig_ctor(RoomSettings.RoomEffect effect, Room room);
        [MonoModLinkTo("UpdatableAndDeletable", "System.Void Update(System.Boolean)")]
        public void base_Update(bool eu) { }
        public extern void orig_Update(bool eu);
        public void Update(bool eu)
        {
            this.base_Update(eu);
            if (this.Intensity == 0f)
            {
                return;
            }
            if (this.soundLoop == null)
            {
                this.soundLoop = new DisembodiedDynamicSoundLoop(this);
                this.soundLoop.sound = SoundID.Death_Lightning_Heavy_Lightning_LOOP;
            }
            else
            {
                this.soundLoop.Update();
                this.soundLoop.Volume = this.Intensity;
            }
            if (this.soundLoop2 == null)
            {
                this.soundLoop2 = new DisembodiedDynamicSoundLoop(this);
                this.soundLoop2.sound = SoundID.Death_Lightning_Early_Sizzle_LOOP;
            }
            else
            {
                this.soundLoop2.Update();
                this.soundLoop2.Volume = Mathf.Pow(this.Intensity, 0.1f) * Mathf.Lerp(0.5f + 0.5f * Mathf.Sin(this.sin * 3.14159274f * 2f), 0f, Mathf.Pow(this.Intensity, 8f));
            }
            this.lastSin = this.sin;
            this.sin += this.Intensity * 0.1f;
            if (this.closeToWallTiles != null && this.room.BeingViewed && UnityEngine.Random.value < Mathf.InverseLerp(1000f, 9120f, (float)(this.room.TileWidth * this.room.TileHeight)) * this.Intensity)
            {
                IntVector2 pos = this.closeToWallTiles[UnityEngine.Random.Range(0, this.closeToWallTiles.Count)];
                Vector2 pos2 = this.room.MiddleOfTile(pos) + new Vector2(Mathf.Lerp(-10f, 10f, UnityEngine.Random.value), Mathf.Lerp(-10f, 10f, UnityEngine.Random.value));
                float num = UnityEngine.Random.value * this.Intensity;
                if (this.room.ViewedByAnyCamera(pos2, 50f))
                {
                    this.room.AddObject(new ElectricDeath.SparkFlash(pos2, num));
                }
                this.room.PlaySound(SoundID.Death_Lightning_Spark_Spontaneous, pos2, num, 1f);
            }
            for (int i = 1; i < this.flashes.Length; i++)
            {
                this.flashes[i].Update();
            }
            this.lastColor = this.color;
            this.color = Vector3.Lerp(this.color, this.getToColor, UnityEngine.Random.value * 0.3f);
            if (UnityEngine.Random.value < 0.333333343f)
            {
                this.getToColor.x = UnityEngine.Random.value;
            }
            else if (UnityEngine.Random.value < 0.333333343f)
            {
                this.getToColor.y = UnityEngine.Random.value;
            }
            else if (UnityEngine.Random.value < 0.333333343f)
            {
                this.getToColor.z = UnityEngine.Random.value;
            }
            if (this.Intensity > 0.5f && UnityEngine.Random.value < Custom.LerpMap(this.Intensity, 0.5f, 1f, 0f, 0.5f))
            {
                for (int j = 0; j < this.room.physicalObjects.Length; j++)
                {
                    for (int k = 0; k < this.room.physicalObjects[j].Count; k++)
                    {
                        for (int l = 0; l < this.room.physicalObjects[j][k].bodyChunks.Length; l++)
                        {
                            if (UnityEngine.Random.value < Custom.LerpMap(this.Intensity, 0.5f, 1f, 0f, 0.5f) && (this.room.physicalObjects[j][k].bodyChunks[l].ContactPoint.x != 0 || this.room.physicalObjects[j][k].bodyChunks[l].ContactPoint.y != 0 || this.room.GetTile(this.room.physicalObjects[j][k].bodyChunks[l].pos).AnyBeam))
                            {
                                float num2 = Mathf.Pow(UnityEngine.Random.value, 0.9f) * Mathf.InverseLerp(0.5f, 1f, this.Intensity);
                                this.room.AddObject(new ElectricDeath.SparkFlash(this.room.physicalObjects[j][k].bodyChunks[l].pos + this.room.physicalObjects[j][k].bodyChunks[l].rad * this.room.physicalObjects[j][k].bodyChunks[l].ContactPoint.ToVector2(), Mathf.Pow(num2, 0.5f)));
                                
                                if (this.room.physicalObjects[j][k] is Creature)
                                {
                                    if (this.room.physicalObjects[j][k] is patch_Player)
                                    {
                                        if ((this.room.physicalObjects[j][k] as patch_Player).playerState.slugcatCharacter == 3)
                                        {
                                            (this.room.physicalObjects[j][k] as patch_Player).EnterChargedModeNoToll();
                                        }
                                        else
                                        {
                                            Vector2 vector = -(this.room.physicalObjects[j][k].bodyChunks[l].ContactPoint.ToVector2() + Custom.RNV()).normalized;
                                            vector *= 22f * num2 / this.room.physicalObjects[j][k].bodyChunks[l].mass;
                                            this.room.physicalObjects[j][k].bodyChunks[l].vel += vector;
                                            this.room.physicalObjects[j][k].bodyChunks[l].pos += vector;
                                            this.room.PlaySound(SoundID.Death_Lightning_Spark_Object, this.room.physicalObjects[j][k].bodyChunks[l].pos, num2, 1f);
                                            (this.room.physicalObjects[j][k] as Creature).Violence(null, null, this.room.physicalObjects[j][k].bodyChunks[l], null, Creature.DamageType.Electric, num2 * 1.8f, num2 * 40f);
                                        }
                                    }
                                    else
                                    {
                                        Vector2 vector = -(this.room.physicalObjects[j][k].bodyChunks[l].ContactPoint.ToVector2() + Custom.RNV()).normalized;
                                        vector *= 22f * num2 / this.room.physicalObjects[j][k].bodyChunks[l].mass;
                                        this.room.physicalObjects[j][k].bodyChunks[l].vel += vector;
                                        this.room.physicalObjects[j][k].bodyChunks[l].pos += vector;
                                        this.room.PlaySound(SoundID.Death_Lightning_Spark_Object, this.room.physicalObjects[j][k].bodyChunks[l].pos, num2, 1f);
                                        (this.room.physicalObjects[j][k] as Creature).Violence(null, null, this.room.physicalObjects[j][k].bodyChunks[l], null, Creature.DamageType.Electric, num2 * 1.8f, num2 * 40f);
                                    }

                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
namespace HUD
{
    [MonoModPatch("global::HUD.FoodMeter")]
    public class patch_FoodMeter : FoodMeter
    {
       [MonoModIgnore]
       patch_FoodMeter(HUD hud, int maxFood, int survivalLimit) : base(hud, maxFood, survivalLimit)
        {
        }
        public extern void orig_ctor(HUD hud, int maxFood, int survivalLimit);
         public void ctor(HUD hud, int maxFood, int survivalLimit)
        {
            
            orig_ctor(hud, maxFood, survivalLimit);
        }
        public extern void orig_UpdateShowCount();
        public void UpdateShowCount()
        {
            
            if (this.showCount > this.hud.owner.CurrentFood)
            {
                if (this.eatCircleDelay == 0)
                {
                    this.eatCircleDelay = 20;
                }
                this.eatCircleDelay--;
                //Debug.Log("eat circle show count hod owner---------------------------");
                if (this.eatCircleDelay < 1)
                {
                    //Debug.Log("eat circle delay ---------------------------");
                    this.circles[this.showCount - 1].EatFade();
                    this.eatCircles--;
                    if (this.showCount >= this.hud.owner.CurrentFood)
                    {
                        //Debug.Log("show count---------------------------");
                        this.showCount--;
                    }
                    this.eatCircleDelay = 0;
                }
            }
            else
            {
                orig_UpdateShowCount();
            }
        }
    }
}