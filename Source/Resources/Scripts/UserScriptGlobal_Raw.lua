-- UserScriptGlobal by Eisenmonoxid - S6Patcher --
-- Find latest S6Patcher version here: https://github.com/Eisenmonoxid/S6Patcher
S6Patcher = S6Patcher or {
	DisableFeatures = (NEP or QSB) and true or false,
};
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
	
	for Key, Value in pairs(Logic.GetEntitiesOfType(Entities.B_NPC_Barracks_ME)) do
		Logic.RespawnResourceSetMaxSpawn(Value, 0.01);
		Logic.RespawnResourceSetMinSpawn(Value, 0.01);
	end
end
-- ************************************************************************************************************************************************************* --
-- Add salt and dye to city storehouse slots																					 								 --
-- ************************************************************************************************************************************************************* --
for i = 1, 8 do
	local StorehouseID = Logic.GetStoreHouse(i);
	if StorehouseID ~= 0 then
		Logic.AddGoodToStock(StorehouseID, Goods.G_Salt, 0, true, true);
		Logic.AddGoodToStock(StorehouseID, Goods.G_Dye, 0, true, true);
	end
end
S6Patcher.GetLocalizedText = function(_text) return (Network.GetDesiredLanguage() == "de" and  _text.de) or _text.en; end
-- #EOF