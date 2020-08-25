FROM mcr.microsoft.com/dotnet/core/runtime:3.1
COPY bin/Release/net5.0/linux-x64/publish/* App/
WORKDIR /App
ENTRYPOINT ["dotnet","corescaletest.dll", "32"]
