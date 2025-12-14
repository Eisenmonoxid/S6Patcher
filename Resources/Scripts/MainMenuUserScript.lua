-- MainMenuUserScript by Eisenmonoxid - S6Patcher --
-- Find latest S6Patcher version here: https://github.com/Eisenmonoxid/S6Patcher
S6Patcher = S6Patcher or {};
S6Patcher.BETA = false;
g_DisplayScriptErrors = S6Patcher.BETA;
-- ************************************************************************************************************************************************************* --
-- Extended Knight Selection and Special Knights 																												 --
-- ************************************************************************************************************************************************************* --
S6Patcher.KnightSelection = {};
S6Patcher.KnightSelection.OverrideGlobalKnightSelection = function()
	if S6Patcher.KnightSelection.StartMapCallback2 == nil then
		S6Patcher.KnightSelection.StartMapCallback2 = CustomGame_StartMapCallback2;
	end
	CustomGame_StartMapCallback2 = function()
		local Context = S6Patcher.KnightSelection;
		local Knight = DisplayOptions.SkirmishGetKnight(1);
		local Validity = Context.IsMapValidForKnightChoice(CustomGame.StartMap, CustomGame.StartMapType);

		if Validity then
			local Name = Context.UpdatedKnightTypes[Knight + 1];
			local Command = "S6Patcher = S6Patcher or {};S6Patcher.SelectedKnight = " .. tostring("Entities." .. Name) .. ";";
			Framework.SetOnGameStartLuaCommand(Command);
		else
			if Context.SavedKnightID ~= -1 then
				DisplayOptions.SkirmishSetKnight(1, Context.SavedKnightID);
				Context.SavedKnightID = -1;
			end
			Framework.SetOnGameStartLuaCommand("return;");
		end

		Context.SetKnightSelection(false);
		Context.StartMapCallback2();
	end

	if S6Patcher.KnightSelection.CustomGame_StartOnLeftClick == nil then
		S6Patcher.KnightSelection.CustomGame_StartOnLeftClick = CustomGame_StartOnLeftClick;
	end
	CustomGame_StartOnLeftClick = function()
		local Context = S6Patcher.KnightSelection;
		local HeroComboBoxID = XGUIEng.GetWidgetID(CustomGame.Widget.KnightsList);
		local Index = XGUIEng.ListBoxGetSelectedIndex(HeroComboBoxID);

		local Validity = Context.IsMapValidForKnightChoice(CustomGame.SelectedMap, CustomGame.SelectedMapType);

		S6Patcher.KnightSelection.SavedKnightID = Validity and -1 or Index;
		if not Validity then
			CustomGame.Knight = Context.MapKnightTypesToTable(Context.UpdatedKnightTypes, CustomGame.CurrentKnightList, Index + 1) - 1;
		end

		Context.CustomGame_StartOnLeftClick();
	end

	if S6Patcher.KnightSelection.RemapKnightID == nil then
		S6Patcher.KnightSelection.RemapKnightID = RemapKnightID;
	end

	if S6Patcher.KnightSelection.CustomGameDialog_CloseOnLeftClick == nil then
		S6Patcher.KnightSelection.CustomGameDialog_CloseOnLeftClick = CustomGameDialog_CloseOnLeftClick;
	end
	CustomGameDialog_CloseOnLeftClick = function()
		S6Patcher.KnightSelection.CustomGameDialog_CloseOnLeftClick();
		S6Patcher.KnightSelection.SetKnightSelection(false);
	end

	if S6Patcher.KnightSelection.OpenCustomGameDialog == nil then
		S6Patcher.KnightSelection.OpenCustomGameDialog = OpenCustomGameDialog;
	end
	OpenCustomGameDialog = function()
		S6Patcher.KnightSelection.SetKnightSelection(true);
		S6Patcher.KnightSelection.OpenCustomGameDialog();
	end

	CustomGame_FillHeroComboBox = function(_TryToKeepSelectedKnight)
		local HeroComboBoxID = XGUIEng.GetWidgetID(CustomGame.Widget.KnightsList);
		XGUIEng.ListBoxPopAll(HeroComboBoxID);

		local KnightSelection = S6Patcher.KnightSelection.SavedOriginalKnightTypes;
		if CustomGame.SelectedMap and CustomGame.SelectedMapType then
			local KnightNames = {Framework.GetValidKnightNames(CustomGame.SelectedMap, CustomGame.SelectedMapType)};
			if #KnightNames > 0 then
				KnightSelection = KnightNames;
			end
		end

		local Validity = S6Patcher.KnightSelection.IsMapValidForKnightChoice(CustomGame.SelectedMap, CustomGame.SelectedMapType);
		if Validity then
			KnightSelection = S6Patcher.KnightSelection.UpdatedKnightTypes;
		end

		CustomGame.KnightTypes = KnightSelection;
		CustomGame.CurrentKnightList = KnightSelection;
		g_MapAndHeroPreview.KnightTypes = KnightSelection;

		for i = 1, #KnightSelection do
			XGUIEng.ListBoxPushItem(HeroComboBoxID, XGUIEng.GetStringTableText("Names/" .. KnightSelection[i]));
		end

		XGUIEng.ListBoxSetSelectedIndex(HeroComboBoxID, 0);
		CustomGame_OnHeroListBoxSelectionChange();
	end
