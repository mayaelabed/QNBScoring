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

    stage('Build & Test (.NET 9)') {
      steps {
        sh '''
          docker run --rm \
            --volumes-from jenkins_server \
            -u 1000:0 \
            -e HOME=/var/jenkins_home/workspace/Qnb-CI-Pipeline \
            -e DOTNET_CLI_HOME=/var/jenkins_home/workspace/Qnb-CI-Pipeline \
            -e NUGET_PACKAGES=/var/jenkins_home/workspace/Qnb-CI-Pipeline/.nuget/packages \
            -e DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1 \
            -e DOTNET_NOLOGO=1 \
            -w /var/jenkins_home/workspace/Qnb-CI-Pipeline \
            mcr.microsoft.com/dotnet/sdk:9.0 \
            bash -lc '
              mkdir -p "$HOME/.dotnet/tools" "$NUGET_PACKAGES"
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
                --volumes-from jenkins_server \
                -u 1000:0 \
                -e HOME=/var/jenkins_home/workspace/Qnb-CI-Pipeline \
                -e DOTNET_CLI_HOME=/var/jenkins_home/workspace/Qnb-CI-Pipeline \
                -e NUGET_PACKAGES=/var/jenkins_home/workspace/Qnb-CI-Pipeline/.nuget/packages \
                -e DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1 \
                -e DOTNET_NOLOGO=1 \
                -e SONAR_HOST_URL="http://host.docker.internal:9000" \
                -e SONAR_TOKEN="$SONAR_TOKEN" \
                -w /var/jenkins_home/workspace/Qnb-CI-Pipeline \
                mcr.microsoft.com/dotnet/sdk:9.0 \
                bash -lc '
                  mkdir -p "$HOME/.dotnet/tools" "$NUGET_PACKAGES"
                  export PATH="$PATH:$HOME/.dotnet/tools"
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
