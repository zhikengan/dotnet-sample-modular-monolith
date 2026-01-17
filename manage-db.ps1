<#
.SYNOPSIS
    Helper script to manage database migrations for the Modular Monolith application.

.DESCRIPTION
    This script automates common EF Core migration tasks for multiple DbContexts (User and Ordering).
    It supports creating migrations, updating the database, and resetting the database.

.PARAMETER Action
    The action to perform. Options: 
    - add: create a new migration
    - update: update database
    - reset: drop db, remove migrations, recreate initial migration, update db
    - drop: drop the database

.PARAMETER Name
    Name of the migration (required for 'add' action)

.PARAMETER Module
    Module to perform action on. Options: All, User, Ordering, Catalog

.EXAMPLE
    .\manage-db.ps1 -Action add -Name AddNewField
    .\manage-db.ps1 -Action update
    .\manage-db.ps1 -Action reset
#>

param(
    [Parameter(Mandatory=$true)]
    [ValidateSet("add", "update", "reset", "drop")]
    [string]$Action,

    [Parameter(Mandatory=$false)]
    [string]$Name = "InitialCreate",

    [Parameter(Mandatory=$false)]
    [ValidateSet("All", "User", "Ordering", "Catalog")]
    [string]$Module = "All"
)

$solutionDir = Get-Location
$defaultStartup = "App.Web"

# Define Modules Config
$modules = @{
    "User" = @{ Project = "App.Modules.User"; Context = "UserDbContext"; Startup = "App.Web" };
    "Ordering" = @{ Project = "App.Modules.Ordering"; Context = "OrderingDbContext"; Startup = "App.Web" };
    "Catalog" = @{ Project = "App.Modules.Catalog"; Context = "CatalogDbContext"; Startup = "App.Service.Catalog" };
}

function Run-DotNetCommand {
    param($cmd)
    Write-Host "Running: $cmd" -ForegroundColor Cyan
    Invoke-Expression $cmd
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Command failed!" -ForegroundColor Red
        exit $LASTEXITCODE
    }
}

# Filter modules based on selection
$targetModules = $modules.Clone()
if ($Module -ne "All") {
    $targetModules = @{ $Module = $modules[$Module] }
}

foreach ($key in $targetModules.Keys) {
    $config = $targetModules[$key]
    $proj = $config.Project
    $ctx = $config.Context
    $startup = if ($config.Startup) { $config.Startup } else { $defaultStartup }

    Write-Host "Processing Module: $key..." -ForegroundColor Magenta

    if ($Action -eq "drop" -or $Action -eq "reset") {
        # Only drop once if handling distinct DBs? Currently all share DB.
        # But 'dotnet ef database drop' works per context connection.
        # If they share the same DB, the first drop wipes it. subsequent drops might fail or warn.
        # For 'reset', we want to drop once globally? 
        # Actually, let's just let it run. EF Core handles 'database does not exist' gracefully usually.
        
        Write-Host "Dropping Database ($ctx)..." -ForegroundColor Yellow
        Run-DotNetCommand "dotnet ef database drop --project $proj --startup-project $startup --context $ctx --force"
    }

    if ($Action -eq "reset") {
        Write-Host "Removing migrations ($proj)..." -ForegroundColor Yellow
        if (Test-Path "$proj\Data\Migrations") { Remove-Item "$proj\Data\Migrations" -Recurse -Force }

        Write-Host "Adding Initial Migration ($ctx)..." -ForegroundColor Yellow
        Run-DotNetCommand "dotnet ef migrations add $Name --project $proj --startup-project $startup --context $ctx"
    }

    if ($Action -eq "add") {
        if ([string]::IsNullOrEmpty($Name)) {
            Write-Error "Migration Name is required for 'add' action."
            exit 1
        }
        Write-Host "Adding Migration '$Name' to ($ctx)..." -ForegroundColor Yellow
        Run-DotNetCommand "dotnet ef migrations add $Name --project $proj --startup-project $startup --context $ctx"
    }

    if ($Action -eq "update" -or $Action -eq "reset") {
        Write-Host "Updating Database ($ctx)..." -ForegroundColor Yellow
        Run-DotNetCommand "dotnet ef database update --project $proj --startup-project $startup --context $ctx"
    }
}

Write-Host "Done!" -ForegroundColor Green
