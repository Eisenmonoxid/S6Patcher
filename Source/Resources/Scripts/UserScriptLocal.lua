-- UserScriptLocal by Eisenmonoxid - S6Patcher --
-- Find latest S6Patcher version here: https://github.com/Eisenmonoxid/S6Patcher
S6Patcher = S6Patcher or {}
S6Patcher.OverrideSelectionChanged = false;
S6Patcher.GlobalDowngradeCosts = 100;
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
		local Language = Network.GetDesiredLanguage();
		if Language == "de" then
			XGUIEng.SetText(WidgetID, "{center}Kathedrale");
		elseif Language == "en" then
			XGUIEng.SetText(WidgetID, "{center}Cathedral");
		end
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

if Options.GetIntValue("S6Patcher", "ExtendedKnightSelection", 0) ~= 0
	and (Framework.GetGameExtraNo() >= 1) 
	and (not Framework.IsNetworkGame())
	and (not S6Patcher.GlobalScriptOverridden)
	and (S6Patcher.SelectedKnight ~= nil)
	and (S6Patcher.IsCurrentMapEligibleForKnightReplacement() == true) then
	
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
if Options.GetIntValue("S6Patcher", "UseSingleStop", 0) ~= 0 then
	S6Patcher.OverrideSelectionChanged = true;
	
	GUI_BuildingButtons.GateAutoToggleClicked = function()
		Sound.FXPlay2DSound("ui\\menu_click");
		local EntityID = GUI.GetSelectedEntity();
		GUI.SetStoppedState(EntityID, not Logic.IsBuildingStopped(EntityID));
	end

	GUI_BuildingButtons.GateAutoToggleMouseOver = function()
		local EntityID = GUI.GetSelectedEntity();
		local Stopped = Logic.IsBuildingStopped(EntityID);
		
		local TooltipTextKey;
		if Stopped then
			TooltipTextKey = "StartBuilding";
		else
			TooltipTextKey = "StopBuilding";
		end

		GUI_Tooltip.TooltipNormal(TooltipTextKey);
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

		local Stopped = Logic.IsBuildingStopped(EntityID);
		if Stopped then
			SetIcon(CurrentWidgetID, {4, 12});
		else
			SetIcon(CurrentWidgetID, {4, 13});
		end
		
		XGUIEng.ShowWidget(CurrentWidgetID, 1);
	end
end
-- ************************************************************************************************************************************************************* --
-- DowngradeButton on Buildings																															 		 --
-- ************************************************************************************************************************************************************* --
if Options.GetIntValue("S6Patcher", "UseDowngrade", 0) ~= 0 then
	S6Patcher.OverrideSelectionChanged = true;
	
	GUI_BuildingButtons.GateOpenCloseClicked = function()
		local PlayerID = GUI.GetPlayerID();
		local EntityID = GUI.GetSelectedEntity();

		local CanBuyBoolean, CanNotBuyString = AreCostsAffordable({Goods.G_Gold, S6Patcher.GlobalDowngradeCosts});
		if CanBuyBoolean == true then
			Sound.FXPlay2DSound("ui\\menu_click");
			GUI.DeselectEntity(EntityID);
			GUI.RemoveGoodFromStock(Logic.GetHeadquarters(PlayerID), Goods.G_Gold, S6Patcher.GlobalDowngradeCosts);
			GUI.SendScriptCommand("Logic.HurtEntity("..EntityID..", ("..Logic.GetEntityHealth(EntityID).." - ("..Logic.GetEntityMaxHealth(EntityID).."/2)))");
		else
			Message(CanNotBuyString);
		end
		--if g_OnGameStartPresentationMode == true or XGUIEng.IsModifierPressed(Keys.ModifierControl) then -- TODO: Upgrade all buildings of type with modifier pressed???
		--    GUI_BuildingButtons.GatesAllOpenCloseClicked()
		--end
	end

	GUI_BuildingButtons.GateOpenCloseUpdate = function()
		local CurrentWidgetID = XGUIEng.GetCurrentWidgetID();
		local EntityID = GUI.GetSelectedEntity();
		
		if EntityID == nil 
			or Logic.IsBuilding(EntityID) == 0 
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

	function GUI_BuildingButtons.GateOpenCloseMouseOver()	
		local TooltipTextKey;
		if Logic.IsGateOpen(EntityID) == true then
			TooltipTextKey = "GateClose"; -- TODO
		else
			TooltipTextKey = "GateOpen"; -- TODO
		end
		
		GUI_Tooltip.TooltipBuy({Goods.G_Gold, S6Patcher.GlobalDowngradeCosts}, TooltipTextKey);
	end
