# Before running, execute:
#
#   dotnet tool install dotnet-reportgenerator-globaltool --tool-path tools
#
dotnet test /p:CollectCoverage=true /p:CoverletOutput=TestResults/coverage.json /p:CoverletOutputFormat=opencover /p:ExcludeByAttribute=CompilerGeneratedAttribute /p:ExcludeByAttribute=GeneratedCodeAttribute

if($LASTEXITCODE -eq 0)
{
    ./tools/reportgenerator.exe "-reports:tests/*/TestResults/coverage.json" "-targetdir:TestResults/CoverageReport" "-assemblyfilters:+AutoStep.Extensions"
}