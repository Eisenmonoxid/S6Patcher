-- UserScriptGlobal by Eisenmonoxid - S6Patcher --
-- Find latest S6Patcher version here: https://github.com/Eisenmonoxid/S6Patcher
S6Patcher = S6Patcher or {};
S6Patcher.DisableFeatures = (function(_param) return type(_param) == "table" and _param[1] == 3 or _param == 3 end)(Framework.GetCurrentMapTypeAndCampaignName());
S6Patcher.GetLocalizedText = function(_text) return (Network.GetDesiredLanguage() == "de" and  _text.de) or _text.en; end
-- ************************************************************************************************************************************************************* --
-- "B_NPC_Barracks_ME" will now correctly respawn soldiers																					 					 --
-- ************************************************************************************************************************************************************* --
if S6Patcher.GameCallback_OnBuildingConstructionComplete == nil then
	S6Patcher.GameCallback_OnBuildingConstructionComplete = GameCallback_OnBuildingConstructionComplete;
end
GameCallback_OnBuildingConstructionComplete = function(_PlayerID, _EntityID)
	S6Patcher.GameCallback_OnBuildingConstructionComplete(_PlayerID, _EntityID);
	
	local EntityType = Logic.GetEntityType(_EntityID);
	if EntityType == Entities.B_NPC_Barracks_ME then
		Logic.RespawnResourceSetMaxSpawn(_EntityID, 0.01);
		Logic.RespawnResourceSetMinSpawn(_EntityID, 0.01);
	end
end
(function(_entityTypes)
	for Key, Value in pairs(_entityTypes) do
		Logic.RespawnResourceSetMaxSpawn(Value, 0.01);
		Logic.RespawnResourceSetMinSpawn(Value, 0.01);
	end
end)(Logic.GetEntitiesOfType(Entities.B_NPC_Barracks_ME));
-- ************************************************************************************************************************************************************* --
-- Add salt and dye to city storehouse slots																					 								 --
-- ************************************************************************************************************************************************************* --
(function()
	for i = 1, 8 do
		local StorehouseID = Logic.GetStoreHouse(i);
		if StorehouseID ~= 0 then
			Logic.AddGoodToStock(StorehouseID, Goods.G_Salt, 0, true, true);
			Logic.AddGoodToStock(StorehouseID, Goods.G_Dye, 0, true, true);
		end
	end
end)();
-- ************************************************************************************************************************************************************* --
-- Some campaign fixes																					 														 --
-- ************************************************************************************************************************************************************* --
S6Patcher.ReplaceKnight = S6Patcher.ReplaceKnight or function(_knightID, _newType)
	if Logic.GetEntityType(_knightID) == _newType then
		return;
	end

	local posX, posY, posZ = Logic.EntityGetPos(_knightID);
	local PlayerID = Logic.EntityGetPlayer(_knightID);
	local Orientation = Logic.GetEntityOrientation(_knightID);
	local ScriptName = Logic.GetEntityName(_knightID);

	local ID = Logic.CreateEntity(_newType, posX, posY, Orientation, PlayerID);
	Logic.SetEntityName(ID, ScriptName);
	Logic.SetPrimaryKnightID(PlayerID, ID);
	Logic.DestroyEntity(_knightID);
end
(function()
	if Framework.GetCampaignName() == "c00" and Framework.GetCampaignMap() == "c00_m16_Rossotorres" then
		-- Interrupt Quests running
		local Found = FindQuestsByName("HiddenQuest_NPCMarcusMustSurvive", false);
		if #Found > 0 then
			for _, Quest in ipairs(Found) do
				Quest:Interrupt();
			end
		end
	
		-- Update Knight Entities and Heads
		local Knights = {
			{Index = HarbourPlayerKnight, 		pID = HarborPlayerID}, 
			{Index = GranCastillaPlayerKnight, 	pID = GranCastillaPlayerID}, 
			{Index = MonasterioPlayerKnight, 	pID = MonasterioPlayerID}
		};
			
		for i = 1, #Knights do
			if Knights[i].Index ~= nil and Knights[i].pID ~= nil then
				S6Patcher.ReplaceKnight(Logic.GetKnightID(Knights[i].pID), Knights[i].Index.Type);
				CreateQuestToProtectKnight(Knights[i].pID);
			end
		end
	elseif Framework.GetCampaignName() == "c00" and Framework.GetCampaignMap() == "c00_m03_Gallos" then
		Logic.ExecuteInLuaLocalState([[
			GUI.SetPlayerName(8, XGUIEng.GetStringTableText("UI_ObjectNames/B_NPC_ShipsStorehouse"));
		]]);
	elseif Framework.GetCampaignName() == "c00" and Framework.GetCampaignMap() == "c00_m11_Tios" then
		StartFlexibalPlayerVoiceAfterOneSecond = function()
			if Logic.GetTime() >= FlexibalPlayerVoiceStart + 1 then
				SendVoiceMessage(FlexibleSpeakerPlayerID, FlexibalPlayerVoiceText);
				FlexibleSpeakerPlayerID = SetupPlayer(FlexibleSpeakerPlayerID, "H_Knight_RedPrince", "Red Prince", "VillageColor2");
				return true;
			end
		end
	end
end)();
-- #EOF