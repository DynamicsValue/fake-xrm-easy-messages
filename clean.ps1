param (
    [string]$folderPath = "./src/FakeXrmEasy.Messages/bin"
)

if (Test-Path -Path $folderPath) {
  Get-ChildItem -Path $folderPath -Include * -File -Recurse | foreach { $_.Delete()}
}
