##### NEEDED VARIABLE KEYS
# SQL_LOGINNAME
# SQL_USERNAME
# SQL_SERVER_NAME
# SQL_DATABASE_NAME
# SQL_ADMIN_LOGIN
# SQL_ADMIN_PASSWORD_VAULT_NAME
# SQL_LOGIN_VAULT_NAME
# SQL_PASSWORD_VAULT_NAME
# SQL_SCHEMA
######

function Get-RandomCharacters($length, $characters) {
    $random = 1..$length | ForEach-Object { Get-Random -Maximum $characters.length }
    $private:ofs = ""
    return [String]$characters[$random]
}
 
function Ramdomize([string]$inputString) {     
    $characterArray = $inputString.ToCharArray()   
    $scrambledStringArray = $characterArray | Get-Random -Count $characterArray.Length     
    $outputString = -join $scrambledStringArray
    return $outputString 
}

$password = Get-RandomCharacters -length 10 -characters 'abcdefghiklmnoprstuvwxyz'
$password += Get-RandomCharacters -length 10 -characters 'ABCDEFGHKLMNOPRSTUVWXYZ'
$password += Get-RandomCharacters -length 10 -characters '1234567890'
 
$password = Ramdomize $password


$login = "$env:SQL_LOGINNAME";
$user = "$env:SQL_USERNAME";
$sqlServerName = "$env:SQL_SERVER_NAME";
$sqlDatabaseName = "$env:SQL_DATABASE_NAME";
$sqlServerAdminLogin = "$env:SQL_ADMIN_LOGIN";
$sqlServerAdminPasswordSecret = Get-AzureKeyVaultSecret -VaultName "$env:KEYVAULT_NAME" -Name "$env:SQL_ADMIN_PASSWORD_VAULT_NAME"
$sqlServerAdminPassword = $sqlServerAdminPasswordSecret.SecretValueText

$masterConnectionString = "Server=tcp:" + $sqlServerName + ".database.windows.net,1433;Initial Catalog=master;Persist Security Info=False;User ID=" + $sqlServerAdminLogin + ";Password=" + $sqlServerAdminPassword + ";MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
$databaseConnectionString = "Server=tcp:" + $sqlServerName + ".database.windows.net,1433;Initial Catalog=" + $sqlDatabaseName + ";Persist Security Info=False;User ID=" + $sqlServerAdminLogin + ";Password=" + $sqlServerAdminPassword + ";MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
function Invoke-SQL-Create-Schema {
    Write-Output("Invoke-SQL-Create-Schema start")
    $sqlCommand = $("IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = N'" + "$env:SQL_SCHEMA" + "') EXEC('CREATE SCHEMA [" + "$env:SQL_SCHEMA" + "]')");
    $connection = new-object system.data.SqlClient.SQLConnection($databaseConnectionString)
    $connection.Open()
    $command = new-object System.Data.SqlClient.SqlCommand($sqlCommand, $connection)
    $command.ExecuteNonQuery();
    $connection.Close()
    Write-Output("Invoke-SQL-Create-Schema success")
}
function Invoke-SQL-Create-Login {
    $connection = new-object system.data.SqlClient.SQLConnection($masterConnectionString)
    $connection.Open()

    $sqlCommandCreate = "IF NOT EXISTS(SELECT NAME FROM sys.sql_logins WHERE NAME = '$login') CREATE LOGIN [$login] WITH PASSWORD='$password';"
    $sqlCommandAlter = "IF EXISTS(SELECT NAME FROM sys.sql_logins WHERE NAME = '$login') ALTER LOGIN [$login] WITH PASSWORD='$password';"
            
    $commandCreate = new-object system.data.sqlclient.sqlcommand($sqlCommandCreate, $connection)
    $commandCreate.ExecuteNonQuery()

    $commandAlter = new-object system.data.sqlclient.sqlcommand($sqlCommandAlter, $connection)
    $commandAlter.ExecuteNonQuery()
  
    $connection.Close()
}
function Invoke-SQL-Create-User {
    $sqlCommand = $("
        IF USER_ID('$user') IS NULL CREATE USER $user FROM LOGIN $login;
        GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA::" + "$env:SQL_SCHEMA" + " TO $user;
    ");
    
    $connection = new-object system.data.SqlClient.SQLConnection($databaseConnectionString)
    $command = new-object system.data.sqlclient.sqlcommand($sqlCommand, $connection)
    $connection.Open()
    $command.ExecuteNonQuery()
    $connection.Close()
}

Invoke-SQL-Create-Schema
Invoke-SQL-Create-Login
Invoke-SQL-Create-User

Write-Output ("##vso[task.setvariable variable=Output.SqlLogin;]$login")
Write-Output ("##vso[task.setvariable variable=Output.SqlPassword;]$password")