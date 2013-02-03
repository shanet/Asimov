#include "server.h"

int main() {
    printf("Connecting to device...\n");
    if(biscInit("/dev/ttyUSB0") == -1) {
        fprintf(stderr, "Error connecting to device.\n");
        return 1;
    }

    printf("Turning on play LED...\n");
    if(biscTurnOnPlayLed() == -1) {
        fprintf(stderr, "Error turning on play LED.\n");
        return 1;
    }

    sleep(1);

    printf("Turning on advance LED...\n");
    if(biscTurnOnAdvanceLed() == -1) {
        fprintf(stderr, "Error turning on advance LED.\n");
        return 1;
    }

    sleep(1);

    printf("Turning off advance LED...\n");
    if(biscTurnOffAdvanceLed() == -1) {
        fprintf(stderr, "Error turning off advance LED.\n");
        return 1;
    }

    sleep(1);

    printf("Changing power LED to red...\n");
    if(biscSetPowerLed(BISC_POWER_LED_RED, BISC_POWER_LED_FULL) == -1) {
        fprintf(stderr, "Error changing power LED to red.\n");
        return 1;
    }

    sleep(1);

    printf("Flashing play LED...\n");
    if(biscFlashLed(BISC_PLAY_LED, 5, 500) == -1) {
        fprintf(stderr, "Error flashing LED.\n");
        return 1;
    }

    printf("Disconnecting from device...\n");
    if(biscDisconnect() == -1) {
        fprintf(stderr, "Error disconnecting from device.\n");
        return 1;
    }

    printf("All done!\n");

    return 0;
}