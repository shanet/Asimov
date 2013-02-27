#include "server.h"

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
        if(verbosity > NO_VERBOSE) printf("%s: Device not specified. Defaulting to \"%s\".\n", prog, DEFAULT_DEVICE);
        device = DEFAULT_DEVICE;
    }
    if(port == NULL) {
        if(verbosity > NO_VERBOSE) printf("%s: Port not specified. Defaulting to \"%s\".\n", prog, DEFAULT_PORT);
        port = DEFAULT_PORT;
    }
}


void installSignalHandlers(void) {
    struct sigaction sa;
    memset(&sa, 0, sizeof(sa));
    sa.sa_handler = signalHandler;
    sa.sa_flags = SA_RESTART;
    sigemptyset(&sa.sa_mask);
    if(sigaction(SIGINT,  &sa, NULL) == -1 || sigaction(SIGTERM, &sa, NULL) == -1 || sigaction(SIGCHLD, &sa, NULL) == -1) {
        fprintf(stderr, "%s: Failed to install signal handlers.\n", prog);
        exit(ABNORMAL_EXIT);
    }
}


void signalHandler(const int signal) {
    if(signal == SIGINT || signal == SIGTERM) {
        serverExit(NORMAL_EXIT);
    } else if(signal == SIGCHLD) {
        // When the child returns, set the child pid back to no child so a new client can be accepted
        pid_t returned_child = wait(NULL);
        if(returned_child == childPid) {
            close(clientSocket);
            childPid = NO_CHILD;
        }
    }
}


void serverExit(int returnCode) {
    stopServer();
    exit(returnCode);
}


void printVersion(void) {
    printf("TODO.\n");
}


void printHelp(void) {
    printf("TODO.\n");
}
