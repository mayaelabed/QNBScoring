pipeline {
  agent any

  triggers { pollSCM('H/2 * * * *') }  // auto-build on pushes (or near real-time)

  environment {
    DOCKER_BUILDKIT = '1'
    NUGET_DIR = '/var/jenkins_home/.nuget/packages'
  }

  options { timestamps() }

  stages {
    stage('Checkout') {
      steps { checkout scm }
    }

    stage('Build & Test (.NET 9)') {
      steps {
        sh '''
          set -euo pipefail

          # Job-scoped caches (no cross-job races)
          export HOME="$WORKSPACE"
          export DOTNET_CLI_HOME="$WORKSPACE"
          export NUGET_PACKAGES="$WORKSPACE/.nuget/packages"
          export NUGET_HTTP_CACHE_PATH="$WORKSPACE/.nuget/http-cache"
          export NUGET_PLUGINS_CACHE_PATH="$WORKSPACE/.nuget/plugin-cache"

          mkdir -p "$HOME/.dotnet/tools" "$NUGET_PACKAGES" "$NUGET_HTTP_CACHE_PATH" "$NUGET_PLUGINS_CACHE_PATH"

          # Clean only transient caches; DO NOT wipe global packages
          dotnet nuget locals http-cache,temp --clear

          # Restore with cache enabled (faster & more reliable); parallel is fine now that caches are isolated
          dotnet restore QNBScoring.sln

          # Build & test
          if ! dotnet build QNBScoring.sln -c Release --no-restore; then
            echo "Build couldn't find some packages; re-restoring and retrying..."
            dotnet restore QNBScoring.sln
            dotnet build QNBScoring.sln -c Release --no-restore
          fi

          dotnet test QNBScoring.UnitTests/QNBScoring.UnitTests.csproj -c Release --no-build
        '''
      }
    }


    stage('SonarQube Analysis') {
      steps {
        withSonarQubeEnv('SonarQube Server') {
          withCredentials([string(credentialsId: 'sonar-token', variable: 'SONAR_TOKEN')]) {
            sh '''
              set -euo pipefail

              export HOME="$WORKSPACE"
              export DOTNET_CLI_HOME="$WORKSPACE"
              export NUGET_PACKAGES="$WORKSPACE/.nuget/sonar-packages"
              export NUGET_HTTP_CACHE_PATH="$WORKSPACE/.nuget/sonar-http-cache"
              export NUGET_PLUGINS_CACHE_PATH="$WORKSPACE/.nuget/sonar-plugin-cache"

              rm -rf "$NUGET_PACKAGES" "$NUGET_HTTP_CACHE_PATH" "$NUGET_PLUGINS_CACHE_PATH"
              mkdir -p "$HOME/.dotnet/tools" "$NUGET_PACKAGES" "$NUGET_HTTP_CACHE_PATH" "$NUGET_PLUGINS_CACHE_PATH"
              export PATH="$PATH:$HOME/.dotnet/tools"

              dotnet tool update --global dotnet-sonarscanner || dotnet tool install --global dotnet-sonarscanner
              dotnet nuget locals http-cache,temp --clear

              dotnet sonarscanner begin /k:"QNBScoring-Web-App" /d:sonar.host.url="$SONAR_HOST_URL" /d:sonar.login="$SONAR_TOKEN"

              dotnet restore QNBScoring.sln
              dotnet build QNBScoring.sln -c Release --no-restore

              dotnet sonarscanner end /d:sonar.login="$SONAR_TOKEN"
            '''
          }
        }
      }
    }



    stage('Build Web App Docker Image') {
      steps {
        sh 'docker build -t mayaelabed/qnb-scoring-web:latest -f Dockerfile .'
      }
    }
  }
}