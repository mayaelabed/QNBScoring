pipeline {
  agent any
  environment {
    DOCKER_BUILDKIT = '1'
  }
  options { timestamps() }

  stages {
    stage('Checkout') {
      steps { checkout scm }
    }

    stage('Build & Test (.NET 6)') {
      steps {
        sh '''
          docker run --rm \
            -u $(id -u):$(id -g) \
            -e HOME=/workspace \
            -e DOTNET_CLI_HOME=/workspace \
            -e NUGET_PACKAGES=/workspace/.nuget/packages \
            -e DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1 \
            -e DOTNET_NOLOGO=1 \
            -v "$PWD":/workspace -w /workspace \
            mcr.microsoft.com/dotnet/sdk:6.0 \
            bash -lc '
              mkdir -p /workspace/.dotnet/tools /workspace/.nuget/packages
              dotnet --info
              dotnet restore QNBScoring.sln
              dotnet build QNBScoring.sln -c Release --no-restore
              dotnet test QNBScoring.UnitTests/QNBScoring.UnitTests.csproj -c Release --no-build
            '
        '''
      }
    }

    stage('SonarQube Analysis') {
      steps {
        withSonarQubeEnv('SonarQube Server') {
          withCredentials([string(credentialsId: 'SONARQUBE_TOKEN', variable: 'SONAR_TOKEN')]) {
            sh '''
              docker run --rm \
                -u $(id -u):$(id -g) \
                -e HOME=/workspace \
                -e DOTNET_CLI_HOME=/workspace \
                -e NUGET_PACKAGES=/workspace/.nuget/packages \
                -e DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1 \
                -e DOTNET_NOLOGO=1 \
                -e SONAR_HOST_URL="$SONAR_HOST_URL" \
                -e SONAR_TOKEN="$SONAR_TOKEN" \
                -v "$PWD":/workspace -w /workspace \
                mcr.microsoft.com/dotnet/sdk:6.0 \
                bash -lc '
                  mkdir -p /workspace/.dotnet/tools /workspace/.nuget/packages
                  export PATH="$PATH:/workspace/.dotnet/tools"
                  dotnet tool update --global dotnet-sonarscanner || dotnet tool install --global dotnet-sonarscanner
                  dotnet sonarscanner begin /k:"QNBScoring-Web-App" /d:sonar.host.url="$SONAR_HOST_URL" /d:sonar.login="$SONAR_TOKEN"
                  dotnet build QNBScoring.sln -c Release
                  dotnet sonarscanner end /d:sonar.login="$SONAR_TOKEN"
                '
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
