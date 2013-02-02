#include "server.h"

int main(int argc, char **argv) {

    /*void *handle = dlopen("libbiscuit.so", RTLD_LAZY);
    if(handle == NULL) {
        fprintf(stderr, "Error loading libbiscuit.\n");
        return 1;
    }*/

    biscInit("/dev/ttyUSB0");
    biscTurnOnAdvanceLed();

    return 0;
}