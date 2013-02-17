#include "server.h"

int main(void) {
    printf("Connecting to device...\n");
    if(biscInit("/dev/ttyUSB0") == BISC_ERR) {
        fprintf(stderr, "Error connecting to device.\n");
        return 1;
    }

    /*printf("Changing to full mode...\n");
    if(biscChangeMode(BISC_MODE_FULL) == BISC_ERR) {
        fprintf(stderr, "Error changing to full mode.\n");
        return 1;
    }*/

    biscTurnOnPlayLed();
    biscTurnOnAdvanceLed();

    biscTimedDriveStraight(BISC_DRIVE_FORWARD_FULL_SPEED, 2000);

    biscBeep();
    
    biscTimedDriveStraight(BISC_DRIVE_BACKWARD_FULL_SPEED, 2000);

    biscBeep();

    biscSpinDegrees(BISC_DRIVE_FORWARD_FULL_SPEED, BISC_SPIN_CCW, 180);
    biscSpinDegrees(BISC_DRIVE_FORWARD_FULL_SPEED, BISC_SPIN_CW, -180);

    biscPlayFireflies();

    printf("Disconnecting from device...\n");
    biscDisconnect();

    printf("All done!\n");

    return 0;
}