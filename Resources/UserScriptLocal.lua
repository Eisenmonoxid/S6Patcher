-- UserScriptLocal by Eisenmonoxid - S6Patcher --
-- Find latest S6Patcher version here: https://github.com/Eisenmonoxid/S6Patcher
S6Patcher = S6Patcher or {}
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
	local Knight = Entities[S6Patcher.DefaultKnightNames[S6Patcher.SelectedKnight]];
	Framework.SetOnGameStartLuaCommand(""); -- free memory
	
	GUI.SendScriptCommand([[
		S6Patcher = S6Patcher or {}
		S6Patcher.UpdateKnight = function()
			local PlayerID = ]]..tostring(GUI.GetPlayerID())..[[;
			local MapType, CampaignName = Framework.GetCurrentMapTypeAndCampaignName();
			local MapName = Framework.GetCurrentMapName();
			local KnightNames = {Framework.GetValidKnightNames(MapName, MapType)};
			local KnightID = Logic.GetKnightID(PlayerID);
	
			if (#KnightNames == 0) and (KnightID ~= 0) then
				S6Patcher.ReplaceKnight(KnightID, ]]..tostring(Knight)..[[);
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
		end

		S6Patcher.UpdateKnight();
		Logic.ExecuteInLuaLocalState('LocalSetKnightPicture();');
	]]);
end
if Framework.GetGameExtraNo() >= 1 and not S6Patcher.GlobalScriptOverridden then
	S6Patcher.OverrideGlobalScript();
	
	if S6Patcher.RestartMap == nil then
		S6Patcher.RestartMap = Framework.RestartMap;
	end
	Framework.RestartMap = function(_knightType)
		local TypeName = Logic.GetEntityTypeName(_knightType);
		for i = 1, #S6Patcher.DefaultKnightNames do
			if S6Patcher.DefaultKnightNames[i] == TypeName then
				Framework.SetOnGameStartLuaCommand("S6Patcher = S6Patcher or {};S6Patcher.SelectedKnight = " .. tostring(i));
				break;
			end
		end
	
		S6Patcher.RestartMap(_knightType);
	end
	
	S6Patcher.GlobalScriptOverridden = true;
end
-- #EOF