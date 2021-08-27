Sefram DAS240 Modbus TCP example for LabVIEW

New Sefram DAS240 DataLogger is a Modbus TCP server. It means all acquired data are shared via Modbus.

That LabVIEW project example is based on Modbus library available on VIPM (VI Package Manager)
https://decibel.ni.com/content/docs/DOC-30140
For more informations about Modbus
https://fr.wikipedia.org/wiki/Modbus

DAS240 Modbus mapping:

Input registers (0x04), from address 0x08 to 0x1A0 (204 channels as float (4 bytes))

Input A1 from address 0x08 to 0x09
At address 0x08 : -16460 (16 bits)
At address 0x09 : 28160 (16 bits)
Value in float : -1.40961 (32 bits)

Discret inputs (0x02), from addresse 0x08 to 0x14 (each one as bit)
Digital input DI1 at address 0x08

To control DAS240, please refer to Programing Interface Manual via TCP (port 23) available in March 2017.

For more informations, please call or contact us : https://www.sefram.com/en/contact-us.html
