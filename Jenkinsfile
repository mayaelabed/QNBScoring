pipeline {
  agent any

  triggers { pollSCM('H/2 * * * *') }
  options { timestamps() }

  environment {
    DOTNET_IMAGE   = 'mcr.microsoft.com/dotnet/sdk:9.0'
    JENKINS_CONTAINER_NAME = 'jenkins_server'   // container that holds /var/jenkins_home
    DOTNET_NOLOGO = '1'
    DOTNET_SKIP_FIRST_TIME_EXPERIENCE = '1'
  }

  stages {
    stage('Checkout') {
      steps { checkout scm }
    }

    stage('Build, Test & Sonar (.NET 9)') {
      steps {
        withSonarQubeEnv('SonarQube Server') {
          withCredentials([string(credentialsId: 'sonar-token', variable: 'SONAR_TOKEN')]) {
            sh """
              docker run --rm \
                --volumes-from ${JENKINS_CONTAINER_NAME} \
                --add-host=host.docker.internal:host-gateway \
                -u 1000:0 \
                -e HOME="${WORKSPACE}" \
                -e DOTNET_CLI_HOME="${WORKSPACE}" \
                -e NUGET_PACKAGES="${WORKSPACE}/.nuget/packages" \
                -e NUGET_HTTP_CACHE_PATH="${WORKSPACE}/.nuget/http-cache" \
                -e NUGET_PLUGINS_CACHE_PATH="${WORKSPACE}/.nuget/plugin-cache" \
                -e DOTNET_NOLOGO="${DOTNET_NOLOGO}" \
                -e DOTNET_SKIP_FIRST_TIME_EXPERIENCE="${DOTNET_SKIP_FIRST_TIME_EXPERIENCE}" \
                -e SONAR_HOST_URL="${SONAR_HOST_URL}" \
                -e SONAR_TOKEN="${SONAR_TOKEN}" \
                -w "${WORKSPACE}" \
                ${DOTNET_IMAGE} bash -lc '
                  set -euo pipefail
                  export PATH="$PATH:$HOME/.dotnet/tools"
                  dotnet --info

                  # Clean transient caches only (keep global packages)
                  dotnet nuget locals http-cache --clear
                  dotnet nuget locals temp --clear
                  dotnet nuget locals plugins-cache --clear || true

                  # Ensure scanner is present
                  dotnet tool update --global dotnet-sonarscanner || dotnet tool install --global dotnet-sonarscanner

                  # Sonar begin -> restore/build/test -> Sonar end
                  dotnet sonarscanner begin /k:"QNBScoring-Web-App" /d:sonar.host.url="$SONAR_HOST_URL" /d:sonar.login="$SONAR_TOKEN"

                  dotnet restore QNBScoring.sln
                  dotnet build QNBScoring.sln -c Release --no-restore || (dotnet restore QNBScoring.sln && dotnet build QNBScoring.sln -c Release --no-restore)
                  dotnet test QNBScoring.UnitTests/QNBScoring.UnitTests.csproj -c Release --no-build

                  dotnet sonarscanner end /d:sonar.login="$SONAR_TOKEN"
                '
            """
          }
        }
      }
    }

    stage('Build Web App Docker Image') {
      when { expression { return fileExists('Dockerfile') } }
      steps {
        sh """
          set -euo pipefail
          docker build -t mayaelabed/qnb-scoring-web:latest -f Dockerfile .
        """
      }
    }
  }
}
