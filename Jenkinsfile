pipeline {
    agent any

    stages {

        stage('Restore') {
            steps {
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
                sh 'dotnet test UrlHealthMonitor.sln --no-build --logger trx'
            }
        }
    }

    post {
        always {
            junit '**/*.trx'
        }
        success {
            echo 'Build succeeded ✅'
        }
        failure {
            echo 'Build failed ❌'
        }
    }
}