end

S6Patcher.KnightSelection.MapKnightTypesToTable = function(_oldKnightTable, _newKnightTable, _newIndex)
	local Name = _newKnightTable[_newIndex];
	for Key, Value in pairs(_oldKnightTable) do
		if Value == Name then
			return Key;
		end
	end

	return S6Patcher.KnightSelection.SavedKnightID;
end

S6Patcher.KnightSelection.SetKnightSelection = function(_showKnights)
	local Context = S6Patcher.KnightSelection;
	CustomGame.KnightTypes = _showKnights and Context.UpdatedKnightTypes or Context.SavedOriginalKnightTypes;
	CustomGame.CurrentKnightList = _showKnights and Context.UpdatedKnightTypes or Context.SavedOriginalKnightTypes;
	g_MapAndHeroPreview.KnightTypes = _showKnights and Context.UpdatedKnightTypes or Context.SavedOriginalKnightTypes;
	RemapKnightID = _showKnights and Context.OverrideRemapKnightID or Context.RemapKnightID;
end

S6Patcher.KnightSelection.OverrideRemapKnightID = function(_ID)
	local Base = (Framework.GetGameExtraNo() < 1) == true;
	local Nr = Base and 6 or 7;
	if _ID > Nr then return "" end;
	return (Base and _ID) or (((_ID == 1) and 7) or (_ID - 1));
end

