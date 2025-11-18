-- UserScriptLocal by Eisenmonoxid - S6Patcher --
-- Find latest S6Patcher version here: https://github.com/Eisenmonoxid/S6Patcher
S6Patcher = S6Patcher or {};
S6Patcher.Options = {"ExtendedKnightSelection", "SpecialKnightsAvailable", "UseSingleStop", "UseDowngrade",
	"UseMilitaryRelease", "DayNightCycle", "FeaturesInUsermaps"};
---------------------------------------------------------------------------------------------------------------------------------------------------------------
for i = 1, #S6Patcher.Options do S6Patcher[S6Patcher.Options[i]] = Options.GetIntValue("S6Patcher", S6Patcher.Options[i], 0) ~= 0; end
---------------------------------------------------------------------------------------------------------------------------------------------------------------
S6Patcher.IsUsermap = ((function(_param) return (type(_param) == "table" and _param[1] == 3) or _param == 3 end)(Framework.GetCurrentMapTypeAndCampaignName()));
S6Patcher.DisableFeatures = S6Patcher.IsUsermap and not S6Patcher.FeaturesInUsermaps;
---------------------------------------------------------------------------------------------------------------------------------------------------------------
StartSimpleJobEx(function()
	local Time = Framework.GetTimeMs();
	if (g_FeedbackSpeech ~= nil) and (g_FeedbackSpeech.LastSpeechEndTime ~= nil) and ((Time + 6000) < g_FeedbackSpeech.LastSpeechEndTime) then
		g_FeedbackSpeech.LastSpeechEndTime = nil;
		XGUIEng.ShowWidget("/InGame/Root/Normal/AlignBottomRight/MapFrame/FeedbackSpeechText", 0);
		Framework.WriteToLog("S6Patcher: Caught Meldungsstau at " .. tostring(Time));
	end
end);
---------------------------------------------------------------------------------------------------------------------------------------------------------------
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
	local EntityID = GUI.GetSelectedEntity();
	if EntityID == 0 or EntityID == nil then
		return;
	end

	local Type = Logic.GetEntityType(EntityID);
	if Type == Entities.B_Cathedral_Big then
		XGUIEng.SetText(XGUIEng.GetCurrentWidgetID(), "{center}" .. XGUIEng.GetStringTableText("Names/" .. Logic.GetEntityTypeName(Type)));
	else
		S6Patcher.BuildingNameUpdate();
	end
end
-- ************************************************************************************************************************************************************* --
-- Enable a Day/Night Cycle																					 									 	 			 --
-- ************************************************************************************************************************************************************* --
if S6Patcher.DayNightCycle and not S6Patcher.DisableFeatures then
	if S6Patcher.GameCallback_Feedback_EndOfMonth == nil then
		S6Patcher.GameCallback_Feedback_EndOfMonth = GameCallback_Feedback_EndOfMonth;
	end
	GameCallback_Feedback_EndOfMonth = function(_LastMonth, _NewMonth)
		S6Patcher.GameCallback_Feedback_EndOfMonth(_LastMonth, _NewMonth);

		local Month = 3; -- May
		local Duration = 160; -- 2:40 minutes (ingame time, not real time)

		if (_NewMonth == Month) and (not Logic.IsWeatherEventActive()) then
			if S6Patcher.DayNightCycleEnvironmentSet == nil then
				S6Patcher.DayNightCycleEnvironmentSet = Display.AddEnvironmentSettingsSequence("ME_Special_Sundawn.xml");
			end

			Display.PlayEnvironmentSettingsSequence(S6Patcher.DayNightCycleEnvironmentSet, Duration);
		end
	end