end

if S6Patcher.GameCallback_GUI_SelectionChanged == nil then
	S6Patcher.GameCallback_GUI_SelectionChanged = GameCallback_GUI_SelectionChanged;
end
function GameCallback_GUI_SelectionChanged(_Source)
	S6Patcher.GameCallback_GUI_SelectionChanged();
	
	if S6Patcher.OverrideSelectionChanged then
		XGUIEng.ShowWidget("/InGame/Root/Normal/BuildingButtons/GateAutoToggle", 1); -- Unused in the game
		XGUIEng.ShowWidget("/InGame/Root/Normal/BuildingButtons/GateOpenClose", 1); -- Unused in the game
	end
end
-- ************************************************************************************************************************************************************* --
-- Release soldiers, siege engines and ammunition carts																									 		 --
-- ************************************************************************************************************************************************************* --
if Options.GetIntValue("S6Patcher", "UseMilitaryRelease", 0) ~= 0 then
	if S6Patcher.RefillClicked == nil then
		S6Patcher.RefillClicked = GUI_Military.RefillClicked;
	end
	GUI_Military.RefillClicked = function()
		local PlayerID = GUI.GetPlayerID();
		local LeaderID = GUI.GetSelectedEntity();
		local BarracksID = Logic.GetRefillerID(LeaderID);
		
		if BarracksID == 0 then
			local posX, posY;
			local Soldiers = {Logic.GetSoldiersAttachedToLeader(LeaderID)};
			if Soldiers[1] > 1 then
				local RetiredSoldier = table.remove(Soldiers, (Soldiers[1] + 1));
				posX, posY = Logic.GetEntityPosition(RetiredSoldier);
				GUI.SendScriptCommand([[
					Logic.CreateEffect(EGL_Effects.FXDie, ]] .. tostring(posX) .. [[, ]] .. tostring(posY) .. [[, ]] .. tostring(PlayerID) .. [[);
					Logic.DestroyEntity(]] .. tostring(RetiredSoldier) .. [[);
				]]);
			else
				GUI.ClearSelection();
				posX, posY = Logic.GetEntityPosition(LeaderID);		
				GUI.SendScriptCommand([[
					Logic.CreateEffect(EGL_Effects.FXDie, ]] .. tostring(posX) .. [[, ]] .. tostring(posY) .. [[, ]] .. tostring(PlayerID) .. [[);
					Logic.DestroyGroupByLeader(]] .. tostring(LeaderID) .. [[);
				]]);
				Soldiers = nil;
			end
			
			Sound.FXPlay2DSound("ui\\menu_click");
		else
			S6Patcher.RefillClicked();
		end
	end

	if S6Patcher.RefillMouseOver == nil then
		S6Patcher.RefillMouseOver = GUI_Military.RefillMouseOver;
	end
	GUI_Military.RefillMouseOver = function()
		local LeaderID = GUI.GetSelectedEntity();
		local BarracksID = Logic.GetRefillerID(LeaderID);
		
		if BarracksID ~= 0 then
			S6Patcher.RefillMouseOver();
			return;
		end
	end

	if S6Patcher.RefillUpdate == nil then
		S6Patcher.RefillUpdate = GUI_Military.RefillUpdate;
	end
	GUI_Military.RefillUpdate = function()
		local CurrentWidgetID = XGUIEng.GetCurrentWidgetID()
		local PlayerID = GUI.GetPlayerID()
		local LeaderID = GUI.GetSelectedEntity()
		local SelectedEntities = {GUI.GetSelectedEntities()}
		
		if LeaderID == nil
		or Logic.IsEntityInCategory(LeaderID, EntityCategories.Leader) == 0 
		or #SelectedEntities > 1 then
			XGUIEng.ShowWidget(CurrentWidgetID, 0);
			return;
		end
		
		local MaxSoldiers = Logic.LeaderGetMaxNumberOfSoldiers(LeaderID);
		local CurrentSoldiers = Logic.GetSoldiersAttachedToLeader(LeaderID);
		local RefillerID = Logic.GetRefillerID(LeaderID);
		
		if RefillerID ~= 0 then
			SetIcon(CurrentWidgetID, {12, 5});
			if CurrentSoldiers == MaxSoldiers then
				XGUIEng.DisableButton(CurrentWidgetID, 1);
			else
				XGUIEng.DisableButton(CurrentWidgetID, 0);
			end
		else
			SetIcon(CurrentWidgetID, {14, 12});
			XGUIEng.DisableButton(CurrentWidgetID, 0);
		end
	end
end
-- #EOF