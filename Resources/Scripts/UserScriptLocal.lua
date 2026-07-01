-- UserScriptLocal by Eisenmonoxid - S6Patcher --
-- Find latest S6Patcher version here: https://github.com/Eisenmonoxid/S6Patcher
S6Patcher = S6Patcher or {};
S6Patcher.BETA = false;
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
-- Update moving RTS camera every frame																 															 --
-- ************************************************************************************************************************************************************* --
if not S6Patcher.DisableFeatures then
	if S6Patcher.UpdateCamera == nil then
		S6Patcher.UpdateCamera = CameraAnimation.UpdateCamera;
	end
	CameraAnimation.UpdateCamera = function()
		if CameraAnimation.CameraMoveToX ~= nil and CameraAnimation.CameraMoveToY ~= nil then
			local LookAtX, LookAtY = Camera.RTS_GetLookAtPosition();
			local Distance = CameraAnimation.GetMoveToLength();

			if CameraAnimation.CameraMoveToLastDistance >= Distance then
				local DeltaTime = 0.01; -- Fixed
				local Speed = CameraAnimation.CameraMoveToSpeed;

				local NewX = LookAtX + CameraAnimation.DirectionX * DeltaTime * Speed;
				local NewY = LookAtY + CameraAnimation.DirectionY * DeltaTime * Speed;

				Camera.RTS_SetLookAtPosition(NewX, NewY)

				CameraAnimation.CameraStartTime = Logic.GetTimeMs() / 1000;
				CameraAnimation.CameraMoveToLastDistance = Distance;

				if XGUIEng.IsWidgetShown("/InGame/Root/Normal/AnimatedCameraMovement") == 0 then
					XGUIEng.ShowWidget("/InGame/Root/Normal/AnimatedCameraMovement", 1);
				end

				return;
			end
		end

		S6Patcher.UpdateCamera();
	end
end
-- ************************************************************************************************************************************************************* --
-- Fix Tutorial Marker being updated every frame																			 									 --
-- ************************************************************************************************************************************************************* --
S6Patcher.TutorialMarkerLastUpdateTime = 0;
if not S6Patcher.DisableFeatures then
	if S6Patcher.TutorialMarkerUpdate == nil then
		S6Patcher.TutorialMarkerUpdate = TutorialMarkerUpdate;
	end
	TutorialMarkerUpdate = function()
		local TimeDifference = 33.3; -- ms
		local CurrentTime = Framework.GetTimeMs();

		if S6Patcher.TutorialMarkerLastUpdateTime + TimeDifference > CurrentTime then
			return;
		end

		S6Patcher.TutorialMarkerLastUpdateTime = CurrentTime;
		S6Patcher.TutorialMarkerUpdate();
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
			S6Patcher.DayNightCycleEnvironmentSet = Display.AddEnvironmentSettingsSequence("ME_Special_Sundawn.xml");
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
			local Options = {Quest.Objectives, Quest.Triggers, Quest.Rewards, Quest.Reprisals};
			for _, Option in ipairs(Options) do
				if Option then
					for i = 1, Option[0] do
						Option[i].Data = S6Patcher.UpdateQuestData(Option[i].Data, S6Patcher.SavedKnightID, Result);
					end
				end
			end
		end
	]]);
end

S6Patcher.IsCurrentMapEligibleForKnightReplacement = function()
	local Base = (Framework.GetGameExtraNo() < 1) == true;
	local Name = Framework.GetCurrentMapName();
	local Type, _ = Framework.GetCurrentMapTypeAndCampaignName();

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
S6Patcher.DismountID = nil;
if S6Patcher.SetNameAndDescription == nil then
	S6Patcher.SetNameAndDescription = GUI_Tooltip.SetNameAndDescription;
