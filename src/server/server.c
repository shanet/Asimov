#include "server.h"

int main(int argc, char **argv) {
    processCmdLineArgs(argc, argv);
    forkOnStartup();
    installSignalHandlers();
    //connectToDevice();
    startServer();

    while(1) {
        if(acceptConnection() == SUCCESS) {
            handleConnection();
        }
    }

    stopServer();

    return 0;
}


void handleConnection() {
    int pid = fork();
    if(pid == 0) {
        uninstallSignalHandlers();
        if(verbosity > NO_VERBOSE) {
            char *clientIp = getClientIpAddress();
            printf("%s: Got connection from %s\n", prog, clientIp);
            free(clientIp);
        }
    } else if(pid != -1) {
        childPid = pid;
    } else {
        fprintf(stderr, "%s: Failed to fork process to handle new connection.\n", prog);
        close(clientSocket);
    }
}


int sendToClient(const char *msg) {
    return send(clientSocket, msg, strlen(msg), 0);
}


int recvFromClient(char *reply) {
    int recvLen = recv(clientSocket, (char*)reply, BUFFER, 0);

    if(recvLen >= 0) {
        reply[recvLen] = '\0';
    }

    return recvLen;
}


int startServer(void) {
    if(verbosity > NO_VERBOSE) {
        printf("%s: Starting server...\n", prog);
    }

    if(getServerInfo(port) == ERR || bindToSocket() == ERR || listen(listenSocket, BACKLOG) != 0) {
        fprintf(stderr, "%s: Failed to start server: %s\n", prog, strerror(errno));
        exit(ABNORMAL_EXIT);
    }

    childPid = NO_CHILD;

    if(verbosity >= DBL_VERBOSE) {
        printf("%s: Server started.\n", prog);
    }
    return SUCCESS;
}


void stopServer(void) {
    if(childPid != NO_CHILD) {
        kill(childPid, SIGTERM);
        childPid = NO_CHILD;
    }

    close(clientSocket);
    close(listenSocket);
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


int acceptConnection(void) {
    socklen_t clientInfoSize = sizeof clientInfo;
    int tmpClientSocket = accept(listenSocket, (struct sockaddr *)&clientInfo, &clientInfoSize);

    // Only allow a connection if one doesn't already exist
    if(childPid == NO_CHILD) {
        clientSocket = tmpClientSocket;
        sendToClient(PROT_REDY);
        return SUCCESS;
    } else {
        if(verbosity > NO_VERBOSE) {
            printf("%s: Rejecting connection due to active existing connection.\n", prog);
        }

        send(tmpClientSocket, PROT_ERR, strlen(PROT_ERR), 0);
        close(tmpClientSocket);
        return ERR;
    }
}


char* getClientIpAddress(void) {
    char *ip = malloc(INET6_ADDRSTRLEN);
    inet_ntop(clientInfo.ss_family, &((struct sockaddr_in*)&clientInfo)->sin_addr, ip, INET6_ADDRSTRLEN);
    return ip;
}


void forkOnStartup(void) {
    if(!noFork) {
        int pid = fork();
        if(pid != 0) {
            exit(NORMAL_EXIT);
        } else if(pid == -1) {
            fprintf(stderr, "%s: Failed to fork on startup.\n", prog);
            exit(ABNORMAL_EXIT);
        }
   }
}


void connectToDevice(void) {
    if(verbosity > NO_VERBOSE) {
        printf("%s: Connecting to device...\n", prog);
    }

    if(biscInit(device) == BISC_ERR) {
        fprintf(stderr, "%s: Error connecting to device \"%s\".\n", prog, device);
        exit(ABNORMAL_EXIT);
    }
}


void processCmdLineArgs(int argc, char **argv) {
    prog = argv[0];

    // In order to call getopt() more than once, optind must be reset to 1
    optind = 1;

    static struct option longOpts[] = {
        {"port",     required_argument, NULL, 'p'},
        {"no-fork",  no_argument,       NULL, 'f'},
        {"verbose",  no_argument,       NULL, 'v'},
        {"version",  no_argument,       NULL, 'V'},
        {"help",     no_argument,       NULL, 'h'},
        {NULL,       0,                 0,      0}
    };

    // Parse the command line args
    char option;
    int optIndex;
    while((option = getopt_long(argc, argv, "p:fvVh", longOpts, &optIndex)) != -1) {
        switch (option) {
            // Port
            case 'p':
                port = optarg;
                break;
            // No fork
            case 'f':
                noFork = 1;
                break;
            // Print help
            case 'h':
                printHelp();
                exit(NORMAL_EXIT);
            // Print version
            case 'V':
                printVersion();
                exit(NORMAL_EXIT);
            // Set verbosity level
            case 'v':
                verbosity++;
                break;
            case '?':
            default:
                printHelp();
                exit(ABNORMAL_EXIT);
        }
    }

    // The last argument should be the device to use
    if(argc == optind+1 && device != NULL) {
        device = argv[optind];
    } else if(argc > optind) {
        fprintf(stderr, "%s: Too many arguments specified.\n", prog);
        printHelp();
        exit(ABNORMAL_EXIT);
    }

    // Default device and port if not specifiec
    if(device == NULL) {
        if(verbosity > NO_VERBOSE) {
            printf("%s: Device not specified. Defaulting to \"%s\".\n", prog, DEFAULT_DEVICE);
        }
        device = DEFAULT_DEVICE;
    }
    if(port == NULL) {
        if(verbosity > NO_VERBOSE) {
            printf("%s: Port not specified. Defaulting to \"%s\".\n", prog, DEFAULT_PORT);
        }
        port = DEFAULT_PORT;
    }
}


void installSignalHandlers(void) {
    struct sigaction sa;
    memset(&sa, 0, sizeof(sa));
    sa.sa_handler = signalHandler;
    //sa.sa_flags = SA_RESTART;
    sigemptyset(&sa.sa_mask);
    if(sigaction(SIGINT,  &sa, NULL) == -1 || sigaction(SIGTERM, &sa, NULL) == -1 || sigaction(SIGCHLD, &sa, NULL) == -1) {
        fprintf(stderr, "%s: Failed to install signal handlers.\n", prog);
        exit(ABNORMAL_EXIT);
    }
}


void uninstallSignalHandlers(void) {
    struct sigaction sa;
    memset(&sa, 0, sizeof(sa));
    sa.sa_handler = SIG_DFL;
    //sa.sa_flags = SA_RESTART;
    sigemptyset(&sa.sa_mask);
    if(sigaction(SIGINT,  &sa, NULL) == -1 || sigaction(SIGTERM, &sa, NULL) == -1 || sigaction(SIGCHLD, &sa, NULL) == -1) {
        fprintf(stderr, "%s: Failed to uninstall signal handlers.\n", prog);
        exit(ABNORMAL_EXIT);
    }
}


void signalHandler(const int signal) {
    if(signal == SIGINT || signal == SIGTERM) {
        if(verbosity >= DBL_VERBOSE) {
            printf("%s: Cleaning up...\n", prog);
        }

        stopServer();
        exit(NORMAL_EXIT);
    } else if(signal == SIGCHLD) {
        // When the child returns, set the child pid back to no child so a new client can be accepted
        pid_t returned_child = waitpid(-1, NULL, WNOHANG);
        if(returned_child == childPid) {
            childPid = NO_CHILD;
        }
    }
}


void printVersion(void) {
    printf("TODO.\n");
}


void printHelp(void) {
    printf("TODO.\n");
}
