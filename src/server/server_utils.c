#include "server.h"

int sendToClient(const char *msg) {
    // Add a newline to the end of the message
    int msgLen = strlen(msg);
    char *newlineMsg = malloc(msgLen + 2);
    strncpy(newlineMsg, msg, msgLen);
    newlineMsg[msgLen] = '\n';
    newlineMsg[msgLen+1] = '\0';

    int ret = send(clientSocket, newlineMsg, msgLen+1, 0);
    free(newlineMsg);
    return ret;
}


int recvFromClient(char *reply) {
    int recvLen = recv(clientSocket, (char*)reply, BUFFER, 0);

    // Remove the newline and/or carriage return character(s) if they exist
    if(recvLen >= 0) {
        if(reply[recvLen-2] == '\r') {
            reply[recvLen-2] = '\0';
        } else if(reply[recvLen-1] == '\n') {
            reply[recvLen-1] = '\0';
        }
    }

    return recvLen;
}


int startServer(void) {
    if(verbosity > NO_VERBOSE) printf("%s: Starting server...\n", prog);

    if(getServerInfo(port) == ERR || bindToSocket() == ERR || listen(listenSocket, BACKLOG) != 0) {
        fprintf(stderr, "%s: Failed to start server: %s\n", prog, strerror(errno));
        exit(ABNORMAL_EXIT);
    }

    childPid = NO_CHILD;

    if(verbosity >= DBL_VERBOSE) printf("%s: Server started.\n", prog);
    return SUCCESS;
}


void stopServer(void) {
    if(childPid != NO_CHILD) {
        kill(childPid, SIGTERM);
        childPid = NO_CHILD;
    }

    close(clientSocket);
    close(listenSocket);

    freeaddrinfo(serverInfo);
    serverInfo = NULL;
}


int getServerInfo(char *port) {
    struct addrinfo hints;
    memset(&hints, 0, sizeof(hints));

    hints.ai_family   = AF_UNSPEC;
    hints.ai_flags    = AI_PASSIVE;
    hints.ai_socktype = SOCK_STREAM;

    return (getaddrinfo(NULL, port, &hints, &serverInfo) == 0 ? SUCCESS : ERR); 
}


int bindToSocket() {
    // Traverse list of results and bind to first socket possible
    struct addrinfo *curServerInfo;
    for(curServerInfo = serverInfo; curServerInfo != NULL; curServerInfo = curServerInfo->ai_next) {
        // Try to get socket
        if((listenSocket = socket(curServerInfo->ai_family, curServerInfo->ai_socktype, curServerInfo->ai_protocol)) == -1) {
            continue;
        }

        // Allow reuse of port
        int sockOpt = 1;
        if(setsockopt(listenSocket, SOL_SOCKET, SO_REUSEADDR, &sockOpt, sizeof(sockOpt)) != 0) {
            continue;
        }

        // Try to bind to socket
        if(bind(listenSocket, curServerInfo->ai_addr, curServerInfo->ai_addrlen) != 0) {
            close(listenSocket);
            continue;
        }

        break;
    }

    // If the current server info is null, we failed to bind
    if(curServerInfo == NULL) {
        listenSocket = ERR;
        return ERR;
    }

    return SUCCESS;
}


char* getClientIpAddress(void) {
    char *ip = malloc(INET6_ADDRSTRLEN);
    inet_ntop(clientInfo.ss_family, &((struct sockaddr_in*)&clientInfo)->sin_addr, ip, INET6_ADDRSTRLEN);
    return ip;
}