#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <unistd.h>

//
// Returns the base address of the passed parallel port
// -1 if no parallel port is found
//
int find_parallel_port(int num_port)
{
	if (num_port < 0 || num_port>9) 
	{
		perror("The num port must be 0..9");
		return -1;
	}
        int baseport;
        FILE *fin = fopen("/proc/ioports","r");
        if( !fin )
        {
                perror("Failed to open /proc/ioports");
                return -1;
        }

	char * port = strdup("parport0");
	port[7] = (char)('0' + num_port);
        while( !feof(fin) )
        {
                char buffer[128];

                buffer[0] = '\0';
                fgets(buffer,127,fin);
                if( buffer[0] == '\0' )
                        break;

                if( strstr(buffer, port) )
                {
                        sscanf(buffer, " %4x", &baseport);

                        fclose(fin);
                        return baseport;
                }
        }

        fclose(fin);
        return -1; // failure
}

int main (int argc, char ** argv)
{
	int c;
	int pport = 0;
	while ((c=getopt(argc, argv, "p:")) != -1) 
	{
		switch (c) {
		case 'p':
			pport = atoi(optarg);
			break;
		case '?':
			puts("Synposys: port_address [-p n]");
			puts("Where n is the port number");
			return 1;
		}
	}
	int port_address = find_parallel_port(pport); 
	if (port_address == -1)
	{
		printf("The parallel port %d does not exist\n",
			pport);
	}
	else
	{
		printf("The parallel port %d base address is 0x%04X\n",
			pport, 
			port_address
			);
	}
}

