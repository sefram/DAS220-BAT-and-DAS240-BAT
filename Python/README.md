------------------------------------------------------------------------
                    DAS220/240 Python Test Script
------------------------------------------------------------------------

To run this example : 
    - Install Python 3.x (download from web site : https://www.python.org/downloads/) (click "ADD Python 3.X to Path" during the installation)
    - Install the script dependancy:
        - for windows open Windows PowerShell 
        - For linux open a shell
        - Run the following command :
                pip install umodbus numpy
    - Connect your DAS220/240 on a network and find is IP adress
    - Open the files ./SCPI_MODBUS_EXAMPLE_script.py
    - Update the line "DAS_IP = '192.168.X.X'" with the DAS IP adresse
    - Run the Script:
        - On Windows :
            - Right click on the files SCPI_MODBUS_EXAMPLE_script.py -> "Edit With IDLE"
            - Type F5 Key (Run->Run Module)