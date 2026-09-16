param(
    [Parameter(Mandatory=$false)]
    [string]$SonarToken,
    
    [Parameter(Mandatory=$false)]
    [string]$ProjectKey = "SIAES.API"
)

# Verifica se o dotnet-sonarscanner está instalado
$sonarInstalled = dotnet tool list -g | Select-String "dotnet-sonarscanner"
if (-not $sonarInstalled) {
    Write-Host "Instalando o dotnet-sonarscanner globalmente..." -ForegroundColor Yellow
    dotnet tool install --global dotnet-sonarscanner
}

if (-not $SonarToken) {
    Write-Host "ATENÇÃO: O SonarToken não foi informado." -ForegroundColor Red
    Write-Host "Como obter o Token:" -ForegroundColor Cyan
    Write-Host "1. Suba o ambiente: docker-compose up -d"
    Write-Host "2. Acesse http://localhost:9000 (admin/admin)"
    Write-Host "3. Crie um projeto manual e gere o token"
    Write-Host "Execute novamente: .\run-sonar.ps1 -SonarToken `"SEU_TOKEN`"" -ForegroundColor Yellow
    exit 1
}

Write-Host "Iniciando a análise do SonarQube..." -ForegroundColor Green

# 1. Iniciar o SonarScanner
# Configura o CollectCoverage e a integração com o Coverlet
dotnet sonarscanner begin /k:"$ProjectKey" /d:sonar.host.url="http://localhost:9000" /d:sonar.token="$SonarToken" /d:sonar.cs.opencover.reportsPaths="**/coverage.opencover.xml" /d:sonar.coverage.exclusions="**Tests*.cs"

# 2. Compilar a solução
Write-Host "Compilando a solução..." -ForegroundColor Green
dotnet build SIAES.slnx --no-incremental

# 3. Executar os testes coletando a cobertura (Coverlet)
Write-Host "Executando testes e coletando cobertura..." -ForegroundColor Green
# O Coverlet vai gerar o arquivo coverage.opencover.xml em cada projeto de teste
dotnet test SIAES.slnx --no-build --collect:"XPlat Code Coverage" -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=opencover

# 4. Finalizar o SonarScanner e enviar pro servidor
Write-Host "Enviando análise para o SonarQube..." -ForegroundColor Green
dotnet sonarscanner end /d:sonar.token="$SonarToken"

Write-Host "Análise concluída com sucesso! Verifique o resultado em http://localhost:9000" -ForegroundColor Green
