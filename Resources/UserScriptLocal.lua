-- UserScriptLocal by Eisenmonoxid - S6Patcher --
-- Find latest S6Patcher version here: https://github.com/Eisenmonoxid/S6Patcher
S6Patcher = S6Patcher or {}

-- Fix "Meldungsstau"
if (S6Patcher.g_FeedbackSpeechFix == nil) or (not Trigger.IsTriggerEnabled(S6Patcher.g_FeedbackSpeechFix)) then
	S6Patcher.g_FeedbackSpeechFix = Trigger.RequestTrigger(Events.LOGIC_EVENT_EVERY_SECOND, "", "FeedbackSpeechEndTimeFixCustom", 1);
	Framework.WriteToLog("S6Patcher: Meldungsstaufix is enabled!");
end
FeedbackSpeechEndTimeFixCustom = function()
	local Time = Framework.GetTimeMs();
	
    if (g_FeedbackSpeech ~= nil) and (g_FeedbackSpeech.LastSpeechEndTime ~= nil) and ((Time + 6000) < g_FeedbackSpeech.LastSpeechEndTime) then
        g_FeedbackSpeech.LastSpeechEndTime = nil;
        XGUIEng.ShowWidget("/InGame/Root/Normal/AlignBottomRight/MapFrame/FeedbackSpeechText", 0);
		
		if Framework ~= nil and Framework.WriteToLog ~= nil then
			Framework.WriteToLog("S6Patcher: Caught Meldungsstau at " .. tostring(Time));
		end
    end
end
-- #EOF