#include "server.h"

int main(int argc, char **argv) {
    processCmdLineArgs(argc, argv);
    forkOnStartup();
    installSignalHandlers();
    //connectToDevice();
    startServer();

    while(1) {
        acceptConnection();
    }

    stopServer();
    return 0;
}


int acceptConnection(void) {
    socklen_t clientInfoSize = sizeof(clientInfo);
    int tmpClientSocket = accept(listenSocket, (struct sockaddr *)&clientInfo, &clientInfoSize);

    // If accept() failed, just return so it's called again
    if(tmpClientSocket == -1) {
        return ERR;
    }
    
    if(verbosity > NO_VERBOSE) {
        char *clientIp = getClientIpAddress();
        printf("%s: Got connection from %s.\n", prog, clientIp);
        free(clientIp);
    }

    // Only allow a connection if one doesn't already exist
    if(childPid == NO_CHILD) {
        clientSocket = tmpClientSocket;

        int pid = fork();
        if(pid == 0) {
            // Get the hello from the client and tell the client we're ready
            char reply[BUFFER];
            int recvStatus = recvFromClient(reply);
            if(recvStatus == NETWORK_ERR || recvStatus == 0 || strcmp(reply, PROT_HELO) != 0 || sendToClient(PROT_REDY) == NETWORK_ERR) {
                if(verbosity > NO_VERBOSE) fprintf(stderr, "%s: Failed handshake with client.\n", prog);
                serverExit(ABNORMAL_EXIT);
            }

            if(verbosity > NO_VERBOSE) printf("%s: Completed handshake with client.\n", prog);

            handleConnection();
            return ERR; // handleConnection() should never return, but to make gcc stop showing a warning, it's here
        } else if(pid != -1) {
            childPid = pid;
            return SUCCESS;
        } else {
            fprintf(stderr, "%s: Failed to fork process to handle new connection.\n", prog);
            close(clientSocket);
            return ERR;
        }
    } else {
        if(verbosity > NO_VERBOSE) printf("%s: Rejecting connection due to active existing connection.\n", prog);
        // Add a newline to the ERR command before sending it (sort of hackish...)
        char tmpErr[sizeof(PROT_ERR)+1];
        strncpy(tmpErr, PROT_ERR, sizeof(PROT_ERR));
        tmpErr[sizeof(PROT_ERR)] = '\n';
        tmpErr[sizeof(PROT_ERR)+1] = '\0';
        send(tmpClientSocket, tmpErr, sizeof(PROT_ERR)+1, 0);
        close(tmpClientSocket);
        return ERR;
    }
}


void handleConnection() {
    char command[BUFFER];

    while(1) {
        // Get the command from the client
        int recvStatus = recvFromClient(command);
        switch(recvStatus) {
            case  0:
                if(verbosity > NO_VERBOSE) fprintf(stderr, "%s: Client unexpectedly closed the connection.\n", prog);
                break;
            case NETWORK_ERR:
                if(verbosity > NO_VERBOSE) fprintf(stderr, "%s: Error communicating with client: %s\n", prog, strerror(errno));
                break;
        }

        if(verbosity >= DBL_VERBOSE) printf("%s: Client sent command \"%s\".\n", prog, command);

        // Exit if client sent the end command
        if(strcmp(command, PROT_END) == 0) {
            if(verbosity > DBL_VERBOSE) printf("%s: Client requested to end the connection.\n", prog);
            break;
        }

        if(processProtocolCommand(command) == ERR) {
            if(verbosity >= DBL_VERBOSE) printf("%s: Sending ERR reply to client.\n", prog);
            sendToClient(PROT_ERR);
        } else {
            if(verbosity >= DBL_VERBOSE) printf("%s: Sending ACK reply to client.\n", prog);
            sendToClient(PROT_ACK);
        }
    }

    if(verbosity > NO_VERBOSE) printf("%s: Ending connection with client.\n", prog);
    serverExit(NORMAL_EXIT);
}


int processProtocolCommand(char *command) {
    char *arg = strtok(command, " ");

    if(strcmp(arg, PROT_DRIVE) == 0) {
        return processDriveCommand();
    } else if(strcmp(arg, PROT_LED) == 0) {
        return processLedCommand();
    } else if(strcmp(arg, PROT_SONG) == 0) {
        return processSongCommand();
    } else if(strcmp(arg, PROT_WAIT) == 0) {
        return processWaitCommand();
    } else if(strcmp(arg, PROT_MODE) == 0) {
        return processModeCommand();
    } else if(strcmp(arg, PROT_BEEP) == 0) {
        biscBeep();
        return SUCCESS;
    } else {
        return ERR;
    }
}


int processDriveCommand(void) {
    char *arg = strtok(NULL, " ");

    while(arg != NULL) {
        arg = strtok(NULL, " ");
    }

    return SUCCESS;
}


int processLedCommand(void) {
    char *arg = strtok(NULL, " ");

    while(arg != NULL) {
        arg = strtok(NULL, " ");
    }

    return SUCCESS;
}


int processSongCommand(void) {
    char *arg = strtok(NULL, " ");

    while(arg != NULL) {
        arg = strtok(NULL, " ");
    }

    return SUCCESS;
}


int processWaitCommand(void) {
    char *arg = strtok(NULL, " ");

    while(arg != NULL) {
        arg = strtok(NULL, " ");
    }

    return SUCCESS;
}


int processModeCommand(void) {
    char *arg = strtok(NULL, " ");

    while(arg != NULL) {
        arg = strtok(NULL, " ");
    }

    return SUCCESS;
}


void forkOnStartup(void) {
    if(!noFork) {
        int pid = fork();
        if(pid != 0) {
            exit(NORMAL_EXIT);
        } else if(pid == -1) {
            fprintf(stderr, "%s: Failed to fork on startup.\n", prog);
        }
   }
}


void connectToDevice(void) {
    if(verbosity > NO_VERBOSE) printf("%s: Connecting to device...\n", prog);

    if(biscInit(device) == BISC_ERR) {
        fprintf(stderr, "%s: Error connecting to device \"%s\".\n", prog, device);
        exit(ABNORMAL_EXIT);
    }
}
