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
      agent {
        docker {
          image 'mcr.microsoft.com/dotnet/sdk:6.0'
          reuseNode true
        }
      }
      steps {
        sh 'dotnet --info'
        sh 'dotnet restore QNBScoring.sln'
        sh 'dotnet build QNBScoring.sln -c Release --no-restore'
        sh 'dotnet test QNBScoring.UnitTests/QNBScoring.UnitTests.csproj -c Release --no-build'
      }
    }

    stage('SonarQube Analysis') {
      agent {
        docker {
          image 'mcr.microsoft.com/dotnet/sdk:6.0'
          reuseNode true
        }
      }
      environment {
        PATH = "$HOME/.dotnet/tools:$PATH"
      }
      steps {
        withSonarQubeEnv('SonarQube Server') {
          withCredentials([string(credentialsId: 'SONARQUBE_TOKEN', variable: 'SONAR_TOKEN')]) {
            sh 'dotnet tool update --global dotnet-sonarscanner || dotnet tool install --global dotnet-sonarscanner'
            sh 'dotnet sonarscanner begin /k:"QNBScoring-Web-App" /d:sonar.host.url="$SONAR_HOST_URL" /d:sonar.login="$SONAR_TOKEN"'
            sh 'dotnet build QNBScoring.sln -c Release'
            sh 'dotnet sonarscanner end /d:sonar.login="$SONAR_TOKEN"'
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
