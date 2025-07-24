-- UserScriptGlobal by Eisenmonoxid - S6Patcher --
-- Find latest S6Patcher version here: https://github.com/Eisenmonoxid/S6Patcher
S6Patcher = S6Patcher or {};
S6Patcher.DisableFeatures = (function(_param) return type(_param) == "table" and _param[1] == 3 or _param == 3 end)(Framework.GetCurrentMapTypeAndCampaignName());
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
S6Patcher.ReplaceGlobalKnight = function(_knightID, _newType)
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
	if g_GlobalThroneRoom ~= nil then
		return;
	end

	local Campaign = Framework.GetCampaignName();
	local Map = Framework.GetCampaignMap();
	
	if Campaign == "c00" and Map == "c00_m16_Rossotorres" then
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
				S6Patcher.ReplaceGlobalKnight(Logic.GetKnightID(Knights[i].pID), Knights[i].Index.Type);
				CreateQuestToProtectKnight(Knights[i].pID);
			end
		end
	elseif Campaign == "c00" and Map == "c00_m03_Gallos" then
		Logic.ExecuteInLuaLocalState([[
			GUI.SetPlayerName(8, XGUIEng.GetStringTableText("UI_ObjectNames/B_NPC_ShipsStorehouse"));
		]]);
	elseif Campaign == "c00" and Map == "c00_m11_Tios" then
		StartFlexibalPlayerVoiceAfterOneSecond = function()
			if Logic.GetTime() >= FlexibalPlayerVoiceStart + 1 then
				SendVoiceMessage(FlexibleSpeakerPlayerID, FlexibalPlayerVoiceText);
				FlexibleSpeakerPlayerID = SetupPlayer(FlexibleSpeakerPlayerID, "H_Knight_RedPrince", "Red Prince", "VillageColor2");
				return true;
			end
		end
	elseif Campaign == "c00" and Map == "c00_m15_Vestholm" then
		local Entity = Logic.GetEntityIDByName("ReinforcementSpawn");
		local posX, posY = Logic.GetEntityPosition(Entity);
		Logic.DEBUG_SetSettlerPosition(Entity, posX + 250, posY);
	end
end)();
-- ************************************************************************************************************************************************************* --
-- Fix Bandit Fireplace crashing the game																										  				 --
-- ************************************************************************************************************************************************************* --
ActivateFireplaceforBanditPack = function(_CampID)
	local BanditsPlayerID = Logic.EntityGetPlayer(_CampID);

	if g_Outlaws.Players[BanditsPlayerID][_CampID].CampFire == nil then
		local ApX, ApY = Logic.GetBuildingApproachPosition(_CampID);
		local PosX, PosY = Logic.GetEntityPosition(_CampID);

		local x = (ApX - PosX) * 1.3 + ApX;
		local y = (ApY - PosY) * 1.3 + ApY;

		local FireplaceType = Entities.D_X_Fireplace01;
		if Logic.IsEntityInCategory(_CampID, EntityCategories.Storehouse) == 1 then
			FireplaceType = Entities.D_X_Fireplace02;
		end

		g_Outlaws.Players[BanditsPlayerID][_CampID].CampFireType = FireplaceType;
		Logic.DestroyEntity(g_Outlaws.Players[BanditsPlayerID][_CampID].ExtinguishedFire);
		g_Outlaws.Players[BanditsPlayerID][_CampID].CampFire = Logic.CreateEntityOnUnblockedLand(FireplaceType, x, y, 0, 0);
		return true;
	end

	return false;
end
DisableFireplaceforBanditPack = function(_CampID)
	local BanditsPlayerID = Logic.EntityGetPlayer(_CampID);

	if g_Outlaws.Players[BanditsPlayerID][_CampID].CampFire ~= nil then      
		local x, y = Logic.GetEntityPosition(g_Outlaws.Players[BanditsPlayerID][_CampID].CampFire);      
		Logic.DestroyEntity(g_Outlaws.Players[BanditsPlayerID][_CampID].CampFire);

		if g_Outlaws.ReplaceCampType == nil then
			g_Outlaws.ReplaceCampType = {};
			g_Outlaws.ReplaceCampType[Entities.D_X_Fireplace01] = Entities.D_X_Fireplace01_Expired;
			g_Outlaws.ReplaceCampType[Entities.D_X_Fireplace02] = Entities.D_X_Fireplace02_Expired;
		end
		
		local FireplaceType = g_Outlaws.ReplaceCampType[g_Outlaws.Players[BanditsPlayerID][_CampID].CampFireType];
		g_Outlaws.Players[BanditsPlayerID][_CampID].ExtinguishedFire = Logic.CreateEntityOnUnblockedLand(FireplaceType, x, y, 0, 0);
		g_Outlaws.Players[BanditsPlayerID][_CampID].CampFire = nil;
	end
end
-- ************************************************************************************************************************************************************* --
-- Special Knight Abilities																										 								 --
-- ************************************************************************************************************************************************************* --
S6Patcher.AbilityEffectsOnMap = {};
S6Patcher.EffectCleanupJob = nil;
S6Patcher.KnightRedPrinceAbility = function(_playerID)
	local EffectTime = 15;
	local Area = 8000;
	local KnightID = Logic.GetKnightID(_playerID);
	local posX, posY, posZ = Logic.EntityGetPos(KnightID);
	local Entries = {Logic.GetEntitiesInArea(0, posX, posY, Area, 16)};
	
	if Entries[1] <= 0 then
		return;
	end
	
	for i = 2, Entries[1] do
		if Logic.IsEntityTypeInCategory(Logic.GetEntityType(Entries[i]), EntityCategories.Worker) == 1 and not Logic.IsIll(Entries[i]) then
			Logic.MakeSettlerIll(Entries[i], true);
			local Position = {Logic.EntityGetPos(Entries[i])};
			local EffectID = Logic.CreateEffectWithOrientation(EGL_Effects.E_SickBuilding, Position[1], Position[2], 0, 1);
			table.insert(S6Patcher.AbilityEffectsOnMap, {EffectID, Logic.GetTime() + EffectTime})
		end
	end
	
	if S6Patcher.EffectCleanupJob == nil then
		S6Patcher.EffectCleanupJob = StartSimpleJobEx(S6Patcher.CleanupKnightEffects);
	end
end
S6Patcher.CleanupKnightEffects = function()
	local CurrentTime = Logic.GetTime();
	for Key, Value in pairs(S6Patcher.AbilityEffectsOnMap) do
		if Value[2] <= CurrentTime then
			Logic.DestroyEffect(Value[1]);
			S6Patcher.AbilityEffectsOnMap[Key] = nil;
		end
	end
end
-- #EOF