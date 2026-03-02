pipeline {
    agent any

    tools {
        dotnetsdk 'dotnet8'
    }

    environment {
        PATH = "${tool 'dotnet8'}:${env.PATH}"
    }

    stages {

        stage('Restore') {
            steps {
                sh 'dotnet --version'
                sh 'dotnet restore UrlHealthMonitor.sln'
            }
        }

        stage('Build') {
            steps {
                sh 'dotnet build UrlHealthMonitor.sln --no-restore'
            }
        }

        stage('Test') {
            steps {
                sh 'dotnet test UrlHealthMonitor.sln --no-build'
            }
        }
    }
}
