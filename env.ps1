param(
    [Parameter(Mandatory = $true)]
    [ValidatePattern('^[a-zA-Z0-9]+$')]
    [String] $Prefix, 
    [Parameter(Mandatory = $false)]
    [String] $Location = "eastus",
    [Parameter(Mandatory = $false)]
    [String] $TemplateFile = './env.bicep'
)

az bicep build --file $TemplateFile

$ResourceGroupName = "$($Prefix)rg"

az group create `
    -n $ResourceGroupName `
    -l $Location `

# az deployment group create `
#     -n ((Get-ChildItem $TemplateFile).BaseName + '-' + ((Get-Date).ToUniversalTime()).ToString('MMdd-HHmm')) `
#     -f $TemplateFile `
#     -g $ResourceGroupName `
#     --parameters productId=$ProductId `
#     --parameters environmentId=$EnvironmentId `
#     --parameters servicePrincipalId=$ServicePrincipalId `
#     --parameters publisherName=$PublisherName `
#     --parameters publisherEmail=$PublisherEmail