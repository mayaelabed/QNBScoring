pipeline {
  agent any
  triggers { pollSCM('H/2 * * * *') }
  options { timestamps() }

  stages {
    stage('Checkout') {
      steps { checkout scm }
    }

    stage('Build & Test (.NET 9)') {
      steps {
        sh '''#!/usr/bin/env bash
        set -euo pipefail

        # Create per-job caches in the workspace
        mkdir -p "$WORKSPACE/.nuget/packages" "$WORKSPACE/.nuget/http-cache" "$WORKSPACE/.nuget/plugin-cache" "$WORKSPACE/.dotnet/tools"

        docker run --rm \
          --volumes-from jenkins_server \
          -u 1000:0 \
          -e HOME="$WORKSPACE" \
          -e DOTNET_CLI_HOME="$WORKSPACE" \
          -e NUGET_PACKAGES="$WORKSPACE/.nuget/packages" \
          -e NUGET_HTTP_CACHE_PATH="$WORKSPACE/.nuget/http-cache" \
          -e NUGET_PLUGINS_CACHE_PATH="$WORKSPACE/.nuget/plugin-cache" \
          -w "$WORKSPACE" \
          mcr.microsoft.com/dotnet/sdk:9.0 \
          bash -lc '
            set -euo pipefail
            dotnet --info
            dotnet nuget locals http-cache,temp --clear
            dotnet restore QNBScoring.sln
            if ! dotnet build QNBScoring.sln -c Release --no-restore; then
              echo "Retrying build after restore…"
              dotnet restore QNBScoring.sln
              dotnet build QNBScoring.sln -c Release --no-restore
            fi
            dotnet test QNBScoring.UnitTests/QNBScoring.UnitTests.csproj -c Release --no-build
          '
        '''
      }
    }

    stage('SonarQube Analysis') {
      steps {
        withSonarQubeEnv('SonarQube Server') {
          withCredentials([string(credentialsId: 'sonar-token', variable: 'SONAR_TOKEN')]) {
            sh '''#!/usr/bin/env bash
            set -euo pipefail

            mkdir -p "$WORKSPACE/.nuget/sonar-packages" "$WORKSPACE/.nuget/sonar-http-cache" "$WORKSPACE/.nuget/sonar-plugin-cache" "$WORKSPACE/.dotnet/tools"

            docker run --rm \
              --volumes-from jenkins_server \
              -u 1000:0 \
              -e HOME="$WORKSPACE" \
              -e DOTNET_CLI_HOME="$WORKSPACE" \
              -e NUGET_PACKAGES="$WORKSPACE/.nuget/sonar-packages" \
              -e NUGET_HTTP_CACHE_PATH="$WORKSPACE/.nuget/sonar-http-cache" \
              -e NUGET_PLUGINS_CACHE_PATH="$WORKSPACE/.nuget/sonar-plugin-cache" \
              -e SONAR_HOST_URL="$SONAR_HOST_URL" \
              -e SONAR_TOKEN="$SONAR_TOKEN" \
              -w "$WORKSPACE" \
              mcr.microsoft.com/dotnet/sdk:9.0 \
              bash -lc '
                set -euo pipefail
                export PATH="$PATH:$HOME/.dotnet/tools"
                dotnet tool update --global dotnet-sonarscanner || dotnet tool install --global dotnet-sonarscanner
                dotnet nuget locals http-cache,temp --clear
                dotnet sonarscanner begin /k:"QNBScoring-Web-App" /d:sonar.host.url="$SONAR_HOST_URL" /d:sonar.login="$SONAR_TOKEN"
                dotnet restore QNBScoring.sln
                dotnet build QNBScoring.sln -c Release --no-restore
                dotnet sonarscanner end /d:sonar.login="$SONAR_TOKEN"
              '
            '''
          }
        }
      }
    }

    stage('Build Web App Docker Image') {
      steps {
        sh '''#!/usr/bin/env bash
        set -euo pipefail
        docker build -t mayaelabed/qnb-scoring-web:latest -f Dockerfile .
        '''
      }
    }
  }
}
