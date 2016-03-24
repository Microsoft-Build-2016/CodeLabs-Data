ALTER TABLE LogsRaw ADD IF NOT EXISTS PARTITION (year=${hiveconf:Year}, month=${hiveconf:Month}, day=${hiveconf:Day}) LOCATION 'wasb://rawdata@datalabstr.blob.core.windows.net/logs/${hiveconf:Year}/${hiveconf:Month}/${hiveconf:Day}';

ALTER TABLE OutputTable ADD IF NOT EXISTS PARTITION (year=${hiveconf:Year}, month=${hiveconf:Month}, day=${hiveconf:Day}) LOCATION 'wasb://processeddata@datalabstr.blob.core.windows.net/logs/${hiveconf:Year}/${hiveconf:Month}/${hiveconf:Day}';
