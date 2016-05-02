DROP TABLE IF EXISTS LogsRaw;
CREATE EXTERNAL TABLE LogsRaw (jsonentry string) 
PARTITIONED BY (year int, month int, day int)
STORED AS TEXTFILE LOCATION 'wasb://partsunlimited@<StorageAccountName>.blob.core.windows.net/logs';

DROP TABLE IF EXISTS OutputTable;
CREATE EXTERNAL TABLE OutputTable (
productid int,
title string,
category string,
type string,
totalClicked int
) PARTITIONED BY (year int, month int, day int) 
ROW FORMAT DELIMITED FIELDS TERMINATED BY ',' LINES TERMINATED BY '\n'
STORED AS TEXTFILE LOCATION 'wasb://processeddata@<StorageAccountName>.blob.core.windows.net/logs';
