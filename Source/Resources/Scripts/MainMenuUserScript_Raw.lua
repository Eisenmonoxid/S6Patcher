-- MainMenuUserScript by Eisenmonoxid - S6Patcher --
-- Find latest S6Patcher version here: https://github.com/Eisenmonoxid/S6Patcher
S6Patcher = S6Patcher or {}
S6Patcher.KnightSelection = S6Patcher.KnightSelection or {}
-- ************************************************************************************************************************************************************* --
-- Make all Knights available in the expansion pack ("Eastern Realm")																					 		 --
-- ************************************************************************************************************************************************************* --
S6Patcher.KnightSelection.OverrideGlobalKnightSelection = function()
	if S6Patcher.KnightSelection.StartMapCallback2 == nil then
		S6Patcher.KnightSelection.StartMapCallback2 = CustomGame_StartMapCallback2;
	end
	CustomGame_StartMapCallback2 = function()
		local CurrentKnight = DisplayOptions.SkirmishGetKnight(1);
		if S6Patcher.KnightSelection.IsMapValidForKnightChoice(CustomGame.StartMap, CustomGame.StartMapType) then
			Framework.SetOnGameStartLuaCommand("S6Patcher = S6Patcher or {};S6Patcher.SelectedKnight = " .. tostring(CurrentKnight + 1) .. ";");
		else
			if S6Patcher.KnightSelection.SavedKnightID ~= -1 then
				DisplayOptions.SkirmishSetKnight(1, S6Patcher.KnightSelection.SavedKnightID);
			end
			Framework.SetOnGameStartLuaCommand("return;");
		end

		S6Patcher.KnightSelection.StartMapCallback2();
	end
	if S6Patcher.KnightSelection.CustomGame_StartOnLeftClick == nil then
		S6Patcher.KnightSelection.CustomGame_StartOnLeftClick = CustomGame_StartOnLeftClick;
	end
	CustomGame_StartOnLeftClick = function()
		local HeroComboBoxID = XGUIEng.GetWidgetID(CustomGame.Widget.KnightsList);
		local Index = XGUIEng.ListBoxGetSelectedIndex(HeroComboBoxID);
		if not S6Patcher.KnightSelection.IsMapValidForKnightChoice(CustomGame.SelectedMap, CustomGame.SelectedMapType) then
			S6Patcher.KnightSelection.SavedKnightID = Index;
		end
		
		S6Patcher.KnightSelection.CustomGame_StartOnLeftClick();
	end
	if S6Patcher.KnightSelection.RemapKnightID == nil then
		S6Patcher.KnightSelection.RemapKnightID = RemapKnightID;
	end
	if S6Patcher.KnightSelection.CustomGameDialog_CloseOnLeftClick == nil then
		S6Patcher.KnightSelection.CustomGameDialog_CloseOnLeftClick = CustomGameDialog_CloseOnLeftClick;
	end
	CustomGameDialog_CloseOnLeftClick = function()
		S6Patcher.KnightSelection.SetKnightSelection(false);
		S6Patcher.KnightSelection.CustomGameDialog_CloseOnLeftClick();
	end
	if S6Patcher.KnightSelection.OpenCustomGameDialog == nil then
		S6Patcher.KnightSelection.OpenCustomGameDialog = OpenCustomGameDialog;
	end
	OpenCustomGameDialog = function()
		S6Patcher.KnightSelection.SetKnightSelection(true);
		S6Patcher.KnightSelection.OpenCustomGameDialog();
	end
end
S6Patcher.KnightSelection.SetKnightSelection = function(_showKnights)
	local Context = S6Patcher.KnightSelection;
	CustomGame.KnightTypes = (_showKnights == true) and Context.NewKnightTypes or Context.SavedOriginalKnightTypes;
	CustomGame.CurrentKnightList = (_showKnights == true) and Context.NewKnightTypes or Context.SavedOriginalKnightTypes;
	g_MapAndHeroPreview.KnightTypes = (_showKnights == true) and Context.NewKnightTypes or Context.SavedOriginalKnightTypes;
	RemapKnightID = (_showKnights == true) and Context.OverrideRemapKnightID or Context.RemapKnightID;
end
S6Patcher.KnightSelection.OverrideRemapKnightID = function(_ID)
	return ((_ID == 1) and 7) or (_ID - 1);
