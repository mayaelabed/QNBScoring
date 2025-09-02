# Start with the official Jenkins LTS image as the base
FROM jenkins/jenkins:lts

# Switch to root user to install software
USER root

# Install the Git command-line tool
RUN apt-get update && apt-get install -y git

# Switch back to the jenkins user
USER jenkins