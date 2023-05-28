$storageKey = (Get-AzureRmStorageAccountKey -ResourceGroupName "$env:ARM_RESOURCEGROUP_STORAGE" -AccountName "$env:ARM_STORAGEACCOUNT")[0].Value
$storageKey | Write-Host
Write-Output ("##vso[task.setvariable variable=Output.StorageKey;]$storageKey")