Prompt *****************************************************
Prompt ****************Starting Installation****************
Prompt *****************************************************
prompt ************Enter inputs in CAPS letters*************
prompt **Input Source Database Details only on this screen**
prompt
prompt
accept INSTANCE       char prompt 'Enter TNSNAMES entry to connect to database : '
accept SYSTEM_PASS	  char prompt 'Enter SYSTEM Password : ' hide
accept APP_USER		  char prompt 'Enter App User Schema Name: '
accept APP_PASSWORD	  char prompt 'Enter App User Schema Password: 'hide
accept APP_TBS_NAME	  char prompt 'Enter Tablespace Name : '
accept APP_DF_PATH	  char prompt 'Enter Data File Path : '

DEFINE INSTANCE = &INSTANCE;
DEFINE SYSTEM_PASS = &SYSTEM_PASS;
DEFINE APP_USER = &APP_USER;
DEFINE APP_PASSWORD = &APP_PASSWORD;
DEFINE APP_TBS_NAME = &APP_TBS_NAME;
DEFINE APP_DF_PATH = &APP_DF_PATH;