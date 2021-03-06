﻿using TGC.Core.Direct3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using TGC.Core.Text;
using TGC.Core.Mathematica;
using TGC.Examples.Engine2D.Spaceship.Core;
using TGC.Core.Input;
using Microsoft.DirectX.DirectInput;
using TGC.Group.Model.Crafting;
using System.Windows.Forms;
using TGC.Core.Sound;
using TGC.Group.Model.Sounds;

namespace TGC.Group.Model.Gui
{
    static class Hud
    {
        public enum Status {
            MainMenu, //This state only works at the start of the game
            Inventory,
            Crafting,
            GameOver, //Player changes to this state when IsDead();
            Gameplay,
            Instructions,
            Win
        };

        struct ItemSprite
        {
            public CustomSprite icon;
            public CustomSprite background;
            public TgcText2D amount;
            public void Dispose() {
                background.Dispose();
                amount.Dispose();
                if (icon != null)
                    icon.Dispose();
            }
        }

        static TgcStaticSound sonido;

        static Player Player = Player.Instance();

        //Control vars
        static Status CurrentStatus = Status.Gameplay;
        static Status LastStatus = Status.Gameplay;

        static TgcText2D SelectedText;
        static int SelectedItemIndex;

        //"Consts"
        static int WIDTH;
        static int HEIGHT;
        const int MAX_INVENTORY_ITEMS = 20; //Deberia ser multiplo de 10.
        const int MAX_CRAFTING_ITEMS = 10;

        //Drawing vars
        static Drawer2D drawer;
        static TgcText2D Start;
        static TgcText2D Instruccion;
        static TgcText2D Exit;

        static CustomSprite Logo;
        static CustomSprite OverlayInv; //Overlay de inventario
        static CustomSprite OverlayCraft;
        static CustomSprite HealthBar;
        static CustomSprite OxygenBar;

        static CustomSprite ItemBackgroundPreset;
        static List<ItemSprite> InventoryItems;
        static List<ItemSprite> CraftingItems;

        static TgcText2D GameOver;
        static TgcText2D GameOverSubtitle;

        static TgcText2D YouWin;
        static TgcText2D YouWinSubtitle;

        static List<TgcText2D> instrucciones;

        static string MediaDir;

        static private bool presionoUp = false;
        static private bool presionoDown = false;
        static private bool presionoLeft = false;
        static private bool presionoRight = false;
        static private bool presionoEnter = false;

