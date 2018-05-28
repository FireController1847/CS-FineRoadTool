﻿using ICities;
using UnityEngine;

using System;

using ColossalFramework;
using ColossalFramework.UI;

namespace FineRoadTool
{
    public class ModInfo : IUserMod
    {
        public ModInfo()
        {
            try
            {
                // Creating setting file
                if(GameSettings.FindSettingsFileByName(FineRoadTool.settingsFileName) == null)
                {
                    GameSettings.AddSettingsFile(new SettingsFile[] { new SettingsFile() { fileName = FineRoadTool.settingsFileName } });
                }
            }
            catch (Exception e)
            {
                DebugUtils.Log("Couldn't load/create the setting file.");
                DebugUtils.LogException(e);
            }
        }

        public string Name
        {
            get { return "Fine Road Tool " + version; }
        }

        public string Description
        {
            get { return "More road tool options"; }
        }

        public void OnSettingsUI(UIHelperBase helper)
        {
            try
            {
                UIHelper group = helper.AddGroup(Name) as UIHelper;
                UIPanel panel = group.self as UIPanel;

                UICheckBox checkBox = (UICheckBox)group.AddCheckbox("Disable debug messages logging", DebugUtils.hideDebugMessages.value, (b) =>
                {
                    DebugUtils.hideDebugMessages.value = b;
                });
                checkBox.tooltip = "If checked, debug messages won't be logged.";

                group.AddSpace(10);

                checkBox = (UICheckBox)group.AddCheckbox("Reduce rail catenary masts", FineRoadTool.reduceCatenary.value, (b) =>
                {
                    FineRoadTool.reduceCatenary.value = b;
                    if (FineRoadTool.instance != null)
                    {
                        FineRoadTool.instance.UpdateCatenary();
                    }
                });
                checkBox.tooltip = "Reduce the number of catenary mast of rail lines from 3 to 1 per segment.\n";

                group.AddSpace(10);

                checkBox = (UICheckBox)group.AddCheckbox("Change max turn angle for more realistic tram tracks turns", FineRoadTool.changeMaxTurnAngle.value, (b) =>
                {
                    FineRoadTool.changeMaxTurnAngle.value = b;

                    if (b) RoadPrefab.SetMaxTurnAngle(FineRoadTool.maxTurnAngle);
                    else RoadPrefab.ResetMaxTurnAngle();
                });
                checkBox.tooltip = "Change all roads with tram tracks max turn angle by the value below if current value is higher";

                group.AddTextfield("Max turn angle: ", FineRoadTool.maxTurnAngle.ToString(), (f) =>{},
                    (s) =>
                    {
                        float f = 0;
                        float.TryParse(s, out f);

                        FineRoadTool.maxTurnAngle.value = Mathf.Clamp(f, 0f, 180f);

                        if (FineRoadTool.changeMaxTurnAngle.value)
                        {
                            RoadPrefab.SetMaxTurnAngle(FineRoadTool.maxTurnAngle.value);
                        }
                    });

                group.AddSpace(10);

                panel.gameObject.AddComponent<OptionsKeymapping>();

                group.AddSpace(10);

                group.AddButton("Reset tool window position", () =>
                {
                    UIToolOptionsButton.savedWindowX.Delete();
                    UIToolOptionsButton.savedWindowY.Delete();

                    if (UIToolOptionsButton.toolOptionsPanel)
                        UIToolOptionsButton.toolOptionsPanel.absolutePosition = new Vector3(-1000, -1000);
                });
            }
            catch (Exception e)
            {
                DebugUtils.Log("OnSettingsUI failed");
                DebugUtils.LogException(e);
            }
        }

        public const string version = "1.3.7";
    }
}
