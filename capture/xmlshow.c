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
	FILE * inf = fopen("capture.dat", "rb");
	if (inf == NULL) 
	{
		perror("The input file cannot be opened");
		exit(-1);
	}
	fread(&transition_count, sizeof(int), 1, inf);
	int transition_buffer_size = TRANSITION_BUFFER_SIZE;
	fread(&transition_buffer_size , sizeof(int),1, inf);
	fread(transitions, sizeof(char), sizeof(transitions), inf);
	fclose(inf);
	
	FILE * xf = fopen("capture.xml", "wt");
	if (xf == NULL) 
	{
		perror("The output file cannot be opened");
		exit(-1);
	}
	fprintf(xf, "<?xml version='1.0' encoding='utf-8'?>\n");
	fprintf(xf, "<Capture  xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema'>\n");
	fprintf(xf, "\t<TransitionCount>%d</TransitionCount>\n", transition_count);
	fprintf(xf ,"\t<BufferSize>%d</BufferSize>\n", TRANSITION_BUFFER_SIZE);
	fprintf(xf, "\t<TransitionContainer>\n");
	
	int initial = 0;
	int final = transition_count;
	
	if (transition_count > TRANSITION_BUFFER_SIZE)
	{
		initial = transition_count % TRANSITION_BUFFER_SIZE +1;
		final = TRANSITION_BUFFER_SIZE;
	}
	for (int i =0; i<final; i++)
	{		
		PTRANSITION ptransition=&transitions[initial % TRANSITION_BUFFER_SIZE];
		double sec_time = ptransition->sec + ptransition->usec / 1E6; 		
		fprintf(xf, "\t\t<Transition time='%.6f' state='%d'/>\n",
			sec_time,
			ptransition->new_state & 0xFF
			);
		initial ++;
	}
	fprintf(xf, "\t</TransitionContainer>\n");
	fprintf(xf, "</Capture>\n");
	fclose(xf);	
}
