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
-- #EOF