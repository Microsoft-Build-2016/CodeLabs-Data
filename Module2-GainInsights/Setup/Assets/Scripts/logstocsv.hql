INSERT OVERWRITE TABLE OutputTable Partition (year=${hiveconf:Year}, month=${hiveconf:Month}, day=${hiveconf:Day})
SELECT CAST(get_json_object(jsonentry, "$.productid") as BIGINT) as productid,
         get_json_object(jsonentry, "$.title") as title,
         get_json_object(jsonentry, "$.category") as category,
         get_json_object(jsonentry, "$.type") as type,
         CAST(get_json_object(jsonentry, "$.total") as BIGINT) as total
FROM LogsRaw