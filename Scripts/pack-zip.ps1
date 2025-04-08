Remove-Item ..\QuickLook.Plugin.LightBurn.qlplugin -ErrorAction SilentlyContinue

$files = Get-ChildItem -Path ..\bin\Release\ -Exclude *.pdb,*.xml
Compress-Archive $files ..\QuickLook.Plugin.LightBurn.zip
Move-Item ..\QuickLook.Plugin.LightBurn.zip ..\QuickLook.Plugin.LightBurn.qlplugin