end
GUI_Tooltip.SetNameAndDescription = function(_TooltipNameWidget, _TooltipDescriptionWidget, _OptionalTextKeyName, _OptionalDisabledTextKeyName, _OptionalMissionTextFileBoolean)
	if not S6Patcher.DisableFeatures then
		if S6Patcher.DismountID == nil then
			S6Patcher.DismountID = XGUIEng.GetWidgetID("/InGame/Root/Normal/AlignBottomRight/DialogButtons/Military/Dismount");
		end
		if S6Patcher.UseDowngrade and _OptionalTextKeyName == "DowngradeButton" then
			S6Patcher.SetTooltip(_TooltipNameWidget, _TooltipDescriptionWidget, S6Patcher.GetLocalizedText("DowngradeTitle"), S6Patcher.GetLocalizedText("DowngradeText"));
			return;
		elseif S6Patcher.UseMilitaryRelease and XGUIEng.GetCurrentWidgetID() == S6Patcher.DismountID then
			if S6Patcher.CanDisplayDismissButton() then
				local Title = XGUIEng.GetStringTableText("UI_Texts/MainMenuMultiTeamKickUser_center");
				S6Patcher.SetTooltip(_TooltipNameWidget, _TooltipDescriptionWidget, Title, S6Patcher.GetLocalizedText("ReleaseSoldiersText"));
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
					Disabled = XGUIEng.GetStringTableText("UI_ButtonDisabled/" .. _OptionalDisabledTextKeyName);
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

	local Height = XGUIEng.GetTextHeight(_TooltipDescriptionWidget, true);
	local W, H = XGUIEng.GetWidgetSize(_TooltipDescriptionWidget);

	XGUIEng.SetWidgetSize(_TooltipDescriptionWidget, W, Height);
end

S6Patcher.GetLocalizedText = function(_text)
	local Language = Network.GetDesiredLanguage();
	if S6Patcher.TranslatedStrings[Language] ~= nil then
		return S6Patcher.TranslatedStrings[Language][_text];
	else
		return S6Patcher.TranslatedStrings["en"][_text];
	end
end

S6Patcher.TranslatedStrings = {};
S6Patcher.TranslatedStrings["de"] =
{
	["DowngradeTitle"] 		= "Rückbau",
	["DowngradeText"] 		= "- Baut das Gebäude um eine Stufe zurück",
	["ReleaseSoldiersText"] = "- Entlässt Soldaten der Reihe nach",
	["Increase"]			= "Erhöhe",
	["Decrease"]			= "Verringere",
	["Exit"]				= "Beenden",
};
S6Patcher.TranslatedStrings["en"] =
{
	["DowngradeTitle"] 		= "Downgrade",
	["DowngradeText"] 		= "- Downgrades the building by one level",
	["ReleaseSoldiersText"] = "- Dismisses soldiers one after another",
	["Increase"]			= "Increase",
	["Decrease"]			= "Decrease",
	["Exit"]				= "Exit",
};
S6Patcher.TranslatedStrings["pl"] =
{
	["DowngradeTitle"]      = "Obniż poziom",
	["DowngradeText"]       = "- Obniża poziom budynku o jeden",
	["ReleaseSoldiersText"] = "- Zwalnia żołnierzy jeden po drugim",
};
S6Patcher.TranslatedStrings["fr"] =
{
	["DowngradeTitle"]      = "Rétrograder",
	["DowngradeText"]       = "- Rétrograde le bâtiment d’un niveau",
	["ReleaseSoldiersText"] = "- Renvoye les soldats un par un",
};
S6Patcher.TranslatedStrings["nl"] =
{
	["DowngradeTitle"]      = "Degraderen",
	["DowngradeText"]       = "- Degradeert het gebouw met één niveau",
	["ReleaseSoldiersText"] = "- Ontslaat soldaten één voor één",
};
S6Patcher.TranslatedStrings["ru"] =
{
	["DowngradeTitle"]      = "Понизить уровень",
	["DowngradeText"]       = "- Понижает уровень здания на один",
	["ReleaseSoldiersText"] = "- Увольняет солдат одного за другим",
};

-- ************************************************************************************************************************************************************* --
-- FPS Mode																																	 					 --
-- ************************************************************************************************************************************************************* --
if not S6Patcher.DisableFeatures and g_Throneroom == nil and not Framework.IsNetworkGame() then
	Input.KeyBindDown(Keys.Z,  "S6Patcher.FPSMode.Toggle()", 2); -- Main Binding
end