end
-- ************************************************************************************************************************************************************* --
-- Make all Knights available in the expansion pack ("Eastern Realm")																					 		 --
-- ************************************************************************************************************************************************************* --
S6Patcher.GlobalScriptOverridden = false;
S6Patcher.OverrideGlobalScript = function(_selectedKnight)
	Framework.SetOnGameStartLuaCommand("return;"); -- free memory
	GUI.SendScriptCommand([[
		S6Patcher = S6Patcher or {};
		S6Patcher.PlayerID = ]] .. tostring(GUI.GetPlayerID()) .. [[;
		S6Patcher.SavedKnightID = Logic.GetKnightID(S6Patcher.PlayerID);

		S6Patcher.ReplaceKnight = function(_knightID, _newType)
			if _knightID == 0 or Logic.GetEntityType(_knightID) == _newType then
				return nil;
			end

			local posX, posY, posZ = Logic.EntityGetPos(_knightID);
			local PlayerID = Logic.EntityGetPlayer(_knightID);
			local Orientation = Logic.GetEntityOrientation(_knightID);
			local ScriptName = Logic.GetEntityName(_knightID);

			local ID = Logic.CreateEntity(_newType, posX, posY, Orientation, PlayerID);
			Logic.SetEntityName(ID, ScriptName);
			Logic.SetPrimaryKnightID(PlayerID, ID);
			Logic.DestroyEntity(_knightID);
			
			return ID;
		end
		
		S6Patcher.UpdateQuestData = function(_data, _savedKnightID, _result)
			if type(_data) == "table" then
				for Key, Value in ipairs(_data) do
					_data[Key] = S6Patcher.UpdateQuestData(Value, _savedKnightID, _result);
				end
			elseif _data == _savedKnightID then
				return _result;
			end
			
			return _data;
		end

		local Result = S6Patcher.ReplaceKnight(S6Patcher.SavedKnightID, ]] .. tostring(_selectedKnight) .. [[);
		if Result == nil or Result == S6Patcher.SavedKnightID then
			return;
		end
		Logic.ExecuteInLuaLocalState('LocalSetKnightPicture();');

		for _, Quest in ipairs(Quests) do
			if Quest.Objectives then
				for i = 1, Quest.Objectives[0] do
					Quest.Objectives[i].Data = S6Patcher.UpdateQuestData(Quest.Objectives[i].Data, S6Patcher.SavedKnightID, Result);
				end
			end
			if Quest.Triggers then
				for i = 1, Quest.Triggers[0] do
					Quest.Triggers[i].Data = S6Patcher.UpdateQuestData(Quest.Triggers[i].Data, S6Patcher.SavedKnightID, Result);
				end
			end
			if Quest.Rewards then
				for i = 1, Quest.Rewards[0] do
					Quest.Rewards[i].Data = S6Patcher.UpdateQuestData(Quest.Rewards[i].Data, S6Patcher.SavedKnightID, Result);
				end
			end
			if Quest.Reprisals then
				for i = 1, Quest.Reprisals[0] do
					Quest.Reprisals[i].Data = S6Patcher.UpdateQuestData(Quest.Reprisals[i].Data, S6Patcher.SavedKnightID, Result);
				end
			end
		end
	]]);
end

