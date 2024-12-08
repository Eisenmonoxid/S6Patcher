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
-- #EOF