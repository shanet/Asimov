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
        return (biscBeep() == BISC_SUCCESS ? SUCCESS : ERR); 
    }

    return ERR;
}


char* getNextArg(void) {
    return strtok(NULL, " ");
}


int processDriveCommand(void) {
    char *arg = getNextArg();
    if(arg == NULL) return ERR;

    // DRIVE NORMAL
    if(strcmp(arg, PROT_DRIVE_NORMAL) == 0) {
        arg = getNextArg();
        if(arg == NULL) return ERR;
        int velocity = atoi(arg);

        arg = getNextArg();
        if(arg == NULL) return ERR;
        int radius = atoi(arg);

        return (biscDrive(velocity, radius) == BISC_SUCCESS ? SUCCESS : ERR); 
    // DRIVE TIME/DISTANCE
    } else if(strcmp(arg, PROT_DRIVE_TIME) == 0 || strcmp(arg, PROT_DRIVE_DISTANCE) == 0) {
        int driveType;
        if(strcmp(arg, PROT_DRIVE_TIME) == 0) {
            driveType = 1;
        } else {
            driveType = 2;
        }

        arg = getNextArg();
        if(arg == NULL) return ERR;
        int velocity = atoi(arg);

        arg = getNextArg();
        if(arg == NULL) return ERR;
        int radius = atoi(arg);

        arg = getNextArg();
        if(arg == NULL) return ERR;
        int waitArg = atoi(arg);

        if(driveType == 1) {
            return (biscTimedDrive(velocity, radius, waitArg) == BISC_SUCCESS ? SUCCESS : ERR); 
        } else {
            return (biscDriveDistance(velocity, radius, waitArg) == BISC_SUCCESS ? SUCCESS : ERR);
        }
    // DRIVE STRAIGHT
    } else if(strcmp(arg, PROT_DRIVE_STRAIGHT) == 0) {
        int driveType;
        if(strcmp(arg, PROT_DRIVE_NORMAL) == 0) {
            driveType = 1;
        } else if(strcmp(arg, PROT_DRIVE_TIME) == 0) {
            driveType = 2;
        } else if(strcmp(arg, PROT_DRIVE_DISTANCE) == 0) {
            driveType = 3;
        } else {
           return ERR;
        }

        arg = getNextArg();
        if(arg == NULL) return ERR;
        int velocity = atoi(arg);

        int waitArg;
        if(driveType == 2 || driveType == 3) {
           arg = getNextArg();
           if(arg == NULL) return ERR;
           waitArg = atoi(arg);
        }

        if(driveType == 1) {
            return (biscDriveStraight(velocity) == BISC_SUCCESS ? SUCCESS : ERR);
        } else if(driveType == 2) {
            return (biscTimedDriveStraight(velocity, waitArg) == BISC_SUCCESS ? SUCCESS : ERR);
        } else {
            return (biscDriveDistanceStraight(velocity, waitArg) == BISC_SUCCESS ? SUCCESS : ERR);
        }
    // DRIVE DIRECT
    } else if(strcmp(arg, PROT_DRIVE_DIRECT) == 0) {
        arg = getNextArg();
        if(arg == NULL) return ERR;
        int rightVelocity = atoi(arg);

        arg = getNextArg();
        if(arg == NULL) return ERR;
        int leftVelocity = atoi(arg);

        return (biscDirectDrive(rightVelocity, leftVelocity) == BISC_SUCCESS ? SUCCESS : ERR); 
    // DRIVE SPIN
    } else if(strcmp(arg, PROT_DRIVE_SPIN) == 0) {
        int spinType;
        if(strcmp(arg, PROT_DRIVE_NORMAL) == 0) {
            spinType = 1;
        } else if(strcmp(arg, PROT_DRIVE_TIME) == 0) {
            spinType = 2;
        } else if(strcmp(arg, PROT_DRIVE_ANGLE) == 0) {
            spinType = 3;
        } else {
           return ERR;
        }

        arg = getNextArg();
        if(arg == NULL) return ERR;
        int velocity = atoi(arg);

        int waitArg;
        if(spinType == 2 || spinType == 3) {
           arg = getNextArg();
           if(arg == NULL) return ERR;
           waitArg = atoi(arg);
        }

        if(spinType == 1) {
            return (biscSpin(velocity) == BISC_SUCCESS ? SUCCESS : ERR);
        } else if(spinType == 2) {
            return (biscTimedSpin(velocity, waitArg) == BISC_SUCCESS ? SUCCESS : ERR);
        } else {
            return (biscSpinAngle(velocity, waitArg) == BISC_SUCCESS ? SUCCESS : ERR);
        }
    // DRIVE STOP
    } else if(strcmp(arg, PROT_DRIVE_STOP) == 0) {
        return (biscDriveStop() == BISC_SUCCESS ? SUCCESS : ERR);
    }

    return ERR;
}


