
from lib_das.DAS220_240 import telnet_das220_240
import time
import socket
import numpy
from umodbus import conf
from umodbus.client import tcp


def SCPI_TEST():
    print("-------------------------------------------------------")
    print("  SCPI interface test : ")
    print("    - Setting channel A1 to Thermocouple K range +/-30°C")
    print("    - Set start trigger to 25° on A1 (Positive edge)")
    print("    - Set stop trigger to 23° on A1 (Negative edge)")
    print("    - Set acquisition sammpling to 500ms")
    print("    - Launch the recording")
    print("    - Getting the value and check the record state each 5s until acquisition ended")
    # Connexion to the DAS220 or DAS240
    mydas = telnet_das220_240(DAS_IP)
    #Setting the channal A1 to thermocouple K +/-30°C range 
    mydas.set_chanmes('A1','THErmo K,COMP,CEL',60,0,0)
    mydas.sendFrame("CHAN A1; CHANNELSAMPPERIOD 100")
    #Enable the Channel A1
    mydas.set_chan_valid('A1')
    #Getting the channel A1 value 
    mydas.get_chanmes('A1')
    #Setting start trigger to > 25°C
    mydas.sendFrame("THRES  S1, ON, 25;")
    time.sleep(0.5)
    mydas.sendFrame("START:TRIG;:TRIG:TYP EDGE;")
    time.sleep(0.5)
    mydas.sendFrame(":TRIG:CHAN A1,S1,POS;")
    time.sleep(0.5)
    #Setting stop trigger to < 22°C
    mydas.sendFrame("THRES  S2, ON, 23;")
    time.sleep(0.5)
    mydas.sendFrame("STOP:TRIG;:TRIG:TYP EDGE;")
    time.sleep(0.5)
    mydas.sendFrame(":TRIG:CHAN A1,S2,NEG;")
    time.sleep(0.5)
    #Setting sampling frequency to 500ms
    mydas.sendFrame("MEMSPEED 500,MIL;")
    time.sleep(1)
    #Launch recording
    mydas.sendFrame("RECORD ON;")   
    #close the connection to the DAS
    time.sleep(1)
    for i in range(1,10):
        res = mydas.sendQuery("RECORD ?")
        print(res)
        if res.endswith("OFF\n"):
            break
            
        time.sleep(5)
        mydas.get_chanmes('A1')
        
    mydas.close();


def modbus_test():
    # Enable values to be signed (default is False).
    print("-------------------------------------------------------")
    print("  Modbus interface test : ")
    print("    -Getting channel A1 value from modbus interface ")
    conf.SIGNED_VALUES = True
    sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    sock.connect((DAS_IP, 502))
   

    # Create modbus message to read A1 channel and send it
    message=tcp.read_input_registers(slave_id=1, starting_address=0x8, quantity=2)
    res = tcp.send_message(message, sock)

    #Convert the answer to float  
    res.reverse()
    tmp=numpy.array(res,numpy.int16)
    tmp.dtype = numpy.float32
    
    print("Channel A1 =" + str(tmp[0]) + "°C")
    # Close the connexion 
    sock.close()

'''
   main script
'''
#Update the DAS IP adress here
DAS_IP = "192.168.0.72"

print("********* Starting DAS220/240 Test Script **************")
SCPI_TEST()
modbus_test()
print("**************** End of DAS220/240 Test Script *******************")

