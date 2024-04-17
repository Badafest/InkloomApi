export ASPNETCORE_ENVIRONMENT=test;
dotnet ef database drop && dotnet ef database update;
cd ../Inkloom.Api.Test;
dotnet run seed;
dotnet test;
cd ../Inkloom.Api;