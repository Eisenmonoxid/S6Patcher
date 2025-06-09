-- UserScriptLocal by Eisenmonoxid - S6Patcher --
-- Find latest S6Patcher version here: https://github.com/Eisenmonoxid/S6Patcher
S6Patcher = S6Patcher or {
	DoNotUseFeatures = (NEP or QSB) and true or false;
	UseSingleStop = false;
	UseDowngrade = false;
	UseMilitaryRelease = false;
};
-- ************************************************************************************************************************************************************* --
-- Fix the "Meldungsstau" Bug in the game																					 									 --
-- ************************************************************************************************************************************************************* --
if (S6Patcher.g_FeedbackSpeechFix == nil) or (not Trigger.IsTriggerEnabled(S6Patcher.g_FeedbackSpeechFix)) then
	S6Patcher.g_FeedbackSpeechFix = Trigger.RequestTrigger(Events.LOGIC_EVENT_EVERY_SECOND, "", "FeedbackSpeechEndTimeFixCustom", 1);
	if Framework.WriteToLog ~= nil then
		Framework.WriteToLog("S6Patcher: Meldungsstaufix is enabled!");
	end
end
FeedbackSpeechEndTimeFixCustom = function()
	local Time = Framework.GetTimeMs();	
	if (g_FeedbackSpeech ~= nil) and (g_FeedbackSpeech.LastSpeechEndTime ~= nil) and ((Time + 6000) < g_FeedbackSpeech.LastSpeechEndTime) then
		g_FeedbackSpeech.LastSpeechEndTime = nil;
		XGUIEng.ShowWidget("/InGame/Root/Normal/AlignBottomRight/MapFrame/FeedbackSpeechText", 0);
	
		if Framework.WriteToLog ~= nil then
			Framework.WriteToLog("S6Patcher: Caught Meldungsstau at " .. tostring(Time));
		end
	end
end
-- ************************************************************************************************************************************************************* --
-- Fix "Build Walls from Fence Button"																					 									 	 --
-- ************************************************************************************************************************************************************* --
if S6Patcher.ContinueWallUpdate == nil then
	S6Patcher.ContinueWallUpdate = GUI_BuildingButtons.ContinueWallUpdate;
end
GUI_BuildingButtons.ContinueWallUpdate = function()
	local EntityType = Logic.GetEntityType(GUI.GetSelectedEntity());
	if EntityType == Entities.B_FenceTurret then
		XGUIEng.ShowWidget(XGUIEng.GetCurrentWidgetID(), 0);
		return;
	else
		S6Patcher.ContinueWallUpdate();
	end
end
-- ************************************************************************************************************************************************************* --
-- Fix "B_Cathedral_Big"																					 									 	 			 --
-- ************************************************************************************************************************************************************* --
if S6Patcher.BuildingNameUpdate == nil then
	S6Patcher.BuildingNameUpdate = GUI_BuildingInfo.BuildingNameUpdate;
end
GUI_BuildingInfo.BuildingNameUpdate = function()
	S6Patcher.BuildingNameUpdate()

	local WidgetID = XGUIEng.GetCurrentWidgetID();
	if XGUIEng.GetText(WidgetID) == "{center}B_Cathedral_Big" then		
		local Text = S6Patcher.GetLocalizedText({de = "{center}Kathedrale", en = "{center}Cathedral"});
		XGUIEng.SetText(WidgetID, Text);
	end
end
-- ************************************************************************************************************************************************************* --
-- Make all Knights available in the expansion pack ("Eastern Realm")																					 		 --
-- ************************************************************************************************************************************************************* --
S6Patcher.GlobalScriptOverridden = false;
S6Patcher.DefaultKnightNames = {"U_KnightSaraya", "U_KnightTrading", "U_KnightHealing", "U_KnightChivalry", "U_KnightWisdom", "U_KnightPlunder", "U_KnightSong"};
S6Patcher.OverrideGlobalScript = function()
	Framework.SetOnGameStartLuaCommand("return;"); -- free memory
	local Knight = Entities[S6Patcher.DefaultKnightNames[S6Patcher.SelectedKnight]];

	GUI.SendScriptCommand([[
		S6Patcher = S6Patcher or {}
		S6Patcher.UpdateKnight = function()
			local PlayerID = ]] .. tostring(GUI.GetPlayerID()) .. [[;
			local KnightID = Logic.GetKnightID(PlayerID);
	
			if (KnightID ~= 0) then
				S6Patcher.ReplaceKnight(KnightID, ]] .. tostring(Knight) .. [[);
			end
		end
		
		S6Patcher.ReplaceKnight = function(_knightID, _newType)
			if Logic.GetEntityType(_knightID) == _newType then
				return;
			end

			local posX, posY, posZ = Logic.EntityGetPos(_knightID);
			local PlayerID = Logic.EntityGetPlayer(_knightID);
			local Orientation = Logic.GetEntityOrientation(_knightID);
			local ScriptName = Logic.GetEntityName(_knightID);

			Logic.DestroyEntity(_knightID);
			local ID = Logic.CreateEntity(_newType, posX, posY, Orientation, PlayerID);
			Logic.SetEntityName(ID, ScriptName);
			Logic.SetPrimaryKnightID(PlayerID, ID);
		end

		S6Patcher.UpdateKnight();
		Logic.ExecuteInLuaLocalState('LocalSetKnightPicture();');
	]]);