        public static void Init(string mediaDir)
        {
            WIDTH = D3DDevice.Instance.Width;
            HEIGHT = D3DDevice.Instance.Height;

            MediaDir = mediaDir;

            InventoryItems = new List<ItemSprite>();
            CraftingItems = new List<ItemSprite>();
            SelectedItemIndex = 0;


            sonido = new TgcStaticSound();
            sonido.loadSound(SoundsManager.Instance().mediaDir + "Sounds\\grab.wav", SoundsManager.Instance().sound);

            drawer = new Drawer2D();

            //Main Menu
            Logo = new CustomSprite();
            Logo.Bitmap = new CustomBitmap(MediaDir + "logo_subnautica.png", D3DDevice.Instance.Device);
            var spriteSize = Logo.Bitmap.Size;
            Logo.Position = new TGCVector2(WIDTH / 2 - spriteSize.Width / 2, Round(HEIGHT * 0.2f) - spriteSize.Height / 2);

            Start = new TgcText2D();
            Start.Text = "Start";
            Start.Align = TgcText2D.TextAlign.LEFT;
            Start.Position = new Point(Round(WIDTH * 0.45f), Round(HEIGHT * 0.7f));
            Start.Size = new Size(300, 100);
            Start.changeFont(new Font("Calibri", 35, FontStyle.Bold));
            Start.Color = Color.White;

            Instruccion = new TgcText2D();
            Instruccion.Text = "Instructions";
            Instruccion.Align = TgcText2D.TextAlign.LEFT;
            Instruccion.Position = new Point(Round(WIDTH * 0.45f), Round(HEIGHT * 0.75f));
            Instruccion.Size = new Size(300, 100);
            Instruccion.changeFont(new Font("Calibri", 35, FontStyle.Bold));
            Instruccion.Color = Color.White;

            Exit = new TgcText2D();
            Exit.Text = "Exit";
            Exit.Align = TgcText2D.TextAlign.LEFT;
            Exit.Position = new Point(Round(WIDTH * 0.45f), Round(HEIGHT * 0.8f));
            Exit.Size = new Size(300, 100);
            Exit.changeFont(new Font("Calibri", 35, FontStyle.Bold));
            Exit.Color = Color.White;


            instrucciones = new List<TgcText2D>();

            TgcText2D instruccionesItem = new TgcText2D();
            instruccionesItem.Text = "- Press A S D W to move";
            instruccionesItem.Align = TgcText2D.TextAlign.LEFT;
            instruccionesItem.Position = new Point(Round(WIDTH * 0.2f), Round(HEIGHT * 0.4f));
            instruccionesItem.Size = new Size(WIDTH - 200, 100);
            instruccionesItem.changeFont(new Font("Calibri", 25, FontStyle.Bold));
            instruccionesItem.Color = Color.White;
            instrucciones.Add(instruccionesItem);
            
            instruccionesItem = new TgcText2D();
            instruccionesItem.Text = "- Press O to go inside/outside the ship";
            instruccionesItem.Align = TgcText2D.TextAlign.LEFT;
            instruccionesItem.Position = new Point(Round(WIDTH * 0.2f), Round(HEIGHT * 0.45f));
            instruccionesItem.Size = new Size(WIDTH - 200, 100);
            instruccionesItem.changeFont(new Font("Calibri", 25, FontStyle.Bold));
            instruccionesItem.Color = Color.White;
            instrucciones.Add(instruccionesItem);

            instruccionesItem = new TgcText2D();
            instruccionesItem.Text = "- Press I to show/hide the inventory";
            instruccionesItem.Align = TgcText2D.TextAlign.LEFT;
            instruccionesItem.Position = new Point(Round(WIDTH * 0.2f), Round(HEIGHT * 0.5f));
            instruccionesItem.Size = new Size(WIDTH - 200, 100);
            instruccionesItem.changeFont(new Font("Calibri", 25, FontStyle.Bold));
            instruccionesItem.Color = Color.White;
            instrucciones.Add(instruccionesItem);

            instruccionesItem = new TgcText2D();
            instruccionesItem.Text = "- Press P to enter developer mode";
            instruccionesItem.Align = TgcText2D.TextAlign.LEFT;
            instruccionesItem.Position = new Point(Round(WIDTH * 0.2f), Round(HEIGHT * 0.55f));
            instruccionesItem.Size = new Size(WIDTH - 200, 100);
            instruccionesItem.changeFont(new Font("Calibri", 25, FontStyle.Bold));
            instruccionesItem.Color = Color.White;
            instrucciones.Add(instruccionesItem);

            instruccionesItem = new TgcText2D();
            instruccionesItem.Text = "- Collect as many items as you can by clicking on them";
            instruccionesItem.Align = TgcText2D.TextAlign.LEFT;
            instruccionesItem.Position = new Point(Round(WIDTH * 0.2f), Round(HEIGHT * 0.6f));
            instruccionesItem.Size = new Size(WIDTH - 200, 100);
            instruccionesItem.changeFont(new Font("Calibri", 25, FontStyle.Bold));
            instruccionesItem.Color = Color.White;
            instrucciones.Add(instruccionesItem);

            instruccionesItem = new TgcText2D();
            instruccionesItem.Text = "- Be careful with the shark! In order to kill it, you must have a weapon";
            instruccionesItem.Align = TgcText2D.TextAlign.LEFT;
            instruccionesItem.Position = new Point(Round(WIDTH * 0.2f), Round(HEIGHT * 0.65f));
            instruccionesItem.Size = new Size(WIDTH - 200, 100);
            instruccionesItem.changeFont(new Font("Calibri", 25, FontStyle.Bold));
            instruccionesItem.Color = Color.White;
            instrucciones.Add(instruccionesItem);

            instruccionesItem = new TgcText2D();
            instruccionesItem.Text = "- To get a weapon, go to the inventory and trade it for items";
            instruccionesItem.Align = TgcText2D.TextAlign.LEFT;
            instruccionesItem.Position = new Point(Round(WIDTH * 0.2f), Round(HEIGHT * 0.7f));
            instruccionesItem.Size = new Size(WIDTH - 200, 100);
            instruccionesItem.changeFont(new Font("Calibri", 25, FontStyle.Bold));
            instruccionesItem.Color = Color.White;
            instrucciones.Add(instruccionesItem);

            instruccionesItem = new TgcText2D();
            instruccionesItem.Text = "- To recover oxygen, go inside the ship or the ocean's surface";
            instruccionesItem.Align = TgcText2D.TextAlign.LEFT;
            instruccionesItem.Position = new Point(Round(WIDTH * 0.2f), Round(HEIGHT * 0.75f));
            instruccionesItem.Size = new Size(WIDTH - 200, 100);
            instruccionesItem.changeFont(new Font("Calibri", 25, FontStyle.Bold));
            instruccionesItem.Color = Color.White;
            instrucciones.Add(instruccionesItem);

            instruccionesItem = new TgcText2D();
            instruccionesItem.Text = "Press Enter to go back to the menu";
            instruccionesItem.Align = TgcText2D.TextAlign.LEFT;
            instruccionesItem.Position = new Point(Round(WIDTH * 0.2f), Round(HEIGHT * 0.8f));
            instruccionesItem.Size = new Size(WIDTH - 200, 100);
            instruccionesItem.changeFont(new Font("Calibri", 25, FontStyle.Bold));
            instruccionesItem.Color = Color.White;
            instrucciones.Add(instruccionesItem);

            SelectedText = Start;

            //Gameplay
            HealthBar = new CustomSprite();
            HealthBar.Bitmap = new CustomBitmap(MediaDir + "bar_health.png", D3DDevice.Instance.Device);
            spriteSize = HealthBar.Bitmap.Size;
            HealthBar.Position = new TGCVector2(WIDTH / 2 - spriteSize.Width / 2, Round(HEIGHT * 0.85f));

            OxygenBar = new CustomSprite();
            OxygenBar.Bitmap = new CustomBitmap(MediaDir + "bar_oxygen.png", D3DDevice.Instance.Device);
            OxygenBar.Position = HealthBar.Position + new TGCVector2(0, 25 + spriteSize.Height);



            //Inventory
            OverlayInv = new CustomSprite();
            OverlayInv.Bitmap = new CustomBitmap(MediaDir + "overlay_inv.png", D3DDevice.Instance.Device);
            spriteSize = OverlayInv.Bitmap.Size;
            OverlayInv.Position = new TGCVector2(WIDTH / 2 - spriteSize.Width / 2, HEIGHT / 2 - spriteSize.Height / 2);

            //Load item background preset to be used on every item sprite
            ItemBackgroundPreset = new CustomSprite();
            ItemBackgroundPreset.Bitmap = new CustomBitmap(MediaDir + "item_placeholder2.png", D3DDevice.Instance.Device);
            spriteSize = ItemBackgroundPreset.Bitmap.Size;

            //10 items per line
            for (int j = 1; j <= MAX_INVENTORY_ITEMS / 10; j++)
            {
                for (int i = 0; i <= 9; i++)
                {
                    var item = new ItemSprite();

                    item.background = new CustomSprite();
                    item.background.Bitmap = ItemBackgroundPreset.Bitmap;
                    var xoffset = 45 + i * spriteSize.Width * 1.5f;
                    var yoffset = 25 + j * spriteSize.Height * 1.5f;
                    item.background.Position = OverlayInv.Position + new TGCVector2(xoffset, yoffset);
                    item.background.Color = Color.CadetBlue;

                    item.amount = new TgcText2D();
                    item.amount.Align = TgcText2D.TextAlign.LEFT;
                    item.amount.Text = "";
                    var xpos = Round(item.background.Position.X + spriteSize.Width / 3);
                    var ypos = Round(item.background.Position.Y + spriteSize.Height / 2);
                    item.amount.Position = new Point(xpos, ypos);
                    item.amount.Size = new Size(50, 15);
                    item.amount.changeFont(new Font("Calibri", 15, FontStyle.Bold));
                    item.amount.Color = Color.White;

                    item.icon = new CustomSprite();
                    item.icon.Position = item.background.Position;

                    InventoryItems.Add(item);
                }
            }

            //Crafting
            OverlayCraft = new CustomSprite();
            OverlayCraft.Bitmap = new CustomBitmap(MediaDir + "overlay_craft.png", D3DDevice.Instance.Device);
            OverlayCraft.Position = OverlayInv.Position;

            for (int i = 0; i < MAX_CRAFTING_ITEMS; i++)
            {
                var item = new ItemSprite();

                item.background = new CustomSprite();
                item.background.Bitmap = ItemBackgroundPreset.Bitmap;
                var xoffset = 45 + i * spriteSize.Width * 1.5f;
                var yoffset = Round(OverlayCraft.Bitmap.Size.Height * 0.8f);
                item.background.Position = OverlayCraft.Position + new TGCVector2(xoffset, yoffset);
                item.background.Color = Color.CadetBlue;

                item.amount = new TgcText2D();
                item.amount.Text = "";

                item.icon = new CustomSprite();
                item.icon.Position = item.background.Position;

                CraftingItems.Add(item);
            }


            //GameOver
            GameOver = new TgcText2D();
            GameOver.Text = "GAME OVER";
            GameOver.Align = TgcText2D.TextAlign.LEFT;
            GameOver.Position = new Point(Round(WIDTH * 0.28f), Round(HEIGHT * 0.4f));
            GameOver.Size = new Size(750, 500);
            GameOver.changeFont(new Font("Calibri", 100, FontStyle.Bold));
            GameOver.Color = Color.Red;

            GameOverSubtitle = new TgcText2D();
            GameOverSubtitle.Text = "Press Enter to exit";
            GameOverSubtitle.Align = TgcText2D.TextAlign.LEFT;
            GameOverSubtitle.Position = new Point(Round(WIDTH * 0.4f), Round(HEIGHT * 0.7f));
            GameOverSubtitle.Size = new Size(350, 200);
            GameOverSubtitle.changeFont(new Font("Calibri", 30, FontStyle.Bold));
            GameOverSubtitle.Color = Color.Red;


            //Win
            YouWin = new TgcText2D();
            YouWin.Text = "YOU WIN!";
            YouWin.Align = TgcText2D.TextAlign.LEFT;
            YouWin.Position = new Point(Round(WIDTH * 0.3f), Round(HEIGHT * 0.4f));
            YouWin.Size = new Size(750, 500);
            YouWin.changeFont(new Font("Calibri", 100, FontStyle.Bold));
            YouWin.Color = Color.Green;

            YouWinSubtitle = new TgcText2D();
            YouWinSubtitle.Text = "Press Enter to exit";
            YouWinSubtitle.Align = TgcText2D.TextAlign.LEFT;
            YouWinSubtitle.Position = new Point(Round(WIDTH * 0.4f), Round(HEIGHT * 0.7f));
            YouWinSubtitle.Size = new Size(350, 200);
            YouWinSubtitle.changeFont(new Font("Calibri", 30, FontStyle.Bold));
            YouWinSubtitle.Color = Color.Green;
        }