end
S6Patcher.KnightSelection.IsMapValidForKnightChoice = function(_selectedMap, _selectedMapType)
	local Names = {Framework.GetValidKnightNames(_selectedMap, _selectedMapType)};	
	return ((_selectedMapType == 0 or _selectedMapType == 3) and #Names == 0);
end
if Framework.GetGameExtraNo() >= 1 and Options.GetIntValue("S6Patcher", "ExtendedKnightSelection", 0) ~= 0 then
	S6Patcher.KnightSelection.SavedKnightID = -1;
	S6Patcher.KnightSelection.SavedOriginalKnightTypes = CustomGame.KnightTypes;
	S6Patcher.KnightSelection.NewKnightTypes = {"U_KnightSaraya", "U_KnightTrading", "U_KnightHealing", "U_KnightChivalry", 
													"U_KnightWisdom", "U_KnightPlunder", "U_KnightSong"};	
	S6Patcher.KnightSelection.OverrideGlobalKnightSelection();
end
-- ************************************************************************************************************************************************************* --
-- Change the mainmenu background, show current S6Patcher version																			 					 --
-- ************************************************************************************************************************************************************* --
if S6Patcher.g_MainMenu_UpdateBackground == nil then
	S6Patcher.g_MainMenu_UpdateBackground = g_MainMenu.UpdateBackground;
end
g_MainMenu.UpdateBackground = function()
	S6Patcher.g_MainMenu_UpdateBackground();

	XGUIEng.SetMaterialTexture("/InGame/Background/BG", 0, "MainMenu/limitedBG.png");
	XGUIEng.ShowWidget("/InGame/Background/Bars/Limited", 0);
	XGUIEng.ShowWidget("/InGame/Background/Bars/BottomBarLimited", 0);
	XGUIEng.ShowWidget("/InGame/Background/Bars/BottomBar", 1);
end
if S6Patcher.GetProgramVersion == nil then
	S6Patcher.GetProgramVersion = Framework.GetProgramVersion;
end
Framework.GetProgramVersion = function() return S6Patcher.GetProgramVersion() .. " - S6Patcher v4"; end
-- ************************************************************************************************************************************************************* --
-- Make 2K (2560x1440) and 4K (3840x2160) resolutions available in the original release																			 --
-- ************************************************************************************************************************************************************* --
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

	if Network.IsNATReady == nil then
		local AdditionalResolutions = {tostring(2560 .. " x " .. 1440 .. " (" .. 32 .." bit)"), tostring(3840 .. " x " .. 2160 .. " (" .. 32 .." bit)")};
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
-- ************************************************************************************************************************************************************* --
-- Sliderfix: Now correctly read from the Options.ini file and add new checkbox to start the game in window mode												 --
-- ************************************************************************************************************************************************************* --
g_GameOptions.OnShow = function()
	if Game ~= nil then
		XGUIEng.ShowWidget("/InGame/GameOptionsMain/Backdrop", 1);
	else
		XGUIEng.ShowWidget("/InGame/GameOptionsMain/Backdrop", 0);
	end
    
	g_MainMenuOptions:ShowHelper();
	-- Border scrolling
	do
		local WidgetID = XGUIEng.GetWidgetID("/InGame/GameOptionsMain/RightContainer/BorderScroll/CheckBox");   
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
	S6Patcher.ExtendedGameOptions();
end
g_GameOptions.OnBack = function()
	XGUIEng.ShowWidget("/InGame/TempStuff/StandardDialog/DemoCheckBox", 0);
	XGUIEng.PopPage();
end
if S6Patcher.GameOptionsRestoreDefaults == nil then
	S6Patcher.GameOptionsRestoreDefaults = g_GameOptions.RestoreDefaults;
end
g_GameOptions.RestoreDefaults = function()
	S6Patcher.GameOptionsRestoreDefaults();
	XGUIEng.CheckBoxSetIsChecked("/InGame/TempStuff/StandardDialog/DemoCheckBox/CheckBox", false);
end
g_GameOptions.OnOK = function()
	do
		local WidgetID = XGUIEng.GetWidgetID("/InGame/TempStuff/StandardDialog/DemoCheckBox/CheckBox");    
		if (XGUIEng.CheckBoxIsChecked(WidgetID) == true) then
			Options.SetIntValue("Display", "Windowed", 1);
		else
			Options.SetIntValue("Display", "Windowed", 0);
		end
	end
	do
		local WidgetID = XGUIEng.GetWidgetID("/InGame/GameOptionsMain/RightContainer/BorderScroll/CheckBox");
		if (XGUIEng.CheckBoxIsChecked(WidgetID) == true) then
			Camera.RTS_SetBorderScrollSize(g_DefaultBorderScrollSize);
		else        
			Camera.RTS_SetBorderScrollSize(0);
		end      
		Options.SetFloatValue("Game", "BorderScrolling", Camera.RTS_GetBorderScrollSize());
	end
	do
		local WidgetID = XGUIEng.GetWidgetID("/InGame/GameOptionsMain/RightContainer/KeyboardScrollSlider");
		Camera.RTS_SetZoomWheelSpeed(XGUIEng.SliderGetValueAbs(WidgetID));
		Options.SetFloatValue("Game", "ZoomSpeed", Camera.RTS_GetZoomWheelSpeed());  
	end        
	do
		local WidgetID = XGUIEng.GetWidgetID("/InGame/GameOptionsMain/RightContainer/MouseScrollSlider");    
		Camera.RTS_SetScrollSpeed(XGUIEng.SliderGetValueAbs(WidgetID) * 100);
		Options.SetFloatValue("Game", "ScrollSpeed", Camera.RTS_GetScrollSpeed());
	end

	g_MainMenuOptions:HideHelper();
	g_GameOptions:Back();
end
S6Patcher.ExtendedGameOptions = function()
	local RootPath = "/InGame/TempStuff/StandardDialog/DemoCheckBox";
	local Width, Height = XGUIEng.GetWidgetScreenSize("/InGame/Background/WholeScreen");
	local posX, posY = XGUIEng.GetWidgetScreenPosition("/InGame/GameOptionsMain/RightContainer/BorderScroll/CheckBox");
	local Text = S6Patcher.GetLocalizedText({de = "Fenstermodus", en = "Windowed"});
	
	XGUIEng.SetWidgetScreenPosition(RootPath, posX, posY - ((Height/1080)*50));
	XGUIEng.SetText(RootPath .. "/TextCheckbox", Text);
	XGUIEng.SetActionFunction(RootPath .. "/CheckBox", "Sound.FXPlay2DSound('ui\\menu_click');g_GameOptions:OnChange();");

	XGUIEng.ShowWidget(RootPath, 1);
	XGUIEng.PushPage(RootPath, false);
	
	local Windowed = Options.GetIntValue("Display", "Windowed", 0);
	if (Windowed == 1) then
		XGUIEng.CheckBoxSetIsChecked(RootPath .. "/CheckBox", true)
	else
		XGUIEng.CheckBoxSetIsChecked(RootPath .. "/CheckBox", false)
	end
end
S6Patcher.GetLocalizedText = function(_text) return (Network.GetDesiredLanguage() == "de" and  _text.de) or _text.en; end
-- #EOF