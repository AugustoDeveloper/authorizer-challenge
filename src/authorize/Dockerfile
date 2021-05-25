# Runtime stage
FROM mcr.microsoft.com/dotnet/runtime-deps:5.0-buster-slim AS base
WORKDIR /authorizer

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:5.0.202 AS build
WORKDIR /project

# Copy all project files to build
COPY ./src /project/src
COPY authorizer.sln /project/authorizer.sln

# Command to builds and generates a binary artifact to execution
RUN dotnet publish src/authorize/Authorize.csproj -c Release -o /output -r linux-x64 --self-contained true -p:PublishSingleFile=true -p:PublishReadyToRun=true

# Back to runtime stage
FROM base AS runtime
WORKDIR /authorizer

#Copy binary artifact folder to runtime stage
COPY --from=build /output .

ENTRYPOINT ["./authorize"]