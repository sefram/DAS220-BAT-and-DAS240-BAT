
from lib_das.DAS220_240 import telnet_das220_240
import tkinter as tk
import time
import socket
import numpy
import threading

                                     

def REMOTE_SAMPLING_MEAN():

    global mydas
    global lbl;
    global Running

    while(True):
        #Make measurement and mean
        res=0
        for ichan in range(0,20):
            res += mydas.get_chanmes("A"+str(ichan+1))
            if(Running == False):
                break;

        if(Running == False):
            break;

        res=res/20

        #display mean
        lbl.configure(text=str(time.strftime("%d_%b_%Y_%H:%M:%S"))+ " => Mean = " + '% .2f' %res + " °C" )



def start_top_clicked():

    global Running
    global mydas
    global thread
    
    if Running == False:
        Running = True
        btn.configure(text="Stop")

        thread  = threading.Thread(target=REMOTE_SAMPLING_MEAN)
        thread.start();
        
    else:
        Running = False
        
        btn.configure(text="Start")

            

'''
   main script
'''
#Update the DAS IP adress here
DAS_IP = "192.168.0.103"
WAIT_TIME = 1   #time in seconds 
Running=False

window = tk.Tk()
    #initialize window
window.geometry('800x150')
#window.columnconfigure(0, weight=100);
window.title("DAS 224-240 MEAN graphical example")

btn = tk.Button(window, text="Start", font=("Arial Bold", 20), command=start_top_clicked, width=50)
btn.grid(column=0, row=0, sticky='nsew')


lbl = tk.Label(window, text="Mean DAS220-240", font=("Arial Bold", 20), bg="orange", width=50)
lbl.grid(column=0, row=1, sticky='nesw')

# Connexion to the DAS220 or DAS240
mydas = telnet_das220_240(DAS_IP)

#Disabling command printing 
mydas.PR_OUT_CMD = 0
mydas.PR_OUT = 0

window.mainloop()

thread.join()

mydas.sendFrame("*LOC")
mydas.close()

print("********* Starting DAS220/240 Remote sampling mean Script **************")
##REMOTE_SAMPLING_MEAN_EXAMPLE()
print("**************** End of DAS220/240 Test Script *******************")