        public static void Update(TgcD3dInput Input)
        {
            bool up = Input.keyDown(Key.Up);
            bool down = Input.keyDown(Key.Down);
            bool left = Input.keyDown(Key.Left);
            bool right = Input.keyDown(Key.Right);
            bool enter = Input.keyDown(Key.Return);
            if (up)
            {
                up = false;
                presionoUp = true;
            }
            else
            {
                if (presionoUp)
                {
                    up = true;
                    presionoUp = false;
                }
            }

            if (down)
            {
                down = false;
                presionoDown = true;
            }
            else
            {
                if (presionoDown)
                {
                    down = true;
                    presionoDown = false;
                }
            }

            if (left)
            {
                left = false;
                presionoLeft = true;
            }
            else
            {
                if (presionoLeft)
                {
                    left = true;
                    presionoLeft = false;
                }
            }

            if (right)
            {
                right = false;
                presionoRight = true;
            }
            else
            {
                if (presionoRight)
                {
                    right = true;
                    presionoRight = false;
                }
            }

            if (enter)
            {
                enter = false;
                presionoEnter = true;
            }
            else
            {
                if (presionoEnter)
                {
                    enter = true;
                    presionoEnter = false;
                }
            }


            bool anyKey = up || down || left || right || enter;


            //Reset effect when any key is pressed
            if (anyKey)
            {
                SelectedText.Color = Color.White;
            }

            ///// si tocas enter volver al menu

            //Update selection var
            if (CurrentStatus == Status.Gameplay)
            {
                var health = Player.Health();
                var maxHealth = Player.MaxHealth();
                var oxygen = Player.Oxygen();
                var maxOxygen = Player.MaxOxygen();

                HealthBar.Scaling = new TGCVector2(health / maxHealth, 1);
                OxygenBar.Scaling = new TGCVector2(oxygen / maxOxygen, 1);
            }
            else if (CurrentStatus == Status.MainMenu)
            {
                if (up)
                {
                    if (SelectedText == Start)
                    {
                        SelectedText = Exit;
                    }
                    else if (SelectedText == Instruccion)
                    {
                        SelectedText = Start;
                    }
                    else if (SelectedText == Exit)
                    {
                        SelectedText = Instruccion;
                    }
                }
                else if (down)
                {
                    if (SelectedText == Start)
                    {
                        SelectedText = Instruccion;
                    }
                    else if (SelectedText == Instruccion)
                    {
                        SelectedText = Exit;
                    }
                    else if (SelectedText == Exit)
                    {
                        SelectedText = Start;
                    }
                } else if (enter)
                    if (SelectedText == Start)
                        ChangeStatus(Status.Gameplay);
                    else if (SelectedText == Exit)
                        Application.Exit();
                    else if (SelectedText == Instruccion)
                        CurrentStatus = Status.Instructions;
            } else if (CurrentStatus == Status.Instructions)
            {
                if (enter)
                {
                    CurrentStatus = Status.MainMenu;
                }
            }
            else if (CurrentStatus == Status.Inventory || CurrentStatus == Status.Crafting)
            {
                if(LastStatus != Status.Inventory && LastStatus != Status.Crafting)
                    UpdateIconSprites();

                CraftingItems[SelectedItemIndex].background.Color = Color.CadetBlue;

                if (CurrentStatus == Status.Crafting)
                {
                    //Change selected item
                    if (left)
                        SelectedItemIndex--;
                    else if (right)
                        SelectedItemIndex++;
                    if (left || right)
                    {
                        sonido.play();
                    }
                    SelectedItemIndex = FastMath.Clamp(SelectedItemIndex, 0, MAX_CRAFTING_ITEMS - 1);
                    CraftingItems[SelectedItemIndex].background.Color = Color.Orange;

                    //Check for crafting
                    if (enter)
                    {
                        bool SelectedItemExists = SelectedItemIndex < Inventory.Instance().GetCraftings().Count;
                        if (SelectedItemExists)
                        {
                            var SelectedItem = Inventory.Instance().GetCraftings()[SelectedItemIndex];
                            if (SelectedItem.EstoyHabilitado())
                            {
                                sonido.play();
                                SelectedItem.Craftear();
                                UpdateIconSprites();
                            }
                        }
                    }
                }
            }
            else if (CurrentStatus == Status.GameOver || CurrentStatus == Status.Win)
            {
                if (enter)
                    Application.Exit();
            }

            //Update last status so it doesn't UpdateIconSprites() multiple times
            LastStatus = CurrentStatus;
        }

