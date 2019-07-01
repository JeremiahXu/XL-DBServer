sc create DBServer binPath= "C:\Users\xun73\Desktop\XL-DBServer\DBServer\bin\XL-DBServer\Release190628\DBServer.exe"

sc Start DBServer

sc config DBServer start= auto

sc config DBServer DisplayName= "XL-LabServer"

sc description DBServer @用于星龙实验室项目-数据库-服务器

sc failure DBServer reset= 30 actions= restart/60000

pause