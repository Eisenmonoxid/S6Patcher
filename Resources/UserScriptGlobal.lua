-- UserScriptGlobal by Eisenmonoxid - S6Patcher --
-- Find latest S6Patcher version here: https://github.com/Eisenmonoxid/S6Patcher
S6Patcher = S6Patcher or {}
-- ************************************************************************************************************************************************************* --
-- Make all Knights available in the expansion pack ("Eastern Realm")																					 		 --
-- ************************************************************************************************************************************************************* --
S6Patcher.UpdateKnight = function()
	local PlayerID = 1;
	local MapType, CampaignName = Framework.GetCurrentMapTypeAndCampaignName();
	local MapName = Framework.GetCurrentMapName();
	local KnightNames = {Framework.GetValidKnightNames(MapName, MapType)};
	
	if (#KnightNames == 0) and (Logic.GetKnightID(PlayerID) ~= 0) and (Logic.PlayerGetIsHumanFlag(PlayerID)) then
		if S6Patcher.SelectedKnight ~= nil then -- This value is set from the game code
			S6Patcher.ReplaceKnight(Logic.GetKnightID(1), Entities[MPDefaultKnightNames[S6Patcher.SelectedKnight]]);
		end
	end
	
	Framework.SetOnGameStartLuaCommand(""); -- free memory
	Logic.ExecuteInLuaLocalState([[LocalSetKnightPicture();]]);
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
if Framework.GetGameExtraNo() >= 1 then
	MPDefaultKnightNames = {"U_KnightSaraya", "U_KnightTrading", "U_KnightHealing", "U_KnightChivalry", "U_KnightWisdom", "U_KnightPlunder", "U_KnightSong"};
	S6Patcher.UpdateKnight();
end
-- #EOF