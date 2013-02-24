# Project Asimov Server
# shane tully (shane@shanetully.com)
# shanetully.com

CC=gcc
LANG=c

PROJ_NAME = asimov-server
VERSION = "\"1.0.0\""

BINARY = $(PROJ_NAME)

INSTALL_DIR = /usr/sbin/

SOURCES = $(wildcard src/server/*.c)
OBJECTS = $(SOURCES:.$(LANG)=.o)

SRC_INCLUDE_DIRS = -Ilibs/libbiscuit/include
LIB_INCLUDE_DIRS = -Llibs/libbiscuit/bin/static

LIBS = -lbiscuit

CFLAGS = -std=c99 -Wall -Wextra $(SRC_INCLUDE_DIRS) -DVERSION=$(VERSION)

DEFINES = -D_POSIX_SOURCE

DEBUG ?= 1
ifeq ($(DEBUG), 1)
	CFLAGS += -ggdb -DDEBUG
else
	CFLAGS += -O2
endif

.PHONY = all install remove clean

all: $(OBJECTS)
	$(CC) $(DEFINES) -o bin/$(BINARY) $^ $(LIB_INCLUDE_DIRS) $(LIBS)

install:
	cp bin/$(BINARY) $(INSTALL_DIR)$(BINARY)

remove:
	rm $(INSTALL_DIR)$(BINARY)

.$(LANG).o:
	$(CC) $(CFLAGS) $(DEFINES) -c $< -o $@

clean:
	rm -f $(OBJECTS)
	rm -f bin/$(BINARY)
