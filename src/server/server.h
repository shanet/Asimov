#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <unistd.h>
#include <getopt.h>
#include <signal.h>
#include <errno.h>
#include <sys/wait.h>

#include <sys/types.h>
#include <sys/socket.h>
#include <arpa/inet.h>
#include <netdb.h>

#include "bisc.h"


#define SUCCESS       0
#define ERR           1
#define NORMAL_EXIT   0
#define ABNORMAL_EXIT 1

#define NO_CHILD       -1
#define BUFFER         1024
#define BACKLOG        10
#define DEFAULT_DEVICE "/dev/ttyUSB0"
#define DEFAULT_PORT   "3737"

#define NO_VERBOSE  0
#define VERBOSE     1
#define DBL_VERBOSE 2
#define TPL_VERBOSE 3

#define PROT_ERR  "ERR\n"
#define PROT_REDY "REDY\n"


int listenSocket;
int clientSocket;
struct addrinfo *serverInfo;
struct sockaddr_storage clientInfo;

char *prog;
char *device;
char *port;
int noFork;
int verbosity;
int childPid;


int sendToClient(const char *msg);
int recvFromClient(char *reply);

int startServer(void);
void stopServer(void);
int getServerInfo(char *port);
int bindToSocket(void);
int acceptConnection(void);
void handleConnection(void);
char* getClientIpAddress(void);

void forkOnStartup(void);
void installSignalHandlers(void);
void uninstallSignalHandlers(void);
void signalHandler(const int signal);
void connectToDevice(void);
void processCmdLineArgs(int argc, char **argv);
void printVersion(void);
void printHelp(void);
