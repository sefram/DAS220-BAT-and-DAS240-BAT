
from lib_das.DAS220_240 import telnet_das220_240
import time
import socket
import numpy



def REMOTE_SAMPLING_EXAMPLE():
    print("-------------------------------------------------------")
    print("  Remote Sammpling example : ")
    print("    - Getting NUMBER_OF_SAMPLE sample of CHANNEL_LIST channel each SAMP_TIME")
    print("    - Save the result into a CSV files")
    launch_time = time.strftime("%d_%b_%Y_%H_%M_%S")
    # Connexion to the DAS220 or DAS240
    mydas = telnet_das220_240(DAS_IP)
    #Setting the channel in the list to thermocouple K +/-30°C range and enable it
    mydas.all_chan_off()
    for chan in CHANNEL_LIST:
        mydas.set_chanmes(chan,'THErmo K,COMP,CEL',60,0,0)

    #Disabling command printing 
    mydas.PR_OUT_CMD = 0
    mydas.PR_OUT = 1
    tab_res  = []
    time_res = []
    for chan in CHANNEL_LIST:
        tab_res.append([]) #create one list by channel

    #Make the measure
    for i in range(0,NUMBER_OF_SAMPLE):
        print("sample " + str(i))
        time.sleep(SAMP_TIME)
        time_res.append(time.strftime("%H:%M:%S"))
        for ichan in range(0,len(CHANNEL_LIST)):
            tab_res[ichan].append(mydas.get_chanmes(CHANNEL_LIST[ichan]))
            
    mydas.close();        
    
    #Create CSV files
    f_name = "Remote_sampling_test_"+ launch_time + ".csv"
    flog = open(f_name,"a")
    #Create the first line
    first_string = "Sample Time" + CSV_SEPARATOR
    for ichan in range(0,len(CHANNEL_LIST)):
        first_string += "CHAN_" + CHANNEL_LIST[ichan] + CSV_SEPARATOR
        
    flog.write(first_string + "\n")

    #write each result in the csv files
    for ires in range(1, NUMBER_OF_SAMPLE):
        line = time_res[ires] + CSV_SEPARATOR
        for ichan in range(0,len(CHANNEL_LIST)):
            line += str(tab_res[ichan][ires]).replace('.',CSV_DECIMAL_DELIMITER) + CSV_SEPARATOR
        flog.write(line + "\n")
    flog.close()
        
    print("Test file Name : " + f_name )


'''
   main script
'''
#Update the DAS IP adress here
DAS_IP = "192.168.0.72"
SAMP_TIME = 1   #time in seconds 
NUMBER_OF_SAMPLE = 10
CHANNEL_LIST = ["A1","A2"] 
CSV_SEPARATOR = ';'
CSV_DECIMAL_DELIMITER  = ',' #update following your country

print("********* Starting DAS220/240 Remote sampling Script **************")
REMOTE_SAMPLING_EXAMPLE()
print("**************** End of DAS220/240 Test Script *******************")

