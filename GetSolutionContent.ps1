# Set your solution file path
$solutionFilePath = "C:\source\repos\Me\test\GptFinance\GptFinance.sln"
$outputFilePath = "C:\source\repos\Me\test\GptFinance\OutputFile.txt"

# Check if the solution file exists
if (-not (Test-Path -Path $solutionFilePath -PathType Leaf)) {
    Write-Error "Solution file not found: $solutionFilePath"
    exit 1
}

# Read the solution file
$solutionFileContent = Get-Content -Path $solutionFilePath

# Extract project file paths
$projectFilePaths = @()
foreach ($line in $solutionFileContent) {
    if ($line -match 'Project\("(.*)"\)\s*=\s*"(.*)",\s*"(.*)",\s*"(.*)"') {
        $projectFilePath = $matches[3]
        $projectFilePaths += $projectFilePath
    }
}

# Function to process and concatenate code from C# files
function ProcessCodeFile {
    param (
        [string]$filePath
    )

    $codeContent = ""
    $inMultilineComment = $false

    $fileLines = Get-Content -Path $filePath
    foreach ($line in $fileLines) {
        # Check for multiline comments
        if ($line -match "\s*/\*") {
            $inMultilineComment = $true
        }

        if (-not $inMultilineComment) {
            # Ignore single-line comments and empty lines
            if (-not ($line -match "\s*//") -and (-not [string]::IsNullOrWhiteSpace($line))) {
                $codeContent += $line + "`n"
            }
        }

        if ($line -match "\s*\*/") {
            $inMultilineComment = $false
        }
    }

    return $codeContent
}

# Concatenate the content of all project files
$outputFileContent = ""
foreach ($projectFilePath in $projectFilePaths) {
    # Resolve the absolute project file path
    $absoluteProjectFilePath = (Resolve-Path -Path (Join-Path -Path (Split-Path -Path $solutionFilePath) -ChildPath $projectFilePath)).Path

    # Check if the project file exists
    if (-not (Test-Path -Path $absoluteProjectFilePath -PathType Leaf)) {
        Write-Error "Project file not found: $absoluteProjectFilePath"
        continue
    }

    # Find all C# files in the project folder
    $projectFolder = Split-Path -Path $absoluteProjectFilePath
    $csFiles = Get-ChildItem -Path $projectFolder -Recurse -Include "*.cs"

    # Process and concatenate code from each C# file
    foreach ($csFile in $csFiles) {
        if ($csFile.FullName -like "*Migration*") {
            continue;
        }
        if ($csFile.FullName -like "*\obj\*") {
            continue;
        }
        if ($csFile.FullName -like "*OutputFile.txt") {
            continue;
        }
        $processedCode = ProcessCodeFile -filePath $csFile.FullName
        if (-not [string]::IsNullOrWhiteSpace($processedCode)) {
            #$outputFileContent += "`n`n----- $($csFile.FullName) ----`n`n" + $processedCode
            $outputFileContent += $processedCode
        }
    }
}

# Write the concatenated content to the output file
Set-Content -Path $outputFilePath -Value $outputFileContent
Write-Output "Code content concatenated to: $outputFilePath"
