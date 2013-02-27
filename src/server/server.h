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


#define SUCCESS        0
#define ERR            1
#define NETWORK_ERR   -1
#define NORMAL_EXIT    0
#define ABNORMAL_EXIT  1

#define NO_CHILD       -1
#define BUFFER         1024
#define BACKLOG        10
#define DEFAULT_DEVICE "/dev/ttyUSB0"
#define DEFAULT_PORT   "4545"

#define NO_VERBOSE  0
#define VERBOSE     1
#define DBL_VERBOSE 2
#define TPL_VERBOSE 3

#define PROT_HELO   "HELO"
#define PROT_REDY   "REDY"
#define PROT_ACK    "ACK"
#define PROT_ERR    "ERR"
#define PROT_END    "END"
#define PROT_BEEP   "BEEP"

#define PROT_DRIVE  "DRIVE"
    #define PROT_DRIVE_NORMAL   "NORMAL"
    #define PROT_DRIVE_TIME     "TIME"
    #define PROT_DRIVE_DISTANCE "DISTANCE"
    #define PROT_DRIVE_STRAIGHT "STRAIGHT"
    #define PROT_DRIVE_DIRECT   "DIRECT"
    #define PROT_DRIVE_SPIN     "SPIN"
    #define PROT_DRIVE_STOP     "STOP"

#define PROT_LED    "LED"
    #define PROT_LED_ADVANCE "ADVANCE"
    #define PROT_LED_PLAY    "PLAY"
    #define PROT_LED_POWER   "POWER"
    #define PROT_LED_FLASH   "FLASH"
    #define PROT_LED_ON      "ON"
    #define PROT_LED_OFF     "OFF"

#define PROT_SONG   "SONG"
    #define PROT_SONG_DEFINE "DEFINE"
    #define PROT_SONG_PLAY   "PLAY"

#define PROT_WAIT   "WAIT"
    #define PROT_WAIT_TIME     "TIME"
    #define PROT_WAIT_DISTANCE "DISTANCE"
    #define PROT_WAIT_ANGLE    "ANGLE"
    #define PROT_WAIT_EVENT    "EVENT"

#define PROT_MODE   "MODE"
    #define PROT_MODE_FULL    "FULL"
    #define PROT_MODE_SAFE    "SAFE"
    #define PROT_MODE_PASSIVE "PASSIVE"


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
void commandLoop(void);
int processProtocolCommand(char *command);
char* getClientIpAddress(void);

int processDriveCommand(void);
int processLedCommand(void);
int processSongCommand(void);
int processWaitCommand(void);
int processModeCommand(void);

void forkOnStartup(void);
void installSignalHandlers(void);
void signalHandler(const int signal);
void connectToDevice(void);
void processCmdLineArgs(int argc, char **argv);
void printVersion(void);
void printHelp(void);
