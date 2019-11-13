<ESRI.Configuration xmlns="http://schemas.esri.com/Desktop/AddIns" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <Name>Sposterezhennya.Visibility.Addin</Name>
  <AddInID>{2EFC4C87-74DF-4A63-A945-511F20ABA9F6}</AddInID>
  <Description>Sposterezhennya Module Visibility</Description>
  <Version>1.5.0002</Version>
  <Image>Images\Visibility.png</Image>
  <Author>DPP</Author>
  <Company>DPP</Company>
  <Date>2019.11.12</Date>
  
  <Targets>
    <Target name="Desktop" version="10.4" />
  </Targets>
  
  <AddIn language="CLR" library="Sposterezhennya.Visibility.dll" namespace="MilSpace.Visibility">
    <ArcMap>
      
      <Commands>
        <Button id="MilSpace_Visibility_ButtonMilSpaceMVis" 
                class="ButtonMilSpaceVisibility" 
                message="Sposterezhennya Module Visibility Window" 
                caption="Sposterezhennya Visibility" 
                tip="Sposterezhennya Module Visibility Window" 
                category="Спостереження інструменти" 
                image="Images\Visibility.png" />
        
        <Tool id="MilSpace_Visibility_MapInteropTool" 
              class="MilSpaceVisibilityMapInteropTool" 
              message="Visibility Map Interop Tool"
              caption="Map Interop Tool" 
              tip="Sposterezhennya Visibility Map Interop Tool" 
              category="Спостереження інструменти" 
              image="Images\Flag.png" />
      </Commands>
      
      <DockableWindows>
        <DockableWindow id="SP_MilSpace_Visibility_DockableWindowMilSpaceMVisibilitySt" 
                        class="DockableWindowMilSpaceMVisibilitySt+AddinImpl" 
                        caption="Sposterezhennya Visibility" 
                        image="Images\Visibility.png">
          <InitialPlacement height="600" width="320" state="unpinned" position="float" />
        </DockableWindow>
      </DockableWindows>

      <Toolbars>
        <Toolbar id="Sposterezhennya.MilspaceArcMapAddinToolbar"
                 caption="Спостереження інструменти"
                 showInitially="true">
          <Items>
            <Button refID="MilSpace_Visibility_ButtonMilSpaceMVis" />
          </Items>
        </Toolbar>
      </Toolbars>
      
    </ArcMap>
  </AddIn>
</ESRI.Configuration>