dotnet publish Desktop_Gremlin/Desktop_Gremlin.csproj -c Release -r linux-x64 --self-contained true /p:PublishSingleFile=true -o Desktop_Gremlin/bin/Published/Linux
cp -r Desktop_Gremlin/Sprites Desktop_Gremlin/bin/Published/Linux/
cp Desktop_Gremlin/icon.ico Desktop_Gremlin/bin/Published/Linux/
cp Desktop_Gremlin/config.txt Desktop_Gremlin/bin/Published/Linux/