end

S6Patcher.IsCurrentMapEligibleForKnightReplacement = function()
	local Name = Framework.GetCurrentMapName();
	local Type, Campaign = Framework.GetCurrentMapTypeAndCampaignName();

	if Type == 0 or Type == 3 then -- Singleplayer and Usermap
		local Names = {Framework.GetValidKnightNames(Name, Type)};
		if #Names == 0 then -- No custom ValidKnightNames in info.xml
			return true;
		end
	end

	return false;
end

if S6Patcher.IsCurrentMapEligibleForKnightReplacement() == true
	and Options.GetIntValue("S6Patcher", "ExtendedKnightSelection", 0) ~= 0
	and (Framework.GetGameExtraNo() >= 1) 
	and (not Framework.IsNetworkGame())
	and (not S6Patcher.GlobalScriptOverridden)
	and (S6Patcher.SelectedKnight ~= nil) then
	
	S6Patcher.OverrideGlobalScript();
	if S6Patcher.RestartMap == nil then
		S6Patcher.RestartMap = Framework.RestartMap;
	end
	Framework.RestartMap = function(_knightType)
		local TypeName = Logic.GetEntityTypeName(_knightType);
		for i = 1, #S6Patcher.DefaultKnightNames do
			if S6Patcher.DefaultKnightNames[i] == TypeName then
				Framework.SetOnGameStartLuaCommand("S6Patcher = S6Patcher or {};S6Patcher.SelectedKnight = " .. tostring(i) .. ";");
				break;
			end
		end
	
		S6Patcher.RestartMap(_knightType);
	end
	
	S6Patcher.GlobalScriptOverridden = true;
end
-- ************************************************************************************************************************************************************* --
-- SingleStopButtons on Buildings																														 		 --
-- ************************************************************************************************************************************************************* --
if Options.GetIntValue("S6Patcher", "UseSingleStop", 0) ~= 0 and not S6Patcher.DoNotUseFeatures then
	S6Patcher.UseSingleStop = true;
	
	GUI_BuildingButtons.GateAutoToggleClicked = function()
		Sound.FXPlay2DSound("ui\\menu_click");
		local EntityID = GUI.GetSelectedEntity();
		GUI.SetStoppedState(EntityID, not Logic.IsBuildingStopped(EntityID));
	end

	GUI_BuildingButtons.GateAutoToggleMouseOver = function()
		GUI_Tooltip.TooltipNormal((Logic.IsBuildingStopped(GUI.GetSelectedEntity()) and "StartBuilding") or "StopBuilding");
	end

	GUI_BuildingButtons.GateAutoToggleUpdate = function()
		local CurrentWidgetID = XGUIEng.GetCurrentWidgetID();
		local EntityID = GUI.GetSelectedEntity();

		if EntityID == nil 
			or Logic.IsBuilding(EntityID) == 0 
			or Logic.IsConstructionComplete(EntityID) == 0 
			or Logic.IsBuildingStoppable(EntityID, true) == false
			or Logic.IsEntityInCategory(EntityID, EntityCategories.Cathedrals) == 1
			or Logic.IsBurning(EntityID) == true
			or Logic.CanCancelKnockDownBuilding(EntityID) == true
		then
			XGUIEng.ShowWidget(CurrentWidgetID, 0);
			return;
		end
		
		SetIcon(CurrentWidgetID, (Logic.IsBuildingStopped(EntityID) and {4, 12}) or {4, 13});
		XGUIEng.ShowWidget(CurrentWidgetID, 1);
	end