S6Patcher.IsCurrentMapEligibleForKnightReplacement = function()
	local Base = (Framework.GetGameExtraNo() < 1) == true;
	local Name = Framework.GetCurrentMapName();
	local Type, Campaign = Framework.GetCurrentMapTypeAndCampaignName();

	if Type == 0 or (Type == 3 and not S6Patcher.DisableFeatures) then
		local Names = {Framework.GetValidKnightNames(Name, Type)};
		if (#Names == 0) or (Base and #Names == 6) then -- No custom ValidKnightNames in info.xml
			return true;
		end
	end

	return false;
end

S6Patcher.ShownFirstAbilityMessage = false;
S6Patcher.EnableSpecialKnights = function()
	g_MilitaryFeedback.Knights[Entities.U_KnightSabatta] = "H_Knight_Sabatt";
	g_HeroAbilityFeedback.Knights[Entities.U_KnightSabatta] = "Sabatta";

	g_MilitaryFeedback.Knights[Entities.U_KnightRedPrince] = "H_Knight_RedPrince";
	g_HeroAbilityFeedback.Knights[Entities.U_KnightRedPrince] = "RedPrince";

	if S6Patcher.GetKnightAbilityAndIcons == nil then
		S6Patcher.GetKnightAbilityAndIcons = GUI_Knight.GetKnightAbilityAndIcons;
	end
	GUI_Knight.GetKnightAbilityAndIcons = function(_KnightID)
		local KnightType = Logic.GetEntityType(_KnightID)
		if KnightType == Entities.U_KnightSabatta then
			return Abilities.AbilityConvert, {11, 6};
		elseif KnightType == Entities.U_KnightRedPrince then
			return Abilities.AbilityFood, {2, 10};
		else
			return S6Patcher.GetKnightAbilityAndIcons(_KnightID);
		end
	end

	if S6Patcher.StartAbilityClicked == nil then
		S6Patcher.StartAbilityClicked = GUI_Knight.StartAbilityClicked;
	end
	GUI_Knight.StartAbilityClicked = function(_Ability)
		local KnightID = GUI.GetSelectedEntity();
		if KnightID == nil or Logic.IsKnight(KnightID) == false then
			return;
		end

		if Logic.GetEntityType(KnightID) == Entities.U_KnightRedPrince then
			if not S6Patcher.ShownFirstAbilityMessage then
				StartKnightVoiceForActionSpecialAbility(Entities.U_KnightRedPrince);
			else
				HeroAbilityFeedback(KnightID);
			end

			Sound.FXPlay2DSound("ui\\menu_click");
			GUI.StartKnightAbility(KnightID, Abilities.AbilityFood);
		else
			return S6Patcher.StartAbilityClicked(_Ability);
		end
	end

	if S6Patcher.GameCallback_Feedback_EntityHurt == nil then
		S6Patcher.GameCallback_Feedback_EntityHurt = GameCallback_Feedback_EntityHurt;
	end
	GameCallback_Feedback_EntityHurt = function(_HurtPlayerID, _HurtEntityID, _HurtingPlayerID, _HurtingEntityID, _DamageReceived, _DamageDealt)
		local Type = Logic.GetEntityType(_HurtingEntityID);
		if Type == Entities.U_KnightSabatta then
			if Logic.GetHeadquarters(_HurtPlayerID) ~= 0 then
				StartKnightVoiceForActionSpecialAbility(Type);
			end
		end

		return S6Patcher.GameCallback_Feedback_EntityHurt(_HurtPlayerID, _HurtEntityID, _HurtingPlayerID, _HurtingEntityID, _DamageReceived, _DamageDealt);
	end

	if S6Patcher.StartKnightVoiceForPermanentSpecialAbility == nil then
		S6Patcher.StartKnightVoiceForPermanentSpecialAbility = StartKnightVoiceForPermanentSpecialAbility;
	end
	StartKnightVoiceForPermanentSpecialAbility = function(_KnightType)
		local Type = Logic.GetEntityType(Logic.GetKnightID(GUI.GetPlayerID()));

		if _KnightType == Entities.U_KnightPlunder and Type == Entities.U_KnightRedPrince then
			_KnightType = Entities.U_KnightRedPrince;
		elseif _KnightType == Entities.U_KnightTrading and Type == Entities.U_KnightSabatta then
			_KnightType = Entities.U_KnightSabatta;
		end

		return S6Patcher.StartKnightVoiceForPermanentSpecialAbility(_KnightType);
	end
end

if S6Patcher.ExtendedKnightSelection
	and S6Patcher.SelectedKnight ~= nil
	and S6Patcher.IsCurrentMapEligibleForKnightReplacement()
	and (not S6Patcher.GlobalScriptOverridden)
	and (not Framework.IsNetworkGame())
	then

	if S6Patcher.SpecialKnightsAvailable then
		S6Patcher.EnableSpecialKnights();
	end
	S6Patcher.OverrideGlobalScript(S6Patcher.SelectedKnight);

	if S6Patcher.RestartMap == nil then
		S6Patcher.RestartMap = Framework.RestartMap;
	end
	Framework.RestartMap = function(_knightType)
		Framework.SetOnGameStartLuaCommand("S6Patcher = S6Patcher or {};S6Patcher.SelectedKnight = " .. tostring(_knightType) .. ";");
		S6Patcher.RestartMap(_knightType);
	end

	S6Patcher.GlobalScriptOverridden = true;
end
-- ************************************************************************************************************************************************************* --
-- SingleStopButtons on Buildings																														 		 --
-- ************************************************************************************************************************************************************* --
if S6Patcher.UseSingleStop and not S6Patcher.DisableFeatures then
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
if S6Patcher.UseDowngrade and not S6Patcher.DisableFeatures then
	GUI_BuildingButtons.GateOpenCloseClicked = function()
		local PlayerID = GUI.GetPlayerID();
		local EntityID = GUI.GetSelectedEntity();
		local Value = Logic.GetEntityHealth(EntityID) - (Logic.GetEntityMaxHealth(EntityID) * 0.5);

		Sound.FXPlay2DSound("ui\\menu_click");
		GUI.DeselectEntity(EntityID);
		GUI.SendScriptCommand([[
			local ID = ]] .. tostring(EntityID) .. [[;
			local posX, posY = Logic.GetEntityPosition(ID);
			Logic.Lightning(posX, posY);
			Logic.HurtEntity(ID, ]] .. tostring(math.ceil(Value)) .. [[);
		]]);
	end

	GUI_BuildingButtons.GateOpenCloseUpdate = function()
		local CurrentWidgetID = XGUIEng.GetCurrentWidgetID();
		local EntityID = GUI.GetSelectedEntity();

		if EntityID == nil
			or (Logic.IsEntityInCategory(EntityID, EntityCategories.CityBuilding) == 0 and Logic.IsEntityInCategory(EntityID, EntityCategories.OuterRimBuilding) == 0)
			or Logic.IsConstructionComplete(EntityID) == 0
			or Logic.IsBurning(EntityID) == true
			or Logic.CanCancelKnockDownBuilding(EntityID) == true
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
if S6Patcher.UseMilitaryRelease and not S6Patcher.DisableFeatures then
	if S6Patcher.DismountClicked == nil then
		S6Patcher.DismountClicked = GUI_Military.DismountClicked;
	end
	GUI_Military.DismountClicked = function()
		local PlayerID = GUI.GetPlayerID();
		local EntityID = GUI.GetSelectedEntity();
		local GuardedEntityID = Logic.GetGuardedEntityID(EntityID);

		if EntityID ~= 0 and GuardedEntityID == 0 and Logic.IsLeader(EntityID) == 1 then
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

S6Patcher.CanDisplayDismissButton = function()
	local EntityID = GUI.GetSelectedEntity();
	return (EntityID ~= 0) and (Logic.GetGuardedEntityID(EntityID) == 0) and (Logic.GetEntityType(EntityID) == Entities.U_MilitaryLeader);
end

if S6Patcher.GameCallback_GUI_SelectionChanged == nil then
	S6Patcher.GameCallback_GUI_SelectionChanged = GameCallback_GUI_SelectionChanged;
end
GameCallback_GUI_SelectionChanged = function(_Source)
	S6Patcher.GameCallback_GUI_SelectionChanged(_Source);

	if S6Patcher.UseSingleStop or S6Patcher.UseDowngrade then
		XGUIEng.ShowWidget("/InGame/Root/Normal/BuildingButtons/GateAutoToggle", 1); -- Unused in the game
		XGUIEng.ShowWidget("/InGame/Root/Normal/BuildingButtons/GateOpenClose", 1); -- Unused in the game
	end
end
-- ************************************************************************************************************************************************************* --
-- Override Multiselection																																 		 --
-- ************************************************************************************************************************************************************* --
if not S6Patcher.DisableFeatures then
	S6Patcher.LeaderSortOrderOriginal = {unpack(LeaderSortOrder)};
	S6Patcher.LeaderSortOrderAmmunition = {unpack(LeaderSortOrder)};
	S6Patcher.LeaderSortOrderThiefAmmunition = {unpack(LeaderSortOrder)};
	S6Patcher.LeaderSortOrderNoThief = {unpack(LeaderSortOrder)};

	S6Patcher.LeaderSortOrderThiefAmmunition[#S6Patcher.LeaderSortOrderThiefAmmunition + 1] = Entities.U_AmmunitionCart;
	for Key, Value in pairs(S6Patcher.LeaderSortOrderNoThief) do
		if Value == Entities.U_Thief then
			S6Patcher.LeaderSortOrderNoThief[Key] = nil;
			S6Patcher.LeaderSortOrderAmmunition[Key] = Entities.U_AmmunitionCart;
		end
	end

	if S6Patcher.SelectAllPlayerUnitsClicked == nil then
		S6Patcher.SelectAllPlayerUnitsClicked = GUI_MultiSelection.SelectAllPlayerUnitsClicked;
	end
	GUI_MultiSelection.SelectAllPlayerUnitsClicked = function()
		local Shift = XGUIEng.IsModifierPressed(Keys.ModifierShift) == true;
		local Control = XGUIEng.IsModifierPressed(Keys.ModifierControl) == true;

		if Shift and Control then
			LeaderSortOrder = S6Patcher.LeaderSortOrderAmmunition;
		elseif Shift and not Control then
			LeaderSortOrder = S6Patcher.LeaderSortOrderThiefAmmunition;
		elseif not Shift and Control then
			LeaderSortOrder = S6Patcher.LeaderSortOrderNoThief;
		else
			LeaderSortOrder = S6Patcher.LeaderSortOrderOriginal;
		end

		S6Patcher.SelectAllPlayerUnitsClicked();
	end
end
-- ************************************************************************************************************************************************************* --
-- Some Helpers																																 					 --
-- ************************************************************************************************************************************************************* --
if S6Patcher.SetNameAndDescription == nil then
	S6Patcher.SetNameAndDescription = GUI_Tooltip.SetNameAndDescription;
end
GUI_Tooltip.SetNameAndDescription = function(_TooltipNameWidget, _TooltipDescriptionWidget, _OptionalTextKeyName, _OptionalDisabledTextKeyName, _OptionalMissionTextFileBoolean)
	if not S6Patcher.DisableFeatures then
		if S6Patcher.UseDowngrade and _OptionalTextKeyName == "DowngradeButton" then
			local Title = {de = "Rückbau", en = "Downgrade"};
			local Text = {de = "- Baut das Gebäude um eine Stufe zurück", en = "- Downgrades the building by one level"};
			S6Patcher.SetTooltip(_TooltipNameWidget, _TooltipDescriptionWidget, S6Patcher.GetLocalizedText(Title), S6Patcher.GetLocalizedText(Text));
			return;
		elseif S6Patcher.UseMilitaryRelease and XGUIEng.GetCurrentWidgetID() == XGUIEng.GetWidgetID("/InGame/Root/Normal/AlignBottomRight/DialogButtons/Military/Dismount") then
			if S6Patcher.CanDisplayDismissButton() then
				local Title = XGUIEng.GetStringTableText("UI_Texts/MainMenuMultiTeamKickUser_center"); -- "Mitglied entlassen"
				local Text = {de = "- Entlässt Soldaten der Reihe nach", en = "- Dismisses soldiers one after another"};
				S6Patcher.SetTooltip(_TooltipNameWidget, _TooltipDescriptionWidget, Title, S6Patcher.GetLocalizedText(Text));
				return;
			end
		end
	end

	if S6Patcher.SpecialKnightsAvailable then
		if _OptionalTextKeyName == "AbilityConvert" then
			local EntityID = GUI.GetSelectedEntity();
			if (EntityID ~= 0) and (Logic.GetEntityType(EntityID) == Entities.U_KnightSabatta) then
				local Title = string.gsub(XGUIEng.GetStringTableText("UI_ObjectNames/" .. _OptionalTextKeyName), "Hakim", "Sabatt");
				local Text = XGUIEng.GetStringTableText("UI_ObjectDescription/".. _OptionalTextKeyName);

				local Disabled = "";
				if _OptionalDisabledTextKeyName ~= nil then
					Disabled = "{cr}" .. XGUIEng.GetStringTableText("UI_ButtonDisabled/" .. _OptionalDisabledTextKeyName);
				end

				S6Patcher.SetTooltip(_TooltipNameWidget, _TooltipDescriptionWidget, Title, Text .. "{cr}{@color:220, 0, 0}" .. Disabled .. "{@color:none}");
				return;
			end
		elseif _OptionalTextKeyName == "AbilityFood" then
			local EntityID = GUI.GetSelectedEntity();
			if (EntityID ~= 0) and (Logic.GetEntityType(EntityID) == Entities.U_KnightRedPrince) then
				local Title = XGUIEng.GetStringTableText("UI_ObjectNames/AbilityPlagueRedPrince");
				local Text = XGUIEng.GetStringTableText("UI_ObjectDescription/AbilityPlagueRedPrince");

				local Disabled = "";
				if _OptionalDisabledTextKeyName ~= nil then
					Disabled = "{cr}" .. XGUIEng.GetStringTableText("UI_ButtonDisabled/" .. _OptionalDisabledTextKeyName);
				end

				S6Patcher.SetTooltip(_TooltipNameWidget, _TooltipDescriptionWidget, Title, Text .. "{@color:220, 0, 0}" .. Disabled .. "{@color:none}");
				return;
			end
		end
	end

	return S6Patcher.SetNameAndDescription(_TooltipNameWidget, _TooltipDescriptionWidget, _OptionalTextKeyName, _OptionalDisabledTextKeyName, _OptionalMissionTextFileBoolean);
end
S6Patcher.SetTooltip = function(_TooltipNameWidget, _TooltipDescriptionWidget, _Title, _Text)
	XGUIEng.SetText(_TooltipNameWidget, "{center}" .. _Title);
	XGUIEng.SetText(_TooltipDescriptionWidget, _Text);

	local Height = XGUIEng.GetTextHeight(_TooltipDescriptionWidget, true)
	local W, H = XGUIEng.GetWidgetSize(_TooltipDescriptionWidget)

	XGUIEng.SetWidgetSize(_TooltipDescriptionWidget, W, Height)
end
S6Patcher.GetLocalizedText = function(_text) return (Network.GetDesiredLanguage() == "de" and _text.de) or _text.en; end;
-- #EOF
