[CmdletBinding()]
Param()

Add-Type -AssemblyName System.Web 

Function GetAccessToken($ClientId, $ClientSecret, $Scope) {
    $ClientIDEncoded = [System.Web.HttpUtility]::UrlEncode($ClientId)
    $ClientSecretEncoded = [System.Web.HttpUtility]::UrlEncode($ClientSecret)
    $Uri = "http://localhost:5000/connect/token"
    $Body = "grant_type=client_credentials&client_id=$ClientIDEncoded&client_secret=$ClientSecretEncoded&scope=$Scope"
    $ContentType = "application/x-www-form-urlencoded"
    Return (Invoke-RestMethod -Uri $Uri -Body $Body -ContentType $ContentType -Method Post).access_token
}

Function SendGetRequest([string]$Url, [string]$Token)
{
    if([string]::IsNullOrWhiteSpace($Token)) 
    {
        Return ConvertTo-Json (Invoke-RestMethod -Uri $Url -Method Get)
    }
    Write-Verbose "Sending request with bearer token..."
    Return ConvertTo-Json (Invoke-RestMethod -Uri $Url -Headers @{Authorization = "Bearer $Token"} -Method Get)
}

# Get the access token.
Write-Host ""
$AccessToken = GetAccessToken -ClientId "client.internal" -ClientSecret "internalSecret" -Scope "api.internal"
if([string]::IsNullOrWhiteSpace($AccessToken)) {
    Throw "Could not retrieve access token."
}
Write-Host "Got access token: $AccessToken"

Write-Host ""
Write-Host "Contacting open API WITHOUT access token..."
$Response = SendGetRequest -Url "http://localhost:7000/api/public"
Write-Host "RESULT: $Response"

Write-Host ""
Write-Host "Contacting claims API WITHOUT access token..."
$Response = SendGetRequest -Url "http://localhost:7000/api/claims"
Write-Host "RESULT: $Response"

Write-Host ""
Write-Host "Contacting claims API WITH access token..."
$Response = SendGetRequest -Url "http://localhost:7000/api/claims" -Token $AccessToken
Write-Host "RESULT: $Response"

Write-Host ""
Write-Host "Contacting third party API WITH access token..."
$Response = SendGetRequest -Url "http://localhost:7000/api/thirdparty" -Token $AccessToken
Write-Host "RESULT: $Response"

Write-Host ""
Write-Host "Contacting internal API WITH access token..."
$Response = SendGetRequest -Url "http://localhost:7000/api/internal" -Token $AccessToken
Write-Host "RESULT: $Response"