pipeline {
    agent any

    stages {
        stage('Checkout') {
            steps {
                echo "Checking out Git repository..."
            }
        }

        stage('Build Core & Infrastructure') {
            steps {
                script {
                    echo "Building QNBScoring.Core and QNBScoring.Infrastructure..."
                    // This command should build the entire solution, handling dependencies
                    sh 'dotnet build QNBScoring.sln'
                }
            }
        }

        stage('Run Unit Tests') {
            steps {
                dir('QNBScoring.UnitTests') {
                    // This command runs all tests in the UnitTests project
                    sh 'dotnet test'
                }
            }
        }

        stage('SonarQube Analysis') {
            steps {
                // SonarQube analysis for the entire solution
                withSonarQubeEnv('SonarQube Server') {
                    withCredentials([string(credentialsId: 'SONARQUBE_TOKEN', variable: 'SONAR_TOKEN')]) {
                        // The /k parameter is the project key in SonarQube
                        sh "dotnet sonarscanner begin /k:QNBScoring-Web-App /d:sonar.login=${SONAR_TOKEN}"
                        sh 'dotnet build QNBScoring.sln'
                        sh "dotnet sonarscanner end /d:sonar.login=${SONAR_TOKEN}"
                    }
                }
            }
        }

        stage('Build Web App Docker Image') {
            steps {
                dir('QNBScoring.Web') {
                    // This step will build the Docker image for the web application
                    sh 'docker build -t mayaelabed/qnb-scoring-web:latest .'
                }
            }
        }

        // Add stages for pushing and deploying the image here...
    }
}