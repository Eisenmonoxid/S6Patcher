-- MainMenuUserScript by Eisenmonoxid - S6Patcher --
-- Find latest S6Patcher version here: https://github.com/Eisenmonoxid/S6Patcher
S6Patcher = S6Patcher or {}

CustomGame.KnightTypes = { -- Use all knights in Extra1
    "U_KnightSaraya",
    "U_KnightTrading",
    "U_KnightChivalry",
    "U_KnightWisdom",
	"U_KnightHealing",
	"U_KnightPlunder",
	"U_KnightSong",
};
g_MapAndHeroPreview.KnightTypes = CustomGame.KnightTypes
MPDefaultKnightNames = CustomGame.KnightTypes

RemapKnightID = function(_ID)
    --local Mapping = {[0] = 0, [1] = 1, [2] = 2, [3] = 3, [4] = 4};
    return _ID;
end

if S6Patcher.g_MainMenu_UpdateBackground == nil then
	S6Patcher.g_MainMenu_UpdateBackground = g_MainMenu.UpdateBackground;
end
g_MainMenu.UpdateBackground = function()
	S6Patcher.g_MainMenu_UpdateBackground();

	XGUIEng.SetMaterialTexture( "/InGame/Background/BG", 0, "MainMenu/limitedBG.png")
	XGUIEng.ShowWidget("/InGame/Background/Bars/Limited", 0)
	XGUIEng.ShowWidget("/InGame/Background/Bars/BottomBarLimited", 0)
	XGUIEng.ShowWidget("/InGame/Background/Bars/BottomBar", 1)
end

if S6Patcher.GetProgramVersion == nil then
	S6Patcher.GetProgramVersion = Framework.GetProgramVersion;
end
Framework.GetProgramVersion = function()
	local String = S6Patcher.GetProgramVersion();
	return String .. " - S6Patcher v2.3";
end

g_VideoOptions.InitResolutions = function()
	local XResolution = Options.GetIntValue("Display", "ResolutionWidth", 1024)
	local YResolution = Options.GetIntValue("Display", "ResolutionHeight", 768)
	local BitDepth = Options.GetIntValue("Display", "ResolutionDepth", 32)
	local CurrentResolutionName = XResolution .. " x " .. YResolution .. " (" .. BitDepth .." bit)"
	local ResolutionListBoxID = XGUIEng.GetWidgetID("/InGame/VideoOptionsMain/OptionFrame/LeftContainer/ResolutionComboBoxContainer/ListBox")
    
	XGUIEng.ListBoxPopAll(ResolutionListBoxID)
    
	local Index = 0;
	local ReturnState;
	local Name;
	ReturnState, Name = DisplayOptions.GetResolutionNames(Index);
	IndexOfCurrentResolution = 0

	while ReturnState == 1 do
        if Name == CurrentResolutionName then
            IndexOfCurrentResolution = Index;
        end
		
		Index = Index + 1
		XGUIEng.ListBoxPushItem(ResolutionListBoxID, Name)
		ReturnState, Name = DisplayOptions.GetResolutionNames(Index)
	end

	if Network.IsNATReady == nil then -- OV
		local AdditionalResolutions = {tostring(2560 .. " x " .. 1440 .. " (" .. 32 .." bit)"), tostring(3840 .. " x " .. 2160 .. " (" .. 32 .." bit)")};
		-- Push two higher resolutions
		XGUIEng.ListBoxPushItem(ResolutionListBoxID, AdditionalResolutions[1]);
		XGUIEng.ListBoxPushItem(ResolutionListBoxID, AdditionalResolutions[2]);
		
		for i = 1, #AdditionalResolutions do
			if AdditionalResolutions[i] == CurrentResolutionName then
				IndexOfCurrentResolution = Index + (i - 1);
			end
		end
	end

	g_VideoOptions.CurrentResolutionIndex = IndexOfCurrentResolution
	XGUIEng.ListBoxSetSelectedIndex(ResolutionListBoxID, IndexOfCurrentResolution)
end

g_GameOptions.OnShow = function()
    if Game ~= nil then
        XGUIEng.ShowWidget("/InGame/GameOptionsMain/Backdrop", 1);
    else
        XGUIEng.ShowWidget("/InGame/GameOptionsMain/Backdrop", 0);
    end
    
    g_MainMenuOptions:ShowHelper();
    -- Border scrolling
    do
        local WidgetID = XGUIEng.GetWidgetID("/InGame/GameOptionsMain/RightContainer/BorderScroll/CheckBox")                     
        if (Camera.RTS_GetBorderScrollSize() > 0) then
            XGUIEng.CheckBoxSetIsChecked(WidgetID, true)
        else
            XGUIEng.CheckBoxSetIsChecked(WidgetID, false)
        end
    end
	-- Mouse scroll speed
	do
		local Value = Options.GetFloatValue("Game", "ZoomSpeed", Camera.RTS_GetZoomWheelSpeed());
		local WidgetID = XGUIEng.GetWidgetID("/InGame/GameOptionsMain/RightContainer/KeyboardScrollSlider");
		XGUIEng.SliderSetValueMin(WidgetID, 2)
		XGUIEng.SliderSetValueMax(WidgetID, 8)
		XGUIEng.SliderSetValueAbs(WidgetID, Value)
	end    
	-- Keyboard scroll speed
	do
		local Value = Options.GetFloatValue("Game", "ScrollSpeed", Camera.RTS_GetScrollSpeed());
		local WidgetID = XGUIEng.GetWidgetID("/InGame/GameOptionsMain/RightContainer/MouseScrollSlider");
		XGUIEng.SliderSetValueMin(WidgetID, 20)
		XGUIEng.SliderSetValueMax(WidgetID, 60)
		XGUIEng.SliderSetValueAbs(WidgetID, Value / 100)
	end

    g_GameOptions.HasChanged = false;
    g_GameOptions.RefreshLeftContainer();
end
-- #EOF