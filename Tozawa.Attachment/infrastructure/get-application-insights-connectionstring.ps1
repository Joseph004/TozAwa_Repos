$appinsights = Get-AzApplicationInsights -ResourceGroupName "$env:ARM_RESOURCEGROUP_APPLICATIONINSIGHTS" -Name "$env:ARM_APPLICATIONINSIGHTS_NAME"
$connectionString = $appinsights.ConnectionString
Write-Output ("##vso[task.setvariable variable=Output.ApplicationInsightsConnectionString;]$connectionString")