end
-- ************************************************************************************************************************************************************* --
-- DowngradeButton on Buildings																															 		 --
-- ************************************************************************************************************************************************************* --
if Options.GetIntValue("S6Patcher", "UseDowngrade", 0) ~= 0 and not S6Patcher.DoNotUseFeatures then
	S6Patcher.UseDowngrade = true;
	
	GUI_BuildingButtons.GateOpenCloseClicked = function()
		local PlayerID = GUI.GetPlayerID();
		local EntityID = GUI.GetSelectedEntity();
		local Value = Logic.GetEntityHealth(EntityID) - (Logic.GetEntityMaxHealth(EntityID) * 0.5);
		
		Sound.FXPlay2DSound("ui\\menu_click");
		GUI.DeselectEntity(EntityID);
		GUI.SendScriptCommand("Logic.HurtEntity(" .. tostring(EntityID) .. ", " .. tostring(math.ceil(Value)) .. ");");
	end

	GUI_BuildingButtons.GateOpenCloseUpdate = function()
		local CurrentWidgetID = XGUIEng.GetCurrentWidgetID();
		local EntityID = GUI.GetSelectedEntity();
		
		if EntityID == nil 
			or Logic.IsBuilding(EntityID) == 0 
			or Logic.IsConstructionComplete(EntityID) == 0 
			or Logic.IsBurning(EntityID) == true
			or Logic.CanCancelKnockDownBuilding(EntityID) == true
			or Logic.IsEntityInCategory(EntityID, EntityCategories.Cathedrals) == 1
			or Logic.GetEntityHealth(EntityID) < Logic.GetEntityMaxHealth(EntityID)
			or Logic.GetUpgradeLevel(EntityID) < Logic.GetMaxUpgradeLevel(EntityID)
		then
			XGUIEng.ShowWidget(CurrentWidgetID, 0);
			return;
		end

		SetIcon(CurrentWidgetID, {3, 15});
		XGUIEng.ShowWidget(CurrentWidgetID, 1);
	end

	GUI_BuildingButtons.GateOpenCloseMouseOver = function()	
		GUI_Tooltip.TooltipNormal("DowngradeButton");
	end
end
-- ************************************************************************************************************************************************************* --
-- Release soldiers																																		 		 --
-- ************************************************************************************************************************************************************* --
if Options.GetIntValue("S6Patcher", "UseMilitaryRelease", 0) ~= 0 and not S6Patcher.DoNotUseFeatures then
	S6Patcher.UseMilitaryRelease = true;

	if S6Patcher.DismountClicked == nil then
		S6Patcher.DismountClicked = GUI_Military.DismountClicked;
	end
	GUI_Military.DismountClicked = function()
		local PlayerID = GUI.GetPlayerID();
		local EntityID = GUI.GetSelectedEntity();
		local GuardedEntityID = Logic.GetGuardedEntityID(EntityID);
		
		if EntityID ~= 0 and Logic.IsLeader(EntityID) == 1 and GuardedEntityID == 0 then
			Sound.FXPlay2DSound("ui\\menu_click");

			local Soldiers = {Logic.GetSoldiersAttachedToLeader(EntityID)};
			if Soldiers[1] > 1 then
				local Soldier = table.remove(Soldiers, (Soldiers[1] + 1));
				local posX, posY = Logic.GetEntityPosition(Soldier);
				GUI.SendScriptCommand([[
					Logic.CreateEffect(EGL_Effects.FXDie, ]] .. tostring(posX) .. [[, ]] .. tostring(posY) .. [[, ]] .. tostring(PlayerID) .. [[);
					Logic.DestroyEntity(]] .. tostring(Soldier) .. [[);
				]]);
			else
				GUI.ClearSelection();
				local posX, posY = Logic.GetEntityPosition(EntityID);		
				GUI.SendScriptCommand([[
					Logic.CreateEffect(EGL_Effects.FXDie, ]] .. tostring(posX) .. [[, ]] .. tostring(posY) .. [[, ]] .. tostring(PlayerID) .. [[);
					Logic.DestroyGroupByLeader(]] .. tostring(EntityID) .. [[);
				]]);
				
				Soldiers = nil;
			end
		else
			S6Patcher.DismountClicked();
		end
	end

	if S6Patcher.DismountUpdate == nil then
		S6Patcher.DismountUpdate = GUI_Military.DismountUpdate;
	end
	GUI_Military.DismountUpdate = function()
		local CurrentWidgetID = XGUIEng.GetCurrentWidgetID();
		local EntityID = GUI.GetSelectedEntity();
		local GuardedEntityID = Logic.GetGuardedEntityID(EntityID);

		if EntityID ~= 0 and Logic.GetEntityType(EntityID) == Entities.U_MilitaryLeader and GuardedEntityID == 0 then
			SetIcon(CurrentWidgetID, {14, 12});
			XGUIEng.DisableButton(CurrentWidgetID, 0);
			return;
		end
		
		SetIcon(CurrentWidgetID, {12, 1});
		S6Patcher.DismountUpdate();
	end
