del *.nupkg

nuget pack MEungblut.TestUtility.csproj

nuget push *.nupkg -s http://meungblutnuget.azurewebsites.net/ 02CCA68B-BE35-BE0F-B06E-C90857260049