export ASPNETCORE_ENVIRONMENT=test;
dotnet ef database drop && dotnet ef database update;
cd ../test;
dotnet run seed;
dotnet test;
cd ../src;