S6Patcher.KnightSelection.IsMapValidForKnightChoice = function(_selectedMap, _selectedMapType)
	if _selectedMapType == 0 or (_selectedMapType == 3 and S6Patcher.KnightSelection.EnableInUsermaps) then
		local Base = (Framework.GetGameExtraNo() < 1) == true;
		local Names = {Framework.GetValidKnightNames(_selectedMap, _selectedMapType)};

		if (#Names == 0) or (Base and #Names == 6) then -- No custom ValidKnightNames in info.xml
			return true;
		end
	end

	return false;
end

if Options.GetIntValue("S6Patcher", "ExtendedKnightSelection", 0) ~= 0 then
	local Context = S6Patcher.KnightSelection;

	Context.SavedKnightID = -1;
	Context.SavedOriginalKnightTypes = CustomGame.KnightTypes;
	Context.UpdatedKnightTypes = {"U_KnightSaraya", "U_KnightTrading", "U_KnightHealing",
		"U_KnightChivalry", "U_KnightWisdom", "U_KnightPlunder", "U_KnightSong"};
	Context.EnableInUsermaps = Options.GetIntValue("S6Patcher", "FeaturesInUsermaps", 0) ~= 0;

	if Framework.GetGameExtraNo() < 1 then
		table.remove(Context.UpdatedKnightTypes, 1);
	end

	if Options.GetIntValue("S6Patcher", "SpecialKnightsAvailable", 0) ~= 0 then
		Context.UpdatedKnightTypes[#Context.UpdatedKnightTypes + 1] = "U_KnightSabatta";
		Context.UpdatedKnightTypes[#Context.UpdatedKnightTypes + 1] = "U_KnightRedPrince";
	end

	Context.OverrideGlobalKnightSelection();
end
-- ************************************************************************************************************************************************************* --
-- Change the mainmenu background, show current S6Patcher version																			 					 --
-- ************************************************************************************************************************************************************* --
if Options.GetIntValue("S6Patcher", "UseAlternateBackground", 0) ~= 0 then
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
end
if S6Patcher.GetProgramVersion == nil then
	S6Patcher.GetProgramVersion = Framework.GetProgramVersion;
end
Framework.GetProgramVersion = function()
	local Text = " - S6Patcher v7.1" .. (S6Patcher.BETA and " - BETA" or "");
	return S6Patcher.GetProgramVersion() .. Text;
end
-- ************************************************************************************************************************************************************* --
-- Make 2K (2560x1440) and 4K (3840x2160) resolutions available in the original release																			 --
-- ************************************************************************************************************************************************************* --
S6Patcher.Resolution = {};
S6Patcher.Resolution.LastResolutionIndex = 0;
S6Patcher.Resolution.WQHD = tostring(2560 .. " x " .. 1440 .. " (" .. 32 .." bit)");
S6Patcher.Resolution.UHD = tostring(3840 .. " x " .. 2160 .. " (" .. 32 .." bit)");

if S6Patcher.Resolution.GetResolutionNames == nil then
	S6Patcher.Resolution.GetResolutionNames = DisplayOptions.GetResolutionNames;
end
DisplayOptions.GetResolutionNames = function(_index)
	local ReturnState, Name = S6Patcher.Resolution.GetResolutionNames(_index);
	if Network.IsNATReady ~= nil then -- Not necessary in History Edition
		return ReturnState, Name;
	end

	if ReturnState ~= 1 then
		if _index <= (S6Patcher.Resolution.LastResolutionIndex + 2) then
			return 1, ((S6Patcher.Resolution.LastResolutionIndex + 1 == _index) and S6Patcher.Resolution.WQHD or S6Patcher.Resolution.UHD);
		else
			return ReturnState, Name;
		end
	else
		S6Patcher.Resolution.LastResolutionIndex = _index;
		return ReturnState, Name;
	end
end
-- ************************************************************************************************************************************************************* --
-- Sliderfix: Now correctly read from the Options.ini file and add new checkbox to start the game in window mode												 --
-- ************************************************************************************************************************************************************* --
g_GameOptions.OnShow = function()
	XGUIEng.ShowWidget("/InGame/GameOptionsMain/Backdrop", (Game ~= nil) and 1 or 0);
	g_MainMenuOptions:ShowHelper();

	do
		local WidgetID = XGUIEng.GetWidgetID("/InGame/GameOptionsMain/RightContainer/BorderScroll/CheckBox");
		XGUIEng.CheckBoxSetIsChecked(WidgetID, (Camera.RTS_GetBorderScrollSize() > 0) and true or false)
	end
	do
		local Value = Options.GetFloatValue("Game", "ZoomSpeed", Camera.RTS_GetZoomWheelSpeed());
		local WidgetID = XGUIEng.GetWidgetID("/InGame/GameOptionsMain/RightContainer/KeyboardScrollSlider");
		XGUIEng.SliderSetValueMin(WidgetID, 2)
		XGUIEng.SliderSetValueMax(WidgetID, 8)
		XGUIEng.SliderSetValueAbs(WidgetID, Value)
	end
	do
		local Value = Options.GetFloatValue("Game", "ScrollSpeed", Camera.RTS_GetScrollSpeed());
		local WidgetID = XGUIEng.GetWidgetID("/InGame/GameOptionsMain/RightContainer/MouseScrollSlider");
		XGUIEng.SliderSetValueMin(WidgetID, 20)
		XGUIEng.SliderSetValueMax(WidgetID, 60)
		XGUIEng.SliderSetValueAbs(WidgetID, Value / 100)
	end
	do
		local Values = {"0025722953", "1703848010"};
		for Key, Value in pairs(Values) do
			if Options.GetIntValue("Codes", Value, 0) == 0 then
				Options.SetIntValue("Codes", Value, 1);
			end
		end
	end

	g_GameOptions.HasChanged = false;
	XGUIEng.ShowWidget("/InGame/GameOptionsMain/LeftContainer", 0);
	S6Patcher.ExtendedGameOptions.Init();
end
g_GameOptions.OnBack = function() S6Patcher.ExtendedGameOptions.OnClose(); end
g_GameOptions.OnOK = function()
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
-- ************************************************************************************************************************************************************* --
-- Extend the GameOptions with additional features																												 --
-- ************************************************************************************************************************************************************* --
S6Patcher.ExtendedGameOptions = {};
S6Patcher.ExtendedGameOptions.PageCount = 0;
S6Patcher.ExtendedGameOptions.Root = "/InGame/Multiplayer/SelectGame/Container/ListGames";
S6Patcher.ExtendedGameOptions.OriginalRootPosition = {0, 0};
S6Patcher.ExtendedGameOptions.Features = {
	{false, "Windowed"},
	{false, "UseAlternateBackground"},
	{false, "UseSingleStop"},
	{false, "UseDowngrade"},
	{false, "UseMilitaryRelease"},
	{false, "DayNightCycle"},
	{false, "ExtendedKnightSelection"},
	{false, "SpecialKnightsAvailable"},
	{false, "FeaturesInUsermaps"},
};

S6Patcher.ExtendedGameOptions.ShowUsermapWarning = function()
	OpenDialog(S6Patcher.GetLocalizedText("Feature"), S6Patcher.GetLocalizedText("Hint"));
end

S6Patcher.ExtendedGameOptions.CanAddSpecialKnights = function()
	if S6Patcher.IsModInstalled ~= true then
		OpenDialog(S6Patcher.GetLocalizedText("Files"), S6Patcher.GetLocalizedText("Error"));
	end

	return S6Patcher.IsModInstalled;
end

S6Patcher.ExtendedGameOptions.InitTempStuff = function()
	GUI_BuildingInfo = GUI_BuildingInfo or {};
	GUI_BuildingInfo.BuildingNameUpdate = function() -- Update function
		return true;
	end

	local BasePath = "/InGame/TempStuff/StandardDialog";
	local Width, Height = XGUIEng.GetWidgetScreenSize("/InGame/Background/WholeScreen");
	local _, posY = XGUIEng.GetWidgetScreenPosition("/InGame/Map/ContainerMap/MapPreview");
	local posX, _ = XGUIEng.GetWidgetScreenPosition("/InGame/Hero/ContainerHero");
	posX = posX - ((Width / 1920) * 100);
	XGUIEng.SetWidgetScreenPosition(BasePath, posX, posY);

	XGUIEng.ShowWidget(BasePath .. "/Background", 1);
	XGUIEng.PushPage(BasePath .. "/Background", false);
	S6Patcher.ExtendedGameOptions.PageCount = S6Patcher.ExtendedGameOptions.PageCount + 1;

	XGUIEng.ShowWidget(BasePath .. "/Buttons", 1);
	XGUIEng.PushPage(BasePath .. "/Buttons", false);
	S6Patcher.ExtendedGameOptions.PageCount = S6Patcher.ExtendedGameOptions.PageCount + 1;

	local CurrentPath = BasePath .. "/Background/TitleBig/Info/";
	XGUIEng.SetText(CurrentPath .. "NameBlack", S6Patcher.GetLocalizedText("Options"));
	XGUIEng.ShowWidget(CurrentPath .. "Name", 0);
	XGUIEng.ShowWidget(CurrentPath .. "NameWhite", 0);

	XGUIEng.ShowWidget(BasePath .. "/Buttons/BG", 0);
	XGUIEng.ShowWidget(BasePath .. "/Buttons/TestDisabled", 0);

	local Action = "Sound.FXPlay2DSound('ui\\menu_click');S6Patcher.ExtendedGameOptions.ToggleFeature();";
	local Text = S6Patcher.GetLocalizedText("Toggle");
	XGUIEng.SetText(BasePath .. "/Buttons/TestLongText", Text);
	XGUIEng.SetActionFunction(BasePath .. "/Buttons/TestLongText", Action);

	return S6Patcher.ExtendedGameOptions.PlaceUIElementsOnDialog(BasePath);
end

S6Patcher.ExtendedGameOptions.PlaceUIElementsOnDialog = function(_basePath)
	local posX, posY = XGUIEng.GetWidgetScreenPosition(_basePath);
	local Width, Height = XGUIEng.GetWidgetScreenSize(_basePath .. "/Background");
	local ButtonWidth, _ = XGUIEng.GetWidgetScreenSize(_basePath .. "/Buttons/TestLongText");

	posX = (posX + (Width * 0.5)) - (ButtonWidth * 0.5);
	posY = (posY + (Height * 0.8));

	XGUIEng.SetWidgetScreenPosition(_basePath .. "/Buttons/TestLongText", posX, posY);
	return posX, posY;
end

S6Patcher.ExtendedGameOptions.Init = function()
	local posX, posY = S6Patcher.ExtendedGameOptions.InitTempStuff(); -- Button Position

	S6Patcher.ExtendedGameOptions.OriginalRootPosition = {XGUIEng.GetWidgetScreenPosition(S6Patcher.ExtendedGameOptions.Root)};
	XGUIEng.SetWidgetScreenPosition(S6Patcher.ExtendedGameOptions.Root, posX * 0.75, posY * 0.5);
	XGUIEng.ShowWidget(S6Patcher.ExtendedGameOptions.Root, 1);
	XGUIEng.PushPage(S6Patcher.ExtendedGameOptions.Root, false);
	S6Patcher.ExtendedGameOptions.PageCount = S6Patcher.ExtendedGameOptions.PageCount + 1;

	S6Patcher.ExtendedGameOptions.LoadFeaturesFromFile();
	S6Patcher.ExtendedGameOptions.UpdateFeatures();
end

S6Patcher.ExtendedGameOptions.LoadFeaturesFromFile = function()
	for _, Value in pairs(S6Patcher.ExtendedGameOptions.Features) do
		local Entry = Options.GetIntValue(Value[2] == "Windowed" and "Display" or "S6Patcher", Value[2], 0);
		Value[1] = Entry == 1;
	end
end

S6Patcher.ExtendedGameOptions.WriteFeaturesToFile = function()
	for _, Value in pairs(S6Patcher.ExtendedGameOptions.Features) do
		Options.SetIntValue(Value[2] == "Windowed" and "Display" or "S6Patcher", Value[2], Value[1] and 1 or 0);
	end
end

S6Patcher.ExtendedGameOptions.OnClose = function()
	XGUIEng.ListBoxPopAll(S6Patcher.ExtendedGameOptions.Root .. "/Icons");
	XGUIEng.ListBoxPopAll(S6Patcher.ExtendedGameOptions.Root .. "/Games");

	for i = 1, S6Patcher.ExtendedGameOptions.PageCount do
		XGUIEng.PopPage();
	end
	S6Patcher.ExtendedGameOptions.PageCount = 0;

	XGUIEng.SetWidgetScreenPosition(S6Patcher.ExtendedGameOptions.Root, S6Patcher.ExtendedGameOptions.OriginalRootPosition[1],
		S6Patcher.ExtendedGameOptions.OriginalRootPosition[2]);

	S6Patcher.ExtendedGameOptions.WriteFeaturesToFile();
end

S6Patcher.ExtendedGameOptions.ToggleFeature = function()
	local WidgetID = XGUIEng.GetWidgetID(S6Patcher.ExtendedGameOptions.Root .. "/SliderWidget");
	local Value = XGUIEng.SliderGetValueAbs(WidgetID);
	local Index = XGUIEng.ListBoxGetSelectedIndex(S6Patcher.ExtendedGameOptions.Root .. "/Games") + 1; -- zero based
	local Active = S6Patcher.ExtendedGameOptions.Features[Index][1];
	local Name = S6Patcher.ExtendedGameOptions.Features[Index][2];

	if not Active then
		if Name == "SpecialKnightsAvailable" then
			if not S6Patcher.ExtendedGameOptions.CanAddSpecialKnights() then
				return;
			else
				S6Patcher.ExtendedGameOptions.Features[Index - 1][1] = true;
			end
		elseif Name == "FeaturesInUsermaps" then
			S6Patcher.ExtendedGameOptions.ShowUsermapWarning();
		end
	else
		if Name == "ExtendedKnightSelection" then
			S6Patcher.ExtendedGameOptions.Features[Index + 1][1] = false;
		end
	end

	S6Patcher.ExtendedGameOptions.Features[Index][1] = not S6Patcher.ExtendedGameOptions.Features[Index][1];
	S6Patcher.ExtendedGameOptions.UpdateFeatures(Index - 1);

	XGUIEng.SliderSetValueAbs(WidgetID, Value);
end

S6Patcher.ExtendedGameOptions.UpdateFeatures = function(_selectedIndex)
	XGUIEng.ListBoxPopAll(S6Patcher.ExtendedGameOptions.Root .. "/Icons");
	XGUIEng.ListBoxPopAll(S6Patcher.ExtendedGameOptions.Root .. "/Games");

	for Key, Value in pairs(S6Patcher.ExtendedGameOptions.Features) do
		S6Patcher.ExtendedGameOptions.AddFeature(S6Patcher.GetLocalizedText(Value[2]), Value[1]);
	end

	XGUIEng.ListBoxSetSelectedIndex(S6Patcher.ExtendedGameOptions.Root .. "/Icons", _selectedIndex);
	XGUIEng.ListBoxSetSelectedIndex(S6Patcher.ExtendedGameOptions.Root .. "/Games", _selectedIndex);
end

S6Patcher.ExtendedGameOptions.AddFeature = function(_name, _checked)
	local posX = _checked and 10 or 11;
	local posY = _checked and 11 or 12;

	local UVOffsetX = _checked and 3 or 9;
	local UVOffsetY = _checked and 8 or 0;

	local u0 = posX * 44 + UVOffsetX;
	local v0 = (posX + 1) * 44 - UVOffsetY;
	local u1 = (posX + 1) * 44 + 8;
	local v1 = (posY + 1) * 44 - 1;

	XGUIEng.ListBoxPushItemEx(S6Patcher.ExtendedGameOptions.Root .. "/Icons", "", "Icons.png", nil, u0, v0, u1, v1);
	XGUIEng.ListBoxPushItem(S6Patcher.ExtendedGameOptions.Root .. "/Games", _name);
end

S6Patcher.GetLocalizedText = function(_text)
	local Language = Network.GetDesiredLanguage();
	if S6Patcher.TranslatedStrings[Language] ~= nil then
		return S6Patcher.TranslatedStrings[Language][_text];
	else
		return S6Patcher.TranslatedStrings["en"][_text];
	end
end

S6Patcher.TranslatedStrings = {};
S6Patcher.TranslatedStrings["de"] =
{
	["Toggle"] 	= "{center}Feature Umschalten",
	["Options"] = "{center}S6Patcher - Optionen",
	["Files"]	= "Fehlende Spieldateien!{cr}Dieses Feature benötigt den {@color:255,0,255}installierten Bugfixmod{@color:none}.",
	["Error"]	= "{center}Fehler",
	["Hint"]	= "{center}Hinweis",
	["Feature"] = "Das Aktivieren dieses Features kann dafür sorgen, dass eine kleine Menge an Usermaps nicht mehr korrekt funktionieren wird.",
	-- Options Menu Texts
	["FeaturesInUsermaps"]		= "Features in Spielerkarten",
	["Windowed"] 				= "Fenstermodus",
	["UseAlternateBackground"] 	= "Hauptmenühintergrund",
	["UseSingleStop"] 			= "Einzelstoppbutton",
	["UseDowngrade"] 			= "Rückbaubutton",
	["UseMilitaryRelease"] 		= "Soldaten Entlassen Button",
	["DayNightCycle"] 			= "Tag/Nacht - Zyklus",
	["ExtendedKnightSelection"] = "Alle Ritter Selektierbar",
	["SpecialKnightsAvailable"] = "Spezialritter (Crimson Sabatt & Roter Prinz)",
};
S6Patcher.TranslatedStrings["en"] =
{
	["Toggle"] 	= "{center}Toggle Feature",
	["Options"] = "{center}S6Patcher - Options",
	["Files"]	= "Missing Gamefiles!{cr}This Feature requires the {@color:255,0,255}Bugfix Mod{@color:none} to be installed.",
	["Error"]	= "{center}Error",
	["Hint"]	= "{center}Hint",
	["Feature"] = "Activating this Feature can lead to a small number of usermaps not working correctly anymore.",
	-- Options Menu Texts
	["FeaturesInUsermaps"]		= "Features in Usermaps",
	["Windowed"] 				= "Window Mode",
	["UseAlternateBackground"] 	= "Main Menu Background",
	["UseSingleStop"] 			= "Single Stop Button",
	["UseDowngrade"] 			= "Downgrade Button",
	["UseMilitaryRelease"] 		= "Military Release Button",
	["DayNightCycle"] 			= "Day/Night - Cycle",
	["ExtendedKnightSelection"] = "All Knights in Selection",
	["SpecialKnightsAvailable"] = "Special Knights (Crimson Sabatt & Red Prince)",
};
S6Patcher.TranslatedStrings["pl"] =
{
	["Toggle"] 	= "{center}Przełącz funkcję",
	["Options"] = "{center}S6Patcher - Opcje",
	["Files"]	= "Brak plików gry!{cr}Ta funkcja wymaga zainstalowania {@color:255,0,255}Bugfix Mod{@color:none}.",
	["Error"]	= "{center}Błąd",
	["Hint"]	= "{center}Wskazówka",
	["Feature"] = "Aktywacja tej funkcji może spowodować, że niewielka liczba map użytkownika przestanie działać poprawnie.",
	-- Options Menu Texts
	["FeaturesInUsermaps"]		= "Funkcje w mapach użytkownika",
	["Windowed"] 				= "Tryb okienkowy",
	["UseAlternateBackground"] 	= "Tło menu głównego",
	["UseSingleStop"] 			= "Przycisk pojedynczego zatrzymania",
	["UseDowngrade"] 			= "Przycisk degradacji",
	["UseMilitaryRelease"] 		= "Przycisk uwolnienia wojsk",
	["DayNightCycle"] 			= "Cykl dnia i nocy",
	["ExtendedKnightSelection"] = "Wszyscy rycerze w wyborze",
	["SpecialKnightsAvailable"] = "Specjalni rycerze (Crimson Sabatt i Red Prince)",
};
S6Patcher.TranslatedStrings["fr"] =
{
	["Toggle"] 	= "{center}Activer/Désactiver la fonction",
	["Options"] = "{center}S6Patcher - Options",
	["Files"]	= "Fichiers du jeu manquants!{cr}Cette fonction nécessite l’installation du {@color:255,0,255}Bugfix Mod{@color:none}.",
	["Error"]	= "{center}Erreur",
	["Hint"]	= "{center}Astuce",
	["Feature"] = "L’activation de cette fonction peut empêcher un petit nombre de cartes utilisateur de fonctionner correctement.",
	-- Options Menu Texts
	["FeaturesInUsermaps"]		= "Fonctions dans les cartes utilisateur",
	["Windowed"] 				= "Mode fenêtré",
	["UseAlternateBackground"] 	= "Fond du menu principal",
	["UseSingleStop"] 			= "Bouton d’arrêt unique",
	["UseDowngrade"] 			= "Bouton de rétrogradation",
	["UseMilitaryRelease"] 		= "Bouton de libération militaire",
	["DayNightCycle"] 			= "Cycle jour/nuit",
	["ExtendedKnightSelection"] = "Tous les chevaliers dans la sélection",
	["SpecialKnightsAvailable"] = "Chevaliers spéciaux (Crimson Sabatt & Red Prince)",
};
S6Patcher.TranslatedStrings["nl"] =
{
	["Toggle"] 	= "{center}Functie in-/uitschakelen",
	["Options"] = "{center}S6Patcher - Opties",
	["Files"]	= "Ontbrekende spelbestanden!{cr}Deze functie vereist dat de {@color:255,0,255}Bugfix Mod{@color:none} is geïnstalleerd.",
	["Error"]	= "{center}Fout",
	["Hint"]	= "{center}Hint",
	["Feature"] = "Het activeren van deze functie kan ervoor zorgen dat een klein aantal gebruikerskaarten niet meer correct werkt.",
	-- Options Menu Texts
	["FeaturesInUsermaps"]		= "Functies in gebruikerskaarten",
	["Windowed"] 				= "Venstermodus",
	["UseAlternateBackground"] 	= "Achtergrond hoofdmenu",
	["UseSingleStop"] 			= "Enkele stopknop",
	["UseDowngrade"] 			= "Degradatieknop",
	["UseMilitaryRelease"] 		= "Legervrijgaveknop",
	["DayNightCycle"] 			= "Dag-/nachtcyclus",
	["ExtendedKnightSelection"] = "Alle ridders in selectie",
	["SpecialKnightsAvailable"] = "Speciale ridders (Crimson Sabatt & Red Prince)",
};
S6Patcher.TranslatedStrings["ru"] =
{
	["Toggle"] 	= "{center}Переключить функцию",
	["Options"] = "{center}S6Patcher - Настройки",
	["Files"]	= "Отсутствуют файлы игры!{cr}Эта функция требует установки {@color:255,0,255}Bugfix Mod{@color:none}.",
	["Error"]	= "{center}Ошибка",
	["Hint"]	= "{center}Подсказка",
	["Feature"] = "Активация этой функции может привести к тому, что некоторые пользовательские карты перестанут правильно работать.",
	-- Options Menu Texts
	["FeaturesInUsermaps"]		= "Функции в пользовательских картах",
	["Windowed"] 				= "Оконный режим",
	["UseAlternateBackground"] 	= "Фон главного меню",
	["UseSingleStop"] 			= "Кнопка одиночной остановки",
	["UseDowngrade"] 			= "Кнопка понижения",
	["UseMilitaryRelease"] 		= "Кнопка освобождения войск",
	["DayNightCycle"] 			= "Цикл день/ночь",
	["ExtendedKnightSelection"] = "Все рыцари в выборе",
	["SpecialKnightsAvailable"] = "Особые рыцари (Crimson Sabatt и Red Prince)",
};
-- #EOF
