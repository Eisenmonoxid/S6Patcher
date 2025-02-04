LuaQ     @UserScriptLocal.lua           	ð      @   
  	À	 Á	 Á	 A      ÀA W@   @B E   FÀÁ  @  @   E  FÂ À  CÁ@  AÁ \	@  @D W@ À   @D A @ $       ÀD @ À    E  FÀÄ 	@  d@  	@   @E @ À    E F@Å 	@ d  	@   	 Á   J @ Á Á A A Á Â b@	@    dÀ  	@    d  	@   @H  H @
À  I A   @	 Á	  WI @  ÀI       J  @  À   ÀE @  À   @J W@ À    H @    J @ À    E  FÊ 	@   d@ 	@    	ÈÀ  I A   À  Á	  WI    @@ WÀJ    	È  d 	@   dÀ 	@  d  	@ À  I A   @ Á	  WI    @@ WÀJ    	È  d@ 	@  d 	@   dÀ 	@À  I A    Á	  WI À   @@ WÀJ À   	H   L @ À    EÀ FÌ 	@ À d  	@     M @ À    EÀ F Í 	@ À d@ 	@    d 	@   M @     E 	@ $À     ÀM @ À    E  FÀÍ 	@  d  	@   d@ 	@   d 	@   ;   
   S6Patcher    CurrentMapType     UseSingleStop     UseDowngrade    UseMilitaryRelease    g_FeedbackSpeechFix    Trigger    IsTriggerEnabled    RequestTrigger    Events    LOGIC_EVENT_EVERY_SECOND        FeedbackSpeechEndTimeFixCustom       ð?
   Framework    WriteToLog '   S6Patcher: Meldungsstaufix is enabled!    ContinueWallUpdate    GUI_BuildingButtons    BuildingNameUpdate    GUI_BuildingInfo    GlobalScriptOverridden    DefaultKnightNames    U_KnightSaraya    U_KnightTrading    U_KnightHealing    U_KnightChivalry    U_KnightWisdom    U_KnightPlunder    U_KnightSong    OverrideGlobalScript )   IsCurrentMapEligibleForKnightReplacement    Options    GetIntValue    ExtendedKnightSelection            GetGameExtraNo    IsNetworkGame    SelectedKnight    RestartMap       @   GateAutoToggleClicked    GateAutoToggleMouseOver    GateAutoToggleUpdate    GateOpenCloseClicked    GateOpenCloseUpdate    GateOpenCloseMouseOver    DismountClicked    GUI_Military    DismountUpdate    HandleMilitaryReleaseTooltip "   GameCallback_GUI_SelectionChanged    SetNameAndDescription    GUI_Tooltip    SetTooltip    GetLocalizedText                 #      @@  E  WÀÀ ÀE  F Á WÀÀ ÀL@A    A E  IÀ@E FÀÁ   Á@ \@E   FÂ WÀÀ ÀE   FÂ À Å     Ü À \@      
   Framework 
   GetTimeMs    g_FeedbackSpeech     LastSpeechEndTime      p·@   XGUIEng    ShowWidget A   /InGame/Root/Normal/AlignBottomRight/MapFrame/FeedbackSpeechText            WriteToLog #   S6Patcher: Caught Meldungsstau at  	   tostring     #                                                                                                                  QDnlt    "                          @@ E  FÀÀ \    E  F@Á @   E FÀÁ   B Á@ \@   E FÀÂ \@         Logic    GetEntityType    GUI    GetSelectedEntity 	   Entities    B_FenceTurret    XGUIEng    ShowWidget    GetCurrentWidgetID         
   S6Patcher    ContinueWallUpdate                                                                                	   LmcA2auZ                              @@ @   À@  E  F Á    \ @Á E   FÁ    ÂÂ\   ÀBÀ     @     
   S6Patcher    BuildingNameUpdate    XGUIEng    GetCurrentWidgetID    GetText    {center}B_Cathedral_Big    GetLocalizedText    de    {center}Kathedrale    en    {center}Cathedral    SetText                                                                                      Q          ZA                  A            @@ A  @ À  E  F@Á   AF @  EÀ F Â @ Å Á ÁB Ü   E   \ A \@      
   Framework    SetOnGameStartLuaCommand    return; 	   Entities 
   S6Patcher    DefaultKnightNames    SelectedKnight    GUI    SendScriptCommand Y   		S6Patcher = S6Patcher or {}
		S6Patcher.UpdateKnight = function()
			local PlayerID =  	   tostring    GetPlayerID w   ;
			local KnightID = Logic.GetKnightID(PlayerID);
	
			if (KnightID ~= 0) then
				S6Patcher.ReplaceKnight(KnightID,  £  );
			end
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
			Logic.SetPrimaryKnightID(PlayerID, ID);
		end

		S6Patcher.UpdateKnight();
		Logic.ExecuteInLuaLocalState('LocalSetKnightPicture();');
	                                               "   $   $   $   $   $   )   *   *   *   A   A      A         _IQQ               B   F            @@  E   FÀ \À ÅÀ  É@ W@Á @ Á ÀÊ     ÁA@   â@  @A@   Â   Þ       
   Framework    GetCurrentMapName !   GetCurrentMapTypeAndCampaignName 
   S6Patcher    CurrentMapType               @   GetValidKnightNames        C   C   C   D   D   D   D   D   E   E   E   E   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F         XpkjA          pVRj          fuZ3z86          er               Q   V     
   E   F@À    \   ÅÀ  Æ ÁÔ    Á  AA@ @A AÁÁ  @ AB ÕAA   ÀûÀ  BÀ   @         Logic    GetEntityTypeName       ð?
   S6Patcher    DefaultKnightNames 
   Framework    SetOnGameStartLuaCommand 8   S6Patcher = S6Patcher or {};S6Patcher.SelectedKnight =  	   tostring    ;    RestartMap        Q   Q   Q   Q   R   S   S   S   S   S   T   T   T   T   T   U   U   V   V   V   V   V   V   U   V   R   V   V   V   V   V         DFb100j           XL_          (for index) 	         (for limit) 	         (for step) 	         WYdR 
              Z   ]            @@ A  @ À   A  EÀ  F@Á    Å ÆÀÁ   Ü Ó \@        Sound    FXPlay2DSound    ui\menu_click    GUI    GetSelectedEntity    SetStoppedState    Logic    IsBuildingStopped        [   [   [   [   [   [   [   \   \   \   ]   ]   ]   ]   ]   \   ]      	   QKKks_zt               ^   a            @@ E  FÀÀ   @A  \  Z    A Z@    AÀ @         GUI_Tooltip    TooltipNormal    Logic    IsBuildingStopped    GUI    GetSelectedEntity    StartBuilding    StopBuilding        _   _   a   a   a   a   a   a   a   a   a   a   a   a   _   a               b   m      N      @@  E  FÀÀ \ W Á 	@ AÀ   WÀA @  BÀ   WÀA@ @BÀ   WBÀ@ ÀBÀ   ACWCÀ@ ÀCÀ   W D@@ @DÀ    D@   DÀ   Á @  À À   A E@    @
 AA  "A A  À 
 AA Á "A @   DÀ    @        XGUIEng    GetCurrentWidgetID    GUI    GetSelectedEntity     Logic    IsBuilding            IsConstructionComplete    IsBuildingStoppable     IsEntityInCategory    EntityCategories    Cathedrals       ð?
   IsBurning    CanCancelKnockDownBuilding    ShowWidget    SetIcon    IsBuildingStopped       @      (@      *@    N   c   c   c   c   c   c   i   i   j   j   j   j   j   j   j   j   j   j   j   j   j   j   j   j   j   j   j   k   k   k   k   k   k   k   k   k   k   k   k   k   k   k   k   k   k   k   k   k   k   k   k   k   k   l   l   m   m   m   m   m   m   m   m   m   m   m   m   m   m   m   m   l   m   m   m   m   m   m         Are7xU    M      yxjl    M           q   w      
)      @@  E   FÀ \ À   AÀ   ÅÀ  Æ@Á  Ü ÎÁÀ ÅÀ Æ ÂA Ü@ Å   ÆÂ  Ü@ Å   ÆÀÂ EA  \  ÅA Â D@  Ü  B Ü@         GUI    GetPlayerID    GetSelectedEntity    Logic    GetEntityHealth    GetEntityMaxHealth       à?   Sound    FXPlay2DSound    ui\menu_click    DeselectEntity    SendScriptCommand    Logic.HurtEntity( 	   tostring    ,     math    ceil    );     )   q   q   q   r   r   r   r   r   r   r   s   s   s   s   s   s   t   t   t   t   t   t   t   t   u   u   u   w   w   w   w   w   w   w   w   w   w   w   w   u   w         ZG    (      Vu0cCAf    (      q    (           x         O      @@  E  FÀÀ \ W Á À@ AÀ   WÀA@@  BÀ   WÀAÀ	@ @BÀ   WB@@ ÀBÀ   WBÀ@  CÀ  A CWÀCÀ@  DÀ   Å@ Æ@Ä  Ü XÀ @@ DÀ   Å@ ÆÀÄ  Ü À @    EÀ   Á @  @ À   
 A Á "A @    EÀ   Á @        XGUIEng    GetCurrentWidgetID    GUI    GetSelectedEntity     Logic    IsBuilding            IsConstructionComplete 
   IsBurning    CanCancelKnockDownBuilding    IsEntityInCategory    EntityCategories    Cathedrals       ð?   GetEntityHealth    GetEntityMaxHealth    GetUpgradeLevel    GetMaxUpgradeLevel    ShowWidget    SetIcon       @      .@    O   y   y   y   y   y   y                                                                                                                                                                                                                                    kP7O5    N      lqT    N                          @@ A  @         GUI_Tooltip    TooltipNormal    DowngradeButton                                      ¤      c      @@  E   FÀ \ À   AÀ   W@Á ÅÀ  ÆÁ  Ü ÀÁ @AÅ  Æ@Â Ü@ Ê   Á  ÁB@  â@  ÁÁ À AC@ÁÁÁAEÁ  FÃ \Á Å  ÆÁÃ EB \  ÅB   Ü  EC   \ Ã ÅC   Ü  ÜA    AEA Á  C@ Á   ÁCÁ B @  A B À Á C @   A C À  Á ÕÁA Ã  ÅÀ Æ ÆÜ@         GUI    GetPlayerID    GetSelectedEntity    Logic    GetGuardedEntityID         	   IsLeader       ð?   Sound    FXPlay2DSound    ui\menu_click    GetSoldiersAttachedToLeader    table    remove    GetEntityPosition    SendScriptCommand ,   					Logic.CreateEffect(EGL_Effects.FXDie,  	   tostring    ,     );
					Logic.DestroyEntity(    );
				    ClearSelection $   );
					Logic.DestroyGroupByLeader( 
   S6Patcher    DismountClicked     c                                                                                                                                                                                                                                                             ¡   ¡   ¡   £   £   £   £   ¤   ¤      ¤   ¤   ¤   ¤   ¤   ¤   	      mP3mlD    b      PrPyxMK    b      tczrIB 
   b      a    ^      wqU76o '   @      LB1Z +   @      N9L +   @      hDc_M H   ^   	   qW0lRiD1 H   ^           ¦   ­      .      @@  E  FÀÀ \   @AÀ   WÁ Å  ÆÀÁ  Ü  AB A Å    J Á Á bA Ü@Å   Æ@Ã   A Ü@  Å    J  Á bA Ü@ÅÀ Æ ÄÜ@         XGUIEng    GetCurrentWidgetID    GUI    GetSelectedEntity    Logic    GetGuardedEntityID            GetEntityType 	   Entities    U_MilitaryLeader    SetIcon       ,@      (@   DisableButton       ð?
   S6Patcher    DismountUpdate     .   §   §   §   §   §   §   ¨   ¨   ¨   ¨   ª   ª   «   «   «   «   «   «   «   «   «   «   «   «   «   «   «   «   «   ¬   ¬   ¬   ¬   ¬   ¬   ¬   ¬   ¬   ¬   ¬   ¬   ¬   ­   ­   ­   ­         iD1IUx    -   	   JLCOx_ak    -      hPQ 
   -           ®   ¶     .   Å   Æ@À  Ü WÀ  @ Â   Þ  Ê  É ÁÉÁ
  	ÁÁ	ÂEA FÂ\ Á CÀ W@ÃÀÅÁ ÆÃ Ü Â D À@C@ÅA ÆÄ  @ À ÜAÂ Þ @ Â  Þ         XGUIEng    GetWidgetID E   /InGame/Root/Normal/AlignBottomRight/DialogButtons/Military/Dismount    de 
   Entlassen    en    Dismiss $   - EntlÃ¤sst Soldaten der Reihe nach '   - Dismisses soldiers one after another    GUI    GetSelectedEntity    Logic    GetGuardedEntityID            GetEntityType 	   Entities    U_MilitaryLeader 
   S6Patcher    SetTooltip     .   °   °   °   °   °   °   °   °   °   °   °   ±   ±   ±   ±   ±   ±   ²   ²   ²   ²   ´   ´   µ   µ   µ   µ   µ   µ   µ   µ   µ   µ   µ   µ   µ   µ   µ   µ   µ   ¶   ¶   ¶   ¶   ¶   ¶         R1FIoQI     -      NsoTwDs     -      HGli     -      iy    -      m6SCS0    -   	   NUhYw6R4    -      Hv    -           ¸   ¾     !   E   F@À    \@ E   FÀ WÀÀ ÀE  F@Á  Å   ÆÀÁ Â Á@ Ú@    Á \@E  F@Á À Å   Æ Ã Â Á@ Ú@    Á \@     
   S6Patcher "   GameCallback_GUI_SelectionChanged    CurrentMapType       @   XGUIEng    ShowWidget 3   /InGame/Root/Normal/BuildingButtons/GateAutoToggle    SingleStopButtons       ð?        2   /InGame/Root/Normal/BuildingButtons/GateOpenClose    UseDowngrade     !   ¹   ¹   ¹   ¹   º   º   º   º   »   »   »   ¼   ¼   ¼   ¼   ¼   ¼   ¼   ¼   »   ½   ½   ½   ¾   ¾   ¾   ¾   ¾   ¾   ¾   ¾   ½   ¾         Ch                 À   È     +   E  FAÀ\   Á@W A@@A  ÁAABÊ  ÉBÉÁB  C@   À  B     ACÀ   @  C      ÁCÀ    @ À            XGUIEng    GetCurrentWidgetID 
   S6Patcher    CurrentMapType       @   DowngradeButton    de 	   RÃ¼ckbau    en 
   Downgrade *   - Baut das GebÃ¤ude um eine Stufe zurÃ¼ck '   - Downgrades the building by one level    SetTooltip    HandleMilitaryReleaseTooltip    SetNameAndDescription     +   Á   Á   Á   Â   Â   Â   Â   Ã   Ã   Ä   Ä   Ä   Å   Å   Å   Å   Å   Å   Å   Å   Å   Å   Æ   Æ   Ç   Ç   Ç   Ç   Ç   Ç   Ç   Ç   Ç   È   È   È   È   È   È   È   È   È   È         urkh     *      zhzpBSx     *   	   rHSjalVy     *      TjhsnP     *      t5jzEd9     *      JZAU2    *      zPXTTg          seMLr               É   Ì     "     A@@    ÅÁ  ÆÁ  Ü ÁA  A@@ Á  AÀ A    AA@  E  FÁ \Á Å  ÆÁÁ  @ ÜA         XGUIEng    SetText 	   {center} 
   S6Patcher    GetLocalizedText    GetTextHeight    GetWidgetSize    SetWidgetSize     "   Ê   Ê   Ê   Ê   Ê   Ê   Ê   Ê   Ê   Ê   Ë   Ë   Ë   Ë   Ë   Ë   Ë   Ë   Ë   Ë   Ë   Ë   Ë   Ì   Ì   Ì   Ì   Ì   Ì   Ì   Ì   Ì   Ì   Ì         qX     !      h_8     !      xL7OTb     !      w8T3f     !      K    !      qL    !      vfIyB    !           Í   Ï        E   F@À \ À  F@ Z@    FÀ@ ^          Network    GetDesiredLanguage    de    en        Î   Î   Î   Î   Î   Ï   Ï   Ï   Ï   Ï   Ï         quNsijN     
       ð                                                                                                                                                                                                                                       A   A   B   F   F   L   L   L   L   L   M   M   M   M   M   M   M   M   N   N   N   N   N   N   N   N   N   N   N   N   N   N   N   N   N   N   N   N   N   P   P   P   P   P   P   P   P   Q   V   V   V   V   X   X   X   X   X   X   X   X   X   X   X   X   Y   Y   Z   ]   ]   ^   a   a   b   m   m   o   o   o   o   o   o   o   o   o   o   o   o   p   p   q   w   w   x                                                                                       ¤   ¤   ¤   ¤   ¤   ¤   ¥   ¥   ¥   ¥   ¦   ­   ­   ®   ¶   ¶   ¶   ¶   ¶   ¶   ·   ·   ·   ¾   ¾   ¾   ¾   ¾   ¾   ¿   ¿   ¿   ¿   À   È   È   É   Ì   Ì   Í   Ï   Ï   Ï           