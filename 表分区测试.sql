insert into HistoryData (ProbeID,CheckTime,GasValue,MachineID)values(1,'20240202',1,1)
insert into HistoryData (ProbeID,CheckTime,GasValue,MachineID)values(1,'20240310',1,1)
insert into HistoryData (ProbeID,CheckTime,GasValue,MachineID)values(1,'20240311',1,1)
insert into HistoryData (ProbeID,CheckTime,GasValue,MachineID)values(1,'20240318',1,1)
insert into HistoryData (ProbeID,CheckTime,GasValue,MachineID)values(1,'20240320',1,1)
insert into HistoryData (ProbeID,CheckTime,GasValue,MachineID)values(1,'20240328',1,1)
insert into HistoryData (ProbeID,CheckTime,GasValue,MachineID)values(1,'20240428',1,1)


select * from HistoryData where $PARTITION.fenquhanshu(CheckTime)=1
select * from HistoryData where $PARTITION.fenquhanshu(CheckTime)=2
select * from HistoryData where $PARTITION.fenquhanshu(CheckTime)=3
select * from HistoryData where $PARTITION.fenquhanshu(CheckTime)=4

truncate table dbo.HistoryData