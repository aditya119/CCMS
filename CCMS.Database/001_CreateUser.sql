-------------------------------------------------------------------------
-- Create new APP_USER
-------------------------------------------------------------------------

CREATE TABLESPACE CCMSDB_TB
DATAFILE '&APP_DF_PATH' 
SIZE 4096M AUTOEXTEND ON NEXT 10M 
MAXSIZE UNLIMITED
/

CREATE USER CCMSDB IDENTIFIED BY &APP_PASSWORD
DEFAULT TABLESPACE CCMSDB_TB
/

GRANT RESOURCE,CONNECT        TO CCMSDB;
GRANT CREATE SESSION          TO CCMSDB;
GRANT UNLIMITED TABLESPACE    TO CCMSDB;
GRANT CREATE TABLE            TO CCMSDB;
GRANT CREATE ANY TABLE        TO CCMSDB;
GRANT CREATE ANY INDEX        TO CCMSDB;
GRANT CREATE SYNONYM          TO CCMSDB;
GRANT CREATE VIEW             TO CCMSDB;
GRANT CREATE SEQUENCE         TO CCMSDB;
GRANT CREATE PROCEDURE        TO CCMSDB;
GRANT CREATE JOB              TO CCMSDB;
GRANT CREATE ANY JOB          TO CCMSDB;
/