S6Patcher.FPSMode = {
	Enabled = false,
	IsMoving = false,

	CurrentPosX = 0,
	CurrentPosY = 0,
	CurrentPosZ = 0,

	Yaw = 0,
	Pitch = 0,
	Limit = math.rad(89),
	MouseSensitivity = 0.005,

	LastHeight = 0,
	LastMouseX = 0,
	LastMouseY = 0,
	LastFrameTime = 0,

	Directions = {
		Forward = 0,
		Backward = 1
	},

	StateModifiers = {
		MinFOV = 40,
		FOV = 75,
		MaxFOV = 120,

		MinViewDistance = 1000,
		ViewDistance = 15000,
		MaxViewDistance = 90000,
	},

	KeyMapping = {
		Forward = XGUIEng.GetStringTableText("KeyBindings/KeyMenuWeather");
		Backward = XGUIEng.GetStringTableText("KeyBindings/KeyBuildingStartStop");
		IncreaseFOV = XGUIEng.GetStringTableText("KeyBindings/KeyBuildingUpgrade");
		DecreaseFOV = XGUIEng.GetStringTableText("KeyBindings/KeyBuildTrail");
		IncreaseViewDistance = XGUIEng.GetStringTableText("KeyBindings/KeyBuildStreet");
		DecreaseViewDistance = XGUIEng.GetStringTableText("KeyBindings/KeyMenuProduction");
	},

	PlayerID = GUI.GetPlayerID(),
}
S6Patcher.FPSMode.CurrentDirection = S6Patcher.FPSMode.Directions.Forward;

S6Patcher.FPSMode.SetKeyBindings = function(_reset)
	local Key, FuncCall;

	Key = Keys[S6Patcher.FPSMode.KeyMapping.Forward];
	FuncCall = (_reset and "KeyBindings_MenuWeather()") or "S6Patcher.FPSMode.ToggleMovement(true, S6Patcher.FPSMode.Directions.Forward)";
	Input.KeyBindDown(Key, FuncCall, 2);
	Input.KeyBindUp(Key, "S6Patcher.FPSMode.ToggleMovement(false, S6Patcher.FPSMode.Directions.Forward)", 2); -- Returns anyway

	Key = Keys[S6Patcher.FPSMode.KeyMapping.Backward];
	Input.KeyBindDown(Key, "S6Patcher.FPSMode.ToggleMovement(true, S6Patcher.FPSMode.Directions.Backward)", 2);
	Input.KeyBindUp(Key, "S6Patcher.FPSMode.ToggleMovement(false, S6Patcher.FPSMode.Directions.Backward)", 2);

	Key = Keys[S6Patcher.FPSMode.KeyMapping.IncreaseFOV];
	FuncCall = (_reset and "KeyBindings_BuildingUpgrade()") or "S6Patcher.FPSMode.ModifyFieldOfView(false)";
	Input.KeyBindDown(Key, FuncCall, 2);

	Key = Keys[S6Patcher.FPSMode.KeyMapping.DecreaseFOV];
	FuncCall = (_reset and "KeyBindings_BuildTrail()") or "S6Patcher.FPSMode.ModifyFieldOfView(true)";
	Input.KeyBindDown(Key, FuncCall, 2);

	Key = Keys[S6Patcher.FPSMode.KeyMapping.IncreaseViewDistance];
	FuncCall = (_reset and "KeyBindings_BuildStreet()") or "S6Patcher.FPSMode.ModifyViewDistance(false)";
	Input.KeyBindDown(Key, FuncCall, 2);

	Key = Keys[S6Patcher.FPSMode.KeyMapping.DecreaseViewDistance];
	FuncCall = _reset and "KeyBindings_MenuProduction()" or "S6Patcher.FPSMode.ModifyViewDistance(true)";
	Input.KeyBindDown(Key, FuncCall, 2);
end

S6Patcher.FPSMode.ToggleMovement = function(_moving, _direction)
	if not S6Patcher.FPSMode.Enabled then
		return;
	end

	S6Patcher.FPSMode.IsMoving = _moving;
	S6Patcher.FPSMode.CurrentDirection = _direction;
end

