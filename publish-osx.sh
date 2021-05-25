# only run this command on linux machines
dotnet publish src/authorize/Authorize.csproj -c Release -o ./output-osx -r osx-x64 --self-contained true -p:PublishSingleFile=true -p:PublishReadyToRun=true