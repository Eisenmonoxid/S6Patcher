-- UserScriptGlobal by Eisenmonoxid - S6Patcher --
-- Find latest S6Patcher version here: https://github.com/Eisenmonoxid/S6Patcher
S6Patcher = S6Patcher or {};
S6Patcher.BETA = false;
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

	local _, Campaign = Framework.GetCurrentMapTypeAndCampaignName();
	local Map = Framework.GetCurrentMapName();

	if Campaign ~= "c00" then
		return;
	end

	Framework.WriteToLog("S6Patcher: Applying " .. Campaign .. " fixes.");
	-- Fix incorrect enemy knight type
	local PossibleIDs = {RedPrincePlayerID, SabattaPlayerID, HusranPlayerID,
		CrimsonCitadelPlayerID, SidiKhemisRuinsPlayerID, KyrkasundPlayerID};
	for _, Value in pairs(PossibleIDs) do
		if Value ~= nil then
			local KnightID = Logic.GetKnightID(Value);
			local Type = Logic.GetEntityType(KnightID);

			if (KnightID ~= 0) and (Type ~= Entities.U_KnightRedPrince) and (Type ~= Entities.U_KnightSabatta) then
				local Replacement = string.find(Map, "m13") and Entities.U_KnightRedPrince or Entities.U_KnightSabatta;
				S6Patcher.ReplaceGlobalKnight(KnightID, Replacement);
			end
		end
	end

	-- Fix "Player x has no name" in various campaign maps
	Logic.ExecuteInLuaLocalState([[
		DisplayVoiceMessage_ORIG = GUI_Interaction.DisplayVoiceMessage;
		GUI_Interaction.DisplayVoiceMessage = function(_QuestIndex, _PlayerID, _MessageKey, _Actor, _ActorMood, _OptionalPlayVoice, _HidePortrait, _PlayerName)
			if string.find(_PlayerName, "has no name") then
				_PlayerName = GetPlayerName(_PlayerID);
			end	

			return DisplayVoiceMessage_ORIG(_QuestIndex, _PlayerID, _MessageKey, _Actor, _ActorMood, _OptionalPlayVoice, _HidePortrait, _PlayerName);
		end
	]]);

	if Map == "c00_m16_rossotorres" then
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
	elseif Map == "c00_m11_tios" then
		StartFlexibalPlayerVoiceAfterOneSecond = function()
			if Logic.GetTime() >= FlexibalPlayerVoiceStart + 1 then
				SendVoiceMessage(FlexibleSpeakerPlayerID, FlexibalPlayerVoiceText);
				FlexibleSpeakerPlayerID = SetupPlayer(FlexibleSpeakerPlayerID, "H_Knight_RedPrince", "Red Prince", "VillageColor2");
				return true;
			end
		end
	elseif Map == "c00_m15_vestholm" then
		local Entity = Logic.GetEntityIDByName("ReinforcementSpawn");
		local posX, posY = Logic.GetEntityPosition(Entity);
		Logic.DEBUG_SetSettlerPosition(Entity, posX + 250, posY);

		SetupPlayer(5, TraitorKnight.Face, "Village of Eastholm", "VillageColor2");
	elseif Map == "c00_m13_montecito" then
		SetDiplomacyState(RedPrincePlayerID, HarborBayPlayerID, DiplomacyStates.Enemy);
	elseif Map == "c00_m03_gallos" then
		Logic.ExecuteInLuaLocalState([[
			GUI.SetPlayerName(8, XGUIEng.GetStringTableText("UI_ObjectNames/B_NPC_ShipsStorehouse"));

			GenerateVoiceMessage_ORIG = GUI_Interaction.GenerateVoiceMessage;
			GUI_Interaction.GenerateVoiceMessage = function(_QuestIndex, _PlayerID, _MessageKey, _PlayDirectly, _Random, 
				_OptionalPlayVoice, _HidePortrait, _OnlyIfMapSpecificKeyExists)

				local Keys = {"Quest_GoToMonsteinStorehouse", "Quest_GoToCloister", "Quest_GoToRiedfurtStorehouse"};
				local UseMessagePatching = false;
				for i = 1, #Keys do
					if _MessageKey == Keys[i] then
						UseMessagePatching = true;
						break;
					end
				end

				if UseMessagePatching then
					local Quest = Quests[_QuestIndex];
					local JanusbergPlayerID = ]] .. tostring(JanusbergPlayerID) .. [[;
					local MonsteinPlayerID 	= ]] .. tostring(MonsteinPlayerID) .. [[;
					local RiedfurtPlayerID 	= ]] .. tostring(RiedfurtPlayerID) .. [[;
					local CloisterPlayerID 	= ]] .. tostring(CloisterPlayerID) .. [[;

					if (Quest.SendingPlayer == JanusbergPlayerID and Quest.Result == QuestResult.Success)
						or (Quest.State == QuestState.Active
							and (Quest.SendingPlayer == MonsteinPlayerID 
								or Quest.SendingPlayer == RiedfurtPlayerID 
								or Quest.SendingPlayer == CloisterPlayerID))
					then
						return false;
					end
				end

				return GenerateVoiceMessage_ORIG(_QuestIndex, _PlayerID, _MessageKey, _PlayDirectly, _Random, 
					_OptionalPlayVoice, _HidePortrait, _OnlyIfMapSpecificKeyExists);
			end
		]]);
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
S6Patcher.SpecialKnights = {}
S6Patcher.SpecialKnights.AbilityEffect = nil;
S6Patcher.SpecialKnights.AbilityEffectsOnMap = {};
S6Patcher.SpecialKnights.EffectCleanupJob = nil;

