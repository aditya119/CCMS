@000_Define.sql

Prompt *****************************************
Prompt ***********Connecting to SYSTEM**********
Prompt *****************************************
prompt

connect SYSTEM/&SYSTEM_PASS.@&INSTANCE.
prompt Press Enter if connected successfully;
pause  Else Close Window and Re-Execute with appropriate Username/Password...

COLUMN SPOOL_FILE_NAME NEW_VALUE PI_SPOOL_FILE_NAME_1

SELECT 'Logs\Installation_Log_' || TO_CHAR(SYSDATE,'YYYYMMDD_HH24MISS') || '.LOG' SPOOL_FILE_NAME FROM V$INSTANCE;

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
@004_Triggers.sql
@005_AuditLog.sql
@006_AuditTriggers.sql
@007_AddFactoryData.sql
@008_PkgAuth.sql
@009_PkgAppUsers.sql
@010_PkgRoles.sql
@011_PkgPlatforms.sql
@012_PkgLawyers.sql
@013_PkgCourts.sql
@014_PkgCaseTypes.sql
@015_PkgLocations.sql
@016_PkgProceedingDecisions.sql
@017_PkgActorTypes.sql

SPOOL OFF
/