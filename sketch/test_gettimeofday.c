#include <stdio.h>
#include <sys/time.h>
//
// This program checks the usage of the long value in the gettimeofday linux system call.
// It returns 999999 and this means that the tv_usec field of timeval struct will never
// be higher than 10^6-1
//
#define TRUE (-1)
#define FALSE (0)
int main (int argc, char ** argv)
{
	struct timeval sampletime;
	gettimeofday(&sampletime,0);
	time_t start = sampletime.tv_sec;
	suseconds_t maxmicroseconds = 0;
	
	while (TRUE)
	{
		gettimeofday(&sampletime,0);
		
		suseconds_t microseconds = sampletime.tv_usec;
		if (microseconds > maxmicroseconds)
		{
			maxmicroseconds = microseconds;
		}
		
		if (sampletime.tv_sec - start > 5)
		{
			break;
		}
	}
	printf("maxmicroseconds = %ld\n", maxmicroseconds);
}