int processLedCommand(void) {
    char *arg = getNextArg();
    if(arg == NULL) return ERR;

    // LED ADVANCE
    if(strcmp(arg, PROT_LED_ADVANCE) == 0) {
        arg = getNextArg();
        if(arg == NULL) return ERR;

        // LED ADVANCE ON
        if(strcmp(arg, PROT_LED_ON) == 0) {
            return (biscTurnOnAdvanceLed() == BISC_SUCCESS ? SUCCESS : ERR); 
        // LED ADVANCE OFF
        } else if(strcmp(arg, PROT_LED_OFF) == 0) {
            return (biscTurnOffAdvanceLed() == BISC_SUCCESS ? SUCCESS : ERR); 
        }
    // LED PLAY
    } else if(strcmp(arg, PROT_LED_PLAY) == 0) {
        arg = getNextArg();
        if(arg == NULL) return ERR;

        // LED PLAY ON
        if(strcmp(arg, PROT_LED_ON) == 0) {
            return (biscTurnOnPlayLed() == BISC_SUCCESS ? SUCCESS : ERR); 
        //LED PLAY OFF
        } else if(strcmp(arg, PROT_LED_OFF) == 0) {
            return (biscTurnOffPlayLed() == BISC_SUCCESS ? SUCCESS : ERR); 
        }
    // LED POWER
    } else if(strcmp(arg, PROT_LED_POWER) == 0) {
        arg = getNextArg();
        if(arg == NULL) return ERR;

        // LED POWER OFF
        if(strcmp(arg, PROT_LED_OFF) == 0) {
            return (biscTurnOffPowerLed() == BISC_SUCCESS ? SUCCESS : ERR); 
        // LED POWER [color] [intensity]
        } else {
            arg = getNextArg();
            if(arg == NULL) return ERR;
            int color = atoi(arg);

            arg = getNextArg();
            if(arg == NULL) return ERR;
            int intensity = atoi(arg);

            return (biscSetPowerLed(color, intensity) == BISC_SUCCESS ? SUCCESS : ERR); 
        }
    // LED FLASH
    } else if(strcmp(arg, PROT_LED_FLASH) == 0) {
        arg = getNextArg();
        if(arg == NULL) return ERR;

        int led;
        if(strcmp(arg, PROT_LED_POWER) == 0) {
            led = BISC_POWER_LED;
        } else if(strcmp(arg, PROT_LED_ADVANCE) == 0) {
            led = BISC_ADVANCE_LED;
        } else if(strcmp(arg, PROT_LED_PLAY) == 0) {
            led = BISC_PLAY_LED;
        } else {
            return ERR;
        }

        arg = getNextArg();
        if(arg == NULL) return ERR;
        int numFlashes = atoi(arg);

        arg = getNextArg();
        if(arg == NULL) return ERR;
        int flashDuration = atoi(arg);

        return (biscFlashLed(led, numFlashes, flashDuration) == BISC_SUCCESS ? SUCCESS : ERR); 
    }

    return ERR;
}


int processSongCommand(void) {
    char *arg = getNextArg();
    if(arg == NULL) return ERR;

    if(strcmp(arg, PROT_SONG_DEFINE) == 0) {
        arg = getNextArg();
        if(arg == NULL) return ERR;
        int songNum = atoi(arg);

        // The notes and durations are in a CSV list. They need parsed separately.
        char *unparsedNotes = getNextArg();
        if(unparsedNotes == NULL) return ERR;
        char *unparsedDurations = getNextArg();
        if(unparsedDurations == NULL) return ERR;

        int songLen;
        unsigned char notes[BISC_MAX_SONG_LEN];
        unsigned char durations[BISC_MAX_SONG_LEN];

        char *note = strtok(unparsedNotes, ",");
        if(note == NULL) return ERR;

        while(note != NULL) {
            notes[songLen] = note[0];
            songLen++;
            note = strtok(NULL, ",");
        }

        char *duration = strtok(unparsedDurations, ",");
        if(duration == NULL) return ERR;

        while(duration != NULL) {
            durations[songLen] = duration[0];
            duration = strtok(NULL, ",");
        }

        return (biscDefineSong((char)songNum, notes, durations, songLen) == BISC_SUCCESS ? SUCCESS : ERR);
    } else if(strcmp(arg, PROT_SONG_PLAY) == 0) {
        arg = getNextArg();
        if(arg == NULL) return ERR;
        int songNum = atoi(arg);

        return (biscPlaySong(songNum) == BISC_SUCCESS ? SUCCESS : ERR);
    }

    return ERR;
}


int processWaitCommand(void) {
    char *arg = getNextArg();
    if(arg == NULL) return ERR;

    char *waitType = strdup(arg);
    if(waitType == NULL) return ERR;

    arg = getNextArg();
    if(arg == NULL) return ERR;
    int waitArg = atoi(arg);    
    
    // WAIT TIME/DISTANCE/ANGLE/EVENT [arg]
    if(strcmp(waitType, PROT_WAIT_TIME) == 0) {
        return (biscWaitTime(waitArg) == BISC_SUCCESS ? SUCCESS : ERR); 
    } else if(strcmp(waitType, PROT_WAIT_DISTANCE) == 0) {
        return (biscWaitDistance(waitArg) == BISC_SUCCESS ? SUCCESS : ERR); 
    } else if(strcmp(waitType, PROT_WAIT_ANGLE) == 0) {
        return (biscWaitAngle(waitArg) == BISC_SUCCESS ? SUCCESS : ERR); 
    } else if(strcmp(waitType, PROT_WAIT_EVENT) == 0) {
        return (biscWaitEvent(waitArg) == BISC_SUCCESS ? SUCCESS : ERR); 
    }

    return ERR;
}


int processModeCommand(void) {
    char *arg = getNextArg();
    if(arg == NULL) return ERR;

    // MODE FULL/SAFE/PASSIVE
    int mode;
    if(strcmp(arg, PROT_MODE_FULL) == 0) {
        mode = BISC_MODE_FULL;
    } else if(strcmp(arg, PROT_MODE_SAFE) == 0) {
        mode = BISC_MODE_SAFE;
    } else if(strcmp(arg, PROT_MODE_PASSIVE) == 0) {
        mode = BISC_MODE_PASSIVE;
    } else {
        return ERR;
    }

    return (biscChangeMode(mode) == BISC_SUCCESS ? SUCCESS : ERR); 
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
