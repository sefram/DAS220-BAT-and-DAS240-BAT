
from lib_das.DAS220_240 import telnet_das220_240
import time
import socket
import numpy



def REMOTE_SAMPLING_MEAN_EXAMPLE():
    print("-------------------------------------------------------")
    print("  Remote Sammpling example : ")
    print("    - Getting MEAN of the first 20 channels each SAMP_TIME")
    # Connexion to the DAS220 or DAS240
    mydas = telnet_das220_240(DAS_IP)
    # Setting the channel in the list to thermocouple K +/-30°C range and enable it
    # mydas.all_chan_off()
    # for chan in CHANNEL_LIST:
        # mydas.set_chanmes(chan,'THErmo K,COMP,CEL',60,0,0)


    #Disabling command printing 
    mydas.PR_OUT_CMD = 0
    mydas.PR_OUT = 0

    #Make measurement and mean
    try:
        while True:
            res=0
            for ichan in range(0,20):
                res += mydas.get_chanmes("A"+str(ichan+1))
            res=res/20
            print(str(time.strftime("%d_%b_%Y_%H:%M:%S"))+ " => Mean = " + '% .2f' %res + " °C" )
            time.sleep(WAIT_TIME)
    except KeyboardInterrupt:
        print("**************** Interrupt *******************")

    mydas.sendQuery("*LOC")
    mydas.close()
    print("**************** close *******************")
            

'''
   main script
'''
#Update the DAS IP adress here
DAS_IP = "192.168.0.103"
WAIT_TIME = 1   #time in seconds 

print("********* Starting DAS220/240 Remote sampling mean Script **************")
REMOTE_SAMPLING_MEAN_EXAMPLE()
print("**************** End of DAS220/240 Test Script *******************")

