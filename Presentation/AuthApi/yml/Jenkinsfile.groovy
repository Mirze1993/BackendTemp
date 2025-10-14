pipeline {
    environment {
        dockerimagename = "mirze1993/auth-api:B${BUILD_NUMBER}"
        dockerImage = ""
        GRAFANA_BASIC_AUTH_PASSWORD= credentials('grafana-password')
    }
    agent any

    stages {
        stage('Hello') {
            steps {
                echo 'Start'
            }
        }
        stage('Checkout Source') {
            steps {
                git credentialsId: 'github',branch: 'main', url: 'https://github.com/Mirze1993/BackendTemp.git'
            }
        }
        
        
        stage('Build image') {
            steps{
                script {
                    dockerImage = docker.build("${dockerimagename}", "-f Presentation/AuthApi/Dockerfile .")
                }
          }
        }
        stage('docker compose up'){
               steps{
                   sh "docker compose -f Presentation/AuthApi/yml/docker-compose.yml up -d"      
               }
       }
        
        
        // stage('Generate Compose Files') {
        //     steps {
        //         script {
        //             sh """
        //                 pwd 
        //                 ls
        //                 cd Presentation/AuthApi/yml
        //                 chmod +x gen-compose.sh
        //                 ./gen-compose.sh 2 ${dockerimagename}
        //                 docker compose -f docker-compose.generated.yml up -d 
        //             """
        //         }
        //     }
        // }
    }
}