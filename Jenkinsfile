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
          docker run --rm \
            --volumes-from jenkins_server \
            -u 1000:0 \
            -e HOME="$WORKSPACE" \
            -e DOTNET_CLI_HOME="$WORKSPACE" \
            -e NUGET_PACKAGES="$NUGET_DIR" \
            -e DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1 \
            -e DOTNET_NOLOGO=1 \
            -w "$WORKSPACE" \
            mcr.microsoft.com/dotnet/sdk:9.0 \
            bash -lc '
              set -euo pipefail
              mkdir -p "$HOME/.dotnet/tools" "$NUGET_PACKAGES"
              dotnet --info
              dotnet nuget locals all --clear
              dotnet restore QNBScoring.sln --force-evaluate --no-cache --disable-parallel
              dotnet build   QNBScoring.sln -c Release --no-restore
              dotnet test    QNBScoring.UnitTests/QNBScoring.UnitTests.csproj -c Release --no-build
            '
        '''
      }
    }

    stage('SonarQube Analysis') {
      when { expression { return currentBuild.resultIsBetterOrEqualTo('SUCCESS') } }
      steps {
        withSonarQubeEnv('SonarQube Server') {
          sh '''
            set -euo pipefail

            # Isolate caches for this stage to avoid corrupted global state
            export HOME="$WORKSPACE"
            export DOTNET_CLI_HOME="$WORKSPACE"
            export NUGET_PACKAGES="$WORKSPACE/.nuget/sonar_packages"

            rm -rf "$NUGET_PACKAGES"
            mkdir -p "$HOME/.dotnet/tools" "$NUGET_PACKAGES"
            export PATH="$PATH:$HOME/.dotnet/tools"

            # Ensure scanner is available
            dotnet tool update --global dotnet-sonarscanner || dotnet tool install --global dotnet-sonarscanner

            # Clear any other NuGet caches
            dotnet nuget locals all --clear

            # Begin analysis
            dotnet sonarscanner begin /k:"QNBScoring-Web-App" /d:sonar.host.url="$SONAR_HOST_URL" /d:sonar.login="$SONAR_TOKEN"

            # Robust restore (retry once if it flakes)
            if ! dotnet restore QNBScoring.sln --force-evaluate --no-cache --disable-parallel; then
              echo "Restore failed once; purging packages and retrying..."
              rm -rf "$NUGET_PACKAGES"
              dotnet restore QNBScoring.sln --force-evaluate --no-cache --disable-parallel
            fi

            dotnet build QNBScoring.sln -c Release --no-restore

            # End analysis (creates report-task.txt)
            dotnet sonarscanner end /d:sonar.login="$SONAR_TOKEN"
          '''
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