sc create DBServer binPath= "C:\Users\xun73\Desktop\XL-DBServer\DBServer\bin\XL-DBServer\Release190628\DBServer.exe"

sc Start DBServer

sc config DBServer start= auto

sc config DBServer DisplayName= "XL-LabServer"

sc description DBServer @��������ʵ������Ŀ-���ݿ�-������

sc failure DBServer reset= 30 actions= restart/60000

pause