if not S6Patcher.DisableFeatures and g_Throneroom == nil and not Framework.IsNetworkGame() then
	if S6Patcher.ThroneRoomCameraControl == nil then
		S6Patcher.ThroneRoomCameraControl = ThroneRoomCameraControl;
	end
	ThroneRoomCameraControl = function()
		local FPS = S6Patcher.FPSMode;

		if not FPS.Enabled then
			S6Patcher.ThroneRoomCameraControl();
			return;
		end

		local Time = Framework.GetTimeMs();
		local MouseX, MouseY = GUI.GetMousePosition();
		local CamX, CamY, CamZ = FPS.CurrentPosX, FPS.CurrentPosY, FPS.CurrentPosZ; -- Camera.ThroneRoom_GetPosition();

		--------------------------------------------------------------------------------------------------------
		local DeltaX = MouseX - FPS.LastMouseX;
		local DeltaY = MouseY - FPS.LastMouseY;

		local Sensitivity = FPS.MouseSensitivity;
		FPS.Yaw = FPS.Yaw - DeltaX * Sensitivity;
		FPS.Pitch = FPS.Pitch - DeltaY * Sensitivity;

		local Limit = FPS.Limit;
		if FPS.Pitch > Limit then
			FPS.Pitch = Limit;
		elseif FPS.Pitch < -Limit then
			FPS.Pitch = -Limit;
		end

		local Yaw = FPS.Yaw;
		local Pitch = FPS.Pitch;

		local CosPitch = math.cos(Pitch);
		local CosYaw = math.cos(Yaw);
		local SinYaw = math.sin(Yaw);

		local DirectionX = CosPitch * CosYaw;
		local DirectionY = CosPitch * SinYaw;
		local DirectionZ = math.sin(Pitch);

		Camera.ThroneRoom_SetLookAt(CamX + DirectionX * 1000, CamY + DirectionY * 1000, CamZ + DirectionZ * 1000);

		FPS.LastMouseX = MouseX;
		FPS.LastMouseY = MouseY;
		--------------------------------------------------------------------------------------------------------
		local LastFrameTime = FPS.LastFrameTime;
		local DeltaTime = 0;
		if LastFrameTime ~= 0 then
			DeltaTime = (Time - LastFrameTime) / 1000.0;
		end
		FPS.LastFrameTime = Time;

		if not FPS.IsMoving then
			return;
		end

		-- Units per Second
		local MoveSpeed = 600;
		local Distance = MoveSpeed * DeltaTime;
		if FPS.CurrentDirection == FPS.Directions.Forward then
			CamX = CamX + CosYaw * Distance;
			CamY = CamY + SinYaw * Distance;
		elseif FPS.CurrentDirection == FPS.Directions.Backward then
			CamX = CamX - CosYaw * Distance;
			CamY = CamY - SinYaw * Distance;
		end

		local Sector = Logic.GetPlayerSectorAtPosition(FPS.PlayerID, CamX, CamY);
		if Sector == 0 then
			return;
		end

		local TargetHeight = Display.GetTerrainHeight(CamX, CamY);
		if FPS.LastHeight == 0 then
			FPS.LastHeight = TargetHeight;
		end

		-- Smooth vertical Movement
		local FollowFactor = math.min(1, 8 * DeltaTime);
		FPS.LastHeight = FPS.LastHeight + (TargetHeight - FPS.LastHeight) * FollowFactor;

		local NewZ = FPS.LastHeight + 200;

		FPS.CurrentPosX = CamX
		FPS.CurrentPosY = CamY
		FPS.CurrentPosZ = NewZ

		Camera.ThroneRoom_SetPosition(CamX, CamY, NewZ);
		--------------------------------------------------------------------------------------------------------
	end
end

S6Patcher.FPSMode.Toggle = function()
	if S6Patcher.DisableFeatures or g_Throneroom ~= nil or Framework.IsNetworkGame() or CameraAnimation.IsRunning() then
		return;
	end

	if S6Patcher.FPSMode.Enabled then
		S6Patcher.FPSMode.Disable();
	else
		S6Patcher.FPSMode.Enable();
	end

	S6Patcher.FPSMode.Enabled = not S6Patcher.FPSMode.Enabled;
end

S6Patcher.FPSMode.ModifyFieldOfView = function(_decrease)
	if not S6Patcher.FPSMode.Enabled then
		return;
	end

	local Value = S6Patcher.FPSMode.StateModifiers.FOV + ((_decrease and -5) or 5);

	GUI.AddNote("FOV: " .. tostring(Value));
	if (Value <= S6Patcher.FPSMode.StateModifiers.MinFOV or Value >= S6Patcher.FPSMode.StateModifiers.MaxFOV) then
		return;
	end

	S6Patcher.FPSMode.StateModifiers.FOV = Value;
	Camera.ThroneRoom_SetFOV(S6Patcher.FPSMode.StateModifiers.FOV);
end

