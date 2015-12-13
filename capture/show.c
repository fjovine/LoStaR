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

int transition_count = 0;

char * tobin(char * buf, int start_bit, int end_bit, int state)
{
	int i, n;
	int msb = start_bit > end_bit ? start_bit : end_bit;
	int lsb = start_bit > end_bit ? end_bit : start_bit;
	
	for (i=0, n = msb; n>=lsb; n--, i++)
	{
		int mask = 0x1 << n;
		if (state & mask)
		{
			buf[i] = '1';
		}
		else
		{
			buf[i] = '-';
		}
	}
	buf[i]='\0';
	return buf;
}

double last_transition;

void show_transition(PTRANSITION ptransition, BOOL first)
{
	char buf[10];
	double t = ptransition->sec + ptransition->usec / 1E6;
	double delta;
	if (! first)
	{
		delta = t - last_transition;
		printf("%12.6f\t%s\t%12.6f\n", t, tobin(buf, 0, 7, ptransition->new_state), delta);
	}
	else
	{
		printf("%12.6f\t%s\n", t, tobin(buf, 0, 7, ptransition->new_state));
	}
	last_transition = t;
}

int main(int argc, char ** argv)
{
	FILE * outf = fopen("capture.dat", "rb");
	fread(&transition_count, sizeof(int), 1, outf);
	int transition_buffer_size = TRANSITION_BUFFER_SIZE;
	fread(&transition_buffer_size , sizeof(int),1, outf);
	fread(transitions, sizeof(char), sizeof(transitions), outf);
	fclose(outf);
	
	printf("# transizioni\t%d\n", transition_count);
	
	int initial = 0;
	int final = transition_count;
	
	if (transition_count > TRANSITION_BUFFER_SIZE)
	{
		initial = transition_count % TRANSITION_BUFFER_SIZE +1;
		final = TRANSITION_BUFFER_SIZE;
	}
	for (int i =0; i<final; i++) {
		show_transition(&transitions[initial % TRANSITION_BUFFER_SIZE], i==0);
		initial ++;
	}	
}
