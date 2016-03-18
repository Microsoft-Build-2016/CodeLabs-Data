DROP TABLE IF EXISTS LogsRaw;
CREATE EXTERNAL TABLE LogsRaw (jsonentry string) 
STORED AS TEXTFILE LOCATION "${hiveconf:inputjson}";

DROP TABLE IF EXISTS OutputTable;
CREATE EXTERNAL TABLE OutputTable
(
    productid       int,
    title           string,
    category        string,
    type            string,
    total            int
)
ROW FORMAT DELIMITED FIELDS TERMINATED BY ','
LINES TERMINATED BY '10'
STORED AS TEXTFILE LOCATION '${hiveconf:outputcsv}'
TBLPROPERTIES("skip.header.line.count"="1");

INSERT OVERWRITE TABLE OutputTable
SELECT CAST(get_json_object(jsonentry, "$.productid") as BIGINT) as productid,
         get_json_object(jsonentry, "$.title") as title,
         get_json_object(jsonentry, "$.category") as category,
         get_json_object(jsonentry, "$.type") as type,
         CAST(get_json_object(jsonentry, "$.total") as BIGINT) as total
FROM LogsRaw