FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0.100-preview.4 AS build
ARG TARGETARCH
ENV TZ=Etc/UTC
ENV DOTNET_TieredPGO=1
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1

#RUN echo "Target: $TARGETARCH" && echo "Build: $BUILDPLATFORM"
WORKDIR /app
#COPY ["nuget.config", "nuget.config"]
# Dependencies of MiniCalc.Web
COPY ["MiniCalc.Lib/MiniCalc.Lib.csproj", "MiniCalc.Lib/"]

# To be built project
COPY ["MiniCalc.Web/MiniCalc.Web.csproj", "MiniCalc.Web/"]
RUN dotnet restore "MiniCalc.Web/MiniCalc.Web.csproj" -a $TARGETARCH

COPY . ./
RUN dotnet publish -c Release -o output

FROM nginx:1.29.2-alpine3.22
WORKDIR /var/www/web
COPY --from=build /app/output/wwwroot .
COPY MiniCalc.Web/nginx.conf /etc/nginx/nginx.conf
EXPOSE 80
