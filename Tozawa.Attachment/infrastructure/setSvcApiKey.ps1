$newKey = [guid]::NewGuid().ToString()
$secretvalue = ConvertTo-SecureString "$newKey" -AsPlainText -Force
$secret = Set-AzKeyVaultSecret -VaultName "$env:KEYVAULT_NAME" -Name "$env:APP_SVC_APIKEY_NAME" -SecretValue $secretvalue
Write-Output ("##vso[task.setvariable variable=App.Svc.ApiKey;]$newKey")