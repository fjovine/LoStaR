#include <signal.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <sys/io.h>
#include <sys/time.h>
#include <unistd.h>

#define TRANSITION_BUFFER_SIZE (100000)
#define TRUE (1)
#define FALSE (0)

typedef struct transition
{
	int sec;
	int usec;
	int new_state;
} TRANSITION, * PTRANSITION;

typedef int BOOL;

TRANSITION transitions[TRANSITION_BUFFER_SIZE];

// This variable stores the real number of transitions stored in the buffer
// if this value is less TRANSITION_BUFFER_SIZE, then all the samples
// are stored in the buffer, otherwise only the last TRANSITION_BUFFER_SIZE
// is available
int transition_count = 0;

// Predicate that becomes true when stop is requested with control C
BOOL stop = FALSE;

// Base address of the chosen parallel port
int inport;

void handler_sigint(int dummy)
	//
	// Function called when control C is pressed.
	//
{
	stop = TRUE;
}

void sample()
{
	struct timeval sampletime;
	time_t start_second;
	
	// trap ctrl-c
	signal (SIGINT, &handler_sigint);
	
	// get the starting time
	gettimeofday(&sampletime,0);
	start_second = sampletime.tv_sec;
	int last_state = inb(inport);	
	puts("Collecting samples; hit CTRL-C to stop...");
	while (1)
	{
		int new_state = inb(inport);
		if (new_state != last_state) 
		{
			gettimeofday(&sampletime,0);
			PTRANSITION current_transition = &transitions[transition_count % TRANSITION_BUFFER_SIZE];
			current_transition->sec = (int)(sampletime.tv_sec - start_second);
			current_transition->usec = (int)sampletime.tv_usec;
			current_transition->new_state = new_state;
			transition_count ++;
			last_state = new_state;
		}
		if (stop) 
		{
			break;			
		}
	}
	puts("Stop");
}

int main (int argc, char ** argv)
{
	int c;
	int port_address = 0xd030;
	while ((c=getopt(argc, argv, "p:")) != -1) 
	{
		switch (c) {
		case 'p':
			port_address = atoi(optarg);
			break;
		case '?':
			puts("Synposys: capture [-p n]");
			puts("Where n is the base port address");
			return 1;
		}
	}
	
	// Get permission on port.
	if( ioperm(port_address,3,1) )
	{
		printf("Could not ioperm(0x%04x, 3,1)... Did you run as root?\n", inport);
		return 1;
	}
	
	inport = port_address + 1;	
	sample();
	printf ("Seen %d transitions\n", transition_count);
	if (transition_count > TRANSITION_BUFFER_SIZE)
	{
		printf("Overflow, saving %d transitions\n", TRANSITION_BUFFER_SIZE);
	}
	else
	{
		printf("Saving %d transitions \n", transition_count);
	}
	
	FILE * outf = fopen("capture.dat", "wb");
	fwrite(&transition_count, sizeof(int), 1, outf);
	int transition_buffer_size = TRANSITION_BUFFER_SIZE;
	fwrite(&transition_buffer_size , sizeof(int),1, outf);
	fwrite(transitions, sizeof(char), sizeof(transitions), outf);
	fclose(outf);
}