            public static void Render()
        {
            //Gameplay
            if(CurrentStatus == Status.Gameplay) {
                drawer.BeginDrawSprite();
                drawer.DrawSprite(HealthBar);
                drawer.DrawSprite(OxygenBar);
                drawer.EndDrawSprite();
            }
            //Main Menu
            else if(CurrentStatus == Status.MainMenu)
            {
                drawer.BeginDrawSprite();
                drawer.DrawSprite(Logo);
                drawer.EndDrawSprite();
                SelectedText.Color = Color.Orange;
                Start.render();
                Instruccion.render();
                Exit.render();
            } else if (CurrentStatus == Status.Instructions){
                drawer.BeginDrawSprite();
                drawer.DrawSprite(Logo);
                drawer.EndDrawSprite();
                foreach (var ins in instrucciones)
                {
                    ins.render();
                }
            }
            //Inventory + Crafting
            else if (CurrentStatus == Status.Inventory || CurrentStatus == Status.Crafting)
            {
                drawer.BeginDrawSprite();

                //Draw overlay
                if (CurrentStatus == Status.Inventory)
                    drawer.DrawSprite(OverlayInv);
                else
                    drawer.DrawSprite(OverlayCraft);


                foreach (var item in InventoryItems)
                {
                    drawer.DrawSprite(item.background);
                    if(item.icon.Bitmap != null)
                        drawer.DrawSprite(item.icon);
                }

                //Show crafting as an extra
                if (CurrentStatus == Status.Crafting)
                {
                    foreach (var item in CraftingItems)
                    {
                        drawer.DrawSprite(item.background);
                        if (item.icon.Bitmap != null)
                            drawer.DrawSprite(item.icon);
                    }
                }

                drawer.EndDrawSprite();

                //Draw text
                foreach (var item in InventoryItems)
                    item.amount.render();
            }
            //GameOver
            else if (CurrentStatus == Status.GameOver)
            {
                GameOver.render();
                GameOverSubtitle.render();
            }
            //Win
            else if (CurrentStatus == Status.Win)
            {
                YouWin.render();
                YouWinSubtitle.render();
            }
        }

