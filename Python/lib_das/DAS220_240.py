'''
    Lib de pilotage des gbf BK serie 4050 (4052,4053,4054,4055)
'''

import time
import sys
import telnetlib
import string

class telnet_das220_240(object):
    '''
    classdocs
    
    '''
    # Timeout on frame receive
    TIMEOUT = 1

    DEST = '192.168.0.115'
    PORT = 23
    tn = 0
    PR_OUT_CMD = 0
    PR_OUT = 1
    def __init__(self,ip_addr):
        '''
        Constructor
        '''
        self.DEST = ip_addr
        self.tn = telnetlib.Telnet(self.DEST, self.PORT)
        print("Connected with : ")
        self.get_id()   
        
    def close(self):
        self.tn.close()

    ##################### Low level functions #####################

    def print_dbg(self,text):
        if self.PR_OUT_CMD == 1:
            print(text)
            
    def print(self,text):
        if self.PR_OUT == 1:
            print(text)

    # Remove end of line chars to print
    def extractCmd(self,cmd):
        cmd = cmd.replace("\n", "")
        cmd = cmd.replace("\r", "")
        return cmd

    # Send a frame and wait for response
    def sendQuery(self,cmd):
        cmd = cmd + "\n"
        self.print_dbg(">> Send : " + self.extractCmd(cmd))
        self.tn.write(cmd.encode('ascii'))
        res = self.tn.read_until(b'\n', self.TIMEOUT).decode('ascii','ignore')
        if len(res) == 0:
            print("Send frame Timeout")
            time.sleep(10)
        else:
            self.print_dbg("<< Rcv  : " + res)
        return res
    
    # Send a frame and not wait for response
    def sendFrame(self,cmd):
        cmd = cmd + "\n"
        self.print_dbg(">> Send : " + self.extractCmd(cmd))
        self.tn.write(cmd.encode('ascii'))
       

    ##################### High level functions #####################

    def get_id(self):
          print(self.sendQuery("*IDN ?;"))
 #Chan_number shall be A1/A2...K1/FA1...
    def set_chan_valid(self,chan_number) :
            self.sendFrame("VALID "+ chan_number + ",ON;")
            
    def all_chan_off(self,) :
            self.sendFrame("VALID ALL,OFF;")

    '''
        This function activate automaticaly the channel and set is
        type of mesure and is 
        channel is channel number A1/A2.....,
        mes shall one of : VOLtage, SHUNT, PT100,PT1000,THErmo
        RESistance, FREQ, PWM, COUNTer
        
    '''
    def set_chanmes(self,channel,mes,cal,zero,pos):
            self.sendFrame("CHAN "+ channel + ";")
            self.sendFrame("TYPE:" + mes + ";")
            self.sendFrame("RANGE " + str(cal) + "," + str(zero) +","+ str(pos) + ";")
            self.sendFrame("VALID "+ channel + ",ON;")

    #lecture de la valeur d'une voie       
    def get_chanmes(self,channel):
        #self.sendFrame("CHAN "+ channel + ";")
        res = self.sendQuery("CHAN "+ channel + ";"+"CHAN ?;")
        self.print(res)
        res =  res.split(";")
        num=""
        mul = 1
        for i in res[1]:
            if i == "°":
               #temperature measure break the loop
                break
            elif i.isdecimal():
                num = num + i
            elif i == "." :
                    num = num + "."
           #If the multiplier is found end of the parsing
            elif (i == "m" and mul != 0):
                mul = 0.001
                break
            elif ((i == "K" or i == 'k')and mul != 0):
                mul = 1000
                break
            elif (i == "M" and mul != 0):
                mul = 1000000
                break

        return (float(num)*mul)
    
            
  
