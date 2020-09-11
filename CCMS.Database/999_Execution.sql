@000_Define.sql

Prompt *****************************************
Prompt ***********Connecting to SYSTEM**********
Prompt *****************************************
prompt

connect SYSTEM/&SYSTEM_PASS.@&INSTANCE.
prompt Press Enter if connected successfully;
pause  Else Close Window and Re-Execute with appropriate Username/Password...

COLUMN SPOOL_FILE_NAME NEW_VALUE PI_SPOOL_FILE_NAME_1

SELECT 'Logs\Installation_Log'|| '_' || TO_CHAR(SYSDATE,'YYYYMMDD_HH24MISS') || '.LOG' SPOOL_FILE_NAME FROM V$INSTANCE;

SPOOL &PI_SPOOL_FILE_NAME_1

Prompt ***********Connected to SYSTEM***********
Prompt ***Creating new App User &APP_USER ***
Prompt *****************************************
prompt

@001_CreateUser.sql

Prompt *****************************************
Prompt *********Connecting to &APP_USER*********
Prompt *****************************************
Prompt  
CONN &APP_USER/&APP_PASSWORD@&INSTANCE.

@002_CreateConfigTables.sql
@003_CreateAppTables.sql
@004_P_AUDIT_LOG.sql
@005_Triggers.sql
@006_AddFactoryData.sql
@007_PKG_LEAVE_MGMT_SPEC.sql
@008_PKG_LEAVE_MGMT_BODY.sql
@009_Views.sql
@010_Jobs.sql

SPOOL OFF
/