S6Patcher.SpecialKnights.KnightRedPrinceAbility = function(_playerID)
	local EffectTime = 12;
	local Area = 4500;
	local MaxSettlers = 32;

	local KnightID = Logic.GetKnightID(_playerID);
	if Logic.GetEntityType(KnightID) ~= Entities.U_KnightRedPrince then
		KnightID = S6Patcher.SpecialKnights.GetSpecialKnightID(_playerID, Entities.U_KnightRedPrince);
	end
	if KnightID == 0 then
		return;
	end

	local Position = {Logic.GetEntityPosition(KnightID)};
	local Entries = S6Patcher.SpecialKnights.GetEntityTypesInArea(KnightID, EntityCategories.Worker, Area);
	local EffectID = Logic.CreateEffectWithOrientation(EGL_Effects.E_HealingFX, Position[1], Position[2], 0, 1);
	S6Patcher.SpecialKnights.AbilityEffect = {EffectID, KnightID, Logic.GetTime() + 5, Position};

	local Counter = 0;
	for i = 1, #Entries do
		if not Logic.IsIll(Entries[i]) then
			Logic.MakeSettlerIll(Entries[i], true);
			Position = {Logic.EntityGetPos(Entries[i])};

			if i % 2 == 0 then
				EffectID = Logic.CreateEffectWithOrientation(EGL_Effects.E_SickBuilding, Position[1], Position[2], 0, 1);
				table.insert(S6Patcher.SpecialKnights.AbilityEffectsOnMap, {EffectID, Logic.GetTime() + EffectTime});
			end

			Counter = Counter + 1;
			if Counter >= MaxSettlers then
				break;
			end
		end
	end

	if S6Patcher.SpecialKnights.EffectCleanupJob == nil then
		S6Patcher.SpecialKnights.EffectCleanupJob = StartSimpleJobEx(S6Patcher.SpecialKnights.CleanupKnightEffects);
	end
end

S6Patcher.SpecialKnights.GetEntityTypesInArea = function(_entityID, _category, _area)
	local TypesInCategory = {Logic.GetEntityTypesInCategory(_category)};
	local Position = {Logic.EntityGetPos(_entityID)};
	local Entries = {};

	for i = 1, #TypesInCategory do
		local Type = TypesInCategory[i];
		if Type ~= nil and Type ~= 0 then
			local CurrentSettlers = {Logic.GetEntitiesInArea(TypesInCategory[i], Position[1], Position[2], _area)}; -- Max 16
			for j = 2, #CurrentSettlers do
				table.insert(Entries, CurrentSettlers[j]);
			end
		end
	end

	return Entries;
end

S6Patcher.SpecialKnights.GetSpecialKnightID = function(_playerID, _type)
	local Knights = {Logic.GetEntitiesOfType(_type)};
	for _, Value in pairs(Knights) do
		if Logic.EntityGetPlayer(Value) == _playerID then
			return Value;
		end
	end

	return 0;
end

S6Patcher.SpecialKnights.CleanupKnightEffects = function()
	local CurrentTime = Logic.GetTime();
	for Key, Value in pairs(S6Patcher.SpecialKnights.AbilityEffectsOnMap) do
		if Value[2] <= CurrentTime then
			Logic.DestroyEffect(Value[1]);
			S6Patcher.SpecialKnights.AbilityEffectsOnMap[Key] = nil;
		end
	end

	if S6Patcher.SpecialKnights.AbilityEffect ~= nil then
		local IsSamePosition = S6Patcher.SpecialKnights.CompareAbsolutePosition(
			{Logic.GetEntityPosition(S6Patcher.SpecialKnights.AbilityEffect[2])},
			S6Patcher.SpecialKnights.AbilityEffect[4]
		);

		if (S6Patcher.SpecialKnights.AbilityEffect[3] <= CurrentTime) or not IsSamePosition then
			Logic.DestroyEffect(S6Patcher.SpecialKnights.AbilityEffect[1]);
			S6Patcher.SpecialKnights.AbilityEffect = nil;
		end
	end
end

S6Patcher.SpecialKnights.CompareAbsolutePosition = function(_pos01, _pos02)
	return (_pos01[1] == _pos02[1]) and (_pos01[2] == _pos02[2]);
end

if S6Patcher.SpecialKnights.GameCallback_KnightAbilityUsed == nil then
	S6Patcher.SpecialKnights.GameCallback_KnightAbilityUsed = GameCallback_KnightAbilityUsed;
end
GameCallback_KnightAbilityUsed = function(_PlayerID, _KnightType)
	if _KnightType == Entities.U_KnightRedPrince then
		S6Patcher.SpecialKnights.KnightRedPrinceAbility(_PlayerID);
	end

	S6Patcher.SpecialKnights.GameCallback_KnightAbilityUsed(_PlayerID, _KnightType);
end
-- #EOF