end

S6Patcher.HandleMilitaryReleaseTooltip = function(_widgetID, _TooltipNameWidget, _TooltipDescriptionWidget)
	if _widgetID ~= XGUIEng.GetWidgetID("/InGame/Root/Normal/AlignBottomRight/DialogButtons/Military/Dismount") then
		return false;
	end

	local Title = {de = "Entlassen", en = "Dismiss"};
	local Text = {de = "- Entlässt Soldaten der Reihe nach", en = "- Dismisses soldiers one after another"};
	
	local EntityID = GUI.GetSelectedEntity();
	local GuardedEntityID = Logic.GetGuardedEntityID(EntityID);
	
	if EntityID ~= 0 and Logic.GetEntityType(EntityID) == Entities.U_MilitaryLeader and GuardedEntityID == 0 then
		S6Patcher.SetTooltip(_TooltipNameWidget, _TooltipDescriptionWidget, Title, Text);
		return true;
	else
		return false;
	end
end

if S6Patcher.GameCallback_GUI_SelectionChanged == nil then
	S6Patcher.GameCallback_GUI_SelectionChanged = GameCallback_GUI_SelectionChanged;
end
GameCallback_GUI_SelectionChanged = function(_Source)
	S6Patcher.GameCallback_GUI_SelectionChanged(_Source);
	
	if (not S6Patcher.DoNotUseFeatures) and (S6Patcher.SingleStopButtons == true or S6Patcher.UseDowngrade == true) then -- Don't show in usermaps to not break compatibility	
		XGUIEng.ShowWidget("/InGame/Root/Normal/BuildingButtons/GateAutoToggle", 1); -- Unused in the game
		XGUIEng.ShowWidget("/InGame/Root/Normal/BuildingButtons/GateOpenClose", 1); -- Unused in the game
	end
end

if S6Patcher.SetNameAndDescription == nil then
	S6Patcher.SetNameAndDescription = GUI_Tooltip.SetNameAndDescription;
end
GUI_Tooltip.SetNameAndDescription = function(_TooltipNameWidget, _TooltipDescriptionWidget, _OptionalTextKeyName, _OptionalDisabledTextKeyName, _OptionalMissionTextFileBoolean)
	local CurrentWidgetID = XGUIEng.GetCurrentWidgetID();
	
	if not S6Patcher.DoNotUseFeatures then -- compatibility with usermaps
		if _OptionalTextKeyName == "DowngradeButton" then
			local Title = {de = "Rückbau", en = "Downgrade"};
			local Text = {de = "- Baut das Gebäude um eine Stufe zurück", en = "- Downgrades the building by one level"};
			S6Patcher.SetTooltip(_TooltipNameWidget, _TooltipDescriptionWidget, Title, Text);
			return;
		elseif S6Patcher.HandleMilitaryReleaseTooltip(CurrentWidgetID, _TooltipNameWidget, _TooltipDescriptionWidget) == true then
			return;
		end
	end
	
	return S6Patcher.SetNameAndDescription(_TooltipNameWidget, _TooltipDescriptionWidget, _OptionalTextKeyName, _OptionalDisabledTextKeyName, _OptionalMissionTextFileBoolean);	
end
S6Patcher.SetTooltip = function(_TooltipNameWidget, _TooltipDescriptionWidget, _Title, _Text)
	XGUIEng.SetText(_TooltipNameWidget, "{center}" .. S6Patcher.GetLocalizedText(_Title));
	XGUIEng.SetText(_TooltipDescriptionWidget, S6Patcher.GetLocalizedText(_Text));

	local Height = XGUIEng.GetTextHeight(_TooltipDescriptionWidget, true)
	local W, H = XGUIEng.GetWidgetSize(_TooltipDescriptionWidget)

	XGUIEng.SetWidgetSize(_TooltipDescriptionWidget, W, Height)
end
S6Patcher.GetLocalizedText = function(_text) return (Network.GetDesiredLanguage() == "de" and  _text.de) or _text.en; end
-- #EOF