S6Patcher.FPSMode.ModifyViewDistance = function(_decrease)
	if not S6Patcher.FPSMode.Enabled then
		return;
	end

	local Value = S6Patcher.FPSMode.StateModifiers.ViewDistance + ((_decrease and -5000) or 5000);

	GUI.AddNote("Draw Distance: " .. tostring(Value));
	if (Value <= S6Patcher.FPSMode.StateModifiers.MinViewDistance or Value >= S6Patcher.FPSMode.StateModifiers.MaxViewDistance) then
		return;
	end

	S6Patcher.FPSMode.StateModifiers.ViewDistance = Value;
	Display.SetFarClipPlaneMinAndMax(S6Patcher.FPSMode.StateModifiers.ViewDistance, S6Patcher.FPSMode.StateModifiers.ViewDistance);
end

if not S6Patcher.DisableFeatures and g_Throneroom == nil and not Framework.IsNetworkGame() then
	if S6Patcher.GameCallback_GUI_SelectionChanged_FPSMode == nil then
		S6Patcher.GameCallback_GUI_SelectionChanged_FPSMode = GameCallback_GUI_SelectionChanged;
	end
	GameCallback_GUI_SelectionChanged = function(_Source)
		if S6Patcher.FPSMode.Enabled then
			GUI.ClearSelection();
		end

		S6Patcher.GameCallback_GUI_SelectionChanged_FPSMode(_Source);
	end
end

S6Patcher.FPSMode.Disable = function()
	Display.SetRenderSky(0);
	Display.SetRenderFogOfWar(1);
	Display.SetRenderBorderPins(1);
	Display.SetUserOptionOcclusionEffect(1);
	Display.SetRenderRoads(0);
	Display.UseStandardSettings();

	GUI.EnableBattleSignals(true);
    GUI.SetFeedbackSoundOutputState(1);
    GUI.PermitContextSensitiveCommandsInSelectionState();

	S6Patcher.FPSMode.ToggleGameWidgets(0);
	S6Patcher.FPSMode.SetKeyBindings(true);
	Display.SetFarClipPlaneMinAndMax(0, 0);

	Camera.SwitchCameraBehaviour(0);
end

S6Patcher.FPSMode.Enable = function()
	local FPS = S6Patcher.FPSMode;

	Display.SetRenderSky(1);
	Display.SetUserOptionOcclusionEffect(0);
	Display.SetRenderBorderPins(0);
	Display.SetRenderFogOfWar(0);
	Display.SetRenderRoads(1); -- Flickering
	Display.UseCloseUpSettings();

	Camera.SwitchCameraBehaviour(5);

	local posX, posY = Logic.GetBuildingApproachPosition(Logic.GetMarketplace(FPS.PlayerID));
	local posZ = Display.GetTerrainHeight(posX, posY);

	FPS.CurrentPosX = posX;
	FPS.CurrentPosY = posY;
	FPS.CurrentPosZ = posZ + 500;

	Camera.ThroneRoom_SetPosition(posX, posY, FPS.CurrentPosZ);
	Camera.ThroneRoom_SetLookAt(posX + 1000, posY + 1000, FPS.CurrentPosZ);
	Camera.ThroneRoom_SetFOV(FPS.StateModifiers.FOV);

	Display.SetFarClipPlaneMinAndMax(FPS.StateModifiers.ViewDistance, FPS.StateModifiers.ViewDistance);

	FPS.LastFrameTime = Framework.GetTimeMs();
	FPS.LastHeight = posZ;
	FPS.Pitch = 0;
	FPS.Yaw = math.pi / 4;

	FPS.ToggleGameWidgets(1);
	FPS.SetKeyBindings(false);

	GUI.ClearSelection();
    GUI.ClearNotes();
    GUI.ForbidContextSensitiveCommandsInSelectionState();
    GUI.SetFeedbackSoundOutputState(0);
    GUI.EnableBattleSignals(false);

	Game.GameTimeSetFactor(FPS.PlayerID, 1);

	local Increase = S6Patcher.GetLocalizedText("Increase");
	local Decrease = S6Patcher.GetLocalizedText("Decrease");
	local Exit = S6Patcher.GetLocalizedText("Exit");
	local Text =
		"--- First Person Mode ---{cr}{cr}" ..
		"{@color:255,0,0,255}- " .. Increase .. " FOV: " .. FPS.KeyMapping.IncreaseFOV .. "{cr}" ..
		"{@color:255,0,0,255}- " .. Decrease .. " FOV: " .. FPS.KeyMapping.DecreaseFOV .. "{cr}" ..
		"{@color:0,255,0,255}- " .. Increase .. " Draw Distance: " .. FPS.KeyMapping.IncreaseViewDistance .. "{cr}" ..
		"{@color:0,255,0,255}- " .. Decrease .. " Draw Distance: " .. FPS.KeyMapping.DecreaseViewDistance .. "{cr}" ..
		"{@color:0,0,255,255}- " .. Exit .. ": Z{cr}";
	GUI.AddNote(Text);
