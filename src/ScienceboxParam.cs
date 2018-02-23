using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using KSP.Localization;
using UnityEngine;

namespace Sciencebox
{
    /// <summary>
    /// Adds the sciencebox option to the new game dialog
    /// </summary>
    [KSPAddon(KSPAddon.Startup.MainMenu, false)]
    public class ScienceboxParams : MonoBehaviour
    {
        /// <summary>
        /// The seed for the new game
        /// </summary>
        public static Boolean Sciencebox { get; set; }

        /// <summary>
        /// The main menu Instance
        /// </summary>
        private MainMenu menu { get; set; }

        /// <summary>
        /// Gets called when the mono behaviour is created and registers a callback for changing
        /// the game types
        /// </summary>
        void Awake()
        {
            GameEvents.onGameStateCreated.Add(OnGameStateCreated);
        }

        /// <summary>
        /// Adds a hook into the MainMenu UI for editing the New Game Dialog
        /// </summary>
        void Start()
        {
            menu = FindObjectsOfType<MainMenu>().FirstOrDefault();
            menu.newGameBtn.onTap += OnNewGameBtnTap;
        }

        /// <summary>
        /// This function gets called when the user clicks the "New Game" button in the main menu
        /// </summary>
        void OnNewGameBtnTap()
        {
            // Grab internal values
            FieldInfo createGameDialog = typeof(MainMenu).GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                .FirstOrDefault(f => f.FieldType == typeof(PopupDialog));
            if (createGameDialog == null)
                return;
            FieldInfo newGameMode = typeof(MainMenu).GetFields(BindingFlags.NonPublic | BindingFlags.Static)
                .FirstOrDefault(f => f.FieldType == typeof(Game.Modes));
            if (newGameMode == null)
                return;
            FieldInfo newGameParameters = typeof(MainMenu).GetFields(BindingFlags.NonPublic | BindingFlags.Static)
                .FirstOrDefault(f => f.FieldType == typeof(GameParameters));
            if (newGameParameters == null)
                return;
            MethodInfo UpdatedGameParameters = typeof(MainMenu).GetMethod("UpdatedGameParameters",
                BindingFlags.NonPublic | BindingFlags.Instance);
            if (UpdatedGameParameters == null)
                return;
            
            // Descent into the popup dialog
            PopupDialog dialog = createGameDialog.GetValue(menu) as PopupDialog;
            if (dialog == null)
                return;
            if (dialog.dialogToDisplay == null)
                return;
            DialogGUIHorizontalLayout d1 = dialog.dialogToDisplay.Options[0] as DialogGUIHorizontalLayout;
            if (d1 == null)
                return;
            DialogGUIVerticalLayout d2 = d1.children[0] as DialogGUIVerticalLayout;
            if (d2 == null)
                return;
            DialogGUIHorizontalLayout d3 = d2.children[1] as DialogGUIHorizontalLayout;
            if (d3 == null)
                return;
            DialogGUIVerticalLayout d4 = d3.children[2] as DialogGUIVerticalLayout;
            if (d4 == null)
                return;
            DialogGUIToggleGroup d5 = d4.children[0] as DialogGUIToggleGroup;
            if (d5 == null)
                return;

            // Create the new layout
            DialogGUIToggle scienceboxButton = new DialogGUIToggle(
                (Game.Modes) newGameMode.GetValue(null) == Game.Modes.SCIENCE_SANDBOX && Sciencebox,
                Localizer.Format("#LOC_SCIENCEBOX_NAME"), b =>
                {
                    if ((Game.Modes) newGameMode.GetValue(null) != Game.Modes.SCIENCE_SANDBOX)
                    {
                        newGameMode.SetValue(null, Game.Modes.SCIENCE_SANDBOX);
                        newGameParameters.SetValue(null,
                            UpdatedGameParameters.Invoke(menu, new[] {newGameParameters.GetValue(null)}));
                    }
                    Sciencebox = true;
                }, 200f, 30f);
            DialogGUIToggle scienceButton = new DialogGUIToggle((Game.Modes) newGameMode.GetValue(null) == Game.Modes.SCIENCE_SANDBOX && !Sciencebox,
                Localizer.Format("#autoLOC_190714"), b =>
                {
                    if ((Game.Modes) newGameMode.GetValue(null) != Game.Modes.SCIENCE_SANDBOX)
                    {
                        newGameMode.SetValue(null, Game.Modes.SCIENCE_SANDBOX);
                        newGameParameters.SetValue(null,
                            UpdatedGameParameters.Invoke(menu, new[] {newGameParameters.GetValue(null)}));
                    }
                    Sciencebox = false;
                }, 200f, 30f);
            d5.children.Insert(2, scienceboxButton);
            d5.children[1] = scienceButton;
            d4.children[0] = d5;
            d3.children[2] = d4;
            d2.children[1] = d3;
            d1.children[0] = d2;
            dialog.dialogToDisplay.Options[0] = d1;
            DialogGUIBox scienceboxBox = new DialogGUIBox(string.Empty, -1f, 100f,
                () => (Game.Modes) newGameMode.GetValue(null) == Game.Modes.SCIENCE_SANDBOX && Sciencebox,
                new DialogGUIBase[]
                {
                    new DialogGUIHorizontalLayout(false, false, 2f, new RectOffset(8, 8, 8, 8), TextAnchor.MiddleLeft,
                        new DialogGUIBase[]
                        {
                            new DialogGUIImage(new Vector2(96f, 96f), Vector2.zero, Color.white,
                                menu.scienceSandboxIcon),
                            new DialogGUILabel(
                                Localizer.Format("#LOC_SCIENCEBOX_TEXT1") + "\n\n" +
                                Localizer.Format("#LOC_SCIENCEBOX_TEXT2"), menu.guiSkinDef.SkinDef.customStyles[6],
                                true,
                                true)
                        })
                });
            DialogGUIBox scienceBox = new DialogGUIBox(string.Empty, -1f, 100f,
                () => (Game.Modes) newGameMode.GetValue(null) == Game.Modes.SCIENCE_SANDBOX && !Sciencebox,
                new DialogGUIBase[]
                {
                    new DialogGUIHorizontalLayout(false, false, 2f, new RectOffset(8, 8, 8, 8), TextAnchor.MiddleLeft,
                        new DialogGUIBase[]
                        {
                            new DialogGUIImage(new Vector2(96f, 96f), Vector2.zero, Color.white,
                                menu.scienceSandboxIcon),
                            new DialogGUILabel(
                                Localizer.Format("#autoLOC_190750") + "\n\n" +
                                Localizer.Format("#autoLOC_190751"), menu.guiSkinDef.SkinDef.customStyles[6],
                                true,
                                true)
                        })
                });
            List<DialogGUIBase> elements = dialog.dialogToDisplay.Options.ToList(); 
            elements[2] = scienceBox;
            elements.Insert(3, scienceboxBox);
            dialog.dialogToDisplay.Options = elements.ToArray();
            PopupDialog newDialog = PopupDialog.SpawnPopupDialog(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), dialog.dialogToDisplay, false, menu.guiSkinDef.SkinDef, true, "");
            dialog.Dismiss();
            createGameDialog.SetValue(menu, newDialog);
        }

        /// <summary>
        /// When a new game gets created, tell the applicator that it should act.
        /// </summary>
        void OnGameStateCreated(Game game)
        {
            if (!Sciencebox || game.Mode != Game.Modes.SCIENCE_SANDBOX)
                return;

            ScienceboxApplicator.isActive = true;
        }
    }
}