
$barber="3.0.0-alpha-2";
dotnet build -c Release
Copy-Item ".\bin\Release\net5.0\*" -Destination $home\.dotnet\tools\.store\barber\$barber\barber\$barber\tools\net5.0\any -Force