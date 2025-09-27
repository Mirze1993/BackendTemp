pipeline {
    agent any

    stages {
        stage('Start') {
            steps {
                echo 'Start'
            }
        }

        stage('Read Config') {
            steps {
                script {
                    // deploy.json faylını oxu
                    def deployConfig = readJSON file: 'Scripts/deploy.json'

                    if (deployConfig.authApi == 1) {
                        echo "Deploy icazəsi var. 'authApi' pipeline işə salınır..."
                        build job: 'authApi', wait: false, propagate: false
                    }

                    if (deployConfig.fileApi == 1) {
                        echo "Deploy icazəsi var. 'fileApi' pipeline işə salınır..."
                        build job: 'fileApi', wait: false, propagate: false
                    }

                    if (deployConfig.aiApi == 1) {
                        echo "Deploy icazəsi var. 'aiApi' pipeline işə salınır..."
                        build job: 'aiApi', wait: false, propagate: false
                    }
                }
            }
        }
    }
}