end

S6Patcher.FPSMode.ToggleGameWidgets = function(_show)
	local _showInv = (_show == 0 and 1) or 0;

	XGUIEng.ShowWidget("/InGame/ThroneRoom", _show);
	XGUIEng.ShowWidget("/InGame/ThroneRoom/Main/updater", _show);

	if _show then
		XGUIEng.PushPage("/InGame/ThroneRoom", false);
		XGUIEng.ShowWidget("/InGame/Root/Normal/AlignBottomLeft/SubTitles", 0);
	else
		XGUIEng.PopPage();
	end

	XGUIEng.ShowAllSubWidgets("/InGame/ThroneRoom/KnightInfo", 0);
	XGUIEng.ShowWidget("/InGame/ThroneRoom/Main/TitleContainer", 0);
    XGUIEng.ShowWidget("/InGame/ThroneRoom/Main/DialogTopChooseKnight", 0);
    XGUIEng.ShowWidget("/InGame/ThroneRoom/Main/DialogBottomRight3pcs", 0);
    XGUIEng.ShowWidget("/InGame/ThroneRoom/Main/KnightInfoButton", 0);
    XGUIEng.ShowWidget("/InGame/ThroneRoom/Main/Briefing", 0);
    XGUIEng.ShowWidget("/InGame/ThroneRoom/Main/MissionBriefing", 0);
	XGUIEng.ShowWidget("/InGame/ThroneRoom/Main/Skip", 0);
    XGUIEng.ShowWidget("/InGame/ThroneRoom/Main/StartButton", 0);
	XGUIEng.ShowWidget("/InGame/ThroneRoom/Main/BackButton", 0);

	XGUIEng.ShowWidget("/InGame/Root/3dOnScreenDisplay", _showInv);
    XGUIEng.ShowWidget("/InGame/Root/Normal", 1);
    XGUIEng.ShowWidget("/InGame/Root/Normal/TextMessages", 1);
    XGUIEng.ShowWidget("/InGame/Root/Normal/AlignBottomLeft/Message/MessagePortrait/SpeechStartAgainOrStop", _showInv);
    XGUIEng.ShowWidget("/InGame/Root/Normal/AlignBottomRight", _showInv);
    XGUIEng.ShowWidget("/InGame/Root/Normal/AlignTopRight", _showInv);
    XGUIEng.ShowWidget("/InGame/Root/Normal/AlignTopLeft", _showInv);
    XGUIEng.ShowWidget("/InGame/Root/Normal/AlignTopLeft/TopBar", _showInv);
    XGUIEng.ShowWidget("/InGame/Root/Normal/AlignTopLeft/TopBar/UpdateFunction", _showInv);
    XGUIEng.ShowWidget("/InGame/Root/Normal/AlignBottomLeft/Message/MessagePortrait/Buttons", _showInv);
    XGUIEng.ShowWidget("/InGame/Root/Normal/AlignTopLeft/QuestLogButton", _showInv);
    XGUIEng.ShowWidget("/InGame/Root/Normal/AlignTopLeft/QuestTimers", _showInv);
    XGUIEng.ShowWidget("/InGame/Root/Normal/AlignBottomLeft/Message", _showInv);
end

if not S6Patcher.DisableFeatures and g_Throneroom == nil and not Framework.IsNetworkGame() then
	if S6Patcher.KeyBindings_SaveGame == nil then
		S6Patcher.KeyBindings_SaveGame = KeyBindings_SaveGame;
	end
	KeyBindings_SaveGame = function()
		if S6Patcher.FPSMode.Enabled then
			return;
		end

		S6Patcher.KeyBindings_SaveGame();
	end
	if S6Patcher.GameCallback_Escape == nil then
		S6Patcher.GameCallback_Escape = GameCallback_Escape;
	end
	GameCallback_Escape = function()
		if S6Patcher.FPSMode.Enabled then
			S6Patcher.FPSMode.Toggle();
		end

		S6Patcher.GameCallback_Escape();
	end
	
	OnBackButtonPressed = function() end
	OnSkipButtonPressed = function() end
	ThroneRoomLeftClick = function() end
end
-- #EOF