        public static void Dispose()
        {
            Start.Dispose();
            Instruccion.Dispose();
            Exit.Dispose();
            GameOver.Dispose();
            GameOverSubtitle.Dispose();
            YouWin.Dispose();
            YouWinSubtitle.Dispose();
            Logo.Dispose();
            OverlayInv.Dispose();
            OverlayCraft.Dispose();
            HealthBar.Dispose();
            OxygenBar.Dispose();
            sonido.dispose();
            foreach (var ins in instrucciones)
            {
                ins.Dispose();
            }
            foreach (var item in InventoryItems)
                item.Dispose();
            foreach (var item in CraftingItems)
                item.Dispose();
        }

        //Interactive functions
        public static void ChangeStatus(Status status) { 
            LastStatus = CurrentStatus; 
            CurrentStatus = status; 
        }

        public static Status GetCurrentStatus() { return CurrentStatus; }
        static public bool IsCurrentStatus(Status status)
        {
            return CurrentStatus == status;
        }
        //Internal functions
        private static int Round(float n) { return (int)Math.Round(n); }

        private static void UpdateIconSprites()
        {
            List<Item> ItemsInInventory = Inventory.Instance().GetItems();
            for (int i = 0; i < InventoryItems.Count; i++)
            {
                
                
                if (i < ItemsInInventory.Count)
                {
                    string path = MediaDir + ItemsInInventory[i].obtenerImagen();
                    InventoryItems[i].amount.Text = ItemsInInventory[i].Amount().ToString();
                    InventoryItems[i].icon.Bitmap = new CustomBitmap(path, D3DDevice.Instance.Device);
                }
                else
                {
                    InventoryItems[i].amount.Text = "";
                    InventoryItems[i].icon.Bitmap = null;
                }
            }

            var CraftsInInventory = Inventory.Instance().GetCraftings();
            for (int i = 0; i < CraftingItems.Count; i++)
            {
                
                if (i < CraftsInInventory.Count)
                {
                    string path = MediaDir + CraftsInInventory[i].ObtenerIcono();
                    CraftingItems[i].icon.Bitmap = new CustomBitmap(path, D3DDevice.Instance.Device);
                }
                else
                    CraftingItems[i].icon.Bitmap = null;
            }
        }
    }
}
