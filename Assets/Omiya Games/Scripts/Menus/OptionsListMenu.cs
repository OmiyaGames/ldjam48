﻿using UnityEngine;
using UnityEngine.UI;

namespace OmiyaGames.Menu
{
    using Settings;

    ///-----------------------------------------------------------------------
    /// <copyright file="OptionsMenu.cs" company="Omiya Games">
    /// The MIT License (MIT)
    /// 
    /// Copyright (c) 2014-2018 Omiya Games
    /// 
    /// Permission is hereby granted, free of charge, to any person obtaining a copy
    /// of this software and associated documentation files (the "Software"), to deal
    /// in the Software without restriction, including without limitation the rights
    /// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    /// copies of the Software, and to permit persons to whom the Software is
    /// furnished to do so, subject to the following conditions:
    /// 
    /// The above copyright notice and this permission notice shall be included in
    /// all copies or substantial portions of the Software.
    /// 
    /// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    /// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    /// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    /// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    /// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    /// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    /// THE SOFTWARE.
    /// </copyright>
    /// <author>Taro Omiya</author>
    /// <date>8/18/2015</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Menu that provides options.  Currently only supports changing sound
    /// and music volume. You can retrieve this menu from the singleton script,
    /// <code>MenuManager</code>.
    /// </summary>
    /// <seealso cref="MenuManager"/>
    [RequireComponent(typeof(Animator))]
    public class OptionsListMenu : IMenu
    {
        [SerializeField]
        Button defaultButton;

        [Header("Background Settings")]
        [SerializeField]
        string projectTitleTranslationKey = "Game Title";
        [SerializeField]
        bool showBackground = false;

        /// <summary>
        /// Flag indicating a button in this menu has already been clicked.
        /// Since this menu leads to other menus, this flag is used to prevent double-clicking.
        /// The value will be reset when the panel is made visible again.
        /// </summary>
        bool isButtonLocked = false;

        public override Type MenuType
        {
            get
            {
                return Type.ManagedMenu;
            }
        }

        public override GameObject DefaultUi
        {
            get
            {
                return null;
            }
        }

        public override bool ShowBackground
        {
            get
            {
                return showBackground;
            }
        }

        public override string TitleTranslationKey
        {
            get
            {
                return projectTitleTranslationKey;
            }
        }

        #region UI events
        public void OnLanguageClicked()
        {
            // Make sure the button isn't locked yet
            if (isButtonLocked == false)
            {
                // FIXME: show the language options
                //Manager.Show<ConfirmationMenu>();

                // Indicate the button has been clicked.
                isButtonLocked = true;
            }
        }

        public void OnResetDataClicked()
        {
            // Make sure the button isn't locked yet
            if (isButtonLocked == false)
            {
                ConfirmationMenu menu = Manager.GetMenu<ConfirmationMenu>();
                if (menu != null)
                {
                    // Display confirmation dialog
                    menu.DefaultToYes = false;
                    menu.Show(CheckResetSavedDataConfirmation);

                    // Indicate the button has been clicked.
                    isButtonLocked = true;
                }
            }
        }
        #endregion

        protected override void OnStateChanged(State from, State to)
        {
            // Call the base method
            base.OnStateChanged(from, to);

            // If this menu is visible again, release the button lock
            if (to == State.Visible)
            {
                isButtonLocked = false;
            }
        }

        void CheckResetSavedDataConfirmation(IMenu menu)
        {
            if (((ConfirmationMenu)menu).IsYesSelected == true)
            {
                // Clear settings
                Singleton.Get<GameSettings>().ClearSettings();

                // Update the level select menu, if one is available
                LevelSelectMenu levelSelect = Manager.GetMenu<LevelSelectMenu>();
                if (levelSelect != null)
                {
                    levelSelect.SetButtonsEnabled(true);
                }
            }
        }
    }
}
